using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace reservationApi.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int personId { get; set; }
        public string pass { get; set; }
        public string name { get; set; }
        public string family { get; set; }
        public string email { get; set; }
        public string refreshToken { get; set; }
        public DateTime create_at { get; set; }
        public DateTime expires_at { get; set; } = DateTime.Now;
    }
}
