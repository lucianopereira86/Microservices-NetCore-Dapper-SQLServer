using APITestRegister.Domain.Domain.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Examples;
using APITestRegister.Presentation.WebAPI.Models.VM;
using APITestRegister.Presentation.WebAPI.SwaggerDocs.Examples.User;
using APITestRegister.Domain.Domain.Models;
using APITestRegister.Domain.Domain.Validations.User;
using APITestRegister.Presentation.WebAPI.Models.Return;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace APITestRegister.Presentation.WebAPI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : BaseController
    {
        private readonly IUserRepository repo;

        public UserController(IUserRepository r) : base()
        {
            repo = r;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="vm" cref="UserRegisterVM"></param>
        [HttpPost]
        [SwaggerResponse(200)]
        [SwaggerResponse(400)]
        [SwaggerResponse(500)]
        [SwaggerRequestExample(typeof(UserRegisterVM), typeof(UserRegisterEx))]
        public async Task<IActionResult> Register([FromBody] UserRegisterVM vm)
        {
            var _vm = Mapper.Map<User>(vm);
            return ExecMethod<User, UserReturnVM>(() => ValidateEntry(new UserRegisterValidation(repo), _vm), () => repo.Register(_vm));
        }

        /// <summary>
        /// Authenticates a user
        /// </summary>
        /// <param name="vm" cref="UserRegisterVM"></param>
        [HttpPost("Login")]
        [SwaggerResponse(200)]
        [SwaggerResponse(400)]
        [SwaggerResponse(500)]
        [SwaggerRequestExample(typeof(UserLoginVM), typeof(UserLoginEx))]
        public async Task<IActionResult> Login([FromBody] UserLoginVM vm)
        {
            var _vm = Mapper.Map<User>(vm);
            return ExecMethod(() => ValidateEntry(new UserLoginValidation(repo), _vm), () => ValidateLogin(_vm));
        }

        private UserReturnVM ValidateLogin(User vm)
        {
            var user = repo.Login(vm);
            var _vm = user ?? new User();
            ValidateEntry(new UserLoginReturnValidation(), _vm);
            var userReturnVM = Mapper.Map<UserReturnVM>(_vm);
            return userReturnVM;
        }
    }
}