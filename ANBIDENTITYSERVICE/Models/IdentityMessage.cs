using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ANBIDENTITYSERVICE.Models
{
    public class IdentityMessage
    {
       
        public string Body { get; set; }
        public string Destination { get; set; }
        public string Subject { get; set; }

    }
}
