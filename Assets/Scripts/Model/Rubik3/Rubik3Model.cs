using Model.Base;

namespace Model.Rubik3
{
    public sealed class Rubik3Model : PuzzleModel
    {
        public Rubik3Model()
            : base(Rubik3Definition.Create())
        {
        }
    }
}
