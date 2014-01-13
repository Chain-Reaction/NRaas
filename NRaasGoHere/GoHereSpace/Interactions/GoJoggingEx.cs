using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class GoJoggingEx : Terrain.GoJogging, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.GetTuning<Sim, Terrain.GoJogging.Definition>();
            if (tuning != null)
            {
                tuning.RemoveFlags(InteractionTuning.FlagField.DisallowUserDirected);
                tuning.RemoveFlags(InteractionTuning.FlagField.DisallowPlayerSim);

                tuning.Availability.AgeSpeciesAvailabilityFlags = CASAGSAvailabilityFlags.HumanTeen | CASAGSAvailabilityFlags.HumanYoungAdult | CASAGSAvailabilityFlags.HumanAdult | CASAGSAvailabilityFlags.HumanElder;
            }

            sOldSingleton = Singleton as InteractionDefinition;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Terrain.GoJogging.Definition>(Singleton as InteractionDefinition);
        }

        public override bool Run()
        {
            try
            {
                GoToVenue.Item choice = GoToVenue.GetChoices(Actor, Terrain.GoJogging.LocalizeString(Actor.IsFemale, "InteractionName", new object[0x0]));
                if (choice == null) return false;

                Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToWorkOut);

                Sim.WalkStyle simWalkStyle = Actor.SimWalkStyle;
                mAthleticSkill = Actor.SkillManager.GetSkill<Athletic>(SkillNames.Athletic);
                if (mAthleticSkill.SkillLevel >= Terrain.JogHere.AthleticSkillLevelToFastJog)
                {
                    RequestWalkStyle(Sim.WalkStyle.FastJog);
                }
                else
                {
                    RequestWalkStyle(Sim.WalkStyle.Jog);
                }

                Destination = Actor.Position;
                mLastTime = SimClock.ElapsedTime(TimeUnit.Minutes);
                AlarmHandle handle = Actor.AddAlarmRepeating(1f, TimeUnit.Minutes, JogLoop, 1f, TimeUnit.Minutes, "Jog Here: Check for Fatigued Buff", AlarmType.DeleteOnReset);
                AlarmHandle handle2 = Actor.AddAlarm(Terrain.JogHere.AthleticTuning.MinsToAddPumped, TimeUnit.Minutes, AddPumped, "AthleticGameObject: Pumped", AlarmType.DeleteOnReset);
               
                float num = 1f - (Terrain.JogHere.AthleticTuning.DelayFatiguePercentagePerAthleticSkillLevel * mAthleticSkill.SkillLevel);
                if (num <= 0f)
                {
                    num = 0f;
                }

                float multiplier = 1f;
                if (Actor.TraitManager.HasElement(TraitNames.Athletic))
                {
                    multiplier = Terrain.JogHere.AthleticTuning.AthleticTraitDestressMultiplier;
                }

                StandardEntry();
                BeginCommodityUpdate(CommodityKind.Fun, multiplier);
                if (mAthleticSkill.IsFitnessNut())
                {
                    num = 0f;
                }

                float athleticTraitSimFatigueMultiplier = 1f;
                if (Actor.TraitManager.HasElement(TraitNames.Athletic))
                {
                    athleticTraitSimFatigueMultiplier = TraitTuning.AthleticTraitSimFatigueMultiplier;
                }

                Lot currentLot = Actor.LotCurrent;

                bool succeeded = false;
                try
                {
                    BeginCommodityUpdate(CommodityKind.Fatigue, num * athleticTraitSimFatigueMultiplier);
                    BeginCommodityUpdates();

                    if (CurrentTone is Athletic.NoSweatTone)
                    {
                        DisableMotiveDelta(CommodityKind.Hygiene);
                    }

                    StartFitness(Actor);

                    Route r = Actor.CreateRoute();
                    r.SetOption(Route.RouteOption.EnablePlanningAsCar, false);
                    r.SetOption(Route.RouteOption.EnableSubwayPlanning, false);
                    r.SetOption(Route.RouteOption.DoNotEmitDegenerateRoutesForRadialRangeGoals, true);
                    r.SetOption(Route.RouteOption.DoLineOfSightCheckUserOverride, true);
                    r.SetOption(Route.RouteOption.CheckForFootprintsNearGoals, true);

                    choice.Value.PlanToLot(r);
                    succeeded = Actor.DoRoute(r);

                    r = Actor.CreateRoute();
                    r.SetOption(Route.RouteOption.EnablePlanningAsCar, false);
                    r.SetOption(Route.RouteOption.EnableSubwayPlanning, false);
                    r.SetOption(Route.RouteOption.DoNotEmitDegenerateRoutesForRadialRangeGoals, true);
                    r.SetOption(Route.RouteOption.DoLineOfSightCheckUserOverride, true);
                    r.SetOption(Route.RouteOption.CheckForFootprintsNearGoals, true);

                    currentLot.PlanToLot(r);
                    succeeded = succeeded && Actor.DoRoute(r);
                }
                finally
                {
                    StopFitness(Actor);

                    EndCommodityUpdates(succeeded);
                    StandardExit();
                }

                Actor.RemoveAlarm(handle);
                Actor.RemoveAlarm(handle2);

                Actor.BuffManager.UnpauseBuff(BuffNames.Pumped);
                return succeeded;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        private new class Definition : Terrain.GoJogging.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GoJoggingEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a != target) return false;

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
