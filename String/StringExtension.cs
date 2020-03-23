using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extension.String
{
  public static class StringExtension {

    private static void addCharToStringInParseAsArgs(ref List<string> result, ref List<char> chs) {
      if (chs != null & chs.Count > 0) {//only adds to the result list if there is anything in the char list
        string s = new string(chs.ToArray());
        if (!string.IsNullOrWhiteSpace(s))
          result.Add(s);
        chs.Clear(); //for next argument
      }
    }

    //Currently, this does not detect error
    public static List<string> ParseAsArgs(this string str) {
      List<string> result = new List<string>();
      List<char> chs = new List<char>();
      bool enclosure = false; //to detect " "
      foreach (char c in str) {
        if (c == '"') { //enclosure character
          enclosure = !enclosure; //flip enclosure flag
          if (!enclosure) //end of enclosed argument
            addCharToStringInParseAsArgs(ref result, ref chs);          
          continue;
        }

        if (c == ' ') { //space character
          if (enclosure) //if it is within the enclosure, simply adds whatever character it is now
            chs.Add(c);
          else  //if not, it is the end of non-enclosed argument
            addCharToStringInParseAsArgs(ref result, ref chs);
          continue;
        }

        //Other characters, enclosed or not, simply adds
        chs.Add(c);
      }
      if (!enclosure) //if the enclosure flag is not raised by the end of the parsing
        addCharToStringInParseAsArgs(ref result, ref chs); //final addition if there is any remaining characters
      return result;
    }

    public static List<int> IndicesOf(this string str, string value, bool allowEmpty = false) {
      if (string.IsNullOrEmpty(value) && !allowEmpty)
        throw new ArgumentException("the string to find may not be empty", "value");
      List<int> indices = new List<int>();
      if (string.IsNullOrEmpty(value))
        return indices;
      for (int index = 0; ; index += value.Length) {
        index = str.IndexOf(value, index);
        if (index == -1)
          return indices;
        indices.Add(index);
      }
    }

    public static string ToCamelBrokenString(this string str) {
      List<char> chars = new List<char>();
      char prevChar = '\0';
      int count = 0;
      foreach (char ch in str) {
        if (!char.IsUpper(ch) || prevChar == '\0' || prevChar == '_' || prevChar == ' ') { //If this char is lower, or the first, or previously is underscore, immediately adds it without any change
          chars.Add(ch == '_' ? ' ' : ch); //under score is replaced by space
        } else if (char.IsLower(prevChar) || char.IsNumber(prevChar)) { //something like ..tB.. or ..2D.., definitely new word
          chars.Add(' ');
          chars.Add(ch);
        } else if (char.IsUpper(prevChar) && count < str.Length - 1) { //something like ..TB.. upper but not the last word, check the next char
          char nextChar = str[count + 1];
          if (char.IsLower(nextChar)) //something like ..TBn.. this B is definitely a new word
            chars.Add(' ');
          chars.Add(ch); //something like TBC or TB_.. or TB2.. this B definitely belongs to old word
        } else if (char.IsUpper(ch) && count == str.Length - 1) { //Upper on the last
          chars.Add(ch);
        }
        count++;
        prevChar = ch;
      }
      return new string(chars.ToArray());
    }

    public static bool EqualsIgnoreCase(this string str, string input) {
      if (str == input)
        return true;
      if ((str == null && input != null) || (str != null && input == null)) //if one is null and the other is not, then return false
        return false;
      return str.ToLower() == input.ToLower();
    }

    public static bool EqualsIgnoreCaseTrim(this string str, string input) {
      if (str == input)
        return true;
      if ((str == null && input != null) || (str != null && input == null)) //if one is null and the other is not, then return false
        return false;
      return str.ToLower().Trim() == input.ToLower().Trim();
    }

    public static List<string> GetTrimmedParts(this string str) {
      List<string> parts = new List<string>();
      if (string.IsNullOrEmpty(str))
        return parts;
      string testStrTrimStart = str.TrimStart();
      string testStrTrim = str.Trim();
      if(str.Length > testStrTrimStart.Length)
        parts.Add(str.Substring(0, str.Length - testStrTrimStart.Length));
      if(!string.IsNullOrEmpty(testStrTrim))
        parts.Add(testStrTrim);
      if (testStrTrimStart.Length > testStrTrim.Length)
        parts.Add(testStrTrimStart.Substring(testStrTrim.Length));      
      return parts;
    }

    //Can only parse <tag>, not < tag> or < tag  > or <TaG>
    public static List<string> ParseStrictTagUntrimmed(this string str, string tag) {
      List<string> components = new List<string>();
      List<char> collectedChars = new List<char>();
      List<char> candidateChars = new List<char>();
      int candidateType = 0; //0 is not a candidate, 1 is tag, 2 is end tag
      string startTagRef = "<" + tag + ">";
      string endTagRef = "</" + tag + ">";
      for (int i = 0; i < str.Length; ++i) {
        char ch = str[i];
        if (candidateChars.Count < startTagRef.Length && ch == startTagRef[candidateChars.Count]) {
          candidateChars.Add(ch);
          candidateType = 1;
        } else if (candidateChars.Count < endTagRef.Length && ch == endTagRef[candidateChars.Count]) {
          candidateChars.Add(ch);
          candidateType = 2;
        } else {
          if (candidateChars.Count > 0) { //there is candidate but unproven
            collectedChars.AddRange(candidateChars); //move the candidate to collected
            candidateChars.Clear();
          }
          candidateType = 0;
          collectedChars.Add(ch); //no matter what happened, take this char as collected
        }

        if ((candidateChars.Count >= startTagRef.Length && candidateType == 1) || //The candidate is proven to be correct
          (candidateChars.Count >= endTagRef.Length && candidateType == 2)) {
          if(collectedChars.Count > 0) {
            string collectedString = new string(collectedChars.ToArray());
            collectedChars.Clear();
            components.Add(collectedString);
          }
          string tagString = new string(candidateChars.ToArray()); //extract the candidate
          candidateChars.Clear();
          components.Add(tagString);
          candidateType = 0;
        }
      }

      if (candidateChars.Count > 0) //failed candidate remainder
        collectedChars.AddRange(candidateChars);
      if (collectedChars.Count > 0) { //final uncollected character yet
        string finalString = new string(collectedChars.ToArray());
        components.Add(finalString);
      }

      return components;
    }

    public static Dictionary<string, string> GetSpeciallyTaggedDictionary(this string input, string tag, string frontElementStart, string frontElementEnd, string backElementStart, string backElementEnd) {
      Dictionary<string, string> dict = new Dictionary<string, string>();
      if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(tag))
        return dict;
      string startTag = string.Concat(frontElementStart, tag.Trim(), frontElementEnd);
      string endTag = string.Concat(backElementStart, tag.Trim(), backElementEnd);
      var innerTagStrings = input.Split(new string[] { endTag },
        StringSplitOptions.RemoveEmptyEntries)
        .Where(x => x.Trim().StartsWith(startTag))
        .Select(x => x.Trim().Substring(startTag.Length).Trim())
        .Where(x => !string.IsNullOrWhiteSpace(x));
      foreach (var innerTagString in innerTagStrings) {
        int index = innerTagString.IndexOf('=');
        if (index <= 0 || innerTagString.Length <= index + 1) //cannot process such
          continue;
        string columnName = innerTagString.Substring(0, index).Trim();
        string value = innerTagString.Substring(index + 1).Trim();
        if (!string.IsNullOrWhiteSpace(columnName) && !string.IsNullOrWhiteSpace(value))
          dict.Add(columnName, value);
      }
      return dict;
    }

    public static List<string> GetSpeciallyTaggedInnerStrings(this string input, string tag, string frontElementStart, string frontElementEnd, string backElementStart, string backElementEnd) {
      List<string> innerStrings = new List<string>();
      if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(tag))
        return innerStrings;
      string startTag = string.Concat(frontElementStart, tag.Trim(), frontElementEnd);
      string endTag = string.Concat(backElementStart, tag.Trim(), backElementEnd);
      return input.Split(new string[] { endTag },
        StringSplitOptions.RemoveEmptyEntries)
        .Where(x => x.Trim().StartsWith(startTag))
        .Select(x => x.Trim().Substring(startTag.Length).Trim())
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .ToList();
    }

    public static Dictionary<string, string> GetXMLTaggedDictionary(this string input, string tag) {
      Dictionary<string, string> dict = new Dictionary<string, string>();
      if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(tag))
        return dict;
      string startTag = string.Concat("<", tag.Trim(), ">");
      string endTag = string.Concat("</", tag.Trim(), ">");
      var innerTagStrings = input.Split(new string[] { endTag },
        StringSplitOptions.RemoveEmptyEntries)
        .Where(x => x.Trim().StartsWith(startTag))
        .Select(x => x.Trim().Substring(startTag.Length).Trim())
        .Where(x => !string.IsNullOrWhiteSpace(x));
      foreach (var innerTagString in innerTagStrings) {
        int index = innerTagString.IndexOf('=');
        if (index <= 0 || innerTagString.Length <= index + 1) //cannot process such
          continue;
        string columnName = innerTagString.Substring(0, index).Trim();
        string value = innerTagString.Substring(index + 1).Trim();
        if (!string.IsNullOrWhiteSpace(columnName) && !string.IsNullOrWhiteSpace(value))
          dict.Add(columnName, value);
      }
      return dict;
    }

    public static List<string> GetXMLTaggedInnerStrings(this string input, string tag) {
      List<string> innerStrings = new List<string>();
      if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(tag))
        return innerStrings;
      string startTag = string.Concat("<", tag.Trim(), ">");
      string endTag = string.Concat("</", tag.Trim(), ">");
      return input.Split(new string[] { endTag },
        StringSplitOptions.RemoveEmptyEntries)
        .Where(x => x.Trim().StartsWith(startTag))
        .Select(x => x.Trim().Substring(startTag.Length).Trim())
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .ToList();
    }

    public static List<string> GetTrimmedNonEmptyParts(this string input, char splitChar) {
      return input.Split(splitChar).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();
    }

    public static List<string> GetTrimmedNonEmptyParts(this string input, char[] splitChars) {
      return input.Split(splitChars).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();
    }

    public static List<string> GetTrimmedNonEmptyParts(this string input, string splitStr) {
      return input.Split(new string[] { splitStr }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
    }

    public static List<string> GetTrimmedNonEmptyParts(this string input, string[] splitStrs) {
      return input.Split(splitStrs, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
    }

    public static List<string> GetTrimmedParts(this string input, char splitChar) {
      return input.Split(splitChar).Where(x => x != null).Select(x => x.Trim()).ToList();
    }

    public static List<string> GetTrimmedParts(this string input, char[] splitChars) {
      return input.Split(splitChars).Where(x => x != null).Select(x => x.Trim()).ToList();
    }

    public static List<string> GetTrimmedParts(this string input, string splitStr) {
      return input.Split(new string[] { splitStr }, StringSplitOptions.None).Select(x => x.Trim()).ToList();
    }

    public static List<string> GetTrimmedParts(this string input, string[] splitStrs) {
      return input.Split(splitStrs, StringSplitOptions.None).Select(x => x.Trim()).ToList();
    }

    public static string GetNonEmptyTrimmedInBetween(this string input, string start, string end) {
      string inputTrimmed = input.Trim();
      if (!inputTrimmed.StartsWith(start) || !inputTrimmed.EndsWith(end) || inputTrimmed.Length <= 2) //wrong too
        return null;
      string content = inputTrimmed.Substring(1, inputTrimmed.Length - 2).Trim();
      return string.IsNullOrWhiteSpace(content) ? null : content;
    }

    public static object Convert(this string input, Type type, string dtFormat = null) {
      object result = null;
      if (input == null || type == null)
        return null;
      if (type == typeof(string))
        return input;
      if (type == typeof(int))
        return int.Parse(input);
      if (type == typeof(uint))
        return uint.Parse(input);
      if (type == typeof(DateTime))
        return string.IsNullOrWhiteSpace(dtFormat) ? DateTime.Parse(input) : DateTime.ParseExact(input, dtFormat, null);
      if (type == typeof(double))
        return double.Parse(input);
      if (type == typeof(decimal))
        return decimal.Parse(input);
      if (type == typeof(bool))
        return bool.Parse(input);
      if (type == typeof(long))
        return long.Parse(input);
      if (type == typeof(ulong))
        return ulong.Parse(input);
      if (type == typeof(float))
        return float.Parse(input);
      if (type == typeof(short))
        return short.Parse(input);
      if (type == typeof(ushort))
        return ushort.Parse(input);
      if (type == typeof(byte))
        return byte.Parse(input);
      if (type == typeof(sbyte))
        return sbyte.Parse(input);
      return result;
    }

    public static bool TryConvert(this string input, Type type, out object output, string dtFormat = null) {
      output = null;
      try {
        output = Convert(input, type, dtFormat);
        return true;
      } catch {
        return false;
      }
    }

    public static string AsSqlStringValueWithNull(this string input) {
      return string.IsNullOrWhiteSpace(input) ? "NULL" :
        "'" + input.Replace("'", "''") + "'";
    }

    public static string AsSqlStringValue(this string input) {
      return "'" + GetSqlSafeStringValue(input) + "'";
    }

    public static string GetSqlSafeStringValue(this string input) {
      return string.IsNullOrWhiteSpace(input) ? string.Empty : input.Replace("'", "''");
    }

    public static string AsCsvStringValue(this string input) {
      return "\"" + GetCsvSafeStringValue(input) + "\"";
    }

    public static string GetCsvSafeStringValue(this string input) {
      return string.IsNullOrWhiteSpace(input) ? string.Empty : input.Replace("\"", "\"\"");
    }

    public static List<KeyValuePair<int, char>> GetBreaks(this string input, List<char> breaks, int offset) {
      List<KeyValuePair<int, char>> results = new List<KeyValuePair<int, char>>();
      if (string.IsNullOrWhiteSpace(input) || breaks == null || breaks.Count <= 0)
        return results;
      for (int i = 0; i < input.Length; ++i)
        if (breaks.Any(x => x == input[i]))
          results.Add(new KeyValuePair<int, char>(offset + i, input[i]));
      return results;
    }

    public static int MostMatchedIndex(this string input, List<string> items) {
      if (items == null || items.Count <= 0 || string.IsNullOrWhiteSpace(input))
        return -1;
      if (items.Count == 1) //definitely return the first one
        return 0;
      var sug = items.Select((x, i) => new { Index = i, Value = x })
        .FirstOrDefault(x => x.Value == input); //if completely matched, then it is the most suggested
      if (sug != null)
        return sug.Index;
      sug = items.Select((x, i) => new { Index = i, Value = x })
        .FirstOrDefault(x => x.Value.ToLower() == input.ToLower()); //matched when its case is lowered, second best
      if (sug != null)
        return sug.Index;
      sug = items.Select((x, i) => new { Index = i, Value = x })
        .FirstOrDefault(x => x.Value.StartsWith(input)); //matched some at start
      if (sug != null)
        return sug.Index;
      sug = items.Select((x, i) => new { Index = i, Value = x })
        .FirstOrDefault(x => x.Value.ToLower().StartsWith(input.ToLower())); //matched some at start when lower-cased
      if (sug != null)
        return sug.Index;
      sug = items.Select((x, i) => new { Index = i, Value = x })
        .FirstOrDefault(x => x.Value.Contains(input)); //matched some
      if (sug != null)
        return sug.Index;
      sug = items.Select((x, i) => new { Index = i, Value = x })
        .FirstOrDefault(x => x.Value.ToLower().Contains(input.ToLower())); //matched some when lower-cased
      if (sug != null)
        return sug.Index;
      return -1; //nothing really matched
    }

    public static bool PartiallyMatchWith(this string input, string part, bool matchIfEmpty = false) {
      if (input == part) //Highest priority, match exactly
        return true;
      if (string.IsNullOrEmpty(part) && matchIfEmpty)
        return true;
      if (input.ToLower().Contains(part.ToLower())) //second priority, match when the sequence of characters match
        return true;
      var words = input.ToCamelBrokenString().Split(' ').ToList(); //break by words
      var subparts = part.ToCamelBrokenString().Split(' ').ToList();
      int partCount = subparts.Count;
      int matchCount = 0;
      foreach (var subpart in subparts)
        matchCount += words.Contains(subpart) ? 1 : 0;
      return matchCount >= partCount;
    }

    public static List<int> IndexSequenceOf(this string input, List<char> sequence) {
      if (sequence == null)
        return new List<int>();
      return IndexSequenceOf(input, sequence.Select(x => x.ToString()).ToList());
    }

    public static List<int> IndexSequenceOf(this string input, List<string> sequence) {
      if (sequence == null || sequence.Count <= 0)
        return new List<int>();
      List<int> results = Enumerable.Repeat(-1, sequence.Count).ToList();
      string text = input;
      int accumulatedIndex = 0;
      for (int i = 0; i < sequence.Count; ++i) {
        int index = text.IndexOf(sequence[i]);
        if (index < 0)
          break;
        results[i] = index + accumulatedIndex; //aa=bb length = 5, index + 1 = 3, length must be at least greater than index + 1 (that is, 4)
        if (text.Length <= index + 1)
          break;
        accumulatedIndex += index + 1;
        text = text.Substring(index + 1);
      }
      return results;
    }

    private static List<char> defaultEncloseChars = new List<char>() { '\'', '"' };
    public static List<int> ComponentIndices(this string input, char parseChar = ';', bool allowWsString = false,
      List<char> encloseChars = null) {
      List<int> components = new List<int>();
      if (string.IsNullOrWhiteSpace(input)) { //probably has nothing
        if (!string.IsNullOrEmpty(input) && allowWsString) //has something and is allowed
          components.Add(0);
        return components;
      }

      List<char> usedEncloseChars = encloseChars == null || encloseChars.Count <= 0 ? defaultEncloseChars : encloseChars;
      char encloseCharFound = '\0';
      char[] chArr = input.ToCharArray();
      char nextChar;
      int componentIndex = 0; 

      for (int i = 0; i < chArr.Length; ++i) {
        char ch = chArr[i];
        if (encloseCharFound != '\0') { //enclose char found means must be completed till the end, surpassing bracket and square bracket
          if (ch == encloseCharFound) { //two possibilities, closing enclose-char or double enclose-char
            if (i == chArr.Length - 1) { //the closing enclose-char for sure, the last char              
              encloseCharFound = '\0'; //reset back
            } else { //means enclosing character and not the last one
              nextChar = chArr[i + 1]; //test next char
              if (nextChar == encloseCharFound) { //double enclose-char
                i++; //skip the next check
              } else { //closing enclose-char
                encloseCharFound = '\0'; //reset back
              }
            }
          }
          continue;
        }
        if (ch == parseChar) { //parseChar is found when not in the open enclose-char, means this is one component
          components.Add(componentIndex);
          if (i != chArr.Length - 1) //not the last one, thus, the next one will be the starting component index, if there is any
            componentIndex = i + 1;
        } else if (usedEncloseChars.Any(x => x == ch)) //the open enclose-char is found here
          encloseCharFound = ch; //add this enclose-char character, thus it will go up        
      }

      if (!components.Contains(componentIndex)) //last component index included
        components.Add(componentIndex);

      return components;
    }

    //Untrimmed
    public static List<string> ParseComponents(this string input, char parseChar = ';', bool allowLastWsString = false, 
      char encloseChar = '\'') {
      List<string> components = new List<string>();
      StringBuilder sb = new StringBuilder();
      bool encloseCharFound = false;
      char[] chArr = input.ToCharArray();
      char nextChar;

      for (int i = 0; i < chArr.Length; ++i) {
        char ch = chArr[i];
        if (encloseCharFound) { //enclose char found means must be completed till the end, surpassing bracket and square bracket
          if (ch == encloseChar) { //two possibilities, closing enclose-char or double enclose-char
            if (i == chArr.Length - 1) { //the closing enclose-char for sure
              sb.Append(ch);
              components.Add(sb.ToString());
              sb = new StringBuilder();
              encloseCharFound = false;
            } else {
              nextChar = chArr[i + 1]; //test next char
              if (nextChar == encloseChar) { //double enclose-char
                sb.Append(encloseChar + encloseChar); //put double enclose-chars to the current component
                i++; //skip the next check
              } else { //closing enclose-char
                sb.Append(ch);
                encloseCharFound = false;
              }
            }
          } else
            sb.Append(ch);
          continue;
        } else {
          if (ch == parseChar) { //parseChar is found when not in the open enclose-char, means this is one component
            components.Add(sb.ToString());
            sb = new StringBuilder();
          } else if (ch == encloseChar) { //the open enclose-char is found here
            encloseCharFound = true;
            sb.Append(ch); //add this enclose-char character
          } else
            sb.Append(ch); //if not parseChar, just add the element
        }
      }

      string str = sb.ToString(); //last component
      if (!string.IsNullOrWhiteSpace(str) || (str != null && str.Length > 0 && allowLastWsString))
        components.Add(str);
      return components;
    }

    private static List<KeyValuePair<char, char>> defaultEncloseCharPairs = new List<KeyValuePair<char, char>> {
      new KeyValuePair<char, char>('(', ')'),
    };
    //Untrimmed
    public static List<string> ParseComponentsWithEnclosurePairs(this string input, char parseChar = ';', bool allowLastWsString = false,
      List<KeyValuePair<char, char>> encloseCharPairs = null) {
      List<string> components = new List<string>();
      List<KeyValuePair<char, char>> usedEncloseChars = encloseCharPairs == null || encloseCharPairs.Count <= 0 ? 
        defaultEncloseCharPairs : encloseCharPairs;
      StringBuilder sb = new StringBuilder();
      bool startEnclosureCharFound = false;
      char[] chArr = input.ToCharArray();
      int depth = 0;
      //char nextChar;
      char foundStartEnclosure = '\0';
      char expectedEndEnclosure = '\0';

      for (int i = 0; i < chArr.Length; ++i) {
        char ch = chArr[i];
        if (startEnclosureCharFound) { //enclose char found means must be completed till the end, surpassing bracket and square bracket
          if (ch == expectedEndEnclosure) { //two possibilities, closing enclose-char or double enclose-char
            if (depth == 1) { //if the depth is exactly 1, then we can process this
              if (i == chArr.Length - 1) { //the closing enclose-char for sure
                sb.Append(ch);
                components.Add(sb.ToString());
                sb = new StringBuilder();
                startEnclosureCharFound = false;
                foundStartEnclosure = '\0';
                expectedEndEnclosure = '\0';
              } else {
                //nextChar = chArr[i + 1]; //test next char
                //if (nextChar == encloseChar) { //double enclose-char
                //  sb.Append(encloseChar + encloseChar); //put double enclose-chars to the current component
                //  i++; //skip the next check
                //} else { //closing enclose-char
                sb.Append(ch);
                startEnclosureCharFound = false;
                foundStartEnclosure = '\0';
                expectedEndEnclosure = '\0';
                //}
              }
            } else {
              sb.Append(ch); //remember to record the character!
              --depth; //if the depth is greater than 1, then reduce it first
            }
          } else if (ch == foundStartEnclosure) { //if the start enclosure is found again, then increase the depth here
            sb.Append(ch); //remember to record the character!
            ++depth;
          } else
            sb.Append(ch);
          continue;
        } else {
          if (ch == parseChar) { //parseChar is found when not in the open enclose-char, means this is one component
            components.Add(sb.ToString());
            sb = new StringBuilder();
          } else if (usedEncloseChars.Any(x => x.Key == ch)) { //the open enclose-char is found here
            KeyValuePair<char, char> kvp = usedEncloseChars.FirstOrDefault(x => x.Key == ch);
            foundStartEnclosure = kvp.Key;
            expectedEndEnclosure = kvp.Value;
            startEnclosureCharFound = true;
            depth = 1;
            sb.Append(ch); //add this enclose-char character
          } else
            sb.Append(ch); //if not parseChar, just add the element
        }
      }

      string str = sb.ToString(); //last component
      if (!string.IsNullOrWhiteSpace(str) || (str != null && str.Length > 0 && allowLastWsString))
        components.Add(str);
      return components;
    }

    //'O''Neil' -> O'Neil
    //'' -> string.Empty
    //'Bryant' -> Bryant
    //50 -> 50
    //76.876 -> 76.876
    public static string ExtractSqlValue(this string input) {
      if (!input.StartsWith("'") || !input.EndsWith("'")) //not a string, return as they are
        return input;
      //string
      return input.Length > 2 ? //has something inside
        input.Substring(1, input.Length - 2).Replace("''", "'") : //replace the string with its equivalent value
        string.Empty;  //does not have anything inside, but is not NULL
    }

    //"Oh, ""Conspiracy""!" -> Oh, "Conspiracy"!
    //"" -> string.Empty
    //"Bryant" -> Bryant
    //50 -> 50
    //76.876 -> 76.876
    public static string ExtractCsvInput(this string input) {
      if (!input.StartsWith("\"") || !input.EndsWith("\"")) //not a string, return as they are
        return input;
      //string
      return input.Length > 2 ? //has something inside
        input.Substring(1, input.Length - 2).Replace("\"", "\"") : //replace the string with its equivalent value
        string.Empty;  //does not have anything inside, but is not NULL
    }

    private static string toBitArrayString(BitArray b, int spacePerByte = 1, int spacePerNibble = 0) {
      StringBuilder sb = new StringBuilder();
      for (int j = b.Length - 1; j >= 0; --j) {
        int i = b.Length - 1 - j;
        if (i % 4 == 0 && i > 0 && spacePerNibble > 0)
          sb.Append(string.Concat(Enumerable.Repeat(" ", spacePerNibble)));
        if (i % 8 == 0 && i > 0 && spacePerByte > 0)
          sb.Append(string.Concat(Enumerable.Repeat(" ", spacePerByte)));
        sb.Append(b[j] ? 1 : 0);
      }
      return sb.ToString();
    }

    private static string toBitBoardString(BitArray b, int spacePerBit = 1) {
      StringBuilder sb = new StringBuilder();
      for (int j = b.Length - 1; j >= 0; j -= 8) {
        for(int i = 7; i >= 0; --i) {
          int k = j - i;
          sb.Append(b[k] ? 1 : 0);
          if (i > 0 && spacePerBit > 0)
            sb.Append(string.Concat(Enumerable.Repeat(" ", spacePerBit)));
        }
        sb.Append("\n");
      }
      return sb.ToString();
    }

    public static string ToBitBoardString(this long input, int spacePerBit = 1) {
      BitArray b = new BitArray(new int[] { (int)(input & 0xFFFFFFFF), (int)(input >> 32) });
      return toBitBoardString(b, spacePerBit);
    }

    public static string ToBitBoardString(this ulong input, int spacePerBit = 1) {
      BitArray b = new BitArray(new int[] { (int)(input & 0xFFFFFFFF), (int)(input >> 32) });
      return toBitBoardString(b, spacePerBit);
    }

    public static string ToBitArrayString(this int input, int spacePerByte = 1, int spacePerNibble = 0) {
      BitArray b = new BitArray(new int[] { input });
      return toBitArrayString(b, spacePerByte, spacePerNibble);
    }

    public static string ToBitArrayString(this byte input, int spacePerByte = 1, int spacePerNibble = 0) {
      BitArray b = new BitArray(new byte[] { input });
      return toBitArrayString(b, spacePerByte, spacePerNibble);
    }

    public static string ToBitArrayString(this short input, int spacePerByte = 1, int spacePerNibble = 0) {
      BitArray b = new BitArray(new byte[] { (byte)(input & 0xFF), (byte)(input >> 8) });
      return toBitArrayString(b, spacePerByte, spacePerNibble);
    }

    public static string ToBitArrayString(this long input, int spacePerByte = 1, int spacePerNibble = 0) {
      BitArray b = new BitArray(new int[] { (int)(input & 0xFFFFFFFF), (int)(input >> 32) });
      return toBitArrayString(b, spacePerByte, spacePerNibble);
    }

    public static string ToBitArrayString(this uint input, int spacePerByte = 1, int spacePerNibble = 0) {
      BitArray b = new BitArray(new int[] { (int)input });
      return toBitArrayString(b, spacePerByte, spacePerNibble);
    }

    public static string ToBitArrayString(this sbyte input, int spacePerByte = 1, int spacePerNibble = 0) {
      BitArray b = new BitArray(new byte[] { (byte)input });
      return toBitArrayString(b, spacePerByte, spacePerNibble);
    }

    public static string ToBitArrayString(this ushort input, int spacePerByte = 1, int spacePerNibble = 0) {
      BitArray b = new BitArray(new byte[] { (byte)(input & 0xFF), (byte)(input >> 8) });
      return toBitArrayString(b, spacePerByte, spacePerNibble);
    }

    public static string ToBitArrayString(this ulong input, int spacePerByte = 1, int spacePerNibble = 0) {
      BitArray b = new BitArray(new int[] { (int)(input & 0xFFFFFFFF), (int)(input >> 32) });
      return toBitArrayString(b, spacePerByte, spacePerNibble);
    }
  }
}
