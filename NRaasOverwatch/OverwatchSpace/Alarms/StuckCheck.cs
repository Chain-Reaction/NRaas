using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace NRaas.OverwatchSpace.Alarms
{
    public class StuckCheck : AlarmOption, Common.IPreLoad, Common.IWorldQuit
    {
        static Tracer sTracer = new Tracer();

        static Dictionary<ulong, StuckSimData> sRouteData = new Dictionary<ulong, StuckSimData>();

        static Dictionary<ulong,StuckSimData> sData = new Dictionary<ulong,StuckSimData>();

        public override string GetTitlePrefix()
        {
            return "StuckCheck";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mStuckCheckV2;
            }
            set
            {
                NRaas.Overwatch.Settings.mStuckCheckV2 = value;
            }
        }

        public void OnPreLoad()
        {
            Route.PostPlanCallback += OnPostPlan;
        }

        public void OnWorldQuit()
        {
            sData.Clear();
        }

        protected static void OnPostPlan(Route r, string routeType, string result)
        {
            Sim sim = null;

            try
            {
                if (!Overwatch.Settings.mStuckCheckV2) return;

                if ((r != null) && (r.Follower != null))
                {
                    sim = r.Follower.Target as Sim;
                    if ((sim != null) && (sim.SimDescription != null))
                    {
                        StuckSimData other;
                        if (!sRouteData.TryGetValue(sim.SimDescription.SimDescriptionId, out other))
                        {
                            other = new StuckSimData();

                            sRouteData.Add(sim.SimDescription.SimDescriptionId, other);
                        }

                        if (other.mInReset) return;

                        if (other.mLastPosition != sim.Position)
                        {
                            other.mLastPosition = sim.Position;
                            other.mChecks = 0;
                        }
                        else if (!r.PlanResult.Succeeded())
                        {
                            if (other.mChecks == 0)
                            {
                                other.mLastPositionTicks = SimClock.CurrentTicks;
                            }

                            other.mChecks++;

                            if (other.mChecks > Overwatch.Settings.mMinimumRouteFail)
                            {
                                if ((SimClock.CurrentTicks - other.mLastPositionTicks) > (SimClock.kSimulatorTicksPerSimMinute * Overwatch.Settings.mRouteFailTestMinutesV2))
                                {
                                    sTracer.Perform();

                                    if (!sTracer.mIgnore)
                                    {
                                        if (Common.kDebugging)
                                        {
                                            Common.DebugStackLog("OnPostPlan" + Common.NewLine + sim.FullName + Common.NewLine + r.PlanResult + Common.NewLine + other.mChecks);
                                            Common.DebugNotify("OnPostPlan" + Common.NewLine + sim.FullName + Common.NewLine + r.PlanResult + Common.NewLine + other.mChecks, sim);
                                        }
                                    }

                                    other.mChecks = 0;

                                    if (!sTracer.mIgnore)
                                    {
                                        other.mInReset = true;

                                        ResetTask.Perform(sim, r.GetDestPoint(), "Unroutable");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, null, "OnPostPlan", e);
            }
        }

        public class ResetTask : Common.AlarmTask
        {
            Sim mSim;

            Vector3 mDestination;

            string mSuffix;

            protected ResetTask(Sim sim, Vector3 destination, string suffix)
                : base(1, TimeUnit.Minutes)
            {
                mSim = sim;
                mDestination = destination;
                mSuffix = suffix;
            }

            public static void Perform(Sim sim, Vector3 destination, string suffix)
            {
                new ResetTask(sim, destination, suffix);//.AddToSimulator();
            }

            protected override void OnPerform()
            {
                ulong id = mSim.SimDescription.SimDescriptionId;

                try
                {
                    if (Overwatch.Settings.mStuckCheckReset)
                    {
                        if (!Instantiation.AttemptToPutInSafeLocation(mSim, mDestination, !mSim.SimDescription.IsImaginaryFriend))
                        {
                            ResetSimTask.Perform(mSim, true);
                        }
                    }
                }
                finally
                {
                    //sData.Remove(id);
                    sRouteData.Remove(id);                        
                }

                // Keeps Stationary notices for debug only
                if (mSuffix == "Stationary" && !Common.kDebugging) return;

                Common.Notify(mSim, Common.Localize("StuckCheck:" + mSuffix, mSim.IsFemale, new object[] { mSim }));
            }
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            Overwatch.Log(Name);

            foreach (Sim createdSim in new List<Sim> (LotManager.Actors))
            {
                StuckSimData other;
                if (!sData.TryGetValue(createdSim.SimDescription.SimDescriptionId, out other))
                {
                    other = new StuckSimData();

                    sData.Add(createdSim.SimDescription.SimDescriptionId, other);
                }

                try
                {
                    bool wasReset = false;

                    if ((createdSim != null) && (createdSim.InWorld) && (createdSim.Proxy != null) && (!SimTypes.IsSelectable(createdSim)))
                    {
                        bool check = true;

                        if (createdSim.Parent is IBed)
                        {
                            check = false;
                        }

                        if (createdSim.OccultManager != null)
                        {
                            OccultRobot bot = createdSim.OccultManager.GetOccultType(Sims3.UI.Hud.OccultTypes.Robot) as OccultRobot;

                            if (bot != null && bot.IsShutdown)
                            {
                                check = false;
                            }
                        }

                        if (check)
                        {
                            InteractionInstance interaction = createdSim.CurrentInteraction;

                            bool sameInteraction = (object.ReferenceEquals(other.mLastInteraction, interaction));

                            other.mLastInteraction = interaction;

                            if (createdSim.LotCurrent.IsRoomHidden(createdSim.RoomId))
                            {
                                ResetTask.Perform(createdSim, Vector3.Invalid, "Unroutable");
                                wasReset = true;
                            }

                            if ((!wasReset) && (other.mLastPosition != Vector3.Invalid) && (other.mLastPosition == createdSim.Position))
                            {
                                if ((interaction == null) || (sameInteraction))
                                {
                                    bool success = false;

                                    try
                                    {
                                        success = SimEx.IsPointInLotSafelyRoutable(createdSim, createdSim.LotCurrent, createdSim.PositionOnFloor);
                                    }
                                    catch (Exception e)
                                    {
                                        Common.DebugException(createdSim, e);
                                        success = false;
                                    }

                                    if (!success)
                                    {
                                        ResetTask.Perform(createdSim, Vector3.Invalid, "Unroutable");
                                        wasReset = true;
                                    }
                                }
                            }

                            if (other.mLastPosition != createdSim.Position)
                            {
                                other.mLastPosition = createdSim.Position;
                                other.mLastPositionTicks = SimClock.CurrentTicks;
                            }
                            else if ((prompt) || ((other.mLastPositionTicks + SimClock.kSimulatorTicksPerSimDay) < SimClock.CurrentTicks))
                            {
                                if (!wasReset)
                                {
                                    bool reset = false;
                                    if (sameInteraction)
                                    {
                                        reset = true;
                                    }

                                    if (reset)
                                    {
                                        ResetTask.Perform(createdSim, Vector3.Invalid, "Stationary");
                                        wasReset = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        wasReset = true;
                    }

                    if (wasReset)
                    {
                        sData.Remove(createdSim.SimDescription.SimDescriptionId);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(createdSim, e);
                }
            }
        }

        protected class StuckSimData
        {
            public Vector3 mLastPosition = Vector3.Invalid;

            public int mChecks = 0;

            public long mLastPositionTicks = 0;

            public InteractionInstance mLastInteraction;

            public bool mInReset = false;

            public StuckSimData()
            { }

            public override string ToString()
            {
                string text = base.ToString();

                text += Common.NewLine + "Last Pos: " + mLastPosition;
                text += Common.NewLine + "Last Pos Ticks: " + mLastPositionTicks;
                text += Common.NewLine + "In Reset: " + mInReset;

                if ((mLastInteraction != null) && (mLastInteraction.InteractionDefinition != null))
                {
                    text += Common.NewLine + "Last Interaction: " + mLastInteraction.InteractionDefinition.GetType();
                }
                else
                {
                    text += Common.NewLine + "Last Interaction: <null>";
                }

                return text;
            }
        }

        public class Tracer : StackTracer
        {
            public bool mIgnore;

            public Tracer()
            {
                AddTest(typeof(Sims3.Gameplay.Core.LotManager.RunStuckCheckHelper), "Void RunStuckCheck", OnIgnore);
            }

            public override void Reset()
            {
                mIgnore = false;
                //mActor = null;

                base.Reset();
            }

            protected bool OnIgnore(StackTrace trace, StackFrame frame)
            {
                mIgnore = true;
                return true;
            }
        }
    }
}
