using CrestronModule.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CrestronModule.Build
{
    internal class Program
    {
        const string SimplPlusInstallDir = @"C:\Program Files (x86)\Crestron\Simpl";
        static void Main(string[] args)
        {
            try
            {
                var command = args[0];
                var filename1 = args[1];
                var filename2 = args.Length > 2 ? args[2] : null;

                switch (command)
                {
                    case "/sign":
                        SignAssembly(filename1);
                        break;
                    case "/usp":
                        Console.WriteLine($"Generating Crestron Module: {filename1} {filename2}");
                        GenerateUsh(filename1, filename2);
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
            var directory = Path.GetDirectoryName(filename);

            var csharpCompilerAsm = Assembly.LoadFrom(Path.Combine(SimplPlusInstallDir, "CSharpCompiler.dll"));
            var signAssemblyType = csharpCompilerAsm.GetType("CSharpCompiler.SignAssembly");
            var signAssemblyCtor = signAssemblyType.GetConstructor(new Type[] { });
            var signer = signAssemblyCtor.Invoke(new object[] { });

            var signMethod = signAssemblyType.GetMethod("Sign");
            signMethod.Invoke(signer, new object[] { filename });

            signMethod.Invoke(signer, new object[] { Path.Combine(directory, @"CrestronModule.Core.dll") });
            signMethod.Invoke(signer, new object[] { Path.Combine(directory, @"CrestronModule.Impl.dll") });
        }

        static void GenerateUsh(string source, string outDir)
        {
            var assembly = Assembly.LoadFrom(source);
            Console.WriteLine("Assembly Loaded");
            var moduleType = assembly.GetTypes().FirstOrDefault(t => typeof(ICrestronModule).IsAssignableFrom(t));
            Console.WriteLine($"Found Module Class: {moduleType != null}");
            if(moduleType == null)
            {
                return;
            }

            var moduleCtor = moduleType.GetConstructor(new Type[] { typeof(IModuleFactory), typeof(ICrestronLogger) });
            Console.WriteLine($"Constructor Found: {moduleCtor != null}, params: {moduleCtor.GetParameters().Length}");
            
            var generator = new UshFileBuilder();
            var instance = moduleCtor.Invoke(new object[] { generator, null });

            //Console.WriteLine($"Module Name? {moduleType.Name} {moduleType.Name.EndsWith("Module")} {moduleType.Name.Substring(0, moduleType.Name.Length - \"Module\".Length)}");

            var moduleName = moduleType.Name.EndsWith("Module") ?
                moduleType.Name.Substring(0, moduleType.Name.Length - "Module".Length) :
                moduleType.Name;

            Directory.CreateDirectory(outDir);
            var destination = Path.Combine(outDir, $"{moduleName}.usp");
            Console.WriteLine($"Creating module: {destination}");
            using (var file = File.OpenWrite(destination))
            {
                var data = Encoding.UTF8.GetBytes(generator.ToString());
                file.Write(data, 0, data.Length);
            }

            Console.WriteLine("Compiling module...");
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                Path.Combine(SimplPlusInstallDir, "spluscc.exe"), $"/build \"{destination}\" /target series3")
            {
                WorkingDirectory = outDir,
                CreateNoWindow = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            }).WaitForExit();

            Console.WriteLine("Done!");
        }
    }
}