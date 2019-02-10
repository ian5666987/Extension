using System;

namespace Extension.Manipulator
{
  public class Key
  {
    private static char mapKeyPadNoToChar(int eVal) {
      switch (eVal) {
        case 0x30: return ')';
        case 0x31: return '!';
        case 0x32: return '@';
        case 0x33: return '#';
        case 0x34: return '$';
        case 0x35: return '%';
        case 0x36: return '^';
        case 0x37: return '&';
        case 0x38: return '*';
        case 0x39: return '(';
        default: return '\0';
      }
    }

    private static char mapKeyPadSpecialCharToChar(int eVal, bool isShifted) {
      switch (eVal) {
        case 186: return isShifted ? ':' : ';';
        case 187: return isShifted ? '+' : '=';
        case 188: return isShifted ? '<' : ',';
        case 189: return isShifted ? '_' : '-';
        case 190: return isShifted ? '>' : '.';
        case 191: return isShifted ? '?' : '/';
        case 192: return isShifted ? '~' : '`';
        case 219: return isShifted ? '{' : '[';
        case 220: return isShifted ? '|' : '\\';
        case 221: return isShifted ? '}' : ']';
        case 222: return isShifted ? '"' : '\'';
        default: return '\0';
      }
    }

    private static char mapMiscSpecialCharToChar(int eVal) {
      switch (eVal) {
        case 0x20: return ' '; //Space
        case 106: return '*';
        case 107: return '+';
        case 109: return '-';
        case 110: return '.';
        case 111: return '/';
        default: return '\0';
      }
    }

    public static char GetCharFromIntShiftLocks(int keyVal, bool isShifted = false, bool isCaps = false, bool isNum = false, bool isScroll = false) {
      if (isNum && (keyVal >= 0x60 && keyVal <= 0x69)) //NumLock is locked and numpad is pressed
        return Convert.ToChar(keyVal - 0x30);
      if ((keyVal >= 186 && keyVal <= 192) || (keyVal >= 219 && keyVal <= 222)) //special chars on keyPad
        return mapKeyPadSpecialCharToChar(keyVal, isShifted);
      if (keyVal >= 0x30 && keyVal <= 0x39) //numbers on keyPad
        return isShifted ? mapKeyPadNoToChar(keyVal) : Convert.ToChar(keyVal);
      if (keyVal >= 0x41 && keyVal <= 0x5A) //alphabet case
        return isCaps ? Convert.ToChar(isShifted ? keyVal + 0x20 : keyVal) : Convert.ToChar(isShifted ? keyVal : keyVal + 0x20); //alphabets to lower/upper
      return mapMiscSpecialCharToChar(keyVal);
    }
  }
}
