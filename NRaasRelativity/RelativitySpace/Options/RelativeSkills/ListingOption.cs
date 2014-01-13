using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RelativitySpace.Options.RelativeSkills
{
    public class ListingOption : InteractionOptionList<IRelativeSkillOption, GameObject>, IPrimaryOption<GameObject>, IPersistence
    {
        public override string GetTitlePrefix()
        {
            return "RelativeSkillsRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<IRelativeSkillOption> GetOptions()
        {
            List<IRelativeSkillOption> results = new List<IRelativeSkillOption>();

            results.Add(new RelativeSkill(null));

            foreach (Skill skill in SkillManager.SkillDictionary)
            {
                results.Add(new RelativeSkill(skill));
            }

            return results;
        }

        public void Import(Persistence.Lookup settings)
        {
            bool relative;

            Relativity.Settings.mRelativeSkills.Clear();
            foreach (Skill skill in SkillManager.SkillDictionary)
            {
                relative = settings.GetBool(skill.Guid.ToString(), true);
                if (relative) continue;

                Relativity.Settings.mRelativeSkills[skill.Guid] = relative;
            }

            relative = settings.GetBool(SkillNames.None.ToString(), true);
            if (!relative)
            {
                Relativity.Settings.mRelativeSkills[SkillNames.None] = relative;
            }
        }

        public void Export(Persistence.Lookup settings)
        {
            foreach (KeyValuePair<SkillNames, bool> kind in Relativity.Settings.mRelativeSkills)
            {
                settings.Add(kind.Key.ToString(), kind.Value);
            }
        }

        public string PersistencePrefix
        {
            get { return GetTitlePrefix(); }
        }
    }
}
