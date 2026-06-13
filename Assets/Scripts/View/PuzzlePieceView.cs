using Cubetorial.Model.Base;
using UnityEngine;

namespace View
{
    public class PuzzlePieceView : MonoBehaviour
    {
        public int SlotIndex;

        [HideInInspector] public Piece Model;
    }
}