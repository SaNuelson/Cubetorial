using System;
using System.Diagnostics;
using System.IO;
using Cubetorial.Tutorials.Scripts;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

        [MenuItem("Cubetorial/Tutorials/Open Selected Guide Markdown In Text Editor")]
        public static void OpenSelectedGuideMarkdownInTextEditor()
        {
            if (Selection.activeObject is not TutorialGuide guide)
            {
                EditorUtility.DisplayDialog(
                    "No tutorial guide selected",
                    "Select a TutorialGuide asset before opening it as markdown.",
                    "OK");
                return;
            }

            TutorialGuideMarkdownExternalEditSession.Begin(guide);
        }

        [MenuItem("Cubetorial/Tutorials/Export Selected Guide To Markdown...", true)]
        private static bool CanExportSelectedGuideToMarkdown()
        {
            return Selection.activeObject is TutorialGuide;
        }

        [MenuItem("Cubetorial/Tutorials/Open Selected Guide Markdown In Text Editor", true)]
        private static bool CanOpenSelectedGuideMarkdownInTextEditor()
        {
            return Selection.activeObject is TutorialGuide;
        }
    }

    [CustomEditor(typeof(TutorialGuide))]
    public sealed class TutorialGuideEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                if (GUILayout.Button("Open Markdown in Text Editor", GUILayout.Height(28f)))
                {
                    TutorialGuideMarkdownExternalEditSession.Begin((TutorialGuide)target);
                }
            }

            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }

    public sealed class TutorialGuideMarkdownExternalEditSession : EditorWindow
    {
        private const string EditorPathPreferenceKey = "Cubetorial.Tutorials.MarkdownEditorPath";

        [SerializeField] private string guideAssetPath;
        [SerializeField] private string markdownPath;
        [SerializeField] private string status;

        private Process editorProcess;

        public static void Begin(TutorialGuide guide)
        {
            if (guide == null)
                throw new ArgumentNullException(nameof(guide));

            var assetPath = AssetDatabase.GetAssetPath(guide);
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                EditorUtility.DisplayDialog(
                    "Guide must be saved",
                    "Save the TutorialGuide asset before editing it as markdown.",
                    "OK");
                return;
            }

            var directory = Path.Combine(Path.GetTempPath(), "Cubetorial", "TutorialGuideMarkdown");
            Directory.CreateDirectory(directory);

            var safeName = string.IsNullOrWhiteSpace(guide.name) ? "TutorialGuide" : MakeSafeFileName(guide.name);
            var tempPath = Path.Combine(directory, $"{safeName}-{Guid.NewGuid():N}.md");
            File.WriteAllText(tempPath, TutorialGuideMarkdown.Export(guide));

            var window = GetWindow<TutorialGuideMarkdownExternalEditSession>(true, "Tutorial Markdown Edit");
            window.guideAssetPath = assetPath;
            window.markdownPath = tempPath;
            window.status = "Markdown exported. Edit the file, save it, then import it back into the guide.";
            window.minSize = new Vector2(420f, 180f);
            window.ShowUtility();
            window.OpenEditor();
        }

        private void OnDisable()
        {
            if (editorProcess == null)
                return;

            editorProcess.Exited -= OnEditorProcessExited;
            editorProcess.Dispose();
            editorProcess = null;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Guide", EditorStyles.boldLabel);
            EditorGUILayout.SelectableLabel(guideAssetPath ?? "", EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            EditorGUILayout.LabelField("Temporary Markdown", EditorStyles.boldLabel);
            EditorGUILayout.SelectableLabel(markdownPath ?? "", EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(status ?? "", MessageType.Info);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Import Saved Markdown", GUILayout.Height(28f)))
                    TryImportAndClose();

                if (GUILayout.Button("Reopen Editor", GUILayout.Height(28f)))
                    OpenEditor();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Choose Editor...", GUILayout.Height(24f)))
                    ChooseEditor();

                if (GUILayout.Button("Use Default App", GUILayout.Height(24f)))
                    OpenWithDefaultApp();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Cancel Edit"))
                CancelEdit();
        }

        private void OpenEditor()
        {
            if (string.IsNullOrWhiteSpace(markdownPath) || !File.Exists(markdownPath))
            {
                status = "The temporary markdown file no longer exists.";
                Repaint();
                return;
            }

            var configuredEditor = EditorPrefs.GetString(EditorPathPreferenceKey, "");
            if (!string.IsNullOrWhiteSpace(configuredEditor) && File.Exists(configuredEditor))
            {
                OpenWithConfiguredEditor(configuredEditor);
                return;
            }

            OpenWithDefaultApp();
        }

        private void OpenWithConfiguredEditor(string editorPath)
        {
            try
            {
                editorProcess?.Dispose();
                editorProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = editorPath,
                        Arguments = QuoteArgument(markdownPath),
                        UseShellExecute = false
                    },
                    EnableRaisingEvents = true
                };

                editorProcess.Exited += OnEditorProcessExited;
                editorProcess.Start();
                status = "Editor launched. If the editor process exits normally, Unity will try to import the saved markdown automatically.";
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                status = $"Could not launch configured editor. {exception.Message}";
            }

            Repaint();
        }

        private void OpenWithDefaultApp()
        {
            EditorUtility.OpenWithDefaultApp(markdownPath);
            status = "Opened with the system default app. Save the file, then click Import Saved Markdown.";
            Repaint();
        }

        private void ChooseEditor()
        {
            var editorPath = EditorUtility.OpenFilePanel("Choose Markdown Editor", "", "exe");
            if (string.IsNullOrWhiteSpace(editorPath))
                return;

            EditorPrefs.SetString(EditorPathPreferenceKey, editorPath);
            OpenWithConfiguredEditor(editorPath);
        }

        private void OnEditorProcessExited(object sender, EventArgs args)
        {
            EditorApplication.delayCall += TryImportAndClose;
        }

        private void TryImportAndClose()
        {
            if (string.IsNullOrWhiteSpace(markdownPath) || !File.Exists(markdownPath))
            {
                status = "The temporary markdown file no longer exists.";
                Repaint();
                return;
            }

            var guide = AssetDatabase.LoadAssetAtPath<TutorialGuide>(guideAssetPath);
            if (guide == null)
            {
                status = "The TutorialGuide asset could not be loaded.";
                Repaint();
                return;
            }

            try
            {
                Undo.RecordObject(guide, "Import Tutorial Guide Markdown");
                TutorialGuideMarkdown.ImportInto(guide, File.ReadAllText(markdownPath));
                EditorUtility.SetDirty(guide);
                AssetDatabase.SaveAssets();
                status = "Markdown imported into the guide.";
                Close();
            }
            catch (Exception exception) when (exception is FormatException || exception is ArgumentException)
            {
                var fix = EditorUtility.DisplayDialog(
                    "Markdown import failed",
                    $"{exception.Message}\n\nReopen the editor and fix the markdown, or cancel this external edit.",
                    "Reopen Editor",
                    "Cancel Edit");

                if (fix)
                {
                    status = $"Import failed: {exception.Message}";
                    OpenEditor();
                }
                else
                {
                    CancelEdit();
                }
            }
        }

        private void CancelEdit()
        {
            TryDeleteTempFile();
            Close();
        }

        private void TryDeleteTempFile()
        {
            if (string.IsNullOrWhiteSpace(markdownPath) || !File.Exists(markdownPath))
                return;

            try
            {
                File.Delete(markdownPath);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private static string MakeSafeFileName(string value)
        {
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(invalidChar, '-');
            }

            return value;
        }

        private static string QuoteArgument(string value)
        {
            return $"\"{value.Replace("\"", "\\\"")}\"";
        }
    }
}
