using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestronModule.Core
{
    public interface IParameter<T>
    {
        T Value { get; }
    }
}
