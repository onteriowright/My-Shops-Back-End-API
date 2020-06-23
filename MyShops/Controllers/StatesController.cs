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
    public class StatesController : ControllerBase
    {
        private readonly IConfiguration _config;
        public StatesController(IConfiguration config)
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
                        SELECT Id, State
                        FROM States"
                    ;

                    SqlDataReader reader = cmd.ExecuteReader();
                    var states = new List<SelectState>();
                  

                    while (reader.Read())
                    {
                        var state = new SelectState
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            State = reader.GetString(reader.GetOrdinal("State"))
                        };

                        states.Add(state);
                    }
                    reader.Close();

                    return Ok(states);
                }
            }
        }

    }
}