using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public class DisallowSkillOption : GenericOptionBase.ListedOptionItem<SkillNames, string>, IReadSimLevelOption, IWriteSimLevelOption, IHouseLevelSimOption
    {
        public DisallowSkillOption()
            : base(new List<string>(), new List<string>())
        { }

        public override string GetTitlePrefix()
        {
            return "DisallowSkill";
        }

        protected override string ValuePrefix
        {
            get { return "Disallowed"; }
        }

        protected override string ConvertFromString(string value)
        {
            return value;
        }

        protected override string ConvertToValue(SkillNames value, out bool valid)
        {
            Skill skill = SkillManager.GetStaticSkill(value);
            if (skill == null)
            {
                valid = false;
                return value.ToString();
            }
            else
            {
                valid = true;
                return skill.Name;
            }
        }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }

        protected override string GetLocalizedValue(SkillNames value, ref ThumbnailKey icon)
        {
            Skill skill = SkillManager.GetStaticSkill(value);
            if (skill == null) return null;

            icon = new ThumbnailKey(skill.DreamsAndPromisesIconKey, ThumbnailSize.Medium);

            return skill.Name;
        }

        protected override IEnumerable<SkillNames> GetOptions()
        {
            List<SkillNames> results = new List<SkillNames>();

            Dictionary<SkillNames, bool> lookup = new Dictionary<SkillNames, bool>();

            foreach (Skill skill in SkillManager.SkillDictionary)
            {
                if (lookup.ContainsKey(skill.Guid)) continue;
                lookup.Add(skill.Guid, true);

                if (skill.NonPersistableData == null) continue;

                results.Add(skill.Guid);
            }

            return results;
        }

        public override bool ShouldDisplay()
        {
            if (!GetValue<AllowSkillOption, bool>()) return false;

            return base.ShouldDisplay();
        }
    }
}

