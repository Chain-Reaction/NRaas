using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public abstract class SplitFamilyScenario : SimScenario
    {
        Household mHouse = null;

        SplitHome mNewHome = null;

        protected SplitFamilyScenario(Household house)
        {
            mHouse = house;
            mNewHome = new SplitHome();
        }
        protected SplitFamilyScenario(SplitFamilyScenario scenario)
            : base (scenario)
        {
            mHouse = scenario.mHouse;
            mNewHome = scenario.mNewHome;
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected abstract bool OnlyChildren
        {
            get;
        }

        protected abstract SplitMoveInScenario GetMoveInScenario(SplitHome newHome);

        protected abstract SplitMoveOutScenario GetMoveOutScenario(SplitHome newHome);

        protected override bool Allow()
        {
            if (HouseholdsEx.NumSims(mHouse) == 0)
            {
                IncStat("Empty");
                return false;
            }

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            if (mHouse == null) return null; 

            return HouseholdsEx.All(mHouse);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Households.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if ((OnlyChildren) && (Households.AllowGuardian(sim)))
            {
                IncStat("Too Old");
                return false;
            }
            else if (Deaths.IsDying(sim))
            {
                IncStat("Dying");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if ((mNewHome.House != null) && (HouseholdsEx.NumSims(mNewHome.House) > 0))
            {
                Add(frame, GetMoveInScenario(mNewHome), ScenarioResult.Start);
            }

            Add(frame, GetMoveOutScenario(mNewHome), ScenarioResult.Failure);
            return false;
        }

        public class SplitHome
        {
            protected Household mHouse;

            public Household House
            {
                get { return mHouse; }
                set { mHouse = value; }
            }
        }
    }
}
