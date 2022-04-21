using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSSystem.ThirdParty.Google.Maps.Geocode
{
    public class GPoint
    {
        double _lat, _lng;
        public double lat { get { return _lat; } set { _lat = value; } }
        public double lng { get { return _lng; } set { _lng = value; } }
    }
}
