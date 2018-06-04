using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Common.VCalendar
{

    public class CalendarDataException : Exception
    {
        public object Element { get; private set; }

        public CalendarDataException(object element, string message)
            : base(message)
        {
            this.Element = element;
        }
    }
}
