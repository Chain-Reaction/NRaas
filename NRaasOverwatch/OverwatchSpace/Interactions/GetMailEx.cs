using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Interactions;
using NRaas.OverwatchSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Interactions
{
    public class GetMailEx : Mailbox.GetMail, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;
        static InteractionDefinition sOldFakeSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Mailbox, Mailbox.GetMail.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();

            sOldFakeSingleton = FakeSingleton;
            FakeSingleton = new FakeDefinition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Mailbox, Mailbox.GetMail.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                MailboxDoor doorOfSim = Target.GetDoorOfSim(Actor);
                if ((doorOfSim == null) || !Actor.RouteToSlotAndCheckInUse(Target, doorOfSim.Slot))
                {
                    return false;
                }

                bool flag = false;
                StandardEntry();
                EnterStateMachine("mailbox", "Enter", "x", "mailbox");
                SetActor("wallMailboxes", Target);
                SetParameter("IsWallMailbox", Target.IsWallMailboxVariant);
                doorOfSim.SetAnimParams(mCurrentStateMachine);
                string instanceName = (Target.Inventory.AmountIn<Bill>() > 0x0) ? "Bill" : "Package";
                mObjectInHand = GlobalFunctions.CreateObjectOutOfWorld(instanceName, "Sims3.Gameplay.Core.Null", null) as GameObject;
                SetActor("bills", mObjectInHand);
                bool flagUp = Target.FlagUp;
                SetParameter("IsFlagAlreadyUp", flagUp);
                doorOfSim.SetProductVersionForDoorAnim(mCurrentStateMachine);
                if (((Target.BoobyTrapComponent != null) ? Target.BoobyTrapComponent.CanTriggerTrap(Actor.SimDescription) : false) && Target.TriggerTrap(Actor))
                {
                    EnterState("x", "Get Mail");
                }
                else
                {
                    AnimateSim("Get Mail");
                    flag = MailboxEx.GrabMail(Target, Actor);
                    doorOfSim.UnsetProductVersionForDoorAnim(mCurrentStateMachine);
                    AnimateSim("Put Flag Down");
                }

                RemoveActor("bills");
                mObjectInHand.Destroy();
                mObjectInHand = null;
                AnimateSim("Exit");
                StandardExit();
                return flag;
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

        public new class Definition : Mailbox.GetMail.Definition
        {
            public Definition()
            { }
            public Definition(Mailbox.GetMailInteractionApartmentUnitData data)
                : base(data)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GetMailEx();
                na.Init(ref parameters);
                return na;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Mailbox target, List<InteractionObjectPair> results)
            {
                foreach (Mailbox.GetMailInteractionApartmentUnitData data in target.GetUnitData())
                {
                    results.Add(new InteractionObjectPair(new Definition(data), target));
                }
            }

            public override string GetInteractionName(Sim actor, Mailbox target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Mailbox target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (a.LotHome == target.mLotCurrent)
                    {
                        if (!target.FlagUp)
                        {
                            return false;
                        }

                        // Custom
                        if (mUnitData == null)
                        {
                            return false;
                        }

                        if (!mUnitData.mIsVirtual)
                        {
                            return true;
                        }

                        greyedOutTooltipCallback = delegate
                        {
                            return Mailbox.LocalizeString("WrongUnitGreyedOutTooltip", new object[0x0]);
                        };
                    }
                    return false;
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }

        public class FakeDefinition : Mailbox.GetMail.FakeGetMailDefinition
        {
            public FakeDefinition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GetMailEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Mailbox target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldFakeSingleton, target));
            }
        }
    }
}
