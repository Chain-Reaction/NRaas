using NRaas.CommonSpace.Helpers;
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
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class FairyHouseGetOutEx : FairyHouse.GetOut, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<FairyHouse, FairyHouse.GetOut.Definition, Definition>(false);

            sOldSingleton = FairyHouse.GetOut.Singleton;
            FairyHouse.GetOut.Singleton = new Definition();
        }


        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<FairyHouse, FairyHouse.GetOut.Definition>(Singleton);
        }

        private void OnShowActorEx(StateMachineClient smc, IEvent evt)
        {
            if (mActorFairy != null)
            {
                OccultFairyEx.ShowHumanAndHideTrueForm(mActorFairy);
            }
            else
            {
                Actor.SetOpacity(1f, 0f);
            }
        }

        public override bool Run()
        {
            try
            {
                RoutePlanResult result = Actor.CreateRoute().PlanToSlot(Target, Target.UseableSlots(Actor, true, true));
                if (!result.Succeeded())
                {
                    return false;
                }

                mSlotIndex = FairyHouse.kSlotToIndex[(Slot)result.mDestSlotNameHash];
                Target.SetRoutingSlotInUse(mSlotIndex);
                if (Actor.IsActiveSim)
                {
                    PlumbBob.HidePlumbBob();
                }

                StandardEntry(false);
                BeginCommodityUpdates();
                mActorFairy = Actor.SimDescription.OccultManager.GetOccultType(OccultTypes.Fairy) as OccultFairy;

                mCurrentStateMachine = Actor.Posture.CurrentStateMachine;
                AddOneShotScriptEventHandler(0x66, OnShowActorEx);
                SetParameter("Slot", FairyHouse.kSlotToCompass[mSlotIndex]);

                if (mActorFairy != null)
                {
                    mActorFairy.FairyTrueForm.SetPosition(Target.GetPositionOfSlot((Slot)result.mDestSlotNameHash));
                    mActorFairy.FairyTrueForm.SetForward(Target.GetForwardOfSlot((Slot)result.mDestSlotNameHash));
                }

                if (mActorFairy != null)
                {
                    mActorFairy.AttachTrueFairyFormToAnimation(mCurrentStateMachine);
                }

                AnimateSim("Fly Out");
                AnimateSim("Exit");
                EndCommodityUpdates(true);
                StandardExit();

                if (Target.IsLightOn() && (Target.ActorsUsingMe.Count == 0x0))
                {
                    Target.TurnOffLight();
                }

                FairyHouse.FairyHousePosture posture = Actor.Posture as FairyHouse.FairyHousePosture;
                if (posture != null)
                {
                    posture.CancelPosture(Actor);
                }

                Actor.PopPosture();
                Actor.BridgeOrigin = Actor.Posture.Idle();
                PlumbBob.Reparent();
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

        public new class Definition : FairyHouse.GetOut.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new FairyHouseGetOutEx();
                na.Init(ref parameters);
                return na;
            }

            public override string  GetInteractionName(Sim actor, FairyHouse target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
