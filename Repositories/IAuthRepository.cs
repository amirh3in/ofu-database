using Dapper;
using Dapper.FastCrud;
using Microsoft.Extensions.Configuration;
using reservationApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace reservationApi.Repositories
{
    public interface IAuthRepository
    {
        public Tokens Login(User user);
        public Tokens SignIn(Customer customer);
        public Tokens RefreshToken(RefreshToken rf);
    }
    public class AuthRepository : IAuthRepository
    {
        private readonly IConfiguration _config;
        private readonly IJwtRepository _jwt;
        public AuthRepository(IConfiguration config, IJwtRepository jwt)
        {
            _config = config;
            _jwt = jwt;
        }
        public Tokens Login(User user)
        {

            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("conn")))
            {
                Customer customer = new Customer();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@name", user.username, DbType.String);
                parameters.Add("@pass", user.password, DbType.String);

                try
                {
                    // check if user exists or not
                    customer = db.QuerySingle<Customer>("select * from Customers where name = @name and pass = @pass", parameters);
                }
                catch
                {
                    return null;
                }

                Tokens token = _jwt.createToken(customer);
                parameters.Add("refreshToken", token.refreshToken);
                parameters.Add("create_at", DateTime.Now);
                parameters.Add("expires_at", DateTime.Now.AddDays(7));

                //update refreshToken
                db.Query<Customer>("update Customers set refreshToken=@refreshToken, create_at=@create_at, expires_at=@expires_at where name=@name and pass=@pass", parameters);

                return token;
            }
        }

        public Tokens RefreshToken(RefreshToken rf)
        {
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("conn")))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("refreshToken", rf.refreshToken);

                Customer customer = db.QueryFirstOrDefault<Customer>("select * from Customers where refreshToken=@refreshToken", parameters);

                // if refresh token is not exist
                if (customer == null) return null;

                // if refreshToken has expired
                if (DateTime.Now > customer.expires_at) return null;

                //else

                User user = new User
                {
                    username = customer.name,
                    password = customer.pass
                };
                Tokens token = Login(user);

                return token;
            }
        }

        public Tokens SignIn(Customer customer)
        {
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("conn")))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@name", customer.name);
                parameters.Add("@family", customer.family);
                parameters.Add("@email", customer.email);
                parameters.Add("@pass", customer.pass);

                try
                {
                    db.Query<Customer>("insert into Customers(name, family, email, pass) values(@name, @family, @email, @pass)", parameters);
                }
                catch
                {
                    return null;
                }

                Tokens token = _jwt.createToken(customer);
                parameters.Add("refreshToken", token.refreshToken);
                parameters.Add("create_at", DateTime.Now);
                parameters.Add("expires_at", DateTime.Now.AddDays(7));

                //set refreshToken
                db.Query<Customer>("update Customers set refreshToken=@refreshToken, create_at=@create_at, expires_at=@expires_at where name=@name and pass=@pass", parameters);

                return token;

            }
        }
    }

}
