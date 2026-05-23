using Model.Base;

namespace Model.Skewb
{
    public sealed class SkewbModel : PuzzleModel
    {
        public SkewbModel()
            : base(SkewbDefinition.Create())
        {
        }
    }
}
