using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNetCoreMVCProject.Validation
{
    public class Validation
    {
        public class RestrictedDate : ValidationAttribute
        {
            public override bool IsValid(object date)
            {
                DateTime pDate = (DateTime)date;
                return pDate < DateTime.Now;
            }
        }
    }
}
