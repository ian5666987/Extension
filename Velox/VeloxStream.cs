using System.Collections.Generic;
using Extension.Developer;

namespace Extension.Velox
{
  class VeloxStream : GenericDataStream
  {    
    public static string[] GetVelox1MainTabWords(string validLine) {
      if (string.IsNullOrWhiteSpace(validLine))
        return null; //If the validLine is Null or WhiteSpace then it is not word trying further
      string velox1TabValidLine = validLine.Trim(); //There is already something here
      string[] wordsArray = velox1TabValidLine.Split(new char[] { '\n', '\0', ';', '=' }); //the words are not trimmed here yet!                
      List<string> wordsList = new List<string>();
      for (int i = 0; i < wordsArray.Length; ++i)
        if (!string.IsNullOrWhiteSpace(wordsArray[i].Trim())) //only adds the words which are not null or white spaces
          wordsList.Add(wordsArray[i].Trim());
      return wordsList.Count > 0 ? wordsList.ToArray() : null;
    }

    //Separate RTC_Calendar_Month.combo        =[January=1, February=2, March=3, April=4, May=5, June=6, July=7, August=8, September=9, October=10, November=11, December=12]
    //into:
    // (1) Separate RTC_Calendar_Month.combo, and
    // (2) [January=1, February=2, March=3, April=4, May=5, June=6, July=7, August=8, September=9, October=10, November=11, December=12]
    public static string[] GetVelox1FirstEqualSignSeparatedWords(string validLine, char commentChar = '#') {
      if (string.IsNullOrWhiteSpace(validLine))
        return null;
      string velox1ValidLine = validLine.Trim(); //this step may result in comment char..
      if (velox1ValidLine[0] == commentChar) //skip comments from being proccessed
        return null;
      string[] words = new string[2]; //Non-comment characters, many things may happen from this point downwards
      words[0] = ReadUntilMeet(velox1ValidLine, '='); //this may give null
      words[1] = ReadAfterMeet(velox1ValidLine, '='); //this may give null
      if (words[0] == null || words[1] == null) //if any of the words is null, then returns null
        return null;
      for (int i = 0; i < words.Length; ++i)
        words[i] = words[i].Trim();
      return words;
    }

    public static string[] GetVelox1ScheduleForWords(string validLine) {
      if (string.IsNullOrWhiteSpace(validLine))
        return null;
      string velox1ScheduleForValidLine = validLine.Trim(); //velox1Schedule won't be null here...
      string[] words = velox1ScheduleForValidLine.Split(new char[] { '\n', '\0', ';', '=' }); //the words are not trimmed here yet!                
      for (int i = 0; i < words.Length; ++i)
        words[i] = words[i].Trim();
      return words;
    }

    public static string[] GetVelox1AckWords(string validLine) {
      if (string.IsNullOrWhiteSpace(validLine))
        return null;
      string velox1AckValidLine = validLine.Trim();
      if (velox1AckValidLine.Length <= 0 || velox1AckValidLine[0] == '#')
        return null; //Returned when comment is read
      string[] words = velox1AckValidLine.Split(new char[] { '\n', '\0', ';', '=' }); //the words are not trimmed here yet!                
      for (int i = 0; i < words.Length; ++i)
        words[i] = words[i].Trim();
      return words;
    }
  }
}


//using System.Collections.Generic;

//namespace ReaderExtension
//{
//  class VeloxReader : GenericDataReader
//  {    
//    //Some examples are like 
//    //! XDATA_OBDH_MET_OFFSET             =value=1400;type=uint16;policy=auto;
//    //! XData_Length
//    //! OBDH_Time                         =type=uint32;min=1735689635;max=1956528035;unit=UTC;
//    //OBDH_Address                    =value=0x0a;type=uint8;policy=auto;
//    //OBDH_Chip_Oscillator.combo      =[external=0, internal=1]
//    //public static string[] GetVelox1ParameterWords(string validLine, bool isExclamationMarkAppearFirst = false) {
//    //  if (string.IsNullOrWhiteSpace(validLine))
//    //    return null;
//    //  string velox1ValidLine = validLine.Trim();
//    //  if (!(velox1ValidLine[0] == '!' || !isExclamationMarkAppearFirst) || (velox1ValidLine[0] == '#'))
//    //    return null; //Only get specific VELOX-I valid line

