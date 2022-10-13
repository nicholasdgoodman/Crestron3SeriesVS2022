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

        public CrestronModuleImpl(IInputOutputFactory ioFactory, ICrestronLogger logger)
        {
            this.logger = logger;
            outputA = ioFactory.CreateDigitalOutput("DOutA");
            
            inputA = ioFactory.CreateDigitalInput("DInA", dInA_OnChange);
            inputB = ioFactory.CreateDigitalInput("DInB", null);

            sOutA = ioFactory.CreateStringOutput("SOutA");
            
            sInA = ioFactory.CreateStringInput("SInA", 25, sInA_OnChange);

            aOutA = ioFactory.CreateAnalogOutput("AOutA");
            aInA = ioFactory.CreateAnalogInput("AInA", aIn_OnChange);
        }


        public void Main()
        {
            this.logger.Trace("Hello World. From IMPL!");
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