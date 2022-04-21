using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSSystem.ThirdParty.Google.Maps.Geocode
{
    public class GAddressComponent
    {
        string _long_name, _short_name;
        string[] _types;

        public string long_name
        {
            get { return _long_name; }
            set { _long_name = value; }
        }

        public string short_name
        {
            get { return _short_name; }
            set { _short_name = value; }
        }

        public string[] types
        {
            get { return _types; }
            set { _types = value; }
        }       
         
    }
}
