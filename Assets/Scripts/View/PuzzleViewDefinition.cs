using System;
using System.Collections.Generic;
using System.Linq;
using Cubetorial.Model;
using Cubetorial.Model.Base;
using Cubetorial.Model.Rubik3;
using Cubetorial.Model.Skewb;
using Sirenix.OdinInspector;
using UnityEngine;

namespace View
{
    [CreateAssetMenu]
    public class PuzzleViewDefinition : ScriptableObject
    {
        public PuzzleFamily family;
        
        public List<PuzzleMoveViewDefinition> Moves;

        private Dictionary<string, PuzzleMoveViewDefinition> moveByNotation;

        public Puzzle CreateModel()
            => family switch
            {
                PuzzleFamily.Rubik3 => Rubik3Puzzle.Create(),
                PuzzleFamily.Skewb => SkewbPuzzle.Create(),
                _ => throw new ArgumentOutOfRangeException()
            };
        
        public PuzzleMoveViewDefinition GetMoveByNotation(string notation)
        {
            if (moveByNotation == null)
                Initialize();
            
            return moveByNotation[notation];
        }
        
        private void Initialize()
        {
            moveByNotation = Moves
                ?.ToDictionary(move => move.MoveNotation);
        }

        [Button("Duplicate Move")]
        public void DuplicateMove()
        {
            if (Moves == null || Moves.Count == 0)
                return;

            Moves.Add(Moves[^1].Clone());
        }
    }
}