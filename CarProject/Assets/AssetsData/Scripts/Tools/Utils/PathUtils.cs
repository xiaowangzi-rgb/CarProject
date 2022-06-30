using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 工具类，定义一些与路径有关的操作
/// </summary>
public static partial class PathUtils {
    /// <summary>
    /// Assets路径
    /// </summary>
    public static string AssetsPath { get { return Application.dataPath; } }
    /// <summary>
    /// Persistent路径
    /// </summary>
    public static string PersistentDataPath { get { return Application.persistentDataPath; } }
    /// <summary>
    /// 临时目录
    /// </summary>
    public static string TempPath = CombinePath(PersistentDataPath, "temp");
    /// <summary>
    /// 缓存的目录
    /// </summary>
    public static string CachePath = CombinePath(PersistentDataPath, "catch");
    /// <summary>
    /// ArchivePath
    /// </summary>
    public static string ArchivePath = CombinePath(PersistentDataPath, "archive");
    /// <summary>
    /// 用户数据路径
    /// </summary>
    public static string UserDataPath = CombinePath(ArchivePath, "User");
    /// <summary>
    /// 资源目录
    /// </summary>
    public static string ResBasePath = CombinePath("Assets", "AppData", "ResBase");

    public static string ResPath = CombinePath("Assets", "AppData", "Res");

    /************************************************************************/
    /* 游戏本地资源路径                                                      */
    /************************************************************************/
    public const string StreamingAssetsName = "StreamingAssets";
    public static string StreamingAssetsWWWPath =
#if UNITY_EDITOR
            CombinePath(Application.dataPath, StreamingAssetsName);
#elif UNITY_STANDALONE_WIN
            CombinePath(AssetsPath, StreamingAssetsName);
#elif UNITY_STANDALONE_OSX
            CombinePath(AssetsPath, "Resources", "Data", StreamingAssetsName);
#elif UNITY_IPHONE
            CombinePath(AssetsPath, "Raw");
#elif UNITY_ANDROID
            CombinePath(Application.streamingAssetsPath);
#else
	        string.Empty;
#endif



    public static string MethodLibraryPath = CombinePath(StreamingAssetsWWWPath, "RuntimeLibrary");

    /// <summary>
    /// 从url中获取文件名
    /// </summary>
    /// <param name="url"></param>
    /// <param name="needNameSuffix">是否需要后缀名</param>
    /// <returns></returns>
    public static string GetFileNameFormURL(string url, bool needNameSuffix = true) {
        if (string.IsNullOrEmpty(url)) {
            return string.Empty;
        }
        return needNameSuffix ? Path.GetFileName(url) : Path.GetFileNameWithoutExtension(url);
    }
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="dirName"></param>
    public static void DirectoryCreate(string dirName, bool recursive = true) {
        if (Directory.Exists(dirName)) {
            return;
        }
        DirectoryInfo dirInfo = new DirectoryInfo(dirName);
        DirectoryCreate(dirInfo, recursive);
    }
    public static void DirectoryCreate(DirectoryInfo dirInfo, bool recursive = true) {
        if (dirInfo.Exists) {
            return;
        }
        if (recursive) {
            if (!dirInfo.Parent.Exists) {
                DirectoryCreate(dirInfo.Parent);
            }
        }
        dirInfo.Create();
    }
    /// <summary>
    /// 移动文件夹
    /// </summary>
    /// <param name="sourceDirName"></param>
    /// <param name="destDirName"></param>
    public static void DirectoryMove(string sourceDirName, string destDirName) {
        if (!Directory.Exists(sourceDirName)) {
            return;
        }
        DirectoryCopy(sourceDirName, destDirName);
        DirectoryDelete(sourceDirName, true);
    }

    public static string GetStreamingAssetsResPath(string name)
    {
        string path = PathUtils.CombinePath(ResourcePath, name);
        if (File.Exists(path))
        {
            return path;
        }
        return "";
        //return PathUtils.CombinePath(PathUtils.StreamingAssetsWWWPath, Global.strMethodName, name);
    }

