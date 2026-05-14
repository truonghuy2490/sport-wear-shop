using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }
}
