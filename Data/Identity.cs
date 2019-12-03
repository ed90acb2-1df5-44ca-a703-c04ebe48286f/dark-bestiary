using System;

namespace DarkBestiary.Data
{
    [Serializable]
    public class Identity<TIdentity>
    {
        public TIdentity Id;
    }
}