namespace MM.Config  {
    public class TableRow {
        public static ClearSerializer g_ClearSerializer = new ClearSerializer();
    }

    /// <summary>
    /// 表格配置接口
    /// </summary>
    public abstract class ITable {
        public int ID { get; private set; }

        public ITable(int id) {
            ID = id;
        }
        public ITable() {
        }

        /// <summary>
        /// 清空缓存 初始化接口
        /// </summary>
        public void ClearBeforeLoad() {
            ParseData(TableRow.g_ClearSerializer);
        }

        public int ParseData(ISerializer s) {
            ID = MapIndex(s);
            MapData(s);
            return ID;
        }

        protected abstract void MapData(ISerializer s);

        public virtual void Refresh() {
        }
        public virtual void OnLoad() {
        }
        public abstract void Clear();

        public int MapIndex(ISerializer s) {
            s.Parse(ref m_Index);
            s.SetCurrentID(m_Index);
            return m_Index;
        }

        public int GetIndex() {
            return m_Index;
        }

        private int m_Index;
    }
}