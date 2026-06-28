# Architecture Overview

Cubetorial is organized around three durable layers:

## Puzzle Model

The puzzle model is a standalone Unity-free package. It owns puzzle topology, pieces, slots, stickers, state, legal moves, validation, state queries, and serialization.

The model should use puzzle-native identifiers rather than Unity scene references. Presentation code may convert those identifiers into meshes, transforms, materials, arrows, or labels at the boundary.

## Presentation

The presentation layer renders puzzles and synchronizes animation, camera, input, UI, and visual annotations with puzzle model state. Puzzle presentation depends on the puzzle model, not on guide data.

MonoBehaviours should stay thin where practical. Durable puzzle rules and move semantics should live outside Unity-specific presentation objects.

## Guide

The guide layer holds authored guide content: guides, sections, blocks, state setup, and guide actions. Guides should describe what the learner should see and read without depending directly on scene objects.

Guide data should not know about puzzle presentation or the puzzle model implementation. It may contain symbolic puzzle references such as puzzle family ids, move notation, slot ids, piece ids, sticker selectors, and face ids, but it does not validate or execute them itself.

## Guide Player

The guide player is the application surface that coordinates guide content, puzzle model, and puzzle presentation. It reads the active guide block, resolves symbolic puzzle references against the selected puzzle model and puzzle view, then calls presentation interfaces to execute moves, highlights, annotations, and view focus changes.

The guide player may be implemented with MonoBehaviours because it owns Unity UI and scene coordination, but it should be treated as orchestration at the application edge rather than as part of the puzzle presentation model.

## Dependency Direction

The puzzle model must not depend on Unity APIs. Puzzle presentation depends on the puzzle model. Guide data should avoid direct dependencies on both puzzle model types and presentation types. The guide player composes all three into the Unity experience.

## Current Prototype Notes

Current guide assets still use the `TutorialGuide` name and the guide assembly currently references the puzzle model package for `PuzzleFamily`. That is a prototype shortcut, not the target boundary. The target is for guide data to store puzzle references in guide-owned types or strings, with the guide player responsible for resolving them.
