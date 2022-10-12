using System;

namespace MyCrestronModule
{
    public class CrestronModuleImpl : ICrestronModule, IInitializable
    {
        ICrestronLogger logger;
        Input<bool> inputA, inputB;
        Output<bool> outputA;

        public CrestronModuleImpl(ICrestronModuleBuilder module, ICrestronLogger logger)
        {
            this.logger = logger;
            inputA = module.CreateDigitalInput("DInA", InputA_OnChange);
            inputB = module.CreateDigitalInput("DInB", null);
            outputA = module.CreateDigitalOutput("DOutA");

            var soutA = module.CreateStringOutput("SOutA");
            module.CreateStringInput("SInA", 25, v => soutA.Value = v);

            var aoutA = module.CreateAnalogOutput("AOutA");
            module.CreateAnalogInput("AInA", v => aoutA.Value = v);
        }

        public void Initialize()
        {
            this.logger.Trace("Hello World. From IMPL");
        }

        void InputA_OnChange(bool value)
        {
            outputA.Value = value;
        }
    }
}