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
    public class FavoriteShopsController : ControllerBase
    {
        private readonly IConfiguration _config;
        public FavoriteShopsController(IConfiguration config)
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

        // Get all FavoriteShops from the database
        [HttpGet]
        public async Task<IActionResult> GET()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, ShopId, ShopName, ShopLocation, Rating, ShopImage, Contact, Street, CityStateZip, UserId
                        FROM FavoriteShops
                       "
                    ;

                    SqlDataReader reader = cmd.ExecuteReader();
                    var favoriteShops = new List<FavoriteShop>();

                    while (reader.Read())
                    {
                        var favoriteShop = new FavoriteShop
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ShopId = reader.GetString(reader.GetOrdinal("ShopId")),
                            ShopName = reader.GetString(reader.GetOrdinal("ShopName")),
                            ShopLocation = reader.GetString(reader.GetOrdinal("ShopLocation")),
                            Rating = reader.GetInt32(reader.GetOrdinal("Rating")),
                            ShopImage = reader.GetString(reader.GetOrdinal("ShopImage")),
                            Contact = reader.GetString(reader.GetOrdinal("Contact")),
                            Street = reader.GetString(reader.GetOrdinal("Street")),
                            CityStateZip = reader.GetString(reader.GetOrdinal("CityStateZip")),
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId"))
                        };

                        favoriteShops.Add(favoriteShop);
                    }
                    reader.Close();

                    return Ok(favoriteShops);
                }
            }
        }

        // Get a single favoriteBarberShop by Id from database
        [HttpGet("{id}", Name = "GetFavoriteShop")]
        public async Task<IActionResult> GET([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, ShopId, ShopName, ShopLocation, Rating, ShopImage, Contact, Street, CityStateZip, UserId 
                        FROM FavoriteShops
                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    FavoriteShop favoriteShop = null;

                    if (reader.Read())
                    {
                        favoriteShop = new FavoriteShop
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ShopId = reader.GetString(reader.GetOrdinal("ShopId")),
                            ShopName = reader.GetString(reader.GetOrdinal("ShopName")),
                            ShopLocation = reader.GetString(reader.GetOrdinal("ShopLocation")),
                            Rating = reader.GetInt32(reader.GetOrdinal("Rating")),
                            ShopImage = reader.GetString(reader.GetOrdinal("ShopImage")),
                            Contact = reader.GetString(reader.GetOrdinal("Contact")),
                            Street = reader.GetString(reader.GetOrdinal("Street")),
                            CityStateZip = reader.GetString(reader.GetOrdinal("CityStateZip")),
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId"))
                        };
                        reader.Close();

                        return Ok(favoriteShop);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }


        // Add shop to favorites
        [HttpPost]
        public async Task<IActionResult> POST([FromBody] FavoriteShop favoriteShop)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        INSERT INTO FavoriteShops (ShopId, ShopName, ShopLocation, Rating, ShopImage, Contact, Street, CityStateZip, UserId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@shopId, @shopName, @shopLocation, @rating, @shopImage, @contact, @street, @cityStateZip, @userId)";

                    cmd.Parameters.Add(new SqlParameter("@shopId", favoriteShop.ShopId));
                    cmd.Parameters.Add(new SqlParameter("@shopName", favoriteShop.ShopName));
                    cmd.Parameters.Add(new SqlParameter("@shopLocation", favoriteShop.ShopLocation));
                    cmd.Parameters.Add(new SqlParameter("@rating", favoriteShop.Rating));
                    cmd.Parameters.Add(new SqlParameter("@shopImage", favoriteShop.ShopImage));
                    cmd.Parameters.Add(new SqlParameter("@contact", favoriteShop.Contact));
                    cmd.Parameters.Add(new SqlParameter("@street", favoriteShop.Street));
                    cmd.Parameters.Add(new SqlParameter("@cityStateZip", favoriteShop.CityStateZip));
                    cmd.Parameters.Add(new SqlParameter("@userId", favoriteShop.UserId));

                    int newId = (int)cmd.ExecuteScalar();
                    favoriteShop.Id = newId;
                    return CreatedAtRoute("GetFavoriteShop", new { id = newId }, favoriteShop);
                }
            }
        }
    }
}