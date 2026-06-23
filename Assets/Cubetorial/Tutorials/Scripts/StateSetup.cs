using System;
using System.Collections.Generic;
using Cubetorial.Odin.Attributes;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Cubetorial.Tutorials.Scripts
{
    public enum PuzzleStateSetupMode
    {
        FromPrevious,
        FromSolved
    }
    
    [Serializable, Toggle(nameof(isEnabled), CollapseOthersOnExpand = false)]
    public class StateSetup
    {
        public bool isEnabled;
        
        [EnumToggleButtons]
        [ShowIf(nameof(isEnabled))]
        public PuzzleStateSetupMode mode;
        
        [ParsedList]
        [ShowIf(nameof(isEnabled))]
        public string[] scrambleMoves;
    }
}
