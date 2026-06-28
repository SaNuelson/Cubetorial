# Cubetorial

Cubetorial is an educational app for learning twisty puzzles.

Its core concept is to make learning efficient through:
- visualizing guide blocks alongside authored explanations
- explaining the rationale behind the moves and visual annotations

Architecturally, core concepts are:
- separation of layers:
  - **Puzzle model layer**
    - Unity-free and compatible with a variety of puzzles - 3x3, 4x4, Skewb, Pyraminx, ...
  - **Visualization layer**
    - scriptable puzzle view wrapper for arbitrary puzzles
    - adaptable puzzle view controller
  - **Guide layer**
    - authored guide structure and actions independent of scene objects
    - supporting localization
    - easy to add new guides (guide editor?)

---

## Core Concepts (V1)

### Visual Guides

Guides are accompanied by a simulated puzzle.

As the user progresses through a guide, the puzzle updates to demonstrate the concepts being explained.

This includes not only applying rotations according to the guide, but also moving the camera and highlighting relevant parts of the puzzle.

---

### Guide-Driven Design

Guides are independent of puzzle implementations.

A guide interacts with the application through a set of actions, such as:

* Move
* Highlight
* Rotate View
* Focus
* Annotation

This allows guides to describe *what should happen* without needing to know *how it is rendered*.

---

### Scenario-Based Teaching

In cases when parts of a guide differ based on the puzzle state, the guide layer offers the option of a case-based section.

These cases allow guides to diverge from the current puzzle state and demonstrate a known case state.

---

### Puzzle-Agnostic Model

The puzzle model is designed to support different puzzle families.

Adding a new puzzle family then consists of 
- creating a definition for its model
- creating a scriptable view for its defined moves
- adding a 3D model and binding its pieces to the puzzle view

Current implementation seemingly supports any twisty puzzle families.

---

### Sandbox

A natural side effect of the architecture.

Once a puzzle can be modeled and visualized, it can also be explored freely.

Sandbox mode allows users to experiment with supported puzzles outside guides.

Sandbox uses an adaptable controller, which can exist because move discovery is data-driven: the puzzle model defines legal moves and affected slots, while the puzzle view definition supplies the visual pivots, axes, and arcs needed to present them.

---

## Future Directions (V2)

### Pattern Recognition

A toolkit for describing and searching puzzle states.

Examples:

```csharp
Match.AnyFace(cube, "*D*DDD*D*")
```

The exact syntax is still evolving, but the goal is to provide a reusable way to describe patterns, states, and conditions.

This system becomes the foundation for several later features.

---

### Solver Support

Individual puzzle families may provide solvers.

The long-term goal is for solvers to be built from the same building blocks used by guides:

* pattern recognition
* state queries
* move execution

This keeps solver implementations independent of visualization and UI.

---

### Interactive Guides

Guides may eventually ask the user to perform tasks themselves.

For example:

* demonstrate how to build a white cross
* let the user attempt it
* verify completion using pattern recognition
* continue once the objective is satisfied

This transforms guides from passive demonstrations into guided exercises.

---

## Architecture

The project is split into three layers.

### Puzzle Model

Handles puzzle logic.

Responsible for:

* puzzle state
* moves
* transformations
* serialization
* pattern recognition
* solver-facing APIs

The puzzle model should remain independent of Unity.

---

### Presentation

Handles visualization and user interaction.

Responsible for:

* rendering
* animation
* camera control
* input
* UI

The presentation layer consumes the puzzle model.

---

### Guide

Handles educational content.

Responsible for:

* guides
* sections
* blocks
* actions
* explanations

The guide layer drives the presentation layer while remaining independent of puzzle rendering details.

---

## Current Status

Implemented:

* 3x3 puzzle model
* 3x3 visualization
* Skewb model
* Skewb visualization
* Move execution
* Puzzle rotation

In Progress:

* Guide player UI
* Guide authoring format
* Guide action system
* Mobile-first lesson experience
