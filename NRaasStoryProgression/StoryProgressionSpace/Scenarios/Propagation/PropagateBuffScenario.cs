using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Propagation
{
    public abstract class PropagateBuffScenario : DualSimScenario, IFriendlyScenario
    {
        public enum WhichSim
        {
            Unset,
            Actor,
            Target,
            NotActor,
            NotTarget,
        }

        string mName = null;

        string mChildName = null;

        BuffNames mBuff;

        Origin mOrigin = Origin.None;

        protected float mTimeoutLength = float.MaxValue;

        WhichSim mFocusSim = WhichSim.Unset;

        SimDescription mFocus;

        public PropagateBuffScenario(BuffNames buff, Origin origin)
            : this(null, buff, float.MaxValue, origin)
        { }
        public PropagateBuffScenario(BuffNames buff, float timeoutLength, Origin origin)
            : this(null, buff, timeoutLength, origin)
        { }
        public PropagateBuffScenario(SimDescription source, BuffNames buff, Origin origin)
            : this(source, buff, float.MaxValue, origin)
        { }
        public PropagateBuffScenario(SimDescription source, BuffNames buff, float timeoutLength, Origin origin)
        {
            mFocus = source;
            mBuff = buff;
            mTimeoutLength = timeoutLength;
            mOrigin = origin;
        }
        protected PropagateBuffScenario(PropagateBuffScenario scenario)
            : base (scenario)
        {
            mFocus = scenario.mFocus;
            mName = scenario.mName;
            mChildName = scenario.mChildName;
            mBuff = scenario.mBuff;
            mTimeoutLength = scenario.mTimeoutLength;
            mOrigin = scenario.mOrigin;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "ChildStory=" + mChildName;
            text += Common.NewLine + "Buff=" + mBuff;
            text += Common.NewLine + "Origin=" + mOrigin;
            text += Common.NewLine + "TimeoutLength=" + mTimeoutLength;
            text += Common.NewLine + "FocusSim=" + mFocusSim;

            return text;
        }

        protected override bool ShouldReport
        {
            get { return false; }
        }

        public virtual bool IsFriendly
        {
            get { return true; }
        }

        protected BuffNames Buff
        {
            get { return mBuff; }
        }

        public Origin Origin
        {
            get { return mOrigin; }
            set { mOrigin = value; }
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return mName;
            }
            else
            {
                if ((Target == null) || (Target.TeenOrAbove))
                {
                    return mName;
                }
                else
                {
                    return mChildName;
                }
            }
        }

        public override bool ShouldPush
        {
            get { return false; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int TargetContinueChance
        {
            get { return 100; }
        }

        protected override int ContinueReportChance
        {
            get { return 0; }
        }

        protected override bool UsesSim(ulong sim)
        {
            if (mFocus != null)
            {
                if (mFocus.SimDescriptionId == sim) return true;
            }

            return base.UsesSim(sim);
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            throw new NotImplementedException();
        }

        public override bool PostParse(ref string error)
        {
            if (mFocusSim == WhichSim.Unset)
            {
                error = "PropagateFocus Unset";
                return false;
            }

            return base.PostParse(ref error);
        }

        public virtual bool Parse(XmlDbRow row, string prefix, bool firstPass, ref string error)
        {
            if ((!string.IsNullOrEmpty(prefix)) && (!Parse(row, null, false, ref error)))
            {
                return false;
            }

            if (firstPass)
            {
                if (!row.Exists(prefix + "PropagateStory"))
                {
                    error = prefix + "PropagateStory missing";
                    return false;
                }

                if (!row.Exists(prefix + "PropagateFocus"))
                {
                    error = prefix + "PropagateFocus missing";
                    return false;
                }
            }

            mName = row.GetString(prefix + "PropagateStory");

            if (row.Exists(prefix + "PropagateFocus"))
            {
                if (!ParserFunctions.TryParseEnum<WhichSim>(row.GetString(prefix + "PropagateFocus"), out mFocusSim, WhichSim.Target))
                {
                    error = prefix + "PropagateFocus unknown";
                    return false;
                }
            }

            mChildName = row.GetString(prefix + "PropagateChildStory");

            mInitialReportChance = row.GetInt(prefix + "PropagateInitialReportChance", row.GetInt(prefix + "PropagateReportChance", 10));
            mContinueReportChance = row.GetInt(prefix + "PropagateContinueReportChance", row.GetInt(prefix + "PropagateReportChance", 10));

            if (row.Exists(prefix + "PropagateBuff"))
            {
                if (!ParserFunctions.TryParseEnum<BuffNames>(row.GetString(prefix + "PropagateBuff"), out mBuff, BuffNames.Undefined))
                {
                    error = prefix + "PropagateBuff unknown";
                    return false;
                }
            }

            if (row.Exists(prefix + "PropagateOrigin"))
            {
                if (!ParserFunctions.TryParseEnum<Origin>(row.GetString(prefix + "PropagateOrigin"), out mOrigin, Origin.None))
                {
                    error = prefix + "PropagateOrigin unknown";
                    return false;
                }
            }

            Filter = new SimScenarioFilter();
            Filter.SetDisallowPartner(true);
            if (!Filter.Parse(row, Manager, this, prefix + "PropagateActor", firstPass, ref error))
            {
                return false;
            }

            TargetFilter = new SimScenarioFilter();
            TargetFilter.SetDisallowPartner(true);
            if (!TargetFilter.Parse(row, Manager, this, prefix + "PropagateTarget", firstPass, ref error))
            {
                return false;
            }

            return true;
        }

        public virtual void SetActors(SimDescription actor, SimDescription target)
        {
            if (mFocusSim == WhichSim.Actor)
            {
                mFocus = actor;
            }
            else
            {
                mFocus = target;
            }
        }
        
        protected override ICollection<SimDescription> GetSims()
        {
            if (mFocus == null) return null;

            if (Filter != null)
            {
                return Filter.Filter(new SimScenarioFilter.Parameters(this), "Actor", mFocus);
            }
            else
            {
                List<SimDescription> result = new List<SimDescription>();
                result.Add(mFocus);
                return result;
            }
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return GetFilteredTargets(sim);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool TargetAllow(SimDescription target)
        {
            if (target == Sim)
            {
                IncStat("Source");
                return false;
            }
            else if (target.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (Target.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (Target.CreatedSim.BuffManager == null)
            {
                IncStat("No Manager");
                return false;
            }

            return base.TargetAllow(target);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            ManagerSim.AddBuff(Target, mBuff, mTimeoutLength, mOrigin);
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (logging == ManagerStory.StoryLogging.Log)
            {
                logging = ManagerStory.StoryLogging.None;
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public class Option : BooleanManagerOptionItem<ManagerSim>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PropagateBuffs";
            }
        }
    }
}
