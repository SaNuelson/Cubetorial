# Cubetorial

Cubetorial is an educational app for learning twisty puzzles.

It's core concept is to make learning efficient through:
- visualizing the tutorial steps alongside the explanation
- explaining rationale behind the taken steps

Architecturally, core concepts are:
- separation of layers:
  - **Core layer** 
    - compatible with variety of puzzles - 3x3, 4x4, Skewb, Pyraminx, ...
  - **Visualization layer**
    - scriptable model view wrapper for arbitrary puzzle
    - adaptible puzzle view controller
  - **Tutorial layer**
    - independent of other layers (uses visualization API)
    - supporting localization
    - easy to add new tutorials (Tutorial editor?)

---

## Core Concepts (V1)

### Visual Tutorials

Tutorials are accompanied by a simulated puzzle.

As the user progresses through a guide, the puzzle updates to demonstrate the concepts being explained.

This includes not only applying rotations according to the guide, but also moving the camera and highlighting relevant parts of the puzzle.

---

### Tutorial-Driven Design

Tutorials are independent of puzzle implementations.

A tutorial interacts with the application through a set of actions, such as:

* Move
* Highlight
* Rotate View
* Focus
* Annotation

This allows tutorials to describe *what should happen* without needing to know *how it is rendered*.

---

### Scenario-Based Teaching

In cases when parts of a tutorial differ based on the cube configuration, tutorial layer offers the option of a case-based section.

These cases allow tutorials to diverge from a current scramble and show solution on a different one.

---

### Puzzle-Agnostic Core

The underlying model is designed to support different puzzle families.

Adding a new puzzle family then consists of 
- creating a builder for its model
- creating a scriptable view for its defined moves
- adding a 3D model and binding its core and pieces to the view

Current implementation seemingly supports any twisty puzzle families.

---

### Sandbox

A natural side effect of the architecture.

Once a puzzle can be simulated and visualized, it can also be explored freely.

Sandbox mode allows users to experiment with supported puzzles outside tutorials.

Sandbox uses an adaptible controller, which can exist thanks to the robust nature of puzzle definition - scriptable object for moves contains all necessary information to dynamically generate a set of movements, while the model gives ability to check for specific states of the puzzle i.e., if it is solved.

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

The long-term goal is for solvers to be built from the same building blocks used by tutorials:

* pattern recognition
* state queries
* move execution

This keeps solver implementations independent of visualization and UI.

---

### Interactive Tutorials

Tutorials may eventually ask the user to perform tasks themselves.

For example:

* demonstrate how to build a white cross
* let the user attempt it
* verify completion using pattern recognition
* continue once the objective is satisfied

This transforms tutorials from passive demonstrations into guided exercises.

---

## Architecture

The project is split into three layers.

### Core

Handles puzzle logic.

Responsible for:

* puzzle state
* moves
* transformations
* serialization
* pattern recognition
* solver-facing APIs

The core should remain independent of Unity.

---

### Presentation

Handles visualization and user interaction.

Responsible for:

* rendering
* animation
* camera control
* input
* UI

The presentation layer consumes the core model.

---

### Tutorials

Handles educational content.

Responsible for:

* guides
* sections
* blocks
* actions
* explanations

The tutorial layer drives the presentation layer while remaining independent of puzzle rendering details.

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
* Tutorial authoring format
* Guide action system
* Mobile-first lesson experience
