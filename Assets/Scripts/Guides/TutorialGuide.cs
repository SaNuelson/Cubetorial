using Cubetorial.Model.Base;
using UnityEngine;

namespace Guides
{
    public class TutorialGuide : ScriptableObject
    {
        public TutorialGuideChapter[] chapters;
    }

    public class TutorialGuideChapter
    {
        public string title;
        public PuzzleState puzzleState;
        public TutorialGuideBlock[] steps;
    }
    
    public class TutorialGuideBlock
    {
        public string description;
        public TutorialAction[] actions;
    }
}