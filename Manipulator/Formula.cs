using System;
using System.Collections.Generic; //used
using System.Globalization; //useds

using Extension.Checker;
using Extension.Reader;

namespace Extension.Manipulator
{
  public class Formula
  {
    public static List<Dictionary<string, string>> VarDictList = new List<Dictionary<string, string>>();
		
    public static double ApplyDataFormula(double origData, string dataFormula) {
			
      double formData = origData;
      try {
        if (!string.IsNullOrWhiteSpace(dataFormula) && dataFormula.Length >= 2) //A formula cannot have less than two characters
          if (dataFormula.Trim().ToLower() == "sqrt") { //special keyword
            formData = Math.Pow(origData, 0.5);
          } else {
            string dataPar = dataFormula.Substring(1);
            double dataParVal = Convert.ToDouble(dataPar);
            switch (dataFormula[0]) {
              case '+': formData += dataParVal; break;
              case '-': formData -= dataParVal; break;
              case '/': formData /= dataParVal; break;
              case '*': 
              case 'x': formData *= dataParVal; break;
              case '%': formData %= dataParVal; break;
              case '^': formData = Math.Pow(origData, dataParVal); break;
              default: break;
            }
          }
      } catch {
        return origData; //fails
      }
      return formData;
    }

    public static double ApplyDataFormulas(double origData, string dataFormulas) {
      return ApplyDataFormulas(origData, GetDataFormulasArray(dataFormulas));
    }

    public static double ApplyDataFormulas(double origData, string[] dataFormulasArray) {
      double formData = origData;
      try {
        if (dataFormulasArray != null && dataFormulasArray.Length >= 1)
          for (int i = 0; i < dataFormulasArray.Length; ++i)
            formData = ApplyDataFormula(formData, dataFormulasArray[i]);
      } catch {
        return origData; //fails
      }
      return formData;
    }

    public static double ApplyReversedDataFormula(double origData, string dataFormula) {
      double formData = origData;
      try {
        if (!string.IsNullOrWhiteSpace(dataFormula) && (dataFormula.Length >= 2)) //A formula cannot have less than two characters           
          if (dataFormula.Trim().ToLower() == "sqrt") { //special keyword
            formData = Math.Pow(origData, 2);
          } else {
            string dataPar = dataFormula.Substring(1);
            double dataParVal = Convert.ToDouble(dataPar);
            switch (dataFormula[0]) {
            case '+': formData -= dataParVal; break;
            case '-': formData += dataParVal; break;
            case '/': formData *= dataParVal; break;
            case '*':
            case 'x': formData /= dataParVal; break;
            case '%': formData %= dataParVal; break;
            case '^': formData = Math.Pow(origData, 1/dataParVal); break;
            default: break;
            }
          }
      } catch {
        return origData; //fails
      }
      return formData;
    }

    public static double ApplyReversedDataFormulas(double origData, string dataFormulas) {
      return ApplyReversedDataFormulas(origData, GetDataFormulasArray(dataFormulas));
    }

    public static double ApplyReversedDataFormulas(double origData, string[] dataFormulasArray) {
      double formData = origData;
      try {
        if (dataFormulasArray != null && dataFormulasArray.Length >= 1)
          for (int i = dataFormulasArray.Length - 1; i >= 0; --i)
            formData = ApplyReversedDataFormula(formData, dataFormulasArray[i]);
      } catch {
        return origData; //fails
      }
      return formData;
    }

    public static string[] GetDataFormulasArray(string dataFormulas) {
      char[] delimiterChars = { ' ' };
      return dataFormulas.Split(delimiterChars);      
    }

    private static string findVarValue(string varName) {
      for (int i = 0; i < VarDictList.Count; ++i)
        if (VarDictList[i].ContainsKey(varName))
          return (VarDictList[i])[varName];
      return null; //fails to find
    }

    public static string[] GetVariables(string formula, bool trimFrist = false) {
      if (trimFrist)
        formula = FileDirText.ReadUntilMeet(Data.TrimWords(formula), ';');
      char[] delimiterChars = { '+', '-', '*', '/', '%', '^', '(', ')' };
      string[] words = formula.Split(delimiterChars);
      return words;
    }

