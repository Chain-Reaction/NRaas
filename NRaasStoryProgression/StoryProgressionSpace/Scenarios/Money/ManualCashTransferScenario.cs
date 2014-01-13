using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class ManualCashTransferScenario : MoneyTransferScenario
    {
        string mAccountingKey;

        int mMinimum = 0;
        int mMaximum = 0;

        public ManualCashTransferScenario(SimDescription sim, SimDescription target, int delta, string accountingKey, int minimum, int maximum)
            : base(sim, target, delta)
        {
            mAccountingKey = accountingKey;
            mMinimum = minimum;
            mMaximum = maximum;
        }
        protected ManualCashTransferScenario(ManualCashTransferScenario scenario)
            : base (scenario)
        {
            mAccountingKey = scenario.mAccountingKey;
            mMinimum = scenario.mMinimum;
            mMaximum = scenario.mMaximum;
        }

        public override string GetTitlePrefix(ManagerProgressionBase.PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ManualCashTransfer";
        }

        public override bool IsFriendly
        {
            get { return true; }
        }

        protected override string AccountingKey
        {
            get { return mAccountingKey; }
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override int Minimum
        {
            get { return mMinimum; }
        }

        protected override int Maximum
        {
            get { return mMaximum; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return null;
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new ManualCashTransferScenario(this);
        }
    }
}
