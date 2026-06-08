using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace View
{
    [CreateAssetMenu]
    public class PuzzleViewDefinition : ScriptableObject
    {
        public List<PuzzleMoveViewDefinition> Moves;

        private Dictionary<string, PuzzleMoveViewDefinition> moveByNotation;
        
        public PuzzleMoveViewDefinition GetMoveByNotation(string notation)
        {
            return moveByNotation[notation];
        }
        
        private void Awake()
        {
            moveByNotation = Moves
                ?.ToDictionary(move => move.MoveNotation);
        }
    }
}