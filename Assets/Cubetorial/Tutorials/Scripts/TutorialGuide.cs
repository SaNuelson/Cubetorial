using System;
using System.Collections.Generic;
using System.Text;
using Cubetorial.Model;
using Cubetorial.Model.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cubetorial.Tutorials.Scripts
{
    [CreateAssetMenu(fileName = "TutorialGuide", menuName = "Cubetorial/Tutorial Guide", order = 1)]
    public class TutorialGuide : ScriptableObject
    {
        /// <summary>
        /// Unique identifier for a group of guides.
        /// </summary>
        public string guideId;
        
        /// <summary>
        /// Language identifier for this guide.
        /// </summary>
        public string language;

        public PuzzleFamily family;
        
        public string title;
        public string description;
        
        [SerializeReference]
        public List<GuideNode> sections = new();
    }

    [Serializable]
    public abstract class GuideNode
    {
        [TextArea]
        public string title;

        [HideLabel, InlineProperty] public StateSetup stateSetup;
    }

    [Serializable]
    public class GuideSection : GuideNode
    {
        [SerializeReference]
        public List<GuideNode> nodes = new();
    }
    
    [Serializable]
    public class GuideBlock : GuideNode
    {
        [TextArea]
        public string body;

        [SerializeReference]
        public List<TutorialAction> actions = new();
    }
}
