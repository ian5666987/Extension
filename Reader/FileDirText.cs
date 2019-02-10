using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualBasic.FileIO; //for TextFieldParser

namespace Extension.Reader
{
  public class FileDirText
  {
		public static List<string> GetAllAccessibleDirectories(string path, string searchPattern) {
			List<string> dirPathList = new List<string>();
			try {
				List<string> childDirPathList = Directory.GetDirectories(path, searchPattern, System.IO.SearchOption.TopDirectoryOnly).ToList();
				if (childDirPathList == null || childDirPathList.Count <= 0) //this directory has no child
					return null;
				foreach (string childDirPath in childDirPathList) { //foreach child directory, do recursive search
					dirPathList.Add(childDirPath); //add the path
					List<string> grandChildDirPath = GetAllAccessibleDirectories(childDirPath, searchPattern);
					if (grandChildDirPath != null && grandChildDirPath.Count > 0) //this child directory has children and nothing has gone wrong
						dirPathList.AddRange(grandChildDirPath.ToArray()); //add the grandchildren to the list
				}
				return dirPathList; //return the whole list found at this level
			} catch {
				return null; //something has gone wrong, return null
			}
		}

		//Example: ReadAllFiles("c:/", ".txt", false). What returned are full path
		public static List<string> ReadAllFiles(string folderpath, string extension, bool includeSubfolder) {
			return Directory.GetFiles(folderpath, string.IsNullOrWhiteSpace(extension) ? "*" : "*." + extension,
				includeSubfolder ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly).ToList();
		}

    //Example: ReadInBetween("[Bach]", '[', ']') will return: Bach
    public static string ReadInBetween(string str, char delimiterStart, char delimiterEnd) {
      if (string.IsNullOrWhiteSpace(str) || str.Length <= 2) //for non-null and non-whitespace string having at least three characters, proceed
        return null;
      bool delimiterStartIsFound = false;
      int posStart = -1;
      for (int i = 0; i < str.Length; ++i) {
        if (!delimiterStartIsFound) { //if delimiter start is not found, finds it first
          if (str[i] != delimiterStart)
            continue;
          if (i >= str.Length - 2) //delimiter start is found in any of the last two characters
            return null; //it means, there isn't anything in between the two
          delimiterStartIsFound = true;
          posStart = i + 1;
          continue; //so long as it delimiter start is not found or is just found, quickly continue...
        }
        if (str[i] == delimiterEnd && i - posStart > 0) //if delimiter end is found after the delimiter start
          return str.Substring(posStart, i - posStart); //only successful if both delimiters are found in the correct order/position
      }
      return null;
    }
		
		public static IEnumerable<string> TextFieldParser(string str, bool hasFieldsEnclosedInQuotes = true, string[] delimiters = null) {
			IEnumerable<string> words;
			using (TextFieldParser parser = new TextFieldParser(new StringReader(str))) {
				parser.Delimiters = delimiters == null || delimiters.Length <= 0 ? new string[] { " " } : delimiters; //actually, I am not sure what will happen here
				parser.HasFieldsEnclosedInQuotes = hasFieldsEnclosedInQuotes;  // important, but not sure why yet...
				words = parser.ReadFields().Where(x => !string.IsNullOrEmpty(x));				
			}
			return words;
		}

    //Examples:
		//[1] (2 + 1) * (5 + 6) will return 2 + 1
		//[2] (2 * (5 + 6) + 1) will return 2 * (5 + 6) + 1
    public static string ReadInBetweenSameDepth(string str, char delimiterStart, char delimiterEnd) {
      if (delimiterStart == delimiterEnd || string.IsNullOrWhiteSpace(str) || str.Length <= 2)
        return null;
      int delimiterStartFound = 0;
      int delimiterEndFound = 0;
      int posStart = -1;
      for (int i = 0; i < str.Length; ++i) {
        if (str[i] == delimiterStart) {
          if (i >= str.Length - 2) //delimiter start is found in any of the last two characters
            return null; //it means, there isn't anything in between the two
          if (delimiterStartFound == 0) //first time
            posStart = i + 1; //assign the starting position only the first time...
          delimiterStartFound++; //increase the number of delimiter start count to get the same depth
        }
        if (str[i] == delimiterEnd) {
          delimiterEndFound++;
          if (delimiterStartFound == delimiterEndFound && i - posStart > 0)
            return str.Substring(posStart, i - posStart); //only successful if both delimiters are found in the same depth
        }
      }
      return null;
    }

