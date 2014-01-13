using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using NRaas.WoohooerSpace.Skills;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class CyberWoohoo : Computer.ChatRandom, Common.IPreLoad, Common.IAddInteraction
    {
        static new InteractionDefinition Singleton = new Definition();
        static InteractionDefinition RandomSingleton = new Definition(true);

        int mClimaxChances = 0;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Woohooer.InjectAndReset<Computer, Computer.ChatRandom.Definition, Definition>(true);
            if (tuning != null)
            {
                tuning.Availability.Children = false;
            }
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Computer>(Singleton);
            interactions.AddNoDupTest<Computer>(RandomSingleton);
        }

        protected static void GetPotentials(Sim actor, Computer target, bool autonomous, bool ignoreGender, List<Sim> unknown, List<Sim> known)
        {
            foreach (Sim sim in LotManager.Actors)
            {
                if (sim == null) continue;

                if (actor.Household == sim.Household) continue;

                //msg += Common.NewLine + sim.FullName;

                string reason;
                GreyedOutTooltipCallback callback = null;
                if (!CommonSocials.CanGetRomantic(sim, actor, autonomous, true, true, ref callback, out reason))
                {
                    //msg += Common.NewLine + " " + callback();
                    continue;
                }

                if (!ignoreGender)
                {
                    if (!CommonSocials.CheckAutonomousGenderPreference(sim.SimDescription, actor.SimDescription))
                    {
                        //msg += Common.NewLine + " Not Gender Preference";
                        continue;
                    }
                }

                if (!target.CanSimBeChattedWith(actor, sim))
                {
                    //msg += Common.NewLine + " Not CanBeChatted";
                    continue;
                }

                if (unknown != null)
                {
                    int score = ScoringLookup.GetScore("LikeCyberWoohoo", sim.SimDescription) + KamaSimtra.Settings.mCyberWoohooBaseChanceScoring;
                    if (score < 0)
                    {
                        //msg += Common.NewLine + " Score Fail " + score;
                        continue;
                    }
                }

                if (Relationship.Get(actor, sim, false) == null)
                {
                    if (unknown != null)
                    {
                        unknown.Add(sim);
                    }
                }
                else
                {
                    known.Add(sim);
                }
            }
        }

        public override bool Run()
        {
            string msg = "Run" + Common.NewLine;

            try
            {
                StandardEntry();
                if (!Target.StartComputing(this, SurfaceHeight.Table, true))
                {
                    StandardExit();
                    return false;
                }

                timeTillLearn = RandomUtil.RandomFloatGaussianDistribution(Target.ComputerTuning.ChatLearnSomethingFrequencyStart, Target.ComputerTuning.ChatLearnSomethingFrequencyEnd);
                Target.StartVideo(Computer.VideoType.Chat);
                if (CheckForCancelAndCleanup())
                {
                    Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                    return false;
                }

                bool ignoreGender = RandomUtil.RandomChance(KamaSimtra.Settings.mCyberWoohooChanceOfMisunderstanding);

                simToChat = GetSelectedObject() as Sim;
                if (simToChat == null)
                {
                    //msg += Common.NewLine + "CyberWoohoo";

                    List<Sim> unknown = new List<Sim>();
                    List<Sim> known = new List<Sim>();
                    GetPotentials(Actor, Target, Autonomous, ignoreGender, unknown, known);

                    //Common.DebugStackLog(msg);

                    if ((unknown.Count > 0x0) && Actor.IsSelectable)
                    {
                        simToChat = RandomUtil.GetRandomObjectFromList(unknown);
                    }
                    else if (known.Count > 0x0)
                    {
                        simToChat = RandomUtil.GetRandomObjectFromList(known);
                    }
                }

                bool succeeded = false;
                if (simToChat == null)
                {
                    Actor.ShowTNSIfSelectable(Common.Localize("CyberWoohoo:NoOne", Actor.IsFemale), StyledNotification.NotificationStyle.kSimTalking);
                }
                else
                {
                    Common.DebugNotify("CyberWoohoo: " + simToChat.FullName, Actor);

                    BeginCommodityUpdates();
                    AnimateSim("GenericTyping");
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), LoopDel, null);
                    EndCommodityUpdates(succeeded);

                    if (Woohooer.Settings.mApplyBuffs)
                    {
                        if (mClimaxChances <= 2)
                        {
                            Actor.BuffManager.AddElement(KamaSimtra.sPremature, WoohooBuffs.sWoohooOrigin);
                        }

                        int score = ScoringLookup.GetScore("LikeCyberWoohoo", Actor.SimDescription) + KamaSimtra.Settings.mCyberWoohooBaseChanceScoring;
                        if (score > 0)
                        {
                            Actor.BuffManager.RemoveElement(KamaSimtra.sDislikeCyberWoohoo);
                            Actor.BuffManager.AddElement(KamaSimtra.sLikeCyberWoohoo, WoohooBuffs.sWoohooOrigin);
                        }
                        else if (score < 0)
                        {
                            Actor.BuffManager.RemoveElement(KamaSimtra.sLikeCyberWoohoo);
                            Actor.BuffManager.AddElement(KamaSimtra.sDislikeCyberWoohoo, WoohooBuffs.sWoohooOrigin);
                        }
                    }
                }

                Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                StandardExit();
                return succeeded;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, msg, e);
                return false;
            }
        }

        private new void LoopDel(StateMachineClient smc, InteractionInstance.LoopData loopData)
        {
            try
            {
                bool flag = false;
                if (simToChat == null)
                {
                    flag = true;
                }
                else
                {
                    SimDescription simDescription = simToChat.SimDescription;
                    if ((simDescription == null) || !simDescription.IsValidDescription)
                    {
                        flag = true;
                    }
                }

                if (flag)
                {
                    Actor.AddExitReason(ExitReason.Finished);
                }
                else
                {
                    int lifeTime = (int)loopData.mLifeTime;
                    if (Actor.HasTrait(TraitNames.AntiTV) && (lifeTime > Computer.kTechnophobeTraitMaximumChatTime))
                    {
                        Actor.AddExitReason(ExitReason.Finished);
                    }

                    int chatRelationshipIncreaseEveryXMinutes = Target.ComputerTuning.ChatRelationshipIncreaseEveryXMinutes;
                    if (chatRelationshipIncreaseEveryXMinutes != 0)
                    {
                        if ((lifeTime / Target.ComputerTuning.ChatRelationshipIncreaseEveryXMinutes) > RelationshipIncreases)
                        {
                            RelationshipIncreases++;
                            Target.ChatUpdate(Actor, simToChat, Target);
                        }
                    }

                    int cyberWoohooChanceToClimaxEveryXMinutes = KamaSimtra.Settings.mCyberWoohooChanceToClimaxEveryXMinutes;
                    if (cyberWoohooChanceToClimaxEveryXMinutes != 0)
                    {
                        if ((lifeTime / cyberWoohooChanceToClimaxEveryXMinutes) > mClimaxChances)
                        {
                            mClimaxChances++;

                            if (RandomUtil.RandomChance(KamaSimtra.Settings.mCyberWoohooChanceToClimax * mClimaxChances))
                            {
                                CommonWoohoo.RunPostWoohoo(Actor, simToChat, Target, CommonWoohoo.WoohooStyle.Safe, CommonWoohoo.WoohooLocation.Computer, false);

                                Actor.AddExitReason(ExitReason.Finished);
                            }
                        }
                    }

                    if (timeTillLearn < lifeTime)
                    {
                        Relationship relation = Relationship.Get(Actor, simToChat, true);
                        if (relation != null)
                        {
                            relation.ConsiderLearningAboutTargetSim(Actor, simToChat, Target.ComputerTuning.ChatIntimacylevel);
                        }

                        timeTillLearn = lifeTime + RandomUtil.RandomFloatGaussianDistribution(Target.ComputerTuning.ChatLearnSomethingFrequencyStart, Target.ComputerTuning.ChatLearnSomethingFrequencyEnd);
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                Actor.AddExitReason(ExitReason.Finished);
            }
        }

        public new class Definition : Computer.ChatRandom.Definition
        {
            bool mRandom;

            public Definition()
            { }
            public Definition(bool random)
            {
                mRandom = random;
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CyberWoohoo();
                na.Init(ref parameters);
                return na;
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                if (!mRandom)
                {
                    Sim actor = parameters.Actor as Sim;
                    Computer target = parameters.Target as Computer;

                    List<Sim> known = new List<Sim>();
                    GetPotentials(actor, target, false, false, null, known);

                    NumSelectableRows = 0x1;
                    PopulateSimPicker(ref parameters, out listObjs, out headers, known, true);
                }
                else
                {
                    base.PopulatePieMenuPicker(ref parameters, out listObjs, out headers, out NumSelectableRows);
                }
            }

            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                if (mRandom)
                {
                    return Common.Localize("CyberWoohooRandom:MenuName", actor.IsFemale);
                }
                else
                {
                    return Common.Localize("CyberWoohooSpecific:MenuName", actor.IsFemale);
                }
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                using (WoohooTuningControl control = new WoohooTuningControl(parameters.InteractionObjectPair.Tuning, Woohooer.Settings.mAllowTeenWoohoo))
                {
                    return base.Test(ref parameters, ref greyedOutTooltipCallback);
                }
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous)
                {
                    if (!mRandom)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Not Random");
                        return false;
                    }

                    if (!Woohooer.Settings.mAutonomousComputer)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("No Autonomous");
                        return false;
                    }

                    if (ScoringLookup.GetScore("LikeCyberWoohoo", a.SimDescription) < 0)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Score Fail");
                        return false;
                    }

                    List<Sim> unknown = new List<Sim>();
                    List<Sim> known = new List<Sim>();
                    GetPotentials(a, target, isAutonomous, false, unknown, known);

                    if ((unknown.Count + known.Count) == 0)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("No Choices");
                        return false;
                    }
                }
                else
                {
                    if (!KamaSimtra.Settings.mShowCyberWoohooInteraction)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("User Hidden");
                        return false;
                    }
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }

        public class LocationControl : WoohooLocationControl
        {
            public override CommonWoohoo.WoohooLocation Location
            {
                get { return CommonWoohoo.WoohooLocation.Computer; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is Computer;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return false;
            }

            public override bool HasLocation(Lot lot)
            {
                return false;// return (lot.CountObjects<Computer>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                return Woohooer.Settings.mAutonomousComputer;
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                return null;
            }

            public override InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style)
            {
                return null;
            }
        }
    }
}