    public static string ReplaceVarWithValue(string formula) { //This also replaces quotedText with UIntStr value
			string replacedFormula = FileDirText.ReadUntilMeet(Data.TrimWords(formula), ';');
      try {
        string[] words = GetVariables(replacedFormula);
        for (int i = 0; i < words.Length; ++i) //each word is to be checked 
          if (Text.IsVar(words[i])) { //if it is a variable, then it is to be replaced
            string noStr = findVarValue(words[i]);
            if (noStr == null) //not found              
              continue;
						if (Text.IsHex(noStr)) //if it is hex, convert to integer first!
              noStr = uint.Parse(noStr.Substring(2), NumberStyles.HexNumber).ToString();
						else if (Text.IsQuotedText(noStr)) //if the variable value is quoted text, change it into the respective UIntStr
							noStr = Data.ConvertQoutedTextToUIntStr(noStr);
            replacedFormula = replacedFormula.Replace(words[i], noStr);
					} else if (Text.IsQuotedText(words[i])) //if it is quoted text, change it into the respective UIntStr
						replacedFormula = replacedFormula.Replace(words[i], Data.ConvertQoutedTextToUIntStr(words[i]));
      } catch {
        return null; //Fails to find, return null (not a very good idea! But better than returning original value!)
      }

			if (Text.IsDateTime(replacedFormula, isJava: true)) {
        uint tempuint = Data.DateTimeToTaiSeconds(replacedFormula, "java");
        replacedFormula = tempuint.ToString();
      }

      return replacedFormula;
    }

    public static List<char> GetMathOperators(string str) {
      if (string.IsNullOrWhiteSpace(str) || str.Length <= 0)
        return null;
      List<char> mathOps = new List<char>();
      for (int i = 0; i < str.Length; ++i)
				if (Text.IsOperatorSign(str[i]))
          mathOps.Add(str[i]);
      return mathOps.Count <= 0 ? null : mathOps;
    }

    public static string SolveOperation(string strLeft, string strRight, char ops, bool isNegativeCase = false) {
			if (Text.IsOperatorSign(ops)) {
        try {
          switch (ops) {
            case '^': return isNegativeCase ? Math.Pow(Convert.ToDouble(strLeft), Convert.ToDouble("-" + strRight)).ToString() : Math.Pow(Convert.ToDouble(strLeft), Convert.ToDouble(strRight)).ToString();
            case '%': return ((Int64)Convert.ToDouble(strLeft) % (Int64)Convert.ToDouble(strRight)).ToString(); //Modulo cannot accept negative case!
            case '*': return isNegativeCase ? (Convert.ToDouble(strLeft) * -Convert.ToDouble(strRight)).ToString() : (Convert.ToDouble(strLeft) * Convert.ToDouble(strRight)).ToString();
            case '/': return isNegativeCase ? (Convert.ToDouble(strLeft) / -Convert.ToDouble(strRight)).ToString() : (Convert.ToDouble(strLeft) / Convert.ToDouble(strRight)).ToString();
            case '+': 
              if (string.IsNullOrWhiteSpace(strLeft)) //There is a possibility that the left string is empty or null because of case like "+21" (just plus 21)
                return Convert.ToDouble(strRight).ToString();
              return isNegativeCase ? (Convert.ToDouble(strLeft) - Convert.ToDouble(strRight)).ToString() : (Convert.ToDouble(strLeft) + Convert.ToDouble(strRight)).ToString();
            case '-':
              if (string.IsNullOrWhiteSpace(strLeft)) //There is a possibility that the left string is empty or null because of case like "-21" (just minus 21)
                return (-Convert.ToDouble(strRight)).ToString();
              return (Convert.ToDouble(strLeft) - Convert.ToDouble(strRight)).ToString();
            default: return null;
          }
        } catch {          
        }
      }
      return null;
    }

    public static string removesAllDoubleSigns(string formula) {
      string beforeReplacedFormula;
      do { //Removes all double signs
        beforeReplacedFormula = formula;
        formula = formula.Replace("++", "+");
        formula = formula.Replace("--", "+");
        formula = formula.Replace("-+", "-");
        formula = formula.Replace("+-", "-");
      } while (beforeReplacedFormula != formula);
      return formula;
    }

