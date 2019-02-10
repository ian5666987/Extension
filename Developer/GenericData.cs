using System;
using System.Collections.Generic;

using Extension.Checker;
using Extension.Manipulator;

namespace Extension.Developer {
  public class GenericData {
    protected List<TextType> textTypeList = new List<TextType>(); //provided such that there is no need to check everytime
    public List<TextType> TextTypeList { get { return textTypeList; } set { } }

    protected string dataName; //can either be put as the first word or be indicated by the keyword "name". Ex: OBDH_Time OR name=OBDH_Time
    public string DataName { get { return dataName; } set { } }

    protected string dataType; //mergable
    public string DataType { get { return dataType; } set { } }

    protected string dataUnit; //mergable
    public string DataUnit { get { return dataUnit; } set { } }

    protected string dataPolicy; //mergable
    public string DataPolicy { get { return dataPolicy; } set { } }

    protected string dataFormula; //mergable
    public string DataFormula { get { return dataFormula; } set { } }

    protected double dataMin = double.MinValue; //mergable
    public double DataMin { get { return dataMin; } set { } }

    protected double dataMax = double.MaxValue; //mergable
    public double DataMax { get { return dataMax; } set { } }

    protected string dataValue; //mergable //used to store the default data value - NOT setable, work together with "auto" policy
    public string DataValue { get { return dataValue; } set { } }

    protected bool isWriteDataNormal; //readonly from outside, only to tell if read data normal, triggered together with data buffer changed. No point in checking if the DataReadBufferChanged is not assigned
    public bool IsWriteDataNormal { get { return isWriteDataNormal; } }

    protected string dataWriteBuffer = null; //mergable, setable //used to store the last write to be, previously called dataBackBuffer logValue
    public string DataWriteBuffer { 
      get { return dataWriteBuffer; } 
      set { 
        dataWriteBuffer = value;
        if (DataWriteBufferChanged == null) //does not have event handler for this case
          return;
        processDataValidity(value, ref isWriteDataNormal);
        DataWriteBufferChanged(this, new EventArgs());
        //TODO to consider to put the write data consideration: whether it is normal or not normal
        //Difficulty here is this: what will determine the color of the control when read & write is together chosen?
        // 1) if both are right (or 1 is right the other is unitialized): then it is normal color
        // 2) if both are wrong (or 1 is wrong the other is unitialized): then it is abnormal color
        // 3) if both are unitialized: then it is unitialized color
        // 4) If one is right and another is wrong, it is mixed color
      } 
    }

    protected List<string> bitsList; //mergable
    public List<string> BitsList { get { return bitsList; } set { } }

    protected List<string> comboList; //mergable
    public List<string> ComboList { get { return comboList; } set { } }

    protected List<string> valueList; //mergeable
    public List<string> ValueList { get { return valueList; } set { } }

    protected string dataInfo; //setable
    public string DataInfo { get { return dataInfo; } set { dataInfo = value; } }

    protected int maxDataLength = Text.DEFAULT_MAX_TEXT_LENGTH;
    public int MaxDataLength { get { return maxDataLength; } set { } }

    protected bool isReadDataNormal; //readonly from outside, only to tell if read data normal, triggered together with data buffer changed. No point in checking if the DataReadBufferChanged is not assigned
    public bool IsReadDataNormal { get { return isReadDataNormal; } }

    protected string dataReadBuffer; //setable used to store last read data, previously called logValue
    public string DataReadBuffer {
      get { return dataReadBuffer; }
      set {
        dataReadBuffer = value;
        if (DataReadBufferChanged == null) //does not have event handler for this case
          return;
        processDataValidity(value, ref isReadDataNormal); //if we want (for some reason like initialization) to process data validity without actual changing of the buffer, special method is provided
        DataReadBufferChanged(this, new EventArgs()); //changed can mean null dataReadBuffer (needs to be taken cared of), important thing: e = null means NOT a new data, when the data buffer is changed here, it is always considered a new data
      }
    }

    protected string dependentUpon; //mergeable //means that this data can only be filled if some other data is filled
    public string DependentUpon { get { return dependentUpon; } set { } }

    protected string derivedFrom; //mergeable //to say that this data is derived directly from other data
    public string DerivedFrom { get { return derivedFrom; } set { } }

    protected string attachedTo; //mergeable //when the data this data is attached to is filled, this must also be filled
    public string AttachedTo { get { return attachedTo; } set { } }

    protected string excludedFrom; //mergeable //when the data this data is excluded to is filled, this data cannot be filled
    public string ExcludedFrom { get { return excludedFrom; } set { } }

    public event EventHandler DataReadBufferChanged; //more important than DataWriteBufferChanged    
    public event EventHandler DataWriteBufferChanged;

