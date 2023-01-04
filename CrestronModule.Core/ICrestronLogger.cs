using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestronModule.Core
{
    public interface ICrestronLogger
    {
        void Trace(string format, params object[] args);
        void Print(string format, params object[] args);
    }
}
