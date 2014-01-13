using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
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
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class ScheduledAssignSchoolScenario : AssignSchoolScenario
    {
        static bool sImmediateUpdate;

        public ScheduledAssignSchoolScenario()
        { }
        public ScheduledAssignSchoolScenario(SimDescription sim)
            : base(sim)
        { }
        protected ScheduledAssignSchoolScenario(AssignSchoolScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ScheduledAssignSchool";
        }

        protected override List<CareerLocation> GetPotentials()
        {
            throw new NotImplementedException();
        }

        public static void SetToImmediateUpdate()
        {
            sImmediateUpdate = true;
        }

        protected override bool Allow(bool fullUpdate, bool initialPass)
        {
            if (sImmediateUpdate)
            {
                sImmediateUpdate = false;
                return true;
            }

            return base.Allow(fullUpdate, initialPass);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Add(frame, new PrivateAssignSchoolScenario(Sim), ScenarioResult.Failure);
            Add(frame, new PublicAssignSchoolScenario(Sim), ScenarioResult.Failure);
            Add(frame, new HomeAssignSchoolScenario(Sim), ScenarioResult.Failure);
            return false;
        }

        public override Scenario Clone()
        {
            return new ScheduledAssignSchoolScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerCareer, ScheduledAssignSchoolScenario>, ManagerCareer.ISchoolOption
        {
            public Option()
                : base (true)
            { }

            public override string GetTitlePrefix()
            {
                return "PeriodicAssignSchool";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool Install(ManagerCareer manager, bool initial)
            {
                if (!base.Install(manager, initial)) return false;

                if (initial)
                {
                    ManagerCaste.OnInitializeCastes += OnInitialize;

                    ManagerLot.OnOptionsUpdated += OnUpdated;
                }

                return true;
            }

            protected static void OnUpdated(Lot lot)
            {
                SetToImmediateUpdate();
            }

            protected static void OnInitialize()
            {
                bool created;
                CasteOptions options = StoryProgression.Main.Options.GetNewCasteOptions("PublicSchool", Common.Localize("Caste:PublicSchool"), out created);
                if (created)
                {
                    options.SetValue<CasteAutoOption, bool>(true);
                    options.SetValue<CasteAgeOption, List<CASAgeGenderFlags>>(null);
                    options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Toddler);
                    options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Child);
                    options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Teen);

                    options.AddValue<CasteTypeOption, SimType>(SimType.NonActiveFamily);

                    options.SetValue<CasteFundsMaxOption, int>(100000);

                    options.SetValue<AssignPublicSchoolOption, bool>(true);
                }

                options = StoryProgression.Main.Options.GetNewCasteOptions("PrivateSchool", Common.Localize("Caste:PrivateSchool"), out created);
                if (created)
                {
                    options.SetValue<CasteAutoOption, bool>(true);
                    options.SetValue<CasteAgeOption, List<CASAgeGenderFlags>>(null);
                    options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Toddler);
                    options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Child);
                    options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Teen);

                    options.AddValue<CasteTypeOption, SimType>(SimType.NonActiveFamily);

                    options.SetValue<CasteFundsMinOption,int>(100000);

                    options.SetValue<AssignPrivateSchoolOption, bool>(true);
                }
            }
        }
    }
}
