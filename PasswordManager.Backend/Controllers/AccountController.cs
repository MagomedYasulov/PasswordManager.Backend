using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using PasswordManager.Backend.Data;
using PasswordManager.Backend.Data.Entities;
using PasswordManager.Backend.DTOs;
using PasswordManager.Backend.Extententions;
using PasswordManager.Backend.Models;
using PasswordManager.Backend.Services;
using PasswordManager.Backend.ViewModels;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace PasswordManager.Backend.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(
            SignInManager<User> signInManager, 
            UserManager<User> userManager,
            ITokenService tokenService,
            IConfiguration configuration,
            IRepository repository, 
            IMapper mapper, 
            IStringLocalizer<BaseController> localizer) : base(repository, localizer, mapper)

        {
            _configuration = configuration;
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Логин
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Возвращет обьект с пользователем, access и refresh токенами</returns>
        [HttpPost("/api/v1/login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginViewModel model)
        {
            var managedUser = await _userManager.FindByNameAsync(model.Login);
            if (managedUser == null)
                return Unauthorized();

            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, model.Password);
            if (!isPasswordValid)
                return Unauthorized();

            var user = _repository.GetOne<User>(u => u.UserName == model.Login);
            if (user is null)
                return Unauthorized();

            var userDTO = _mapper.Map<UserDTO>(user);
            var serializeSettings = HttpContext.RequestServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value.SerializerSettings;
            var identity = userDTO.GetIdentity(serializeSettings);
           
            var accessToken = _tokenService.GenerateAccessToken(identity);
            var token = _repository.GetOne<Token>(rt => rt.UserId == user.Id && rt.DeviceId == model.DeviceId);
            var accessTokenJti = identity.FindFirst(JwtRegisteredClaimNames.Jti)!.Value.ToString();
            if (token == null)
            {
                token = new Token()
                {
                    AccessTokenJti = accessTokenJti,
                    DeviceId = model.DeviceId,
                    ExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetSection("JwtToken:RefreshTokenLifeTime").Get<int>()),
                    UserId = user.Id,
                    RefreshToken = _tokenService.GenerateRefreshToken(),
                };
                _repository.Create(token);
            }
            else
            {
                token.AccessTokenJti = accessTokenJti;
                token.PreviousRefreshToken = null; //что бы после логина нельзя было выкинуть пользователя 
                token.RefreshToken = _tokenService.GenerateRefreshToken();
                token.ExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetSection("JwtToken:RefreshTokenLifeTime").Get<int>());
                _repository.Update(token);
            }
            _repository.Save();

            HttpContext.Response.Headers.Append("accessToken", accessToken);
            HttpContext.Response.Headers.Append("refreshToken", token.RefreshToken);

            return Ok(new AuthResponse { AccessToken = accessToken, RefreshToken = token.RefreshToken, User = userDTO });
        }

        [HttpPost("/api/v1/register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.Login,
                Salt = GetSalt()
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            if (!result.Succeeded) 
                return ValidationProblem(ModelState);

            var findUser = _repository.GetOne<User>(u => u.UserName == model.Login);
            if (findUser == null) 
                throw new Exception($"User {model.Login} not found");

            return await Login(new LoginViewModel
            {
                Login = model.Login,
                Password = model.Password,
                DeviceId = Guid.NewGuid().ToString(),
            });
        }

        /// <summary>
        /// Обновление access и refresh токена
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns>Возвращает новые access и refresh токены</returns>
        [HttpPost("/api/v1/refresh-token")]
        public async Task<ActionResult<TokenViewModel>> RefreshToken(TokenViewModel tokenModel)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken);

            if (principal == null)
            {
                ModelState.AddModelError(nameof(TokenViewModel.AccessToken), _localizer["InvalidAccessToken"]);
                return ValidationProblem(ModelState);
            }

            var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var user = await _repository.GetByIdAsync<User>(userId);
            if (user == null) //|| user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                ModelState.AddModelError("error", _localizer["InvalidAccessToken"]);
                return Conflict(ModelState);
            }

            var token = _repository.GetOne<Token>(rt => rt.UserId == user.Id &&
                                                 (rt.RefreshToken == tokenModel.RefreshToken || rt.PreviousRefreshToken == tokenModel.RefreshToken));
            if (token == null)
            {
                ModelState.AddModelError("error", _localizer["InvalidRefreshToken"]);
                return Conflict(ModelState);
            }

            if (token.PreviousRefreshToken == tokenModel.RefreshToken)
            {
                _repository.Delete(token);
                _repository.Save();
                ModelState.AddModelError("error", _localizer["InvalidRefreshToken"]);
                return Conflict(ModelState);
            }

            if (DateTime.UtcNow >= token.ExpiryTime)
            {
                ModelState.AddModelError("error", _localizer["RefreshTokenExpired", token.ExpiryTime]);
                return Conflict(ModelState);
            }

            var userDTO = _mapper.Map<UserDTO>(user);
            var serializeSettings = HttpContext.RequestServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value.SerializerSettings;
            var identity = userDTO.GetIdentity(serializeSettings);
            
            var newAccessToken = _tokenService.GenerateAccessToken(identity);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            token.AccessTokenJti = identity.FindFirst(JwtRegisteredClaimNames.Jti)!.Value.ToString();
            token.PreviousRefreshToken = token.RefreshToken;
            token.RefreshToken = newRefreshToken;
            _repository.Update(token);
            _repository.Save();

            HttpContext.Response.Headers.Append("accessToken", newAccessToken);
            HttpContext.Response.Headers.Append("refreshToken", newRefreshToken);

            return Ok(new TokenViewModel()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        /// <summary>
        /// Получить свой аккунт пользователя
        /// </summary>
        /// <returns>Обьект User</returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetAccount()
        {
            var userDTO = HttpContext.Items["user"] as UserDTO;
            userDTO = await _repository.GetByIdAsync<User, UserDTO>(userDTO!.Id);

            return Ok(userDTO);
        }

        /// <summary>
        /// Сброс refresh токенов юзера(выход со всех устройств)
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("revoke")]
        public IActionResult Revoke()
        {
            var userDTO = HttpContext.Items["user"] as UserDTO;

            _repository.Delete<Token>(rt => rt.UserId == userDTO!.Id);
            _repository.Save();

            return Ok();
        }

        /// <summary>
        /// Выход с одного текущего устройства
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("/api/v1/logout")]
        public IActionResult Logout(DeviceViewModel model)
        {
            var userDTO = HttpContext.Items["user"] as UserDTO;

            _repository.Delete<Token>(rt => rt.UserId == userDTO!.Id && rt.DeviceId == model.DeviceId);
            _repository.Save();

            return Ok();
        }

        private string GetSalt()
        {
            var salt = new byte[32];
            using var random = RandomNumberGenerator.Create();
            random.GetNonZeroBytes(salt);
            return Convert.ToBase64String(salt);
        }
    }
}
