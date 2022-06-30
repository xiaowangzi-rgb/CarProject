using System;
using System.Collections.Generic;
using UnityEngine;

namespace MM.Config {
    public sealed class Table<T> where T : ITable, new() {

        /// <summary>
        /// 表格数据存放的地方
        /// </summary>
        private Dictionary<int, T> mTables { get; set; } = new Dictionary<int, T>();
        /// <summary>
        /// 文件路径
        /// </summary>
        private string mFileName;

        public Table() {
        }

        /// <summary>
        /// 加载表格
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool Load(string tableName, Func<ITable> func) {
            string originalTableName = tableName;

            string originalFileName = mFileName = PathUtils.GetStreamingAssetsResPath(
                PathUtils.CombinePath("table", originalTableName));
            CleanBeforeReload();
            if (string.IsNullOrEmpty(mFileName)) {
                return false;
            }

            TableSerializer s = new TableSerializer();
            s.SetCheckColumn(true);
            if (!s.OpenRead(originalFileName)) {
                Debug.LogError(originalTableName + "Open Read Table Failed.");
                return false;
            }
            s.PreprocessTable();
            int lineCount = s.GetLineCount();
            if (lineCount <= 0) {
                return false;
            }
            for (int i = 0; i < lineCount; ++i) {
                var row = func?.Invoke();
                row.ClearBeforeLoad();
                s.SetCurrentLine(i);
                mTables.Add(row.ParseData(s), row as T);
                row.OnLoad();
            }
            s.Close();
            return true;
        }

        public void Refresh() {
            List<T> values = new List<T>(mTables.Values);
            for (int i = 0; i < values.Count; ++i) {
                values[i].Refresh();
            }
        }

        /// <summary>
        /// 尝试获取某个value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(int key, out T value) {
            return mTables.TryGetValue(key, out value);
        }

        /// <summary>
        /// 通过key获取对应的value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T this[int key] {
            get {
                if (mTables.TryGetValue(key, out T value)) {
                    return value;
                }
                Debug.LogError("Table " + mFileName + " do not have key" + key.ToString());
                return default;
            }
        }

        /// <summary>
        /// 是否有某个key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsRow(int key) {
            return mTables.ContainsKey(key);
        }

        public int RowCount() {
            return mTables.Count;
        }

        /// <summary>
        /// 获取表格的所有数据，用来遍历表格
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, T> GetDatas() {
            return mTables;
        }

        public List<T> GetValues() {
            return new List<T>(mTables.Values);
        }

        public List<int> GetKeys() {
            return new List<int>(mTables.Keys);
        }

        /// <summary>
        /// 释放m_Rows
        /// </summary>
        private void CleanBeforeReload() {
            if (mTables == null) {
                mTables = new Dictionary<int, T>();
                return;
            }
            foreach (var v in mTables.Values) {
                v?.Clear();
            }
            mTables.Clear();
        }
    }
}
