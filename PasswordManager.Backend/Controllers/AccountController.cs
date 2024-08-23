using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using PasswordManager.Backend.Data;
using PasswordManager.Backend.Data.Entities;

namespace PasswordManager.Backend.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(
            SignInManager<User> signInManager, 
            UserManager<User> userManager, 
            IRepository repository, 
            IMapper mapper, 
            IStringLocalizer<BaseController> localizer) : base(repository, localizer, mapper) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
    }
}
