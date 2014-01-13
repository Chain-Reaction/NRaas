using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Deaths;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Personalities
{
    public class SlayScenario : AntagonizeScenario
    {
        public SlayScenario()
            : base(-50)
        { }
        protected SlayScenario(SlayScenario scenario)
            : base (scenario)
        { }

        protected override SimDescription.DeathType DeathType
        {
            get { return SimDescription.DeathType.Thirst; }
        }

        protected override bool TargetAllow(SimDescription target)
        {
            if (Target.TeenOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (Target.OccultManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (OccultTypeHelper.CreateList(target, true).Count == 0)
            {
                IncStat("Not Occult");
                return false;
            }
            else if (!GetValue<AllowPersonalityOccultOption, bool>(target))
            {
                IncStat("Occult Denied");
                return false;
            }

            return base.TargetAllow(target);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            if ((!mFail) && (GetValue<AllowRevertOption,bool>()))
            {
                foreach (OccultTypes type in OccultTypeHelper.CreateList(Target, true))
                {
                    OccultTypeHelper.Remove(Target, type, true);
                }
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new SlayScenario(this);
        }

        public class AllowRevertOption : BooleanManagerOptionItem<ManagerPersonality>
        {
            public AllowRevertOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowVampireRevert";
            }
        }
    }
}
