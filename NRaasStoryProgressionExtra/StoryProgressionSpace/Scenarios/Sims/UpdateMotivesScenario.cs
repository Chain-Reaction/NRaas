using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class UpdateMotivesScenario : SimUpdateScenario, IEventScenario
    {
        static bool sImmediateUpdate = false;

        public UpdateMotivesScenario()
        { }
        protected UpdateMotivesScenario(UpdateMotivesScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "UpdateMotives";
        }

        protected override bool ContinuousUpdate
        {
            get { return false; }
        }

        public static void SetToImmediateUpdate()
        {
            sImmediateUpdate = true;
        }

        public bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSimInstantiated);
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

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected static void AlterDecay(Motive motive, bool freeze)
        {
            if (motive == null) return;

            if (freeze)
            {
                motive.FreezeDecay(true);
            }
            else
            {
                motive.RestoreDecay();
            }
        }

        protected override void PrivatePerform(SimDescription sim, SimData data, ScenarioFrame frame)
        {
            if (SimTypes.IsService(sim.Household)) return;

            if (sim.LotHome == null) return;

            if (sim.AssignedRole != null)
            {
                if (sim.AssignedRole.IsActive) return;
            }

            if (sim.CreatedSim == null) return;

            if (sim.CreatedSim.HasBeenDestroyed) return;

            if (sim.CreatedSim.Autonomy == null) return;

            Motives motives = sim.CreatedSim.Autonomy.Motives;
            if (motives == null) return;

            bool frozen = data.GetValue<StaticHungerOption,bool>();

            AlterDecay(motives.GetMotive(CommodityKind.VampireThirst), frozen);
            AlterDecay(motives.GetMotive(CommodityKind.Hunger), frozen);
            AlterDecay(motives.GetMotive(CommodityKind.BatteryPower), frozen);

            if (!sim.IsMummy)
            {
                frozen = data.GetValue<StaticEnergyOption, bool>();

                AlterDecay(motives.GetMotive(CommodityKind.Energy), frozen);
                AlterDecay(motives.GetMotive(CommodityKind.AlienBrainPower), frozen);

                frozen = data.GetValue<StaticBladderOption, bool>();

                AlterDecay(motives.GetMotive(CommodityKind.Bladder), frozen);
            }

            frozen = data.GetValue<StaticHygieneOption, bool>();

            if (!sim.IsFrankenstein)
            {
                AlterDecay(motives.GetMotive(CommodityKind.Hygiene), frozen);
            }

            AlterDecay(motives.GetMotive(CommodityKind.Maintenence), frozen);

            frozen = data.GetValue<StaticFunOption, bool>();

            AlterDecay(motives.GetMotive(CommodityKind.Fun), frozen);

            frozen = data.GetValue<StaticSocialOption, bool>();

            AlterDecay(motives.GetMotive(CommodityKind.Social), frozen);

            if (GameUtils.IsInstalled(ProductVersion.EP8))
            {
                frozen = data.GetValue<StaticTemperatureOption, bool>();

                AlterDecay(motives.GetMotive(CommodityKind.Temperature), frozen);
            }

            if (GameUtils.IsInstalled(ProductVersion.EP5))
            {
                switch (sim.Species)
                {
                    case CASAgeGenderFlags.Horse:
                        frozen = data.GetValue<StaticExerciseOption, bool>();

                        AlterDecay(motives.GetMotive(CommodityKind.HorseExercise), frozen);

                        frozen = data.GetValue<StaticThirstOption, bool>();

                        AlterDecay(motives.GetMotive(CommodityKind.HorseThirst), frozen);
                        break;
                    case CASAgeGenderFlags.Cat:
                        frozen = data.GetValue<StaticDestructionOption, bool>();

                        AlterDecay(motives.GetMotive(CommodityKind.CatScratch), frozen);
                        break;
                    case CASAgeGenderFlags.Dog:
                    case CASAgeGenderFlags.LittleDog:
                        frozen = data.GetValue<StaticDestructionOption, bool>();

                        AlterDecay(motives.GetMotive(CommodityKind.DogDestruction), frozen);

                        break;
                }
            }
        }

        public override Scenario Clone()
        {
            return new UpdateMotivesScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSim, UpdateMotivesScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "UpdateMotives";
            }
        }

        public class EventOption : BooleanEventOptionItem<ManagerSim, UpdateMotivesScenario>, IDebuggingOption
        {
            public EventOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "UpdateMotivesEvent";
            }
        }
    }
}
