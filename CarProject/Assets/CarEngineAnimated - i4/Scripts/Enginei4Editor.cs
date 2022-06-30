using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif 

#if UNITY_EDITOR
[CustomEditor(typeof(Enginei4))]
public class EngineEditor : Editor {
	
	public override void OnInspectorGUI(){
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		Enginei4 targetScript = (Enginei4) target;

		using (new EditorGUILayout.HorizontalScope ()) {
			if (GUILayout.Button ("Tuning 1")) targetScript.SetVariation (0);
			if (GUILayout.Button ("Tuning 2")) targetScript.SetVariation (1);
			if (GUILayout.Button ("Tuning 3")) targetScript.SetVariation (2);
			if (GUILayout.Button ("Tuning 4")) targetScript.SetVariation (3);
		}

		if (GUILayout.Button ("Activate all gameobjects"))
			targetScript.ActivateAllObjects ();

		EditorGUILayout.Space ();

		using (new EditorGUILayout.HorizontalScope ()) {
			if (GUILayout.Button ("Enable transparency")) targetScript.EnableTransparency ();
			if (GUILayout.Button ("Disable transparency")) targetScript.DisableTransparency ();
		}
	}

}
#endif 