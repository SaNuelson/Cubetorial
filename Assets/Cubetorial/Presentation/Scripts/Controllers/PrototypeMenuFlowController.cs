using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PrototypeMenuFlowController : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset mainMenu;
    [SerializeField] private VisualTreeAsset tutorialPicker;
    [SerializeField] private VisualTreeAsset lessonPlayer;

    private readonly string[] stepTitles =
    {
        "Find the target pieces",
        "Rotate the top face",
        "Check the result"
    };

    private readonly string[] stepBodies =
    {
        "The tutorial explains what the learner should look for before any move is played.",
        "The puzzle view will animate the authored move sequence for this step.",
        "The final state and highlights show what changed and why it matters."
    };

    private UIDocument document;
    private int currentStep;

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        LoadScreen(mainMenu);

        document.rootVisualElement.Q<Button>("StartTutorialButton")
            ?.RegisterCallback<ClickEvent>(_ => ShowTutorialPicker());

        document.rootVisualElement.Q<Button>("OpenSandboxButton")
            ?.RegisterCallback<ClickEvent>(_ => Debug.Log("Sandbox flow is not wired yet."));
    }

    private void ShowTutorialPicker()
    {
        LoadScreen(tutorialPicker);

        document.rootVisualElement.Q<Button>("BeginFirstLayerButton")
            ?.RegisterCallback<ClickEvent>(_ => ShowLessonPlayer());

        document.rootVisualElement.Q<Button>("BeginSkewbButton")
            ?.RegisterCallback<ClickEvent>(_ => ShowLessonPlayer());

        document.rootVisualElement.Q<Button>("BackButton")
            ?.RegisterCallback<ClickEvent>(_ => ShowMainMenu());
    }

    private void ShowLessonPlayer()
    {
        currentStep = 0;
        LoadScreen(lessonPlayer);
        BindLessonControls();
        RefreshLessonStep();
    }

    private void BindLessonControls()
    {
        document.rootVisualElement.Q<Button>("PreviousStepButton")
            ?.RegisterCallback<ClickEvent>(_ => MoveStep(-1));

        document.rootVisualElement.Q<Button>("NextStepButton")
            ?.RegisterCallback<ClickEvent>(_ => MoveStep(1));

        document.rootVisualElement.Q<Button>("LessonBackButton")
            ?.RegisterCallback<ClickEvent>(_ => ShowTutorialPicker());
    }

    private void MoveStep(int offset)
    {
        currentStep = Mathf.Clamp(currentStep + offset, 0, stepTitles.Length - 1);
        RefreshLessonStep();
    }

    private void RefreshLessonStep()
    {
        document.rootVisualElement.Q<Label>("StepCounter").text = $"Step {currentStep + 1} of {stepTitles.Length}";
        document.rootVisualElement.Q<Label>("StepTitle").text = stepTitles[currentStep];
        document.rootVisualElement.Q<Label>("StepBody").text = stepBodies[currentStep];
    }

    private void LoadScreen(VisualTreeAsset screen)
    {
        if (screen == null)
        {
            Debug.LogWarning("Prototype menu flow is missing a UXML asset reference.");
            return;
        }

        var root = document.rootVisualElement;
        root.Clear();
        screen.CloneTree(root);
    }
}
