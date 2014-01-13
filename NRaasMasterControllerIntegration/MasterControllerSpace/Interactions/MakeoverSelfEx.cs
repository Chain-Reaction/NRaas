using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class MakeoverSelfEx : StylingStation.MakeoverSelf, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<StylingStation, StylingStation.MakeoverSelf.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<StylingStation, StylingStation.MakeoverSelf.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (!Actor.RouteToSlotAndCheckInUse(Target, StylingStation.kPlatformRoutingSlot))
                {
                    return false;
                }
                StandardEntry();
                Styling.PreMakeover(Actor, Actor);
                EnterStateMachine("StylingStation", "Enter", "x", "stylistStation");
                AddSynchronousOneShotScriptEventHandler(0x384, new SacsEventHandler(SnapSimToPlatform));
                AddSynchronousOneShotScriptEventHandler(0x385, new SacsEventHandler(SnapSimToGround));
                AnimateSim("Get On Platform");
                if (Actor.HasExitReason())
                {
                    AnimateSim("Get Off Platform");
                    AnimateSim("Exit");
                    StandardExit();
                    return false;
                }
                BeginCommodityUpdates();
                AnimateSim("Customer Alone Loop");
                float duration = Actor.IsNPC ? StylingStation.kNPCMakeoverDurationInMinutes : StylingStation.kMakeoverDurationInMinutes;
                bool succeeded = DoTimedLoop(duration);
                if (succeeded)
                {
                    Styling.MakeoverOutcome success = Styling.MakeoverOutcome.Success;
                    bool isSelectable = Actor.IsSelectable;
                    if (!isSelectable)
                    {
                        Sim.SwitchOutfitHelper switchOutfitHelper = null;
                        Styling.LoadMakeoverOutfitForClothesSpin(Actor, false, Autonomous, ref switchOutfitHelper);
                        mSwitchOutfitHelper = switchOutfitHelper;
                    }
                    Animate("x", "Gussy Anims Done");
                    if (!isSelectable && (mSwitchOutfitHelper != null))
                    {
                        mSwitchOutfitHelper.Wait(true);
                        mSwitchOutfitHelper.AddScriptEventHandler(this);
                    }
                    if (isSelectable)
                    {
                        bool tookSemaphore = mTookSemaphore;
                        isSelectable = GetMakeoverEx.DisplayCAS(Actor, null, ref tookSemaphore, false);
                        mTookSemaphore = tookSemaphore;
                    }
                    SkillLevel expert = SkillLevel.expert;
                    SetParameter("doClothesSpin", !isSelectable);
                    SetParameter("customerReactionType", expert);
                    Animate("x", "Customer Reaction");
                    ReleaseSemaphore();
                    Styling.PostMakeover(Actor, Actor, success, false, expert, isSelectable, isSelectable, null);
                }
                if (succeeded)
                {
                    Actor.SkillManager.AddElement(SkillNames.Styling);
                }
                EndCommodityUpdates(succeeded);
                AnimateSim("Get Off Platform");
                AnimateSim("Exit");
                StandardExit();
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

        public new class Definition : StylingStation.MakeoverSelf.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new MakeoverSelfEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, StylingStation target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
