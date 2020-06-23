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
    public class ServicesController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ServicesController(IConfiguration config)
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
                        SELECT Id, Service
                        FROM Services"
                    ;

                    SqlDataReader reader = cmd.ExecuteReader();
                    var services = new List<SelectService>();


                    while (reader.Read())
                    {
                        var service = new SelectService
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Service = reader.GetString(reader.GetOrdinal("Service"))
                        };

                        services.Add(service);
                    }
                    reader.Close();

                    return Ok(services);
                }
            }
        }
    }
}