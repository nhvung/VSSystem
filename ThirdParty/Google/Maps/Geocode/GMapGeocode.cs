using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSSystem.ThirdParty.Google.Maps.Geocode
{
    public class GMapGeocode
    {
        GAddressComponent[] _address_components;
        string _formatted_address, _place_id;
        GGeometry _geometry;
        string[] _types;

        public GAddressComponent[] address_components { get { return _address_components; } set { _address_components = value; } }
        public string formatted_address { get { return _formatted_address; } set { _formatted_address = value; } }
        public string place_id { get { return _place_id; } set { _place_id = value; } }
        public string[] types { get { return _types; } set { _types = value; } }
        public GGeometry geometry { get { return _geometry; } set { _geometry = value; } }

        public GAddressDetail ConvertToAddressDetail()
        {
            GAddressDetail detail = new GAddressDetail();
            detail.FullAddress = _formatted_address;
            foreach (GAddressComponent ac in _address_components)
            {
                if (ac.types.Contains("country", StringComparer.InvariantCultureIgnoreCase))
                {
                    detail.Country = ac.long_name;
                }
                else if (ac.types.Contains("administrative_area_level_1", StringComparer.InvariantCultureIgnoreCase))
                {
                    detail.State = ac.long_name;
                }
                if (ac.types.Contains("administrative_area_level_2", StringComparer.InvariantCultureIgnoreCase))
                {
                    detail.County = ac.long_name;
                }
                if (ac.types.Contains("route", StringComparer.InvariantCultureIgnoreCase))
                {
                    detail.StreetName = ac.long_name;
                }
                if (ac.types.Contains("street_number", StringComparer.InvariantCultureIgnoreCase))
                {
                    detail.StreetNumber = ac.long_name;
                }
            }
            return detail;
        }
    }
}
