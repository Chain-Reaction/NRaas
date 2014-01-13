using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Deaths
{
    public class AncestralSaveScenario : BreakdownMoveInScenario
    {
        public AncestralSaveScenario(SimDescription sim)
            : base (sim, null, true)
        { }
        protected AncestralSaveScenario(AncestralSaveScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "AncestralSave";
            }
            else
            {
                return "MoveIn";
            }
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.LotHome == null)
            {
                IncStat("No Home");
                return false;
            }
            else if (!GetValue<IsAncestralOption, bool>(sim.Household))
            {
                IncStat("Not Ancestral");
                return false;
            }
            else if (sim.Genealogy == null)
            {
                IncStat("No Genealogy");
                return false;
            }

            foreach (SimDescription other in HouseholdsEx.Humans(sim.Household))
            {
                if (other == sim) continue;

                if (Deaths.IsDying(other)) continue;

                if (other.YoungAdultOrAbove) return false;
            }

            return base.Allow(sim);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            List<SimDescription> children = new List<SimDescription>();
            foreach (SimDescription child in Relationships.GetChildren(sim))
            {
                if (!child.YoungAdultOrAbove) continue;

                if (SimTypes.IsDead(child)) continue;

                children.Add(child);
            }

            return children;
        }

        protected override bool TargetSort(SimDescription primary, ref List<SimDescription> sims)
        {
            SaveType type = GetValue<SaveTakeBaseOption, SaveType>();

            if (type == SaveType.Random)
            {
                return false;
            }

            sims.Sort(new SortByAge());

            if (type == SaveType.LastBorn)
            {
                SimDescription choice = sims[sims.Count - 1];

                sims.Clear();
                sims.Add(choice);
            }
            else if (type != SaveType.FirstBorn)
            {
                List<SimDescription> top = new List<SimDescription>();
                List<SimDescription> rest = new List<SimDescription>();

                foreach (SimDescription sim in sims)
                {
                    if (type == SaveType.FirstMale)
                    {
                        if (sim.IsMale)
                        {
                            top.Add(sim);
                        }
                        else
                        {
                            rest.Add(sim);
                        }
                    }
                    else
                    {
                        if (sim.IsFemale)
                        {
                            top.Add(sim);
                        }
                        else
                        {
                            rest.Add(sim);
                        }
                    }
                }

                sims.Clear();

                sims.AddRange(top);
                sims.AddRange(rest);
            }

            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            SimDescription sim = Target;
            Target = Sim;
            Sim = sim;

            return base.PrivateUpdate(frame);
        }

        public override Scenario Clone()
        {
            return new AncestralSaveScenario(this);
        }

        protected class SortByAge : IComparer<SimDescription>
        {
            public int Compare(SimDescription x, SimDescription y)
            {
                float ageX = AgingManager.Singleton.GetCurrentAgeInDays(x);
                float ageY = AgingManager.Singleton.GetCurrentAgeInDays(y);

                if (ageX < ageY) return 1;

                if (ageX > ageY) return -1;

                return 0;
            }
        }

        public enum SaveType : int
        {
            Random = 0,
            FirstBorn = 1,
            FirstMale = 2,
            FirstFemale = 3,
            LastBorn = 4
        }

        public class SaveTakeBaseOption : EnumManagerOptionItem<ManagerDeath, SaveType>
        {
            public SaveTakeBaseOption()
                : base(SaveType.FirstBorn, SaveType.FirstBorn)
            { }

            public override string GetTitlePrefix()
            {
                return "AncestralFirstChoice";
            }

            protected override string GetLocalizationValueKey()
            {
                return "AncestralSaveType";
            }

            protected override SaveType Convert(int value)
            {
                return (SaveType)value;
            }

            protected override SaveType Combine(SaveType original, SaveType add, out bool same)
            {
                same = (original == add);
                return add;
            }
        }
    }
}
