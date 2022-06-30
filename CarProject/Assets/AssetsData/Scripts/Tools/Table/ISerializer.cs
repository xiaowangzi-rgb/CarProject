using System.Collections.Generic;

namespace MM.Config  {

    public interface ISerializer {
        ISerializer Parse(ref int value);
        ISerializer Parse(ref uint value);
        ISerializer Parse(ref long value);
        ISerializer Parse(ref ulong value);
        ISerializer Parse(ref float value);
        ISerializer Parse(ref string value);
        ISerializer Parse(ref List<int> value);
        ISerializer Parse(ref List<float> value);
        ISerializer Parse(ref List<string> value);
        void SkipField();
        void SetCheckColumn(bool isCheck);
        void SetCurrentID(int id);
    }
}