using System.Collections.Generic;
using UnityEngine;
using Cubetorial.Model.Base;
using Cubetorial.Model.Rubik3;
using Sirenix.OdinInspector;

namespace View
{
    public class PuzzleView : MonoBehaviour
    {

        public Puzzle Model;
        
        /// <summary>
        /// Piece currently occupying a slot.
        /// </summary>
        private Dictionary<int, PuzzlePieceView> PieceInSlot;
        /// <summary>
        /// Piece belonging to a slot in a solved state.
        /// </summary>
        private Dictionary<int, PuzzlePieceView> PieceFromSlot;

        public void Initialize()
        {
            PieceInSlot = new Dictionary<int, PuzzlePieceView>();
            PieceFromSlot = new Dictionary<int, PuzzlePieceView>();
            var pieces = GetComponentsInChildren<PuzzlePieceView>();
            foreach (var piece in pieces)
            {
                PieceFromSlot.Add(piece.SlotIndex, piece);
                PieceInSlot.Add(piece.SlotIndex, piece);
            }
        }
        
        #region Logic

        #endregion
        
        #region Movement

        public void ApplyMove(string moveId)
        {
            
        }

        #endregion
        
        #region Animation
        
        [ReadOnly] private bool isAnimating;
        
        

        #endregion
    }
}