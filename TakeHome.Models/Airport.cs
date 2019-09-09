using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class Airport
    {
        public string Name { get; set; }
        public string City { get; set; }
		public string Country { get; set; }
        public string Iata3 { get; set; }
        public Coordinates Coordinates { get; set; }
    }
}
