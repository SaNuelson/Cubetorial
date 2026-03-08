using System;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cube.Cube333
{
    public enum Cube3Axis
    {
        Up,
        Down,
        Left,
        Right,
        Front,
        Back,
        
        MidDown,
        MidRight,
        MidClockwise,
    }

    public class Cube333 : MonoBehaviour
    {
        [ReadOnly] public Cubelet333[] cubelets;
        private Camera _camera;

        [ReadOnly] public bool isAnimating;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void OnValidate()
        {
            cubelets = this.GetComponentsInChildren<Cubelet333>();

            if (cubelets.Length != 26)
            {
                Debug.LogWarning($"Invalid Cube3, should have 26 cubelets, has {cubelets.Length}");
            }
        }

        [Button]
        public void Rotate(Cube3Axis axis, bool reverse = false)
        {
            if (isAnimating)
                return;
            isAnimating = true;
            
            var faceCubelets = cubelets
                .Where(cubelet => cubelet.IsInFace(axis))
                .ToList();

            var rotation = axis switch
            {
                Cube3Axis.Up => new Vector3(0, 90f, 0),
                Cube3Axis.Down => new Vector3(0, -90, 0),
                Cube3Axis.Left => new Vector3(0, 0, 90f),
                Cube3Axis.Right => new Vector3(0, 0, -90),
                Cube3Axis.Front => new Vector3(90f, 0, 0),
                Cube3Axis.Back => new Vector3(-90f, 0, 0),
                Cube3Axis.MidRight => new Vector3(0, -90f, 0),
                Cube3Axis.MidDown => new Vector3(0, 0, 90f),
                Cube3Axis.MidClockwise => new Vector3(90f, 0, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
            };
            
            if (reverse)
                rotation *= -1;

            var face = new GameObject("Face");
            face.transform.SetParent(transform, false);
            
            foreach (var cubelet in faceCubelets)
            {
                cubelet.transform.SetParent(face.transform, true);
            }
            
            var tween = face.transform.DOLocalRotate(rotation, .5f, RotateMode.LocalAxisAdd);
            tween.onComplete += () =>
            {
                foreach (var cubelet in faceCubelets)
                {
                    var worldPos = cubelet.transform.position;
                    cubelet.transform.SetParent(transform);
                    cubelet.transform.position = worldPos;
                }
                Destroy(face);
                isAnimating = false;
            };
            DOTween.Play(tween);
        }

        public static readonly Color TopColor = Color.yellow;
        public static readonly Color BottomColor = Color.white;
        public static readonly Color LeftColor = Color.red;
        public static readonly Color RightColor = new Color(1, .5f, 0);
        public static readonly Color FrontColor = Color.green;
        public static readonly Color BackColor = Color.blue;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleInput(Input.mousePosition);
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                HandleInput(Input.GetTouch(0).position);
            }
        }

        void HandleInput(Vector2 screenPosition)
        {
            if (!_camera) 
                return;
            
            var ray = _camera.ScreenPointToRay(screenPosition);

            if (!Physics.Raycast(ray, out var hit)) 
                return;
            
            var cubelet = hit.collider.GetComponent<Cubelet333>();

            if (cubelet)
            {
                Debug.Log("Tapped cubelet at " + cubelet.Coordinates);
                // Tell cube to handle selection
            }
        }
    }
}