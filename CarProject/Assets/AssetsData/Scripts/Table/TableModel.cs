using MM.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

public class TableModel : ITable
{
    public string Model_Name;
    public int Model_Trigger;
    public string Model_dec;//√Ë ˆ

    public override void Clear()
    {
    }

    protected override void MapData(ISerializer s)
    {
        s.Parse(ref Model_Name);
        s.Parse(ref Model_Trigger);
        s.Parse(ref Model_dec);
    }

    public override void OnLoad()
    {
        base.OnLoad();
        Log.Debug("Model_Name : " + Model_Name + "Model_Trigger :" + Model_Trigger + "Model_dec : " + Model_dec);
    }
}
