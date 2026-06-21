# AGENTS.md

Guidance for AI agents working in this repository.

## Project Intent

Cubetorial is a Unity tutorial app for learning twisty puzzles. The core product is not merely rendering or simulating a puzzle; it is helping a user understand how a puzzle can be solved through clear visual guidance, authored explanations, and synchronized puzzle animation.

The primary product surface is the tutorial/guide player.

The MVP is:

* Visualize twisty puzzles clearly.
* Represent puzzle state and legal moves in an extensible, Unity-free core model.
* Let Unity wrap that model for visualization, animation, input, and platform-specific presentation.
* Support authored guides made of explanatory blocks, puzzle states, move sequences, and visual annotations.
* Use highlights, overlays, arrows, camera/view changes, and guided movement to explain what each block is doing.
* Keep the architecture open to multiple puzzle families without overbuilding abstractions before they are needed.

The intended scope goes beyond a 3x3 Rubik's Cube. The architecture should be able to grow toward 3x3, 4x4, Skewb, Pyraminx, shapemods, and other twisty puzzles.

Important: much of the current code is prototype code. Do not assume existing structure, names, or behavior represent the final architecture. Use it as context for what has been tried, not as a binding design.

## Product Priorities

Primary target: phones. The app should feel quick to open while someone is physically working with a puzzle. Favor clean, sleek, readable mobile UI over dense desktop tooling.

Secondary target: web. Web layouts may take advantage of larger resolutions, but mobile ergonomics come first.

Product priority order:

1. Tutorial/guide experience.
2. Puzzle visualization and animation.
3. Clean puzzle state/move model.
4. Sandbox interaction.
5. Rule-based interactive tutorials.
6. Solver and image recognition features.

Nice-to-have features after the MVP:

* Direct puzzle interaction, likely tap-slide-release on puzzle pieces, with contextual arrows for possible rotations.
* View rotation by tapping/sliding outside the puzzle.
* Rule-based tutorial progress checking, where guide blocks can detect whether a user-created state satisfies requirements such as "bottom layer solved".
* Image recognition to import a scramble from a photo.
* Solver support for finding a solution from a random, manual, or image-imported scramble.
* Dynamically generated tutorial paths for arbitrary scrambles.

## Working Principles

* Prefer simple, explicit C# over clever abstractions.
* Keep puzzle/domain logic testable outside MonoBehaviours wherever practical.
* Separate concerns:
  * puzzle model/state, pieces, stickers, legal moves, and invariants
  * visual puzzle representation
  * rotation/animation presentation
  * input/controller behavior
  * tutorial guide data, blocks, actions, checks, and explanation text
* When touching prototype areas, improve them only as far as needed for the task. Avoid broad rewrites unless explicitly requested.
* Preserve Unity `.meta` files for assets and scripts. If adding, moving, or deleting assets, account for the matching `.meta` files.
* Avoid editing third-party plugin folders under `Assets/Plugins/` unless the task is specifically about those assets.

## Architecture Direction

Favor a layered design:

* Core puzzle layer: plain C# model for puzzle topology, state, pieces, stickers, moves, move application, validation, state queries, and serialization.
* Tutorial layer: authored guide definitions that reference puzzle states, move sequences, visual annotations, and explanatory content without depending directly on Unity scene objects.
* Unity presentation layer: MonoBehaviours, prefabs, materials, animation, input, camera, and UI that consume the core puzzle and tutorial APIs.

The core puzzle layer should not depend on Unity APIs. This keeps it portable, testable, and useful for future solver, image-recognition, and web-facing logic.

Avoid putting Unity vectors, Transforms, GameObjects, MonoBehaviours, Materials, or Scene references in the core. Prefer puzzle-native identifiers and topology data such as piece ids, sticker ids, face ids, move ids, permutations, orientations, and adjacency. If geometric data is truly needed, isolate it behind core-owned value types so Unity-specific conversions stay at the presentation boundary.

Prefer generic puzzle terminology in core code. Use "puzzle", "piece", "sticker", "move", "axis", "face", "state", and "guide block" where appropriate. Reserve "cube" for cube-specific code.

Avoid over-genericizing early. A useful abstraction should remove duplication or enable a real second puzzle family, not merely anticipate possible future puzzles.

## Tutorial / Guide Direction

Cubetorial guides should be treated as authored educational timelines, not as generic solver output.

Preferred conceptual structure:

* `Guide`

  * represents a full tutorial, such as "3x3 Beginner Method" or "Skewb Beginner Method"
* `GuideSection`

  * represents a major logical section, such as "White Cross", "White Corners", or "Last Layer"
  * sections may contain nested subsections for case catalogues
* `GuideBlock`

  * represents one coherent explanation unit, usually 1-3 sentences
  * a block may or may not contain puzzle moves
* `GuideAction`

  * represents something the visualizer should do while the block is active

Guide blocks may include:

* Human-readable explanation text.
* Optional initial puzzle state.
* One or more puzzle moves to animate.
* Camera/view hints.
* Temporary visual annotations such as sticker markers, face highlights, slot outlines, arrows, ghost targets, or other gizmos.

A guide should support both sequential learning and case catalogues.

Sequential example:

