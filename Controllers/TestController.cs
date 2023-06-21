using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using reservationApi.Models;
using Dapper.FastCrud;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace reservationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TestController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<Customer>>> getAll()
        {
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("conn")))
            {
                var res = db.Find<Customer>().ToList();
                return Ok(res);
            }
        }
    }
}
