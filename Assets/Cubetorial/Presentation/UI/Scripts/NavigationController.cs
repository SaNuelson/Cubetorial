using UnityEngine;
using UnityEngine.UIElements;

namespace Cubetorial.Presentation.UI.Scripts
{
    [RequireComponent(typeof(UIDocument))]
    public class NavigationController : MonoBehaviour
    {
        [SerializeField] private UIDocument document;
        [SerializeField] private NavigationGraph graph;

        private string currentScreenId;

        private void OnEnable()
        {
            if (document == null)
            {
                document = GetComponent<UIDocument>();
            }

            if (graph == null)
            {
                Debug.LogError("NavigationController is missing a NavigationGraph.");
                return;
            }

            ShowScreen(graph.initialScreenId);
        }

        public void ShowScreen(string screenId)
        {
            var screen = graph.FindScreen(screenId);
            if (screen == null)
            {
                Debug.LogError($"Unknown screen '{screenId}'.");
                return;
            }

            Load(screen);
        }

        private void BindNavigation()
        {
            var screen = graph.FindScreen(currentScreenId);
            if (screen == null)
            {
                Debug.LogError($"Unknown screen '{currentScreenId}'.");
                return;
            }

            var root = document.rootVisualElement;

            foreach (var link in screen.links)
            {
                var button = root.Q<Button>(link.buttonName);
                if (button == null)
                {
                    Debug.LogWarning($"Screen '{screen.id}' has no button named '{link.buttonName}'.");
                    continue;
                }

                var targetScreenId = link.targetScreenId;
                button.clicked += () => ShowScreen(targetScreenId);
            }
        }

        private void Load(NavigationScreen screen)
        {
            currentScreenId = screen.id;

            var root = document.rootVisualElement;
            root.Clear();
            screen.asset.CloneTree(root);

            BindNavigation();
        }
    }
}
