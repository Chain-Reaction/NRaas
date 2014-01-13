using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
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
    public class TurnScenario : CreateOccultScenario, IViolentScenario
    {
        WeightOption.NameOption mName = null;

        bool mFail = false;

        FightScenarioHelper mFight = null;

        public TurnScenario()
            : base(-50, OccultTypes.Vampire)
        { }
        protected TurnScenario(TurnScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mFail = scenario.mFail;
            mFight = scenario.mFight;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Fight" + Common.NewLine + mFight;

            return text;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return mName.WeightName;
            }
            else
            {
                if (mFail)
                {
                    return mName + "Fail";
                }
                else if ((Target != null) && (Target.Child))
                {
                    return mName + "Child";
                }
                else
                {
                    return mName.ToString();
                }
            }
        }

        public override bool IsFriendly
        {
            get { return false; }
        }

        public bool IsViolent
        {
            get { return true; }
        }

        public override bool ShouldPush
        {
            get
            {
                return mFight.ShouldPush(mFail, base.ShouldPush);
            }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mFight = new FightScenarioHelper(Origin.FromWatchingSimSuffer, SimDescription.DeathType.Thirst);
            if (!mFight.Parse(row, Manager, this, ref error))
            {
                return false;
            }

            return base.Parse(row, ref error);
        }

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mFight.Perform(this, frame, Sim, Target, null, out mFail)) return false;

            if (!mFail)
            {
                return base.PrivateUpdate(frame);
            }
            else
            {
                return AlterRelationship();
            }
        }

        public override Scenario Clone()
        {
            return new TurnScenario(this);
        }
    }
}
