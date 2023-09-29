using System;
using DarkBestiary.Movers;

namespace DarkBestiary.Data
{
    [Serializable]
    public class MoverData
    {
        public MoverType Type;
        public float Speed;
        public float Height;
        public float Acceleration;
        public bool Rotate;

        public MoverData()
        {
        }

        public MoverData(MoverType type, float speed, float acceleration, float height, bool rotate)
        {
            Type = type;
            Speed = speed;
            Height = height;
            Acceleration = acceleration;
            Rotate = rotate;
        }
    }
}