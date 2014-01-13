using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Scenarios;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerScenario : Manager
    {
        List<ScenarioFrame> mFrames = new List<ScenarioFrame>();

        int mNextID = 0;

        int mTrackingID = -1;
        Common.StringBuilder mTracking = null;
        bool mTrack = false;

        public ManagerScenario(Main manager)
            : base (manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Scenarios";
        }

        public int TrackingID
        {
            get
            {
                if (!Main.DebuggingEnabled) return -1;

                return mTrackingID;
            }
            set
            {
                mTrackingID = value;
            }
        }

        public bool Trace
        {
            get { return mTrack; }
            set { mTrack = value; }
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerScenario>(this).Perform(initial);
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            /*
            if (DebuggingLevel >= Common.DebugLevel.High)
            {
                int prevSize = mFrames.Count;

                string prevText = null;
                int count = 1;

                string msg = null;
                foreach (ScenarioFrame frame in mFrames)
                {
                    string text = frame.ToString();

                    if (text == prevText)
                    {
                        count++;
                    }
                    else
                    {
                        if (prevText != null)
                        {
                            msg += Common.NewLine;
                            if (count > 1)
                            {
                                msg += "(" + count + ") ";
                            }
                            msg += prevText;
                        }

                        prevText = text;
                        count = 1;
                    }
                }

                if (prevText != null)
                {
                    msg += Common.NewLine;
                    if (count > 1)
                    {
                        msg += "(" + count + ") ";
                    }
                    msg += prevText;
                }

                Common.Notify("-- Frames (" + mFrames.Count + ") --" + msg);
            }
            */

            foreach(ScenarioFrame frame in mFrames)
            {
                frame.Countdown();
            }

            bool printTrace = false;

            int frameCount = 0;

            int pass = 0;
            while ((mFrames.Count > 0) && (pass < GetValue<PassesPerCycleOption,int>()))
            {
                pass++;

                List<ScenarioFrame> newFrames = new List<ScenarioFrame>();

                List<ScenarioFrame> oldFrames = new List<ScenarioFrame>(mFrames);

                mFrames.Clear();

                foreach (ScenarioFrame frame in oldFrames)
                {
                    frameCount++;

                    List<ScenarioFrame> children = frame.Run();

                    if ((children != null) && (children.Count > 0))
                    {
                        newFrames.AddRange(children);
                    }

                    if (!frame.IsCompleted())
                    {
                        newFrames.Add(frame);
                    }

                    if ((mTracking != null) && (frame.ID == TrackingID))
                    {
                        printTrace = true;
                    }

                    Main.Sleep("ManagerScenario:PrivateUpdate");
                }

                mFrames.AddRange(newFrames);
            }

            AddStat("Update: Passes", pass);
            AddStat("Update: Frame Count", frameCount);

            if (printTrace)
            {
                WriteTrack(true);
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public override void Shutdown()
        {
            mFrames.Clear();

            mNextID = 0;
            mTrackingID = -1;
            mTrack = false;
            mTracking = null;

            base.Shutdown();
        }

        public override void IncStat(string stat, Common.DebugLevel minLevel)
        {
            if (mTrack)
            {
                Track(stat);
            }

            base.IncStat(stat, minLevel);
        }

        public override float AddStat(string stat, float val, Common.DebugLevel minLevel)
        {
            if (mTrack)
            {
                Track(stat + " (" + val + ")");
            }

            return base.AddStat(stat, val, minLevel);
        }

        public override void RemoveSim(ulong sim)
        {
            base.RemoveSim(sim);

            foreach (ScenarioFrame frame in mFrames)
            {
                frame.RemoveSim(sim);
            }
        }

        public void Track(string text)
        {
            if (mTracking == null)
            {
                mTracking = new Common.StringBuilder();
            }

            mTracking.Append(text + Common.NewLine + Common.NewLine);
        }

        public void WriteTrack(bool prompt)
        {
            if ((mTracking != null) && (Common.WriteLog(mTracking.ToString())))
            {
                if (prompt)
                {
                    SimpleMessageDialog.Show("Tracking", "Log Written");
                }
            }
            mTracking = null;
        }

        public ScenarioFrame Post(Scenario scenario, StoryProgressionObject manager, bool track)
        {
            if (scenario == null) return null;

            return Post(new Scenario.ScenarioRun(scenario, manager, ScenarioResult.Start), track);
        }
        public ScenarioFrame Post(Scenario.ScenarioRun scenario, bool track)
        {
            if (!scenario.Allow())
            {
                return null;
            }

            foreach (ScenarioFrame frame in mFrames)
            {
                if (frame.Matches(scenario))
                {
                    frame.Restart();
                    return null;
                }
            }

            return Post(new ScenarioFrame(scenario), track);
        }
        public ScenarioFrame Post (ScenarioFrame frame, bool track)
        {
            if ((track) && (mNextID == 0))
            {
                mNextID++;
            }

            frame.ID = mNextID;

            if (track)
            {
                mTrackingID = frame.ID;
            }
            else
            {
                frame.CheckDelay();
            }

            mNextID++;

            mFrames.Add(frame);
            return frame;
        }

        public class PassesPerCycleOption : IntegerManagerOptionItem<ManagerScenario>, IDebuggingOption
        {
            public PassesPerCycleOption()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "PassesPerCycle";
            }
        }

        public class DebugOption : DebugLevelOption<ManagerScenario>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        protected class DumpStatsOption : DumpStatsBaseOption<ManagerScenario>
        {
            public DumpStatsOption()
                : base(60)
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerScenario>
        {
            public DumpScoringOption()
            { }
        }

        protected class TicksPassedOption : TicksPassedBaseOption<ManagerScenario>
        {
            public TicksPassedOption()
            { }
        }
    }
}

