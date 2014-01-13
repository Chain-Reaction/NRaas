using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Spawners;
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
    public class CheatTheSystemEx : ArcadeClawMachine.CheatTheSystem, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<ArcadeClawMachine, ArcadeClawMachine.CheatTheSystem.Definition, Definition>(false);

            sOldSingleton = ArcadeClawMachine.CheatTheSystem.Singleton;
            ArcadeClawMachine.CheatTheSystem.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<ArcadeClawMachine, ArcadeClawMachine.CheatTheSystem.Definition>(ArcadeClawMachine.CheatTheSystem.Singleton);
        }

        private void OnShowActorEx(StateMachineClient smc, IEvent evt)
        {
            if (mFairy != null)
            {
                OccultFairyEx.ShowHumanAndHideTrueForm(mFairy);
            }
            Actor.AddExitReason(ExitReason.Finished);
        }

        public override bool Run()
        {
            try
            {
                if (!Actor.RouteToSlotAndCheckInUse(Target, Target.GetRoutingSlot(Actor)))
                {
                    return false;
                }
                
                bool paramValue = RandomUtil.RandomChance(ArcadeClawMachine.kPercentChanceOfCheatingTheSystem);
                bool isRare = false;
                bool flag3 = false;
                mPrize = Target.DecidePrize(Actor, out isRare);
                if (!paramValue)
                {
                    flag3 = true;
                }

                bool flag4 = mPrize is StuffedAnimal;
                EnterStateMachine("arcadeclawmachine", "enter", "x");
                mCurrentStateMachine.AddPersistentScriptEventHandler(0x65, OnHideActor);
                mCurrentStateMachine.AddPersistentScriptEventHandler(0x66, OnShowActorEx);
                mCurrentStateMachine.AddPersistentScriptEventHandler(0x79, ShowPrize);
                mCurrentStateMachine.AddPersistentScriptEventHandler(0x7a, HidePrize);
                mFairy = Actor.SimDescription.OccultManager.GetOccultType(OccultTypes.Fairy) as OccultFairy;
                mFairy.AttachTrueFairyFormToAnimation(mCurrentStateMachine);
                StandardEntry();
                BeginCommodityUpdates();
                SetActor("ClawMachine", Target);
                if (flag4)
                {
                    SetActor("teddybear", mPrize);
                }
                else
                {
                    SetActor("prize", mPrize);
                }

                SetParameter("ArcadeSkill", Actor.SkillManager.GetSkillLevel(SkillNames.ArcadeMachine));
                SetParameter("PickedUpPrize", paramValue);
                SetParameter("RarePrize", isRare);
                SetParameter("IsTeddyBear", flag4);
                SetParameter("PickedUpPrize", flag3);
                mPrize.AddToWorld();
                if (paramValue)
                {
                    AnimateSim("fairy success");
                    if ((mPrize != null) && (Actor.Inventory != null))
                    {
                        if (mPrize is Gem)
                        {
                            Gem prize = mPrize as Gem;
                            string randomStringFromList = RandomUtil.GetRandomStringFromList(ArcadeClawMachine.kCutTypes);
                            Gem gem2 = Gem.MakeCutGem(prize.Guid, randomStringFromList, false);
                            if (Actor.Inventory.TryToAdd(gem2))
                            {
                                prize.SetHiddenFlags(HiddenFlags.Nothing);
                                Actor.ShowTNSIfSelectable(TNSNames.ArcadeClawMachinePrizeGem, Actor, Target, new object[] { gem2.GemName });
                                EventTracker.SendEvent(EventTypeId.kUseClawMachine, Actor, gem2);
                                (Actor.SkillManager.AddElement(SkillNames.Collecting) as Collecting).UpdateGemRecords(gem2.Guid, (float)gem2.Value, gem2.LocalizedCutName);
                            }
                            prize.RemoveFromWorld();
                        }
                        else if (Inventories.TryToMove(mPrize, Actor))
                        {
                            mPrize.SetHiddenFlags(HiddenFlags.Nothing);
                            Actor.ShowTNSIfSelectable(TNSNames.ArcadeClawMachinePrize, Actor, Target, new object[] { Actor, mPrize });
                            EventTracker.SendEvent(EventTypeId.kUseClawMachine, Actor, mPrize);
                            mPrize = null;
                        }
                    }
                }
                else
                {
                    AnimateSim("fairy fail");
                    EventTracker.SendEvent(EventTypeId.kUseClawMachine, Actor, null);
                }

                EndCommodityUpdates(true);
                StandardExit();
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

        public new class Definition : ArcadeClawMachine.CheatTheSystem.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CheatTheSystemEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, ArcadeClawMachine target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
