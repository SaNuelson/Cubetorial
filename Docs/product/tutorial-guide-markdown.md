# Guide Markdown

Guides are currently stored as `TutorialGuide` assets. Markdown is an editing format for exporting a guide, making text and structure changes quickly, and importing it back into the asset.

> [!TODO]
> Rename `TutorialGuide` and related editor labels to `Guide` when serialized asset migration is safe.

## Metadata

A guide file may start with a metadata header:

```markdown
---
guideId: 3x3.beginner
family: Rubik3
---
```

`guideId` identifies the guide concept, such as `3x3.beginner`. Multiple guide assets may share a `guideId` when they are separate language implementations of the same concept.

`family` identifies the puzzle family used by the guide.

The guide title and description are not frontmatter metadata. They are authored markdown content directly after the header.

```markdown
# Beginner Method

Learn a practical first method for solving a 3x3 cube.
```

## Sections And Blocks

Use headings to structure the guide:

```markdown
## First layer

### White cross

Find an edge that belongs in the white cross.
```

The first `#` heading is the guide title. Text beneath it, up to the next heading, becomes the guide description.

`##` headings create top-level guide sections. Deeper headings create guide blocks under the current section. Paragraph text below a block heading becomes the block body.

## State Setup

Use quoted directive lines to set up puzzle state for the current section or block.

```markdown
> # R U R' U'
```

The leading `#` means load from solved before applying the setup moves.

```markdown
> R U R' U'
```

No prefix means apply the setup from the previous state.

## Actions

Action directives also use quoted lines.

```markdown
> move U
```

Applies a puzzle move.

```markdown
> face F
```

Focuses the view on a piece or face id. A second value may be added later for an up direction.

```markdown
> show DF~:F
```

Highlights selected stickers.

```markdown
> text DF~:F "These faces need to be solved next"
```

Adds annotation text to selected stickers.

## Sticker Selection

Sticker selections use `slot:faces`. By default, selectors target slots: the current position on the puzzle, regardless of which physical piece is there.

```markdown
URF:UR
```

Selects the `U` and `R` stickers in the exact `URF` slot.

```markdown
@URF:UR
```

The `@` prefix targets the physical `URF` piece wherever it currently is.

```markdown
URF
URF:
```

Selects the whole `URF` slot.

```markdown
@URF
@URF:
```

Selects the whole physical `URF` piece.

```markdown
:UR
```

Selects all `U` and `R` stickers by current slot position.

```markdown
@:UR
```

Selects all physical stickers whose home faces include `U` or `R`.

```markdown
UR~:U
```

The `~` means "match ids containing these face letters, allowing extra letters." For a 3x3, `UR~:U` matches the `U` sticker in slots such as `URF`, `UR`, and `URB`.

```markdown
@UR~:U
```

Targets the `U` stickers on physical pieces whose home ids contain both `U` and `R`.

```markdown
DF~:F
```

Selects the `F` sticker in every slot containing both `D` and `F`.

```markdown
@DF~:F
```

Selects the `F` sticker on every physical piece whose home id contains both `D` and `F`.

## Example

```markdown
---
guideId: 3x3.beginner
family: Rubik3
---

# Beginner Method

Learn a practical first method for solving a 3x3 cube.

## First layer orientation
> # R U R' U' F U F'

### Start from a fixed setup

This guide starts from a known setup so the guide player can show the same pieces every time.

> text DF~:F "These faces need to be solved next"

### Watch the setup move

The first move turns the top layer so the target edge can be lined up before insertion.

> face F
> move U
> show DF~:F
```