    public static string ReadUntilMeet(string str, string delimiter, bool isInclusive = false, bool isTrimmed = false) {
      if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(delimiter) || str.Length <= delimiter.Length)
        return null;
      int indexof = str.IndexOf(delimiter, 0);
      if (indexof <= 0) //not found or in the zero offset
        return str; //return the original string
      str = str.Substring(0, indexof + (isInclusive ? delimiter.Length : 0));
      return isTrimmed ? str.Trim() : str;
    }

    public static string ReadUntilMeet(string str, char delimiter, bool isInclusive = false, bool isTrimmed = false) {
      return ReadUntilMeet(str, delimiter.ToString(), isInclusive, isTrimmed);
    }

    //Example: ReadAfterMeet(something.combo="paradoxa", '=', don't care) will return "paradoxa"
    //   while ReadAfterMeet(something.combo="paradoxa", '=', don't care, true) will return paradoxa
    public static string ReadAfterMeet(string str, string delimiter, bool removeEnclosure = false, bool isTrimmed = false) {
      if (string.IsNullOrWhiteSpace(str) || str.Length < 2 || delimiter == null || delimiter.Length < 1) //must contain at least 2 characters, delimiter must have length of at least 1, can be whitespace but not null
        return null; //for non-null and non-whitespace string only, delimiter can be white space but not null
      int indexof = str.IndexOf(delimiter, 0);
      if (indexof == -1 || indexof == str.Length - delimiter.Length) //not found or is the last characters
        return null;
      string returnedStr = str.Substring(indexof + 1); //prepare the remaining string
      if (removeEnclosure && returnedStr.Length > 2) //Must be having length of at least three to remove the enclosing quotation mark
        return returnedStr.Substring(1, returnedStr.Length - 2);
      return returnedStr;
    }

    public static string ReadAfterMeet(string str, char delimiter, bool removeEnclosure = false, bool isTrimmed = false) {
      return ReadAfterMeet(str, delimiter.ToString(), removeEnclosure, isTrimmed);
    }

    public static string ReadAfterMeet(string str, string delimiter, bool removeEnclosure, char startEnclosure, char endEnclosure, bool isTrimmed = false) {
      if (string.IsNullOrWhiteSpace(str) || str.Length < 2 || delimiter == null || delimiter.Length < 1) //must contain at least 2 characters, delimiter must have length of at least 1, can be whitespace but not null
        return null; //for non-null and non-whitespace string only, delimiter can be white space but not null
      int indexof = str.IndexOf(delimiter, 0);
      if (indexof == -1 || indexof == str.Length - delimiter.Length) //not found or is the last characters
        return null;
      string returnedStr = str.Substring(indexof + 1); //prepare the remaining string
      if (removeEnclosure && returnedStr.Length > 2 && returnedStr[0] == startEnclosure && returnedStr[returnedStr.Length - 1] == endEnclosure) //Must be having length of at least three to remove the enclosing quotation mark
        return returnedStr.Substring(1, returnedStr.Length - 2);
      return returnedStr;
    }

    public static string ReadAfterMeet(string str, char delimiter, bool removeEnclosure, char startEnclosure, char endEnclosure, bool isTrimmed = false) {
      return ReadAfterMeet(str, delimiter.ToString(), removeEnclosure, startEnclosure, endEnclosure, isTrimmed);
    }

