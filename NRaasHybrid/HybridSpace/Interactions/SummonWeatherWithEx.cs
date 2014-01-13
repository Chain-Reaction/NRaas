using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Interactions
{
    public class SummonWeatherWithEx : WeatherStone.SummonWeatherWith, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<WeatherStone, WeatherStone.SummonWeatherWith.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<WeatherStone, WeatherStone.SummonWeatherWith.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                List<Sim> followers = new List<Sim>();
                if (SelectedObjects != null)
                {
                    foreach (object obj2 in SelectedObjects)
                    {
                        followers.Add(obj2 as Sim);
                    }
                }

                if (Target.LotCurrent != Actor.LotCurrent)
                {
                    Route r = Actor.CreateRoute();
                    Target.LotCurrent.PlanToLot(r);
                    Actor.DoRouteWithFollowers(r, followers);
                    Lot.ValidateFollowers(followers);
                }

                Definition definition = InteractionDefinition as Definition;

                InteractionDefinition summon = new SummonWeatherEx.Definition(definition.mType);

                foreach (Sim sim in followers)
                {
                    if (sim.LotCurrent == Target.LotCurrent)
                    {
                        InteractionPriority priority = GetPriority();
                        if (!sim.IsSelectable)
                        {
                            priority = new InteractionPriority(InteractionPriorityLevel.NonCriticalNPCBehavior);
                        }
                        sim.InteractionQueue.AddNext(summon.CreateInstance(Target, sim, priority, Autonomous, CancellableByPlayer));
                    }
                }

                Actor.InteractionQueue.PushAsContinuation(summon, Target, true);
                return true;
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

        public new class Definition : WeatherStone.SummonWeatherWith.Definition
        {
            public readonly WeatherStone.SummonWeatherType mType;

            public Definition()
            { }
            public Definition(WeatherStone.SummonWeatherType type)
            {
                mType = type;
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SummonWeatherWithEx();
                na.Init(ref parameters);
                return na;
            }

            private static List<Sim> GetValidCandidates(Lot lot, Sim actor, WeatherStone.SummonWeatherType type)
            {
                return lot.GetSims(delegate(Sim s)
                {
                    return IsValidCandidate(s, actor, type);
                });
            }

            private static bool HasValidCandidates(Lot lot, Sim actor, WeatherStone.SummonWeatherType type)
            {
                return lot.DoesSimExist(delegate(Sim s)
                {
                    return (s != actor) && IsValidCandidate(s, actor, type);
                });
            }

            private static bool IsValidCandidate(Sim sim, Sim actor, WeatherStone.SummonWeatherType type)
            {
                if (sim.SimDescription.ChildOrBelow)
                {
                    return false;
                }
                else if (sim == actor)
                {
                    return false;
                }

                return SummonWeatherEx.GetSummonWeatherTypeForSim(sim.SimDescription).Contains(type);
            }

            public override string GetInteractionName(Sim actor, WeatherStone target, InteractionObjectPair iop)
            {
                bool isFemale = actor.IsFemale;
                return WeatherStone.LocalizeString(isFemale, "SummonWeatherWithInteractionName", new object[] { WeatherStone.LocalizeString(isFemale, mType.ToString(), new object[0]) });
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, WeatherStone target, List<InteractionObjectPair> results)
            {
                foreach (WeatherStone.SummonWeatherType type in SummonWeatherEx.GetSummonWeatherTypeForSim(actor.SimDescription))
                {
                    results.Add(new InteractionObjectPair(new Definition(type), target));
                }
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                NumSelectableRows = -1;
                PopulateSimPicker(ref parameters, out listObjs, out headers, GetValidCandidates(parameters.Actor.LotCurrent, parameters.Actor as Sim, mType), false);
            }

            public override bool Test(Sim actor, WeatherStone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!SummonWeatherEx.CommonTest(target, actor, mType, ref greyedOutTooltipCallback, isAutonomous))
                {
                    return false;   
                }
                
                return HasValidCandidates(actor.LotCurrent, actor, mType);
            }
        }
    }
}
