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
    }
}