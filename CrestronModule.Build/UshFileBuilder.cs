using System;
using System.Text;
using CrestronModuleCore;

namespace CrestronModule.Build
{
    internal class UshFileBuilder : IModuleFactory
    {
        private readonly StringBuilder moduleSb = new StringBuilder();
        public UshFileBuilder()
        {
            moduleSb.AppendLine("#DEFAULT_VOLATILE");
            moduleSb.AppendLine("#ENABLE_STACK_CHECKING");
            moduleSb.AppendLine("#ENABLE_TRACE");
            moduleSb.AppendLine();
        }

        public Input<ushort> AnalogInput(string name, Action<ushort> onChange)
        {
            moduleSb.AppendLine($"ANALOG_INPUT {name};");
            return null;
        }

        public Output<ushort> AnalogOutput(string name)
        {
            moduleSb.AppendLine($"ANALOG_OUTPUT {name};");
            return null;
        }
        public void AnalogInputSkip()
        {
            moduleSb.AppendLine($"ANALOG_INPUT _SKIP_;");
        }

        public void AnalogOutputSkip()
        {
            moduleSb.AppendLine($"ANALOG_OUTPUT _SKIP_;");
        }

        public Input<bool> DigitalInput(string name, Action<bool> onChange)
        {
            moduleSb.AppendLine($"DIGITAL_INPUT {name};");
            return null;
        }

        public Output<bool> DigitalOutput(string name)
        {
            moduleSb.AppendLine($"DIGITAL_OUTPUT {name};");
            return null;
        }
        public void DigitalInputSkip()
        {
            moduleSb.AppendLine($"DIGITAL_INPUT _SKIP_;");
        }

        public void DigitalOutputSkip()
        {
            moduleSb.AppendLine($"DIGITAL_OUTPUT _SKIP_;");
        }

        public Input<string> StringInput(string name, int maxCapacity, Action<string> onChange)
        {
            moduleSb.AppendLine($"STRING_INPUT {name}[{maxCapacity}];");
            return null;
        }

        public Output<string> StringOutput(string name)
        {
            moduleSb.AppendLine($"STRING_OUTPUT {name};");
            return null;
        }

        public void StringInputSkip()
        {
            moduleSb.AppendLine($"STRING_INPUT _SKIP_;");

        }

        public void StringOutputSkip()
        {
            moduleSb.AppendLine($"STRING_OUTPUT _SKIP_;");
        }

        public override string ToString()
        {
            return moduleSb.ToString();
        }
    }
}
