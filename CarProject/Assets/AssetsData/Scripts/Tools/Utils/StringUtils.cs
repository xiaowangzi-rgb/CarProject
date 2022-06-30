using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 工具类，定义一些字符串相关的操作
/// </summary>
public static class StringUtils {

    public const string COLON = ":";
    public const string DIVIDE = "/";
    public const string PERCENT = "%";
    public const string BRACKET_SMALL_LEFT = "(";
    public const string BRACKET_SMALL_RIGHT = ")";
    public const string BRACKET_MIDDLE_LEFT = "[";
    public const string BRACKET_MIDDLE_RIGHT = "]";
    public const string MINUS = "-";
    public const string ADD = "+";
    public const string COMMA = ",";
    public const string POINT = ".";
    public const string UNDERLINE = "_";
    public const string VERTICAL_LINE = "|";
    public const string SEMICOLON = ";";
    public const string RMB = "￥";
    public const string CAESURA_SIGN = "、";

    public const string LOGIC_AND = "&";
    public const string LOGIC_OR = "|";
    public const string LOGIC_NOT = "!";
    public const string LOGIC_EQUAL = "=";

    public const string BLANK = " ";
    public const string COMMA_UP_CHAR = "'";

    public const string URL_QUESTION = "?";

    public const char VERTICAL_LINE_CHAR = '|';
    public const char PERCENT_CHAR = '%';
    public const char COMMA_CHAR = ',';
    public const char MINUS_CHAR = '-';
    public const char COLON_CHAR = ':';
    public const char POUND_CHAR = '#';
    public const char ADD_CHAR = '+';
    public const char SEMICOLON_CHAR = ';';
    public const char POINT_CHAR = '.';
    public const char MUL_CHAR = '*';

    /// <summary>
    ///  获得字符串拼接函数都是非线程安全的，多线程禁止调用
    /// </summary>
    public static StringBuilder stringBuilder = new StringBuilder();
    /// <summary>
    /// 获得带颜色的文本（非线程安全）
    /// </summary>
    /// <param name="sText">目标文本</param>
    /// <param name="sColor">颜色16进制字符串</param>

