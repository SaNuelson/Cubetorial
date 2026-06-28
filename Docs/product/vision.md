# Product Vision

Cubetorial aims to help people learn twisty puzzles by pairing authored explanations with synchronized puzzle animation. The intended product is not merely a puzzle simulator; its main job is to make each solving idea visible, paced, and understandable while the learner may also be holding a physical puzzle.

## MVP Goal

The MVP aims to be a mobile-first guide player.

The guide player should let a user choose an authored guide, read through guide blocks, and see the puzzle view respond with state setup, moves, highlights, annotations, and view changes. The first success condition is that a beginner can follow a short guide because the app aims to show what the text is talking about at the same time the text explains why it matters.

## Product Priorities

1. Guide player experience.
2. Puzzle visualization and animation.
3. Unity-free puzzle model.
4. Sandbox interaction.
5. Rule-based interactive guides.
6. Solver and image-recognition features.

## Experience Principles

- The app should aim to open quickly on a phone and be useful while the user is physically solving a puzzle.
- The active guide block should drive the puzzle view.
- Guide blocks are not moves. A block can be explanation-only, visual-only, one move, or several actions.
- Playback controls should support precise movement through the active block without making the main experience feel like an editor.
- Sandbox is valuable, but it should reuse guide-era puzzle state and move APIs rather than shape them.

## Scope Boundaries

The architecture should support multiple puzzle families over time, including 3x3, 4x4, Skewb, Pyraminx, and shapemods. It should not become generic for its own sake before a second real puzzle family forces the abstraction.

Solver support, image recognition, arbitrary-scramble guide generation, and rule-based progress checks are future capabilities. The guide data should stay structured enough to grow toward them, but the MVP should remain focused on authored guides.
