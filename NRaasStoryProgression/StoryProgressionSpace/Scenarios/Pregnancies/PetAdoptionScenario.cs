using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Pets;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class PetAdoptionScenario : PetAdoptionBaseScenario
    {
        public PetAdoptionScenario()
        { }
        public PetAdoptionScenario(CASAgeGenderFlags species, SimDescription owner)
            : base(species)
        {
            Sim = owner;
        }
        public PetAdoptionScenario(SimDescription newSim, bool forceInspection)
            : base(newSim, forceInspection)
        { }
        protected PetAdoptionScenario(PetAdoptionScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "PetImmigration";
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

 	        return base.Allow();
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Pregnancies;
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new PetAdoptionScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerPregnancy, PetAdoptionScenario>, ManagerPregnancy.IAdoptionOption, IAdjustForVacationOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PetImmigration";
            }

            public override bool Value
            {
                get
                {
                    if (!ShouldDisplay()) return false;

                    return base.Value;
                }
            }

            public bool AdjustForVacationTown()
            {
                SetValue(false);
                return true;
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP5);
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0)
                {
                    if (!Manager.GetValue<UsePetPoolOption, bool>())
                    {
                        return false;
                    }
                }

 	            return base.ShouldDisplay();
            }
        }

        public class UsePetPoolOption : BooleanManagerOptionItem<ManagerPregnancy>, ManagerPregnancy.IAdoptionOption
        {
            public UsePetPoolOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "UsePetPoolAdoption";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP5);
            }
        }
    }
}
