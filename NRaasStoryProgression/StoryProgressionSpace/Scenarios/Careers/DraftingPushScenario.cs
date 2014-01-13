using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class DraftingPushScenario : SimScenario, IHasSkill
    {
        public DraftingPushScenario()
        { }
        public DraftingPushScenario(SimDescription sim)
            : base (sim)
        { }
        protected DraftingPushScenario(DraftingPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Drafting";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.Painting, SkillNames.Styling };
            }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (!Skills.Allow(this, sim))
            {
                IncStat("Skill Denied");
                return false;
            }
            else if (!Careers.Allow(this, sim))
            {
                IncStat("Careers Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected bool TakeDraft(DraftingTable table)
        {
            table.Draft.FadeOut(true);

            if (!Inventories.TryToMove(table.Draft, Sim.CreatedSim))
            {
                table.Draft.Destroy();
                return false;
            }

            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<GameObject> tables = new List<GameObject>();
            List<GameObject> empties = new List<GameObject>();

            OccupationNames occupation = OccupationNames.Undefined;
            SkillNames skill = SkillNames.Painting;

            if (Sim.CreatedSim.Occupation is Stylist)
            {
                skill = SkillNames.Styling;
                occupation = OccupationNames.Stylist;
            }
            else if (Sim.CreatedSim.Occupation is InteriorDesigner)
            {
                occupation = OccupationNames.InteriorDesigner;
            }

            foreach (Lot lot in ManagerLot.GetOwnedLots(Sim.Household))
            {
                foreach (DraftingTable table in lot.GetObjects<DraftingTable>())
                {
                    if (table.Stool == null) continue;

                    if (table.InUse) continue;

                    DraftingTable.Canvas draft = table.Draft;
                    if (draft != null)
                    {
                        if (!table.IsDraftValid(Sim.CreatedSim, skill, occupation))
                        {
                            IncStat("Draft Mismatch");
                            continue;
                        }
                        else if ((draft.IsComplete) && (!TakeDraft(table)))
                        {
                            IncStat("Take Fail");
                            continue;
                        }
                        else
                        {
                            tables.Add(table);
                        }
                    }
                    else
                    {
                        empties.Add(table);
                    }
                }
            }

            if (tables.Count == 0)
            {
                tables.AddRange(empties);
            }

            if (tables.Count == 0)
            {
                IncStat("No Tables");
                return false;
            }

            GameObject choice = RandomUtil.GetRandomObjectFromList(tables);

            if (Sim.CreatedSim.Occupation is Stylist)
            {
                return Situations.PushInteraction(this, Sim, choice, DraftingTable.Research.SingletonStylist);
            }
            else if (Sim.CreatedSim.Occupation is InteriorDesigner)
            {
                return Situations.PushInteraction(this, Sim, choice, DraftingTable.Research.SingletonInteriorDesigner);
            }
            else
            {
                return Situations.PushInteraction(this, Sim, choice, DraftingTable.Research.SingletonPainting);
            }
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new DraftingPushScenario(this);
        }
    }
}
