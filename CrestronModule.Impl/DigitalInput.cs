using Crestron.Logos.SplusObjects;
using CrestronModule.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestronModule.Impl
{
    public class DigitalInput : IInput<bool>
    {
        Crestron.Logos.SplusObjects.DigitalInput input;

        public DigitalInput(Crestron.Logos.SplusObjects.DigitalInput input)
        {
            this.input = input;
        }

        public bool Value
        {
            get { return input.Value == 0 ? false : true; }
        }
    }

}
