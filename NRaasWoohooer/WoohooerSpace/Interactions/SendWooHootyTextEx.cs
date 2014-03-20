using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    class SendWooHootyTextEx : Phone.SendWooHootyText, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;        

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Phone, Phone.SendWooHootyText.Definition, Definition>(false);            
            if (tuning != null)
            {
                tuning.SetFlags(InteractionTuning.FlagField.DisallowAutonomous, false);
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Phone, Phone.SendWooHootyText.Definition>(Singleton);
        }

        public override bool SimCanTextWithActor(SimDescription other)
        {    
            if (base.Actor == null || other == null)
            {
                return false;
            }

            if (other.ChildOrBelow)
            {
                return false;
            }            
            
            return SimCanTextWithActorEx(base.Actor, other, false);
        }

        public static bool SimCanTextWithActorEx(Sim actor, IMiniSimDescription other, bool foreignText)
        {            
            if (foreignText)
            {
                return false;
            }
            SimDescription description = other as SimDescription;
            Sim target = (description != null) ? description.CreatedSim : null;
            if (target == null)
            {
                return false;
            }
            if (actor.LotCurrent == target.LotCurrent)
            {
                return false;
            }
            if (actor.SimDescription.IsEP11Bot)
            {
                if ((actor.TraitManager == null) || !actor.TraitManager.HasElement(TraitNames.CapacityToLoveChip))
                {
                    return false;
                }

                if (description.IsEP11Bot && (description.CreatedSim.TraitManager == null || !description.CreatedSim.TraitManager.HasElement(TraitNames.CapacityToLoveChip)))
                {
                    return false;
                }   
            }            
            return true;
        }

        private Phone.Call.ConversationBehavior onCallConnectedBase()
        {        
            if (base.BalloonIconData != null)
            {
                base.Actor.ThoughtBalloonManager.ShowBalloon(base.BalloonIconData);
            }
            base.Actor.SkillManager.AddElement(SkillNames.SocialNetworking);
            if (base.Actor.IsSelectable)
            {
                Tutorialette.TriggerLesson(Lessons.SocialNetworking, base.Actor);
            }
            base.AnimateSim(kAnimStates[(int)base.AnimState]);
            return Phone.Call.ConversationBehavior.NoBehavior;
        }

        public override Phone.Call.ConversationBehavior OnCallConnected()
        {        
            Phone.Call.ConversationBehavior behavior = this.onCallConnectedBase();            
            base.mProceed = false;

            float gate = 0;
            if (base.Autonomous || Woohooer.Settings.mLikingGateForUserDirected)
            {
                gate = Woohooer.Settings.mLikingGatingForAutonomousWoohoo[PersistedSettings.GetSpeciesIndex(base.Actor)];

                if (gate > Phone.kLTRThresholdForInviteOverMax)
                {
                    gate = Phone.kLTRThresholdForInviteOverMax;
                }

            }
            
            return this.DetermineOutcomeForInvitationEx("InviteOver", gate, Phone.kLTRThresholdForInviteOverMax, (gate == 0 ? 100f : Phone.kChanceAtMinForInviteOver), 100f, Phone.kLTRHitForFailedInviteOver, new Phone.Call.CustomAcceptanceTestDelegate(this.CustomAcceptanceTest), base.mOtherSimDesc as SimDescription, out base.mProceed);
        }

        public Phone.Call.ConversationBehavior DetermineOutcomeForInvitationEx(string localizationPrefix, float ltrThresholdMin, float ltrThresholdMax, float chanceAtMin, float chanceAtMax, float ltrHitForFailure, CustomAcceptanceTestDelegate customAcceptanceTest, SimDescription simToCall, out bool bProceed)
        {
            ConversationBehavior behavior = base.DetermineCallOutcome(localizationPrefix, ltrThresholdMin, ltrThresholdMax, chanceAtMin, chanceAtMax, ltrHitForFailure, false, simToCall, false);
            bProceed = false;
            if (behavior != ConversationBehavior.Chat)
            {
                this.ShowDialog("Rejected");
                return behavior;
            }
            AcceptanceTestResult dontCare = AcceptanceTestResult.DontCare;
            if (customAcceptanceTest != null)
            {
                dontCare = customAcceptanceTest(simToCall);
            }
            switch (dontCare)
            {
                case AcceptanceTestResult.InProgress:
                    this.ShowDialog("InProgress");
                    break;

                case AcceptanceTestResult.ForceReject:
                    this.ShowDialog("Rejected");
                    return ConversationBehavior.ExpressDisappointment;

                default:
                    this.ShowDialog("Accepted");
                    bProceed = true;
                    break;
            }
            return ConversationBehavior.ExpressSatisfaction;
        }

        private void ShowDialog(string messageType)
        {
            if(base.Actor.IsSelectable)
            {
                SimpleMessageDialog.Show(this.GetInteractionName(), LocalizeCallString("InviteOver", messageType, new object[0]), ModalDialog.PauseMode.PauseSimulator);
            }
        }

        public new Phone.Call.AcceptanceTestResult CustomAcceptanceTest(SimDescription simDescription)
        {            
            if (!RentScheduler.IsAllowedOnLotNow(simDescription, base.Actor.LotCurrent) && !RentScheduler.InviteToLotNow(base.Actor, simDescription, base.Actor.LotCurrent))
            {
                return Phone.Call.AcceptanceTestResult.ForceReject;
            }

            Sim createdSim = simDescription.CreatedSim;
            if ((createdSim != null) && (GroupingSituation.ShouldSoftReject(base.Actor, createdSim) || GroupingSituation.ShouldHardReject(base.Actor, createdSim)))
            {
                return Phone.Call.AcceptanceTestResult.ForceReject;
            }
            
            GreyedOutTooltipCallback greyedOutTooltipCallback = null;
            ActiveTopic topic = null;            
            if(!SimWoohoo.PublicTest(base.Actor, createdSim, topic, base.Autonomous, ref greyedOutTooltipCallback))
            {                
                return Phone.Call.AcceptanceTestResult.ForceReject;
            }

            return Phone.Call.AcceptanceTestResult.DontCare;
        }

        public override void OnCallFinished()
        {            
            if (base.mProceed && base.mOtherSimDesc != null)
            {
                Lot lotHome = base.Actor.LotHome;
                if (lotHome != null)
                {
                    this.GetSimToLotEx(base.mOtherSimDesc as SimDescription, lotHome);
                }
            }
        }

        public void GetSimToLotEx(SimDescription simDesc, Lot actorHome)
        {            
            Sim createdSim = simDesc.CreatedSim;
            if (createdSim == null)
            {
                createdSim = simDesc.Instantiate(actorHome);
            }
            if (createdSim != null)
            {
                bool flag = false;
                if (createdSim.Household == base.Actor.Household)
                {
                    GoToLot entry = GoToLot.Singleton.CreateInstanceWithCallbacks(actorHome, createdSim, base.GetPriority(), false, true, null, new Sims3.Gameplay.Autonomy.Callback(this.GoToLotSuccessEx), null) as GoToLot;
                    flag = createdSim.InteractionQueue.Add(entry);
                }
                else
                {
                    createdSim.SocialComponent.SetInvitedOver(actorHome);
                    if (!actorHome.IsBaseCampLotType)
                    {
                        Sims3.Gameplay.Core.VisitLot lot2 = Sims3.Gameplay.Core.VisitLot.Singleton.CreateInstanceWithCallbacks(actorHome, createdSim, base.GetPriority(), false, true, null, new Sims3.Gameplay.Autonomy.Callback(this.GoToLotSuccessEx), null) as Sims3.Gameplay.Core.VisitLot;
                        flag = createdSim.InteractionQueue.Add(lot2);
                    }
                    else
                    {
                        GoToLot lot3 = GoToLot.Singleton.CreateInstanceWithCallbacks(actorHome, createdSim, base.GetPriority(), false, true, null, new Sims3.Gameplay.Autonomy.Callback(this.GoToLotSuccessEx), null) as GoToLot;
                        flag = createdSim.InteractionQueue.Add(lot3);
                    }
                }
                if (flag)
                {
                    GroupingSituation situationOfType = createdSim.GetSituationOfType<GroupingSituation>();
                    if (situationOfType != null)
                    {
                        Sim leader = situationOfType.Leader;
                        if ((leader == null) || leader.IsNPC)
                        {
                            situationOfType.LeaveGroup(createdSim);
                        }
                    }
                    EventTracker.SendEvent(EventTypeId.kInvitedSimOver, base.Actor, createdSim);
                }
                else
                {
                    if (base.Actor.IsSelectable)
                    {
                        ShowInviteFailedDialog(this.GetInteractionName(), createdSim);
                    }
                }
            }
        }

        public void GoToLotSuccessEx(Sim sim, float f)
        {
            if (base.Actor != null)
            {
                Relationship relationship = Relationship.Get(base.Actor.SimDescription, base.mOtherSimDesc as SimDescription, false);
                if (relationship != null)
                {
                    relationship.STC.Set(base.Actor, sim, CommodityTypes.Amorous, 500f);
                    base.Actor.InteractionQueue.CancelAllInteractions();
                    while (base.Actor.CurrentInteraction != null)
                    {
                        Common.Sleep(5);
                    }                    
                    
                    base.Actor.GreetSimOnMyLotIfPossible(sim);
                    new CommonWoohoo.PushWoohoo(base.Actor, sim, base.Autonomous, CommonWoohoo.WoohooStyle.Safe);
                }
            }
        }

        public override string GetInteractionName()
        {            
            if(base.mOtherSimDesc != null)
            {
                return Phone.TextingBase.LocalizeTextingString(base.mOtherSimDesc.IsFemale, "SendWooHootyTextTo", new object[] { base.mOtherSimDesc});
            }
            
            return Phone.TextingBase.LocalizeTextingString("SendWooHootyText", new object[0]);            
        }

        public new class Definition : Phone.Call.CallDefinition<Phone.SendWooHootyText>
        {
            public override string GetInteractionName(Sim actor, Phone target, InteractionObjectPair iop)
            {
                return Phone.TextingBase.LocalizeTextingString("SendWooHootyText", new object[0]) + Localization.Ellipsis;
            }

            public override string[] GetPath(bool isFemale)
            {                
                return new string[] { (Phone.LocalizeString("SocialNetworking", new object[0]) + Localization.Ellipsis), (Phone.TextingBase.LocalizeTextingString("GroupName", new object[0]) + Localization.Ellipsis) };
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SendWooHootyTextEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim actor, Phone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {                               
                if (base.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback))
                {
                    if (isAutonomous)
                    {
                        if (!Woohooer.Settings.mWoohootyTextAutonomous[PersistedSettings.GetSpeciesIndex(actor)])
                        {                            
                            return false;
                        }
                    }

                    if (!actor.IsAtHome)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(TravelUtil.LocalizeString(actor.IsFemale, "NotAtHome", new object[] { actor }));
                        return false;
                    }

                    if (!base.CanSimInviteOver(actor, isAutonomous) || !base.CanInviteOverToLot(actor.LotCurrent, isAutonomous))
                    {                        
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(TravelUtil.LocalizeString(actor.IsFemale, "CannotInviteOver", new object[] { actor }));
                        return false;
                    }                    

                    if (actor.SimDescription.IsEP11Bot)
                    {
                        if ((actor.TraitManager == null) || !actor.TraitManager.HasElement(TraitNames.CapacityToLoveChip))
                        {
                            return false;
                        }
                    }

                    bool flag = false;
                    bool flag2 = false;
                    if (actor.SimDescription.Teen && Woohooer.Settings.AllowTeen(true))
                    {
                        flag = true;
                    }

                    if (actor.SimDescription.Teen && Woohooer.Settings.AllowTeenAdult(true))
                    {
                        flag2 = true;
                    }

                    foreach (IMiniRelationship relationship in Relationship.GetMiniRelationships(actor.SimDescription))
                    {
                        SimDescription description = SimDescription.Find(relationship.GetOtherSimDescriptionId(actor.SimDescription));
                        if (description != null && description.CreatedSim != null && description.CreatedSim.LotCurrent != actor.LotCurrent && !description.ChildOrBelow && description.IsHuman)
                        {
                            if (!flag && actor.SimDescription.Teen && description.Teen)
                            {
                                continue;
                            }

                            if (!flag2 && actor.SimDescription.Teen && description.YoungAdultOrAbove)
                            {
                                continue;
                            }

                            if (isAutonomous && !CommonSocials.CheckAutonomousGenderPreference(actor.SimDescription, description))
                            {
                                continue;
                            }

                            if (isAutonomous && !CommonWoohoo.SatisfiesCooldown(actor, description.CreatedSim, isAutonomous, ref greyedOutTooltipCallback))
                            {
                                continue;
                            }

                            if (!CommonWoohoo.HasWoohooableObject(actor.LotHome, actor, description.CreatedSim))
                            {
                                continue;
                            }
                                                      
                            return true;
                        }                                 
                    }
                }
               
                return false;
            }
        }
    }
}
