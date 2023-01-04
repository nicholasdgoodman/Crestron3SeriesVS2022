using Crestron.Logos.SplusObjects;
using CrestronModule.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestronModule.Impl
{
    public class StringOutput : IOutput<string>
    {
        Crestron.Logos.SplusObjects.StringOutput output;
        public StringOutput(Crestron.Logos.SplusObjects.StringOutput output)
        {
            this.output = output;
        }
        public string Value
        {
            get => output.Value.ToString();
            set => output.UpdateValue(value);
        }
    }

}
