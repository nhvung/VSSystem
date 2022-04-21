using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Net.Mail
{
    public enum EMailProcessType : int
    {
        Smtp = 1, // default
        SendgridApi = 2
    }
}
