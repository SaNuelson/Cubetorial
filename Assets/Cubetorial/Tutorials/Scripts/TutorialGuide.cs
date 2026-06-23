using System;
using System.Collections.Generic;
using System.Text;
using Cubetorial.Model;
using Cubetorial.Model.Base;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cubetorial.Tutorials.Scripts
{
    [CreateAssetMenu(fileName = "TutorialGuide", menuName = "Cubetorial/Tutorial Guide", order = 1)]
    public class TutorialGuide : ScriptableObject
    {
        public string titleKey;
        public string descriptionKey;
        public PuzzleFamily family;
        
        [SerializeReference]
        public List<GuideNode> sections = new();
    }

    [Serializable]
    public abstract class GuideNode
    {
        [TextArea]
        public string title;

        [HideLabel, InlineProperty, CanBeNull] public StateSetup stateSetup;
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

        [ButtonGroup("Create action")]
        [Button("Move")]
        public void CreateMoveAction()
        {
            actions.Add(new TutorialMove());
        }
    }
}
