using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Caste
{
    public class CasteMoveInLotScenario : ScheduledSoloScenario, IEventScenario
    {
        Lot mLot;

        public CasteMoveInLotScenario()
        { }
        protected CasteMoveInLotScenario(CasteMoveInLotScenario scenario)
            : base (scenario)
        {
            mLot = scenario.mLot;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "CasteMoveIn";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override ListenHandleType HandleImmediately
        {
            get { return ListenHandleType.Task; }
        }

        public bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kLotChosenForActiveHousehold);
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            mLot = e.TargetObject as Lot;
            if (mLot == null) return null;

            if (mLot.Household == null) return null;

            return base.Handle(e, ref result);
        }


        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            foreach (SimDescription sim in CommonSpace.Helpers.Households.All(mLot.Household))
            {
                SimData data = GetData(sim);
                if (data == null) continue;

                data.InvalidateCache();
            }

            HouseholdOptions houseData = GetHouseOptions(mLot.Household);
            if (houseData != null)
            {
                houseData.InvalidateCache();
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new CasteMoveInLotScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerCaste, CasteMoveInLotScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CasteMoveIn";
            }
        }
    }
}
