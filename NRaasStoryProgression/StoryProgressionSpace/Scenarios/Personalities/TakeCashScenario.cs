using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Personalities
{
    public class TakeCashScenario : MoneyTransferScenario, IHasPersonality, IInvestigationScenario
    {
        WeightOption.NameOption mName = null;

        IntegerOption.OptionValue mMinimum;
        IntegerOption.OptionValue mMaximum;

        bool mFail = false;

        bool mAllowDebt = false;

        string mAccountingKey = "TakeCash";

        FightScenarioHelper mFight = null;

        InvestigationHelper mInvestigate = null;

        static UpdateDelegate OnInvestigateScenario;

        public TakeCashScenario() // Required for DerivativeSearch
            : base (-50)
        { }
        protected TakeCashScenario(TakeCashScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mMinimum = scenario.mMinimum;
            mMaximum = scenario.mMaximum;
            mAllowDebt = scenario.mAllowDebt;
            mAccountingKey = scenario.mAccountingKey;
            mFight = scenario.mFight;
            mFail = scenario.mFail;
            mInvestigate = scenario.mInvestigate;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Minimum=" + mMinimum;
            text += Common.NewLine + "Maximum=" + mMaximum;
            text += Common.NewLine + "AllowDebt=" + mAllowDebt;
            text += Common.NewLine + "AccountingKey=" + mAccountingKey;
            text += Common.NewLine + "Fight" + Common.NewLine + mFight;
            text += Common.NewLine + "Investigate" + Common.NewLine + mInvestigate;

            return text;
        }

        protected override string AccountingKey
        {
            get { return mAccountingKey; }
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return mName.WeightName;
            }
            else
            {
                if (mFail)
                {
                    return mName + "Fail";
                }
                else
                {
                    return mName.ToString();
                }
            }
        }

        protected override bool AllowDebt
        {
            get { return mAllowDebt; }
        }

        public bool AllowGoToJail
        {
            get { return mFight.AllowGoToJail; }
        }

        public string InvestigateStoryName
        {
            get 
            {
                return mInvestigate.GetStoryName("InvestigateMooch");
            }
        }

        public int InvestigateMinimum
        {
            get { return mInvestigate.GetMinimum(100); }
        }

        public int InvestigateMaximum
        {
            get { return mInvestigate.GetMaximum(250); }
        }

        public override bool IsFriendly
        {
            get { return false; }
        }

        protected override int Minimum
        {
            get { return mMinimum.Value; }
        }

        protected override int Maximum
        {
            get { return mMaximum.Value; }
        }

        protected override bool CheckBusy
        {
            get { return base.ShouldPush; }
        }

        public override bool ShouldPush
        {
            get
            {
                return mFight.ShouldPush(mFail, base.ShouldPush);
            }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mAllowDebt = row.GetBool("AllowDebt");

            if (row.Exists("AccountingKey"))
            {
                mAccountingKey = row.GetString("AccountingKey");
            }
            else
            {
                error = "AccountingKey missing";
                return false;
            }

            mFight = new FightScenarioHelper(Origin.FromTheft, SimDescription.DeathType.OldAge);
            if (!mFight.Parse(row, Manager, this, ref error))
            {
                return false;
            }

            mInvestigate = new InvestigationHelper();
            if (!mInvestigate.Parse(row, Manager, this, ref error))
            {
                return false;
            }

            mMinimum = new IntegerOption.OptionValue();
            if (!mMinimum.Parse(row, "Minimum", Manager, this, ref error))
            {
                return false;
            }

            mMaximum = new IntegerOption.OptionValue();
            if (!mMaximum.Parse(row, "Maximum", Manager, this, ref error))
            {
                return false;
            }

            return base.Parse(row, ref error);
        }

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return GetFilteredSims(Personalities.GetClanLeader(Manager));
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return GetFilteredTargets(sim);
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Personalities.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        public bool InstallInvestigation(Scenario.UpdateDelegate func)
        {
            OnInvestigateScenario += func;
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool result = mFight.Perform(this, frame, Sim, Target, base.PrivateUpdate, out mFail);

            if (!mFail)
            {
                if (OnInvestigateScenario != null)
                {
                    OnInvestigateScenario(this, frame);
                }
            }

            return result;
        }

        protected override bool Push()
        {
            if (!mFail) 
            {
                SimDescription leader = Personalities.GetClanLeader(Manager);
                if (leader != null)
                {
                    int score = mFight.SuccessDelta.Score(new DualSimScoringParameters(Sim, leader));
                    if (score != 0)
                    {
                        return Situations.PushMeetUp(this, Sim, leader, ManagerSituation.MeetUpType.Commercial, DetermineFirstAction(score, IsRomantic));
                    }
                }
            }

            return Situations.PushMeetUp(this, Sim, Target, ManagerSituation.MeetUpType.Commercial, FirstAction);
        }

        public override Scenario Clone()
        {
            return new TakeCashScenario(this);
        }
    }
}
