using System;
using UnityEngine;

namespace DarkBestiary.Leaderboards
{
    public interface ILeaderboardEntry : IDisposable
    {
        int GetRank();

        int GetScore();

        int[] GetSkills();

        string GetName();

        Texture2D GetAvatar();
    }
}