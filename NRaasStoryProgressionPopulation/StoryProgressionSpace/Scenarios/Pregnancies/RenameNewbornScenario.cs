using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class RenameNewbornScenario : SimScenario
    {
        public RenameNewbornScenario(SimDescription sim)
            : base (sim)
        { }
        protected RenameNewbornScenario(RenameNewbornScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "RenameNewborn";
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

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.Genealogy == null)
            {
                IncStat("No Genealogy");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            SimDescription mom = null, dad = null;
            Relationships.GetParents(Sim, out mom, out dad);

            bool wasEither;
            string lastName = null;
            if (mom != null)
            {
                lastName = GetData(mom).HandleName<BabyLastNameOption>(Sims, dad, out wasEither);
            }
            else if (dad != null)
            {
                lastName = GetData(dad).HandleName<BabyLastNameOption>(Sims, mom, out wasEither);
            }

            if (lastName != null)
            {
                Sim.LastName = lastName;
            }
            else if (((dad != null) && (RandomUtil.CoinFlip())) || (mom == null))
            {
                Sim.LastName = dad.LastName;
            }
            else
            {
                Sim.LastName = mom.LastName;
            }

            if (GetValue<RenameNewbornOption, bool>(Sim))
            {
                CASAgeGenderFlags parentGender = CASAgeGenderFlags.None;

                List<object> parameters = Stories.AddGenderNouns(Sim);

                string key = null;
                if (mom != null)
                {
                    parentGender = mom.Gender;

                    parameters.AddRange(Stories.AddGenderNouns(mom));
                }

                if (dad != null)
                {
                    if (parentGender == CASAgeGenderFlags.None)
                    {
                        parentGender = dad.Gender;
                    }

                    parameters.AddRange(Stories.AddGenderNouns(dad));
                }

                if (mom != null)
                {
                    if (dad != null)
                    {
                        key = "DuoPrompt";
                    }
                    else
                    {
                        key = "SinglePrompt";
                    }
                }
                else if (dad != null)
                {
                    key = "SinglePrompt";
                }
                else
                {
                    key = "Prompt";
                }

                ManagerStory.AvailableStory story = new ManagerStory.AvailableStory(GetOption<Option>(), key, parameters.ToArray());

                string text = StringInputDialog.Show(GetOption<Option>().Name, story.Localize(GetOption<Option>(), Sim.Gender, parentGender, parameters.ToArray()), Sim.FirstName, 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(text)) return false;

                Sim.FirstName = text;
            }

            if ((!SimTypes.IsSelectable(Sim)) && (GetValue<PromptToMoveNewbornOption, bool>()))
            {
                foreach (SimDescription parent in Relationships.GetParents(Sim))
                {
                    if (SimTypes.IsSelectable(parent))
                    {
                        if (AcceptCancelDialog.Show(Common.Localize("PromptToMoveNewborn:Prompt", Sim.IsFemale, new object[] { Sim })))
                        {
                            Sim.Household.Remove(Sim);

                            parent.Household.Add(Sim);

                            if (Sim.CreatedSim == null)
                            {
                                Instantiation.EnsureInstantiate(Sim, parent.LotHome);
                            }
                        }
                    }
                }
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new RenameNewbornScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerPregnancy>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "RenameNewborns";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool Install(ManagerPregnancy main, bool initial)
            {
                if (initial)
                {
                    BirthScenario.OnRenameNewbornsScenario += OnRun;
                }

                return base.Install(main, initial);
            }

            public static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                BirthScenario s = scenario as BirthScenario;
                if (s == null) return;

                foreach (SimDescription baby in s.Babies)
                {
                    s.Add(frame, new RenameNewbornScenario(baby), ScenarioResult.Start);
                }
            }
        }

        public class PromptToMoveNewbornOption : BooleanManagerOptionItem<ManagerPregnancy>
        {
            public PromptToMoveNewbornOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "PromptToMoveNewborn";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
