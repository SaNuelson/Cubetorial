using System;
using UnityEngine;
using View;

public class PuzzleController : MonoBehaviour
{
    public PuzzleView Puzzle;

    private void Start()
    {
        if (Puzzle is null)
        {
            Debug.LogError("PuzzleView is not assigned!");
            return;
        }

        Initialize();
    }
    
    public void Update()
    {
        HandleCamera();
        HandleCube();
    }

    private void Initialize()
    {
        
    }
    
    private void HandleCamera()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        Puzzle.transform.Rotate(Vector3.up, horizontal);
        Puzzle.transform.Rotate(Vector3.right, vertical);
    }

    private void HandleCube()
    {
        var reverse = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

        if (Input.GetKey(KeyCode.Keypad8))
        {
            Puzzle.ApplyMove(reverse ? "U'" : "U");
        }

        if (Input.GetKey(KeyCode.Keypad2))
        {
            Puzzle.ApplyMove(reverse ? "D'" : "D");
        }

        if (Input.GetKey(KeyCode.Keypad4))
        {
            Puzzle.ApplyMove(reverse ? "L'" : "L");
        }

        if (Input.GetKey(KeyCode.Keypad6))
        {
            Puzzle.ApplyMove(reverse ? "R'" : "R");
        }

        if (Input.GetKey(KeyCode.Keypad5))
        {
            Puzzle.ApplyMove(reverse ? "F'" : "F");
        }

        if (Input.GetKey(KeyCode.Keypad7))
        {
            Puzzle.ApplyMove(reverse ? "B'" : "B");
        }
    }
}