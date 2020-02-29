using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geo_coding_addresses
{
    public class Location
    {
        public int Id { get; set; }
        public string Address { get; set; }

        public string Long
        {
            get;set;
        }

        public string Lat
        {
            get;set;
        }
    }
}
