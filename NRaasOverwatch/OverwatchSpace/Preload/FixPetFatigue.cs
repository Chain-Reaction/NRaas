using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Preload
{
    public class FixPetFatigue : PreloadOption
    {
        public FixPetFatigue()
        { }

        public override string GetTitlePrefix()
        {
            return "FixPetFatigue";
        }

        public override void OnPreLoad()
        {
            Overwatch.Log(GetTitlePrefix());

            if (GameUtils.IsInstalled(ProductVersion.EP5))
            {
                if (SkillManager.sPetSkillFatigueTuning.Count == 0)
                {
                    SkillManager.ParsePetSkillFatigueRates(XmlDbData.ReadData("Skills"));

                    Overwatch.Log("  Loaded Missing Pet Fatigue Rates");
                }
            }
        }
    }
}
