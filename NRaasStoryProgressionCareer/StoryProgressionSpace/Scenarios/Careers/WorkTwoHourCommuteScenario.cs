using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class WorkTwoHourCommuteScenario : TwoHourCommuteScenario
    {
        public WorkTwoHourCommuteScenario(SimDescription sim)
            : base(sim)
        { }
        protected WorkTwoHourCommuteScenario(WorkTwoHourCommuteScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "WorkTwoHourCommute";
        }

        public override Career Job
        {
            get { return Sim.Occupation as Career; }
        }

        protected override TwoHourCommuteScenario.AlarmSimData AlarmData
        {
            get { return GetData<AlarmSimData>(Sim); }
        }

        protected override bool AllowHoliday(Season season)
        {
            return HasValue<AllowWorkHolidayOption,Season>(season);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            Add(frame, new WorkPushScenario(Sim), ScenarioResult.Start);
            Add(frame, new WorkSetAlarmScenario(Sim), ScenarioResult.Failure);
            return false;
        }

        public override Scenario Clone()
        {
            return new WorkTwoHourCommuteScenario(this);
        }

        protected new class AlarmSimData : TwoHourCommuteScenario.AlarmSimData
        {
            public AlarmSimData()
            { }
        }

        protected class WorkSetAlarmScenario : SetAlarmScenario
        {
            public WorkSetAlarmScenario(SimDescription sim)
                : base(sim)
            { }
            protected WorkSetAlarmScenario(WorkSetAlarmScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;
                
                return "WorkCommuteSetAlarm";
            }

            public override Career Job
            {
                get { return Sim.Occupation as Career; }
            }

            protected override CommuteScenario GetCommuteScenario(bool push)
            {
                return new WorkCommuteScenario(Sim, push);
            }

            protected override string GetCarpoolMessage(bool selfCommute)
            {
                if (selfCommute)
                {
                    return Common.Localize("WorkPush:CommuteComing", Sim.IsFemale, new object[] { Sim });
                }
                else
                {
                    return Common.LocalizeEAString(Sim.IsFemale, "Gameplay/Objects/Vehicles/CarpoolManager:CarpoolComing", new object[] { Sim });
                }
            }

            public override Scenario Clone()
            {
                return new WorkSetAlarmScenario(this);
            }
        }

        public class AllowWorkHolidayOption : MultiEnumManagerOptionItem<ManagerCareer, Season>
        {
            public AllowWorkHolidayOption()
                : base(new Season[] { Season.Spring, Season.Summer, Season.Fall, Season.Winter })
            { }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP8);
            }

            public override string GetTitlePrefix()
            {
                return "AllowWorkHoliday";
            }
        }
    }
}
