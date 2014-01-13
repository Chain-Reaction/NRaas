using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
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
    public class FineScenario : AlterFundsScenario, IHasPersonality
    {
        WeightOption.NameOption mName = null;

        IntegerOption.OptionValue mMinimum;
        IntegerOption.OptionValue mMaximum;

        string mAccountingKey = "Fines";

        WeightScenarioHelper mSuccess = null;

        BooleanOption.OptionValue mAllowDebt = null;

        public FineScenario()
        { }
        protected FineScenario(FineScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mMinimum = scenario.mMinimum;
            mMaximum = scenario.mMaximum;
            mAccountingKey = scenario.mAccountingKey;
            mSuccess = scenario.mSuccess;
            mAllowDebt = scenario.mAllowDebt;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "AllowDebt=" + mAllowDebt;
            text += Common.NewLine + "Minimum=" + mMinimum;
            text += Common.NewLine + "Maximum=" + mMaximum;
            text += Common.NewLine + "AccountingKey=" + mAccountingKey;
            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;

            return text;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return mName.WeightName;
            }
            else
            {
                return mName.ToString();
            }
        }

        protected override string AccountingKey
        {
            get { return mAccountingKey; }
        }

        protected override bool Subtraction
        {
            get { return true; }
        }

        protected override bool AllowDebt
        {
            get { return mAllowDebt.Value; }
        }

        protected override int Minimum
        {
            get 
            {
                return mMinimum.Value;
            }
        }

        protected override int Maximum
        {
            get 
            {
                return mMaximum.Value;
            }
        }

        protected override bool CheckBusy
        {
            get { return base.ShouldPush; }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            if (row.Exists("AccountingKey"))
            {
                mAccountingKey = row.GetString("AccountingKey");
            }
            else
            {
                error = "AccountingKey missing";
                return false;
            }

            mAllowDebt = new BooleanOption.OptionValue();
            if (!mAllowDebt.Parse(row, "AllowDebt", Manager, this, ref error))
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

            mSuccess = new WeightScenarioHelper(Origin.FromGettingGifts);
            if (!mSuccess.Parse(row, Manager, this, "Success", ref error))
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

        protected override bool Allow(SimDescription sim)
        {
            if (!Personalities.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mSuccess.TestBeforehand(Manager, Sim, Sim))
            {
                IncStat("Success TestBeforehand Fail");
                return false;
            }

            if (!base.PrivateUpdate(frame)) return false;

            mSuccess.Perform(this, frame, "Success", Sim, Sim);
            return true;
        }

        public override Scenario Clone()
        {
            return new FineScenario(this);
        }
    }
}
