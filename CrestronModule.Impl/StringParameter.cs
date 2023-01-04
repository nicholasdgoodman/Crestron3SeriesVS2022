using Crestron.Logos.SplusObjects;
using CrestronModule.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestronModule.Impl
{
    public class StringParameter : IParameter<string>
    {
        Crestron.Logos.SplusObjects.StringParameter parameter;
        public StringParameter(Crestron.Logos.SplusObjects.StringParameter parameter)
        {
            this.parameter = parameter;
        }
        public string Value
        {
            get => parameter.Value.ToString();
        }
    }

}
