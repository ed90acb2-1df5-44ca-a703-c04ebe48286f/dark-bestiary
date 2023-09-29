namespace DarkBestiary
{
    public class SkinPartInfo
    {
        public SkinPart Part { get; }
        public string Mesh { get; }

        public SkinPartInfo(SkinPart part, string mesh)
        {
            Part = part;
            Mesh = mesh;
        }
    }
}