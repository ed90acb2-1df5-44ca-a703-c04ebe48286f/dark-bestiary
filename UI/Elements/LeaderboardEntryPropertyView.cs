using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class LeaderboardEntryPropertyView : PoolableMonoBehaviour
    {
        [SerializeField] private CharacterInfoRow a;
        [SerializeField] private CharacterInfoRow b;

        public void Construct(string keyA, string valueA, string keyB, string valueB)
        {
            this.a.Construct(keyA, valueA);
            this.b.Construct(keyB, valueB);
        }
    }
}