    public static string SolveOperations(string formula) { //solve operation without bracket
      string solvedFormula = removesAllDoubleSigns(formula);
      List<char> mathOps = GetMathOperators(solvedFormula);
      char[] operatorChars = { '+', '-', '*', '/', '%', '^' };
      char[] ops1 = { '^', '%', '*', '+' }; //Power will be treated first, then modulo, then multiplicative, then additive
      char[] ops2 = { '^', '%', '/', '-' };
      for (int i = 0; i < ops1.Length; ++i) {
        if (mathOps == null || mathOps.Count <= 0) //If there isn't any operator left or exist
          break;
        while (mathOps.Contains(ops1[i]) || mathOps.Contains(ops2[i])) {
          int opsIndex1 = mathOps.Contains(ops1[i]) ? mathOps.IndexOf(ops1[i]) : 0x7fffffff; //gives very large index when failed to get the index
          int opsIndex2 = mathOps.Contains(ops2[i]) ? mathOps.IndexOf(ops2[i]) : 0x7fffffff;
          int opsIndex = opsIndex1 < opsIndex2 ? opsIndex1 : opsIndex2;
          char ops = opsIndex1 < opsIndex2 ? ops1[i] : ops2[i]; //get the left most operator
          string[] words = solvedFormula.Split(operatorChars);
          bool isNegativeCase = false;
          bool isPositiveCase = false;
          string addSignString = "";
          string rightStr = words[opsIndex + 1];
          if (string.IsNullOrWhiteSpace(words[opsIndex + 1]) && //next word is null "2" "" "21"
            words.Length > opsIndex + 2 && //It is at least three words
            mathOps.Count > opsIndex + 1 && //the real math ops still has something after this index.
            !string.IsNullOrWhiteSpace(words[opsIndex + 2])) {//then this is a negative case
            isNegativeCase = mathOps[opsIndex + 1] == '-';
            isPositiveCase = mathOps[opsIndex + 1] == '+';
            if (isNegativeCase || isPositiveCase) { //for cases like 2*-3 or 2/+5
              addSignString = isNegativeCase ? "-" : "+";
              rightStr = words[opsIndex + 2];
            }
          }
          string subSolvedFormula = SolveOperation(words[opsIndex], rightStr, ops, isNegativeCase);
          if (subSolvedFormula == null)
            return null; //fails!
          solvedFormula = removesAllDoubleSigns(solvedFormula.Replace(words[opsIndex] + ops + addSignString + rightStr, subSolvedFormula));
          mathOps.Clear();
          if ((words.Length > 2 && !isNegativeCase && !isPositiveCase) ||
            (words.Length > 3 && (isNegativeCase || isPositiveCase))) //if this is false, that is the end!
            mathOps.AddRange(GetMathOperators(solvedFormula));
        }
      }
      return solvedFormula;
    }

