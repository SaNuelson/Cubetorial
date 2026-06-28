using Cubetorial.Model;
using Cubetorial.Tutorials.Scripts;
using NUnit.Framework;
using UnityEngine;

namespace Cubetorial.Tutorials.Tests
{
    public class TutorialGuideMarkdownTests
    {
        [Test]
        public void ImportInto_ParsesMetadataSectionsBlocksStateAndActions()
        {
            var guide = ScriptableObject.CreateInstance<TutorialGuide>();

            TutorialGuideMarkdown.ImportInto(guide, @"
---
guideId: beginner-3x3
family: Rubik3
---

# Beginner Method
Learn the basics.

## White Cross

### Insert Edge
Put the edge in place.

> # R U R'
> move F
> show UF:U
");

            Assert.That(guide.guideId, Is.EqualTo("beginner-3x3"));
            Assert.That(guide.family, Is.EqualTo(PuzzleFamily.Rubik3));
            Assert.That(guide.title, Is.EqualTo("Beginner Method"));
            Assert.That(guide.description, Is.EqualTo("Learn the basics."));

            Assert.That(guide.sections, Has.Count.EqualTo(1));
            var section = (GuideSection)guide.sections[0];
            Assert.That(section.title, Is.EqualTo("White Cross"));

            Assert.That(section.nodes, Has.Count.EqualTo(1));
            var block = (GuideBlock)section.nodes[0];
            Assert.That(block.title, Is.EqualTo("Insert Edge"));
            Assert.That(block.body, Is.EqualTo("Put the edge in place."));
            Assert.That(block.stateSetup.isEnabled, Is.True);
            Assert.That(block.stateSetup.mode, Is.EqualTo(PuzzleStateSetupMode.FromSolved));
            Assert.That(block.stateSetup.scrambleMoves, Is.EqualTo(new[] { "R", "U", "R'" }));

            Assert.That(block.actions, Has.Count.EqualTo(2));
            var move = (TutorialMove)block.actions[0];
            Assert.That(move.moveId, Is.EqualTo("F"));

            var highlight = (TutorialHighlight)block.actions[1];
            Assert.That(highlight.selections, Has.Length.EqualTo(1));
            Assert.That(highlight.selections[0].cubiePattern, Is.EqualTo("UF"));
            Assert.That(highlight.selections[0].faceIds, Is.EqualTo(new[] { "U" }));
        }
    }
}
