using System.IO;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/// <summary>
/// 工具类，定义一些文件操作
/// </summary>
public static class FileUtils {
    ///// <summary>
    ///// 读取xml文件
    ///// </summary>
    ///// <param name="name"></param>
    ///// <returns></returns>
    //public static SecurityElement ReadXMLFile(string name) {
    //    SecurityParser parser = new SecurityParser();
    //    string strXML = ReadAllText(name);
    //    try {
    //        if (string.IsNullOrEmpty(strXML) ||
    //            string.IsNullOrWhiteSpace(strXML)) {
    //            return null;
    //        }
    //        parser.LoadXml(strXML);
    //    } catch (System.Exception e) {
    //        //LogUtils.LogException(name + "\n" + strXML + "\n" + e);
    //        return null;
    //    }
    //    return parser.ToXml();
    //}

    //public static SecurityElement ReadXMLFile(byte[] bytes) {
    //    try {
    //        if (bytes == null || bytes.Length <= 0) {
    //            return null;
    //        }
    //        SecurityParser parser = new SecurityParser();
    //        string str = System.Text.Encoding.UTF8.GetString(bytes);
    //        parser.LoadXml(str);
    //        return parser.ToXml();
    //    } catch (System.Exception e) {
    //        LogUtils.LogException(e);
    //        return null;
    //    }
    //}

    /// <summary>
    /// 读取二进制文件
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static MemoryStream ReadFileByBytes(string name) {
        string fileFullPath = name;
        if (File.Exists(fileFullPath)) {
            FileStream tableFileStream = File.OpenRead(fileFullPath);
            if (null == tableFileStream) {
                return null;
            }
            MemoryStream fileMemoryStream = new MemoryStream();
            InternalCopyTo(tableFileStream, fileMemoryStream, tableFileStream.Length);
            fileMemoryStream.Position = 0;
            //关闭流  
            tableFileStream.Close();
            //销毁流  
            tableFileStream.Dispose();

            return fileMemoryStream;
        } else {

            bool archivePath = fileFullPath.StartsWith(Application.persistentDataPath);
            if (archivePath) {
                return null;
            }

            var www = UnityEngine.Networking.UnityWebRequest.Get(name);
            www.SendWebRequest();
            while (!www.isDone) {
                if (null != www.error) {
                    return null;
                }
            }
            if (www.downloadedBytes <= 0) {
                return null;
            }
            return new MemoryStream(www.downloadHandler.data);
        }
    }

    /// <summary>
    /// 读本地资源
    /// </summary>
    /// <param name="fileFullPath"></param>
    /// <returns></returns>
    public static MemoryStream ReadFileByBytesFromLocal(string fileFullPath) {
        if (!File.Exists(fileFullPath)) {
            return null;
        }
        FileStream fileStream = File.OpenRead(fileFullPath);
        if (fileStream == null) {
            return null;
        }
        MemoryStream fileMemoryStream = new MemoryStream();
        InternalCopyTo(fileStream, fileMemoryStream, fileStream.Length);

        fileMemoryStream.Position = 0;
        //关闭流  
        fileStream.Close();
        //销毁流  
        fileStream.Dispose();

        return fileMemoryStream;
    }
    /// <summary>
    /// 读整个文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadAllText(string path) {
        MemoryStream file = FileUtils.ReadFileByBytes(path);
        if (null == file) {
            return string.Empty;
        }
        file.Position = 0;
        StreamReader sr = new StreamReader(file);
        string strLine = sr.ReadToEnd();
        sr.Close();
        file.Close();

