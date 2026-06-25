using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cubetorial.Tutorials.Scripts
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TutorialActionDirectiveAttribute : Attribute
    {
        public TutorialActionDirectiveAttribute(string directive)
        {
            Directive = directive;
        }

        public string Directive { get; }
    }

    [Serializable]
    public enum StickerSelectionTarget
    {
        Slot,
        Piece
    }

    [Serializable]
    public class StickerSelection
    {
        public StickerSelectionTarget target;
        public string cubiePattern;
        public bool allowExtraFaces;
        public string[] faceIds = Array.Empty<string>();

        public bool TargetsAllFaces => faceIds == null || faceIds.Length == 0;

        public static StickerSelection Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new FormatException("Sticker selection cannot be empty.");

            var trimmed = text.Trim();
            var target = StickerSelectionTarget.Slot;

            if (trimmed.StartsWith("@", StringComparison.Ordinal))
            {
                target = StickerSelectionTarget.Piece;
                trimmed = trimmed[1..];
            }

            var colonIndex = trimmed.IndexOf(':');
            var cubiePart = colonIndex >= 0 ? trimmed[..colonIndex] : trimmed;
            var facePart = colonIndex >= 0 ? trimmed[(colonIndex + 1)..] : "";
            var allowExtraFaces = cubiePart.EndsWith("~", StringComparison.Ordinal);

            if (allowExtraFaces)
                cubiePart = cubiePart[..^1];

            return new StickerSelection
            {
                target = target,
                cubiePattern = cubiePart,
                allowExtraFaces = allowExtraFaces,
                faceIds = ParseFaceIds(facePart)
            };
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            if (target == StickerSelectionTarget.Piece)
                builder.Append('@');

            builder.Append(cubiePattern);

            if (allowExtraFaces)
                builder.Append('~');

            if (TargetsAllFaces)
            {
                if (string.IsNullOrEmpty(cubiePattern))
                    builder.Append(':');

                return builder.ToString();
            }

            builder.Append(':');
            builder.Append(string.Concat(faceIds));
            return builder.ToString();
        }

        private static string[] ParseFaceIds(string facePart)
        {
            if (string.IsNullOrWhiteSpace(facePart))
                return Array.Empty<string>();

            return facePart
                .Where(c => !char.IsWhiteSpace(c) && c != ',')
                .Select(c => c.ToString())
                .ToArray();
        }
    }

    [Serializable]
    public abstract class TutorialAction
    {
        public abstract void ParseMarkdown(string payload);

        public abstract string ToMarkdown();
    }

    [TutorialActionDirective("move")]
    [Serializable]
    public class TutorialMove : TutorialAction
    {
        public string moveId;

        public override void ParseMarkdown(string payload)
        {
            moveId = RequireSingleValue(payload, "move");
        }

        public override string ToMarkdown()
        {
            return moveId;
        }

        private static string RequireSingleValue(string payload, string directive)
        {
            var value = payload?.Trim();
            if (string.IsNullOrEmpty(value))
                throw new FormatException($"Directive '{directive}' needs a value.");

            return value;
        }
    }

    [TutorialActionDirective("face")]
    [Serializable]
    public class TutorialFocus : TutorialAction
    {
        [FormerlySerializedAs("focusedCubeId")]
        public string pieceId;

        public string upId;

        public override void ParseMarkdown(string payload)
        {
            var parts = TutorialActionMarkdown.SplitValues(payload);
            if (parts.Length == 0)
                throw new FormatException("Directive 'face' needs a piece or face id.");

            pieceId = parts[0];
            upId = parts.Length > 1 ? parts[1] : "";
        }

        public override string ToMarkdown()
        {
            return string.IsNullOrWhiteSpace(upId)
                ? pieceId
                : $"{pieceId} {upId}";
        }
    }

    [TutorialActionDirective("show")]
    [Serializable]
    public class TutorialHighlight : TutorialAction
    {
        public StickerSelection[] selections = Array.Empty<StickerSelection>();

        [HideInInspector]
        [FormerlySerializedAs("highlightedCubeIds")]
        public string[] legacyHighlightedCubeIds = Array.Empty<string>();

        public override void ParseMarkdown(string payload)
        {
            selections = TutorialActionMarkdown.SplitValues(payload)
                .Select(StickerSelection.Parse)
                .ToArray();

            if (selections.Length == 0)
                throw new FormatException("Directive 'show' needs at least one sticker selection.");
        }

        public override string ToMarkdown()
        {
            var exportedSelections = selections is { Length: > 0 }
                ? selections
                : (legacyHighlightedCubeIds ?? Array.Empty<string>()).Select(StickerSelection.Parse).ToArray();

            return string.Join(" ", exportedSelections.Select(selection => selection.ToString()));
        }
    }

    [TutorialActionDirective("text")]
    [Serializable]
    public class TutorialAnnotation : TutorialAction
    {
        public StickerSelection selection;

        [HideInInspector]
        [FormerlySerializedAs("annotatedCubeId")]
        public string legacyAnnotatedCubeId;

        public string text;

        public override void ParseMarkdown(string payload)
        {
            var trimmed = payload?.Trim();
            if (string.IsNullOrEmpty(trimmed))
                throw new FormatException("Directive 'text' needs a sticker selection and text.");

            var firstSpace = trimmed.IndexOf(' ');
            if (firstSpace < 0)
                throw new FormatException("Directive 'text' needs quoted text after the sticker selection.");

            selection = StickerSelection.Parse(trimmed[..firstSpace]);
            text = ParseQuotedText(trimmed[(firstSpace + 1)..].Trim());
        }

        public override string ToMarkdown()
        {
            var exportedSelection = selection ?? StickerSelection.Parse(legacyAnnotatedCubeId ?? ":");
            return $"{exportedSelection} {Quote(text)}";
        }

        private static string ParseQuotedText(string value)
        {
            if (value.Length < 2 || value[0] != '"' || value[^1] != '"')
                throw new FormatException("Text annotation must be wrapped in double quotes.");

            return value[1..^1].Replace("\\\"", "\"");
        }

        private static string Quote(string value)
        {
            return $"\"{(value ?? "").Replace("\"", "\\\"")}\"";
        }
    }

    internal static class TutorialActionMarkdown
    {
        public static string[] SplitValues(string payload)
        {
            return string.IsNullOrWhiteSpace(payload)
                ? Array.Empty<string>()
                : payload.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
