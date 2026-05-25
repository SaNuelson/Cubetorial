# AGENTS.md

Guidance for AI agents working in this repository.

## Project Intent

Cubetorial is a Unity tutorial app for learning twisty puzzles. The core product is not just rendering a puzzle; it is helping a user understand how a scrambled puzzle can be solved through clear visual guidance.

The MVP is:

- Visualize twisty puzzles clearly.
- Represent puzzle state and legal moves in an extensible, Unity-free core model.
- Let Unity wrap that model for visualization, animation, input, and platform-specific presentation.
- Support authored tutorials as step-by-step guides with fixed scrambles and fixed solving steps.
- Use visual annotations, highlights, overlays, and guided movement to explain what each step is doing.

The intended scope goes beyond a 3x3 Rubik's Cube. The architecture should be able to grow toward 3x3, 4x4, Skewb, Pyraminx, shapemods, and other twisty puzzles.

The MVP should prove extensibility with more than one puzzle family. Avoid building a 3x3-specific foundation and promising to generalize it later.

Important: much of the current code is prototype code. Do not assume existing structure, names, or behavior represent the final architecture. Use it as context for what has been tried, not as a binding design.

## Product Priorities

Primary target: phones. The app should feel quick to open while someone is physically working on a puzzle. Favor clean, sleek, readable mobile UI over dense desktop tooling.

Secondary target: web. Web layouts may take advantage of larger resolutions, but mobile ergonomics come first.

Nice-to-have features after the MVP:

- Direct puzzle interaction, likely tap-slide-release on puzzle pieces, with contextual arrows for possible rotations.
- View rotation by tapping/sliding outside the puzzle.
- Rule-based tutorial progress checking, where tutorial steps can detect whether a user-created state satisfies requirements such as "bottom layer solved".
- Image recognition to import a scramble from a photo.
- Solver support for finding a solution from a random, manual, or image-imported scramble.
- Dynamically generated tutorial paths for arbitrary scrambles.

Do not let nice-to-have features distort the MVP architecture. Keep interfaces open for them, but prioritize puzzle models, visualization, and tutorials first.

## Environment

- Unity version: `6000.3.7f1` from `ProjectSettings/ProjectVersion.txt`.
- Main project folders:
  - `Assets/` - Unity assets, scenes, prefabs, scripts, settings.
  - `Packages/` - Unity package manifest and lock data.
  - `ProjectSettings/` - project-wide Unity settings.
- Generated/local folders such as `Library/`, `Temp/`, `Logs/`, `obj/`, `UserSettings/`, `.vs/`, and generated `.csproj`/`.sln` files should not be edited or committed.

## Working Principles

- Prefer simple, explicit C# over clever abstractions.
- Keep puzzle/domain logic testable outside MonoBehaviours wherever practical.
- Separate concerns:
  - puzzle model/state, pieces, stickers, legal moves, and invariants
  - rotation/animation presentation
  - input/controller behavior
  - tutorial steps, move sequences, checks, and hint text
- When touching prototype areas, improve them only as far as needed for the task. Avoid broad rewrites unless explicitly requested.
- Preserve Unity `.meta` files for assets and scripts. If adding, moving, or deleting assets, account for the matching `.meta` files.
- Avoid editing third-party plugin folders under `Assets/Plugins/` unless the task is specifically about those assets.

## Architecture Direction

Favor a layered design:

- Core puzzle layer: plain C# model for puzzle topology, state, moves, move application, validation, fixed scrambles, and state queries.
- Tutorial layer: authored step-by-step guide definitions that reference puzzle states, move sequences, visual annotations, and explanatory content without depending directly on Unity scene objects.
- Unity presentation layer: MonoBehaviours, prefabs, materials, animation, input, camera, and UI that consume the core puzzle and tutorial APIs.

The core puzzle layer should not depend on Unity APIs. This keeps it portable, testable, and useful for future solver, image-recognition, and web-facing logic.

Avoid putting Unity vectors in the core. Prefer puzzle-native identifiers and topology data such as piece ids, sticker ids, face ids, move ids, permutations, orientations, and adjacency. If geometric data is truly needed, isolate it behind core-owned value types or data structures so Unity-specific conversions stay at the presentation boundary.

Prefer generic puzzle terminology in core code. Use "puzzle", "piece", "sticker", "move", "axis", "face", "state", and "tutorial step" where appropriate. Reserve "cube" for cube-specific code.

## Tutorial Direction

Start with hardcoded authored tutorials. Web-style fixed explanations are acceptable for the first version as long as the app adds value through synchronized visualization.

Tutorial steps may include:

- Human-readable explanation text.
- A fixed starting scramble or required tutorial state.
- One or more puzzle moves to animate.
- Camera/view hints.
- Temporary visual annotations such as sticker markers, face highlights, slot outlines, arrows, ghost targets, or other gizmos.

Rule-based completion checks, arbitrary-scramble adaptation, and solver-generated solutions are future features. Keep tutorial data structured enough that those systems can be added later, but do not implement them for the initial MVP unless explicitly requested.

## UI Direction

The first product surface should be a lesson player, not a generic menu-heavy puzzle sandbox.

For mobile, a reasonable starting layout is:

- Puzzle visualization in the upper portion of the screen.
- Tutorial content and step controls in the lower portion.
- The active phrase or step drives the cube animation and visual annotations.
- Tutorial controls should be reachable with one hand where practical.
- Future direct puzzle interaction can let the tutorial panel collapse or slide down to give the puzzle more space.

Avoid investing heavily in complex navigation before the lesson experience works. Start with a small tutorial picker, then enter a focused tutorial view.

## Code Style

- Use C# conventions already common in Unity projects: PascalCase for types/methods/properties, camelCase for locals/parameters.
- Keep MonoBehaviours thin. Put durable puzzle rules, tutorial data, and algorithm logic in plain C# classes where possible.
- Prefer serialized fields over public mutable fields for Unity inspector wiring.
- Avoid hard-coding 3x3 assumptions in new core logic unless the task is explicitly 3x3-specific.
- Add comments only when they clarify non-obvious puzzle math, coordinate transforms, rule checks, or Unity lifecycle behavior.

## Testing and Validation

- If changing pure puzzle logic, add or update Edit Mode tests when feasible.
- If changing scene/prefab behavior, validate in Unity when available and describe any manual verification needed.
- Before finishing code changes, at minimum check for compile-level issues when practical.
- Do not rely on generated IDE project files as source of truth; Unity regenerates them.

## Unity Asset Safety

- Scenes, prefabs, materials, render pipeline assets, and input actions are serialized Unity files. Make focused edits and avoid unrelated churn.
- Be careful with automatic reserialization. If Unity rewrites a large asset, verify the diff is intentional.
- Keep assets organized under `Assets/` by feature or asset type, following existing folders unless creating a clearer feature boundary.

## Dependency Notes

Installed packages include URP, Input System, Unity Test Framework, Timeline, UI Toolkit/uGUI, DOTween, and Odin/Sirenix assets. Reuse existing dependencies when they fit; do not add new packages without a clear need.

## Communication

When reporting work:

- Call out whether changes are prototype-compatible or intended as foundation for the final direction.
- Mention tests or validation performed.
- If Unity editor validation was not run, say so directly.
