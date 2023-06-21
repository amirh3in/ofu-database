using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using reservationApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using reservationApi.Models;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace reservationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepo;
        public AuthController(IAuthRepository authRepo2)
        {
            authRepo = authRepo2;
        }
        [HttpPost("login")]
        public async Task<ActionResult<Tokens>> login(User user)
        {
            Tokens token = authRepo.Login(user);

            if (token != null) return Ok(token);

            return StatusCode(StatusCodes.Status401Unauthorized, new { message = "نام کاربری یا رمز عبور اشتباه است" });

        }

        [HttpPost("signin")]
        public async Task<ActionResult<Tokens>> Signin([FromBody] Customer customer)
        {
            var res = authRepo.SignIn(customer);

            if (res != null) return Ok(res);

            return StatusCode(StatusCodes.Status401Unauthorized, new { message = "کاربری با این مشخصات قبلا ثبت شده است" });
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<Tokens>> refreshToken([FromBody] RefreshToken refreshToken)
        {
            var res = authRepo.RefreshToken(refreshToken);

            if (res == null) return StatusCode(StatusCodes.Status401Unauthorized, new { message = "رفرش توکن نامعتبر میباشد" });

            return res;

        }

    }
}