//    //  string trimmedString = isExclamationMarkAppearFirst ? velox1ValidLine.Substring(1).Trim() : velox1ValidLine;
//    //  char[] delimiterChars = { '\n', '\0', ';', '=' };
//    //  string[] words = trimmedString.Split(delimiterChars); //the words are not trimmed here yet!                
//    //  List<string> wordsList = new List<string>();
//    //  for (int i = 0; i < words.Length; ++i)
//    //    if (!string.IsNullOrWhiteSpace(words[i].Trim())) //only adds the words which are not null or white spaces
//    //      wordsList.Add(words[i].Trim());
//    //  if (string.IsNullOrWhiteSpace(wordsList[0])) //if the first word is not there, returns null
//    //    return null;

//    //  //. (dot) implies [something=something]
//    //  char[] delimiterCharsFirstWord = { '.' };  //But words could be read differently here if dot exists in the first word
//    //  string[] wordsFirst = wordsList[0].Split(delimiterCharsFirstWord); //Split the special first word further
//    //  if (wordsFirst.Length <= 1)
//    //    return wordsList.ToArray(); //by default, return the words, unless it is special firstword case
//    //  string wordInsideBracket = ReadInBetweenSameDepth(velox1ValidLine, '[', ']'); //Means there is something fishy here, because . (dot) presents
//    //  if (string.IsNullOrWhiteSpace(wordInsideBracket))
//    //    return null; //If the words inside the bracket is invalid (undefined), return null
//    //  string[] wordsNew = new string[2];
//    //  wordsNew[0] = wordsList[0]; //wordsList[0] is something.combo etc...
//    //  wordsNew[1] = wordInsideBracket;
//    //  return wordsNew; //VERY SPECIAL return here because of the taking of the something.combo and the blabla0=0, blabla1=1      

//    //  //if (string.IsNullOrWhiteSpace(validLine)) 
//    //  //  return null;
//    //  //try {
//    //  //  string velox1ValidLine = validLine.Trim();
//    //  //  if ((velox1ValidLine[0] == '!' || !isExclamationMarkAppearFirst) && (velox1ValidLine[0] != '#')) { //Only get specific VELOX-I valid line          
//    //  //    string trimmedString = isExclamationMarkAppearFirst ? velox1ValidLine.Substring(1) : velox1ValidLine;
//    //  //    char[] delimiterChars = { '\n', '\0', ';', '=' };
//    //  //    string[] words = trimmedString.Split(delimiterChars); //the words are not trimmed here yet!                
//    //  //    for (int i = 0; i < words.Length; ++i)
//    //  //      words[i] = words[i].Trim(); //ok, this is the default way of reading.

//    //  //    char[] delimiterCharsFirstWord = { '.' };  //But words could be read differently here if dot exists in the first word
//    //  //    string[] wordsFirst = words[0].Split(delimiterCharsFirstWord); //Split the special first word further          
//    //  //    if (wordsFirst.Length > 1) { //Means there is something fishy here!  
//    //  //      string wordInsideBracket = ReadInBetweenSameDepth(velox1ValidLine, '[', ']');
//    //  //      if (!string.IsNullOrWhiteSpace(wordInsideBracket)) {
//    //  //        string[] wordsNew = new string[2];
//    //  //        wordsNew[0] = words[0];
//    //  //        wordsNew[1] = wordInsideBracket;
//    //  //        return wordsNew;
//    //  //      } else //If undefined, return null
//    //  //        return null;
//    //  //    }          
//    //  //    return words; //by default, return the words, unless it is special firstword case
//    //  //  }
//    //  //} catch {
//    //  //}
//    //  //return null;
//    //}

