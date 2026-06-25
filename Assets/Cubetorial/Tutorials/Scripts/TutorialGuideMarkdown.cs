using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Cubetorial.Model;

namespace Cubetorial.Tutorials.Scripts
{
    public static class TutorialGuideMarkdown
    {
        private static readonly Lazy<Dictionary<string, Type>> ActionTypesByDirective = new(BuildActionRegistry);

        public static void ImportInto(TutorialGuide guide, string markdown)
        {
            if (guide == null)
                throw new ArgumentNullException(nameof(guide));

            var parsed = Parse(markdown);

            guide.titleKey = parsed.titleKey;
            guide.descriptionKey = parsed.descriptionKey;
            guide.family = parsed.family;
            guide.sections = parsed.sections;
        }

        public static string Export(TutorialGuide guide)
        {
            if (guide == null)
                throw new ArgumentNullException(nameof(guide));

            var builder = new StringBuilder();
            builder.AppendLine("---");
            builder.AppendLine($"titleKey: {guide.titleKey}");
            builder.AppendLine($"descriptionKey: {guide.descriptionKey}");
            builder.AppendLine($"family: {guide.family}");
            builder.AppendLine("---");
            builder.AppendLine();

            foreach (var section in guide.sections)
            {
                AppendNode(builder, section, 1);
            }

            return builder.ToString();
        }

        private static ParsedGuide Parse(string markdown)
        {
            var lines = SplitLines(markdown);
            var parsed = new ParsedGuide();
            var index = ParseFrontmatter(lines, parsed);
            var sectionStack = new Stack<(int Level, GuideSection Section)>();
            GuideBlock currentBlock = null;
            var paragraphLines = new List<string>();

            while (index < lines.Length)
            {
                var line = lines[index];
                var trimmed = line.Trim();

                if (string.IsNullOrEmpty(trimmed))
                {
                    FlushParagraph(currentBlock, paragraphLines);
                    index++;
                    continue;
                }

                if (TryParseHeading(trimmed, out var headingLevel, out var headingText))
                {
                    FlushParagraph(currentBlock, paragraphLines);

                    if (headingLevel == 1)
                    {
                        var section = new GuideSection { title = headingText };
                        parsed.sections.Add(section);
                        sectionStack.Clear();
                        sectionStack.Push((headingLevel, section));
                        currentBlock = null;
                    }
                    else
                    {
                        var parent = GetCurrentSection(parsed, sectionStack);
                        currentBlock = new GuideBlock { title = headingText };
                        parent.nodes.Add(currentBlock);
                    }

                    index++;
                    continue;
                }

                if (TryParseDirective(trimmed, out var directive))
                {
                    FlushParagraph(currentBlock, paragraphLines);
                    ApplyDirective(parsed, sectionStack, currentBlock, directive);
                    index++;
                    continue;
                }

                paragraphLines.Add(line.Trim());
                index++;
            }

            FlushParagraph(currentBlock, paragraphLines);
            return parsed;
        }

        private static void AppendNode(StringBuilder builder, GuideNode node, int headingLevel)
        {
            switch (node)
            {
                case GuideSection section:
                    builder.AppendLine($"{new string('#', headingLevel)} {section.title}");
                    AppendStateSetup(builder, section.stateSetup);
                    builder.AppendLine();

                    foreach (var child in section.nodes)
                    {
                        AppendNode(builder, child, headingLevel + 1);
                    }

                    break;
                case GuideBlock block:
                    builder.AppendLine($"{new string('#', headingLevel)} {block.title}");
                    AppendStateSetup(builder, block.stateSetup);

                    if (!string.IsNullOrWhiteSpace(block.body))
                    {
                        builder.AppendLine();
                        builder.AppendLine(block.body.Trim());
                    }

                    foreach (var action in block.actions)
                    {
                        var directive = GetDirective(action.GetType());
                        if (string.IsNullOrWhiteSpace(directive))
                            continue;

                        builder.AppendLine($"> {directive} {action.ToMarkdown()}".TrimEnd());
                    }

                    builder.AppendLine();
                    break;
            }
        }

        private static void AppendStateSetup(StringBuilder builder, StateSetup stateSetup)
        {
            if (stateSetup == null || !stateSetup.isEnabled)
                return;

            var moves = stateSetup.scrambleMoves == null
                ? ""
                : string.Join(" ", stateSetup.scrambleMoves.Where(move => !string.IsNullOrWhiteSpace(move)));

            var prefix = stateSetup.mode == PuzzleStateSetupMode.FromSolved ? "# " : "";
            builder.AppendLine($"> {prefix}{moves}".TrimEnd());
        }

