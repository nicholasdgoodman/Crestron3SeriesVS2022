using Crestron.Logos.SplusObjects;
using CrestronModule.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestronModule.Impl
{
    public class AnalogOutput : IOutput<ushort>
    {
        Crestron.Logos.SplusObjects.AnalogOutput output;
        public AnalogOutput(Crestron.Logos.SplusObjects.AnalogOutput output)
        {
            this.output = output;
        }
        public ushort Value
        {
            get => output.Value;
            set => output.Value = value;
        }
    }

}
