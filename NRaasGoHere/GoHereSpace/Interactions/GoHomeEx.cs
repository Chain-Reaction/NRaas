using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class GoHomeEx : Common.IPreLoad
    {
        static Tracer sTracer = new Tracer();

        static GoHomeEx()
        { }

        public void OnPreLoad()
        {
            Tunings.Inject<Lot, GoHome.Definition, Definition>(false);

            GoHome.Singleton = new Definition();
        }

        public class GoHomeStubEx : GoHome
        {
            public override void Cleanup()
            { 
                // Override to hide base class functionality
            }

            public override void CleanupAfterExitReason()
            {
                // Override to hide base class functionality
            }

            public override bool Run()
            {
                // Override to hide base class functionality
                return false;
            }

            public override bool Test()
            {
                return false;
            }
        }

        public class Tracer : StackTracer
        {
            public bool mFail;
            public bool mTestForActive;
            public bool mIgnore;

            public Sim mActor;

            public Tracer()
            {
                AddTest(typeof(Autonomy), "Boolean RunAutonomy(CommodityKind, Boolean)", OnRunAutonomy);
                AddTest(typeof(VisitSituation.GuestLeftAlone), "Void AddInteractionToGoOutside()", OnAddInteractionToGoOutside);

                AddTest(typeof(HudModel), "PushInteractionErrorCode PushGoHome()", OnIgnoreActive);
                AddTest(typeof(CarpoolManager.GetInCarpool), "Boolean Run", OnIgnoreActive);
                AddTest(typeof(VisitSituation.GuestLeavesAfterWaitingTooLong), "Void Init(", OnIgnoreActive);
                AddTest(typeof(GoToSchoolInRabbitHole), "Boolean AfterExitingRabbitHole()", OnIgnoreActive);
                AddTest(typeof(WorkInRabbitHole), "Boolean AfterExitingRabbitHole()", OnIgnoreActive);
                AddTest(typeof(SocialCallback), "Void OnInvitedOver", OnIgnoreActive);
                AddTest(typeof(SocialCallback), "Void TargetAskedToLeave", OnIgnoreActive);
                AddTest(typeof(ActiveFireFighter), "Void RegularWorkDayEndAlarmHandler", OnIgnoreActive);
                AddTest(typeof(Bartending.BartendingVenueHelper), "Boolean MakeSimLeaveVenueIfNecessary", OnIgnoreActive);
                AddTest(typeof(InteractionQueue), "Void InsertTakeBabyOrToddlerHome", OnIgnoreActive);
                AddTest(typeof(FieldTripSituation.RabbitHoleFieldTrip), "Boolean InRabbitHole()", OnIgnoreActive);
                AddTest(typeof(Windows.RunTowardsPeepingSimAndShooAway), "Boolean Run()", OnIgnoreActive);
                AddTest(typeof(SocialCallback), "Void BouncerAskToGetInFailure", OnIgnoreActive);
                AddTest(typeof(Pregnancy.HaveBabyHospital), "Boolean AfterExitingRabbitHole", OnIgnoreActive);
                AddTest(typeof(CarOwnable.GoHome), "Boolean Run", OnIgnoreActive);
                AddTest(typeof(HostedSituation), "Void MakeGuestGoHome", OnIgnoreActive);
                AddTest(typeof(DaycareToddlerDropoffSituation), "Void OnPutDownChildSucceeded", OnIgnoreActive);
                AddTest(typeof(DaycareToddlerDropoffSituation.MakeAdultSimGoHome), "Void Init", OnIgnoreActive);
                AddTest("NRaas.PortraitPanelSpace.Dialogs.SkewerEx", "Void OnGoHome", OnIgnoreActive);
                AddTest("NRaas.CareerSpace.Skills.Assassination", "Void AnimateWitnesses", OnIgnoreActive);
                AddTest("NRaas.MasterControllerSpace.Sims.Basic.GoHome", "Boolean Run", OnIgnoreActive);
                AddTest("NRaas.MasterControllerSpace.Households.AddSim", "OptionResult Perform", OnIgnoreActive);
                AddTest("NRaas.StoryProgressionSpace.Managers.ManagerSituation", "Boolean PushGoHome", OnIgnoreActive);
                AddTest("NRaas.StoryProgressionSpace.Scenarios.Situations.GoHomePushScenario", "Boolean Push()", OnIgnoreActive);
                AddTest("NRaas.StoryProgressionSpace.Scenarios.Sims.MoodCheckScenario", "Void PrivatePerform", OnIgnoreActive);
                AddTest("NRaas.StoryProgressionSpace.Scenarios.Pregnancies.AdoptionBaseScenario", "Boolean Push()", OnIgnoreActive);
                AddTest("NRaas.StoryProgressionSpace.Situations.HousePartySituation", "Void CheckIfGuestsNeedToGoHome", OnIgnoreActive);

                AddTest(typeof(Sims3.Gameplay.Actors.Sim.GoInside), "Boolean Run", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Roles.RoleSpecialMerchant), "Void EndRole", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Skills.Styling.StylistRole), "Void EndRole", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Skills.Tattooing.TattooArtist), "Void EndRole", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.Objects.Miscellaneous.BakeSaleTable.BrowseOrPurchaseFromSim), "Boolean Run", OnIgnore);
                AddTest(typeof(VisitSituation), "Boolean MakeSimExitToYard", OnIgnore);
                AddTest(typeof(Sim.FollowParent), "Boolean Run()", OnIgnore);
                AddTest(typeof(Sim), "Void EnforceChildCurfew", OnIgnore);
                AddTest(typeof(Sim), "Void EnforceTeenCurfew", OnIgnore);
                AddTest(typeof(Sims3.Gameplay.ActorSystems.BuffHeatingUp), "Void OnAddition", OnIgnore);
                AddTest(typeof(BuffTransformationCallback), "Void OnRemovalOfZombification", OnIgnore);
                AddTest(typeof(BuffTerrified.BuffInstanceTerrified), "Void RandomReactions", OnIgnore);
            }

            public override void Reset()
            {
                mFail = false;
                mTestForActive = (Household.ActiveHousehold != null);
                mIgnore = false;
                //mActor = null;

                base.Reset();
            }

            protected bool OnAddInteractionToGoOutside(StackTrace trace, StackFrame frame)
            {
                mFail = true;
                mIgnore = true;

                if (mActor != null)
                {
                    VisitSituation visit = mActor.GetSituationOfType<VisitSituation>();
                    if (visit != null)
                    {
                        VisitSituation.GuestLeftAlone child = visit.Child as VisitSituation.GuestLeftAlone;
                        if (child != null)
                        {
                            AlarmManager.Global.RemoveAlarm(child.mRemindGuestToGoOutside);
                            child.mRemindGuestToGoOutside = AlarmHandle.kInvalidHandle;
                        }
                    }
                }

                return true;
            }

            protected bool OnIgnore(StackTrace trace, StackFrame frame)
            {
                mIgnore = true;
                return true;
            }

            protected bool OnIgnoreActive(StackTrace trace, StackFrame frame)
            {
                mTestForActive = false;
                mIgnore = true;
                return true;
            }

            protected bool OnRunAutonomy(StackTrace trace, StackFrame frame)
            {
                TypeStackFrame test = new TypeStackFrame(typeof(Autonomy), "Sims3.Gameplay.Interactions.InteractionInstance AssignProbabilitiesAndChooseInteraction()", null);

                mIgnore = true;

                foreach (StackFrame frame2 in trace.GetFrames())
                {
                    if (test.Test(frame2))
                    {
                        return false;
                    }
                }

                mTestForActive = false;
                return false;
            }

            public override string ToString()
            {
                string msg = base.ToString() + Common.NewLine;

                msg += Common.NewLine + "Actor=";
                if (mActor != null)
                {
                    msg += mActor.FullName;
                }
                else
                {
                    msg += "<null>";
                }

                msg += Common.NewLine + "Fail=" + mFail;
                msg += Common.NewLine + "Ignore=" + mIgnore;
                msg += Common.NewLine + "TestForActive=" + mTestForActive;

                return msg;
            }
        }

        public static bool TestAllow(InteractionDefinition definition, ref InteractionInstanceParameters parameters)
        {

            try
            {
                Common.StringBuilder msg = new Common.StringBuilder();

                GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                InteractionTestResult result = definition.Test(ref parameters, ref greyedOutTooltipCallback);
                if (!IUtil.IsPass(result))
                {
                    msg += Common.NewLine + result;

                    sTracer.Reset();

                    sTracer.mActor = parameters.Actor as Sim;
                    sTracer.mFail = true;
                    sTracer.mIgnore = true;
                }
                else
                {
                    sTracer.mActor = parameters.Actor as Sim;
                    sTracer.Perform();

                    if ((sTracer.mActor != null) && (sTracer.mTestForActive))
                    {
                        if (sTracer.mActor.IsSelectable)
                        {
                            if (GoHere.Settings.mDisallowActiveGoHome)
                            {
                                sTracer.mFail = true;
                            }
                        }
                        else
                        {
                            if ((GoHere.Settings.mDisallowInactiveLeaveActiveLot) &&
                                (!sTracer.mActor.LotCurrent.IsBaseCampLotType) &&
                                (!sTracer.mActor.Autonomy.Motives.IsSleepy()))
                            {
                                foreach (Sim sim in Households.AllSims(Household.ActiveHousehold))
                                {
                                    if (sim.LotCurrent == sTracer.mActor.LotCurrent)
                                    {
                                        sTracer.mFail = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (((!sTracer.mIgnore) || (!GoHere.Settings.mIgnoreLogs)) && (Common.kDebugging))
                {
                    msg += Common.NewLine + sTracer.ToString();

                    Common.DebugException(parameters.Actor, parameters.Target, msg, new Exception());
                }

                if (sTracer.mFail)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Common.Exception(parameters.Actor, parameters.Target, e);
            }

            return true;
        }

        public class Definition : GoHome.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                if (!TestAllow(this, ref parameters))
                {
                    InteractionInstance na = new GoHomeStubEx();
                    na.Init(ref parameters);
                    return na;
                }

                return base.CreateInstance(ref parameters);
            }

            public override bool Test(Sim a, Lot target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a.LotCurrent == a.LotHome)
                {
                    if ((!isAutonomous) && (a.InteractionQueue.Count > 0))
                    {
                        return true;
                    }
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
