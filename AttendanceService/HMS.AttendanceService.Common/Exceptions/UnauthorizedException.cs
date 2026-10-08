using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.AttendanceService.Common.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
