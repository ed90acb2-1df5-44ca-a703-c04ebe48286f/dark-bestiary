using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class LeaderboardEntryPropertyView : PoolableMonoBehaviour
    {
        [SerializeField] private CharacterInfoRow m_A;
        [SerializeField] private CharacterInfoRow m_B;

        public void Construct(string keyA, string valueA, string keyB, string valueB)
        {
            m_A.Construct(keyA, valueA);
            m_B.Construct(keyB, valueB);
        }
    }
}