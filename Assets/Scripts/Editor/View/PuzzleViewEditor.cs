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

            DrawStateDump(puzzleView);
        }

        private static void DrawStateDump(PuzzleView puzzleView)
        {
            if (puzzleView.PieceInSlot == null || puzzleView.PieceFromSlot == null)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State Diff", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                DrawDiffHeader();
                DrawDiffRows(puzzleView);
            }
        }

        private static void DrawDiffHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Slot", EditorStyles.boldLabel, GUILayout.Width(90f));
            GUILayout.Label("Model", EditorStyles.boldLabel);
            GUILayout.Label("View", EditorStyles.boldLabel);
            GUILayout.Label("Diff", EditorStyles.boldLabel, GUILayout.Width(80f));
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawDiffRows(PuzzleView puzzleView)
        {
            var model = puzzleView.Model;

            for (var slotIndex = 0; slotIndex < model.Pieces.Count; slotIndex++)
            {
                var pieceState = puzzleView.state.GetPieceInSlot(slotIndex);
                var slot = model.Slots[slotIndex];
                var modelPiece = model.Pieces[pieceState.PieceIndex];
                var hasViewPiece = puzzleView.PieceInSlot.TryGetValue(slotIndex, out var pieceView);
                var viewMatchesModel = hasViewPiece && pieceView.SlotIndex == pieceState.PieceIndex;

                var originalBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = viewMatchesModel ? new Color(0.35f, 0.65f, 0.35f) : new Color(0.85f, 0.45f, 0.25f);

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUI.backgroundColor = originalBackgroundColor;

                GUILayout.Label($"{slotIndex} ({slot.Id})", GUILayout.Width(90f));
                GUILayout.Label($"{modelPiece.Id} [{pieceState.PieceIndex}, ori {pieceState.Orientation}]");
                GUILayout.Label(GetViewPieceLabel(pieceView, hasViewPiece));
                GUILayout.Label(viewMatchesModel ? "OK" : "DIFF", GUILayout.Width(80f));

                EditorGUILayout.EndHorizontal();
            }
        }

        private static string GetViewPieceLabel(PuzzlePieceView pieceView, bool hasViewPiece)
        {
            if (!hasViewPiece)
                return "<missing>";

            var modelId = pieceView.Model != null ? pieceView.Model.Id : "<no model>";
            return $"{modelId} [{pieceView.SlotIndex}] ({pieceView.name})";
        }
    }
}
