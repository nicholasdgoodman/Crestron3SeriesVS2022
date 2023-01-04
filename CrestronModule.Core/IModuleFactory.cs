using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestronModule.Core
{
    public interface IModuleFactory
    {
        IInput<bool> DigitalInput(string name, Action<bool> onChange);
        IOutput<bool> DigitalOutput(string name);
        void DigitalInputSkip();
        void DigitalOutputSkip();
        IInput<string> StringInput(string name, int maxCapacity, Action<string> onChange);
        IOutput<string> StringOutput(string name);
        void StringInputSkip();
        void StringOutputSkip();
        IInput<ushort> AnalogInput(string name, Action<ushort> onChange);
        IOutput<ushort> AnalogOutput(string name);
        void AnalogInputSkip();
        void AnalogOutputSkip();

        IParameter<string> StringParameter(string name, int maxCapacity);
        void StringParameterSkip();
    }
}
