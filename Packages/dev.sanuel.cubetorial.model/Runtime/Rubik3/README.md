# Rubik3 Model Notes

This folder uses ideas from Herbert Kociemba's 3x3 cubie model. The goal is not to copy a solver implementation, but to use the same mental model for describing puzzle state and moves.

References:

- Kociemba cubie level: https://kociemba.org/math/cubielevel.htm
- Kociemba cube definitions: https://kociemba.org/math/CubeDefs.htm
- Singmaster notation overview: https://www.speedsolving.com/wiki/index.php/Singmaster_notation

## Core Idea

A 3x3 cube can be described by cubies, not by individual colored stickers alone.

There are:

- 8 corner cubies
- 12 edge cubies
- 6 center cubies

The centers define the fixed cube orientation in the usual 3x3 model. Corners and edges move between slots. Each movable cubie also has an orientation:

- corner orientation: `0`, `1`, or `2`
- edge orientation: `0` or `1`

So a cube state is essentially:

```text
for each slot:
  which cubie is here?
  how is that cubie oriented?
```

For example:

```text
slot URF contains cubie DFR with orientation 2
slot UF contains cubie BR with orientation 1
```

## Face Names

The usual face names are:

```text
U = Up
D = Down
F = Front
B = Back
R = Right
L = Left
```

These names are used both for slots and for stickers.

Examples:

```text
URF = the corner slot touching Up, Right, and Front
DFR = the corner cubie whose solved stickers are Down, Front, and Right
UF  = the edge slot touching Up and Front
BR  = the edge cubie whose solved stickers are Back and Right
```

## Cubie Naming

Kociemba names cubies by their sticker faces in a consistent order. The first sticker is the reference sticker.

For corners, the reference sticker is usually the `U` or `D` sticker. The remaining faces follow a fixed order around the cubie.

Examples:

```text
URF
UFL
ULB
UBR
DFR
DLF
DBL
DRB
```

For edges, the reference sticker is usually the `U` or `D` sticker when present. For middle-layer edges, `F` or `B` is used as the reference.

Examples:

```text
UR
UF
UL
UB
DR
DF
DL
DB
FR
FL
BL
BR
```

The exact order matters because orientation is defined relative to this convention.

## Moves as Transitions

A move is not just a notation letter like `R`. It is a transition from one cubie state to another.

For example, an `R` turn cycles the right-side corners:

```text
DFR -> URF -> UBR -> DRB -> DFR
```

It also cycles the right-side edges:

```text
DR -> FR -> UR -> BR -> DR
```

Those cycles describe where cubies move. They do not fully describe orientation. Orientation also depends on how the stickers rotate.

## Face Direction Transitions

To compute orientation, it helps to describe how face directions change during a move.

For an `R` turn, one common direction transition is:

```text
U -> B
B -> D
D -> F
F -> U
R -> R
L -> L
```

This says how sticker directions rotate in space when the right face turns. The `R` and `L` directions stay unchanged because the rotation axis runs through them.

The move still needs separate piece cycles for corners, edges, and any other affected piece groups. The face transition only describes how sticker directions rotate.

## Orientation

Orientation is stored as a small number, but that number only makes sense because of the cubie naming convention.

For corners:

```text
0 = reference sticker is oriented normally for the slot
1 = reference sticker is twisted one way
2 = reference sticker is twisted the other way
```

For edges:

```text
0 = edge is not flipped
1 = edge is flipped
```

When a move is applied, orientation is updated with modular arithmetic:

```text
new orientation = old orientation + move delta mod orientation count
```

For a corner, the orientation count is `3`.
For an edge, the orientation count is `2`.

The important distinction:

```text
move delta != final orientation
```

The move delta belongs to a slot transition, such as:

```text
DFR -> URF has corner orientation delta 2
URF -> UBR has corner orientation delta 1
```

If a cubie already has orientation `2`, and then moves through a transition with delta `1`, the final orientation is:

```text
(2 + 1) mod 3 = 0
```

## Why Not Store Stickers Only?

A sticker-only model can describe visible colors, but tutorials often need to talk about pieces:

```text
move this corner into this slot
insert this edge
keep this solved pair intact
```

That is easier if the core state knows which cubie is in which slot. Sticker placement can still be derived later for rendering.

## Generalizing Beyond 3x3

The high-level idea generalizes:

```text
state = pieces in slots + orientations
move = slot cycles + orientation changes
```

But Kociemba's exact cubie names, orientation rules, and move tables are specific to the 3x3 cube.

Other puzzles need their own conventions:

- 4x4 has wing edges and movable centers.
- Skewb rotates around corners and has different piece cycles.
- Pyraminx has tips and tetrahedral piece movement.
- Shapemods may need additional visual or geometric metadata.

So this folder can follow Kociemba closely for 3x3, while the shared model should remain flexible enough for other puzzle definitions.
