using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShops.Models
{
    public class ShopReview
    {
        public int Id { get; set; }
        public string Reviews { get; set; }
        public DateTime? DateCreated { get; set; }
        public string ShopName { get; set; }
        public string ShopLocation { get; set; }
        public int UserId { get; set; }

    }
}
