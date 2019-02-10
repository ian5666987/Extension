using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using System.Linq;

namespace Extension.Cryptography {
  public class Cryptography {
    private readonly static string DefaultExtension = "liviancfile";
    private readonly static string DefaultPassword = "livianencryptor";
    private readonly static byte[] DefaultAesKey = new byte[] {
        0x25, 0x12, 0x00, 0x01,
        0x03, 0x05, 0x19, 0x87,
        0x05, 0x12, 0x19, 0x93,
        0x09, 0x12, 0x20, 0x12
      };
    private static string Extension;
    private static string Password;
    private static byte[] AesKey;
    public static bool IsOwner { get; private set; }
    public static bool IsValid { get; private set; }
    public static void SetExtension(string extension) { Extension = extension; }
    public static void SetPassword(string password) { Password = password; }
    public static void SetAesKey(byte[] aesKey) { AesKey = aesKey; }
    public static void SetValidity() {
      IsValid = false;
      IsOwner = false;
      if (string.IsNullOrWhiteSpace(Extension) ||
        string.IsNullOrWhiteSpace(Password) ||
        AesKey == null ||
        AesKey.Length != 16)
        return;              
      IsValid = true;

      if (Extension.Equals(DefaultExtension) && Password.Equals(DefaultPassword)) {
        for (int i = 0; i < AesKey.Length; ++i)
          if (AesKey[i] != DefaultAesKey[i])
            return;
        IsOwner = true; //I might want to do something with this ownership...
      }
    }

    public static void CryptoSerialize<T>(T item, string folderPath, string fileName) {
      if (!IsValid)
        return;
      //Writing and Serializing      
      XmlSerializer serializer = new XmlSerializer(typeof(T));
      Directory.CreateDirectory(folderPath);
      string filePath = Path.Combine(folderPath, fileName + ".xml");
      TextWriter textWriter = new StreamWriter(filePath);
      serializer.Serialize(textWriter, item);
      textWriter.Close();

      //Encrypting
      EncryptAndSaveFile(folderPath, fileName);
      File.Delete(filePath);
    }

    public static void CryptoSerializeAll<T>(List<T> items, string folderPath, List<string> fileNames) {
      if (!IsValid)
        return;
      int count = 0;
      foreach (var item in items)
        CryptoSerialize(item, folderPath, fileNames[count++]);
    }

    public static T DecryptoSerialize<T>(string filePath) {
      if (!IsValid)
        throw new Exception("Invalid initialization");
      DecryptAndSaveFile(filePath);
      string shortenedFilePath = filePath.Substring(0, filePath.Length - Extension.Length);
      string targetFile = shortenedFilePath + "xml";
      FileStream filestream = new FileStream(targetFile, FileMode.Open, FileAccess.Read, FileShare.Read);
      XmlSerializer serializer = new XmlSerializer(typeof(T));
      T item = (T)serializer.Deserialize(filestream);
      filestream.Close();
      File.Delete(targetFile);
      return item;
    }

    public static List<T> DecryptoSerializeAll<T>(string folderPath) {
      if (!IsValid)
        return null;
      Directory.CreateDirectory(folderPath);
      return Directory.GetFiles(folderPath, "*." + Extension, SearchOption.AllDirectories)
        .Select(x => DecryptoSerialize<T>(x))
        .ToList();
    }

    public static void DecryptAndSaveFile(string filePath) {
      if (!IsValid || string.IsNullOrWhiteSpace(filePath) || !filePath.ToLower().EndsWith(Extension))
        return;
      byte[] bytes = File.ReadAllBytes(filePath);
      byte[] decryptedBytes = Decrypt(bytes);
      string shortenedFilePath = filePath.Substring(0, filePath.Length - Extension.Length);
      string targetFile = shortenedFilePath + "xml";
      File.WriteAllBytes(targetFile, decryptedBytes);
    }

    public static void DecryptAndSaveFile(string folderPath, string fileName) {
      if (!IsValid)
        return;
      Directory.CreateDirectory(folderPath);
      string filepath = Path.Combine(folderPath, string.Concat(fileName, ".", Extension));
      byte[] bytes = File.ReadAllBytes(filepath);
      byte[] decryptedBytes = Decrypt(bytes);
      string targetFile = Path.Combine(folderPath, string.Concat(fileName + ".xml"));
      File.WriteAllBytes(targetFile, decryptedBytes);
    }

    public static void EncryptAndSaveFile(string folderPath, string fileName) {
      if (!IsValid)
        return;
      Directory.CreateDirectory(folderPath);
      string filepath = Path.Combine(folderPath, string.Concat(fileName, ".xml"));
      byte[] bytes = File.ReadAllBytes(filepath);
      byte[] encryptedBytes = Encrypt(bytes);
      string targetFile = Path.Combine(folderPath, string.Concat(fileName, ".", Extension));
      File.WriteAllBytes(targetFile, encryptedBytes);
    }

    public static string Encrypt(string input) {
      if (!IsValid)
        return null;
      return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(input)));
    }

    public static byte[] Encrypt(byte[] input) {
      if (!IsValid)
        return null;
      PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, AesKey);
      MemoryStream ms = new MemoryStream();
      Aes aes = new AesManaged();
      aes.Key = pdb.GetBytes(aes.KeySize / 8);
      aes.IV = pdb.GetBytes(aes.BlockSize / 8);
      CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
      cs.Write(input, 0, input.Length);
      cs.Close();
      return ms.ToArray();
    }

    public static string Decrypt(string input) {
      if (!IsValid)
        return null;
      return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(input)));
    }

    public static byte[] Decrypt(byte[] input) {
      if (!IsValid)
        return null;
      PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, AesKey);
      MemoryStream ms = new MemoryStream();
      Aes aes = new AesManaged();
      aes.Key = pdb.GetBytes(aes.KeySize / 8);
      aes.IV = pdb.GetBytes(aes.BlockSize / 8);
      CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
      cs.Write(input, 0, input.Length);
      cs.Close();
      return ms.ToArray();
    }
  }
}
