using System;
using UnityEngine;

namespace View
{
    [Serializable]
    public class PuzzleMoveViewDefinition
    {
        public string MoveNotation;
        
        public string Title;
        public Vector3 LocalPivot;
        public Vector3 LocalAxis;
        public float AngleDegrees;
        
        public PuzzleMoveViewDefinition Clone()
        {
            return new PuzzleMoveViewDefinition
            {
                MoveNotation = MoveNotation,
                Title = Title,
                LocalPivot = LocalPivot,
                LocalAxis = LocalAxis,
                AngleDegrees = AngleDegrees
            };
        }
    }
}