    /// <summary>
    /// 资源路径
    /// </summary>
    public static string ResourcePath
    {
        get
        {
            return PathUtils.CombinePath(PathUtils.StreamingAssetsWWWPath, "car");
        }
    }

    /// <summary>
    /// 拷贝文件夹
    /// </summary>
    /// <param name="sourceDirName"></param>
    /// <param name="destDirName"></param>
    public static void DirectoryCopy(string sourceDirName, string destDirName, string exsuff = "") {
        if (Directory.Exists(sourceDirName)) {
            if (!Directory.Exists(destDirName)) {
                Directory.CreateDirectory(destDirName);
            }

            foreach (string fls in Directory.GetFiles(sourceDirName)) {
                FileInfo flinfo = new FileInfo(fls);
                if (!string.IsNullOrEmpty(exsuff) && flinfo.Name.EndsWith(exsuff)) {
                    continue;
                }
                flinfo.CopyTo(CombinePath(destDirName, flinfo.Name), true);
            }

            foreach (string drs in Directory.GetDirectories(sourceDirName)) {
                DirectoryInfo drinfo = new DirectoryInfo(drs);
                DirectoryCopy(drs, CombinePath(destDirName, drinfo.Name), exsuff);
            }
        }
    }
    /// <summary>
    /// 删除文件夹
    /// </summary>
    /// <param name="path"></param>
    /// <param name="recursive"></param>
    public static void DirectoryDelete(string dirName, bool recursive = true) {
        if (!Directory.Exists(dirName)) {
            return;
        }
        // Delete all files and sub-folders?
        if (recursive) {
            // Yep... Let's do this
            var subfolders = Directory.GetDirectories(dirName);
            foreach (var s in subfolders) {
                DirectoryDelete(s, recursive);
            }
        }

        // Get all files of the folder
        var files = Directory.GetFiles(dirName);
        foreach (var f in files) {
            // Get the attributes of the file
            var attr = File.GetAttributes(f);

            // Is this file marked as 'read-only'?
            if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                // Yes... Remove the 'read-only' attribute, then
                File.SetAttributes(f, attr ^ FileAttributes.ReadOnly);
            }

            // Delete the file
            File.Delete(f);
        }