//    public static string[] GetVelox1MainTabWords(string validLine) {
//      if (string.IsNullOrWhiteSpace(validLine))
//        return null; //If the validLine is Null or WhiteSpace then it is not word trying further
//      string velox1TabValidLine = validLine.Trim(); //There is already something here
//      string[] wordsArray = velox1TabValidLine.Split(new char[] { '\n', '\0', ';', '=' }); //the words are not trimmed here yet!                
//      List<string> wordsList = new List<string>();
//      for (int i = 0; i < wordsArray.Length; ++i)
//        if (!string.IsNullOrWhiteSpace(wordsArray[i].Trim())) //only adds the words which are not null or white spaces
//          wordsList.Add(wordsArray[i].Trim());
//      return wordsList.Count > 0 ? wordsList.ToArray() : null;
//      //try {
//      //  string velox1TabValidLine = validLine.Trim();
//      //  char[] delimiterChars = { '\n', '\0', ';', '=' };
//      //  string[] words = velox1TabValidLine.Split(delimiterChars); //the words are not trimmed here yet!                
//      //  for (int i = 0; i < words.Length; ++i)
//      //    words[i] = words[i].Trim();
//      //  return words;
//      //} catch {
//      //}
//      //return null;
//    }

//    //Separate RTC_Calendar_Month.combo        =[January=1, February=2, March=3, April=4, May=5, June=6, July=7, August=8, September=9, October=10, November=11, December=12]
//    //into:
//    // (1) Separate RTC_Calendar_Month.combo, and
//    // (2) [January=1, February=2, March=3, April=4, May=5, June=6, July=7, August=8, September=9, October=10, November=11, December=12]
//    public static string[] GetVelox1FirstEqualSignSeparatedWords(string validLine, char commentChar = '#') {
//      if (string.IsNullOrWhiteSpace(validLine))
//        return null;
//      string velox1ValidLine = validLine.Trim(); //this step may result in comment char..
//      if (velox1ValidLine[0] == commentChar) //skip comments from being proccessed
//        return null;
//      string[] words = new string[2]; //Non-comment characters, many things may happen from this point downwards
//      words[0] = StreamReaderExtension.ReadUntilMeet(velox1ValidLine, '='); //this may give null
//      words[1] = StreamReaderExtension.ReadAfterMeet(velox1ValidLine, '='); //this may give null
//      if (words[0] == null || words[1] == null) //if any of the words is null, then returns null
//        return null;
//      for (int i = 0; i < words.Length; ++i)
//        words[i] = words[i].Trim();
//      return words;
//      //try {
//      //  if (validLine[0] != commentChar) {
//      //    string velox1ValidLine = validLine.Trim();
//      //    string[] words = new string[2];
//      //    words[0] = StreamReaderExtension.ReadUntilMeet(velox1ValidLine, '=');
//      //    words[1] = StreamReaderExtension.ReadAfterMeet(velox1ValidLine, '=');
//      //    for (int i = 0; i < words.Length; ++i)
//      //      words[i] = words[i].Trim();
//      //    return words;
//      //  }
//      //} catch {
//      //}
//      //return null;
//    }

//    public static string[] GetVelox1ScheduleForWords(string validLine) {
//      if (string.IsNullOrWhiteSpace(validLine))
//        return null;
//      string velox1ScheduleForValidLine = validLine.Trim(); //velox1Schedule won't be null here...
//      string[] words = velox1ScheduleForValidLine.Split(new char[] { '\n', '\0', ';', '=' }); //the words are not trimmed here yet!                
//      for (int i = 0; i < words.Length; ++i)
//        words[i] = words[i].Trim();
//      return words;
//      //try {
//      //  string velox1ScheduleForValidLine = validLine.Trim();
//      //  char[] delimiterChars = { '\n', '\0', ';', '=' };
//      //  string[] words = velox1ScheduleForValidLine.Split(delimiterChars); //the words are not trimmed here yet!                
//      //  for (int i = 0; i < words.Length; ++i)
//      //    words[i] = words[i].Trim();
//      //  return words;
//      //} catch {
//      //}
//      //return null;
//    }

//    public static string[] GetVelox1AckWords(string validLine) {
//      if (string.IsNullOrWhiteSpace(validLine))
//        return null;
//      string velox1AckValidLine = validLine.Trim();
//      if (velox1AckValidLine.Length <= 0 || velox1AckValidLine[0] == '#')
//        return null; //Returned when comment is read
//      string[] words = velox1AckValidLine.Split(new char[] { '\n', '\0', ';', '=' }); //the words are not trimmed here yet!                
//      for (int i = 0; i < words.Length; ++i)
//        words[i] = words[i].Trim();
//      return words;
//    }
//  }
//}

