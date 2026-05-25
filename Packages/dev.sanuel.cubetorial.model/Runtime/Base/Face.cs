namespace Cubetorial.Model.Base
{
    public sealed class Face
    {
        public Face(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }

        public string Id { get; }

        public string DisplayName { get; }
    }
}
