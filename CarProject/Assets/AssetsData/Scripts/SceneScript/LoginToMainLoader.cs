using GameFramework.Procedure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Fsm;
using UnityGameFramework.Runtime;

/// <summary>
/// Login到Main场景的loading
/// </summary>
public class LoginToMainLoader : ProcedureBase
{
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        //加载场景
        GameEntry.GetComponent<SceneComponent>().
            LoadScene("Assets/Study/Scene_Play.unity");
    }
}
