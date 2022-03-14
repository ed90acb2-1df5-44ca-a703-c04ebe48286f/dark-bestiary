using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Readers;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Visions;
using UnityEngine;
using Zenject;

namespace DarkBestiary
{
    public class VisionProgression : IInitializable
    {
        public const int MaxLevel = 5;

        public Experience Experience { get; private set; }
        public TalentsComponent Talents { get; private set; }

        private readonly StorageId storageId;
        private readonly IFileReader reader;
        private readonly ITalentRepository talentRepository;

        public VisionProgression(StorageId storageId, IFileReader reader, ITalentRepository talentRepository)
        {
            this.storageId = storageId;
            this.reader = reader;
            this.talentRepository = talentRepository;
        }

        public void Initialize()
        {
            var data = this.reader.Read<VisionProgressionSaveData>(GetDataPath()) ?? new VisionProgressionSaveData();

            Experience = new Experience(data.Level, MaxLevel, data.Experience, Formula);
            Experience.Add(0);
            Experience.CreateSnapshot(0);

            VisionManager.Completed += OnVisionCompleted;

            Talents = new GameObject("VisionTalents").AddComponent<TalentsComponent>();
            Talents.Construct(this.talentRepository.Find(t => t.CategoryId == Constants.TalentCategoryIdDreams), new int[]{}, Experience.Level);
            Talents.Initialize();
            Talents.transform.position = new Vector3(-500, 0, 0);

            Application.quitting += OnApplicationQuitting;
        }

        private void OnVisionCompleted()
        {
            const int experience = 50;
            Experience.CreateSnapshot(experience);
            Experience.Add(experience);
            Talents.Points = Experience.Level;
            Save();
        }

        private void OnApplicationQuitting()
        {
            Application.quitting -= OnApplicationQuitting;
            Save();
        }

        private void Save()
        {
            var data = new VisionProgressionSaveData
            {
                Level = Experience.Level,
                Experience = Experience.Current
            };

            this.reader.Write(data, GetDataPath());
        }

        private int Formula(int level)
        {
            if (level < 2)
            {
                return 0;
            }

            return Mathf.Max(1, level - 1) * 50;
        }

        private string GetDataPath()
        {
            return Environment.PersistentDataPath + $"/{this.storageId}/vision_progression.save";
        }
    }
}