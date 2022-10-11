using System;
using System.Diagnostics;
using CSharpCompiler;

namespace AssemblySign
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var signer = new SignAssembly();

            foreach(var filename in args)
            {
                try
                {
                    signer.Sign(filename);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("Unable to sign file {0}\n{1})", filename, ex.Message);
                }
            }
        }
    }
}
