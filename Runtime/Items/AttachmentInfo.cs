using System;

namespace DarkBestiary.Items
{
    [Serializable]
    public class AttachmentInfo
    {
        public string Prefab;
        public CasterOrTarget Target;
        public AttachmentPoint Point;
        public bool Rotate;
        public bool RotateSameAsCaster;
        public bool FlipRotation;
        public bool OriginalScale;
    }
}