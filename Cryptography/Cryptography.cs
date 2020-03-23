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

    /// <summary>
    /// New function to save an encrypted file, do not use obsolete <see cref="EncryptAndSaveFile(string, string)"/>
    /// function anymore
    /// </summary>
    /// <param name="filepath">The file path to save the encrypted file to</param>
    /// <param name="aes16BytesKey">The 16-bytes AES Key</param>
    /// <param name="encoding">The encoding used for the <see cref="textData"/></param>
    /// <param name="textData">The data text to be encrypted</param>
    public static void EncryptAndSave(string filepath, byte[] aes16BytesKey, Encoding encoding, string textData) {
      if (encoding == null)
        throw new Exception("Encoding cannot be null");
      if (string.IsNullOrWhiteSpace(textData))
        throw new Exception("Text data cannot be empty or only consists of white space(s)");
      EncryptAndSave(filepath, aes16BytesKey, encoding.GetBytes(textData));
    }

    /// <summary>
    /// New function to save an encrypted file, do not use obsolete <see cref="EncryptAndSaveFile(string, string)"/>
    /// function anymore
    /// </summary>
    /// <param name="filepath">The file path to save the encrypted file to</param>
    /// <param name="aes16BytesKey">The 16-bytes AES Key</param>
    /// <param name="data">The data to be encrypted</param>
    public static void EncryptAndSave(string filepath, byte[] aes16BytesKey, byte[] data) {
      if (aes16BytesKey == null || aes16BytesKey.Length != 16)
        throw new Exception("AES key must be 16 bytes");
      if (string.IsNullOrWhiteSpace(filepath))
        throw new Exception("File path cannot be empty or only consists of white space(s)");
      if (data == null || data.Length < 0)
        throw new Exception("Data cannot be empty");
      SetAesKey(aes16BytesKey);
      string ext = Path.GetExtension(filepath);
      string pass = new string(aes16BytesKey.Select(x => x.ToString("X2")).SelectMany(x => x).ToArray());
      SetExtension(ext);
      SetPassword(pass);
      SetValidity();
      byte[] output = Encrypt(data);
      using (var fileStream = File.Create(filepath))
        fileStream.Write(output, 0, output.Length);
    }

    /// <summary>
    /// New function to save an encrypted file, do not use obsolete <see cref="EncryptAndSaveFile(string, string)"/>
    /// function anymore
    /// </summary>
    /// <param name="filepath">The file path to save the encrypted file to</param>
    /// <param name="aes16BytesKey">The 16-bytes AES Key</param>
    /// <param name="password">The additional password to generate the real AES-key</param>
    /// <param name="encoding">The encoding used for the <see cref="textData"/></param>
    /// <param name="textData">The data text to be encrypted</param>
    public static void EncryptAndSave(string filepath, byte[] aes16BytesKey, string password, Encoding encoding, string textData) {
      if (encoding == null)
        throw new Exception("Encoding cannot be null");
      if (string.IsNullOrWhiteSpace(textData))
        throw new Exception("Text data cannot be empty or only consists of white space(s)");
      EncryptAndSave(filepath, aes16BytesKey, password, encoding.GetBytes(textData));
    }

    /// <summary>
    /// New function to save an encrypted file, do not use obsolete <see cref="EncryptAndSaveFile(string, string)"/>
    /// function anymore
    /// </summary>
    /// <param name="filepath">The file path to save the encrypted file to</param>
    /// <param name="aes16BytesKey">The 16-bytes AES Key</param>
    /// <param name="password">The additional password to generate the real AES-key</param>
    /// <param name="data">The data to be encrypted</param>
    public static void EncryptAndSave(string filepath, byte[] aes16BytesKey, string password, byte[] data) {
      if (aes16BytesKey == null || aes16BytesKey.Length != 16)
        throw new Exception("AES key must be 16 bytes");
      if (string.IsNullOrWhiteSpace(filepath))
        throw new Exception("File path cannot be empty or only consists of white space(s)");
      if (string.IsNullOrEmpty(password))
        throw new Exception("Password cannot be empty");
      if (data == null || data.Length < 0)
        throw new Exception("Data cannot be empty");
      SetAesKey(aes16BytesKey);
      string ext = Path.GetExtension(filepath);
      SetExtension(ext);
      SetPassword(password);
      SetValidity();
      byte[] output = Encrypt(data);
      using (var fileStream = File.Create(filepath))
        fileStream.Write(output, 0, output.Length);
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

    /// <summary>
    /// File to load an encrypted file and decrypt its data
    /// </summary>
    /// <param name="filepath">The file path to load the encrypted file from</param>
    /// <param name="aes16BytesKey">The 16-bytes AES Key</param>
    /// <returns>The decrypted data of the file</returns>
    public static byte[] LoadAndDecrypt(string filepath, byte[] aes16BytesKey) {
      if (aes16BytesKey == null || aes16BytesKey.Length != 16)
        throw new Exception("AES key must be 16 bytes");
      if (string.IsNullOrWhiteSpace(filepath))
        throw new Exception("File path cannot be empty or only consists of white space(s)");
      SetAesKey(aes16BytesKey);
      string ext = Path.GetExtension(filepath);
      string pass = new string(aes16BytesKey.Select(x => x.ToString("X2")).SelectMany(x => x).ToArray());
      SetExtension(ext);
      SetPassword(pass);
      SetValidity();
      byte[] data = File.ReadAllBytes(filepath);
      return Decrypt(data);
    }

    /// <summary>
    /// File to load an encrypted file and decrypt its data, returning a string
    /// </summary>
    /// <param name="filepath">The file path to load the encrypted file from</param>
    /// <param name="aes16BytesKey">The 16-bytes AES Key</param>
    /// <param name="password">The additional password to generate the real AES-key</param>
    /// <param name="encoding">The encoding used to return the string</param>
    /// <returns>The decrypted data of the file in string</returns>
    public static string LoadAndDecrypt(string filepath, byte[] aes16BytesKey, string password, Encoding encoding) {
      if (encoding == null)
        throw new Exception("Encoding cannot be null");
      byte[] output = LoadAndDecrypt(filepath, aes16BytesKey, password);
      return encoding.GetString(output);
    }

    /// <summary>
    /// File to load an encrypted file and decrypt its data
    /// </summary>
    /// <param name="filepath">The file path to load the encrypted file from</param>
    /// <param name="aes16BytesKey">The 16-bytes AES Key</param>
    /// <param name="password">The additional password to generate the real AES-key</param>
    /// <returns>The decrypted data of the file</returns>
    public static byte[] LoadAndDecrypt(string filepath, byte[] aes16BytesKey, string password) {
      if (aes16BytesKey == null || aes16BytesKey.Length != 16)
        throw new Exception("AES key must be 16 bytes");
      if (string.IsNullOrWhiteSpace(filepath))
        throw new Exception("File path cannot be empty or only consists of white space(s)");
      if (string.IsNullOrEmpty(password))
        throw new Exception("Password cannot be empty");
      SetAesKey(aes16BytesKey);
      string ext = Path.GetExtension(filepath);
      SetExtension(ext);
      SetPassword(password);
      SetValidity();
      byte[] data = File.ReadAllBytes(filepath);
      return Decrypt(data);
    }

    /// <summary>
    /// File to load an encrypted file and decrypt its data, returning a string
    /// </summary>
    /// <param name="filepath">The file path to load the encrypted file from</param>
    /// <param name="aes16BytesKey">The 16-bytes AES Key</param>
    /// <param name="encoding">The encoding used to return the string</param>
    /// <returns>The decrypted data of the file in string</returns>
    public static string LoadAndDecrypt(string filepath, byte[] aes16BytesKey, Encoding encoding) {
      if (encoding == null)
        throw new Exception("Encoding cannot be null");
      byte[] output = LoadAndDecrypt(filepath, aes16BytesKey);
      return encoding.GetString(output);
    }
  }
}
