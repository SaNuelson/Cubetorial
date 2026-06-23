# Controls

Cubetorial controls are touch-first and must also work with a mouse on desktop and web builds.

The main interaction goal is to let users rotate the view and perform puzzle moves without relying on puzzle-specific buttons. Puzzle moves should be discoverable from the selected piece and the active puzzle definition.

## Input Priorities

When input begins, controls resolve intent in this order:

1. UI controls consume pointer input first.
2. Pressing a puzzle piece starts move selection.
3. Pressing outside the puzzle starts view rotation.

Puzzle input is ignored while a puzzle move animation is already running.

Tutorial playback may also lock direct puzzle input unless the active guide block explicitly enables an interactive exercise.

## View Rotation

### Phone

- Drag outside the puzzle to rotate the view.
- Rotation should begin after a small drag threshold to avoid accidental movement.
- Releasing the touch stops rotation.

### PC / Web

- Left drag outside the puzzle rotates the view.
- Right drag on non-UI space may also rotate the view as a desktop convenience.
- Arrow keys or WASD may be supported for debug, accessibility, or desktop convenience, but are not required for the core mobile interaction model.

## Puzzle Moves

Puzzle moves use a select-and-drag gesture.

1. Press on a puzzle piece.
2. Determine the current slot that was hit.
3. Find legal moves whose affected slots include that current slot.
4. Convert those moves into visible move arrows using the puzzle view definition.
5. Drag toward one arrow.
6. Highlight the closest valid arrow once the drag exceeds the selection threshold.
7. Release to perform the selected move.
8. Release without a clear selection cancels the gesture.

The gesture performs one quarter-turn or one puzzle-native move. Double turns such as `U2` are not required for direct manipulation in the MVP. Users can perform the same move twice.

## Move Discovery

Move discovery is data-driven:

- The core puzzle model defines legal moves and affected slots.
- The puzzle view definition defines visual move data such as pivot, axis, angle, and arc direction.
- The input controller combines both to show candidate arrows for the touched slot.

The selected slot must be the current occupied slot hit by raycast, not the piece's solved slot. After a scramble, a physical piece may no longer occupy its original slot.

## Gesture States

The controller should use explicit interaction states:

```text
Idle
ViewRotating
MoveChoosing
Animating
TutorialLocked
```

Expected transitions:

- `Idle` -> `ViewRotating` when dragging outside the puzzle.
- `Idle` -> `MoveChoosing` when pressing a puzzle piece.
- `MoveChoosing` -> `Animating` when releasing over a selected move arrow.
- `MoveChoosing` -> `Idle` when releasing without a selected move.
- `Animating` -> `Idle` after the move animation completes.
- `TutorialLocked` blocks sandbox-style direct manipulation until tutorial context allows it.

## Current Prototype Note

`Assets/Cubetorial/Presentation/Scripts/Controllers/PuzzleController.cs` is prototype-only.

Known limitations:

- It uses the legacy Unity `Input` API.
- It hard-codes 3x3 keypad controls.
- It can trigger repeatedly while keys are held.
- It does not represent the touch-first move-selection interaction described here.

Future controller work should replace this with a data-driven pointer gesture controller.