    private void processDataValidity(string value, ref bool validityFlag) {
      validityFlag = false; //until proven otherwise
      if (value == null)
        return; //wrong from the beginning

      //value list dependent
      if (valueList != null && valueList.Count > 0) {
        if (comboList != null && comboList.Count > 0 && valueList.Count == comboList.Count) { //combo list case
          for (int i = 0; i < valueList.Count; ++i)
            if (Text.StringsHaveTheSameValue(value, valueList[i], dataType)) { //found
              validityFlag = true;
              break;
            }
        } else if (bitsList != null && bitsList.Count > 0 && valueList.Count == bitsList.Count) { //bits list case
          try {
						uint uintVal = Text.IsHex(value) ? Data.HexString0xToUint(value) : Convert.ToUInt32(value);
            validityFlag = uintVal >= DataMin && uintVal <= DataMax;
          } catch {
          }
        }
      } else { //non value-list dependent
        //TODO consider "TEXT" text type, "attachment", quoted text, etc...
        //if (TextCheckerExtension.IsQuotedText(v1prop.LogValue)) //To anticipate quotedText type log!!
        //  loggedStr = DataManipulator.ConvertQoutedTextToUIntStr(v1prop.LogValue);
        try { //integer data
          double msgDoubleVal = Data.ConvertTextTypeListedStringToDouble(value, DataType, TextTypeList, isJava: true, isUtc: true);
          validityFlag = msgDoubleVal <= DataMax && msgDoubleVal >= DataMin;
        } catch { //unprocessed exception, may come due failure to process text typed data
        }
      }
    }

    public void Invalidate() {
      processDataValidity(dataReadBuffer, ref isReadDataNormal);
      processDataValidity(dataWriteBuffer, ref isWriteDataNormal);
    }

    //There are few types of inputs which we could thought of:
    // (1) dataname.combo = [bla01=0, bla02=5, bla03=77]; -> the words are before comma and after comma, only two words
    // (2) dataname =type=uint16;min=0;max=20000;unit=mA; -> there are many words involved in it, but the base rule is: the next word must be the value for what the previous word is saying
    // (3) name=dataname=type=uint16;min=0;max=20000;unit=mA; -> alternative way of putting (2)
    //What is not possible here is such as
    // (1) name=dataname.combo = [bla01=0, bla02=5, bla03=77];
    public GenericData(string[] properties = null) { //either the input is collection of words or the input is single line, but single line is not generic enough
      if (properties == null || properties.Length <= 0)
        return;
      try {
        for (int j = 0; j < properties.Length; ++j) {
          if (string.IsNullOrWhiteSpace(properties[j]))
            continue; //cannot process null or white space
          string word = properties[j].Trim(); //every word is a trimmed property
          
          if (0 == j) { //first word is special, either "name" or dataname
            string[] words = word.Split(new char[] { '.' }); //whenever possible try to split word into 2 words, and really, only 2 will be considered here...
            dataName = words[0] != "name" && string.IsNullOrWhiteSpace(dataName) ? words[0] : null; //it is possibly a name for the first word without qualifier "name" and if there is no name, then uses the words[0] as the name. By this, we are going to continue to the next word if things cannot be split further
            if (words.Length > 1 && dataName == words[0]) { //it is worth to check for the second word, only if it is there in the first place and that the first word is the name of the data
              string[] wordsInsideBracket = properties[1].Trim().Split(new char[] { ',' }); //it is a must that the values defined in the properties are split based on comma: bla01=1, bla02=2 etc...
              for (int i = 0; i < wordsInsideBracket.Length; ++i)
                wordsInsideBracket[i] = wordsInsideBracket[i].Trim();
              switch (words[1]) {
              case "combo": //clearly, that the combo and bits are not separated with its actual values
                if (comboList == null)
                  comboList = new List<string>();
                comboList.Clear(); //Edit combo or bits list with the latest read
                if (valueList == null)
                  valueList = new List<string>();
                valueList.Clear(); //The values will follow
                for (int k = 0; k < wordsInsideBracket.Length; ++k){
                  string[] comboNameAndValue = wordsInsideBracket[k].Split(new char[] { '=' });
                  if (comboNameAndValue.Length != 2) //fail case
                    continue;
                  comboNameAndValue[0] = comboNameAndValue[0].Trim();
                  comboNameAndValue[1] = comboNameAndValue[1].Trim();                    
                  comboList.Add(comboNameAndValue[0]);
                  valueList.Add(comboNameAndValue[1]);
                }
                break;
              case "bits":
                if (bitsList == null)
                  bitsList = new List<string>();
                bitsList.Clear();
                if (valueList == null)
                  valueList = new List<string>();
                valueList.Clear(); //The values will follow
                for (int k = 0; k < wordsInsideBracket.Length; ++k){
                  string[] bitsNameAndValue = wordsInsideBracket[k].Split(new char[] { '=' });
                  if (bitsNameAndValue.Length != 2) //fail case
                    continue;
                  bitsNameAndValue[0] = bitsNameAndValue[0].Trim();
                  bitsNameAndValue[1] = bitsNameAndValue[1].Trim();
                  bitsList.Add(bitsNameAndValue[0]);
                  valueList.Add(bitsNameAndValue[1]);
                }
                break;
              default:
                break;
              }
              break; //in the case of adding bits or combo, the loop will be terminated here!
            }
          }

          switch (word.ToLower()) { //lowering it when it is read
          case "name": //we can input name here! at least to make life easier...
            dataName = properties[++j].Trim(); //if this fails, what is going to happen to name?
            break;
          case "value":
            dataValue = properties[++j].Trim();
            break;
          case "type":
            dataType = properties[++j].Trim();
            dataMin = Data.GetDataLowerLimitByType(dataType);
            dataMax = Data.GetDataUpperLimitByType(dataType);
            break;
          case "unit":
            dataUnit = properties[++j].Trim();
            break;
          case "min":
            try {
              dataMin = Convert.ToDouble(properties[++j].Trim());
            } catch {
              dataMin = double.MinValue; //very large negative value
            }
            break;
          case "max":
            try {
              dataMax = Convert.ToDouble(properties[++j].Trim());
            } catch {
              dataMax = double.MaxValue; //very large value
            }
            break;
          case "policy":
            dataPolicy = properties[++j].Trim();
            break;
          case "formula":
            dataFormula = properties[++j].Trim();
            break;
          case "dependentupon":
            dependentUpon = properties[++j].Trim();
            break;
          case "derivedfrom":
            derivedFrom = properties[++j].Trim();
            break;
          case "attachedto":
            attachedTo = properties[++j].Trim();
            break;
          case "excludedfrom":
            excludedFrom = properties[++j].Trim();
            break;
          default: //for something that it cannot process, it won't process, but leave it to the other data structure to process it
            break; //By default, there is nothing!
          }
        } //end of for loop

        if (textTypeList.Count <= 0) // if uninitialized
          textTypeList = Text.GetTextTypeList(dataUnit, dataType);
				if (maxDataLength == Text.DEFAULT_MAX_TEXT_LENGTH) // if uninitialized
					maxDataLength = Text.GetMaxDataLength(dataUnit, dataType);

      } catch (Exception exc) {
        throw exc; //the listener should be implemented outside, by using logBox, not inside
      }
    }

