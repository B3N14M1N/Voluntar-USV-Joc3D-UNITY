using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(Node)), CanEditMultipleObjects]
public class GeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {

        if (DrawDefaultInspector()) { };

        if (targets.Length == 1)
        {
            Node script = (Node)target;

            EditorGUILayout.BeginHorizontal();
            GUILayout.TextField("Node Name:", EditorStyles.boldLabel);
            string input = GUILayout.TextField(script.name);
            if(input != string.Empty)
            {
                script.name = input;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Node"))
            {
                Selection.activeObject = script.AddPath();
            }


            if (GUILayout.Button("Remove Node"))
            {
                Selection.activeObject = script.RemoveNode();
            }

            EditorGUILayout.EndHorizontal();
        }
        // Check if multiple targets are selected
        if (targets.Length > 1)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Merge Paths"))
            {
                List<Node> nodes = new List<Node>();
                foreach (Transform t in Selection.GetTransforms(SelectionMode.Unfiltered))
                {
                    if (t.GetComponent<Node>() != null)
                    {
                        nodes.Add(t.GetComponent<Node>());
                    }
                }

                foreach (Node target in targets)
                {
                    target.MergePaths(nodes);
                }
            }

            if (GUILayout.Button("Remove Node"))
            {
                foreach (Node target in targets.Cast<Node>())
                {
                    target.RemoveNode();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
