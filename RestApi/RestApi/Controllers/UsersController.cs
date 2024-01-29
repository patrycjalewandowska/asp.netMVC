using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestApi.DbServices;
using RestApi.Models;

namespace RestApi.Controllers
{    [Route("api/[controller]")]
     [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IConfiguration configuration;
        private readonly IUserService userService;

        public UsersController(IConfiguration configuration, IUserService userService)
        {
            this.configuration = configuration;
            this.userService = userService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserRegisterModel>> Register(UserRegisterModel user)
        {
            var dbUser = await userService.AddUser(user);
            return StatusCode(StatusCodes.Status200OK, dbUser);

        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserLoginModel>> Login(UserLoginModel user)
        {
            var res = await userService.LoginUser(user);
            return StatusCode(StatusCodes.Status200OK, res.Value);

        }
    }
}