        if (strLine.Trim().ToLower() == "notfound") {
            return string.Empty;
        }
        return strLine;
    }

    ///// <summary>
    ///// 读取加密的文件
    ///// </summary>
    ///// <param name="path"></param>
    ///// <param name="key"></param>
    ///// <param name="iv"></param>
    ///// <returns></returns>
    //public static string ReadEncryptText(string path) {
    //    MemoryStream file = FA90D518E1C4C9A8AAE614C6D34F975FF(path, StringUtils.GetMD5(PathUtils.GetFileNameFormURL(path) + "joycastle;2021"));
    //    if (null == file) {
    //        return string.Empty;
    //    }
    //    file.Position = 0;
    //    StreamReader sr = new StreamReader(file);
    //    string strLine = sr.ReadToEnd();
    //    sr.Close();
    //    file.Close();

    //    return strLine;
    //}

    /// <summary>
    /// 深度拷贝
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="bufferSize"></param>
    public static void InternalCopyTo(Stream source, Stream destination, long bufferSize) {
        byte[] array = new byte[bufferSize];
        int count;
        while ((count = source.Read(array, 0, array.Length)) != 0) {
            destination.Write(array, 0, count);
        }
    }

    public static List<string> GetDirFileNames(string dirName) {
        if (!Directory.Exists(dirName)) {
            return null;
        }
        DirectoryInfo dir = new DirectoryInfo(dirName);
        List<string> fileNames = new List<string>();
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files) {
            if (file.Name.EndsWith(".meta")) {
                continue;
            }
            fileNames.Add(file.Name);
        }
        return fileNames;
    }

    public static void FileDelete(string fileName) {
        try {
            if (!File.Exists(fileName)) {
                return;
            }
            File.Delete(fileName);
        } catch (System.Exception e) {
            //LogUtils.LogException(e);
        }
    }

    //public static void FileMove(string sourceName, string desName) {
    //    try {
    //        if (!File.Exists(sourceName)) {
    //            return;
    //        }
    //        if (sourceName == desName) {
    //            return;
    //        }
    //        Copy(sourceName, desName);
    //        FileDelete(sourceName);
    //    } catch (System.Exception e) {
    //        //LogUtils.LogException(e);
    //    }
    //}

    ///// <summary>
    ///// 写文件
    ///// </summary>
    ///// <param name="path"></param>
    ///// <param name="text"></param>
    //public static void WriteAllText(string path, string text) {
    //    try {
    //        string strpath = Path.GetDirectoryName(path);
    //        if (!Directory.Exists(strpath)) {
    //            PathUtils.DirectoryCreate(strpath);
    //        }
    //        FileDelete(path);
    //        File.WriteAllText(path, text);
    //    } catch (System.Exception e) {
    //        //LogUtils.LogException(e);
    //    }

    //}
    ///// <summary>
    ///// 写文件
    ///// </summary>
    ///// <param name="path"></param> 
    ///// <param name="bytes"></param>
    //public static void WriteAllBytes(string path, byte[] bytes) {
    //    try {
    //        if (bytes == null) {
    //            return;
    //        }
    //        PathUtils.DirectoryCreate(Path.GetDirectoryName(path));
    //        FileDelete(path);
    //        File.WriteAllBytes(path, bytes);
    //    } catch (System.Exception e) {
    //        LogUtils.LogException(e);
    //    }
    //}

    /// <summary>
    /// 获取文件md5
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetMD5(string path) {
        System.Security.Cryptography.MD5 mD = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] value = mD.ComputeHash(File.ReadAllBytes(path));
        return System.BitConverter.ToString(value).Replace("-", string.Empty).ToUpper();
    }

    /// <summary>
    /// 获取文件md5
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string GetMD5(byte[] bytes, bool upper = true) {
        if (bytes == null) {
            return "";
        }
        System.Security.Cryptography.MD5 mD = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] value = mD.ComputeHash(bytes);
        return upper ? System.BitConverter.ToString(value).Replace("-", string.Empty).ToUpper() :
            System.BitConverter.ToString(value).Replace("-", string.Empty).ToLower();
    }


    /// <summary>
    /// 获取16位的MD5
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="upper"></param>
    /// <returns></returns>
    public static string GetMD5_16(byte[] bytes, bool upper = true) {
        if (bytes == null) {
            return "";
        }
        System.Security.Cryptography.MD5 mD = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] value = mD.ComputeHash(bytes);
        return upper ? System.BitConverter.ToString(value, 4, 8).Replace("-", string.Empty).ToUpper() :
            System.BitConverter.ToString(value, 4, 8).Replace("-", string.Empty).ToLower();
    }

    //public static void Copy(string srcPath, string destPath) {
    //    FileInfo fileInfo = new FileInfo(destPath);
    //    PathUtils.DirectoryCreate(fileInfo.DirectoryName);
    //    File.Copy(srcPath, destPath, true);
    //}

    //public static void WritePBFile(string path, IMessage pb) {
    //    PathUtils.DirectoryCreate(Path.GetDirectoryName(path));
    //    using (var fs = File.Open(path, FileMode.Create)) {
    //        fs.Position = 0;
    //        pb.WriteTo(fs);
    //        fs.Flush();
    //        fs.Dispose();
    //        fs.Close();
    //    }
    //}

    public static MemoryStream ReadTable(string fileName) {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        return ReadFileByBytes(fileName);
#else
        return FA90D518E1C4C9A8AAE614C6D34F975FF(fileName, StringUtils.GetMD5(PathUtils.GetFileNameFormURL(fileName) + "joycastle;2021"));
#endif
    }



