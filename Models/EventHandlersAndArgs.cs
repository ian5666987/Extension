using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extension.Models {
  public class ObjectEventArgs : EventArgs {
    public object Value { get; private set; }

    public ObjectEventArgs(object value) {
      Value = value;
    }
  }

  public class SByteEventArgs : EventArgs {
    public sbyte Value { get; private set; }

    public SByteEventArgs(sbyte value) {
      Value = value;
    }
  }

  public class ByteEventArgs : EventArgs {
    public byte Value { get; private set; }

    public ByteEventArgs(byte value) {
      Value = value;
    }
  }

  public class ByteArrayEventArgs : EventArgs {
    public byte[] Value { get; private set; }

    public ByteArrayEventArgs(byte[] value) {
      Value = value;
    }
  }

  public class ShortEventArgs : EventArgs {
    public short Value { get; private set; }

    public ShortEventArgs(short value) {
      Value = value;
    }
  }

  public class UShortEventArgs : EventArgs {
    public ushort Value { get; private set; }

    public UShortEventArgs(ushort value) {
      Value = value;
    }
  }

  public class IntEventArgs : EventArgs {
    public int Value { get; private set; }

    public IntEventArgs(int value) {
      Value = value;
    }
  }

  public class UIntEventArgs : EventArgs {
    public uint Value { get; private set; }

    public UIntEventArgs(uint value) {
      Value = value;
    }
  }

  public class LongEventArgs : EventArgs {
    public long Value { get; private set; }

    public LongEventArgs(long value) {
      Value = value;
    }
  }

  public class ULongEventArgs : EventArgs {
    public ulong Value { get; private set; }

    public ULongEventArgs(ulong value) {
      Value = value;
    }
  }

  public class FloatEventArgs : EventArgs {
    public float Value { get; private set; }

    public FloatEventArgs(float value) {
      Value = value;
    }
  }

  public class DoubleEventArgs : EventArgs {
    public double Value { get; private set; }

    public DoubleEventArgs(double value) {
      Value = value;
    }
  }

  public class DecimalEventArgs : EventArgs {
    public decimal Value { get; private set; }

    public DecimalEventArgs(decimal value) {
      Value = value;
    }
  }

  public class StringEventArgs : EventArgs {
    public string Value { get; private set; }

    public StringEventArgs(string value) {
      Value = value;
    }
  }

  public class MessageEventArgs : EventArgs {
    public string Message { get; private set; }
    public bool IsError { get; private set; }

    public MessageEventArgs(string message, bool isError = false) {
      Message = message;
      IsError = isError;
    }
  }

  public class BooleanEventArgs : EventArgs {
    public bool Value { get; private set; }

    public BooleanEventArgs(bool value) {
      Value = value;
    }
  }

  public delegate void ObjectEventHandler(object source, ObjectEventArgs e);
  public delegate void SByteEventHandler(object source, SByteEventArgs e);
  public delegate void ByteEventHandler(object source, ByteEventArgs e);
  public delegate void ByteArrayEventHandler(object source, ByteArrayEventArgs e);
  public delegate void ShortEventHandler(object source, ShortEventArgs e);
  public delegate void UShortEventHandler(object source, UShortEventArgs e);
  public delegate void IntEventHandler(object source, IntEventArgs e);
  public delegate void UIntEventHandler(object source, UIntEventArgs e);
  public delegate void LongEventHandler(object source, LongEventArgs e);
  public delegate void ULongEventHandler(object source, ULongEventArgs e);
  public delegate void FloatEventHandler(object source, FloatEventArgs e);
  public delegate void DoubleEventHandler(object source, DoubleEventArgs e);
  public delegate void DecimalEventHandler(object source, DecimalEventArgs e);
  public delegate void StringEventHandler(object source, StringEventArgs e);
  public delegate void MessageEventHandler(object source, MessageEventArgs e);
  public delegate void BooleanEventHandler(object source, BooleanEventArgs e);
}
