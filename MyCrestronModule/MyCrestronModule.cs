using System;
using CrestronModuleCore;

namespace MyCrestronModule
{
    public class CrestronModuleImpl : ICrestronModule, IMainMethod
    {
        ICrestronLogger logger;
        Input<bool> inputA, inputB;
        Output<bool> outputA;
        Input<string> sInA;
        Output<string> sOutA;
        Input<ushort> aInA;
        Output<ushort> aOutA;

        public CrestronModuleImpl(IModuleFactory module, ICrestronLogger logger)
        {
            this.logger = logger;
            
            this.inputA = module.DigitalInput("DInA", dInA_OnChange);
            this.inputB = module.DigitalInput("DInB", null);
            module.DigitalInputSkip();

            this.sInA = module.StringInput("SInA", 25, sInA_OnChange);
            this.aInA = module.AnalogInput("AInA", aIn_OnChange);

            this.outputA = module.DigitalOutput("DOutA");
            module.DigitalOutputSkip();
            module.DigitalOutputSkip();

            this.sOutA = module.StringOutput("SOutA");
            this.aOutA = module.AnalogOutput("AOutA");
        }

        public void Main()
        {
            this.logger.Trace($"Hello World. From IMPL! HostName {System.Net.Dns.GetHostName()}");
            this.sOutA.Value = "Initial Value";
            this.aOutA.Value = 1234;
        }

        private void dInA_OnChange(bool value)
        {
            this.logger.Trace($"DInA OnChange {value}");
            outputA.Value = value;
        }

        private void sInA_OnChange(string value)
        {
            this.logger.Trace($"SInA OnChange {value}");
            sOutA.Value = value;
        }
        private void aIn_OnChange(ushort value)
        {
            this.logger.Trace($"AInA OnChange {value}");
            aOutA.Value = value;
        }
    }
}