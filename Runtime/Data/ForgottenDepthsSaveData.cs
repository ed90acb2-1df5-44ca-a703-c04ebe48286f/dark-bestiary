using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ForgottenDepthsSaveData
    {
        public int Depth;
        public List<int> Behaviours = new();
    }
}