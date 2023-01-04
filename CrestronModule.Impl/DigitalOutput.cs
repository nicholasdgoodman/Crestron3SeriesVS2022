using Crestron.Logos.SplusObjects;
using CrestronModule.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestronModule.Impl
{
    public class DigitalOutput : IOutput<bool>
    {
        Crestron.Logos.SplusObjects.DigitalOutput output;

        public DigitalOutput(Crestron.Logos.SplusObjects.DigitalOutput output)
        {
            this.output = output;
        }

        public bool Value
        {
            get { return output.Value == 0 ? false : true; }
            set { output.Value = value ? 1 : 0; }
        }
    }

}
