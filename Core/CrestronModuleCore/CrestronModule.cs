using Crestron.Logos.SplusObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCrestronModule
{
    public interface IInitializable
    {
        void Initialize();
    }
    
    public interface ICrestronModule { }

    public interface ICrestronModuleBuilder
    {
        Input<int> CreateDigitalInput(string name, Action<int> onChange);
        Output<int> CreateDigitalOutput(string name);
    }

    public interface ICrestronLogger
    {
        void Trace(string format, params object[] args);
        void Print(string format, params object[] args);
    }

    public interface Input<T>
    {
        T Value { get; }
    }

    public interface Output<T>
    {
        T Value { get; set; }
    }

    internal class DigitalInputWrapper : Input<int>
    {
        DigitalInput input;

        public DigitalInputWrapper(DigitalInput input)
        {
            this.input = input;
        }

        public int Value
        {
            get { return this.input.Value; }
        }
    }

    internal class DigitalOutputWrapper: Output<int>
    {
        DigitalOutput output;

        public DigitalOutputWrapper(DigitalOutput output)
        {
            this.output = output;
        }

        public int Value
        {
            get { return this.output.Value; }
            set { this.output.Value = value; }
        }
    }

    public class CrestronModule : SplusObject, ICrestronModuleBuilder, ICrestronLogger
    {
        ICrestronModule moduleImpl;

        public CrestronModule(
            string InstanceName,
            string ReferenceID,
            CrestronStringEncoding nEncodingType)
            : base(InstanceName, ReferenceID, nEncodingType) { }

        public override void LogosSplusInitialize()
        {
            try
            {
                this.Trace("CrestronModule LogosSplusInitialize");
                var moduleType = this.GetType()
                    .Assembly.GetTypes()
                    .FirstOrDefault(t => typeof(ICrestronModule).IsAssignableFrom(t) && t.IsClass);

                if (moduleType != null)
                {
                    this.Trace("Module found: {0}", moduleType.Name);
                    this.moduleImpl = moduleType
                        .GetConstructor(new Type[] { typeof(ICrestronModuleBuilder), typeof(ICrestronLogger) })
                        .Invoke(new object[] { this, this }) as ICrestronModule;
                }
            }
            catch (Exception ex) { base.ObjectCatchHandler(ex); }
            finally { base.ObjectFinallyHandler(); }
        }

        public override object FunctionMain(object __obj__)
        {
            try
            {
                var ctx = base.SplusFunctionMainStartCode();
                this.Trace("CrestronModule FunctionMain");

                var initializableModule = this.moduleImpl as IInitializable;
                if (initializableModule != null) initializableModule.Initialize();
            }
            catch (Exception ex) { base.ObjectCatchHandler(ex); }
            finally { base.ObjectFinallyHandler(); }
            return __obj__;
        }

        public Input<int> CreateDigitalInput(string name, Action<int> onChange)
        {
            var join = (uint)m_DigitalInputList.Count;
            var input = new DigitalInput(join, this);
            m_DigitalInputList.Add(join, input);
            if (onChange != null) BindDigitalInput(input, onChange);
            return new DigitalInputWrapper(input);
        }
        public Output<int> CreateDigitalOutput(string name)
        {
            var join = (uint)m_DigitalOutputList.Count;
            var output = new DigitalOutput(join, this);
            m_DigitalOutputList.Add(join, output);
            return new DigitalOutputWrapper(output);
        }
        public void BindDigitalInput(DigitalInput input, Action<int> onChange)
        {
            input.OnDigitalChange.Add(new InputChangeHandlerWrapper(o =>
            {
                var e = o as SignalEventArgs;
                try
                {
                    var ctx = this.SplusThreadStartCode(e);
                    if (onChange != null) onChange(input.Value);
                }
                catch (Exception ex) { this.ObjectCatchHandler(ex); }
                finally { this.ObjectFinallyHandler(e); }
                return this;
            }));
        }
    }
}
