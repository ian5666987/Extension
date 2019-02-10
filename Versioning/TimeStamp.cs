﻿using System;
using System.Reflection;
using System.IO;

namespace Extension.Versioning
{
  public class TimeStamp
  {
    public static DateTime RetrieveLinkerTimestamp() {
      string filePath = Assembly.GetCallingAssembly().Location;
      const int c_PeHeaderOffset = 60;
      const int c_LinkerTimestampOffset = 8;
      byte[] b = new byte[2048];
      Stream s = null;

      try {
        s = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        s.Read(b, 0, 2048);
      } finally {
        if (s != null) {
          s.Close();
        }
      }

      int i = BitConverter.ToInt32(b, c_PeHeaderOffset);
      int secondsSince1970 = BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
      DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      dt = dt.AddSeconds(secondsSince1970);
      dt = dt.ToLocalTime();
      return dt;

			//Assembly assembly = Assembly.GetExecutingAssembly();
			//FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
			//Text += " v" + fvi.ProductVersion;

		}

	}
}