    public static string SolveFormula(string formula) {
      string solvedFormula = formula;
      bool bracketFound;
      string testGetInsideBracket = null;
      do {
        bracketFound = false;
        testGetInsideBracket = FileDirText.ReadInBetweenSameDepth(solvedFormula, '(', ')');
        if (testGetInsideBracket != null) { //found!          
          bracketFound = true;
          string solvedInsideBracketFormula = SolveFormula(testGetInsideBracket); //recursive          
          solvedFormula = solvedFormula.Replace("(" + testGetInsideBracket + ")", solvedInsideBracketFormula);
        } else //if it is not found anymore, solves it!
          solvedFormula = SolveOperations(solvedFormula);
      } while (bracketFound); //whil bracket is found, continues!
      return solvedFormula;
    }

  }
}

    //public static List<Dictionary<string, string>> VarDictList = new List<Dictionary<string, string>>();

    //public static double ApplyDataFormula(double origData, string dataFormula) {
    //  double formData = origData;
    //  try {
    //    if (!string.IsNullOrWhiteSpace(dataFormula))
    //      if (dataFormula.Length >= 2) //A formula cannot have less than two characters
    //        if (dataFormula.Trim().ToLower() == "sqrt") { //special keyword
    //          formData = Math.Pow(origData, 0.5);
    //        } else {
    //          string dataPar = dataFormula.Substring(1);
    //          double dataParVal = Convert.ToDouble(dataPar);
    //          switch (dataFormula[0]) {
    //            case '+': formData += dataParVal; break;
    //            case '-': formData -= dataParVal; break;
    //            case '/': formData /= dataParVal; break;
    //            case '*': 
    //            case 'x': formData *= dataParVal; break;
    //            case '%': formData %= dataParVal; break;
    //            case '^': formData = Math.Pow(origData, dataParVal); break;
    //            default: break;
    //          }
    //        }
    //  } catch {
    //    return origData; //fails
    //  }
    //  return formData;
    //}

    //public static double ApplyDataFormulas(double origData, string dataFormulas) {
    //  return ApplyDataFormulas(origData, GetDataFormulasArray(dataFormulas));
    //}

    //public static double ApplyDataFormulas(double origData, string[] dataFormulasArray) {
    //  double formData = origData;
    //  try {
    //    if (dataFormulasArray != null)
    //      if (dataFormulasArray.Length >= 1)
    //        for (int i = 0; i < dataFormulasArray.Length; ++i)
    //          formData = ApplyDataFormula(formData, dataFormulasArray[i]);
    //  } catch {
    //    return origData; //fails
    //  }
    //  return formData;
    //}

    //public static double ApplyReversedDataFormula(double origData, string dataFormula) {
    //  double formData = origData;
    //  try {
    //    if (!string.IsNullOrWhiteSpace(dataFormula))
    //      if (dataFormula.Length >= 2) //A formula cannot have less than two characters
    //        if (dataFormula.Trim().ToLower() == "sqrt") { //special keyword
    //          formData = Math.Pow(origData, 2);
    //        } else {
    //          string dataPar = dataFormula.Substring(1);
    //          double dataParVal = Convert.ToDouble(dataPar);
    //          switch (dataFormula[0]) {
    //          case '+': formData -= dataParVal; break;
    //          case '-': formData += dataParVal; break;
    //          case '/': formData *= dataParVal; break;
    //          case '*':
    //          case 'x': formData /= dataParVal; break;
    //          case '%': formData %= dataParVal; break;
    //          case '^': formData = Math.Pow(origData, 1/dataParVal); break;
    //          default: break;
    //          }
    //        }
    //  } catch {
    //    return origData; //fails
    //  }
    //  return formData;
    //}

    //public static double ApplyReversedDataFormulas(double origData, string dataFormulas) {
    //  return ApplyReversedDataFormulas(origData, GetDataFormulasArray(dataFormulas));
    //}

    //public static double ApplyReversedDataFormulas(double origData, string[] dataFormulasArray) {
    //  double formData = origData;
    //  try {
    //    if (dataFormulasArray != null)
    //      if (dataFormulasArray.Length >= 1)
    //        for (int i = dataFormulasArray.Length - 1; i >= 0; --i)
    //          formData = ApplyReversedDataFormula(formData, dataFormulasArray[i]);
    //  } catch {
    //    return origData; //fails
    //  }
    //  return formData;
    //}

    //public static string[] GetDataFormulasArray(string dataFormulas) {
    //  char[] delimiterChars = { ' ' };
    //  return dataFormulas.Split(delimiterChars);      
    //}

    //private static string findVarValue(string varName) {
    //  for (int i = 0; i < VarDictList.Count; ++i)
    //    if (VarDictList[i].ContainsKey(varName))
    //      return (VarDictList[i])[varName];
    //  return null; //fails to find
    //}

    //public static string[] GetVariables(string formula, bool trimFrist = false) {
    //  if (trimFrist)
    //    formula = StreamReaderExtension.ReadUntilMeet(DataManipulator.TrimWords(formula), ';');
    //  char[] delimiterChars = { '+', '-', '*', '/', '%', '^', '(', ')' };
    //  string[] words = formula.Split(delimiterChars);
    //  return words;
    //}

    //public static string ReplaceVarWithValue(string formula) { //This also replaces quotedText with UIntStr value
    //  string replacedFormula = StreamReaderExtension.ReadUntilMeet(DataManipulator.TrimWords(formula), ';');
    //  try {        
    //    string[] words = GetVariables(replacedFormula);
    //    for (int i = 0; i < words.Length; ++i) //each word is to be checked 
    //      if (TextCheckerExtension.IsVar(words[i])) { //if it is a variable, then it is to be replaced
    //        string noStr = findVarValue(words[i]);
    //        if (noStr != null) { //found              
    //          if (TextCheckerExtension.IsHex(noStr)) //if it is hex, convert to integer first!
    //            noStr = uint.Parse(noStr.Substring(2), NumberStyles.HexNumber).ToString();
    //          else if (TextCheckerExtension.IsQuotedText(noStr)) //if the variable value is quoted text, change it into the respective UIntStr
    //            noStr = DataManipulator.ConvertQoutedTextToUIntStr(noStr);
    //          replacedFormula = replacedFormula.Replace(words[i], noStr);
    //        }
    //      } else if (TextCheckerExtension.IsQuotedText(words[i])) //if it is quoted text, change it into the respective UIntStr
    //        replacedFormula = replacedFormula.Replace(words[i], DataManipulator.ConvertQoutedTextToUIntStr(words[i]));
    //  } catch {
    //    return null; //Fails to find, return null (not a very good idea! But better than returning original value!)
    //  }

    //  if (TextCheckerExtension.IsDateTime(replacedFormula, isJava: true)) {
    //    uint tempuint = DataManipulator.DateTimeToTaiSeconds(replacedFormula, "java");
    //    replacedFormula = tempuint.ToString();
    //  }

    //  return replacedFormula;
    //}

    //public static List<char> GetMathOperators(string str) {
    //  if (!string.IsNullOrWhiteSpace(str)) {
    //    if (str.Length > 0) {
    //      List<char> mathOps = new List<char>();
    //      for (int i = 0; i < str.Length; ++i)
    //        if (TextCheckerExtension.IsOperatorSign(str[i]))
    //          mathOps.Add(str[i]);
    //      if (mathOps.Count <= 0)
    //        return null;
    //      return mathOps;
    //    }
    //  }
    //  return null;
    //}

    //public static string SolveOperation(string strLeft, string strRight, char ops, bool isNegativeCase = false) {
    //  if (TextCheckerExtension.IsOperatorSign(ops)) {
    //    try {
    //      switch (ops) {
    //        case '^': return isNegativeCase ? Math.Pow(Convert.ToDouble(strLeft), Convert.ToDouble("-" + strRight)).ToString() : Math.Pow(Convert.ToDouble(strLeft), Convert.ToDouble(strRight)).ToString();
    //        case '%': return ((Int64)Convert.ToDouble(strLeft) % (Int64)Convert.ToDouble(strRight)).ToString(); //Modulo cannot accept negative case!
    //        case '*': return isNegativeCase ? (Convert.ToDouble(strLeft) * -Convert.ToDouble(strRight)).ToString() : (Convert.ToDouble(strLeft) * Convert.ToDouble(strRight)).ToString();
    //        case '/': return isNegativeCase ? (Convert.ToDouble(strLeft) / -Convert.ToDouble(strRight)).ToString() : (Convert.ToDouble(strLeft) / Convert.ToDouble(strRight)).ToString();
    //        case '+': 
    //          if (string.IsNullOrWhiteSpace(strLeft)) //There is a possibility that the left string is empty or null because of case like "+21" (just plus 21)
    //            return Convert.ToDouble(strRight).ToString();
    //          return isNegativeCase ? (Convert.ToDouble(strLeft) - Convert.ToDouble(strRight)).ToString() : (Convert.ToDouble(strLeft) + Convert.ToDouble(strRight)).ToString();
    //        case '-':
    //          if (string.IsNullOrWhiteSpace(strLeft)) //There is a possibility that the left string is empty or null because of case like "-21" (just minus 21)
    //            return (-Convert.ToDouble(strRight)).ToString();
    //          return (Convert.ToDouble(strLeft) - Convert.ToDouble(strRight)).ToString();
    //        default: return null;
    //      }
    //    } catch {          
    //    }
    //  }
    //  return null;
    //}

    //public static string removesAllDoubleSigns(string formula) {
    //  string beforeReplacedFormula;
    //  do { //Removes all double signs
    //    beforeReplacedFormula = formula;
    //    formula = formula.Replace("++", "+");
    //    formula = formula.Replace("--", "+");
    //    formula = formula.Replace("-+", "-");
    //    formula = formula.Replace("+-", "-");
    //  } while (beforeReplacedFormula != formula);
    //  return formula;
    //}

    //public static string SolveOperations(string formula) { //solve operation without bracket
    //  string solvedFormula = removesAllDoubleSigns(formula); 
    //  List<char> mathOps = FormulaManipulator.GetMathOperators(solvedFormula);
    //  char[] operatorChars = { '+', '-', '*', '/', '%', '^' };
    //  char[] ops1 = { '^', '%', '*', '+' }; //Power will be treated first, then modulo, then multiplicative, then additive
    //  char[] ops2 = { '^', '%', '/', '-' };
    //  for (int i = 0; i < ops1.Length; ++i) {
    //    if (mathOps != null) {
    //      if (mathOps.Count > 0) { //If there is still any operator exists
    //        while (mathOps.Contains(ops1[i]) || mathOps.Contains(ops2[i])) {
    //          int opsIndex1 = mathOps.Contains(ops1[i]) ? mathOps.IndexOf(ops1[i]) : 0x7fffffff; //gives very large index when failed to get the index
    //          int opsIndex2 = mathOps.Contains(ops2[i]) ? mathOps.IndexOf(ops2[i]) : 0x7fffffff;
    //          int opsIndex = opsIndex1 < opsIndex2 ? opsIndex1 : opsIndex2;
    //          char ops = opsIndex1 < opsIndex2 ? ops1[i] : ops2[i]; //get the left most operator
    //          string[] words = solvedFormula.Split(operatorChars);
    //          bool isNegativeCase = false;
    //          bool isPositiveCase = false;
    //          string addSignString = "";
    //          string rightStr = words[opsIndex + 1];
    //          if (string.IsNullOrWhiteSpace(words[opsIndex + 1]) && //next word is null "2" "" "21"
    //            words.Length > opsIndex + 2 && //It is at least three words
    //            mathOps.Count > opsIndex + 1 && //the real math ops still has something after this index.
    //            !string.IsNullOrWhiteSpace(words[opsIndex + 2])) {//then this is a negative case
    //            isNegativeCase = mathOps[opsIndex + 1] == '-';
    //            isPositiveCase = mathOps[opsIndex + 1] == '+';
    //            if (isNegativeCase || isPositiveCase) { //for cases like 2*-3 or 2/+5
    //              addSignString = isNegativeCase ? "-" : "+";
    //              rightStr = words[opsIndex + 2];
    //            }
    //          }
    //          string subSolvedFormula = SolveOperation(words[opsIndex], rightStr, ops, isNegativeCase);
    //          if (subSolvedFormula != null) {
    //            solvedFormula = removesAllDoubleSigns(solvedFormula.Replace(words[opsIndex] + ops + addSignString + rightStr, subSolvedFormula));                
    //            mathOps.Clear();
    //            if ((words.Length > 2 && !isNegativeCase && !isPositiveCase) || 
    //              (words.Length > 3 && (isNegativeCase || isPositiveCase))) //if this is false, that is the end!
    //              mathOps.AddRange(FormulaManipulator.GetMathOperators(solvedFormula));
    //          } else
    //            return null; //fails!
    //        }
    //      } else
    //        break; //there are more than one break in this loop, all are necessary          
    //    } else
    //      break;        
    //  }
    //  return solvedFormula;
    //}

    //public static string SolveFormula(string formula) {
    //  string solvedFormula = formula;
    //  bool bracketFound;
    //  string testGetInsideBracket = null;
    //  do {
    //    bracketFound = false;
    //    testGetInsideBracket = StreamReaderExtension.ReadInBetweenSameDepth(solvedFormula, '(', ')');
    //    if (testGetInsideBracket != null) { //found!          
    //      bracketFound = true;
    //      string solvedInsideBracketFormula = SolveFormula(testGetInsideBracket); //recursive          
    //      solvedFormula = solvedFormula.Replace("(" + testGetInsideBracket + ")", solvedInsideBracketFormula);
    //    } else //if it is not found anymore, solves it!
    //      solvedFormula = SolveOperations(solvedFormula);
    //  } while (bracketFound); //whil bracket is found, continues!
    //  return solvedFormula;
    //}

