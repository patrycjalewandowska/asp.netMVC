using Microsoft.AspNetCore.Mvc;
using RestApi.Models;

namespace RestApi.DbServices
{
    public interface IUserService
    {
        Task<UserRegisterModel> AddUser(UserRegisterModel user);
        Task<ActionResult<string>> LoginUser(UserLoginModel user);
        string CreateToken(UserLoginModel user);
    }
}
