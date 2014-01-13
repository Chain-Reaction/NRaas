using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.KamaSimtra.SkillPoints
{
    public class ListingOption : OptionList<ISkillPointsOption>, IKamaSimtraOption
    {
        public override string GetTitlePrefix()
        {
            return "SkillPoints";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<ISkillPointsOption> GetOptions()
        {
            List<ISkillPointsOption> results = new List<ISkillPointsOption>();

            for (int i = 0; i < 10; i++)
            {
                results.Add(new SkillPointsSetting(i));
            }

            return results;
        }
    }
}
