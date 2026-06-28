using Cubetorial.Tutorials.Scripts;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cubetorial.Presentation.UI.Scripts
{
    public sealed class TutorialGuideView : MonoBehaviour
    {
        [SerializeField] private UIDocument document;
        [SerializeField] private VisualTreeAsset sectionTemplate;
        [SerializeField] private VisualTreeAsset blockTemplate;
        [SerializeField] private TutorialGuide guide;

        private Label guideTitle;
        private VisualElement guideContent;

        private void Awake()
        {
            var root = document.rootVisualElement;

            guideTitle = root.Q<Label>("GuideTitle");
            guideContent = root.Q<VisualElement>("GuideContent");

            if (guide != null)
            {
                Bind(guide);
            }
        }

        public void Bind(TutorialGuide guide)
        {
            if (guideTitle == null || guideContent == null)
            {
                Debug.LogError("Tutorial view is missing GuideTitle or GuideContent.");
                return;
            }

            guideTitle.text = guide.title;
            guideContent.Clear();

            foreach (var node in guide.sections)
            {
                guideContent.Add(CreateNodeElement(node, depth: 0));
            }
        }

        private VisualElement CreateNodeElement(GuideNode node, int depth)
        {
            return node switch
            {
                GuideSection section => CreateSectionElement(section, depth),
                GuideBlock block => CreateBlockElement(block, depth),
                _ => CreateUnknownNodeElement(node, depth)
            };
        }

        private VisualElement CreateSectionElement(GuideSection section, int depth)
        {
            var element = sectionTemplate.Instantiate();

            var sectionRoot = element.Q<VisualElement>(className: "guide-section") ?? element;
            sectionRoot.style.marginLeft = depth * 16;

            element.Q<Label>("SectionTitle").text = section.title;

            var childrenRoot = element.Q<VisualElement>("SectionChildren");
            childrenRoot.Clear();

            foreach (var child in section.nodes)
            {
                childrenRoot.Add(CreateNodeElement(child, depth + 1));
            }

            return element;
        }

        private VisualElement CreateBlockElement(GuideBlock block, int depth)
        {
            var element = blockTemplate.Instantiate();

            var blockRoot = element.Q<VisualElement>(className: "guide-block") ?? element;
            blockRoot.style.marginLeft = depth * 16;

            element.Q<Label>("BlockTitle").text = block.title;
            element.Q<Label>("BlockBody").text = block.body;

            return element;
        }

        private VisualElement CreateUnknownNodeElement(GuideNode node, int depth)
        {
            var element = new Label($"Unsupported node type: {node.GetType().Name}");
            element.style.marginLeft = depth * 16;
            return element;
        }
    }
}
