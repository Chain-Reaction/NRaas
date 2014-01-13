using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public class CleanupSkillModifiers : AlarmOption, Common.IDelayedWorldLoadFinished
    {
        public override string GetTitlePrefix()
        {
            return "CleanupSkillModifiers";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mCleanupSkillModifiers;
            }
            set
            {
                NRaas.Overwatch.Settings.mCleanupSkillModifiers = value;
            }
        }

        public void OnDelayedWorldLoadFinished()
        {
            PerformAction(false);
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log("Cleanup Skill Modifiers");

                float vampireTotal = OccultVampire.kSkillGainModifierArtistic;
                vampireTotal += OccultVampire.kSkillGainModifierCreative;
                vampireTotal += OccultVampire.kSkillGainModifierMental;
                vampireTotal += OccultVampire.kSkillGainModifierMusical;
                vampireTotal += OccultVampire.kSkillGainModifierPhysical;

                float vampireAverage = vampireTotal / 5;

                foreach (SimDescription sim in Household.EverySimDescription())
                {
                    try
                    {
                        if (sim.SkillManager == null) continue;

                        float maxMultiplier = 2;

                        OccultVampire vampire = sim.OccultManager.GetOccultType(OccultTypes.Vampire) as OccultVampire;
                        if ((vampire != null) && (vampire.AppliedNightBenefits))
                        {
                            maxMultiplier += vampireAverage;
                        }

                        if (sim.SkillManager.mSkillModifiers != null)
                        {
                            bool recompute = false;

                            foreach (KeyValuePair<SkillNames, float> modifier in sim.SkillManager.mSkillModifiers)
                            {
                                if (modifier.Value > maxMultiplier)
                                {
                                    recompute = true;
                                }
                                else if (modifier.Value < -1)
                                {
                                    recompute = true;
                                }

                            }

                            if (recompute)
                            {
                                ResetSimTask.ResetSkillModifiers(sim);

                                Overwatch.Log("Recalculated Skill Modifiers : " + sim.FullName);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }
    }
}
