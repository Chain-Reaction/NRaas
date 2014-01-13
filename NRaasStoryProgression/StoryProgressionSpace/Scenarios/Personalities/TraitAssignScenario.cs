using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
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
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Personalities
{
    public class TraitAssignScenario : DualSimScenario, IHasPersonality
    {
        WeightOption.NameOption mName = null;

        string mTrait;

        WeightScenarioHelper mSuccess = null;

        public TraitAssignScenario()
        { }
        protected TraitAssignScenario(TraitAssignScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mTrait = scenario.mTrait;
            mSuccess = scenario.mSuccess;
        }

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Trait=" + mTrait;
            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;

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
                return mName.ToString();
            }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return base.ShouldPush; }
        }

        public override bool ShouldPush
        {
            get
            {
                return mSuccess.ShouldPush(base.ShouldPush);
            }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mSuccess = new WeightScenarioHelper(Origin.FromSocialization);
            if (!mSuccess.Parse(row, Manager, this, "Success", ref error))
            {
                return false;
            }

            return base.Parse(row, ref error);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return GetFilteredSims(Personalities.GetClanLeader(Manager));
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return GetFilteredTargets(sim);
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Personalities.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mSuccess.TestBeforehand(Manager, Sim, Target))
            {
                IncStat("Success TestBeforehand Fail");
                return false;
            }

            List<Trait> choices = new List<Trait>();

            foreach (Trait trait in Sim.TraitManager.List)
            {
                if (trait.IsReward) continue;

                if (trait.IsHidden) continue;

                choices.Add(trait);
            }

            Trait choice = null;
            while (choices.Count > 0)
            {
                choice = RandomUtil.GetRandomObjectFromList(choices);
                choices.Remove(choice);

                if (!Target.TraitManager.HasElement(choice.Guid))
                {
                    break;
                }

                choice = null;
            }

            if (choice != null)
            {
                Trait remove = null;

                choices.Clear();
                foreach (Trait trait in Target.TraitManager.List)
                {
                    if (trait.IsReward) continue;

                    if (trait.IsHidden) continue;

                    choices.Add(trait);
                }

                if (choices.Count > 0)
                {
                    remove = RandomUtil.GetRandomObjectFromList(choices);
                }

                if (remove != null)
                {
                    Target.TraitManager.RemoveElement(remove.Guid);
                    Target.TraitManager.AddElement(choice.Guid);

                    mTrait = choice.ToString();

                    mSuccess.Perform(this, frame, "Success", Sim, Target);
                    return true;
                }
            }
            return false;
        }

        protected override bool Push()
        {
            return Situations.PushMeetUp(this, Sim, Target, ManagerSituation.MeetUpType.Commercial, ManagerFriendship.FriendlyFirstAction);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                parameters = new object[] { Sim, Target, mTrait };
            }

            if (extended == null)
            {
                extended = new string[] { mTrait };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new TraitAssignScenario(this);
        }
    }
}
