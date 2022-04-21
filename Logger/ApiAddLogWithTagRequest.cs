using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Logger
{
    class ApiAddLogWithTagRequest : ApiAddLogRequest
    {
        string _TagName;
        public string TagName { get { return _TagName; } set { _TagName = value; } }

        public ApiAddLogWithTagRequest() : base()
        {
            _TagName = string.Empty;
        }
    }
}
