using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class ReplaceItemsScenario : LotScenario
    {
        public ReplaceItemsScenario(Lot lot)
            : base (lot)
        { }
        protected ReplaceItemsScenario(ReplaceItemsScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ReplaceLot";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(Lot lot)
        {
            if (lot == Household.ActiveHouseholdLot)
            {
                IncStat("Active");
                return false;
            }

            if (lot.CountObjects<JunkyardSpawner>() > 0)
            {
                IncStat("Junkyard");
                return false;
            }

            return base.Allow(lot);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Household house = Lot.Household;

            foreach (GameObject obj in Lot.GetObjects<GameObject>())
            {
                if (!obj.Charred)
                {
                    RepairableComponent repair = obj.Repairable;
                    if (repair == null) continue;

                    if (!repair.TestReplace()) continue;
                }
                else
                {
                    if (obj is PlumbBob)
                    {
                        obj.Charred = false;
                        continue;
                    }
                }

                try
                {
                    IncStat("Found: " + obj.CatalogName);
                }
                catch
                { }

                GameObject go = null;
                try
                {
                    go = RepairableComponent.CreateReplaceObject(obj);
                }
                catch (Exception e)
                {
                    Common.DebugException(obj.CatalogName, e);
                }
                if (go == null) continue;

                IncStat("Replaced");

                if (house != null)
                {
                    Money.AdjustFunds(house, "Replacement", -obj.Cost);
                }
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new ReplaceItemsScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerLot>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ReplaceLot";
            }
        }
    }
}
