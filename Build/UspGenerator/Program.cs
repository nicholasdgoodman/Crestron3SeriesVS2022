using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using MyCrestronModule;

namespace UspGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var filepath = args[0];
                var filePath = @"C:\Users\nicho\source\repos\Crestron3SeriesVS2022\MyCrestronModule\bin\Debug\net35\MyCrestronModule.dll";

                var fullPath = Path.GetFullPath(filePath);
                var directory = Path.GetDirectoryName(fullPath);
                var fileName = Path.GetFileNameWithoutExtension(fullPath);

                var assembly = Assembly.LoadFrom(fullPath);
                var type = assembly.GetType("MyCrestronModule.CrestronModuleImpl");

                var constructors = type.GetConstructors();
                
                foreach(var constructor in constructors)
                {
                    var parameters = constructor.GetParameters();
                    var moduleParameterIndex = parameters.ToList().FindIndex(pi => typeof(MyCrestronModule.ICrestronModuleBuilder).IsAssignableFrom(pi.ParameterType));
                    if(moduleParameterIndex != -1)
                    {
                        var parameterValues = parameters.Select(pi => pi.ParameterType.IsValueType ? Activator.CreateInstance(pi.ParameterType) : null).ToArray();
                        var generator = new UshFileBuilder();
                        parameterValues[moduleParameterIndex] = generator;
                        constructor.Invoke(parameterValues);
                        using (var file = File.OpenWrite(Path.Combine(directory, fileName + ".usp")))
                        {
                            var data = Encoding.UTF8.GetBytes(generator.ToString());
                            file.Write(data, 0, data.Length);
                        }
                    }
                }
            }
            finally
            {

            }
        }

        private class UshFileBuilder : ICrestronModuleBuilder
        {
            private readonly StringBuilder moduleSb = new StringBuilder();

            public UshFileBuilder()
            {
                moduleSb.AppendLine("#DEFAULT_VOLATILE");
                moduleSb.AppendLine("#ENABLE_STACK_CHECKING");
                moduleSb.AppendLine("#ENABLE_TRACE");
                moduleSb.AppendLine();
            }

            public Input<ushort> CreateAnalogInput(string name, Action<ushort> onChange)
            {
                moduleSb.AppendLine($"ANALOG_INPUT {name};");
                return null;
            }

            public Output<ushort> CreateAnalogOutput(string name)
            {
                moduleSb.AppendLine($"ANALOG_OUTPUT {name};");
                return null;
            }

            public Input<bool> CreateDigitalInput(string name, Action<bool> onChange)
            {
                moduleSb.AppendLine($"DIGITAL_INPUT {name};");
                return null;
            }

            public Output<bool> CreateDigitalOutput(string name)
            {
                moduleSb.AppendLine($"DIGITAL_OUTPUT {name};");
                return null;
            }

            public Input<string> CreateStringInput(string name, int maxCapacity, Action<string> onChange)
            {
                moduleSb.AppendLine($"STRING_INPUT {name}[{maxCapacity}];");
                return null;
            }

            public Output<string> CreateStringOutput(string name)
            {
                moduleSb.AppendLine($"STRING_OUTPUT {name};");
                return null;
            }

            public override string ToString()
            {
                return moduleSb.ToString();
            }
        }
    }
}
