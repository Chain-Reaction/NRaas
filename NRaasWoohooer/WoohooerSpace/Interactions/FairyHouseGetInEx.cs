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
    public class FairyHouseGetInEx : FairyHouse.GetIn, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<FairyHouse, FairyHouse.GetIn.Definition, Definition>(false);

            sOldSingleton = FairyHouse.GetIn.Singleton;
            FairyHouse.GetIn.Singleton = new Definition();
        }


        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<FairyHouse, FairyHouse.GetIn.Definition>(Singleton);
        }

        private void OnHideActorEx(StateMachineClient smc, IEvent evt)
        {
            if (mActorFairy != null)
            {
                mActorFairy.HideHumanAndShowTrueForm();
            }
            else
            {
                Actor.SetOpacity(0f, 0f);
            }
        }

        public override bool Run()
        {
            try
            {
                if (!Target.IsActorUsingMe(Actor))
                {
                    List<ISurface> objectsInRoom = Actor.LotCurrent.GetObjectsInRoom<ISurface>(Actor.RoomId);
                    if (!CarrySystem.PutDownOnNearestSurface(Actor, objectsInRoom, SurfaceType.Normal, false, false, true))
                    {
                        CarrySystem.PutDownOnFloor(Actor);
                    }

                    if (!Actor.RouteToSlotList(Target, Target.UseableSlots(Actor, false, true), out mSlotIndex))
                    {
                        return false;
                    }

                    Target.SetRoutingSlotInUse(mSlotIndex);
                    if (Actor.HasExitReason())
                    {
                        return false;
                    }

                    SetPriority(new InteractionPriority(InteractionPriorityLevel.High));
                    mActorFairy = Actor.SimDescription.OccultManager.GetOccultType(OccultTypes.Fairy) as OccultFairy;
                    
                    StandardEntry();
                    BeginCommodityUpdates();
                    EnterStateMachine("fairyhouseaccess", "Enter", "x");
                    SetParameter("Slot", FairyHouse.kSlotToCompass[mSlotIndex]);

                    if (mActorFairy != null)
                    {
                        mActorFairy.AttachTrueFairyFormToAnimation(mCurrentStateMachine);
                    }
                    
                    AddOneShotScriptEventHandler(0x65, OnHideActorEx);
                    AnimateSim("Fly In");

                    if (mActorFairy != null)
                    {
                        mActorFairy.HideFairyTrueFormHardStop();
                    }
                    
                    FairyHouse.FairyHousePosture posture = new FairyHouse.FairyHousePosture(Actor, Target, mCurrentStateMachine);
                    Actor.Posture = posture;
                    Actor.BridgeOrigin = posture.Idle();
                    PlumbBob.Reparent();

                    if (!Target.IsLightOn())
                    {
                        Target.TurnOnLight();
                    }
                    EndCommodityUpdates(true);
                    StandardExit(false, false);
                }
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

        public new class Definition : FairyHouse.GetIn.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new FairyHouseGetInEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, FairyHouse target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, FairyHouse target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if ((!InteractionsEx.HasInteraction<FairyHouseWoohoo.BaseDefinition>(a)) &&
                    (!InteractionsEx.HasInteraction<FairyHouseWoohoo.ProxyDefinition>(a)))
                {
                    if (!target.IsAllowedSim(a))
                    {
                        return false;
                    }
                }

                if (target.IsActorUsingMe(a))
                {
                    return false;
                }
                return true;
            }
        }
    }
}
