using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using View;

namespace Editor.View
{
    [CustomEditor(typeof(PuzzlePieceView))]
    public sealed class PuzzlePieceViewEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var pieceView = (PuzzlePieceView)target;
            var puzzleView = pieceView.transform.parent.GetComponent<PuzzleView>();

            if (puzzleView is null || puzzleView.Model is null)
            {
                EditorGUILayout.LabelField($"Missing puzzle view or piece model.");
                return;
            }

            var pieceOriginalSlotIndex = pieceView.SlotIndex;
            var pieceCurrentSlotIndex = puzzleView.PieceInSlot.Where(x => x.Value == pieceView).Select(x => x.Key).FirstOrDefault();
            
            var piece = puzzleView.Model.Pieces[pieceView.SlotIndex];
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Model state");
            EditorGUILayout.LabelField($"Piece name: {piece.Id}");
            EditorGUILayout.LabelField($"Piece kind: {piece.Kind.Id}");
            var pieceInSlot = puzzleView.state.GetPieceInSlot(pieceView.SlotIndex);
            EditorGUILayout.LabelField($"Piece slot: {puzzleView.state.PiecesBySlot[pieceView.SlotIndex]}"); // TODO: Cont.
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("View state");
            EditorGUILayout.LabelField($"Orig. index: {pieceOriginalSlotIndex}");
            EditorGUILayout.LabelField($"Orig. name: {puzzleView.PieceFromSlot[pieceOriginalSlotIndex].Model.Id}");
            EditorGUILayout.LabelField($"Curr. index: {pieceCurrentSlotIndex}");
            EditorGUILayout.LabelField($"Curr. name: {puzzleView.PieceFromSlot[pieceCurrentSlotIndex].Model.Id}");
        }
    }
}