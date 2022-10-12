using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;

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
                        var generator = new UshGeneratorModule();
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

        private class UshGeneratorModule : MyCrestronModule.ICrestronModuleBuilder
        {
            private StringBuilder moduleSb = new StringBuilder();

            public UshGeneratorModule()
            {
                moduleSb.AppendLine("#DEFAULT_VOLATILE");
                moduleSb.AppendLine("#ENABLE_STACK_CHECKING");
                moduleSb.AppendLine("#ENABLE_TRACE");
                moduleSb.AppendLine();
            }

            public MyCrestronModule.Input<bool> CreateDigitalInput(string name, Action<bool> onChange)
            {
                moduleSb.Append("DIGITAL_INPUT ");
                moduleSb.Append(name);
                moduleSb.AppendLine(";");
                return null;
            }

            public MyCrestronModule.Output<bool> CreateDigitalOutput(string name)
            {
                moduleSb.Append("DIGITAL_OUTPUT ");
                moduleSb.Append(name);
                moduleSb.AppendLine(";");
                return null;
            }

            public override string ToString()
            {
                return moduleSb.ToString();
            }
        }
    }
}
