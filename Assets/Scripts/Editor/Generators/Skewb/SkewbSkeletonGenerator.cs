using System;
using Cubetorial.Model.Base;
using Cubetorial.Model.Rubik3;
using Cubetorial.Model.Skewb;
using UnityEditor;
using UnityEngine;
using View;

namespace Editor.Generators.Skewb
{
    public static class SkewbSkeletonGenerator
    {
        public static void CreateSkewbSkeleton(GameObject root)
        {
            var modelView = root.GetComponent<PuzzleView>();
            Puzzle model;
            if (modelView is null)
            {
                throw new ArgumentException("Creating root not supported.");
            }
            else
            {
                model = root.GetComponent<PuzzleView>().Model;
                if (model.Id != SkewbPuzzle.Family.ToString())
                {
                    Debug.LogError($"Selected root has missing or invalid puzzle type. Expected {Rubik3Puzzle.Family.ToString()}, got {model.Id}.");
                    return;
                }
            }

            var iOrder = new[] { Rubik3FaceMask.L, (Rubik3FaceMask)0, Rubik3FaceMask.R };
            var jOrder = new[] { Rubik3FaceMask.D, (Rubik3FaceMask)0, Rubik3FaceMask.U };
            var kOrder = new[] { Rubik3FaceMask.F, (Rubik3FaceMask)0, Rubik3FaceMask.B };
            for (var i = -1; i < 2; i++)
            {
                for (var j = -1; j < 2; j++)
                {
                    for (var k = -1; k < 2; k++)
                    {
                        if (i == 0 && j == 0 && k == 0)
                            continue;
                        if ((i == 0 ? 1 : 0) + (j == 0 ? 1 : 0) + (k == 0 ? 1 : 0) == 1)
                            continue;
                        
                        var slot = (Rubik3Slot)(iOrder[i+1] | jOrder[j+1] | kOrder[k+1]);
                        var label = slot.ToString();
                        var id = model.GetSlotIndex(label);
                        
                        var cube = root.transform.Find(label)?.gameObject;
                        if (cube is null)
                        {
                            cube = new GameObject(label);
                            Undo.RegisterCreatedObjectUndo(cube, "Create Skewb Cube");
                            cube.transform.SetParent(root.transform, false);
                            cube.transform.localPosition = new Vector3(i, j, k);
                        }
                        
                        var pieceView = cube.GetComponent<PuzzlePieceView>();
                        if (pieceView is null)
                        {
                            pieceView = cube.AddComponent<PuzzlePieceView>();
                        }
                        pieceView.SlotIndex = id;
                        EditorUtility.SetDirty(cube);
                    }
                }
            }
        }
    }
}