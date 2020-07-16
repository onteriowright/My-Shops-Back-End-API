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
    public class ShopReviewsController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ShopReviewsController(IConfiguration config)
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

        // Get all Shop Reviews from the database
        [HttpGet]
        public async Task<IActionResult> GET()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Reviews, DateCreated, ShopName, ShopLocation, UserId
                        FROM ShopReviews
                       "
                    ;

                    SqlDataReader reader = cmd.ExecuteReader();
                    var shopReviews = new List<ShopReview>();

                    while (reader.Read())
                    {
                        var shopReview = new ShopReview
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Reviews = reader.GetString(reader.GetOrdinal("Reviews")),
                            DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated")),
                            ShopName = reader.GetString(reader.GetOrdinal("ShopName")),
                            ShopLocation = reader.GetString(reader.GetOrdinal("ShopLocation")),
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId"))
                        };

                        shopReviews.Add(shopReview);
                    }
                    reader.Close();

                    return Ok(shopReviews);
                }
            }
        }

        // Get a single favoriteBarberShop by Id from database
        [HttpGet("{id}", Name = "GetShopReviews")]
        public async Task<IActionResult> GET([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Reviews, DateCreated, ShopName, ShopLocation, UserId
                        FROM ShopReviews
                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    ShopReview shopReview = null;

                    if (reader.Read())
                    {
                        shopReview = new ShopReview
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Reviews = reader.GetString(reader.GetOrdinal("Reviews")),
                            DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated")),
                            ShopName = reader.GetString(reader.GetOrdinal("ShopName")),
                            ShopLocation = reader.GetString(reader.GetOrdinal("ShopLocation")),
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId"))
                        };
                        reader.Close();

                        return Ok(shopReview);
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
        public async Task<IActionResult> POST([FromBody] ShopReview shopReview)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        INSERT INTO ShopReviews (Reviews, DateCreated, ShopName, ShopLocation, UserId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@reviews, @dateCreated, @shopName, @shopLocation, @userId)";

                    cmd.Parameters.Add(new SqlParameter("@reviews", shopReview.Reviews));
                    cmd.Parameters.Add(new SqlParameter("@dateCreated", shopReview.DateCreated));
                    cmd.Parameters.Add(new SqlParameter("@shopName", shopReview.ShopName));
                    cmd.Parameters.Add(new SqlParameter("@shopLocation", shopReview.ShopLocation));
                    cmd.Parameters.Add(new SqlParameter("@userId", shopReview.UserId));

                    int newId = (int)cmd.ExecuteScalar();
                    shopReview.Id = newId;
                    return CreatedAtRoute("GetShopReviews", new { id = newId }, shopReview);
                }
            }
        }

        //Update single shop review by id in database
        [HttpPut("{id}")]
        public async Task<IActionResult> PUT([FromRoute] int id, [FromBody] ShopReview shopReview)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE ShopReviews
                                            SET 
                                            Reviews = @reviews,
                                            DateCreated = @dateCreated,
                                            ShopName = @shopName,
                                            ShopLocation = @shopLocation,
                                            UserId = @userId
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@reviews", shopReview.Reviews));
                        cmd.Parameters.Add(new SqlParameter("@dateCreated", shopReview.DateCreated));
                        cmd.Parameters.Add(new SqlParameter("@shopName", shopReview.ShopName));
                        cmd.Parameters.Add(new SqlParameter("@shopLocation", shopReview.ShopLocation));
                        cmd.Parameters.Add(new SqlParameter("@userId", shopReview.UserId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ShopReviewExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // Delete single shop review by id from database
        [HttpDelete("{id}")]
        public async Task<IActionResult> DELETE([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            DELETE FROM ShopReviews
                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ShopReviewExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // Check to see if shop review exist by id in database
        private bool ShopReviewExist(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Reviews, DateCreated, ShopName, ShopLocation, UserId
                        FROM ShopReviews
                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}