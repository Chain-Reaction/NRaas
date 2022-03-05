using NRaas.CommonSpace.Helpers;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class RummageEx : TrashcanOutside.Rummage, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<TrashcanOutside, TrashcanOutside.Rummage.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<IOutdoorAcceptsGarbage, TrashcanOutside.Rummage.Definition>(Singleton);
        }

        public new void LoopDel(StateMachineClient smc, Interaction<Sim, IOutdoorAcceptsGarbage>.LoopData loopData)
        {
            int mLifeTime = (int)loopData.mLifeTime;
            if (Autonomous)
            {
                if (mLifeTime >= (TrashcanOutside.kRevealItemEveryXMinutes * TrashcanOutside.kRummageMaxItems))
                {
                    Actor.AddExitReason(ExitReason.Finished);
                }
            }
            else if ((mLifeTime % base.Target.RevealItemEveryXMinutes) == 0)
            {
                Target.GiveObject(mNetWorth, Actor);
                mNumItemsFound++;
                if (mNumItemsFound >= TrashcanOutside.kRummageMaxItems)
                {
                    Actor.AddExitReason(ExitReason.Finished);
                    if ((Target.LotCurrent != Actor.LotHome) && (Target.LotCurrent.Household != null))
                    {
                        Journalism journalism = OmniCareer.Career<Journalism>(Actor.Occupation);
                        LawEnforcement law = OmniCareer.Career<LawEnforcement>(Actor.Occupation);

                        Common.DebugNotify("RummageEx:LoopDel");

                        if ((journalism != null) && Actor.Occupation.CanInterview())
                        {
                            JournalismRummage(Actor, Target);
                        }
                        else if ((law != null) && Actor.Occupation.CanInterview())
                        {
                            LawEnforcementRummage(Actor, Target);
                        }
                    }
                }
            }
        }

        protected new void JournalismRummage(Sim rummager, IOutdoorAcceptsGarbage can)
        {
            Journalism job = OmniCareer.Career<Journalism>(rummager.Occupation);

            Common.DebugNotify("RummageEx:JournalismRummage");

            List<SimDescription> choices = new List<SimDescription>();
            foreach (SimDescription sim in Households.Humans(can.LotCurrent.Household))
            {
                if (sim.YoungAdultOrAbove && !job.SimsTrashScoped.Contains(sim))
                {
                    choices.Add(sim);
                }
            }
            if (choices.Count != 0x0)
            {
                SimDescription choice = RandomUtil.GetRandomObjectFromList(choices);

                Common.DebugNotify("JournalismRummage: " + choice.FullName);

                job.SimsTrashScoped.Add(choice);
                rummager.ShowTNSIfSelectable(Common.LocalizeEAString(rummager.IsFemale, "Gameplay/Objects/Miscellaneous/TrashcanOutside:RummageForInfo" + RandomUtil.GetInt(0x1, 0x3), new object[] { rummager, choice }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, rummager.ObjectId);
            }
        }

        protected new void LawEnforcementRummage(Sim rummager, IOutdoorAcceptsGarbage can)
        {
            LawEnforcement job = OmniCareer.Career<LawEnforcement>(rummager.Occupation);

            Common.DebugNotify("RummageEx:LawEnforcementRummage");

            List<SimDescription> choices = new List<SimDescription>();
            foreach (SimDescription sim in Households.Humans(can.LotCurrent.Household))
            {
                if (sim.YoungAdultOrAbove && !job.SimsInterviewed.Contains(sim))
                {
                    choices.Add(sim);
                }
            }
            if (choices.Count != 0x0)
            {
                SimDescription choice = RandomUtil.GetRandomObjectFromList(choices);

                Common.DebugNotify("LawEnforcementRummage: " + choice.FullName);

                job.SimsInterviewed.Add(choice);
                rummager.ShowTNSIfSelectable(Common.LocalizeEAString(rummager.IsFemale, "Gameplay/Objects/Miscellaneous/TrashcanOutside:RummageForInfo" + RandomUtil.GetInt(0x1, 0x3), new object[] { rummager, choice }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, rummager.ObjectId);
            }
        }

        public override bool Run()
        {
            try
            {
                if (!Target.RouteToCan(Actor, true))
                {
                    return false;
                }
                StateMachineClient stateMachine = Target.GetStateMachine(Actor, false);
                stateMachine.AddOneShotScriptEventHandler(0x65, Target.EventCallbackRemoveLid);
                stateMachine.AddOneShotScriptEventHandler(0x66, Target.EventCallbackAddLid);

                Trashcan target = Target as Trashcan;
                if (target != null)
                {
                    mLidGuid = target.SetupLidProp(stateMachine);
                }

                IFairyBoobyTrap trap = Target as IFairyBoobyTrap;
                if (((trap != null) && ((trap.BoobyTrapComponent != null) ? trap.BoobyTrapComponent.CanTriggerTrap(Actor.SimDescription) : false)) && trap.TriggerTrap(Actor))
                {
                    TrashcanOutside outsideCan = Target as TrashcanOutside;
                    if (outsideCan != null)
                    {
                        TrashPile.CreateFairyTrapTrashPiles(outsideCan);
                    }
                    return false;
                }

                StandardEntry();
                BeginCommodityUpdates();

                try
                {
                    if (!Autonomous)
                    {
                        Target.HasBeenRummagedByPlayer = true;
                    }
                    Target.SetRummagingSim(Actor);
                    Target.RummageBroadcasterCreate();
                    stateMachine.RequestState("x", "LoopRummage");
                    if ((!Autonomous && (HudController.InfoState != InfoState.Inventory)) && Actor.IsActiveSim)
                    {
                        HudController.SetInfoState(InfoState.Inventory);
                    }
                    mNetWorth = Target.LotCurrent.Household.NetWorth();
                    DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), LoopDel, stateMachine);
                    Target.RummageBroadcasterDestroy();
                    stateMachine.RequestState("x", "Exit");
                    if (!Autonomous)
                    {
                        HudModel.OpenObjectInventoryForOwner(null);
                    }
                }
                finally
                {
                    EndCommodityUpdates(true);
                }

                BuffInstance element = Actor.BuffManager.GetElement(BuffNames.Smelly);
                if (element != null)
                {
                    element.mBuffOrigin = Origin.FromDumpsterDiving;
                }
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

        public new class Definition : TrashcanOutside.Rummage.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new RummageEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, IOutdoorAcceptsGarbage target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
