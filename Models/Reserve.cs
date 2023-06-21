using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace reservationApi.Models
{
    public class Reserve
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int customerId { get; set; }
        public int placeId { get; set; }
        public string price { get; set; }
        public DateTime reserveDate { get; set; }
        public DateTime dateAdded { get; set; }
    }
}
