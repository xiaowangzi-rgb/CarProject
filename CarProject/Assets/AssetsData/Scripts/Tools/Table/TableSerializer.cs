using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace MM.Config  {
    public class TableSerializer : ISerializer {
        /// <summary>
        /// 表格中元素类型
        /// </summary>
        public enum EM_TYPE_COLUMN {
            INVALID = -1,
            INT,
            UINT,
            LONG,
            ULONG,
            FLOAT,
            STRING,
            INT_ARRAY,
            FLOAT_ARRAY,
            STRING_ARRAY,
            COUNT,
        };

        /// <summary>
        /// 表格中元素类型字符串
        /// </summary>
        public static readonly string[] COLUMN_TYPE_TO_STRING = new string[]
        {
            "INT",
            "UINT",
            "LONG",
            "ULONG",
            "FLOAT",
            "STRING",
            "INT_ARRAY",
            "FLOAT_ARRAY",
            "STRING_ARRAY",
        };

        /// <summary>
        /// 最小行数
        /// </summary>
        public const int MIN_LINE_COUNT = 2;

        /// <summary>
        /// 忽略行
        /// </summary>
        public const char TABLE_SKIP_CHAR = '#';

        /// <summary>
        /// 默认表格字段值
        /// </summary>
        public const string DEFAUL_TABLE_FIELD = "-1";

        /// <summary>
        /// 表格字符串数组分隔符
        /// </summary>
        public const string TABLE_STRING_ARRAY_SEPARATOR = "|";

        /// <summary>
        /// 表格行分隔符
        /// </summary>
        public static readonly string[] TABLE_LINE_SEPARATOR = new string[] { "\r\n", "\n" };

        /// <summary>
        /// 表格列分隔符
        /// </summary>
        public const string TABLE_COLUMN_SEPARATOR = ",";

        /// <summary>
        /// 字节段
        /// </summary>
        public struct ByteSection {
            public ByteSection(int beginPos, int length) {
                m_SectionBeginPos = beginPos;
                m_SectionLength = length;
            }

            public int GetSectionEnd() {
                return m_SectionBeginPos + m_SectionLength;
            }

            public int m_SectionBeginPos;
            public int m_SectionLength;
        }

        /// <summary>
        /// 表格行
        /// </summary>
        public class TableLine {
            public TableLine(ByteSection section, List<ByteSection> columnField) {
                m_LineSection = section;
                m_ColumnField = columnField;
            }

            public ByteSection m_LineSection;
            public List<ByteSection> m_ColumnField;
        }

        /// <summary>
        /// 预处理数据
        /// </summary>
        public class TablePreprocess {
            public int m_ColumnCount;
            public TableLine m_ColumnName = null;
            public TableLine m_ColumnType = null;
            public List<TableLine> m_Data;
        }

        /// <summary>
        /// 文件名
        /// </summary>
        private string m_FileName;

        /// <summary>
        /// 文件
        /// </summary>
        private MemoryStream m_File;

        /// <summary>
        /// 读取缓存
        /// </summary>
        private Byte[] m_ReadBuffer;

        /// <summary>
        /// 表格预处理数据
        /// </summary>
        private TablePreprocess m_PreprocessData;

        /// <summary>
        /// 列名
        /// </summary>
        private string[] m_ColumnNames;

        /// <summary>
        /// 列类型
        /// </summary>
        private EM_TYPE_COLUMN[] m_ColumnTypes;

        /// <summary>
        /// 当前行
        /// </summary>
        private int m_CurrentLine;

        /// <summary>
        /// 当前列
        /// </summary>
        private int m_CurrentColumn;

        /// <summary>
        /// 是否检查列
        /// </summary>
        private bool m_IsCheckColumn;

        /// <summary>
        /// 当前的ID
        /// </summary>
        private int m_CurrentID;

        public TableSerializer() {
            m_File = null;
            m_PreprocessData = new TablePreprocess();
            m_CurrentLine = 0;
            m_CurrentColumn = 0;
            m_IsCheckColumn = false;
            m_CurrentID = -1;
        }

        public ISerializer Parse(ref int value) {
            _Set(ref value); return this;
        }

        public ISerializer Parse(ref uint value) {
            _Set(ref value); return this;
        }

        public ISerializer Parse(ref long value) {
            _Set(ref value); return this;
        }

        public ISerializer Parse(ref ulong value) {
            _Set(ref value); return this;
        }

        public ISerializer Parse(ref float value) {
            _Set(ref value); return this;
        }

        public ISerializer Parse(ref string value) {
            _Set(ref value); return this;
        }

        public ISerializer Parse(ref List<float> value) {
            _Set(ref value); return this;
        }

        public ISerializer Parse(ref List<int> value) {
            _Set(ref value); return this;
        }
        public ISerializer Parse(ref List<string> value) {
            _Set(ref value); return this;
        }

        public void SkipField() {
            _EndParseColumn();
        }

        public void SetCheckColumn(bool isCheck) {
            m_IsCheckColumn = isCheck;
        }

        public void SetCurrentID(int id) {
            this.m_CurrentID = id;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool OpenRead(string fileName) {
            m_FileName = string.Copy(fileName);

            m_File = FileUtils.ReadTable(fileName);
            if (null == m_File) {
                return false;
            }
            m_File.Seek(0, SeekOrigin.Begin);

            byte[] bom = new byte[2];
            m_File.Read(bom, 0, 2);
            if (!(0xFF == bom[0] && 0xFE == bom[1] || 0xFE == bom[0] && 0xFF == bom[1])) {
                m_File.Seek(0, SeekOrigin.Begin);
            }
            return true;
        }

        /// <summary>
        /// 预处理文件
        /// </summary>
        public void PreprocessTable() {
            m_PreprocessData = new TablePreprocess();
            _ReadFileToMemory();
            _ReadLine();
            _SaveColumnNames();
            _SaveColumnTypes();
        }

        /// <summary>
        /// 获得行数
        /// </summary>
        /// <returns></returns>
        public int GetLineCount() {
            return m_PreprocessData.m_Data.Count;
        }

        /// <summary>
        /// 获得列数
        /// </summary>
        /// <returns></returns>
        public int GetColumnCount() {
            return m_PreprocessData.m_ColumnCount;
        }

        /// <summary>
        /// 获得列名
        /// </summary>
        /// <returns></returns>
        public string[] GetColumnNames() {
            return m_ColumnNames;
        }

        /// <summary>
        /// 设置当前行号
        /// </summary>
        /// <param name="line"></param>
        public void SetCurrentLine(int line) {
            m_CurrentLine = line;
            m_CurrentColumn = 0;
        }

        /// <summary>
        /// 设置列号
        /// </summary>
        /// <param name="column"></param>
        public void SetCurrentColumn(int column) {
            m_CurrentColumn = column;
        }

        /// <summary>
        /// 关闭文件
        /// </summary>
        public void Close() {
            if (null != m_File) {
                m_File.Close();
            }
        }

        /// <summary>
        /// 得到字段数据
        /// </summary>
        /// <param name="value"></param>
        private void _Set(ref int value) {
            if (m_CurrentColumn >= m_PreprocessData.m_ColumnCount) {
                //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + "] line[" + m_CurrentLine + "], column[" + m_CurrentColumn + "], max column count[" + m_PreprocessData.m_ColumnCount + "]");
                _EndParseColumn();
                return;
            }
            if (!_CheckColumnType(EM_TYPE_COLUMN.INT)) {
                _EndParseColumn();
                return;
            }
            string subString = _GetSubString(m_ReadBuffer, 0, m_ReadBuffer.Length, m_PreprocessData.m_Data[m_CurrentLine].m_ColumnField[m_CurrentColumn]);
            if (string.IsNullOrWhiteSpace(subString)) {
                // _EndParseColumn();
                // //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + "] has invalid value in line[" + m_CurrentLine + "] and column[" + m_CurrentColumn + "]");
                // return;
                value = 0;
            } else {
                value = StringUtils.StringToInt(subString);
            }

            _EndParseColumn();
        }

        /// <summary>
        /// 得到字段数据
        /// </summary>
        /// <param name="value"></param>
        private void _Set(ref uint value) {
            if (m_CurrentColumn >= m_PreprocessData.m_ColumnCount) {
                //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + "] line[" + m_CurrentLine + "], column[" + m_CurrentColumn + "], max column count[" + m_PreprocessData.m_ColumnCount + "]");
                _EndParseColumn();
                return;
            }
            if (!_CheckColumnType(EM_TYPE_COLUMN.UINT)) {
                _EndParseColumn();
                return;
            }
            string subString = _GetSubString(m_ReadBuffer, 0, m_ReadBuffer.Length, m_PreprocessData.m_Data[m_CurrentLine].m_ColumnField[m_CurrentColumn]);
            if (string.IsNullOrWhiteSpace(subString)) {
                // _EndParseColumn();
                // //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + " has invalid value in line[" + m_CurrentLine + "] and column[" + m_CurrentColumn + "]");
                // return;
                value = 0;
            } else {
                value = StringUtils.StringToUInt(subString);
            }
            _EndParseColumn();
        }

        /// <summary>
        /// 得到字段数据
        /// </summary>
        /// <param name="value"></param>
        private void _Set(ref long value) {
            if (m_CurrentColumn >= m_PreprocessData.m_ColumnCount) {
                //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + "] line[" + m_CurrentLine + "], column[" + m_CurrentColumn + "], max column count[" + m_PreprocessData.m_ColumnCount + "]");
                _EndParseColumn();
                return;
            }
            if (!_CheckColumnType(EM_TYPE_COLUMN.LONG)) {
                _EndParseColumn();
                return;
            }
            string subString = _GetSubString(m_ReadBuffer, 0, m_ReadBuffer.Length, m_PreprocessData.m_Data[m_CurrentLine].m_ColumnField[m_CurrentColumn]);
            if (string.IsNullOrWhiteSpace(subString)) {
                value = 0;
            } else {
                value = StringUtils.StringToLong(subString);
            }
            _EndParseColumn();
        }

        /// <summary>
        /// 得到字段数据
        /// </summary>
        /// <param name="value"></param>
        private void _Set(ref ulong value) {
            if (m_CurrentColumn >= m_PreprocessData.m_ColumnCount) {
                //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + "] line[" + m_CurrentLine + "], column[" + m_CurrentColumn + "], max column count[" + m_PreprocessData.m_ColumnCount + "]");
                _EndParseColumn();
                return;
            }

            if (!_CheckColumnType(EM_TYPE_COLUMN.ULONG)) {
                _EndParseColumn();
                return;
            }
            string subString = _GetSubString(m_ReadBuffer, 0, m_ReadBuffer.Length, m_PreprocessData.m_Data[m_CurrentLine].m_ColumnField[m_CurrentColumn]);
            if (string.IsNullOrWhiteSpace(subString)) {
                value = 0;
            } else {
                value = StringUtils.StringToULong(subString);
            }
            _EndParseColumn();
        }

        /// <summary>
        /// 得到字段数据
        /// </summary>
        /// <param name="value"></param>
        private void _Set(ref float value) {
            if (m_CurrentColumn >= m_PreprocessData.m_ColumnCount) {
                //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + "] line[" + m_CurrentLine + "], column[" + m_CurrentColumn + "], max column count[" + m_PreprocessData.m_ColumnCount + "]");
                _EndParseColumn();
                return;
            }
            if (!_CheckColumnType(EM_TYPE_COLUMN.FLOAT)) {
                _EndParseColumn();
                return;
            }
            string subString = _GetSubString(m_ReadBuffer, 0, m_ReadBuffer.Length, m_PreprocessData.m_Data[m_CurrentLine].m_ColumnField[m_CurrentColumn]);
            if (string.IsNullOrWhiteSpace(subString)) {
                value = 0.0f;
            } else {
                value = StringUtils.StringToFloat(subString);
            }
            _EndParseColumn();
        }

        /// <summary>
        /// 得到字段数据
        /// </summary>
        /// <param name="value"></param>
        private void _Set(ref string value) {
            if (m_CurrentColumn >= m_PreprocessData.m_ColumnCount) {
                //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + "] line[" + m_CurrentLine + "], column[" + m_CurrentColumn + "], max column count[" + m_PreprocessData.m_ColumnCount + "]");
                _EndParseColumn();
                return;
            }
            if (!_CheckColumnType(EM_TYPE_COLUMN.STRING)) {
                _EndParseColumn();
                return;
            }
            string subString = _GetSubString(m_ReadBuffer, 0, m_ReadBuffer.Length, m_PreprocessData.m_Data[m_CurrentLine].m_ColumnField[m_CurrentColumn]);
            if (string.IsNullOrWhiteSpace(subString)) {
                value = "";
            } else {
                value = subString;
            }
            _EndParseColumn();
        }

        /// <summary>
        /// 得到字段数据
        /// </summary>
        /// <param name="value"></param>
        private void _Set(ref List<int> value) {
            if (m_CurrentColumn >= m_PreprocessData.m_ColumnCount) {
                //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + "] line[" + m_CurrentLine + "], column[" + m_CurrentColumn + "], max column count[" + m_PreprocessData.m_ColumnCount + "]");
                _EndParseColumn();
                return;
            }
            if (!_CheckColumnType(EM_TYPE_COLUMN.INT_ARRAY)) {
                _EndParseColumn();
                return;
            }

            string subString = _GetSubString(m_ReadBuffer, 0, m_ReadBuffer.Length, m_PreprocessData.m_Data[m_CurrentLine].m_ColumnField[m_CurrentColumn]);
            if (DEFAUL_TABLE_FIELD == subString || string.IsNullOrWhiteSpace(subString)) {
                value = null;
            } else {
                string[] parseStrList = subString.Split(TABLE_STRING_ARRAY_SEPARATOR.ToCharArray());
                value = new List<int>(parseStrList.Length);
                for (int i = 0; i < parseStrList.Length; ++i) {
                    if (string.IsNullOrWhiteSpace(parseStrList[i])) {
                        value.Add(0);
                    } else {
                        if (!int.TryParse(parseStrList[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out int res)) {
                            value.Add(0);
                            //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + " has invalid value in line[" + m_CurrentLine + "] and column[" + m_CurrentColumn + "]");
                        } else {
                            value.Add(res);
                        }
                    }
                }
            }
            _EndParseColumn();
        }

        /// <summary>
        /// 得到字段数据
        /// </summary>
        /// <param name="value"></param>
        private void _Set(ref List<float> value) {
            if (m_CurrentColumn >= m_PreprocessData.m_ColumnCount) {
                //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + "] line[" + m_CurrentLine + "], column[" + m_CurrentColumn + "], max column count[" + m_PreprocessData.m_ColumnCount + "]");
                _EndParseColumn();
                return;
            }
            if (!_CheckColumnType(EM_TYPE_COLUMN.FLOAT_ARRAY)) {
                _EndParseColumn();
                return;
            }

            string subString = _GetSubString(m_ReadBuffer, 0, m_ReadBuffer.Length, m_PreprocessData.m_Data[m_CurrentLine].m_ColumnField[m_CurrentColumn]);
            if (DEFAUL_TABLE_FIELD == subString || string.IsNullOrWhiteSpace(subString)) {
                value = null;
            } else {
                string[] parseStrList = subString.Split(TABLE_STRING_ARRAY_SEPARATOR.ToCharArray());
                value = new List<float>(parseStrList.Length);
                for (int i = 0; i < parseStrList.Length; ++i) {
                    if (string.IsNullOrEmpty(parseStrList[i])) {
                        //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + " has invalid value in line[" + m_CurrentLine + "] and column[" + m_CurrentColumn + "]");
                        value.Add(0.0f);
                    } else {
                        value.Add(StringUtils.StringToFloat(parseStrList[i]));
                    }
                }
            }
            _EndParseColumn();
        }

        private void _Set(ref List<string> value) {
            if (m_CurrentColumn >= m_PreprocessData.m_ColumnCount) {
                //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + "] line[" + m_CurrentLine + "], column[" + m_CurrentColumn + "], max column count[" + m_PreprocessData.m_ColumnCount + "]");
                _EndParseColumn();
                return;
            }
            if (!_CheckColumnType(EM_TYPE_COLUMN.STRING_ARRAY)) {
                _EndParseColumn();
                return;
            }

            string subString = _GetSubString(m_ReadBuffer, 0, m_ReadBuffer.Length, m_PreprocessData.m_Data[m_CurrentLine].m_ColumnField[m_CurrentColumn]);
            if (DEFAUL_TABLE_FIELD == subString || string.IsNullOrWhiteSpace(subString)) {
                value = null;
            } else {
                string[] parseStrList = subString.Split(TABLE_STRING_ARRAY_SEPARATOR.ToCharArray());
                value = new List<string>(parseStrList.Length);
                for (int i = 0; i < parseStrList.Length; ++i) {
                    if (string.IsNullOrWhiteSpace(parseStrList[i])) {
                        //LogUtils.LogError("table[" + m_FileName + "] id[" + m_CurrentID + " has invalid value in line[" + m_CurrentLine + "] and column[" + m_CurrentColumn + "]");
                        value.Add("");
                    } else {
                        value.Add(parseStrList[i]);
                    }
                }
            }
            _EndParseColumn();
        }

        /// <summary>
        /// 读取文件内容至内存
        /// </summary>
        private void _ReadFileToMemory() {
            if (null == m_File) {
                return;
            }
            //根据文件大小分配缓冲区
            long fileSize = m_File.Length;
            if (fileSize <= 0) {
                return;
            }
            m_ReadBuffer = new Byte[fileSize];
            //读取整个文件到缓冲区
            int readSize = m_File.Read(m_ReadBuffer, 0, m_ReadBuffer.Length);
            if (readSize <= 0) {
                return;
            }
            //LOG(LOG_//LogUtils, StringUtils::ToString("_ReadFileToMemory FileSize:%d, ReadSize:%d, ReadBuffer:%s", fileSize, readSize, m_ReadBuffer.c_str()));
        }

        /// <summary>
        /// 读取文件行
        /// </summary>
        private void _ReadLine() {
            List<ByteSection> lineSectionArray = null;
            for (int i = 0; i < TABLE_LINE_SEPARATOR.Length; ++i) {
                lineSectionArray = _SeparateString(m_ReadBuffer, 0, m_ReadBuffer.Length, TABLE_LINE_SEPARATOR[i]);
                if (lineSectionArray.Count >= MIN_LINE_COUNT) {
                    break;
                }
            }
            if (null == lineSectionArray || lineSectionArray.Count < MIN_LINE_COUNT) {
                //LogUtils.LogError("table [" + m_FileName + "], line count[" + lineSectionArray.Count + "]");
                return;
            }

            int currentLine = 0;

            //ColumnName
            {
                List<ByteSection> columnField = _SeparateString(m_ReadBuffer, lineSectionArray[currentLine].m_SectionBeginPos, lineSectionArray[currentLine].m_SectionLength, TABLE_COLUMN_SEPARATOR);
                m_PreprocessData.m_ColumnName = new TableLine(lineSectionArray[currentLine], columnField);
                m_PreprocessData.m_ColumnCount = m_PreprocessData.m_ColumnName.m_ColumnField.Count;
                ++currentLine;
            }

            //ColumnType
            {
                List<ByteSection> columnField = _SeparateString(m_ReadBuffer, lineSectionArray[currentLine].m_SectionBeginPos, lineSectionArray[currentLine].m_SectionLength, TABLE_COLUMN_SEPARATOR);
                m_PreprocessData.m_ColumnType = new TableLine(lineSectionArray[currentLine], columnField);
                int columnTypeCount = m_PreprocessData.m_ColumnType.m_ColumnField.Count;
                if (columnTypeCount != m_PreprocessData.m_ColumnCount) {
                    //LogUtils.LogError(m_FileName + " ColumnNameCount[" + m_PreprocessData.m_ColumnCount + "]" + ", ColumnTypeCount[" + columnTypeCount + "]");
                    return;
                }
                ++currentLine;
            }

            m_PreprocessData.m_Data = new List<TableLine>();
            for (; currentLine < lineSectionArray.Count; ++currentLine) {
                if (lineSectionArray.Count <= currentLine) {
                    continue;
                }
                var pos = lineSectionArray[currentLine].m_SectionBeginPos;
                if (pos >= m_ReadBuffer.Length) {
                    continue;
                }
                //忽略该行
                if (TABLE_SKIP_CHAR == m_ReadBuffer[pos]) {
                    continue;
                }
                List<ByteSection> tempFiledSectionArray = _SeparateString(m_ReadBuffer, lineSectionArray[currentLine].m_SectionBeginPos, lineSectionArray[currentLine].m_SectionLength, TABLE_COLUMN_SEPARATOR);
                if (tempFiledSectionArray.Count != m_PreprocessData.m_ColumnCount) {
                    //LogUtils.LogError(m_FileName + " ColumnNameCount[" + m_PreprocessData.m_ColumnCount + "]" + ", ColumnCount of line[" + currentLine + "] is [" + tempFiledSectionArray.Count + "]");
                    continue;
                }
                m_PreprocessData.m_Data.Add(new TableLine(lineSectionArray[currentLine], tempFiledSectionArray));
            }
        }

        /// <summary>
        /// 保存列的名称列表
        /// </summary>
        private void _SaveColumnNames() {
            m_ColumnNames = new string[GetColumnCount()];
            for (int i = 0; i < m_ColumnNames.Length; ++i) {
                m_ColumnNames[i] = _GetSubString(m_ReadBuffer, 0, m_ReadBuffer.Length, m_PreprocessData.m_ColumnName.m_ColumnField[i]);
            }
        }

        /// <summary>
        /// 保存列的类型列表
        /// </summary>
        private void _SaveColumnTypes() {
            m_ColumnTypes = new EM_TYPE_COLUMN[GetColumnCount()];
            for (int i = 0; i < m_PreprocessData.m_ColumnName.m_ColumnField.Count; ++i) {
                m_ColumnTypes[i] = _ParseColumnType(m_PreprocessData.m_ColumnType.m_ColumnField[i]);
                if (m_ColumnTypes[i] <= EM_TYPE_COLUMN.INVALID || m_ColumnTypes[i] >= EM_TYPE_COLUMN.COUNT) {
                    //LogUtils.LogError("table [" + m_FileName + "] invalid column type = [" + m_PreprocessData.m_ColumnType.m_ColumnField[i] + "]" + "in column[" + i + "]");
                }
            }
        }

        /// <summary>
        /// 得到类型字符串
        /// </summary>
        /// <param name="fieldSection"></param>
        /// <returns></returns>
        private EM_TYPE_COLUMN _ParseColumnType(ByteSection fieldSection) {
            string fieldString = _GetSubString(m_ReadBuffer, 0, m_ReadBuffer.Length, fieldSection);
            for (int i = (int)EM_TYPE_COLUMN.INVALID + 1; i < (int)EM_TYPE_COLUMN.COUNT; ++i) {
                if (COLUMN_TYPE_TO_STRING[i] == fieldString)
                    return (EM_TYPE_COLUMN)i;
            }
            return EM_TYPE_COLUMN.INVALID;
        }

        /// <summary>
        /// 检查列类型
        /// </summary>
        /// <param name="columnType"></param>
        /// <returns></returns>
        private bool _CheckColumnType(EM_TYPE_COLUMN columnType) {
            if (m_IsCheckColumn && m_ColumnTypes[m_CurrentColumn] != columnType) {
                //LogUtils.LogError("table [" + m_FileName + "], line [" + m_CurrentLine + "] column [" + m_CurrentColumn + "] isn't match! in struct TableRow : [" + COLUMN_TYPE_TO_STRING[(int)columnType] + "] in .tab file : [" + COLUMN_TYPE_TO_STRING[(int)m_ColumnTypes[m_CurrentColumn]] + "]");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 结束当前列
        /// </summary>
        /// <returns></returns>
        private void _EndParseColumn() {
            ++m_CurrentColumn;
        }

        /// <summary>
        /// 分隔字符串
        /// </summary>
        /// <param name="bufferSectionArray"></param>
        /// <param name="sourceString"></param>
        /// <param name="off"></param>
        /// <param name="length"></param>
        /// <param name="separateString"></param>
        private static List<ByteSection> _SeparateString(Byte[] sourceString, int off, int length, string separateString, bool isLine = false) {
            if (null == sourceString || string.IsNullOrEmpty(separateString)) {
                return null;
            }
            List<int> foundPositions = _SearchBytePattern(sourceString, off, length, Encoding.UTF8.GetBytes(separateString));
            if (null == foundPositions) {
                return null;
            }

            List<ByteSection> sectionList = new List<ByteSection>();

            if (foundPositions.Count <= 0) {
                if (!isLine) {
                    sectionList.Add(new ByteSection(off, length));
                }
            } else {
                //添加第一个
                sectionList.Add(new ByteSection(off, foundPositions[0] - off));

                int separateStringSize = separateString.Length;
                for (int i = 1; i < foundPositions.Count; ++i) {
                    int beginPos = foundPositions[i - 1] + separateStringSize;
                    sectionList.Add(new ByteSection(beginPos, foundPositions[i] - beginPos));
                }

                //添加最后一个
                if (!isLine) {
                    int beginPos = foundPositions[foundPositions.Count - 1] + separateStringSize;
                    sectionList.Add(new ByteSection(beginPos, off + length - beginPos));
                }
            }

            return sectionList;
        }

        /// <summary>
        /// Byte[] 匹配
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="off"></param>
        /// <param name="length"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static List<int> _SearchBytePattern(Byte[] bytes, int off, int length, Byte[] pattern) {
            if (null == bytes || null == pattern) {
                return null;
            }
            List<int> positions = new List<int>();
            int patternLength = pattern.Length;
            Byte firstMatchByte = pattern[0];
            for (int i = off; i < off + length; ++i) {
                if (firstMatchByte == bytes[i] && off + length - i >= patternLength) {
                    bool isEqual = true;
                    for (int j = 1; j < pattern.Length; ++j) {
                        if (pattern[j] != bytes[i + j]) {
                            isEqual = false;
                            break;
                        }
                    }
                    if (isEqual) {
                        positions.Add(i);
                        i += pattern.Length - 1;
                    }
                }
            }
            return positions;
        }

        /// <summary>
        /// 获得子字符串
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="off"></param>
        /// <param name="length"></param>
        /// <param name="sectionPos"></param>
        /// <param name="sectionLength"></param>
        /// <returns></returns>
        private string _GetSubString(Byte[] sourceString, int off, int length, int sectionPos, int sectionLength) {
            if (sectionPos < off || sectionPos + sectionLength > off + length) {
                //LogUtils.LogError("SourceString:[" + sourceString + "], Off:[" + off + "], Length:[" + length + "], StringSectionBegin:[" + sectionPos + "], StringSectionLength:[" + sectionLength + "]");
                return null;
            }
            if (sectionLength <= 0) {
                return string.Empty;
            }
            return Encoding.UTF8.GetString(sourceString, sectionPos, sectionLength);
        }

        /// <summary>
        /// 获得子字符串
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="off"></param>
        /// <param name="length"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private string _GetSubString(Byte[] sourceString, int off, int length, ByteSection section) {
            return _GetSubString(sourceString, off, length, section.m_SectionBeginPos, section.m_SectionLength);
        }
    }
}