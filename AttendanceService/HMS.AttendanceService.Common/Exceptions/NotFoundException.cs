using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.AttendanceService.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
