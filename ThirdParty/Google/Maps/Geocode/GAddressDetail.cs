using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSSystem.ThirdParty.Google.Maps.Geocode
{
    public class GAddressDetail
    {
        string _country, _state, _county, streetName, _streetNumber;
        string _fullAddress;
        public string Country { get { return _country; } set { _country = value; } }
        public string County { get { return _county; } set { _county = value; } }
        public string FullAddress { get { return _fullAddress; } set { _fullAddress = value; } }
        public string State { get { return _state; } set { _state = value; } }
        public string StreetName { get { return streetName; } set { streetName = value; } }
        public string StreetNumber { get { return _streetNumber; } set { _streetNumber = value; } }
        public override string ToString()
        {
            return _fullAddress;
        }

        public bool IsSouthCalifornia()
        {
            try
            {
                string[] southCACounties = new string[] { "Imperial", "Kern", "Los Angeles", "Orange", "Riverside", "San Bernardino", "San Diego", "San Luis Obispo", "Santa Cruz", "Ventura" };
                foreach (string county in southCACounties)
                {
                    if (_county.IndexOf(county, StringComparison.InvariantCultureIgnoreCase) >= 0) return true;
                }
                return false;     
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
