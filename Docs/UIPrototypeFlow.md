# UI Prototype Flow

The runtime menu flow is currently data-driven through:

- `Assets/Cubetorial/Presentation/UI/Scripts/NavigationController.cs`
- `Assets/Cubetorial/Presentation/UI/Scripts/NavigationGraph.cs`
- `Assets/Cubetorial/Presentation/UI/Navigation/MainNavigationGraph.asset`

`NavigationController` is the only MonoBehaviour involved in menu switching. It loads a `VisualTreeAsset` for the current screen, clears the `UIDocument` root, clones the new tree, then binds button names from the navigation graph.

## Current Screens

```text
MainMenu
  Guides -> GuidePuzzleSelection
  Sandbox -> SandboxPuzzleSelection
  Solver -> disabled

GuidePuzzleSelection
  3x3 Cube -> GuideTutorialSelectionRubik3
  Skewb -> GuideTutorialSelectionSkewb
  Back -> MainMenu

GuideTutorialSelectionRubik3
  Beginner method -> LessonPlayer
  Back -> GuidePuzzleSelection

GuideTutorialSelectionSkewb
  Back -> GuidePuzzleSelection

SandboxPuzzleSelection
  3x3 Cube -> SandboxRubik3
  Skewb -> SandboxSkewb
  Back -> MainMenu

SandboxRubik3
  Back -> SandboxPuzzleSelection

SandboxSkewb
  Back -> SandboxPuzzleSelection

LessonPlayer
  Back -> GuideTutorialSelectionRubik3
```

## Notes

The guide and sandbox puzzle-selection screens are intentionally separate for now. They contain similar fixed puzzle entries, but they lead to different app contexts: tutorial selection versus free puzzle simulation.

Once puzzle choices are backed by data instead of placeholder buttons, this can become one reusable puzzle-selection view controlled by navigation context:

- title text
- available puzzle list
- selected puzzle callback
- back target

Until that state/context exists, separate screens keep the placeholder flow explicit and easy to wire.
