using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cube.Cube333
{
    public class Cubelet333 : MonoBehaviour
    {
        [HideInInspector] public const float OffsetDelta = 1e-5f;
        
        [ReadOnly]
        public Vector3Int Coordinates;
        
        public Renderer Left;
        public Renderer Right;
        public Renderer Top;
        public Renderer Bottom;
        public Renderer Front;
        public Renderer Back;

        [ReadOnly] private bool IsUp  => transform.position.y > OffsetDelta;
        [ReadOnly] private bool IsDown  => transform.position.y < -OffsetDelta;
        [ReadOnly] private bool IsLeft  => transform.position.z < -OffsetDelta;
        [ReadOnly] private bool IsRight  => transform.position.z > OffsetDelta;
        [ReadOnly] private bool IsFront  => transform.position.x > OffsetDelta;
        [ReadOnly] private bool IsBack  => transform.position.x < -OffsetDelta;

        public bool IsInFace(Cube3Axis axis) => axis switch
        {
            Cube3Axis.Up => IsUp,
            Cube3Axis.Down => IsDown,
            Cube3Axis.Left => IsLeft,
            Cube3Axis.Right => IsRight,
            Cube3Axis.Front => IsFront,
            Cube3Axis.Back => IsBack,
            
            Cube3Axis.MidDown => !IsLeft && !IsRight,
            Cube3Axis.MidRight => !IsUp && !IsDown,
            Cube3Axis.MidClockwise => !IsFront && !IsBack,
            
            _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
        };
        
        private void OnValidate()
        {
            var pos = transform.position;
            var inferred = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
            Coordinates = inferred;

            if (Left.sharedMaterial is null)
                return;
            
            Left.sharedMaterial.color = Color.gray;
            Right.sharedMaterial.color = Color.gray;
            Top.sharedMaterial.color = Color.gray;
            Bottom.sharedMaterial.color = Color.gray;
            Front.sharedMaterial.color = Color.gray;
            Back.sharedMaterial.color = Color.gray;

            if (IsBack)
            {
                Back.sharedMaterial.color = Cube333.BackColor;
            }

            if (IsFront)
            {
                Front.sharedMaterial.color = Cube333.FrontColor;
            }

            if (IsDown)
            {
                Bottom.sharedMaterial.color = Cube333.BottomColor;
            }

            if (IsUp)
            {
                Top.sharedMaterial.color = Cube333.TopColor;
            }

            if (IsLeft)
            {
                Left.sharedMaterial.color = Cube333.LeftColor;
            }

            if (IsRight)
            {
                Right.sharedMaterial.color = Cube333.RightColor;
            }
        }

        #region Rotation handling

        public void Select()
        {
            this.transform.DOScale(Vector3.one * 1.1f, 0.2f);
        }
        
        public void Deselect()
        {
            this.transform.DOScale(Vector3.one, 0.2f);
        }

        public void ShowHandles()
        {
            
        }
        
        public void HideHandles()
        {
            
        }

        #endregion
    }
}
