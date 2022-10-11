using System;

namespace MyCrestronModule
{
    public class CrestronModuleImpl : IInitializable
    {
        ICrestronModuleBuilder module;
        ICrestronLogger logger;
        Input<int> inputA;
        Input<int> inputB;
        Output<int> outputA;

        public CrestronModuleImpl(ICrestronModuleBuilder module, ICrestronLogger logger)
        {
            this.module = module;
            this.logger = logger;
            inputA = module.CreateDigitalInput("InputA", InputA_OnChange);
            inputB = module.CreateDigitalInput("InputB", null);
            outputA = module.CreateDigitalOutput("OuputA");
        }

        public void Initialize()
        {
            this.logger.Trace("Hello World. From IMPL");
        }

        void InputA_OnChange(int value)
        {
            outputA.Value = (ushort)value;
        }
    }
}