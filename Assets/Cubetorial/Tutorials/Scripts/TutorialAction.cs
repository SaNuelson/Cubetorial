using System;

namespace Cubetorial.Tutorials.Scripts
{
    [Serializable]
    public abstract class TutorialAction
    {
        
    }

    [Serializable]
    public class TutorialMove : TutorialAction
    {
        public string moveId;
    }
    
    [Serializable]
    public class TutorialFocus : TutorialAction
    {
        public string focusedCubeId;
    }
    
    [Serializable]
    public class TutorialHighlight: TutorialAction
    {
        public string[] highlightedCubeIds;
    }

    [Serializable]
    public class TutorialAnnotation : TutorialAction
    {
        public string annotatedCubeId;
        public string text;
    }
}