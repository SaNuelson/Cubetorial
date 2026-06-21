using System;
using System.Collections.Generic;
using System.Linq;
using Cubetorial.Model;
using UnityEngine;
using Cubetorial.Model.Base;
using Cubetorial.Model.Rubik3;
using Cubetorial.Model.Skewb;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Utils;

namespace View
{
    public class PuzzleView : MonoBehaviour
    {
        [OnValueChanged(nameof(Initialize))]
        public PuzzleViewDefinition definition;
        
        private Puzzle model;

        public Puzzle Model
        {
            get
            {
                if (model == null)
                    Initialize();
                return model;
            }
        }
        public PuzzleState state;
        
        /// <summary>
        /// Piece currently occupying a slot.
        /// </summary>
        public Dictionary<int, PuzzlePieceView> PieceInSlot;
        /// <summary>
        /// Piece belonging to a slot in a solved state.
        /// </summary>
        public Dictionary<int, PuzzlePieceView> PieceFromSlot;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            Debug.Log($"PuzzleView {name} initalizing (definition null? {definition is null})");
            if (definition is null)
                return;
            
            model = definition.CreateModel();
            state = model.SolvedState;
            
            PieceInSlot = new Dictionary<int, PuzzlePieceView>();
            PieceFromSlot = new Dictionary<int, PuzzlePieceView>();
            var pieces = GetComponentsInChildren<PuzzlePieceView>();
            foreach (var piece in pieces)
            {
                piece.Model = model.Pieces[piece.SlotIndex];
                PieceFromSlot.Add(piece.SlotIndex, piece);
                PieceInSlot.Add(piece.SlotIndex, piece);
            }
        }
        
        #region Movement

        public bool CanMove => !isAnimating;
        
        public void ApplyMove(string moveId)
        {
            if (!CanMove)
                return;
            
            // Animate
            var moveModel = model.GetMove(moveId);
            var movedSlotIndices = moveModel.SourceSlotIndices;
            var movedPieces = movedSlotIndices
                .Select(slotIndex => PieceInSlot[slotIndex])
                .ToArray();
            
            // Update mapping
            var oldPieceInSlot = PieceInSlot;
            PieceInSlot = PieceInSlot.ToDictionary(x => x.Key, x => x.Value);
            foreach (var move in moveModel.SlotMoves)
            {
                PieceInSlot[move.DestinationSlotIndex] = oldPieceInSlot[move.SourceSlotIndex];
            }
            
            // Update model
            var moveView = definition.GetMoveByNotation(moveId);
            AnimateMove(moveView, movedPieces);
            
            state = state.Apply(moveModel);
        }

        #endregion
        
        #region Animation
        
        [ReadOnly] private bool isAnimating;

        private void AnimateMove(PuzzleMoveViewDefinition move, PuzzlePieceView[] movedPieces)
        {
            isAnimating = true;
            
            var moveGroup = new GameObject("Move");
            moveGroup.transform.SetParent(transform, false);
            moveGroup.transform.localPosition = move.LocalPivot;
            moveGroup.transform.localRotation = Quaternion.identity;

            foreach (var piece in movedPieces)
            {
                piece.transform.SetParent(moveGroup.transform, true);
            }

            var axis = move.LocalAxis.normalized;
            var duration = 0.4f;

            DOVirtual
                .Float(0f, move.AngleDegrees, duration, angle =>
                {
                    moveGroup.transform.localRotation = Quaternion.AngleAxis(angle, axis);
                })
                .SetEase(Ease.InOutCubic)
                .OnComplete(() =>
                {
                    foreach (var piece in movedPieces)
                    {
                        piece.transform.SetParent(transform, true);
                    }

                    Destroy(moveGroup);
                    isAnimating = false;
                });
        }

        #endregion

        #region Editor

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (definition is null)
                return;

            foreach (var moveView in definition.Moves)
            {
                var centerPosition = transform.position;
                var pivotPosition = centerPosition + moveView.LocalPivot;
                
                Gizmos.DrawSphere(pivotPosition, 0.05f);
                Handles.Label(pivotPosition, moveView.MoveNotation);
                
                // Axis line
                Handles.DrawDottedLine(centerPosition, pivotPosition, 0.1f);
                
                // Rotation arc
                var radiusVector = moveView.LocalAxis.normalized;
                var fromDirection = Vector3.Cross(moveView.LocalAxis, Vector3.up).normalized;
                if (fromDirection.sqrMagnitude < 0.1f)
                {
                    fromDirection = Vector3.Cross(moveView.LocalAxis, Vector3.right).normalized;
                }

                var arcRadius = 1f;//moveView.LocalAxis.magnitude;
                
                using (new Handles.DrawingScope(Color.white.WithAlpha(0.2f)))
                {
                    Handles.DrawWireArc(pivotPosition, moveView.LocalAxis.normalized, fromDirection, 360f, arcRadius);
                    Handles.DrawSolidArc(pivotPosition, moveView.LocalAxis.normalized, fromDirection, moveView.AngleDegrees, arcRadius);
                }
                
                // Draw arrow at the end of the arc
                var angleRadians = moveView.AngleDegrees * Mathf.Deg2Rad;
                var endDirection = Quaternion.AngleAxis(moveView.AngleDegrees, moveView.LocalAxis.normalized) * fromDirection;
                var arcEndPoint = pivotPosition + endDirection * arcRadius;
                var arrowDirection = Vector3.Cross(moveView.LocalAxis.normalized, endDirection).normalized;
                var arrowSize = arcRadius * 0.15f;
                var arrowWing1 = arcEndPoint - arrowDirection * arrowSize + endDirection * arrowSize * 0.3f;
                var arrowWing2 = arcEndPoint - arrowDirection * arrowSize - endDirection * arrowSize * 0.3f;
                Handles.DrawLine(arcEndPoint, arrowWing1);
                Handles.DrawLine(arcEndPoint, arrowWing2);
            }
        }
        #endif

        #endregion
    }
}