    public static string[] SplitBeforeAndAfterMeet(string str, string delimiter, bool removeEnclosure = false, bool isInclusive = false, bool isTrimmed = false) {
      string[] words = new string[2]; //the returned string must be of size 2 for successful case. Will return null otherwise
      words[0] = ReadUntilMeet(str, delimiter, isInclusive, isTrimmed);
      words[1] = ReadAfterMeet(str, delimiter, removeEnclosure, isTrimmed);
      return words[0] == null || words[1] == null ? null : words;
    }

    public static string[] SplitBeforeAndAfterMeet(string str, char delimiter, bool removeEnclosure = false, bool isInclusive = false, bool isTrimmed = false) {
      return SplitBeforeAndAfterMeet(str, delimiter.ToString(), removeEnclosure, isInclusive, isTrimmed);
    }

    public static string[] SplitBeforeAndAfterMeet(string str, string delimiter, bool removeEnclosure, char startEnclosure, char endEnclosure, bool isInclusive = false, bool isTrimmed = false) {
      string[] words = new string[2]; //the returned string must be of size 2 for successful case. Will return null otherwise
      words[0] = ReadUntilMeet(str, delimiter, isInclusive, isTrimmed);
      words[1] = ReadAfterMeet(str, delimiter, removeEnclosure, startEnclosure, endEnclosure, isTrimmed);
      return words[0] == null || words[1] == null ? null : words;
    }

    public static string[] SplitBeforeAndAfterMeet(string str, char delimiter, bool removeEnclosure, char startEnclosure, char endEnclosure, bool isInclusive = false, bool isTrimmed = false) {
      return SplitBeforeAndAfterMeet(str, delimiter.ToString(), removeEnclosure, startEnclosure, endEnclosure, isInclusive, isTrimmed);
    }

    private static void processStripMark(ref List<string> validLines, string validLine, bool isTrimmed, bool stripmark, char[] strippedmarks, char mark) {
      if (!stripmark) //if we don't need to strip the mark it will be straight-forward
        validLines.Add(validLine);
      if (stripmark && validLine.Length > 1 && strippedmarks != null && strippedmarks.Length > 0 && strippedmarks.Contains(mark)) { //if strip mark is to be used and we can really strip the mark and this mark is to be stripped
        validLine = validLine.Substring(1); //strip the mark
        if (!string.IsNullOrWhiteSpace(validLine)) //not becoming invalid after stripping
          validLines.Add(isTrimmed ? validLine.Trim() : validLine); //add according to the instruction: trimmed or not trimmed
      } //else we cannot really strip the mark, though we are supposed to. Then, just ignore this case!
    }

    private static void processStripMark(ref List<string> validLines, string validLine, bool isTrimmed, bool stripmark, char[] strippedmarks) { //without mark reference
      if (!stripmark) //if we don't need to strip the mark it will be straight-forward
        validLines.Add(validLine);
      if (stripmark && validLine.Length > 1 && strippedmarks != null && strippedmarks.Length > 0) { //if strip mark is to be used and we can really strip the mark and this mark is to be stripped
        for (int i = 0; i < strippedmarks.Length; ++i) //check for every stripmark, see if it can really be stripped before being added
          if (validLine[0] == strippedmarks[i]) {
            validLine = validLine.Substring(1); //strip the mark
            if (!string.IsNullOrWhiteSpace(validLine)) //not becoming invalid after stripping
              validLines.Add(isTrimmed ? validLine.Trim() : validLine); //add according to the instruction: trimmed or not trimmed
            break;
          }
      } //else we cannot really strip the mark, though we are supposed to. Then, just ignore this case!
    }

