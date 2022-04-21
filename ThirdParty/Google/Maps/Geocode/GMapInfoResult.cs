using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSSystem.ThirdParty.Google.Maps.Geocode
{
    public class GMapInfoResult
    {
        string _status;
        GMapGeocode[] _results;
        public string Status { get { return _status; } set { _status = value; } }
        public GMapGeocode[] Results { get { return _results; } set { _results = value; } }
    }
}
