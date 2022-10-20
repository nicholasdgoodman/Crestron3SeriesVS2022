using CrestronModuleCore;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CrestronModule.Build
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var command = args[0];
                var filename = args[1];

                switch (command)
                {
                    case "/sign":
                        SignAssembly(filename);
                        break;
                    case "/usp":
                        GenerateUsh(filename);
                        break;
                    default:
                        throw new Exception($"Unknown command: {command}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void SignAssembly(string filename)
        {
            var csharpCompilerAsm = Assembly.LoadFrom(@"C:\Program Files (x86)\Crestron\Simpl\CSharpCompiler.dll");
            var signAssemblyType = csharpCompilerAsm.GetType("CSharpCompiler.SignAssembly");
            var signAssemblyCtor = signAssemblyType.GetConstructor(new Type[] { });
            var signer = signAssemblyCtor.Invoke(new object[] { });

            var signMethod = signAssemblyType.GetMethod("Sign");
            signMethod.Invoke(signer, new object[] { filename });
        }

        static void GenerateUsh(string filename)
        {
            var directory = Path.GetDirectoryName(filename);
            var fileName = Path.GetFileNameWithoutExtension(filename);

            var assembly = Assembly.LoadFrom(filename);
            var moduleType = assembly.GetTypes().FirstOrDefault(t => typeof(ICrestronModule).IsAssignableFrom(t));
            var moduleCtor = moduleType.GetConstructor(new Type[] { typeof(IModuleFactory), typeof(ICrestronLogger) });

            var generator = new UshFileBuilder();
            moduleCtor.Invoke(new object[] { generator, null });

            using (var file = File.OpenWrite(Path.Combine(directory, fileName + ".usp")))
            {
                var data = Encoding.UTF8.GetBytes(generator.ToString());
                file.Write(data, 0, data.Length);
            }
        }
    }
}