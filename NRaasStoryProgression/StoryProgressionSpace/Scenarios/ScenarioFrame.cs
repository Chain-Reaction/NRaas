using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Interfaces;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public class ScenarioFrame : List<Scenario.ScenarioRun>
    {
        Scenario.ScenarioRun mRoot = null;

        ScenarioFrame mParent = null;

        List<ScenarioFrame> mChildren = new List<ScenarioFrame>();

        ScenarioResult mLastResult = ScenarioResult.Start;

        int mCountdown = 0;

        public ScenarioFrame(Scenario.ScenarioRun run)
        {
            mRoot = run;

            Add(run);
        }
        protected ScenarioFrame(ScenarioFrame parent, Scenario.ScenarioRun root)
        {
            mRoot = root;
            mParent = parent;
        }

        protected bool ChildOfRoot
        {
            get
            {
                if (mParent == null) return true;

                return mParent.TopLevel;
            }
        }

        public bool Success
        {
            get { return (mLastResult == ScenarioResult.Success); }
        }

        public bool TopLevel
        {
            get { return (mParent == null); }
        }

        public bool Delayed
        {
            get { return (mCountdown > 0); }
        }

        public string UnlocalizedName
        {
            get
            {
                if (mRoot == null) return null;

                return mRoot.UnlocalizedName;
            }
        }

        public int ID
        {
            get
            {
                if (mRoot == null) return -1;

                return mRoot.ID;
            }
            set
            {
                foreach (Scenario.ScenarioRun run in this)
                {
                    run.ID = value;
                }
            }
        }

        public override string ToString()
        {
            string text = null;

            if (TopLevel)
            {
                text = "Root ";

                if (mCountdown > 0)
                {
                    text += "(Delayed: " + mCountdown + ") ";
                }
            }
            else
            {
                text = "Run ";
            }

            text += mRoot.ToString();

            text += Common.NewLine + mLastResult.ToString();

            if (Count > 0)
            {
                text += " (" + Count.ToString() + ")";
            }

            if (mParent != null)
            {
                text += Common.NewLine + "----" + Common.NewLine + mParent.ToString();
            }

            return text;
        }

        public ScenarioResult LastResult
        {
            get { return mLastResult; }
            set
            {
                if (mLastResult == value) return;

                mLastResult = value;

                if ((Success) && (mRoot != null))
                {
                    mRoot.PushAndPrint();
                }

                if (mParent != null)
                {
                    if (!mParent.Success)
                    {
                        mParent.LastResult = mLastResult;
                    }
                }
            }
        }

        public bool Matches(Scenario.ScenarioRun run)
        {
            if (!TopLevel) return false;

            return mRoot.Matches(run);
        }

        public void CheckDelay()
        {
            if (mRoot == null) return;

            if (!mRoot.Delayed) return;

            mCountdown = mRoot.Reschedule();
        }

        public void Restart()
        {
            mCountdown = 0;

            mRoot.Restart();
        }

        protected void RemoveFromParent(ScenarioFrame me)
        {
            mChildren.Remove(me);

            if (mParent != null)
            {
                mParent.RemoveFromParent(me);
            }
        }

        protected void AddChildren(List<ScenarioFrame> frames)
        {
            mChildren.AddRange(frames);

            if (mParent != null)
            {
                mParent.AddChildren(frames);
            }
        }

        public void Countdown()
        {
            if (mCountdown <= 0) return;

            mCountdown -= 10;

            if (mCountdown <= 0)
            {
                mLastResult = ScenarioResult.Start;

                Add(mRoot);
            }
        }

        public bool IsCompleted()
        {
            if (Count > 0) return false;

            if (mChildren.Count > 0) return false;

            if (!TopLevel) return true;

            if ((Success) && (!mRoot.AlwaysReschedule)) return true;

            if (mCountdown > 0) return false;

            mCountdown = mRoot.Reschedule();

            return (mCountdown <= 0);
        }

        public void Add(StoryProgressionObject manager, Scenario scenario, ScenarioResult precondition)
        {
            if (scenario == null) return;

            if (manager == null) return;

            Add(new Scenario.ScenarioRun (scenario, manager, precondition));
        }
        public void Add(StoryProgressionObject manager, Scenario scenario, int chance)
        {
            if (scenario == null) return;

            if (manager == null) return;

            Add(new Scenario.ScenarioRun(scenario, manager, chance));
        }

        public void RemoveSim(ulong sim)
        {
            int index = 0;
            while (index < Count)
            {
                Scenario.ScenarioRun run = this[index];

                if (run.UsesSim(sim))
                {
                    RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        public List<ScenarioFrame> Run()
        {
            if (StoryProgression.Main == null) return null;

            using (Common.TestSpan span = new Common.TestSpan(StoryProgression.Main.Scenarios, "ScenarioFrameRun", Common.DebugLevel.Stats))
            {
                if (mCountdown > 0)
                {
                    return null;
                }

                if (mChildren.Count > 0)
                {
                    return null;
                }

                ScenarioResult lastResult = LastResult;

                List<ScenarioFrame> frames = new List<ScenarioFrame>();

                while (Count > 0)
                {
                    Scenario.ScenarioRun run = this[0];

                    string test = UnlocalizedName;

                    if ((frames.Count > 0) && ((!run.IsInstant) || (!ChildOfRoot)))
                    {
                        break;
                    }

                    RemoveAt(0);

                    ScenarioFrame frame = new ScenarioFrame(this, run);

                    ScenarioResult prevResult = lastResult;

                    //lastResult = Scenario.ScenarioRun.RunTask.Wait(run, lastResult, frame);
                    lastResult = run.Run(lastResult, frame);

                    if (frame.Count > 0)
                    {
                        if ((StoryProgression.Main != null) && (StoryProgression.Main.Scenarios.TrackingID == ID))
                        {
                            StoryProgression.Main.Scenarios.Track("Before: " + prevResult + Common.NewLine + "After: Nested" + Common.NewLine + run.ToString() + Common.NewLine + ToString());
                        }

                        frame.ID = run.ID;
                        frames.Add(frame);
                    }
                    else
                    {
                        if ((StoryProgression.Main != null) && (StoryProgression.Main.Scenarios.TrackingID == ID))
                        {
                            StoryProgression.Main.Scenarios.Track("Before: " + prevResult + Common.NewLine + "After: " + lastResult + Common.NewLine + run.ToString() + Common.NewLine + ToString());
                        }

                        LastResult = lastResult;
                    }
                }

                if (Count == 0)
                {
                    RemoveFromParent(this);
                }

                if (frames.Count > 0)
                {
                    AddChildren(frames);
                }

                return frames;
            }
        }
    }
}