        private static void ApplyDirective(
            ParsedGuide parsed,
            Stack<(int Level, GuideSection Section)> sectionStack,
            GuideBlock currentBlock,
            string directive)
        {
            if (string.IsNullOrWhiteSpace(directive))
                return;

            if (directive.StartsWith("#", StringComparison.Ordinal))
            {
                ApplyStateSetup(GetCurrentNode(parsed, sectionStack, currentBlock), PuzzleStateSetupMode.FromSolved, directive[1..]);
                return;
            }

            var firstSpace = directive.IndexOf(' ');
            var firstToken = firstSpace < 0 ? directive : directive[..firstSpace];
            var payload = firstSpace < 0 ? "" : directive[(firstSpace + 1)..];

            if (ActionTypesByDirective.Value.TryGetValue(firstToken, out var actionType))
            {
                if (currentBlock == null)
                    throw new FormatException($"Action directive '{firstToken}' must be placed after a guide block heading.");

                var action = (TutorialAction)Activator.CreateInstance(actionType);
                action.ParseMarkdown(payload);
                currentBlock.actions.Add(action);
                return;
            }

            ApplyStateSetup(GetCurrentNode(parsed, sectionStack, currentBlock), PuzzleStateSetupMode.FromPrevious, directive);
        }

        private static GuideNode GetCurrentNode(
            ParsedGuide parsed,
            Stack<(int Level, GuideSection Section)> sectionStack,
            GuideBlock currentBlock)
        {
            if (currentBlock != null)
                return currentBlock;

            return GetCurrentSection(parsed, sectionStack);
        }

        private static GuideSection GetCurrentSection(
            ParsedGuide parsed,
            Stack<(int Level, GuideSection Section)> sectionStack)
        {
            if (sectionStack.Count > 0)
                return sectionStack.Peek().Section;

            var section = new GuideSection { title = "Guide" };
            parsed.sections.Add(section);
            sectionStack.Push((1, section));
            return section;
        }

        private static void ApplyStateSetup(GuideNode node, PuzzleStateSetupMode mode, string movesText)
        {
            node.stateSetup = new StateSetup
            {
                isEnabled = true,
                mode = mode,
                scrambleMoves = SplitMoves(movesText)
            };
        }

        private static string[] SplitMoves(string movesText)
        {
            return string.IsNullOrWhiteSpace(movesText)
                ? Array.Empty<string>()
                : movesText.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void FlushParagraph(GuideBlock currentBlock, List<string> paragraphLines)
        {
            if (paragraphLines.Count == 0)
                return;

            if (currentBlock == null)
                throw new FormatException("Paragraph text must be placed after a guide block heading.");

            var paragraph = string.Join(Environment.NewLine, paragraphLines).Trim();
            currentBlock.body = string.IsNullOrWhiteSpace(currentBlock.body)
                ? paragraph
                : $"{currentBlock.body}{Environment.NewLine}{Environment.NewLine}{paragraph}";

            paragraphLines.Clear();
        }

        private static int ParseFrontmatter(string[] lines, ParsedGuide parsed)
        {
            if (lines.Length == 0 || lines[0].Trim() != "---")
                return 0;

            var index = 1;
            while (index < lines.Length && lines[index].Trim() != "---")
            {
                var line = lines[index];
                var colonIndex = line.IndexOf(':');
                if (colonIndex > 0)
                {
                    var key = line[..colonIndex].Trim();
                    var value = line[(colonIndex + 1)..].Trim();
                    ApplyFrontmatterValue(parsed, key, value);
                }

                index++;
            }

            return index < lines.Length ? index + 1 : index;
        }

        private static void ApplyFrontmatterValue(ParsedGuide parsed, string key, string value)
        {
            switch (key)
            {
                case "titleKey":
                    parsed.titleKey = value;
                    break;
                case "descriptionKey":
                    parsed.descriptionKey = value;
                    break;
                case "family":
                    if (Enum.TryParse(value, out PuzzleFamily family))
                        parsed.family = family;
                    break;
            }
        }

        private static bool TryParseHeading(string line, out int level, out string title)
        {
            level = 0;
            title = "";

            while (level < line.Length && line[level] == '#')
            {
                level++;
            }

            if (level == 0 || level >= line.Length || line[level] != ' ')
                return false;

            title = line[(level + 1)..].Trim();
            return title.Length > 0;
        }

        private static bool TryParseDirective(string line, out string directive)
        {
            directive = "";

            if (!line.StartsWith(">", StringComparison.Ordinal))
                return false;

            directive = line[1..].Trim();
            return true;
        }

        private static Dictionary<string, Type> BuildActionRegistry()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(GetTypes)
                .Where(type => !type.IsAbstract && typeof(TutorialAction).IsAssignableFrom(type))
                .Select(type => new
                {
                    Type = type,
                    Attribute = type.GetCustomAttribute<TutorialActionDirectiveAttribute>()
                })
                .Where(entry => entry.Attribute != null)
                .ToDictionary(entry => entry.Attribute.Directive, entry => entry.Type);
        }

        private static IEnumerable<Type> GetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                return exception.Types.Where(type => type != null);
            }
        }

        private static string GetDirective(Type actionType)
        {
            return actionType.GetCustomAttribute<TutorialActionDirectiveAttribute>()?.Directive;
        }

        private static string[] SplitLines(string text)
        {
            return (text ?? "").Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        }

        private sealed class ParsedGuide
        {
            public string titleKey;
            public string descriptionKey;
            public PuzzleFamily family;
            public List<GuideNode> sections = new();
        }
    }
}
