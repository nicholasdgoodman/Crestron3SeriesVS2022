using Crestron.Logos.SplusObjects;
using CrestronModule.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestronModule.Impl
{
    public class AnalogInput : IInput<ushort>
    {
        Crestron.Logos.SplusObjects.AnalogInput input;
        public AnalogInput(Crestron.Logos.SplusObjects.AnalogInput input)
        {
            this.input = input;
        }
        public ushort Value { get => input.UshortValue; }
    }

}
