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
    public class RefundScenario : AlterFundsScenario, IHasPersonality
    {
        WeightOption.NameOption mName = null;

        string mAlterKey = null;

        IntegerOption.OptionValue mMinimum;
        IntegerOption.OptionValue mMaximum;

        string mAccountingKey = "Bills";

        WeightScenarioHelper mSuccess = null;

        public RefundScenario()
        { }
        protected RefundScenario(RefundScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mMinimum = scenario.mMinimum;
            mMaximum = scenario.mMaximum;
            mAlterKey = scenario.mAlterKey;
            mAccountingKey = scenario.mAccountingKey;
            mSuccess = scenario.mSuccess;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Minimum=" + mMinimum;
            text += Common.NewLine + "Maximum=" + mMaximum;
            text += Common.NewLine + "AlterKey=" + mAlterKey;
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
            get 
            { 
                if (!string.IsNullOrEmpty(mAlterKey))
                {
                    return mAlterKey;
                }
                else
                {
                    return mAccountingKey; 
                }
            }
        }

        protected override bool Subtraction
        {
            get { return false; }
        }

        protected override bool AllowDebt
        {
            get { return false; }
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
                if (!string.IsNullOrEmpty(mAlterKey))
                {
                    AccountingData accounting = GetValue<AcountingOption, AccountingData>(Sim.Household);
                    if (accounting != null)
                    {
                        int result = accounting.Get(mAlterKey);
                        if (result > 0)
                        {
                            return 0;
                        }
                        else
                        {
                            return -result;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return mMaximum.Value;
                }
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

            mMaximum = new IntegerOption.OptionValue();

            if (row.Exists("AlterKey"))
            {
                mAlterKey = row.GetString("AlterKey");
            }
            else
            {
                if (row.Exists("AccountingKey"))
                {
                    mAccountingKey = row.GetString("AccountingKey");
                }
                else
                {
                    error = "AccountingKey missing";
                    return false;
                }

                if (!mMaximum.Parse(row, "Maximum", Manager, this, ref error))
                {
                    return false;
                }
            }

            mMinimum = new IntegerOption.OptionValue();
            if (!mMinimum.Parse(row, "Minimum", Manager, this, ref error))
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
            return new RefundScenario(this);
        }
    }
}
