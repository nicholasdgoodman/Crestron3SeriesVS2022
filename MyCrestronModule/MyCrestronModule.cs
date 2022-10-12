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
            inputA = module.CreateDigitalInput("InputA", InputA_OnChange);
            inputB = module.CreateDigitalInput("InputB", null);
            outputA = module.CreateDigitalOutput("OuputA");
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