    public virtual void CombineAndOverride(GenericData newdata) { //Not that elegant..
      if (!string.IsNullOrWhiteSpace(newdata.dataType))
        this.dataType = newdata.dataType;
      if (!string.IsNullOrWhiteSpace(newdata.dataUnit))
        this.dataUnit = newdata.dataUnit;
      if (!string.IsNullOrWhiteSpace(newdata.dataPolicy))
        this.dataPolicy = newdata.dataPolicy;
      if (!string.IsNullOrWhiteSpace(newdata.dataFormula))
        this.dataFormula = newdata.dataFormula;
      if (!string.IsNullOrWhiteSpace(newdata.dataValue))
        this.dataValue = newdata.dataValue;
      if (!string.IsNullOrWhiteSpace(newdata.dataWriteBuffer))
        this.dataWriteBuffer = newdata.dataWriteBuffer;
      if (newdata.dataMin != double.MinValue)
        this.dataMin = newdata.dataMin;
      if (newdata.dataMax != double.MaxValue)
        this.dataMax = newdata.dataMax;
      if (newdata.bitsList != null && newdata.bitsList.Count > 0)
          this.bitsList = newdata.bitsList;
      if (newdata.comboList != null && newdata.comboList.Count > 0)
          this.comboList = newdata.comboList;
      if (newdata.valueList != null && newdata.valueList.Count > 0)
        this.valueList = newdata.valueList;
      if (!string.IsNullOrWhiteSpace(newdata.dependentUpon))
        this.dependentUpon = newdata.dependentUpon;
      if (!string.IsNullOrWhiteSpace(newdata.derivedFrom))
        this.derivedFrom = newdata.derivedFrom;
      if (!string.IsNullOrWhiteSpace(newdata.attachedTo))
        this.attachedTo = newdata.attachedTo;
      if (!string.IsNullOrWhiteSpace(newdata.excludedFrom))
        this.excludedFrom = newdata.excludedFrom;
    }
  }
}

    //protected bool isPacketOffset; //Special property of VELOX, should be removed
    //public bool IsPacketOffset { get { return isPacketOffset; } set { } }

    //protected bool isHeaderOrTail; //Special property of VELOX, should be removed
    //public bool IsHeaderOrTail { get { return isHeaderOrTail; } set { } }

    //protected bool isSubsystemAddress; //Special property of VELOX, should be removed
    //public bool IsSubsystemAddress { get { return isSubsystemAddress; } set { } }

    //protected string packetName; //setable //Special property of VELOX, should be removed
    //public string PacketName { get { return packetName; } set { packetName = value; } }

    //protected int xdataOffset; //setable //Special property of VELOX, should be removed
    //public int XdataOffset { get { return xdataOffset; } set { xdataOffset = value; } }

    //protected int xdataAbsolutePosition; //setable //Special property of VELOX, should be removed
    //public int XdataAbsolutePosition { get { return xdataAbsolutePosition; } set { xdataAbsolutePosition = value; } }

    //protected string subsystem; //setable //Special property of VELOX, should be removed
    //public string Subsystem { get { return subsystem; } set { subsystem = value; } }

