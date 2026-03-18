using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextTyping))]
public class DataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TextTyping data = (TextTyping)target;       
        if (GUILayout.Button("ResetValue"))
        {
            data.ResetTyping();
        }
    }
}
