using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class ValidatePregnancyScenario : SimScenario, IAlarmScenario
    {
        public ValidatePregnancyScenario()
        { }
        protected ValidatePregnancyScenario(ValidatePregnancyScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ValidatePregnancy";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override int MaximumReschedules
        {
            get { return int.MaxValue; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            Pregnancies.ResetPregnantSims();

            return Pregnancies.PregnantSims;
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDelayed(this, 6, TimeUnit.Hours);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!sim.IsPregnant)
            {
                IncStat("Not Pregnant");
                return false;
            }
            else if (GetValue<AllowCanBePregnantOption, bool>(sim))
            {
                IncStat("Allowed");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Pregnancy pregnancy = Sim.Pregnancy;
            if (pregnancy == null) return false;

            pregnancy.ClearPregnancyData();

            if (Sim.CreatedSim != null)
            {
                Sim.CreatedSim.BuffManager.RemoveElement(BuffNames.Pregnant);
            }

            IncStat("Discontinued");
            return true;
        }

        public override Scenario Clone()
        {
            return new ValidatePregnancyScenario(this);
        }

        public class Option : BooleanAlarmOptionItem<ManagerPregnancy, ValidatePregnancyScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ValidatePregnancy";
            }
        }
    }
}
