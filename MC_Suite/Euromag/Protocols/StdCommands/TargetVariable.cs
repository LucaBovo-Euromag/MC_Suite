using Euromag.Utility.Endianness;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    //REV NOTES 
    
    // [LUIS: 15-01-2018] Aggiunto TargetVarible Mancancte Int32  
    // [LUIS: 15-01-2018] Classe  Int32TargetVariable  , WritableInt32TargetVariable
    

    //public enum TargetDataType : byte
    //{
    //    /*
    //    * 0x00 unsigned 8 (intero senza segno a 8bit / 1byte)
    //    * 0x01 unsigned 16 (intero senza segno a 16bit / 2byte)
    //    * 0x02 signed 16 (intero con segno a 16bit / 2byte)
    //    * 0x03 unsigned 32 (intero senza segno a 32bit / 4byte)
    //    * 0x04 signed 32 (intero con segno a 32bit / 4byte)
    //    * 0x05 float 32 (numero in virgola mobile IEE 754 a 32bit / 4byte)
    //    * 0x06 float 64 (numero in virgola mobile IEE 754 a 64bit / 8byte)
    //    * 0x07 string (sequenza di byte codificati come caratteri ASCII)
    //    * 0x08 data (sequenza di byte senza codifica) 
    //    */
    //    uint8       = 0x00,
    //    uint16      = 0x01,
    //    int16       = 0x02,
    //    uint32      = 0x03,
    //    int32       = 0x04,
    //    flt32       = 0x05,
    //    flt64       = 0x06,
    //    chrstr      = 0x07,
    //    data        = 0x08,
    //    placeHolder = 0x09,
    //    enumerator  = 0x0A
    //}

    public enum TargetDataType
    {
        TYPE_UC = 0,
        TYPE_US = 1,
        TYPE_SS = 2,
        TYPE_UL = 3,
        TYPE_SL = 4,
        TYPE_FL = 5,
        TYPE_DB = 6,
        TYPE_STR = 7,
        TYPE_DATA = 8,
        TYPE_PLACE_HOLDER = 9,
        TYPE_ENUM = 10
    };

    public interface ITargetVariable
    {
        String Name
        { get; }

        String ValAsString
        { get; }
        
        TargetDataType DataType
        { get; }
        
        Int32 Size
        { get; }
        
        void Parse(List<Byte> buff);
    }

    public interface ITargetWritable : ITargetVariable
    {        
        new String ValAsString
        { get; set; }
        
        List<Byte> ToList();
    }

    public interface IHasVariable
    {
        ITargetVariable Variable
        { get; set; }

        ICollection<ITargetVariable> AvailableVariables
        {
            get;
        }
    }

    public interface IHasWritableVariable : IHasVariable
    {
        new ITargetWritable Variable
        { get; set; }

        new ICollection<ITargetWritable> AvailableVariables
        {
            get;
        }
    }

    public abstract class TargetVariable<T> : ITargetVariable, INotifyPropertyChanged
    {
        public virtual T Value
        {
            get
            { return _value; }
        }

        public virtual String Name
        {
            get
            {
                return this.ToString();
            }
        }

        public virtual String ValAsString
        { 
            get 
            { return _value.ToString(); }
        }

        public abstract TargetDataType DataType
        { get; }

        public virtual Int32 Size
        {
            get
            {
                return System.Runtime.InteropServices.Marshal.SizeOf(_value);
            }
        }

        public abstract void Parse(List<Byte> buff);

        private T _privValue;
        protected T _value
        {
            get { return _privValue; }
            set 
            {
                _privValue = value;
                OnPropertyChanged("Value");
                OnPropertyChanged("ValAsString");
                OnPropertyChanged("Size");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
    
    public interface WritableTargetVariable<T> : ITargetWritable
    {
        T Value
        {
            get;
            set;
        }
    }

    public class ByteTargetVariable : TargetVariable<Byte>
    {
        public ByteTargetVariable()
        {
            _value = 0;
        }
        
        public override TargetDataType DataType
        { 
            get 
            { return TargetDataType.TYPE_UC; } 
        }
        
        public override void Parse(List<Byte> buff)
        {
            _value = buff[0];
        }

    }

    public class WritableByteTargetVariable : ByteTargetVariable, WritableTargetVariable<Byte>
    {        
        public WritableByteTargetVariable()
            : base()
        {
        }

        public WritableByteTargetVariable(Byte val)
        {
            _value = val;
        }

        public new virtual Byte Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                _value = value;
            }
        }

        public new virtual String ValAsString
        {
            get
            { return base.ValAsString; }
            set
            { _value = Byte.Parse(value); }
        }
        
        public List<Byte> ToList()
        {
            List<Byte> val = new List<Byte>(Size);
            val.Add(_value);
            return val;
        }

    }

    public class UInt32TargetVariable : TargetVariable<UInt32>
    {
        public UInt32TargetVariable()
        {
            _value = 0;
        }

        public override TargetDataType DataType
        {
            get
            { return TargetDataType.TYPE_UL; }
        }

        public override void Parse(List<Byte> buff)
        {
            _value = LEconverter.LEArrayToUInt32(buff.ToArray());
        }

    }

    public class WritableUInt32TargetVariable : UInt32TargetVariable, WritableTargetVariable<UInt32>
    {
        public WritableUInt32TargetVariable()
            : base()
        {
        }

        public WritableUInt32TargetVariable(UInt32 val)
        {
            _value = val;
        }

        public new virtual UInt32 Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                _value = value;
            }
        }

        public new virtual String ValAsString
        {
            get
            { return base.ValAsString; }
            set
            { _value = UInt32.Parse(value); }
        }

        public List<Byte> ToList()
        {
            return new List<Byte>(LEconverter.toLEArray(_value));
        }

    }

    public class UInt16TargetVariable : TargetVariable<UInt16>
    {
        public UInt16TargetVariable()
        {
            _value = 0;
        }

        public override TargetDataType DataType
        {
            get
            { return TargetDataType.TYPE_US; }
        }

        public override void Parse(List<Byte> buff)
        {
            _value = LEconverter.LEArrayToUInt16(buff.ToArray());
        }

    }

    public class WritableUInt16TargetVariable : UInt16TargetVariable, WritableTargetVariable<UInt16>
    {
        public WritableUInt16TargetVariable()
            : base()
        {
        }

        public WritableUInt16TargetVariable(UInt16 val)
        {
            _value = val;
        }

        public new virtual UInt16 Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                _value = value;
            }
        }

        public new virtual String ValAsString
        {
            get
            { return base.ValAsString; }
            set
            { _value = UInt16.Parse(value); }
        }

        public List<Byte> ToList()
        {
            return new List<Byte>(LEconverter.toLEArray(_value));
        }

    }

    public class Int16TargetVariable : TargetVariable<Int16>
    {
        public Int16TargetVariable()
        {
            _value = 0;
        }

        public override TargetDataType DataType
        {
            get
            { return TargetDataType.TYPE_SS; }
        }

        public override void Parse(List<Byte> buff)
        {
            _value = LEconverter.LEArrayToInt16(buff.ToArray());
        }

    }

    public class WritableInt16TargetVariable : Int16TargetVariable, WritableTargetVariable<Int16>
    {
        public WritableInt16TargetVariable()
            : base()
        {
        }

        public WritableInt16TargetVariable(Int16 val)
        {
            _value = val;
        }

        public new virtual Int16 Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                _value = value;
            }
        }

        public new virtual String ValAsString
        {
            get
            { return base.ValAsString; }
            set
            { _value = Int16.Parse(value); }
        }

        public List<Byte> ToList()
        {
            return new List<Byte>(LEconverter.toLEArray(_value));
        }

    }

    public class Int32TargetVariable : TargetVariable<Int32>
    {
        public Int32TargetVariable()
        {
            _value = 0;
        }

        public override TargetDataType DataType
        {
            get
            {
                return TargetDataType.TYPE_SL;
            }
        }

        public override void Parse(List<Byte> buff)
        {
            _value = LEconverter.LEArrayToInt32(buff.ToArray());
        }

    }

    public class WritableInt32TargetVariable : Int32TargetVariable, WritableTargetVariable<Int32>
    {
        public WritableInt32TargetVariable()
            : base()
        {
        }

        public WritableInt32TargetVariable(Int32 val)
        {
            _value = val;
        }

        public new virtual Int32 Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                _value = value;
            }
        }

        public new virtual String ValAsString
        {
            get
            {
                return base.ValAsString;
            }
            set
            {
                _value = Int32.Parse(value);
            }
        }

        public List<Byte> ToList()
        {
            return new List<Byte>(LEconverter.toLEArray(_value));
        }

    }
   
    
    public class FloatTargetVariable : TargetVariable<float>
    {
        public FloatTargetVariable()
        {
            _value = 0;
        }

        public override TargetDataType DataType
        {
            get
            { return TargetDataType.TYPE_FL; }
        }

        public override void Parse(List<Byte> buff)
        {
            _value = LEconverter.LEArrayToSingle(buff.ToArray());
        }

    }

    public class WritableFloatTargetVariable : FloatTargetVariable, WritableTargetVariable<float>
    {
        public WritableFloatTargetVariable()
            : base()
        {
        }

        public WritableFloatTargetVariable(float val)
        {
            _value = val;
        }

        public new virtual float Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                _value = value;
            }
        }

        public new virtual String ValAsString
        {
            get
            { return base.ValAsString; }
            set
            { _value = float.Parse(value); }
        }

        public List<Byte> ToList()
        {
            return new List<Byte>(LEconverter.toLEArray(_value));
        }

    }

    public class DoubleTargetVariable : TargetVariable<double>
    {
        public DoubleTargetVariable()
        {
            _value = 0;
        }

        public override TargetDataType DataType
        {
            get
            { return TargetDataType.TYPE_DB; }
        }

        public override void Parse(List<Byte> buff)
        {
            _value = LEconverter.LEArrayToDouble(buff.ToArray());
        }

    }

    public class WritableDoubleTargetVariable : DoubleTargetVariable, WritableTargetVariable<double>
    {
        public WritableDoubleTargetVariable()
            : base()
        {
        }

        public WritableDoubleTargetVariable(double val)
        {
            _value = val;
        }

        public new virtual double Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                _value = value;
            }
        }

        public new virtual String ValAsString
        {
            get
            { return base.ValAsString; }
            set
            { _value = double.Parse(value); }
        }

        public List<Byte> ToList()
        {
            return new List<Byte>(LEconverter.toLEArray(_value));
        }

    }

    public class StringTargetVariable : TargetVariable<String>
    {
        public StringTargetVariable()
        {
            _value = String.Empty;
        }

        public override TargetDataType DataType
        {
            get
            { return TargetDataType.TYPE_STR; }
        }

        public override Int32 Size
        {
            get
            {
                return ((String)_value).Length;
            }
        }

        public override String ValAsString
        {
            get
            {
                char[] trimmedChars = {'\0' };
                return _value.Trim(trimmedChars); 
            }
        }

        public override void Parse(List<Byte> buff)
        {
            String str = String.Empty;

            foreach (Byte b in buff)
            {
                if (b == 0)
                    break;
                str += (char)b;
            }

            _value = str;
        }

    }

    public class WritableStringTargetVariable : StringTargetVariable, WritableTargetVariable<String>
    {
        public WritableStringTargetVariable()
            : base()
        {
        }

        public WritableStringTargetVariable(String val)
        {
            _value = val;
        }

        public new virtual String Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                _value = value;
            }
        }

        public new virtual String ValAsString
        {
            get
            { return base.ValAsString; }
            set
            { _value = value; }
        }

        public List<Byte> ToList()
        {
            List<Byte> buff = new List<Byte>();
            
            foreach (char c in _value)
                buff.Add((Byte)c);

            while (buff.Count < this.Size)
                buff.Add(0);

            return buff;
        }

    }

    public class DataTargetVariable : TargetVariable<List<Byte>>
    {
        public DataTargetVariable()
        {
            _value = new List<Byte>();
        }

        public override TargetDataType DataType
        {
            get
            { return TargetDataType.TYPE_DATA; }
        }

        public override Int32 Size
        {
            get
            { return _value.Count; }
        }

        public override String ValAsString
        {
            get
            {
                String ret = String.Empty;
                foreach (Byte b in _value)
                    ret += "0x" + b.ToString("X2") + ", ";

                if (ret != String.Empty)
                {
                    int idx = ret.LastIndexOf(',');
                    return ret.Remove(idx).Trim();
                }

                return String.Empty;
            }
        }

        public override void Parse(List<Byte> buff)
        {
            _value = new List<Byte>(buff);
        }

    }

    public class WritableDataTargetVariable : DataTargetVariable, WritableTargetVariable<List<Byte>>
    {
        public WritableDataTargetVariable()
            : base()
        {
        }

        public WritableDataTargetVariable(List<Byte> val)
        {
            _value = val;
        }

        public new virtual List<Byte> Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                _value = value;
            }
        }

        public new virtual String ValAsString
        {
            get
            { return base.ValAsString; }
            set
            { 
                //TODO: fix this
                throw new NotImplementedException();
            }
        }

        public List<Byte> ToList()
        {
            return new List<Byte>(_value);
        }

    }

    public class EnumTargetVariable : ByteTargetVariable
    {
        public override TargetDataType DataType
        {
            get
            { return TargetDataType.TYPE_ENUM; }
        }

        public override string ValAsString
        {
            get
            {
                if (Options.ContainsKey(_value))
                {
                    return Options[_value];
                }
                else
                    return base.ValAsString;
            }
        }

        public virtual IDictionary<int, String> Options
        { 
            get { return new Dictionary<int, string>(); }
        }
    }

    public class WritableEnumTargetVariable : WritableByteTargetVariable
    {
        public WritableEnumTargetVariable()
            : base()
        {
        }

        public WritableEnumTargetVariable(Byte val)
            : base(val)
        {
            
        }

        public override TargetDataType DataType
        {
            get
            { return TargetDataType.TYPE_ENUM; }
        }

        public override string ValAsString
        {
            get
            {
                if (Options.ContainsKey(_value))
                {
                    return Options[_value];
                }
                else
                    return base.ValAsString;
            }
            set
            {
                try
                {
                    var item = Options.First( kvPair => kvPair.Value == value );
                    _value = (Byte)item.Key;
                }
                catch (InvalidOperationException)
                {
                    base.ValAsString = value;
                }
            }
        }

        public virtual IDictionary<int, String> Options
        {
            get { return new Dictionary<int, string>(); }
        }
    }


}
