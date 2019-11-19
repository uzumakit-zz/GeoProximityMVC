using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoProximityMVC.Models.DTO
{
    public class UserDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public double Distance { get; set; }
    }
}