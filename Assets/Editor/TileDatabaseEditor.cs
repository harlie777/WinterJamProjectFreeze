using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileDatabase))]
public class TileDatabaseEditor : Editor
{
    private SerializedProperty tileEntries;

    private void OnEnable()
    {
        tileEntries = serializedObject.FindProperty("tiles");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Tile Database", EditorStyles.boldLabel);

        for (int i = 0; i < tileEntries.arraySize; i++)
        {
            SerializedProperty element = tileEntries.GetArrayElementAtIndex(i);
            SerializedProperty typeProp = element.FindPropertyRelative("type");
            SerializedProperty dataProp = element.FindPropertyRelative("data");

            string label = typeProp.enumDisplayNames[typeProp.enumValueIndex];
            EditorGUILayout.PropertyField(dataProp, new GUIContent(label));
        }

        serializedObject.ApplyModifiedProperties();
    }
}