        // When we get here, all the files of the folder were
        // already deleted, so we just delete the empty folder
        Directory.Delete(dirName);
    }
    /// <summary>
    /// 查找指定文件夹下指定后缀名的文件
    /// </summary>
    /// <param name="dirName">文件夹</param>
    /// <param name="pattern">后缀名</param>
    /// <returns>文件路径</returns>
    public static List<string> GetFilesInDirectory(string dirName, string pattern, bool recursive = true) {
        DirectoryInfo directoryInfo = new DirectoryInfo(dirName);
        return GetFilesInDirectory(directoryInfo, pattern, recursive);
    }
    /// <summary>
    /// 查找指定文件夹下指定后缀名的文件
    /// </summary>
    /// <param name="directory">文件夹</param>
    /// <param name="pattern">后缀名</param>
    /// <returns>文件路径</returns>
    public static List<string> GetFilesInDirectory(DirectoryInfo directoryInfo, string pattern, bool recursive = true) {
        List<string> result = new List<string>();
        if (directoryInfo.Exists) {
            try {
                if (string.IsNullOrEmpty(pattern)) {
                    foreach (FileInfo info in directoryInfo.GetFiles()) {
                        result.Add(info.FullName.ToString());
                    }
                } else {
                    foreach (FileInfo info in directoryInfo.GetFiles(pattern)) {
                        result.Add(info.FullName.ToString());
                    }
                }

            } catch (Exception e) {
                //LogUtils.LogException(e);
            }
            if (recursive) {
                foreach (DirectoryInfo info in directoryInfo.GetDirectories()) {
                    result.AddRange(GetFilesInDirectory(info, pattern));
                }
            }
        }
        return result;
    }
    /// <summary>
    /// 查找指定文件夹下指定多个后缀名的文件
    /// </summary>
    /// <param name="dirName">文件夹</param>
    /// <param name="patterns">后缀名</param>
    /// <returns>文件路径</returns>
    public static List<string> GetMultiFilesInDirectory(string dirName, string[] patterns, bool recursive = true) {
        DirectoryInfo directoryInfo = new DirectoryInfo(dirName);
        return GetMultiFilesInDirectory(directoryInfo, patterns, recursive);
    }
    /// <summary>
    /// 查找指定文件夹下指定多个后缀名的文件
    /// </summary>
    /// <param name="directory">文件夹</param>
    /// <param name="patterns">后缀名数组</param>
    /// <returns>文件路径</returns>
    public static List<string> GetMultiFilesInDirectory(DirectoryInfo directoryInfo, string[] patterns, bool recursive = true) {
        List<string> result = new List<string>();

        foreach (string pattern in patterns) {
            result.AddRange(GetFilesInDirectory(directoryInfo, pattern, recursive));
        }

        return result;
    }
    /// <summary>
    /// 删除指定文件夹下指定后缀名的文件
    /// </summary>
    /// <param name="dirName">文件夹</param>
    /// <param name="pattern">后缀名</param>
    /// <returns>文件路径</returns>
    public static void DeleteFilesInDirectory(string dirName, string pattern, bool recursive = true) {
        DirectoryInfo directoryInfo = new DirectoryInfo(dirName);
        DeleteFilesInDirectory(directoryInfo, pattern, recursive);
    }
    /// <summary>
    /// 删除指定文件夹下指定后缀名的文件
    /// </summary>
    /// <param name="directory">文件夹</param>
    /// <param name="pattern">后缀名</param>
    /// <returns>文件路径</returns>
    public static void DeleteFilesInDirectory(DirectoryInfo directoryInfo, string pattern, bool recursive = true) {
        if (directoryInfo.Exists && pattern.Trim() != string.Empty) {
            try {
                foreach (FileInfo info in directoryInfo.GetFiles(pattern)) {
                    info.Delete();
                }
            } catch { }
            if (recursive) {
                foreach (DirectoryInfo info in directoryInfo.GetDirectories()) {
                    DeleteFilesInDirectory(info, pattern);
                }
            }
        }
    }
    /// <summary>
    /// 判断是否是目录
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsDirectory(string path) {
        return Directory.Exists(path) && ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory);
    }
    /// <summary>
    /// Creates a relative path from one file or folder to another.
    /// </summary>
    /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
    /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
    /// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
    public static string MakeRelativePath(string fromPath, string toPath) {
        if (string.IsNullOrEmpty(fromPath) || string.IsNullOrEmpty(toPath)) {
            return string.Empty;
        }

        string modifiedFromPath = Path.GetFullPath(fromPath);
        if (!IsDirectory(modifiedFromPath)) {
            modifiedFromPath = Path.GetDirectoryName(modifiedFromPath);
        }
        modifiedFromPath = CombinePath(modifiedFromPath, ".");

        string modifiedToPath = Path.GetFullPath(toPath);
        if (!IsDirectory(modifiedToPath)) {
            modifiedToPath = Path.GetDirectoryName(modifiedToPath);
        }

        Uri fromUri = new Uri(modifiedFromPath);
        Uri toUri = new Uri(modifiedToPath);

        Uri relativeUri = fromUri.MakeRelativeUri(toUri);
        string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        if (!IsDirectory(toPath)) {
            relativePath = CombinePath(relativePath, Path.GetFileName(toPath));
        }
        return relativePath;
    }
    /// <summary>
    /// 拼接Path
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    public static string CombinePath(params string[] paths) {
        return paths == null || paths.Length <= 0 ? "" : Path.Combine(paths).Replace("\\", "/");
    }
}
