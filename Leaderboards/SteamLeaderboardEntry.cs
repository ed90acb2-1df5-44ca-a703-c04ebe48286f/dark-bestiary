using Steamworks;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Leaderboards
{
    public class SteamLeaderboardEntry : ILeaderboardEntry
    {
        private readonly LeaderboardEntry_t entry;
        private readonly int[] skills;
        private readonly string name;
        private readonly Texture2D avatar;

        public SteamLeaderboardEntry(LeaderboardEntry_t entry, int[] skills)
        {
            this.entry = entry;
            this.skills = skills;
            this.name = SteamFriends.GetFriendPersonaName(this.entry.m_steamIDUser);
            this.avatar = CreateAvatar(entry.m_steamIDUser);
        }

        public LeaderboardEntry_t GetSteamEntry()
        {
            return this.entry;
        }

        public int GetRank()
        {
            return this.entry.m_nGlobalRank;
        }

        public int GetScore()
        {
            return this.entry.m_nScore;
        }

        public int[] GetSkills()
        {
            return this.skills;
        }

        public string GetName()
        {
            return this.name;
        }

        public Texture2D GetAvatar()
        {
            return this.avatar;
        }

        public void Dispose()
        {
            Object.Destroy(this.avatar);
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