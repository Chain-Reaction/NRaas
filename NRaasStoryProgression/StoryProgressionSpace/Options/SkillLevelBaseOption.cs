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
    public abstract class SkillLevelBaseOption : GenericOptionBase.ListedOptionItem<string, string>
    {
        public SkillLevelBaseOption()
            : base(new List<string>(), new List<string>())
        { }

        protected override string ConvertFromString(string value)
        {
            // these should probably be used instead of the code I have in CasteOptions but it
            // is called at loadup too and I'm not sure how it'd affect the option listing if
            // I transformed the value here...
            return value;
        }

        protected override string ConvertToValue(string value, out bool valid)
        {
            // called when saving
            valid = true;
            return value;
        }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }        

        protected override string GetLocalizedValue(string value, ref ThumbnailKey icon)
        {           
            // called from option dialog
            string[] split = value.Split('-');

            if (split.Length < 2)
            {
                split[0] = ""; // will cause TryParseEnum to fail
            }

            SkillNames result;
            Skill skill;
            if (!ParserFunctions.TryParseEnum<SkillNames>(split[0], out result, SkillNames.None))
            {
                ulong guid;
                if (ulong.TryParse(split[0], out guid))
                {
                    result = (SkillNames)guid;
                } else {
                    return SkillNames.None + " " + EAText.GetNumberString(0);
                }
            }

            skill = SkillManager.GetStaticSkill(result);

            if (skill == null)
            {
                return SkillNames.None + " " + EAText.GetNumberString(0);
            }

            icon = new ThumbnailKey(skill.DreamsAndPromisesIconKey, ThumbnailSize.Medium);

            int num = 0;
            int.TryParse(split[1], out num);

            return skill.Name + " " + EAText.GetNumberString(num);            
        }        

        protected override string GetLocalizationValueKey()
        {
            // ditto
            return "SkillLevel";
        }        

        protected override string ValuePrefix
        {
            // ditto
            get { return "Boolean"; }
        }

        protected override IEnumerable<string> GetOptions()
        {
            // ditto
            List<string> results = new List<string>();

            foreach (Skill skill in SkillManager.SkillDictionary)
            {
                if (skill.mNonPersistableData == null) continue;

                int start = 1;
                while (start <= skill.mNonPersistableData.MaxSkillLevel)
                {
                    results.Add(skill.Guid + "-" + start);
                    start++;
                }
            }

            return results;
        }
    }
}