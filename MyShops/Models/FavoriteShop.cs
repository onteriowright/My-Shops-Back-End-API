using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShops.Models
{
    public class FavoriteShop
    {
        public int Id { get; set; }
        public string ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopLocation { get; set; }
        public int Rating { get; set; }
        public string ShopImage { get; set; }
        public string Contact { get; set; }
        public string Street { get; set; }
        public string CityStateZip { get; set; }
        public int UserId { get; set; }

    }
}
