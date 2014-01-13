using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.CommonSpace.Scoring;
using NRaas.StoryProgressionSpace.Scenarios.Skills;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
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
    public class RequestRepairmanScenario : LotScenario
    {
        public RequestRepairmanScenario()
        { }
        public RequestRepairmanScenario(Lot lot)
            : base(lot)
        { }
        protected RequestRepairmanScenario(RequestRepairmanScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "RequestRepairman";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool> ()) return false;

            if (ManagerCareer.HasSkillCareer(Household.ActiveHousehold, SkillNames.Handiness))
            {
                IncStat("Active Repairman");
                return false;
            }

            return base.Allow();
        }

        protected override bool Allow(Lot lot)
        {
            if (lot.IsActive)
            {
                IncStat("Active");
                return false;
            }
            else if (lot.Household == null)
            {
                IncStat("No Family");
                return false;
            }
            else if (lot.IsWorldLot)
            {
                IncStat("World Lot");
                return false;
            }
            else if (!lot.IsResidentialLot)
            {
                IncStat("Commercial");
                return false;
            }

            return base.Allow(lot);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Dictionary<Household, List<GameObject>> repairs = new Dictionary<Household, List<GameObject>>();
            ScheduledRepairScenario.GetRepairs(Manager, Lot.GetObjects<GameObject>(), repairs);

            List<GameObject> repairWork = null;
            if (repairs.TryGetValue(Lot.Household, out repairWork))
            {
                AddStat("Broken Count", repairWork.Count);

                if (repairWork.Count < 4)
                {
                    IncStat("Not enough broken");
                    return false;
                }
            }
            else
            {
                IncStat("No Work");
                return false;
            }

            return ScheduledRepairScenario.PushForRepairman(this, Manager, Lot.Household);
        }

        public override Scenario Clone()
        {
            return new RequestRepairmanScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerLot, RequestRepairmanScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "RequestRepairman";
            }
        }
    }
}