    //isTrimmed -> to trim the line obtained
    //markInclusionExclusion -> true means inclusion, false means exclusion, as indicated by marks. Example true, '#' will only include lines with '#' as the first char while false, '#' will include all but lines with '#' as the first char
		//marks -> the marks to be included or excluded. markInclusionExclusion = false and marks = null accepts all lines  
		//stripmark -> makes the mark stripped before considered valid once more, done before considered valid (can be used for both inclusion and exclusion case, just before things are added)
    //strippedmarks -> for inclusion case: onyl mark among the included will be counter. 
    //                 for exclusion, some remainder can be checked whether things with marks among the included are to be stripped first before considered valid or not
    //                 if mark is not a concerned, this array can still be used to strip the unwanted mark
    public static List<string> GetAllValidLines(string filepath, bool isTrimmed = false, bool markInclusionExclusion = false, char[] marks = null, bool stripmark = false, char[] strippedmarks = null) { 
      StreamReader sr = null;
      try {
        sr = new StreamReader(filepath);
        List<string> validLines = new List<string>();
				while (sr.EndOfStream == false) {
					string line = sr.ReadLine(); //Read line by line
					if (string.IsNullOrWhiteSpace(line)) //Proceeds if the string is not null or empty or white space              
						continue;
					string validLine = isTrimmed ? line.Trim() : line; //at this point, it cannot be whitespace
					if (marks == null || marks.Length <= 0) { //going in here means mark is not a concern
						processStripMark(ref validLines, validLine, isTrimmed, stripmark, strippedmarks);
						continue;
					} //if marks is specified, it means, further process is needed before 
					bool hasmark = marks.Contains(validLine[0]); //check if the mark is contained
					if (hasmark && markInclusionExclusion) //If mark is found, process it with the input mark
						processStripMark(ref validLines, validLine, isTrimmed, stripmark, strippedmarks, validLine[0]);
					if (!hasmark && !markInclusionExclusion) //if mark is supposed to be excluded and there isn't such mark detected, then we can include this line
						processStripMark(ref validLines, validLine, isTrimmed, stripmark, strippedmarks);
				}
        sr.Close();
        return validLines;
      } catch {
        if (sr != null)
          sr.Close();       
        return null;
      }
    }

		public static List<string> GetAllNonEmptyLines(string filepath) {
			List<string> lines = GetAllLines(filepath);
			if (lines == null)
				return null;
			return lines.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
		}

		public static List<string> GetAllLines(string filepath) {
      StreamReader sr = null;
      try {
        sr = new StreamReader(filepath);
        List<string> lines = new List<string>();
        while (sr.EndOfStream == false)          
          lines.Add(sr.ReadLine());        
        sr.Close();
        return lines;
      } catch {
        if (sr != null)
          sr.Close();
        return null;
      }
    }