//    public static string ReadMap(string fileName) {
//        var filePath = MethodManager.GetStreamingAssetsResPath(PathUtils.CombinePath("table", MM.Config.TableABTest.GetABTableName(fileName)));
//#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
//        return ReadAllText(filePath);
//#else
//        return ReadEncryptText(filePath);
//#endif
//    }


    /// <summary>
    /// AES解密，读取openssl加密的dll文件
    /// </summary>
    /// <param name="inFile"></param>
    /// <returns></returns>
    public static MemoryStream FA90D518E1C4C9A8AAE614C6D34F975FFDll(string inFile) {
        using (MemoryStream ms = FileUtils.ReadFileByBytes(inFile)) {
            if (ms == null || ms.Length <= 0) {
                return null;
            }
            var _keyByte = StringToByteArrayFastest("B4F943A3119EFB4AE4FBD7ED197475E7");
            var _ivByte = StringToByteArrayFastest("D711AAF7743989C424D202C6F99125A9");
            var inputByteArray = new byte[ms.Length];
            ms.Read(inputByteArray, 0, inputByteArray.Length);
            ms.Seek(0, SeekOrigin.Begin);
            ms.Close();

            using (var aes = new RijndaelManaged()) {
                aes.IV = _ivByte;
                aes.Key = _keyByte;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                var cryptoTransform = aes.CreateDecryptor();
                var resultArray = cryptoTransform.TransformFinalBlock(inputByteArray, 0, inputByteArray.Length);
                return new MemoryStream(resultArray);
            }
        }
    }

    private static byte[] StringToByteArrayFastest(string hex) {
        byte[] arr = new byte[hex.Length >> 1];
        for (int i = 0; i < hex.Length >> 1; ++i) {
            arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
        }
        return arr;
    }

    private static int GetHexVal(char hex) {
        int val = (int)hex;
        //For uppercase A-F letters:
        //return val - (val < 58 ? 48 : 55);
        //For lowercase a-f letters:
        //return val - (val < 58 ? 48 : 87);
        //Or the two combined, but a bit slower:
        return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
    }

    /// <summary>
    /// Aes解密
    /// </summary>
    /// <param name="inFile"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static MemoryStream FA90D518E1C4C9A8AAE614C6D34F975FF(string inFile, string key = "") {

        if (string.IsNullOrEmpty(key)) {
            key = StringUtils.LinkText("xi", "aok", "eket", "angu", "3d201", "905171", "81155", "da3");
        }
        string iv = StringUtils.LinkText("gba", "o;", "yhgo", "2i3", "589", "y");

        if (key == null) {
            //LogUtils.LogError("Aes Decrypt Error!");
            return null;
        }
        if (key.Length < 32) {
            for (int i = key.Length; i < 32; ++i) {
                key += i;
            }
        }
        if (key.Length > 32) {
            key = key.Substring(0, 32);
        }
        if (!string.IsNullOrEmpty(iv) && iv.Length < 16) {
            //LogUtils.LogError("Aes Decrypt Error!");
            return null;
        }
        using (MemoryStream ms = FileUtils.ReadFileByBytes(inFile)) {
            if (ms == null || ms.Length <= 0) {
                return null;
            }
            var _keyByte = Encoding.UTF8.GetBytes(key);
            var inputByteArray = new byte[ms.Length];
            ms.Read(inputByteArray, 0, inputByteArray.Length);
            ms.Seek(0, SeekOrigin.Begin);
            ms.Close();

            using (var aes = new RijndaelManaged()) {
                aes.IV = !string.IsNullOrEmpty(iv) ? Encoding.UTF8.GetBytes(iv) : Encoding.UTF8.GetBytes(key.Substring(0, 16));
                aes.Key = _keyByte;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                var cryptoTransform = aes.CreateDecryptor();
                var resultArray = cryptoTransform.TransformFinalBlock(inputByteArray, 0, inputByteArray.Length);
                return new MemoryStream(resultArray);
            }
        }
    }
#if UNITY_EDITOR
    ///// <summary>
    ///// Aes加密
    ///// </summary>
    ///// <param name="inFile"></param>
    ///// <param name="key"></param>
    ///// <param name="iv"></param>
    //public static void AesEncrypt(string inFile, string key, string iv = "gbao;yhgo2i3589y") {
    //    if (key == null) {
    //        LogUtils.LogError("Aes Encrypt Error!");
    //        return;
    //    }
    //    key = StringUtils.GetMD5(key + "joycastle;2021");
    //    if (key.Length < 32) {
    //        for (int i = key.Length; i < 32; ++i) {
    //            key += i;
    //        }
    //    }
    //    if (key.Length > 32) {
    //        key = key.Substring(0, 32);
    //    }
    //    if (!string.IsNullOrEmpty(iv) && iv.Length < 16) {
    //        LogUtils.LogError("Aes Encrypt Error!");
    //        return;
    //    }
    //    using (MemoryStream ms = FileUtils.ReadFileByBytes(inFile)) {
    //        if (ms == null || ms.Length <= 0) {
    //            LogUtils.LogError("Aes Encrypt Error!");
    //            return;
    //        }
    //        var _keyByte = Encoding.UTF8.GetBytes(key);
    //        var inputByteArray = new byte[ms.Length];
    //        ms.Read(inputByteArray, 0, inputByteArray.Length);
    //        ms.Seek(0, SeekOrigin.Begin);
    //        ms.Dispose();
    //        ms.Close();

    //        using (var aes = new RijndaelManaged()) {
    //            aes.IV = !string.IsNullOrEmpty(iv) ? Encoding.UTF8.GetBytes(iv) : Encoding.UTF8.GetBytes(key.Substring(0, 16));
    //            aes.Key = _keyByte;
    //            aes.Mode = CipherMode.CBC;
    //            aes.Padding = PaddingMode.PKCS7;
    //            var cryptoTransform = aes.CreateEncryptor();
    //            var resultArray = cryptoTransform.TransformFinalBlock(inputByteArray, 0, inputByteArray.Length);
    //            FileUtils.WriteAllBytes(inFile, resultArray);
    //        }
    //    }
    //}
#endif
}