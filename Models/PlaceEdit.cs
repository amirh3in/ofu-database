using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace reservationApi.Models
{
    public class PlaceEdit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int placeId { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string type { get; set; }
    }
}
