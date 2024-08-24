using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using PasswordManager.Backend.Data;
using PasswordManager.Backend.Data.Entities;
using PasswordManager.Backend.DTOs;
using PasswordManager.Backend.Models;
using PasswordManager.Backend.ViewModels;

namespace PasswordManager.Backend.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CredentialsController : BaseController
    {
        public CredentialsController(IRepository repository, IStringLocalizer<BaseController> localizer, IMapper mapper)  : base(repository, localizer, mapper)
        {

        }

        /// <summary>
        /// Получение пароля по id
        /// </summary>
        /// <param name="credentialId"></param>
        /// <returns></returns>
        [HttpGet("{credentialId}")]
        public ActionResult<CredentialDTO> Get(int credentialId)
        {
            var userDTO = HttpContext.Items["user"] as UserDTO;

            var credentialDTO = _repository.GetOne<Credential, CredentialDTO>(c => c.Id == credentialId && c.UserId == userDTO!.Id);
            if (credentialDTO == null)
                return NotFound(new NotFoundDescription(_localizer["NotFoundCredential"], _localizer["NotFoundCredentialDescription", credentialId]));

            return Ok(credentialDTO);
        }

        /// <summary>
        /// Получения всех паролей пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<CredentialDTO>> Get()
        {
            var userDTO = HttpContext.Items["user"] as UserDTO;
            var credentialsDTO = await _repository.GetAsync<Credential, CredentialDTO>(c => c.UserId == userDTO!.Id);
            return Ok(credentialsDTO);
        }

        /// <summary>
        /// Создание пароля
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<CredentialDTO> Create(CredentialViewModel model)
        {
            var userDTO = HttpContext.Items["user"] as UserDTO;

            var credential = new Credential()
            {
                Login = model.Login,
                Password = model.Password,
                CreatedAt = DateTime.UtcNow,
                Service = model.Service,
                URL = model.URL,
                UserId = userDTO!.Id
            };

            _repository.Create(credential);
            _repository.Save();

            var credentialDTO = _mapper.Map<CredentialDTO>(credential);
            return Ok(credentialDTO);
        }

        /// <summary>
        /// Создание пароля
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{credentialId}")]
        public ActionResult<CredentialDTO> Update(int credentialId, CredentialViewModel model)
        {
            var userDTO = HttpContext.Items["user"] as UserDTO;

            var credential = _repository.GetOne<Credential>(c => c.Id == credentialId && c.UserId == userDTO!.Id);
            if(credential == null)
                return NotFound(new NotFoundDescription(_localizer["NotFoundCredential"], _localizer["NotFoundCredentialDescription", credentialId]));

            credential.Login = model.Login;
            credential.Password = model.Password;
            credential.Service = model.Service;
            credential.URL = model.URL;
            
            _repository.Update(credential);
            _repository.Save();

            var credentialDTO = _mapper.Map<CredentialDTO>(credential);
            return Ok(credentialDTO);
        }

        /// <summary>
        /// Удаление пароля
        /// </summary>
        /// <param name="credentialId"></param>
        /// <returns></returns>
        [HttpDelete("{credentialId}")]
        public ActionResult Delete(int credentialId)
        {
            var userDTO = HttpContext.Items["user"] as UserDTO;
            if(_repository.Any<Credential>(c => c.Id == credentialId && c.UserId == userDTO!.Id))
                return NotFound(new NotFoundDescription(_localizer["NotFoundCredential"], _localizer["NotFoundCredentialDescription", credentialId]));

            _repository.Delete<Credential>(c => c.Id == credentialId && c.UserId == userDTO!.Id);
            _repository.Save();
            return Ok();
        }
    }
}
