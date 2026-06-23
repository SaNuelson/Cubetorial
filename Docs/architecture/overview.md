# Architecture Overview

Cubetorial is organized around three layers:

- Core puzzle model.
- Unity presentation.
- Tutorial guide data and playback.

The core puzzle model should remain independent of Unity APIs. It owns puzzle topology, pieces, slots, state, legal moves, move application, validation, and serialization-facing concepts.

The Unity presentation layer owns scene objects, prefabs, materials, animation, input, camera behavior, and UI. It consumes core puzzle data and maps it to visible puzzle pieces and move animations.

The tutorial layer owns authored educational structure: guides, sections, blocks, actions, explanation text, optional initial states, and visual annotations. Tutorials should describe what should happen without depending directly on scene object references.

## Direction

The first product surface is the guide player. Sandbox is useful, but secondary. Both guide playback and sandbox interaction should instantiate puzzle views from shared puzzle catalog data instead of relying on one scene per puzzle.

Recommended runtime shape:

```text
Home / Menu
Guide Player
Sandbox
```

The selected guide or puzzle family determines which puzzle view prefab, view definition, and model are instantiated into the reusable host scene.