//public virtual void InjectProperties(string[] propWords) { //overrided from its parent
//  bool completeFlag = false;
//  bool isThisName = false;
//  try {
//    for (int j = 0; j < propWords.Length; ++j) {
//      string wordStr = propWords[j].Trim();
//      char[] delimiterChars = { '.' }; //try to split further, if there is any name involved
//      string[] words = wordStr.Split(delimiterChars);
//      string nameword = words[0];
//      if (0 == j && nameword != "name") { //it is possibly a name for the first word
//        if (string.IsNullOrWhiteSpace(dataName)) //If there is no name, then uses the propWord as the name
//          dataName = nameword;
//        if (dataName == nameword)
//          isThisName = true; //it is this name

//        if (words.Length > 1 && isThisName) { //the case if this is name, we can add combo boxes or bits
//          string strInsideBracket = propWords[1].Trim(); //This is a must!
//          char[] delimiterCharsInsideBracket = { ',' };
//          string[] wordsInsideBracket = strInsideBracket.Split(delimiterCharsInsideBracket);
//          for (int i = 0; i < wordsInsideBracket.Length; ++i)
//            wordsInsideBracket[i] = wordsInsideBracket[i].Trim();
//          switch (words[1]) {
//          case "combo":
//            comboList.Clear(); //Edit combo or bits list with the latest read
//            comboList.AddRange(wordsInsideBracket);
//            break;
//          case "bits":
//            bitsList.Clear();
//            bitsList.AddRange(wordsInsideBracket);
//            break;
//          default:
//            break;
//          }
//          completeFlag = true; //raised when we read till the end!
//        }
//      }

//      if (!completeFlag && !isThisName) { //other than names or member, only access if completeFlag is not raised, 
//        switch (wordStr.ToLower()) { //lowering it when it is read
//        case "name": //we can input name here! at least to make life easier...
//          dataName = propWords[++j].Trim();
//          break;
//        case "value":
//          dataValue = propWords[++j].Trim();
//          break;
//        case "type":
//          dataType = propWords[++j].Trim();
//          dataMin = DataManipulator.GetDataLowerLimitByType(dataType);
//          dataMax = DataManipulator.GetDataUpperLimitByType(dataType);
//          break;
//        case "unit":
//          dataUnit = propWords[++j].Trim();
//          break;
//        case "min":
//          try {
//            dataMin = Convert.ToDouble(propWords[++j].Trim());
//          } catch {
//            dataMin = DataManipulator.MIN_VAL; //very large value
//          }
//          break;
//        case "max":
//          try {
//            dataMax = Convert.ToDouble(propWords[++j].Trim());
//          } catch {
//            dataMax = DataManipulator.MAX_VAL; //very large value
//          }
//          break;
//        case "policy":
//          dataPolicy = propWords[++j].Trim();
//          break;
//        case "formula":
//          dataFormula = propWords[++j].Trim();
//          break;
//        default:
//          break; //By default, there is nothing!
//        }
//      }
//    }
//    if (textTypeList != null)
//      if (textTypeList.Count <= 0)
//        textTypeList = TextCheckerExtension.GetTextTypeList(dataUnit, dataType);
//    if (maxDataLength == TextCheckerExtension.DEFAULT_MAX_TEXT_LENGTH)
//      maxDataLength = TextCheckerExtension.GetMaxDataLength(dataUnit, dataType);
//  } catch (Exception exc) {
//    throw exc; //the listener should be implemented outside, by using logBox, not inside
//  }
//}


