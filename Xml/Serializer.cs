using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Extension.Xml {
  public class Serializer {
    public static T Deserialize<T>(string filepath) {
      try {
        FileStream filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        T item = (T)serializer.Deserialize(filestream);
        filestream.Close();
        return item;
      } catch {
        throw;
        //return default(T);
      }
    }

    public static bool Serialize<T>(T item, string filepath, bool createPathIfNotExist = false) {
      XmlSerializer serializer = null;
      StreamWriter streamWriter = null;
      try {
        if (createPathIfNotExist) {
          string directoryPath = Path.GetDirectoryName(filepath);
          if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        }

        serializer = new XmlSerializer(typeof(T));
        streamWriter = new StreamWriter(filepath);
        serializer.Serialize(streamWriter, item);
        streamWriter.Close();
        return true;
      } catch {
        if (streamWriter != null)
          streamWriter.Close();
        throw; 
        //return false;
      }
    }

    //Obtained from https://social.msdn.microsoft.com/Forums/en-US/b02ff158-b265-4c1f-b100-0849fc59a4d3/how-to-serialize-an-object-to-xml-in-memory-and-get-the-xml-string?forum=netfxremoting
    public static string GetSerialization<T>(T item, bool includeNameSpace = true, bool removeNull = false) {
      XmlSerializer xmlSerializer;
      StreamWriter stWriter = null;
      string buffer;
      try {
        xmlSerializer = new XmlSerializer(typeof(T));
        MemoryStream memStream = new MemoryStream();
        stWriter = new StreamWriter(memStream);
        if (!includeNameSpace) {
          XmlSerializerNamespaces xs = new XmlSerializerNamespaces();
          //To remove namespace and any other inline
          //information tag
          xs.Add("", "");
          xmlSerializer.Serialize(stWriter, item, xs);
        } else {
          xmlSerializer.Serialize(stWriter, item);
        }
        buffer = Encoding.ASCII.GetString(memStream.GetBuffer());
        if (removeNull)
          buffer = buffer.TrimEnd('\0');
      } catch (Exception Ex) {
        throw Ex;
      } finally {
        if (stWriter != null) stWriter.Close();
      }
      return buffer;
    }

    public static T GetDeserialization<T>(string xmlString) {
      try {
        MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        T item = (T)serializer.Deserialize(memStream);
        memStream.Close();
        return item;
      } catch {
        throw;
        //return default(T);
      }
    }
  }
}
