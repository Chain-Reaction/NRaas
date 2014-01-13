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
    public class SummonWeatherEx : WeatherStone.SummonWeather, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<WeatherStone, WeatherStone.SummonWeather.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<WeatherStone, WeatherStone.SummonWeather.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                Definition interactionDefinition = InteractionDefinition as Definition;

                mIsJoin = interactionDefinition.IsJoin;
                if (!Actor.RoutingComponent.RouteToObjectRadialRange(Target, WeatherStone.kSummonWeatherRouteDistance[0], WeatherStone.kSummonWeatherRouteDistance[1], AlwaysRoute ? new Route.RouteOption[] { Route.RouteOption.DoNotEmitDegenerateRoutesForRadialRangeGoals } : null))
                {
                    return false;
                }

                if ((interactionDefinition.mType == WeatherStone.SummonWeatherType.HuntersStorm) && !Actor.BuffManager.HasElement(BuffNames.Werewolf))
                {
                    InteractionInstance instance = OccultWerewolf.TransformationToWerewolf.Singleton.CreateInstance(Actor, Actor, GetPriority(), Autonomous, CancellableByPlayer);
                    if (instance != null)
                    {
                        if (instance.RunInteraction())
                        {
                            Actor.ClearExitReasons();
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                if (!Target.TryStartSummoningWeather(Actor, interactionDefinition.mType, mIsJoin))
                {
                    if (mIsJoin || !Target.TryStartSummoningWeather(Actor, interactionDefinition.mType, true))
                    {
                        Sim createdSim = null;
                        if (Target.mSummoningSimDescriptionId != 0L)
                        {
                            createdSim = SimDescription.GetCreatedSim(Target.mSummoningSimDescriptionId);
                        }
                        Actor.PlayRouteFailure(createdSim);
                        return false;
                    }
                    mIsJoin = true;
                }

                StandardEntry();
                EnterStateMachine("SummonWeather", "Enter", "x");
                if (Target.mSummonWeatherType == WeatherStone.SummonWeatherType.BewitchingRain)
                {
                    StartLightningEffects();
                }

                AnimateSim(interactionDefinition.mType.ToString());
                BeginCommodityUpdates();
                if (mIsJoin)
                {
                    mSummonWeatherTimeRemaining = WeatherStone.kSummonWeatherDetermineOutcomeTime[1] + 10f;
                }
                else
                {
                    mSummonWeatherTimeRemaining = RandomUtil.GetFloat(WeatherStone.kSummonWeatherDetermineOutcomeTime[0], WeatherStone.kSummonWeatherDetermineOutcomeTime[1]);
                }

                bool succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), SummonLoop, mCurrentStateMachine);
                EndCommodityUpdates(succeeded);
                StandardExit();
                switch (Target.CurrentWeatherStoneState)
                {
                    case WeatherStone.WeatherStoneState.AfterSuccessfulSummon:
                        AnimateSim("Success");
                        BuffNames buffNameFromSummonWeatherType = WeatherStone.GetBuffNameFromSummonWeatherType(Target.mSummonWeatherType);
                        if (buffNameFromSummonWeatherType != BuffNames.Undefined)
                        {
                            Actor.BuffManager.AddElement(buffNameFromSummonWeatherType, Origin.FromWeatherStone);
                        }
                        if (!mIsJoin)
                        {
                            Target.StopElectrifiedEffect();
                        }
                        break;
                    case WeatherStone.WeatherStoneState.AfterFailureSummon:
                        if (!Target.mUseExtremeFailure)
                        {
                            AnimateSim("FailureNormal");
                        }
                        else
                        {
                            BuffSinged.SingeViaInteraction(this, Origin.FromLightning);
                            AnimateSim("FailureExtreme");
                        }

                        if (!HasOtherSimsSummoning(Actor))
                        {
                            Target.AnimateChangeWeatherStoneState(false, WeatherStone.WeatherStoneState.Resting);
                        }
                        break;
                    default:
                        if (!mIsJoin)
                        {
                            Target.OnSummoningStoppedEarly();
                        }
                        AnimateSim("FailureNormal");
                        break;
                }

                AnimateSim("Exit");
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

        public override string GetInteractionName()
        {
            return InteractionDefinition.GetInteractionName(Actor, Target, InteractionObjectPair);
        }

        public static bool CommonTest(WeatherStone ths, Sim actor, WeatherStone.SummonWeatherType type, ref GreyedOutTooltipCallback greyedOutTooltipCallback, bool isAutonomous)
        {
            if (!isAutonomous)
            {
                switch (ths.CurrentWeatherStoneState)
                {
                    case WeatherStone.WeatherStoneState.ReadyForSummoning:
                        return true;
                    case WeatherStone.WeatherStoneState.SummoningWeather:
                        if (type == ths.mSummonWeatherType) return true;

                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(WeatherStone.LocalizeString(actor.IsFemale, "TryingToSummonTooltip", new object[0]));
                        break;

                    case WeatherStone.WeatherStoneState.AfterSuccessfulSummon:
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(WeatherStone.LocalizeString(actor.IsFemale, "StoneCurrentSummoningWeatherTooltip", new object[0]));
                        break;

                    case WeatherStone.WeatherStoneState.AfterFailureSummon:
                    case WeatherStone.WeatherStoneState.Resting:
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(WeatherStone.LocalizeString(actor.IsFemale, "StoneRestingTooltip", new object[0]));
                        break;
                }
            }

            return false;
        }

        public static List<WeatherStone.SummonWeatherType> GetSummonWeatherTypeForSim(SimDescription sim)
        {
            List<WeatherStone.SummonWeatherType> types = new List<WeatherStone.SummonWeatherType>();

            if (sim.IsVampire)
            {
                types.Add(WeatherStone.SummonWeatherType.EclipsingFog);
            }

            if (sim.IsWerewolf)
            {
                types.Add(WeatherStone.SummonWeatherType.HuntersStorm);
            }
            
            if (sim.IsFairy)
            {
                types.Add(WeatherStone.SummonWeatherType.RevivingSprinkle);
            }

            if (sim.IsWitch)
            {
                types.Add(WeatherStone.SummonWeatherType.BewitchingRain);
            }

            return types;
        }

        public new class Definition : WeatherStone.SummonWeather.Definition
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
                InteractionInstance na = new SummonWeatherEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, WeatherStone target, InteractionObjectPair iop)
            {
                return WeatherStone.LocalizeString(actor.IsFemale, mIsJoin ? "JoinSummonWeatherInteractionName" : "SummonWeatherInteractionName", new object[] { WeatherStone.LocalizeString(actor.IsFemale, mType.ToString(), new object[0]) });
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, WeatherStone target, List<InteractionObjectPair> results)
            {
                foreach (WeatherStone.SummonWeatherType type in GetSummonWeatherTypeForSim(actor.SimDescription))
                {
                    results.Add(new InteractionObjectPair(new Definition(type), target));
                }
            }

            public override bool Test(Sim actor, WeatherStone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!mIsJoin)
                {
                    if (!CommonTest(target, actor, mType, ref greyedOutTooltipCallback, isAutonomous))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!target.IsSummoning)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Not Summoning");
                        return false;
                    }
                    else if (target.mSummonWeatherType != mType)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Not Correct Type");
                        return false;
                    }
                    else if (target.mSummoningSimDescriptionId == actor.SimDescription.SimDescriptionId)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Is Summoner");
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
