using Crestron.Logos.SplusObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestronModuleCore
{
    public interface ICrestronModule { }
    public interface IMainMethod
    {
        void Main();
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
    public interface IInputOutputFactory
    {
        Input<bool> CreateDigitalInput(string name, Action<bool> onChange);
        Output<bool> CreateDigitalOutput(string name);
        Input<string> CreateStringInput(string name, int maxCapacity, Action<string> onChange);
        Output<string> CreateStringOutput(string name);
        Output<ushort> CreateAnalogOutput(string name);
        Input<ushort> CreateAnalogInput(string name, Action<ushort> onChange);
    }
    internal class DigitalInputWrapper : Input<bool>
    {
        DigitalInput input;

        public DigitalInputWrapper(DigitalInput input)
        {
            this.input = input;
        }

        public bool Value
        {
            get { return this.input.Value == 0 ? false: true; }
        }
    }
    internal class DigitalOutputWrapper: Output<bool>
    {
        DigitalOutput output;

        public DigitalOutputWrapper(DigitalOutput output)
        {
            this.output = output;
        }

        public bool Value
        {
            get { return this.output.Value == 0 ? false : true; }
            set { this.output.Value = value ? 1 : 0; }
        }
    }
    internal class StringInputWrapper : Input<string>
    {
        StringInput input;
        public StringInputWrapper(StringInput input)
        {
            this.input = input;
        }
        public string Value { get => this.input.Value.ToString(); }
    }
    internal class StringOutputWrapper : Output<string>
    {
        StringOutput output;
        public StringOutputWrapper(StringOutput output)
        {
            this.output = output;
        }
        public string Value
        {
            get => this.output.Value.ToString();
            set => this.output.UpdateValue(value);
        }
    }
    internal class AnalogInputWrapper : Input<ushort>
    {
        AnalogInput input;
        public AnalogInputWrapper(AnalogInput input)
        {
            this.input = input;
        }
        public ushort Value { get => this.input.UshortValue; }
    }
    internal class AnalogOutputWrapper : Output<ushort>
    {
        AnalogOutput output;
        public AnalogOutputWrapper(AnalogOutput output)
        {
            this.output = output;
        }
        public ushort Value
        {
            get => this.output.Value;
            set => this.output.Value = value;
        }
    }

    public class CrestronModule : SplusObject, IInputOutputFactory, ICrestronLogger
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
                        .GetConstructor(new Type[] { typeof(IInputOutputFactory), typeof(ICrestronLogger) })
                        .Invoke(new object[] { this, this }) as ICrestronModule;
                }
            }
            catch (Exception ex) { this.ObjectCatchHandler(ex); }
            finally { this.ObjectFinallyHandler(); }
        }

        public override object FunctionMain(object __obj__)
        {
            try
            {
                var ctx = base.SplusFunctionMainStartCode();
                this.Trace("CrestronModule FunctionMain");

                var initializableModule = this.moduleImpl as IMainMethod;
                if (initializableModule != null) initializableModule.Main();
            }
            catch (Exception ex) { this.ObjectCatchHandler(ex); }
            finally { this.ObjectFinallyHandler(); }
            return __obj__;
        }

        protected override void ObjectCatchHandler(Exception e)
        {
            this.Trace("ObjectCatchHandler: {0}", e.Message);
            base.ObjectCatchHandler(e);
        }

        protected override void ObjectFinallyHandler()
        {
            this.Trace("ObjectFinallyHandler");
            base.ObjectFinallyHandler();
        }

        public Input<bool> CreateDigitalInput(string name, Action<bool> onChange)
        {
            var join = (uint)m_DigitalInputList.Count;
            var input = new DigitalInput(join, this);
            this.Trace("CreateDigitalInput {0}", join);
            m_DigitalInputList.Add(join, input);
            if (onChange != null) BindDigitalInput(input, onChange);
            return new DigitalInputWrapper(input);
        }
        public Output<bool> CreateDigitalOutput(string name)
        {
            var join = (uint)m_DigitalOutputList.Count;
            var output = new DigitalOutput(join, this);
            this.Trace("CreateDigitalOutput {0}", join);
            m_DigitalOutputList.Add(join, output);
            return new DigitalOutputWrapper(output);
        }
        private void BindDigitalInput(DigitalInput input, Action<bool> onChange)
        {
            input.OnDigitalChange.Add(new InputChangeHandlerWrapper(o =>
            {
                this.Trace("OnDigitalChange");
                var e = o as SignalEventArgs;
                try
                {
                    var ctx = this.SplusThreadStartCode(e);
                    if (onChange != null) onChange(input.Value == 0 ? false : true);
                }
                catch (Exception ex) { this.ObjectCatchHandler(ex); }
                finally { this.ObjectFinallyHandler(e); }
                return this;
            }));
        }
        public Input<string> CreateStringInput(string name, int maxCapacity, Action<string> onChange)
        {
            var join = (uint)(m_AnalogInputList.Count + m_StringInputList.Count);
            var input = new StringInput(join, maxCapacity, this);
            this.Trace("CreateStringInput {0}", join);
            m_StringInputList.Add(join, input);
            if (onChange != null) BindStringInput(input, onChange);
            return new StringInputWrapper(input);
        }
        public Output<string> CreateStringOutput(string name)
        {
            var join = (uint)(m_AnalogOutputList.Count + m_StringOutputList.Count);
            var output = new StringOutput(join, this);
            this.Trace("CreateStringOutput {0}", join);
            m_StringOutputList.Add(join, output);
            return new StringOutputWrapper(output);
        }
        private void BindStringInput(StringInput input, Action<string> onChange)
        {
            this.Trace("BindStringInput");
            input.OnSerialChange.Add(new InputChangeHandlerWrapper(o =>
            {
                this.Trace("OnSerialChange");
                var e = o as SignalEventArgs;
                try
                {
                    var ctx = this.SplusThreadStartCode(e);
                    if (onChange != null) onChange(input.Value.ToString());
                }
                catch (Exception ex) { this.ObjectCatchHandler(ex); }
                finally { this.ObjectFinallyHandler(e); }
                return this;
            }));
        }
        public Input<ushort> CreateAnalogInput(string name, Action<ushort> onChange)
        {
            var join = (uint)(m_AnalogInputList.Count + m_StringInputList.Count);
            var input = new AnalogInput(join, this);
            this.Trace("CreateAnalogInput {0}", join);
            m_AnalogInputList.Add(join, input);
            if (onChange != null) BindAnalogInput(input, onChange);
            return new AnalogInputWrapper(input);
        }
        public Output<ushort> CreateAnalogOutput(string name)
        {
            var join = (uint)(m_AnalogOutputList.Count + m_StringOutputList.Count);
            var output = new AnalogOutput(join, this);
            this.Trace("CreateAnalogOutput {0}", join);
            m_AnalogOutputList.Add(join, output);
            return new AnalogOutputWrapper(output);
        }
        private void BindAnalogInput(AnalogInput input, Action<ushort> onChange)
        {
            this.Trace("BindAnalogInput");
            input.OnAnalogChange.Add(new InputChangeHandlerWrapper(o =>
            {
                this.Trace("OnAnalogChange");
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
