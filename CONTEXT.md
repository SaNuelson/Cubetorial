# Cubetorial

Cubetorial is an educational twisty-puzzle app whose domain language centers on authored guides, puzzle state, and visual explanation. This glossary defines product and puzzle terms, not implementation details.

## Language

**Guide**:
An authored learning path for a puzzle, such as a beginner method or a case catalogue. A guide is educational content, not solver output.
_Avoid_: Tutorial, lesson, walkthrough

**Guide Player**:
The primary product surface where a user reads a guide while the puzzle view animates and highlights the active explanation. It coordinates guide content and puzzle presentation without making either one own the other.
_Avoid_: Sandbox, tutorial screen

**Guide Section**:
A major division within a guide, such as a solving phase or a case family. Sections organize guide blocks and may establish an initial puzzle state.
_Avoid_: Chapter, module

**Guide Block**:
One coherent explanation unit within a guide, usually short text plus optional actions. A block may be explanatory only, visual only, or contain one or more moves.
_Avoid_: Step, slide, move

**Guide Action**:
An authored instruction executed while a guide block is active, such as applying a move, focusing the view, highlighting stickers, or showing annotation text. Actions use guide-owned symbolic references that the guide player resolves during playback.
_Avoid_: Command, animation, script

**State Setup**:
An optional starting state declared by a guide section or block. It may load from solved or continue from the previous state before applying setup moves.
_Avoid_: Scramble, reset

**Puzzle**:
A twisty-puzzle type with faces, pieces, slots, stickers, legal moves, and solved-state rules. The term is generic across 3x3, 4x4, Skewb, Pyraminx, shapemods, and future families.
_Avoid_: Cube

**Puzzle Family**:
A concrete supported kind of puzzle with its own topology and move set, such as Rubik3 or Skewb. The term names app-supported definitions, not broad mechanical categories.
_Avoid_: Puzzle type, cube type, cube

**Puzzle Reference**:
A symbolic name in guide content that points at a puzzle family, move, slot, piece, sticker, or face without depending on puzzle-model types. The guide player resolves puzzle references during playback.
_Avoid_: Model reference, scene reference

**Puzzle Model**:
The Unity-free puzzle layer that defines topology, state, legal moves, validation, queries, and serialization. It is the source of truth for puzzle state.
_Avoid_: Core, simulation, Unity model

**Puzzle State**:
A particular arrangement and orientation of pieces in slots for a puzzle. A state can be current, solved, authored, imported, or produced by applying moves.
_Avoid_: Scramble

**Move**:
A legal puzzle-native transformation identified by notation and defined as transfers between slots plus orientation changes.
_Avoid_: Rotation, turn, animation

**Piece**:
A physical puzzle part that can move between slots and carries one or more stickers.
_Avoid_: Cubie, cubelet

**Slot**:
A puzzle position that can be occupied by a piece. In the solved state a slot is occupied by its matching piece, but slot-based references target the position on the puzzle, not whichever physical piece currently occupies it.
_Avoid_: Position, cell

**Sticker**:
A visible facelet on a piece. Sticker selections may target current slots or physical pieces depending on guide intent.
_Avoid_: Facelet, color square

**Face**:
A named puzzle side or direction used by moves, stickers, and notation.
_Avoid_: Side

**Puzzle View**:
The Unity presentation of a puzzle model, including meshes, transforms, materials, move pivots, animation, highlighting, and input affordances.
_Avoid_: Model view, visual model

**Visual Annotation**:
A temporary visual explanation attached to the puzzle view, such as a highlight, arrow, marker, outline, ghost target, or text callout.
_Avoid_: Decoration, gizmo

**Sandbox**:
A secondary product surface for free puzzle exploration. Sandbox uses the same puzzle model and puzzle view APIs as guides, but it does not drive the architecture.
_Avoid_: Main mode
