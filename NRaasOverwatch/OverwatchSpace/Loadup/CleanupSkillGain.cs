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
using Sims3.Gameplay.Objects.Electronics;
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

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupSkillGain : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupSkillGain");

            foreach (Sim sim in LotManager.Actors)
            {
                Corrections.CorrectOverallSkillModifier(sim.SimDescription);
            }

            new Common.AlarmTask(1, TimeUnit.Hours, OnCheckGains, 1, TimeUnit.Hours);
        }

        public static void OnCheckGains()
        {
            foreach (Sim sim in LotManager.Actors)
            {
                if ((sim.InteractionQueue != null) && (sim.InteractionQueue.GetCurrentInteraction() == null))
                {
                    if (sim.SkillManager != null)
                    {
                        foreach (Skill skill in sim.SkillManager.List)
                        {
                            if (skill.SkillGainRate > 0)
                            {
                                skill.StopSkillGain();
                            }
                        }
                    }
                }
            }
        }
    }
}
