using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Dialogues;
using DarkBestiary.Managers;

namespace DarkBestiary.Data.Mappers
{
    public class DialogueMapper : Mapper<DialogueData, Dialogue>
    {
        private static readonly Dictionary<string, Type> Mapping = new Dictionary<string, Type>();

        static DialogueMapper()
        {
            Assembly.GetAssembly(typeof(DialogueAction))
                .GetTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(DialogueAction)) && !type.IsAbstract)
                .ToList()
                .ForEach(type => Mapping.Add(type.Name, type));
        }

        private readonly IValidatorRepository validatorRepository;
        private readonly CharacterManager characterManager;

        public DialogueMapper(IValidatorRepository validatorRepository, CharacterManager characterManager)
        {
            this.validatorRepository = validatorRepository;
            this.characterManager = characterManager;
        }

        public override DialogueData ToData(Dialogue entity)
        {
            throw new NotImplementedException();
        }

        public override Dialogue ToEntity(DialogueData data)
        {
            return new Dialogue(
                data, MapActions(data.Actions), this.validatorRepository.Find(data.Validators), this.characterManager);
        }

        private static IReadOnlyCollection<DialogueAction> MapActions(IEnumerable<DialogueActionData> actions)
        {
            return actions.Select(MapAction).ToList();
        }

        private static DialogueAction MapAction(DialogueActionData data)
        {
            if (!Mapping.ContainsKey(data.Type))
            {
                throw new Exception("Unknown dialogue action type " + data.Type);
            }

            return Container.Instance.Instantiate(Mapping[data.Type], new object[] {data}) as DialogueAction;
        }
    }
}