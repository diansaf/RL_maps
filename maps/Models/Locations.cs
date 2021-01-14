using System.Collections.Generic;

namespace maps.Models
{
    public class Locations
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public int Type { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Locations()
        {

        }

        public Locations(int id, string code, string address, int type, double latitude, double longitude)
        {
            this.Id = id;
            this.Code = code;
            this.Address = address;
            this.Type = type;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }

    public class LocationLists
    {
        public IEnumerable<Locations> LocationList { get; set; }
    }
}