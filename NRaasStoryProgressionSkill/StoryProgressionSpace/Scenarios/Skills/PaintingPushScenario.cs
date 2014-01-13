using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class PaintingPushScenario : SimSingleProcessScenario, IHasSkill
    {
        public PaintingPushScenario()
        { }
        protected PaintingPushScenario(PaintingPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Painting";
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.Painting };
            }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return new SimScoringList(this, "Painting", Sims.All, false).GetBestByMinScore(1);
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
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situations Denied");
                return false;
            }
            else if ((sim.IsEP11Bot) && (!sim.HasTrait(TraitNames.ArtisticAlgorithmsChip)))
            {
                IncStat("Chip Denied");
                return false;
            }

            return base.Allow(sim);
        }

        public static void TakePainting(Easel ths, Sim sim)
        {
            EaselCanvas canvas = ths.CurrentCanvas;
            if (canvas == null) return;

            bool flag = Inventories.TryToMove(canvas, sim);
            canvas.DetachFromEasel();
            canvas.EnableDropShadow();
            ths.CanvasHasBeenTaken();
            canvas.RemoveFromUseList(sim);
            if (!flag)
            {
                canvas.Destroy();
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            List<GameObject> easels = new List<GameObject>();
            List<GameObject> empties = new List<GameObject>();

            /*
            foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
            {
                foreach (DraftingTable table in lot.GetObjects<DraftingTable>())
                {
                    if (table.Stool == null) continue;

                    if (table.InUse) continue;

                    DraftingTable.Canvas draft = table.Draft;
                    if (draft != null) 
                    {                        
                        if (!table.IsDraftValid(Sim.CreatedSim, SkillNames.Painting, OccupationNames.Undefined))
                        {
                            continue;
                        }

                        if (draft.IsComplete)
                        {
                            if (!Situations.PushInteraction(Sim, table, DraftingTable.TakeDraft.Singleton))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            easels.Add(table);
                        }
                    }
                    else
                    {
                        empties.Add(table);
                    }
                }
            }
            if (easels.Count == 0)
            {
                easels.AddRange(empties);
            }
            */

            if (easels.Count == 0)
            {
                foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
                {
                    foreach (Easel easel in lot.GetObjects<Easel>())
                    {
                        if (easel.CanSell(Sim.CreatedSim))
                        {
                            TakePainting(easel, Sim.CreatedSim);
                        }

                        if (!easel.CanPaint(Sim.CreatedSim)) continue;

                        if (easel.CurrentCanvas != null)
                        {
                            easels.Add(easel);
                        }
                        else
                        {
                            empties.Add(easel);
                        }
                    }
                }

                if (easels.Count == 0)
                {
                    easels.AddRange(empties);
                }
            }

            if (easels.Count == 0)
            {
                IncStat("No Easel");
                return false;
            }

            GameObject choice = RandomUtil.GetRandomObjectFromList(easels);

            if (choice is Easel)
            {
                return Situations.PushInteraction(this, Sim, choice, PaintEx.Singleton);
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
            return new PaintingPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, PaintingPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PaintingPush";
            }
        }
    }
}
