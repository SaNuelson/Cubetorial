using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using View;

namespace Editor.View
{
    [CustomEditor(typeof(PuzzleView))]
    public sealed class PuzzleViewEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var puzzleView = (PuzzleView)target;

            if (puzzleView.Model == null)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Moves", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                foreach (var move in puzzleView.Model.Moves)
                {
                    if (GUILayout.Button(move.Notation))
                    {
                        puzzleView.ApplyMove(move.Notation);
                    }
                }
            }
        }
    }
}