* White Cross

  * explain the goal
  * find an edge
  * insert the edge
  * repeat conceptually

Case catalogue example:

* Last Layer

  * Sune case

    * load Sune initial state
    * explain recognition
    * animate algorithm
  * Anti-Sune case

    * load Anti-Sune initial state
    * explain recognition
    * animate algorithm

State ownership rule:

* A guide section or block may optionally declare an initial puzzle state.
* If a node declares an initial state, entering it loads that state.
* If no initial state is declared, it inherits the current state from the parent/previous block.
* This allows both continuous tutorials and independent case demonstrations.

Do not assume every block is one move. Blocks may be purely explanatory, visual-only, or multi-action.

Rule-based completion checks, arbitrary-scramble adaptation, and solver-generated solutions are future features. Keep guide data structured enough that those systems can be added later, but do not implement them for the initial MVP unless explicitly requested.

## UI Direction

The first product surface should be a lesson player, not a generic menu-heavy puzzle sandbox.

For mobile, a reasonable starting layout is:

* Puzzle visualization in the upper portion of the screen.
* Playback controls between puzzle and text.
* Tutorial content in the lower portion.
* The active block drives the puzzle animation and visual annotations.
* Tutorial controls should be reachable with one hand where practical.
* Future direct puzzle interaction can let the tutorial panel collapse or slide down to give the puzzle more space.

The guide player should support scroll-driven activation:

* The user scrolls through guide blocks.
* The block closest to the active reading position becomes active.
* When a block becomes active, its actions are executed.
* The cube animation is not directly scrubbed by scroll percentage unless specifically implemented later.
* Playback controls allow precision control within the active block.

Suggested playback controls:

* Previous block
* Previous action/move
* Play/Pause or Replay
* Next action/move
* Next block

Navigation should be minimal:

* Home

  * Guides
  * Sandbox
  * Solver, disabled/coming later
* Guides

  * grouped by puzzle
  * each puzzle contains methods/guides
* Guide Player

  * focused reading/visualization screen

Inside a guide, sections should be part of one continuous scroll where practical. Use sticky section headers and an optional section navigator/bottom sheet instead of permanently visible side navigation on mobile.

Avoid investing heavily in complex navigation before the lesson experience works. Start with a small guide picker, then enter a focused tutorial view.

## Sandbox Direction

Sandbox is secondary. It is useful once puzzle state, legal moves, and visualization already exist, but it should not drive the architecture.

Sandbox may eventually provide:

* puzzle selection
* direct manipulation
* scramble/reset
* undo/redo
* free camera/view rotation

Do not add sandbox-specific assumptions to the core model. It should use the same move/state APIs as tutorials and future solvers.

## Code Style

* Use C# conventions common in Unity projects: PascalCase for types/methods/properties, camelCase for locals/parameters.
* Keep MonoBehaviours thin. Put durable puzzle rules, tutorial data, and algorithm logic in plain C# classes where possible.
* Prefer `[SerializeField] private` fields over public mutable fields for Unity inspector wiring.
* Avoid hard-coding 3x3 assumptions in new core logic unless the task is explicitly 3x3-specific.
* Add comments only when they clarify non-obvious puzzle math, coordinate transforms, rule checks, data ownership, or Unity lifecycle behavior.
* Prefer clear names over terse mathematical notation in production code. Dense notation is acceptable only in well-isolated move tables or tests.

## Testing and Validation

* If changing pure puzzle logic, add or update Edit Mode tests when feasible.
* If changing guide action sequencing, test that actions execute deterministically and can be replayed.
* If changing scene/prefab behavior, validate in Unity when available and describe any manual verification needed.
* Before finishing code changes, at minimum check for compile-level issues when practical.
* Do not rely on generated IDE project files as source of truth; Unity regenerates them.

Useful puzzle tests include:

* solved state remains valid
* applying a move and its inverse restores the state
* applying the same quarter turn four times restores the state
* serialized state round-trips correctly
* guide block action sequences produce expected states
* puzzle-specific invariants are preserved

## Unity Asset Safety

* Scenes, prefabs, materials, render pipeline assets, and input actions are serialized Unity files. Make focused edits and avoid unrelated churn.
* Be careful with automatic reserialization. If Unity rewrites a large asset, verify the diff is intentional.
* Keep assets organized under `Assets/` by feature or asset type, following existing folders unless creating a clearer feature boundary.
* Do not rename or move serialized assets casually; Unity references may break if `.meta` files are mishandled.

## Dependency Notes

Installed packages include URP, Input System, Unity Test Framework, Timeline, UI Toolkit/uGUI, DOTween, and Odin/Sirenix assets. Reuse existing dependencies when they fit; do not add new packages without a clear need.

Prefer built-in Unity UI Toolkit or uGUI for app UI unless there is a clear reason to introduce something else.

Use DOTween where it simplifies presentation animation, but keep puzzle state transitions independent of animation timing.

## Communication

When reporting work:

* Call out whether changes are prototype-compatible or intended as foundation for the final direction.
* Mention tests or validation performed.
* If Unity editor validation was not run, say so directly.
* If a change introduces a temporary 3x3-specific shortcut, mark it clearly.
* If architecture was narrowed intentionally for MVP scope, explain why.
