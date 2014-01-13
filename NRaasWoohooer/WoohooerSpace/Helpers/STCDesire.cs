using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class STCDesire : Common.IWorldLoadFinished, Common.IWorldQuit
    {
        public void OnWorldLoadFinished()
        {
            new Common.AlarmTask(1f, TimeUnit.Minutes, OnTimer, 30f, TimeUnit.Minutes);

            ScoringLookup.UnloadCaches(true);
        }

        public void OnWorldQuit()
        {
            ScoringLookup.UnloadCaches(true);
        }

        protected static void OnTimer()
        {
            try
            {
                using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration STCDesire:OnTimer"))
                {
                    ScoringLookup.UnloadCaches(false);

                    Common.FunctionTask.Perform(OnRecalulateDesire);
                }
            }
            catch (Exception e)
            {
                Common.Exception("STCDesire:OnTimer", e);
            }
        }

        protected static void OnRecalulateDesire()
        {
            try
            {
                List<Sim> sims = new List<Sim>();
                foreach (Sim sim in LotManager.Actors)
                {
                    try
                    {
                        if (sim.InteractionQueue == null) continue;

                        if (sim.InteractionQueue.GetCurrentInteraction() is SocialInteraction) continue;

                        if (sim.Autonomy == null) continue;

                        if (sim.Autonomy.SituationComponent == null) continue;

                        if (sim.Autonomy.SituationComponent.mSituations == null) continue;

                        if (sim.Autonomy.SituationComponent.mSituations.Count > 0)
                        {
                            ScoringLookup.IncStat("STC Desire In Situation");
                            continue;
                        }

                        sims.Add(sim);
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);
                    }
                }

                foreach (Sim sim in sims)
                {
                    try
                    {
                        SocialComponent social = sim.SocialComponent;
                        if (social == null) continue;

                        social.mShortTermDesireToSocializeWith.Clear();

                        if (!Woohooer.Settings.UsingTraitScoring) continue;

                        if (sim.Autonomy.Actor == null) continue;

                        if (!sim.Autonomy.ShouldRunLocalAutonomy) continue;

                        if (sim.LotCurrent == null) continue;

                        if (sim.LotCurrent.IsWorldLot) continue;

                        SpeedTrap.Sleep();

                        if (!WoohooScoring.TestScoringNormal(sim, null, "InterestInRomance", true))
                        {
                            ScoringLookup.IncStat("STC Desire Fail");
                            continue;
                        }

                        ScoringLookup.IncStat("STC Desire Success");

                        List<Sim> others = new List<Sim>(sim.LotCurrent.GetAllActors());
                        foreach (Sim other in others)
                        {
                            if (sim == other) continue;

                            string reason;
                            GreyedOutTooltipCallback callback = null;
                            if (!CommonSocials.CanGetRomantic(sim, other, true, false, true, ref callback, out reason)) continue;

                            int std = (int)(RelationshipEx.GetAttractionScore(sim.SimDescription, other.SimDescription, false) * 2);

                            ScoringLookup.AddStat("Desire " + sim.FullName, std);

                            social.AddShortTermDesireToSocializeWith(other, std);

                            SpeedTrap.Sleep();
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
                Common.Exception("STCDesire:OnRecalculateDesire", e);
            }
        }
    }
}
