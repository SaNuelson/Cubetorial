# Current vs Target Architecture

This map relates the current repository shape to the intended Cubetorial architecture. It is a refactoring guide, not a demand that every prototype file move immediately.

## Target Shape

```text
Puzzle Model
  Plain C# puzzle topology, state, moves, validation, and queries.

Guide
  Authored educational content with guide-owned symbolic puzzle references.

Puzzle Presentation
  Unity visualization, animation, input affordances, and puzzle view state.

Guide Player
  Application-edge coordinator that reads guide content, resolves puzzle references,
  and calls presentation interfaces.
```

Dependency direction:

```text
Puzzle Presentation -> Puzzle Model
Guide Player -> Guide
Guide Player -> Puzzle Model
Guide Player -> Puzzle Presentation
Guide -> no Puzzle Model dependency
Guide -> no Puzzle Presentation dependency
Puzzle Model -> no Unity dependency
```

## Current Repository Map

| Area | Current files | Current role | Target role |
| --- | --- | --- | --- |
| Puzzle Model | `Packages/dev.sanuel.cubetorial.model/Runtime` | Unity-free model package with puzzle definitions, state, moves, and tests. | Keep as the puzzle model boundary. |
| Guide | `Assets/Cubetorial/Tutorials/Scripts` | ScriptableObject guide assets, guide nodes, guide actions, state setup, and markdown import/export. Currently references the puzzle model package for `PuzzleFamily`. | Keep authored guide data here, but replace direct model types with guide-owned puzzle references when migration is safe. |
| Guide Editor | `Assets/Cubetorial/Tutorials/Editor` | Import/export and external markdown editing tools for `TutorialGuide`. | Keep as authoring tooling, with names eventually moved from tutorial wording to guide wording. |
| Guide Tests | `Assets/Cubetorial/Tutorials/Tests` | Editor tests for guide markdown parsing and export. Currently assert model `PuzzleFamily` values. | Keep focused on guide serialization and authoring rules; update once guide-owned puzzle references replace model enum usage. |
| Puzzle Presentation | `Assets/Cubetorial/Presentation/Scripts/View` | Unity `PuzzleView`, `PuzzlePieceView`, and view definitions that bind model state and moves to transforms and animation data. | Keep as puzzle presentation. Add a narrow interface for guide-player calls before guide actions start driving it heavily. |
| Sandbox Prototype | `Assets/Cubetorial/Presentation/Scripts/Controllers/PuzzleController.cs` | Prototype keyboard camera/move controller with 3x3-specific keypad controls. | Replace with data-driven pointer input later; do not let it shape guide-player architecture. |
| Guide Player Prototype | `Assets/Cubetorial/Presentation/UI/Scripts/TutorialController.cs` | Renders guide title, sections, and blocks into UI. It does not yet execute guide actions or coordinate puzzle presentation. | Evolve into, or be replaced by, the guide player orchestration surface. |
| Navigation Prototype | `Assets/Cubetorial/Presentation/UI/Scripts/NavigationController.cs` and `NavigationGraph.cs` | Data-driven screen switching for prototype menus. | Keep as presentation UI unless app navigation needs a separate application layer. |
| Presentation Editor Tools | `Assets/Cubetorial/Presentation/Editor` | Puzzle view generators and editor inspectors. Some namespaces are generic prototype names. | Keep as editor tooling for puzzle presentation; clean namespaces as part of later code hygiene. |

## Current Shortcuts

- `Cubetorial.Tutorials` references `dev.sanuel.cubetorial.model` because `TutorialGuide` stores `PuzzleFamily`.
- Guide action classes are still named `TutorialMove`, `TutorialFocus`, `TutorialHighlight`, and `TutorialAnnotation`.
- `StickerSelection` stores `cubiePattern`, even though the glossary now prefers piece or slot language.
- Puzzle presentation code is in the default Unity assembly rather than a named presentation assembly.
- The guide player role is not yet complete; current UI code mostly renders guide content.

## Refactoring Direction

1. Introduce guide-owned puzzle reference types or strings for guide metadata and guide actions.
2. Move model-specific resolution into the guide player.
3. Give puzzle presentation a small API for guide-player operations, such as apply move, focus view, highlight stickers, clear annotations, and load state setup.
4. Rename tutorial-era guide classes only when Unity serialized asset migration is deliberate and verified.
5. Add a named presentation assembly once dependencies are stable enough for Unity asmdef boundaries to help rather than churn.

## Boundary Test

When adding guide or presentation code, ask:

- Could this guide asset be serialized without referencing model classes?
- Could this puzzle view be used by sandbox without referencing guide classes?
- Is this code translating guide intent into model or presentation calls? If yes, it belongs in the guide player.
- Is this code enforcing legal puzzle moves or state invariants? If yes, it belongs in the puzzle model.
- Is this code about transforms, materials, animation, camera, UI, or input affordances? If yes, it belongs in presentation.
