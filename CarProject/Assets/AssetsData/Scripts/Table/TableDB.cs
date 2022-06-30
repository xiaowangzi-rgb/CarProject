using MM.Config;
using System.Collections.Generic;
using UnityEngine;

namespace MM.Config {
    public class TableDB : IManager<TableDB> {
        private TableDB() {
        }

        public Table<TableModel> ModelTable { get; private set; } = new Table<TableModel>();

        public void LoadPublicAndGameConfigTables() {
        }

        public void LoadTables() {
            Debug.Log("Load表格开始");
            ModelTable.Load("modeltable.csv",()=>{return new TableModel(); });
        }

        public void RefreshAllTables() {
          
        }
    }
}
