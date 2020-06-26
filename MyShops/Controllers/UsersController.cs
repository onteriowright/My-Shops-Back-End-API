using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyShops.Models;

namespace MyShops.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _config;
        public UsersController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }


        [HttpGet]
        public async Task<IActionResult> GET()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Email, Password, Username
                        FROM Users"
                    ;

                    SqlDataReader reader = cmd.ExecuteReader();
                    var users = new List<User>();


                    while (reader.Read())
                    {
                        var user = new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Password = reader.GetString(reader.GetOrdinal("Password")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                        };

                        users.Add(user);
                    }
                    reader.Close();

                    return Ok(users);
                }
            }
        }

        // Add users
        [HttpPost]
        public async Task<IActionResult> POST([FromBody] User user)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Users (Email, Password, Username)
                                        OUTPUT INSERTED.Id
                                        VALUES (@email, @password, @username)";

                    cmd.Parameters.Add(new SqlParameter("@email", user.Email));
                    cmd.Parameters.Add(new SqlParameter("@password", user.Password));
                    cmd.Parameters.Add(new SqlParameter("@username", user.Username));

                    int newId = (int)cmd.ExecuteScalar();
                    user.Id = newId;
                    return Ok(user);
                }
            }
        }


    }
}