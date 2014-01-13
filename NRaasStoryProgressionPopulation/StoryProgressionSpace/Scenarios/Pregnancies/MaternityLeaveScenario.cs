using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Tasks;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class MaternityLeaveScenario : SimScenario
    {
        int mAdditionalLeave;

        int mBabyCount;

        public MaternityLeaveScenario(SimDescription sim, int babyCount)
            : base(sim)
        {
            mBabyCount = babyCount;
        }
        protected MaternityLeaveScenario(MaternityLeaveScenario scenario)
            : base (scenario)
        {
            mAdditionalLeave = scenario.mAdditionalLeave;
            mBabyCount = scenario.mBabyCount;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "MaternityLeave";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }      

        protected void Add(Occupation job, int value)
        {
            if (job == null) return;

            if (job is SkillBasedCareer) return;

            if (job is Retired) return;

            if (value < 0)
            {
                if (job.mDaysOff == 0) return;

                job.mDaysOff += value;
                if (job.mDaysOff < 0)
                {
                    job.mDaysOff = 0;
                }

                job.SetHoursUntilWork();
            }
            else
            {
                job.TakePaidTimeOff(value);
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int leave = GetValue<MaternityLeaveOption, int>(Sim);
            if (leave == 0) return false;

            // -2 : 1 for the Option, 1 for the BabyCount
            mAdditionalLeave = AddScoring("MaternityLeave", leave + mBabyCount - 2, ScoringLookup.OptionType.Unbounded, Sim);

            int maximumLeave = GetValue<MaxMaternityLeaveOption, int>();
            if (mAdditionalLeave > maximumLeave)
            {
                mAdditionalLeave = maximumLeave;
            }

            Add(Sim.Occupation, mAdditionalLeave);
            Add(Sim.CareerManager.School, mAdditionalLeave);

            return (mAdditionalLeave != 0);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (!SimTypes.IsSelectable(Sim)) return null;

            if (Sim.Occupation == null) return null;

            if (Sim.Occupation.mDaysOff == 0)
            {
                name = "MaternityLeaveImmediate";
            }
            else if (mAdditionalLeave < 0)
            {
                name = "MaternityLeaveEarly";
            }
            
            if (parameters == null)
            {
                parameters = new object[] { Sim, Math.Abs(mAdditionalLeave) }; 
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new MaternityLeaveScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerPregnancy>
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerPregnancy main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    BirthScenario.OnBirthScenario += OnRun;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "MaternityLeave";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            protected static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                BirthScenario s = scenario as BirthScenario;
                if (s == null) return;

                SimDescription mom = null;
                SimDescription dad = null;

                Relationships.GetParents(s.Babies[0], out mom, out dad);

                if ((mom != null) && (mom.Household == s.Babies[0].Household))
                {
                    scenario.Add(frame, new MaternityLeaveScenario(mom, s.Babies.Count), ScenarioResult.Start);
                }

                if ((scenario.GetValue<PaternityLeaveOption,bool>()) && (dad != null) && (dad.Household == s.Babies[0].Household))
                {
                    scenario.Add(frame, new MaternityLeaveScenario(dad, s.Babies.Count), ScenarioResult.Start);
                }
            }
        }

        public class PaternityLeaveOption : BooleanManagerOptionItem<ManagerPregnancy>
        {
            public PaternityLeaveOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PaternityLeave";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class MaxMaternityLeaveOption : IntegerManagerOptionItem<ManagerPregnancy>
        {
            public MaxMaternityLeaveOption()
                : base(5)
            { }

            public override string GetTitlePrefix()
            {
                return "MaxMaternityLeave";
            }
        }
    }
}
