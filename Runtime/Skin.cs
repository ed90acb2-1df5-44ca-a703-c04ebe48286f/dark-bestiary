using System.Collections.Generic;
using DarkBestiary.Data;

namespace DarkBestiary
{
    public class Skin
    {
        public List<SkinPartInfo> Parts { get; }

        public Skin(SkinData data)
        {
            Parts = new List<SkinPartInfo>();

            foreach (var part in data.Parts)
            {
                Parts.Add(new SkinPartInfo(part.Part, part.Mesh));
            }
        }
    }
}