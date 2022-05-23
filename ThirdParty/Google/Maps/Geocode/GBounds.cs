using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSSystem.ThirdParty.Google.Maps.Geocode
{
    public class GBounds
    {
        GPoint _northeast, _southwest;
        public GPoint northeast { get { return _northeast; } set { _northeast = value; } }
        public GPoint southwest { get { return _southwest; } set { _southwest = value; } }
    }
}
