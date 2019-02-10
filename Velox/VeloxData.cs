using System;
using Extension.Developer;

namespace Extension.Velox {
  class VeloxData : GenericData {
    protected bool isPacketOffset;
    public bool IsPacketOffset { get { return isPacketOffset; } set { } }

    protected bool isHeaderOrTail;
    public bool IsHeaderOrTail { get { return isHeaderOrTail; } set { } }

    protected bool isSubsystemAddress;
    public bool IsSubsystemAddress { get { return isSubsystemAddress; } set { } }

    protected string packetName; //setable
    public string PacketName { get { return packetName; } set { packetName = value; } }

    protected int xdataOffset; //setable
    public int XdataOffset { get { return xdataOffset; } set { xdataOffset = value; } }

    protected int xdataAbsolutePosition; //setable
    public int XdataAbsolutePosition { get { return xdataAbsolutePosition; } set { xdataAbsolutePosition = value; } }

    protected string subsystem; //setable
    public string Subsystem { get { return subsystem; } set { subsystem = value; } }

    //Giving packet name and will trigger giving the subsystem name as well as giving the
    public VeloxData(string packetname = null, int packetoffset = 0, int xdataoffset = 0, string[] properties = null) : base(properties) { //the properties will first be processed by the generic data
      if (dataName == null || properties == null || properties.Length <= 0) //if name is not properly assigned, return, 
        return;
      try {
        string[] words = properties[0].Trim().Split(new char[] { '.' });
        isPacketOffset = words[0].StartsWith("XDATA_"); //It means, this is a packet offset
        isHeaderOrTail = words[0].StartsWith("XData_"); //It means, this is a header or a tail
        isSubsystemAddress = words[0].Length > 8 && words[0].EndsWith("_Address"); //It means, this is a subsystem address
        packetName = packetname; //either having packet name as null or as something
        if (packetname == null || !packetName.StartsWith("XDATA_")) //The subsystem name can be derived from the packet name
          return;
        string[] packetwords = packetname.Split(new char[] { '_' });
        if (packetwords == null || packetwords.Length <= 1)
          return;
        subsystem = packetwords[1]; //assign the subsystem name automatically
        xdataOffset = xdataoffset; //The XDATA offset is now set from outside
        xdataAbsolutePosition = packetoffset + xdataoffset; //The data type will affect the coming velox data, but must be taken cared of outside!
      } catch (Exception exc) {
        throw exc;
      }
    }

    //The injection process is meant to be called over and over while each data from a packet.properties is called! Not sure if it is the best idea by introducing new variable
    //public virtual void SetPacketProperties() { //to tell what is the subsystem of this data, the packet offset
      //if (isSubsystemAddress) //if it is subsystem address, update the subsystem name according to the data name, only data name which is subsystem can give the subsystem name. If data 
      //  advProp.Subsystem = dataName.Substring(0, dataName.Length - 8);
      //if (!isPacketOffset && !isHeaderOrTail && !isSubsystemAddress) {
      //  subsystem = advProp.Subsystem;
      //  advProp.XdataOffset += advProp.LastDataSize;
      //  advProp.LastDataSize = DataManipulator.GetDataSizeByType(dataType); //This must be taken from the previous data
      //}
      //packetName = advProp.PacketName;
      //xdataOffset = advProp.XdataOffset;
    //}

  }
}
