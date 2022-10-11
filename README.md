# Crestron3SeriesVS2022

Build 3-Series Programs from VS2022!

Requires:
- Crestron SIMPL Windows to be installed
- .NET Framework 3.5 (and possibly .NET Compact Framework 3.5)

This project leverages the available `Crestron.SimplSharp.SDK.Program` nuget package with a .NET Framework 3.5 project (even though the packages indicate a required Framework verison of 4.7). For some reason, in this configuration and with the latest IDE, the generated assembly is not digitally signed during build and then would fail to load within the Crestron's Win CE .NET sandbox.

As a result, a helper project was added which invokes the same helper method used by SIMPL to sign SIMPL+ assemblies on the project output file. This is executed as a separate application because the code requires running in Framework 4.6 and is only x86 compatibile, so it cannot be invoked from a Rosyln Code Task Factory.

So far, the only functionalty test was a basic "Hello World" application on a DMPS3-4K-150-C.
Your mileage may vary!
