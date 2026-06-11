using Editor.Generators.Rubik3;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class PuzzleGeneratorMenu
    {
        #region Rubik3

        [MenuItem("Cubetorial/Generate/Rubik 3/Skeleton")]
        public static void CreateRubik3Skeleton()
        {
            var root = GetOrCreateRoot();
            if (root is null)
                return;
            
            Rubik3SkeletonGenerator.CreateRubik3Skeleton(root);
        }

        #endregion

        #region Utils

        [CanBeNull]
        public static GameObject GetOrCreateRoot()
        {
            var root = Selection.activeGameObject;
            if (root is not null) 
                return root;
            
            var result = EditorDialog.DisplayDecisionDialog("No game object selected", "Create skeleton around a new root?",
                "Create root", "Cancel");

            if (!result)
                return null;
                
            root = new GameObject("Rubik3");

            return root;
        }

        #endregion
    }
}