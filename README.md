# Cubetorial

A Rubik's Cube tutorial app.

## Goals

- Extensibility for arbitrary cubes.
- Extensibility for different algorithms / solutions.
- Aesthetics.

## Structure

Abstract superclass extended for each cube type.

- Cube 
  - (T): TAxis
  - Rotate(TAxis axis)
- Cubelet

### 3x3x3

- Cube3Axis : TAxis
  - Left, Right, Up, Down, Front, Back
  - ReverseLeft, ...
  - MidLeft, ...

- Cube333 : Cube
  - (T): TAxis = Cube3Axis
- Cubelet333

### 5x5x5

- Cube5Axis : TAxis

## Tasklog

- [x] 3x3x3 cube.
- [x] Rotation.
- [ ] Controls
  - [ ] Cubelet-based
    - hover cubelet, move in direction
  - 