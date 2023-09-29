using Steamworks;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Leaderboards
{
    public class SteamLeaderboardEntry : ILeaderboardEntry
    {
        private readonly LeaderboardEntry_t m_Entry;
        private readonly int[] m_Skills;
        private readonly string m_Name;
        private readonly Texture2D m_Avatar;

        public SteamLeaderboardEntry(LeaderboardEntry_t entry, int[] skills)
        {
            m_Entry = entry;
            m_Skills = skills;
            m_Name = SteamFriends.GetFriendPersonaName(m_Entry.m_steamIDUser);
            m_Avatar = CreateAvatar(entry.m_steamIDUser);
        }

        public LeaderboardEntry_t GetSteamEntry()
        {
            return m_Entry;
        }

        public int GetRank()
        {
            return m_Entry.m_nGlobalRank;
        }

        public int GetScore()
        {
            return m_Entry.m_nScore;
        }

        public int[] GetSkills()
        {
            return m_Skills;
        }

        public string GetName()
        {
            return m_Name;
        }

        public Texture2D GetAvatar()
        {
            return m_Avatar;
        }

        public void Dispose()
        {
            Object.Destroy(m_Avatar);
        }

        private static Texture2D CreateAvatar(CSteamID userId)
        {
            var avatarId = SteamFriends.GetMediumFriendAvatar(userId);

            if (!SteamUtils.GetImageSize(avatarId, out var width, out var height))
            {
                return null;
            }

            var bufferPool = ArrayPool<byte>.GetPool((int) (width * height * 4));
            var buffer = bufferPool.Spawn();

            if (!SteamUtils.GetImageRGBA(avatarId, buffer, (int) (width * height * 4)))
            {
                bufferPool.Despawn(buffer);
                return null;
            }

            var texture = new Texture2D((int) width, (int) height, TextureFormat.RGBA32, false, true);
            texture.LoadRawTextureData(buffer);
            texture.Apply();

            bufferPool.Despawn(buffer);

            return texture;
        }
    }
}