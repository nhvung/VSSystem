using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSSystem.ThirdParty.Google.Maps.Geocode
{
    public class GGeometry
    {
        GPoint _location;
        string _location_type;
        GViewPort _viewport;
        GBounds _bounds;
        public GPoint location { get { return _location; } set { _location = value; } }
        public string location_type { get { return _location_type; } set { _location_type = value; } }
        public GViewPort viewport { get { return _viewport; } set { _viewport = value; } }
        public GBounds bounds { get { return _bounds; } set { _bounds = value; } }
    }
}