    public static string GetColor(string sText, string sColor) {
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append(BRACKET_MIDDLE_LEFT);
        stringBuilder.Append(sColor);
        stringBuilder.Append(BRACKET_MIDDLE_RIGHT);
        stringBuilder.Append(sText);
        stringBuilder.Append(BRACKET_MIDDLE_LEFT);
        stringBuilder.Append(MINUS);
        stringBuilder.Append(BRACKET_MIDDLE_RIGHT);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 获得带颜色的文本（非线程安全）<see cref="stringBuilder"/>
    /// </summary>
    /// <param name="oText">目标对象</param>
    /// <param name="sColor">颜色16进制字符串</param>
    public static string GetColor(object oText, string sColor) {
        return GetColor(oText.ToString(), sColor);
    }

    /// <summary>
    /// 获得带括弧的文本 类似 "(1)"（非线程安全）
    /// </summary>
    /// <returns></returns>
    public static string GetBraceText(string str) {
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append(BRACKET_SMALL_LEFT);
        stringBuilder.Append(str);
        stringBuilder.Append(BRACKET_SMALL_RIGHT);
        return stringBuilder.ToString();
    }
    /// <summary>
    /// 得到带百分号的文本 类似 "1%"（非线程安全）
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string GetPerText(string str) {
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append(str);
        stringBuilder.Append(PERCENT);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 获得带括弧的文本 类似"（1/10）"（非线程安全）
    /// </summary>
    /// <param name="iCur">当前值，（第一个值）</param>
    /// <param name="iMax">最大值，（第二个值）</param>

    public static string GetBracketsText(string sCur, string sMax) {
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append(BRACKET_SMALL_LEFT);
        stringBuilder.Append(sCur);
        stringBuilder.Append(DIVIDE);
        stringBuilder.Append(sMax);
        stringBuilder.Append(BRACKET_SMALL_RIGHT);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 获得带括弧的文本 类似"（1/10）"（非线程安全）
    /// </summary>
    /// <param name="iCur">当前值，（第一个值）</param>
    /// <param name="iMax">最大值，（第二个值）</param>
    public static string GetBracketsText(int iCur, int iMax) {
        return GetBracketsText(iCur.ToString(), iMax.ToString());
    }

    /// <summary>
    /// 获得带括弧的文本 类似"[目标文本]"（非线程安全）
    /// </summary>
    /// <param sStr="iCur">目标文本</param>
    public static string GetBracketsText(string sStr) {
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append(BRACKET_MIDDLE_LEFT);
        stringBuilder.Append(sStr);
        stringBuilder.Append(BRACKET_MIDDLE_RIGHT);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 获得带斜杠的文本 类似"1/10"（非线程安全）
    /// </summary>
    /// <param name="iCur">当前值，（第一个值）</param>
    /// <param name="iMax">最大值，（第二个值）</param>

    public static string GetDivideText(string sCur, string sMax) {
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append(sCur);
        stringBuilder.Append(DIVIDE);
        stringBuilder.Append(sMax);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 获得带斜杠的文本 类似"1/10"（非线程安全）
    /// </summary>
    /// <param name="iCur">当前值，（第一个值）</param>
    /// <param name="iMax">最大值，（第二个值）</param>

    public static string GetDivideText(int iCur, int iMax) {
        return GetDivideText(iCur.ToString(), iMax.ToString());
    }

    /// <summary>
    /// 获得带冒号的文本 类似 "等级：1"（非线程安全）
    /// </summary>
    /// <param name="sPre">冒号前部分</param>
    /// <param name="sStd">冒号后部分</param>

    public static string GetColonText(string sPre, string sStd) {
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append(sPre);
        stringBuilder.Append(COLON);
        stringBuilder.Append(sStd);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 两个文本相加（非线程安全）
    /// </summary>
    /// <param name="str1">目标文本1</param>
    /// <param name="str2">目标文本2</param>
    public static string AddText(string str1, string str2) {
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append(str1);
        stringBuilder.Append(str2);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 连接字符串（非线程安全）
    /// </summary>
    /// <param name="strings">需要连接的字符串</param>
    /// <returns>返回字符串相的连接</returns>
    public static string LinkText(params string[] strings) {
        stringBuilder.Remove(0, stringBuilder.Length);
        for (int i = 0, len = strings.Length; i < len; i++) {
            stringBuilder.Append(strings[i]);
        }
        return stringBuilder.ToString();
    }
    /// <summary>
    /// String 转 bool
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool StringToBool(string str) {
        if (string.IsNullOrEmpty(str)) {
            return false;
        }
        bool result;
        if (!bool.TryParse(str.Trim(), out result)) {
            //LogUtils.LogError(LinkText("StringToBool error from ", str));
            return false;
        }
        return result;
    }
    /// <summary>
    /// String 转 int
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int StringToInt(string str) {
        if (string.IsNullOrEmpty(str)) {
            return int.MinValue;
        }
        if (!int.TryParse(str.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)) {
            //LogUtils.LogError(LinkText("StringToInt error from ", str));
            return int.MinValue;
        }
        return value;
    }
    /// <summary>
    /// String 转 uint
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static uint StringToUInt(string str) {
        if (string.IsNullOrEmpty(str)) {
            return uint.MinValue;
        }
        if (!uint.TryParse(str.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out uint value)) {
            //LogUtils.LogError(LinkText("StringToUInt error from ", str));
            return uint.MinValue;
        }
        return value;
    }
    /// <summary>
    /// String 转 long
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static long StringToLong(string str) {
        if (string.IsNullOrEmpty(str)) {
            return long.MinValue;
        }
        if (!long.TryParse(str.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out long value)) {
            //LogUtils.LogError(LinkText("StringToLong error from ", str));
            return long.MinValue;
        }
        return value;
    }
    /// <summary>
    /// String 转 ulong
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static ulong StringToULong(string str) {
        if (string.IsNullOrEmpty(str)) {
            return ulong.MinValue;
        }
        if (!ulong.TryParse(str.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out ulong value)) {
            //LogUtils.LogError(LinkText("StringToULong error from ", str));
            return ulong.MinValue;
        }
        return value;
    }
    /// <summary>
    /// String 转 float
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static float StringToFloat(string str) {
        if (string.IsNullOrEmpty(str)) {
            return float.NaN;
        }
        if (!float.TryParse(str.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) {
            //LogUtils.LogError(LinkText("StringToFloat error from ", str));
            return float.NaN;
        }
        return value;
    }
    /// <summary>
    /// String 转 double
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static double StringToDouble(string str) {
        if (string.IsNullOrEmpty(str)) {
            return double.NaN;
        }
        if (!double.TryParse(str.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double value)) {
            //LogUtils.LogError(LinkText("StringToDouble error from ", str));
        }
        return value;
    }
    /// <summary>
    /// String 转 Vector3
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Vector3 StringToVector3(string str) {
        string[] strvector = str.Split(',');
        if (strvector == null || strvector.Length < 3) {
            return Vector3.zero;
        }
        return new Vector3(StringToFloat(strvector[0]), StringToFloat(strvector[1]), StringToFloat(strvector[2]));
    }

    /// <summary>
    /// String 转 Vector3
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Vector2 StringToVector2(string str) {
        string[] strvector = str.Split(',');
        if (strvector == null || strvector.Length < 2) {
            return Vector2.zero;
        }
        return new Vector2(StringToFloat(strvector[0]), StringToFloat(strvector[1]));
    }

    /// <summary>
    /// 是否是数字
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool IsNumber(string key) {
        if (string.IsNullOrEmpty(key)) {
            return false;
        }

        return Regex.IsMatch(key, @"^\d+$");
    }


    public static string GetMD5(string str) {
        byte[] bytValue = Encoding.UTF8.GetBytes(str);
        System.Security.Cryptography.MD5 mD = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] value = mD.ComputeHash(bytValue);
        return BitConverter.ToString(value).Replace("-", string.Empty).ToUpper();
    }

    /// <summary>
    /// base64转整形
    /// </summary>
    /// <param name="base64String"></param>
    /// <returns></returns>
    public static int Base64StringToInt(string base64String) {
        try {
            byte[] value = Convert.FromBase64String(base64String);
            return StringUtils.StringToInt(Encoding.UTF8.GetString(value));
        } catch (Exception) {
            return 0;
        }
    }

    /// <summary>
    /// 整形转base64string
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string IntToBase64String(int value) {
        try {
            byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(value.ToString());
            return Convert.ToBase64String(bytValue);
        } catch (Exception) {
            byte[] bytValue = System.Text.Encoding.UTF8.GetBytes("0");
            return Convert.ToBase64String(bytValue);
        }
    }

    /// <summary>
    /// 是否是INT
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsInt(string value) {
        return Regex.IsMatch(value, @"^[+-]?\d*$");
    }

    /// <summary>
    /// 删除字符串中的空格
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string RemoveStringBlankSpace(string str) {
        return Regex.Replace(str, @"\s", "");
    }

    public static string GetTargetStr(string str) {
        return str.Replace("#$", "<").Replace("$#", ">").Replace(" ", "\u00A0").Replace("\\n", "\n");
    }


    public static string GetTrueLineString(Text text, string strstr) {
        int contentLength = GetStringLengthInText(strstr, text.font, text.fontSize);
        int tottleLen = Mathf.FloorToInt(text.rectTransform.rect.width);
        if (tottleLen < 0) {
            return strstr;
        }
        if (contentLength <= tottleLen) {
            return strstr;
        }
        Regex regex = new Regex(@"[【】（）()《》]");

        Regex regex1 = new Regex(@"[，,。.!！?？]");

        string str = strstr;
        char[] charArray = str.ToCharArray();
        int curLength = 0;

        for (int i = 0; i < charArray.Length; ++i) {
            char ch = charArray[i];
            int newLen = GetStringLengthInText(ch.ToString(), text.font, text.fontSize);
            if (newLen + curLength >= tottleLen) {
                if (regex.IsMatch(charArray[i].ToString()) || (i + 1 < charArray.Length && regex.IsMatch(charArray[i + 1].ToString()))) {
                    return InsertString(regex, charArray, i - 1);
                } else if (regex1.IsMatch(charArray[i].ToString()) || (i + 1 < charArray.Length && regex1.IsMatch(charArray[i + 1].ToString()))) {
                    return str.Insert(i - 1, "\n");
                }
            } else {
                curLength += newLen;
            }
        }

        return str;
    }

    private static string InsertString(Regex regex, char[] str, int index) {
        if (index - 1 > 0) {
            if (regex.IsMatch(str[index - 1].ToString())) {
                return InsertString(regex, str, --index);
            } else {
                byte[] bb = Encoding.UTF8.GetBytes(str);
                return Encoding.UTF8.GetString(bb).Insert(index, "\n");
            }
        }
        byte[] ll = Encoding.UTF8.GetBytes(str);
        return Encoding.UTF8.GetString(ll).Insert(index, "\n");
    }

    public static int GetStringLengthInText(string text, Font font, int size) {
        font.RequestCharactersInTexture(text, size);
        CharacterInfo chaInfo = new CharacterInfo();
        int totalLegth = 0;

        char[] charArray = text.ToCharArray();

        foreach (char ch in charArray) {
            if (font.GetCharacterInfo(ch, out chaInfo, size)) {
                totalLegth += chaInfo.advance;
            }
        }

        return totalLegth;
    }

    public static string MidStrEx(string sourse, string startstr, string endstr) {
        string result = string.Empty;
        int startindex = 0;
        int endindex = 0;

        try {
            startindex = sourse.IndexOf(startstr);
            if (startindex == -1) {
                return result;
            }
            string tmpstr = sourse.Substring(startindex + startstr.Length);
            endindex = tmpstr.IndexOf(endstr);
            if (endindex == -1) {
                return result;
            }
            result = tmpstr.Remove(endindex);
        } catch (Exception ex) {
            Debug.LogException(ex);
        }

        return result;
    }
}