    public static byte[] ReadFileStream(string filepath) {
      try {
        BinaryReader binReader = new BinaryReader(File.Open(filepath, FileMode.Open));
        MemoryStream memStream = new MemoryStream();
        bool isEndOfFile = false;
        byte readByte = 0;
        while (!isEndOfFile) {
          try {
            readByte = binReader.ReadByte();
            memStream.WriteByte(readByte);
          } catch {
            isEndOfFile = true;
          }
        }
        binReader.Close();
        return memStream.ToArray();
      } catch {
        return null;
      }
    }

  }

}


    ////Example: ReadInBetween("[Bach]", '[', ']') will return: Bach
    //public static string ReadInBetween(string str, char delimiterStart, char delimiterEnd) {
    //  if (string.IsNullOrWhiteSpace(str) || str.Length <= 2) //for non-null and non-whitespace string having at least three characters, proceed
    //    return null;
    //  bool delimiterStartIsFound = false;
    //  int posStart = -1;
    //  for (int i = 0; i < str.Length; ++i) {
    //    if (!delimiterStartIsFound) { //if delimiter start is not found, finds it first
    //      if (str[i] != delimiterStart)
    //        continue;
    //      if (i >= str.Length - 2) //delimiter start is found in any of the last two characters
    //        return null; //it means, there isn't anything in between the two
    //      delimiterStartIsFound = true;
    //      posStart = i + 1;
    //      continue; //so long as it delimiter start is not found or is just found, quickly continue...
    //    }
    //    if (str[i] == delimiterEnd && i - posStart > 0) //if delimiter end is found after the delimiter start
    //      return str.Substring(posStart, i - posStart); //only successful if both delimiters are found in the correct order/position
    //  }
    //  return null;
    //}

    ////Example: (2 + 1) * (5 + 6) will return 2 + 1, (2 * (5 + 6) + 1) will return 2 * (5 + 6) + 1
    //public static string ReadInBetweenSameDepth(string str, char delimiterStart, char delimiterEnd) {
    //  if (delimiterStart == delimiterEnd || string.IsNullOrWhiteSpace(str) || str.Length <= 2)
    //    return null;
    //  int delimiterStartFound = 0;
    //  int delimiterEndFound = 0;
    //  int posStart = -1;
    //  for (int i = 0; i < str.Length; ++i) {
    //    if (str[i] == delimiterStart) {
    //      if (i >= str.Length - 2) //delimiter start is found in any of the last two characters
    //        return null; //it means, there isn't anything in between the two
    //      if (delimiterStartFound == 0) //first time
    //        posStart = i + 1; //assign the starting position only the first time...
    //      delimiterStartFound++; //increase the number of delimiter start count to get the same depth
    //    }
    //    if (str[i] == delimiterEnd) {
    //      delimiterEndFound++;
    //      if (delimiterStartFound == delimiterEndFound && i - posStart > 0)
    //        return str.Substring(posStart, i - posStart); //only successful if both delimiters are found in the same depth
    //    }
    //  }
    //  return null;
    //  //if (delimiterStart == delimiterEnd)
    //  //  return null; //unable to take care of this case
    //  //if (!string.IsNullOrWhiteSpace(str)) {//for non-null and non-whitespace string
    //  //  if (str.Length > 2) { //at least three characters
    //  //    int delimiterStartFound = 0;
    //  //    int delimiterEndFound = 0;
    //  //    int posStart = -1;
    //  //    for (int i = 0; i < str.Length; ++i) {
    //  //      if (str[i] == delimiterStart) {
    //  //        if (i < str.Length - 2) {
    //  //          if (delimiterStartFound == 0) //first time
    //  //            posStart = i + 1;
    //  //          delimiterStartFound++;
    //  //        } else
    //  //          return null;
    //  //      }
    //  //      if (str[i] == delimiterEnd) {
    //  //        delimiterEndFound++;
    //  //        if (delimiterStartFound == delimiterEndFound && i - posStart > 0)
    //  //          return str.Substring(posStart, i - posStart); //only successful if both delimiters are found
    //  //      }
    //  //    }
    //  //  }
    //  //}
    //  //return null;
    //}

    //public static string ReadUntilMeet(string str, string delimiter, bool isInclusive = false) {
    //  if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(delimiter) || str.Length <= delimiter.Length)
    //    return null;
    //  int indexof = str.IndexOf(delimiter, 0);
    //  if (indexof <= 0) //not found or in the zero offset
    //    return str; //return the original string
    //  return str.Substring(0, indexof + (isInclusive ? delimiter.Length : 0));
    //  //if (!string.IsNullOrWhiteSpace(str) && !string.IsNullOrWhiteSpace(limiter))//for non-null and non-whitespace string
    //  //  if (str.Length > limiter.Length) //the string must be longer than the limiter
    //  //    for (int i = 0; i < str.Length - limiter.Length; ++i)           
    //  //      if (str.Substring(i, limiter.Length) == limiter)
    //  //        return str.Substring(i + limiter.Length).Trim();          
    //  //return null;
    //}

    ////public static int GetIndexUntilMeet(string str, char delimiter) {
    ////  if (!string.IsNullOrWhiteSpace(str)) //for non-null and non-whitespace string
    ////    for (int i = 0; i < str.Length; ++i)
    ////      if (str[i] == delimiter)
    ////        return i;
    ////  return -1;
    ////}

    //public static string ReadUntilMeet(string str, char delimiter, bool isInclusive = false) {
    //  if (string.IsNullOrWhiteSpace(str))
    //    return null;
    //  int indexof = str.IndexOf(delimiter, 0);
    //  if (indexof <= 0) //not found or in the zero offset
    //    return str; //return the original string
    //  return str.Substring(0, indexof + (isInclusive ? 1 : 0));
    //  //if (!string.IsNullOrWhiteSpace(str)) //for non-null and non-whitespace string
    //  //  for (int i = 0; i < str.Length; ++i)
    //  //    if (str[i] == delimiter)
    //  //      return str.Substring(0, i);
    //  //return null;
    //}

    ////public static string ReadUntilMeetInclusive(string str, char delimiter) {
    ////  if (string.IsNullOrWhiteSpace(str))
    ////    return null;
    ////  int indexof = str.IndexOf(delimiter, 0);
    ////  if (indexof <= 0) //not found or in the zero offset
    ////    return null;
    ////  return str.Substring(0, indexof + 1);
    //  //if (!string.IsNullOrWhiteSpace(str)) //for non-null and non-whitespace string
    //  //  for (int i = 0; i < str.Length; ++i)
    //  //    if (str[i] == delimiter)
    //  //      return str.Substring(0, i + 1);
    //  //return null;
    ////}

    ////Example: ReadAfterMeet(something.combo="paradoxa", '=') will return "paradoxa"
    ////   while ReadAfterMeet(something.combo="paradoxa", '=', true) will return paradoxa
    //public static string ReadAfterMeet(string str, char delimiter, bool enclosingQuotationMarkRemoved = false) {
    //  if (string.IsNullOrWhiteSpace(str) || str.Length < 2) //must contain at least 2 characters
    //    return null; //for non-null and non-whitespace string only
    //  int indexof = str.IndexOf(delimiter, 0);
    //  if (indexof == -1 || indexof == str.Length - 1) //not found or is the last character
    //    return null;
    //  string returnedStr = str.Substring(indexof + 1); //prepare the remaining string
    //  if (enclosingQuotationMarkRemoved && returnedStr.Length > 2 && returnedStr[0] == '"' && returnedStr[returnedStr.Length - 1] == '"') //Must be having length of at least three to remove the enclosing quotation mark
    //    return returnedStr.Substring(1, returnedStr.Length - 2);
    //  return returnedStr;
    //  //for (int i = 0; i < str.Length; ++i)
    //  //  if (str[i] == delimiter)
    //  //    try {
    //  //      string returnedStr = str.Substring(i + 1);
    //  //      if (quotationMarkRemoved && returnedStr.Length > 2 && returnedStr[0] == '"' && returnedStr[returnedStr.Length - 1] == '"') //Must be having length of at least three
    //  //        return returnedStr.Substring(1, returnedStr.Length - 2);
    //  //      return returnedStr;
    //  //    } catch {
    //  //      return null;
    //  //    }
    //  //return null;
    //}

    //public static string ReadAfterMeet(string str, string delimiter, bool enclosingQuotationMarkRemoved = false) {
    //  if (string.IsNullOrWhiteSpace(str) || str.Length < 2 || delimiter != null || delimiter.Length < 1) //must contain at least 2 characters, delimiter must have length of at least 1, can be whitespace but not null
    //    return null; //for non-null and non-whitespace string only, delimiter can be white space but not null
    //  int indexof = str.IndexOf(delimiter, 0);
    //  if (indexof == -1 || indexof == str.Length - delimiter.Length) //not found or is the last characters
    //    return null;
    //  string returnedStr = str.Substring(indexof + 1); //prepare the remaining string
    //  if (enclosingQuotationMarkRemoved && returnedStr.Length > 2 && returnedStr[0] == '"' && returnedStr[returnedStr.Length - 1] == '"') //Must be having length of at least three to remove the enclosing quotation mark
    //    return returnedStr.Substring(1, returnedStr.Length - 2);
    //  return returnedStr;
    //}

    //public static string[] SplitBeforeAndAfterMeet(string str, char delimiter, bool isInclusive = false, bool enclosingQuotationMarkRemoved = false) {
    //  string[] words = new string[2]; //the returned string must be of size 2 for successful case. Will return null otherwise
    //  words[0] = ReadUntilMeet(str, delimiter, isInclusive);
    //  words[1] = ReadAfterMeet(str, delimiter, enclosingQuotationMarkRemoved);
    //  return words[0] == null || words[1] == null ? null : words;
    //}

    //public static string[] SplitBeforeAndAfterMeet(string str, string delimiter, bool isInclusive = false, bool enclosingQuotationMarkRemoved = false) {
    //  string[] words = new string[2]; //the returned string must be of size 2 for successful case. Will return null otherwise
    //  words[0] = ReadUntilMeet(str, delimiter, isInclusive);
    //  words[1] = ReadAfterMeet(str, delimiter, enclosingQuotationMarkRemoved);
    //  return words[0] == null || words[1] == null ? null : words;
    //}

    ////public static string[] SplitBeforeAndAfterMeet(string str, char delimiter, bool enclosureRemoval = false, char enclosureStart = '\0', char enclosureEnd = '\0') {
    ////  string[] words = SplitBeforeAndAfterMeet
    ////}

    //public static string[] SplitBeforeAndAfterMeet(string str, string delimiter, bool isInclusive = false, bool enclosingQuotationMarkRemoved = false) {
    //  string[] words = new string[2]; //the returned string must be of size 2 for successful case. Will return null otherwise
    //  words[0] = ReadUntilMeet(str, delimiter, isInclusive);
    //  words[1] = ReadAfterMeet(str, delimiter, enclosingQuotationMarkRemoved);
    //  return words[0] == null || words[1] == null ? null : words;
    //}

    ////public static List<string> GetAllValidLinesWithMark(string filepath, bool isTrimmed = false, params char[] marks) { //Either having the mark or not having the mark to be returned
    ////  List<string> validLines = GetAllValidLines(filepath, isTrimmed);
    ////  if (validLines == null || validLines 
    ////}

    ////public static List<string> GetAllValidLinesWithoutMark(string filepath, bool isTrimmed = false, params char[] marks) { //Either having the mark or not having the mark to be returned
    ////}

    //private static void processStripMark(ref List<string> validLines, string validLine, bool isTrimmed, bool stripmark, char[] strippedmarks, char mark) {
    //  if (!stripmark) //if we don't need to strip the mark it will be straight-forward
    //    validLines.Add(validLine);
    //  if (stripmark && validLine.Length > 1 && strippedmarks != null && strippedmarks.Length > 0 && strippedmarks.Contains(mark)) { //if strip mark is to be used and we can really strip the mark and this mark is to be stripped
    //    validLine = validLine.Substring(1); //strip the mark
    //    if (!string.IsNullOrWhiteSpace(validLine)) //not becoming invalid after stripping
    //      validLines.Add(isTrimmed ? validLine.Trim() : validLine); //add according to the instruction: trimmed or not trimmed
    //  } //else we cannot really strip the mark, though we are supposed to. Then, just ignore this case!
    //}

    //private static void processStripMark(ref List<string> validLines, string validLine, bool isTrimmed, bool stripmark, char[] strippedmarks) { //without mark reference
    //  if (!stripmark) //if we don't need to strip the mark it will be straight-forward
    //    validLines.Add(validLine);
    //  if (stripmark && validLine.Length > 1 && strippedmarks != null && strippedmarks.Length > 0) { //if strip mark is to be used and we can really strip the mark and this mark is to be stripped
    //    for (int i = 0; i < strippedmarks.Length; ++i) //check for every stripmark, see if it can really be stripped before being added
    //      if (validLine[0] == strippedmarks[i]) {
    //        validLine = validLine.Substring(1); //strip the mark
    //        if (!string.IsNullOrWhiteSpace(validLine)) //not becoming invalid after stripping
    //          validLines.Add(isTrimmed ? validLine.Trim() : validLine); //add according to the instruction: trimmed or not trimmed
    //        break;
    //      }
    //  } //else we cannot really strip the mark, though we are supposed to. Then, just ignore this case!
    //}

    ////isTrimmed -> to trim the line obtained
    ////markInclusionExclusion -> true means inclusion, false means exclusion, as indicated by marks. Example true, '#' will only include lines with '#' as the first char while false, '#' will include all but lines with '#' as the first char
    ////stripmark -> makes the mark stripped before considered valid once more, done before considered valid (can be used for both inclusion and exclusion case, just before things are added)
    ////marks -> the marks to be included or excluded
    ////strippedmarks -> for inclusion case: onyl mark among the included will be counter. 
    ////                 for exclusion, some remainder can be checked whether things with marks among the included are to be stripped first before considered valid or not
    ////                 if mark is not a concerned, this array can still be used to strip the unwanted mark
    //public static List<string> GetAllValidLines(string filepath, bool isTrimmed = false, bool markInclusionExclusion = false, char[] marks = null, bool stripmark = false, char[] strippedmarks = null) { 
    //  StreamReader sr = null;
    //  try {
    //    sr = new StreamReader(filepath);
    //    List<string> validLines = new List<string>();
    //    while (sr.EndOfStream == false) {
    //      string line = sr.ReadLine(); //Read line by line
    //      if (string.IsNullOrWhiteSpace(line)) //Proceeds if the string is not null or empty or white space              
    //        continue;
    //      string validLine = isTrimmed ? line.Trim() : line; //at this point, it cannot be whitespace
    //      if (marks != null && marks.Length > 0) { //if marks is specified, it means, further process is needed before 
    //        bool hasmark = false; //initiates this as false
    //        for (int i = 0; i < marks.Length; ++i) //check for every mark
    //          if (validLine[0] == marks[i]) { //If mark is found, we will always "break" after processing this line
    //            if (markInclusionExclusion) //and is supposed to be included
    //              processStripMark(ref validLines, validLine, isTrimmed, stripmark, strippedmarks, marks[i]);
    //            hasmark = true; //hasmark is always true for this case
    //            break; //because the mark has necessarily been found
    //          }
    //        if (!hasmark && !markInclusionExclusion) //if mark is supposed to be excluded and there isn't such mark detected, then we can include this line
    //          processStripMark(ref validLines, validLine, isTrimmed, stripmark, strippedmarks);
    //      } else //going out of here means mark is not a concern
    //        processStripMark(ref validLines, validLine, isTrimmed, stripmark, strippedmarks);
    //    }
    //    sr.Close();
    //    return validLines;
    //  } catch {
    //    if (sr != null)
    //      sr.Close();       
    //    return null;
    //  }
    //}

    //public static List<string> GetAllLines(string filepath) {
    //  StreamReader sr = null;
    //  try {
    //    sr = new StreamReader(filepath);
    //    List<string> lines = new List<string>();
    //    while (sr.EndOfStream == false)          
    //      lines.Add(sr.ReadLine());        
    //    sr.Close();
    //    return lines;
    //  } catch {
    //    if (sr != null)
    //      sr.Close();
    //    return null;
    //  }
    //}

    //public static byte[] ReadFileStream(string filepath) {
    //  try {
    //    BinaryReader binReader = new BinaryReader(File.Open(filepath, FileMode.Open));
    //    MemoryStream memStream = new MemoryStream();
    //    bool isEndOfFile = false;
    //    byte readByte = 0;
    //    while (!isEndOfFile) {
    //      try {
    //        readByte = binReader.ReadByte();
    //        memStream.WriteByte(readByte);
    //      } catch {
    //        isEndOfFile = true;
    //      }
    //    }
    //    binReader.Close();
    //    return memStream.ToArray();
    //  } catch {
    //    return null;
    //  }
    //}

