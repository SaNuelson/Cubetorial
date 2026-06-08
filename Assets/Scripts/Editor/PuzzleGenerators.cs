using Cubetorial.Model.Base;
using Cubetorial.Model.Rubik3;
using UnityEditor;
using UnityEngine;
using View;

namespace Editor
{
    public static class PuzzleGenerators
    {
        [MenuItem("Cubetorial/Rubik3/CreateSkeleton")]
        public static void CreateRubik3Skeleton()
        {
            var root = Selection.activeGameObject;
            if (root == null)
            {
                var result = EditorDialog.DisplayDecisionDialog("No game object selected", "Create skeleton around a new root?",
                    "Create root", "Cancel");

                if (!result)
                    return;
                
                root = new GameObject("Rubik3");
            }

            Puzzle model;
            if (!root.GetComponent<PuzzleView>())
            {
                var puzzleView = root.AddComponent<PuzzleView>();
                puzzleView.Model = Rubik3Puzzle.Create();
                model = puzzleView.Model;
            }
            else
            {
                model = root.GetComponent<PuzzleView>().Model;
                if (model is not { Id: Rubik3Puzzle.Id })
                {
                    Debug.LogError($"Selected root has missing or invalid puzzle type. Expected {Rubik3Puzzle.Id}, got {model.Id}.");
                    return;
                }
            }

            var iLabels = new[] { "L", "", "R" };
            var jLabels = new[] { "D", "", "U" };
            var kLabels = new[] { "F", "", "B" };
            for (var i = -1; i < 2; i++)
            {
                for (var j = -1; j < 2; j++)
                {
                    for (var k = -1; k < 2; k++)
                    {
                        if (i == 0 && j == 0 && k == 0)
                            continue;
                        
                        var label = $"{iLabels[i+1]}{jLabels[j+1]}{kLabels[k+1]}";
                        var id = model.GetSlotIndex(label);
                        
                        var cube = new GameObject(label);
                        cube.transform.SetParent(root.transform, false);
                        cube.transform.localPosition = new Vector3(i, j, k);
                        
                        var pieceView = cube.AddComponent<PuzzlePieceView>();
                        pieceView.SlotIndex = id;
                    }
                }
            }
        }
    }
}