# Cubetorial Docs

Cubetorial is an educational Unity app for learning twisty puzzles through clear visualization, authored explanations, and synchronized puzzle animation.

These docs are the source of truth for product behavior, architecture direction, and prototype notes. The root `CONTEXT.md` is the source of truth for domain vocabulary.

## Current Focus

- Mobile-first guide player as the first product surface.
- Self-contained Unity-free puzzle model for puzzle state, moves, and validation.
- Reusable puzzle visualization and animation.
- Authored guides with structured guide blocks and visual actions.

## Future Plans

Future plans include but are not limited to (in the order of priority):

- Web-based deployment.
- Pattern-matching toolkit
  - Vital for expanding the app's capabilities from fixed content.
- Interactive guides
  - Extension of the guide layer to support user interaction during guides.
- Support for solver implementations
  - Most likely based on the toolkit.
  - Similarly to the model, Unity-free and dependent solely on the model package.

## Documentation Areas

- Product specs describe user-facing behavior, guide-player goals, and interaction rules.
- Architecture notes describe durable code boundaries and system design.
- Prototype notes describe temporary implementation flow and known shortcuts.
