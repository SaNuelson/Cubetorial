using System;
using System.Collections.Generic;
using Cubetorial.Model.Base;

namespace Cubetorial.Presentation.Scripts.View
{
    public enum PuzzleStateSetupMode
    {
        FromPrevious,
        FromSolved
    }
    
    [Serializable]
    public class StateSetup
    {
        public PuzzleStateSetupMode mode;

        public List<string> scrambleMoves = new();
    }
}
