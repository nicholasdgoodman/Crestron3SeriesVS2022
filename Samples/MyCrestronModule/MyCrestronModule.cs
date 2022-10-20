using System;
using System.IO;
using System.Threading;
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
            this.inputB = module.DigitalInput("DInB", dInB_OnChange);
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


        System.Net.Sockets.TcpClient tcpClient;

        private void dInB_OnChange(bool value)
        {
            if (value)
            {
                this.logger.Trace($"Connecting to {sInA.Value}");

                var hostnamePort = sInA.Value.Split(':');
                this.tcpClient = new System.Net.Sockets.TcpClient();
                this.tcpClient.Client.BeginConnect(
                    new System.Net.IPEndPoint(System.Net.IPAddress.Parse(hostnamePort[0]), Convert.ToInt32(hostnamePort[1])),
                    ConnectCallback,
                    null
                );
            }
            else
            {
                this.tcpClient?.Close();
            }
        }

        void ConnectCallback(IAsyncResult ar) 
        {
            try
            {
                this.logger.Trace("Connected!");
                this.tcpClient.Client.EndConnect(ar);
                (new Thread(ReadThread)).Start();
            }
            catch(Exception ex)
            {
                this.logger.Trace(ex.Message);
            }
        }
        
        void ReadThread()
        {
            this.logger.Trace("Read Thread Started");
            try
            {
                using (var stream = this.tcpClient.GetStream())
                using (var reader = new BinaryReader(stream))
                {
                    while (true)
                    {
                        var value = reader.ReadInt32();
                        this.logger.Trace($"Got Value {value}");
                    }
                }
            }
            catch(Exception ex)
            {
                this.logger.Trace(ex.Message);
            }
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