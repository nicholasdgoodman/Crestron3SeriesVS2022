using Crestron.Logos.SplusObjects;
using CrestronModule.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestronModule.Impl
{
    public class StringInput : IInput<string>
    {
        Crestron.Logos.SplusObjects.StringInput input;
        public StringInput(Crestron.Logos.SplusObjects.StringInput input)
        {
            this.input = input;
        }
        public string Value { get => input.Value.ToString(); }
    }

}
