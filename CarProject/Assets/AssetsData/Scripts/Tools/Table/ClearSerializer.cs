using System;
using System.Collections.Generic;

namespace MM.Config  {
    /// <summary>
    /// 初始化各个元素
    /// </summary>
    public class ClearSerializer : ISerializer {
        public ClearSerializer() {
        }

        public ISerializer Parse(ref int value) {
            value = 0;
            return this;
        }

        public ISerializer Parse(ref uint value) {
            value = 0;
            return this;
        }

        public ISerializer Parse(ref long value) {
            value = 0;
            return this;
        }

        public ISerializer Parse(ref ulong value) {
            value = 0;
            return this;
        }

        public ISerializer Parse(ref float value) {
            value = 0.0F;
            return this;
        }

        public ISerializer Parse(ref string value) {
            value = null;
            return this;
        }

        public ISerializer Parse(ref List<int> value) {
            return this;
        }

        public ISerializer Parse(ref List<float> value) {
            return this;
        }

        public void SkipField() {
        }

        public void SetCheckColumn(bool isCheck) {
        }

        public void SetCurrentID(int id) {
        }

        public ISerializer Parse(ref List<string> value) {
            return this;
        }
    }
}