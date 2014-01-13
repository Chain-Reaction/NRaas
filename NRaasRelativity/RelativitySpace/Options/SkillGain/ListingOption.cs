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

namespace NRaas.RelativitySpace.Options.SkillGain
{
    public class ListingOption : InteractionOptionList<ISkillGainOption, GameObject>, IPrimaryOption<GameObject>, IPersistence
    {
        public override string GetTitlePrefix()
        {
            return "SkillGainRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<ISkillGainOption> GetOptions()
        {
            List<ISkillGainOption> results = new List<ISkillGainOption>();

            results.Add(new SkillGainFactor(null));

            foreach (Skill skill in SkillManager.SkillDictionary)
            {
                results.Add(new SkillGainFactor(skill));
            }

            return results;
        }

        public void Import(Persistence.Lookup settings)
        {
            float delta;

            Relativity.Settings.mSkillGains.Clear();
            foreach (Skill skill in SkillManager.SkillDictionary)
            {
                delta = settings.GetFloat(skill.Guid.ToString(), 1f);
                if (delta == 1f) continue;

                Relativity.Settings.mSkillGains[skill.Guid] = delta;
            }

            delta = settings.GetFloat(SkillNames.None.ToString(), 1f);
            if (delta != 1f)
            {
                Relativity.Settings.mSkillGains[SkillNames.None] = delta;
            }
        }

        public void Export(Persistence.Lookup settings)
        {
            foreach (KeyValuePair<SkillNames, float> kind in Relativity.Settings.mSkillGains)
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
