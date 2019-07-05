using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNetCoreMVCProject.Services
{
    public class AuthSender
    {
        //Values stored as environment variables 
        public string SendGridUser { get; set; }
        //API key - Needed for emails to be sent by SendGrid
        public string SendGridKey { get; set; }
    }
}
