using System.IO;
using Cubetorial.Tutorials.Scripts;
using UnityEditor;
using UnityEngine;

namespace Cubetorial.Tutorials.Editor
{
    public static class TutorialGuideMarkdownMenu
    {
        [MenuItem("Cubetorial/Tutorials/Import Markdown Into Selected Guide...")]
        public static void ImportMarkdownIntoSelectedGuide()
        {
            var path = EditorUtility.OpenFilePanel("Import Tutorial Guide Markdown", Application.dataPath, "md");
            if (string.IsNullOrWhiteSpace(path))
                return;

            var guide = Selection.activeObject as TutorialGuide;
            if (guide == null)
            {
                var assetPath = EditorUtility.SaveFilePanelInProject(
                    "Create Tutorial Guide",
                    Path.GetFileNameWithoutExtension(path),
                    "asset",
                    "Choose where to create the imported tutorial guide asset.");

                if (string.IsNullOrWhiteSpace(assetPath))
                    return;

                guide = ScriptableObject.CreateInstance<TutorialGuide>();
                AssetDatabase.CreateAsset(guide, assetPath);
                Selection.activeObject = guide;
            }

            Undo.RecordObject(guide, "Import Tutorial Guide Markdown");
            TutorialGuideMarkdown.ImportInto(guide, File.ReadAllText(path));
            EditorUtility.SetDirty(guide);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Cubetorial/Tutorials/Export Selected Guide To Markdown...")]
        public static void ExportSelectedGuideToMarkdown()
        {
            if (Selection.activeObject is not TutorialGuide guide)
            {
                EditorUtility.DisplayDialog(
                    "No tutorial guide selected",
                    "Select a TutorialGuide asset before exporting.",
                    "OK");
                return;
            }

            var defaultName = string.IsNullOrWhiteSpace(guide.name) ? "TutorialGuide" : guide.name;
            var path = EditorUtility.SaveFilePanel(
                "Export Tutorial Guide Markdown",
                Application.dataPath,
                defaultName,
                "md");

            if (string.IsNullOrWhiteSpace(path))
                return;

            File.WriteAllText(path, TutorialGuideMarkdown.Export(guide));
            AssetDatabase.Refresh();
        }

        [MenuItem("Cubetorial/Tutorials/Export Selected Guide To Markdown...", true)]
        private static bool CanExportSelectedGuideToMarkdown()
        {
            return Selection.activeObject is TutorialGuide;
        }
    }
}
