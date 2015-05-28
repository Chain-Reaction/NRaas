using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Skills;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class Rendezvous : RabbitHole.RabbitHoleInteraction<Sim, DaySpa>, Common.IAddInteraction
    {
        static List<BuffNames> sRandomBuffs = new List<BuffNames>(new BuffNames[] { BuffNames.Kicked, BuffNames.LetOffSteam, BuffNames.Bananah, BuffNames.BeguiledByBubbles, BuffNames.Bitten, BuffNames.Bored, BuffNames.BrainFreeze, BuffNames.CompletelyAtEase, BuffNames.MeterHighClub, BuffNames.ExhilaratingShower, BuffNames.Embarrassed, BuffNames.Dizzy, BuffNames.Backache, BuffNames.AdrenalineRush, BuffNames.BadLanding, BuffNames.CoveredInNectar, BuffNames.DuckTimeFun, BuffNames.ForciblyDismounted, BuffNames.DreamComeTrue, BuffNames.FeelingAlive, BuffNames.GreatDate, BuffNames.GreatTimeOut, BuffNames.Humiliated, BuffNames.HurtFoot, BuffNames.HurtHand, BuffNames.IAmTheGreatest, BuffNames.Impressed, BuffNames.MintyBreath, BuffNames.Relaxed, BuffNames.Rejuvinated, BuffNames.RolledInTheHay, BuffNames.SaddleSore, BuffNames.Soaked, BuffNames.Sore, BuffNames.Strained, BuffNames.ScubaAdventure, BuffNames.SupremelyFulfilled, BuffNames.TastesLikeHeaven, BuffNames.TastesLikeFridge, BuffNames.Fatigued, BuffNames.TotallyMellow, BuffNames.MMMApples, BuffNames.ChocolateChuckles, BuffNames.DrawnIn, BuffNames.AcrobatPoor, BuffNames.FreezerBurn, BuffNames.CatScratch });

        bool mMaster = true;
        bool mBegin = false;

        static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<DaySpa>(Singleton);
        }

        protected static int OnSort(Pair<int, SimDescription> a, Pair<int, SimDescription> b)
        {
            try
            {
                return a.First.CompareTo(b.First);
            }
            catch (Exception e)
            {
                Common.Exception(a.Second.FullName + " " + b.Second.FullName, e);
            }

            return 0;
        }

        public override void ConfigureInteraction()
        {
            base.ConfigureInteraction();

            TimedStage stage = new TimedStage(GetInteractionName(), mMaster ? KamaSimtra.Settings.mRendezvousWaitPeriod : KamaSimtra.Settings.mRendezvousDuration, false, false, true);
            Stages = new List<Stage>(new Stage[] { stage });
            ActiveStage = stage;
        }

        public override bool InRabbitHole()
        {
            try
            {
                ActiveStage.Start();

                SimDescription choice = null;

                if (mMaster)
                {
                    if (!AcceptCancelDialog.Show(Common.Localize("Rendezvous:Prompt", Actor.IsFemale, new object[] { KamaSimtra.Settings.mRendezvousCostPerLevel })))
                    {
                        return false;
                    }

                    CASAgeGenderFlags allow = CASAgeGenderFlags.None;
                    if ((Actor.SimDescription.Teen && Woohooer.Settings.AllowTeen(true)) || (Actor.SimDescription.YoungAdultOrAbove && Woohooer.Settings.AllowTeenAdult(true)))
                    {
                        allow |= CASAgeGenderFlags.Teen;                        
                    }

                    if(Actor.SimDescription.Teen && Woohooer.Settings.AllowTeen(true) && Woohooer.Settings.AllowTeenAdult(true))
                    {
                        allow |= CASAgeGenderFlags.YoungAdult | CASAgeGenderFlags.Adult | CASAgeGenderFlags.Elder; 
                    }

                    if (Actor.SimDescription.YoungAdultOrAbove)
                    {
                        allow |= CASAgeGenderFlags.YoungAdult | CASAgeGenderFlags.Adult | CASAgeGenderFlags.Elder; 
                    }

                    Dictionary<int, List<SimDescription>> potentials = KamaSimtra.GetPotentials(allow, false);

                    List<SimDescription> choices = new List<SimDescription>();
                    for (int i = 1; i <= 10; i++)
                    {
                        List<SimDescription> fullList;
                        if (!potentials.TryGetValue(i, out fullList)) continue;

                        bool needFemale = false;

                        if (Actor.SimDescription.CanAutonomouslyBeRomanticWithGender(CASAgeGenderFlags.Male))
                        {
                            if (Actor.SimDescription.CanAutonomouslyBeRomanticWithGender(CASAgeGenderFlags.Female))
                            {
                                if (RandomUtil.CoinFlip())
                                {
                                    needFemale = true;
                                }
                            }
                            else
                            {
                                needFemale = false;
                            }
                        }
                        else if (Actor.SimDescription.CanAutonomouslyBeRomanticWithGender(CASAgeGenderFlags.Female))
                        {
                            needFemale = true;
                        }
                        else
                        {
                            needFemale = !Actor.IsFemale;
                        }

                        List<SimDescription> randomList = new List<SimDescription>();

                        foreach (SimDescription sim in fullList)
                        {
                            if (sim.IsFemale != needFemale) continue;

                            if (sim.Household == Actor.Household) continue;

                            string reason;
                            GreyedOutTooltipCallback callback = null;
                            if (!CommonSocials.CanGetRomantic(Actor.SimDescription, sim, false, true, true, ref callback, out reason))
                            {
                                if (callback != null)
                                {
                                    Common.DebugNotify(sim.FullName + Common.NewLine + callback());
                                }
                                continue;
                            }

                            if (choices.Contains(sim)) continue;

                            randomList.Add(sim);
                        }

                        if (randomList.Count > 0)
                        {
                            choices.Add(RandomUtil.GetRandomObjectFromList(randomList));
                        }
                    }

                    if (choices.Count == 0)
                    {
                        Common.Notify(Common.Localize("Rendezvous:NoneAvailable", Actor.IsFemale));
                        return false;
                    }

                    choice = new SimSelection(Common.Localize("Rendezvous:MenuName"), Actor.SimDescription, choices, SimSelection.Type.Rendezvous, -1000).SelectSingle();
                    if (choice == null)
                    {
                        Common.Notify(Common.Localize("Rendezvous:NoSelect", Actor.IsFemale));
                        return false;
                    }

                    if (Instantiation.PerformOffLot(choice, Target.LotCurrent, null) == null)
                    {
                        Common.Notify(Common.Localize("Rendezvous:BadSim", Actor.IsFemale, new object[] { choice }));
                        return false;
                    }

                    Rendezvous interaction = Singleton.CreateInstance(Target, choice.CreatedSim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as Rendezvous;
                    interaction.mMaster = false;
                    interaction.LinkedInteractionInstance = this;

                    choice.CreatedSim.InteractionQueue.CancelAllInteractions();
                    if (!choice.CreatedSim.InteractionQueue.AddNext(interaction))
                    {
                        Common.Notify(Common.Localize("Rendezvous:BadSim", Actor.IsFemale, new object[] { choice }));
                        return false;
                    }

                    if (!DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), WaitPeriodLoop, null))
                    {
                        Common.Notify(Common.Localize("Rendezvous:BadSim", Actor.IsFemale, new object[] { choice }));
                        return false;
                    }

                    if (!mBegin)
                    {
                        Common.Notify(Common.Localize("Rendezvous:BadSim", Actor.IsFemale, new object[] { choice }));
                        return false;
                    }
                    else
                    {
                        Actor.ClearExitReasons();

                        TimedStage stage = new TimedStage(GetInteractionName(), KamaSimtra.Settings.mRendezvousDuration, false, false, true);
                        Stages = new List<Stage>(new Stage[] { stage });
                        ActiveStage = stage;
                        ActiveStage.Start();
                    }
                }
                else
                {
                    Rendezvous interaction = LinkedInteractionInstance as Rendezvous;
                    if (interaction == null) return false;

                    interaction.mBegin = true;
                }

                if (mMaster)
                {
                    if (!CelebrityManager.TryModifyFundsWithCelebrityDiscount(Actor, Target, KamaSimtra.Settings.mRendezvousCostPerLevel * choice.SkillManager.GetSkillLevel(KamaSimtra.StaticGuid), true))
                    {
                        Common.Notify(Common.Localize("Rendezvous:CannotPay", Actor.IsFemale));
                        return false;
                    }

                    Common.Notify(choice.CreatedSim, Common.Localize("Rendezvous:Success", Actor.IsFemale, choice.IsFemale, new object[] { choice }));

                    KamaSimtra skill = KamaSimtra.EnsureSkill(Actor);
                    if (skill != null)
                    {
                        skill.RendezvousActive = true;
                    }
                }

                BeginCommodityUpdates();
                bool succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                EndCommodityUpdates(succeeded);

                if (KamaSimtra.Settings.mRandomRendezvousMoodlet)
                {
                    Actor.BuffManager.AddElement(RandomUtil.GetRandomObjectFromList(sRandomBuffs), WoohooBuffs.sWoohooOrigin);
                }

                if (mMaster)
                {
                    CommonWoohoo.WoohooLocation location = CommonWoohoo.WoohooLocation.RabbitHole;

                    List<WoohooLocationControl> choices = CommonWoohoo.GetValidLocations(Actor.SimDescription);
                    if (choices.Count > 0)
                    {
                        location = RandomUtil.GetRandomObjectFromList(choices).Location;
                    }

                    CommonWoohoo.RunPostWoohoo(Actor, choice.CreatedSim, Target, CommonWoohoo.WoohooStyle.Safe, location, false);
                }

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

        private void WaitPeriodLoop(StateMachineClient smc, InteractionInstance.LoopData loopData)
        {
            if (mBegin)
            {
                Actor.AddExitReason(ExitReason.StageComplete);
            }
            else
            {
                Sim actor = LinkedInteractionInstance.InstanceActor;
                if ((actor == null) || (actor.HasBeenDestroyed))
                {
                    Actor.AddExitReason(ExitReason.StageComplete);
                }
                else if (!actor.InteractionQueue.HasInteraction(LinkedInteractionInstance))
                {
                    Actor.AddExitReason(ExitReason.StageComplete);
                }
            }
        } 

        public class Definition : InteractionDefinition<Sim, DaySpa, Rendezvous>
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, DaySpa target, InteractionObjectPair iop)
            {
                return Common.Localize("Rendezvous:MenuName", actor.IsFemale);
            }

            public override bool Test(Sim a, DaySpa target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!Skills.KamaSimtra.Settings.mShowRendezvousInteraction) return false;

                if (!a.SimDescription.TeenOrAbove)
                {
                    return false;
                }
                else if (a.SimDescription.Teen)
                {
                    if (!Woohooer.Settings.mAllowTeenWoohoo) return false;
                }

                if (a.FamilyFunds < KamaSimtra.Settings.mRendezvousCostPerLevel)
                {
                    greyedOutTooltipCallback = delegate
                    {
                        return Common.Localize("Rendezvous:Failure", a.IsFemale, new object[] { KamaSimtra.Settings.mRendezvousCostPerLevel });
                    };
                    return false;
                }

                return true;
            }
        }
    }
}
