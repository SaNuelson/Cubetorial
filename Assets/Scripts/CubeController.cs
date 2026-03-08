using System;
using System.Collections.Generic;
using Cube.Cube333;
using UnityEngine;

namespace DefaultNamespace
{
    public class Cube333Controller : MonoBehaviour
    {
        public Cube333 cube;

        public void Update()
        {
            HandleCamera();
            HandleCube();
        }

        private void HandleCamera()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            cube.transform.Rotate(Vector3.up, horizontal);
            cube.transform.Rotate(Vector3.right, vertical);
        }

        private void HandleCube()
        {
            var reverse = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

            if (Input.GetKey(KeyCode.Keypad8))
            {
                cube.Rotate(Cube3Axis.Up, reverse);
            }

            if (Input.GetKey(KeyCode.Keypad2))
            {
                cube.Rotate(Cube3Axis.Down, reverse);
            }

            if (Input.GetKey(KeyCode.Keypad4))
            {
                cube.Rotate(Cube3Axis.Left, reverse);
            }

            if (Input.GetKey(KeyCode.Keypad6))
            {
                cube.Rotate(Cube3Axis.Right, reverse);
            }

            if (Input.GetKey(KeyCode.Keypad5))
            {
                cube.Rotate(Cube3Axis.Front, reverse);
            }

            if (Input.GetKey(KeyCode.Keypad7))
            {
                cube.Rotate(Cube3Axis.Back, reverse);
            }
            
            if (Input.GetKey(KeyCode.Keypad9))
            {
                cube.Rotate(Cube3Axis.MidClockwise, reverse);
            }
            
            if (Input.GetKey(KeyCode.Keypad3))
            {
                cube.Rotate(Cube3Axis.MidRight, reverse);
            }
            
            if (Input.GetKey(KeyCode.Keypad1))
            {
                cube.Rotate(Cube3Axis.MidDown, reverse);
            }
        }
    }
}