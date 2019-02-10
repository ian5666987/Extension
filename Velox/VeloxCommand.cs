using System;
using System.Collections.Generic;

using Extension.Developer;

namespace Extension.Velox {
  class VeloxCommand : GenericData {
    protected Dictionary<string, string> satParFormulaDict = new Dictionary<string, string>(); //filled with the parameters and the formula of which this advance parameter influence the basic parameters
    public Dictionary<string, string> SatParFormulaDict { get { return satParFormulaDict; } set { } }

    protected List<string> operationParList = new List<string>(); //filled with operation parameters, used by VELOX-I to translater from this command to parameters using formula
    public List<string> OperationParList { get { return operationParList; } set { } }

    protected List<string> logSatParList = new List<string>();
    public List<string> LogSatParList { get { return logSatParList; } set { } }

    protected List<string> unlogSatParList = new List<string>();
    public List<string> UnlogSatParList { get { return unlogSatParList; } set { } }

    protected int noOfPackets = 0;
    public int NoOfPackets { get { return noOfPackets; } set { noOfPackets = value >= 0 ? value : 0; } }

    //Giving packet name and will trigger giving the subsystem name as well as giving the
    public VeloxCommand(string[] properties = null) : base(properties) { //the properties will first be processed by the generic data
      if (dataName == null || properties == null || properties.Length <= 0) //if name is not properly assigned, return, 
        return;
      try {
        for (int j = 0; j < properties.Length; ++j) {
          if (string.IsNullOrWhiteSpace(properties[j]))
            continue; //cannot process null or white space
          string word = properties[j].Trim(); //every word is a trimmed property
          if (0 == j) { //first word is special, either "name", "satmember", or dataname, "name" and dataname case have been taken cared of by the base calling
            string[] words = word.Split(new char[] { '.' }); //whenever possible try to split word into 2 words, and really, only 2 will be considered here...
            if (words[0] == "satmember" && words.Length > 1 && properties.Length > 1) { //only word cheking if the words are correct (2), and the properties are also having satellite formula correctly (in the properties[1])
              string newMember = words[1];
              string newMemberFormula = properties[1].Trim();
              if (!satParFormulaDict.ContainsKey(newMember)) //Note that only the first formula appeared would be taken!
                satParFormulaDict.Add(newMember, newMemberFormula + ";"); //";" is just to make sure that the formula would be ended
              break; //get out from for loop immediately
            }
          }
          switch (word.ToLower()) { //lowering it when it is read
          case "opmember": //basically another V1 basic properties. Here, it only have the name, the definition could be put elsewhere
            if (operationParList != null)
              if (!operationParList.Contains(word)) //Add if it is new item
                operationParList.Add(properties[++j].Trim());
            break;
          case "logmember": //Log member is to be read only, and can be duplicated! It is to log satPar for others' use
            if (logSatParList != null)
              if (!logSatParList.Contains(word)) //Add if it is new item
                logSatParList.Add(properties[++j].Trim());
            break;
          case "unlogmember": //unlog member is to be read only, and can be duplicated! It is to unlog satPar after use
            if (unlogSatParList != null)
              if (!unlogSatParList.Contains(word)) //Add if it is new item
                unlogSatParList.Add(properties[++j].Trim());
            break;
          default:
            break; //By default, there is nothing!
          }
        }
      } catch (Exception exc) {
        throw exc;
      }
    }

  }
}
