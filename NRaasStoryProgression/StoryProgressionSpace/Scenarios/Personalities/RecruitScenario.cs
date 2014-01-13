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
    public class RecruitScenario : RelationshipScenario, IHasPersonality
    {
        WeightOption.NameOption mName = null;

        bool mAllowSteal = false;

        WeightScenarioHelper mSuccess = null;
        WeightScenarioHelper mFailure = null;

        bool mFail = false;

        public RecruitScenario()
            : base(50)
        { }
        protected RecruitScenario(RecruitScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mAllowSteal = scenario.mAllowSteal;
            mSuccess = scenario.mSuccess;
            mFailure = scenario.mFailure;
            // mFail = scenario.mFail;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;
            text += Common.NewLine + "Failure" + Common.NewLine + mFailure;
            text += Common.NewLine + "AllowSteal=" + mAllowSteal;

            return text;
        }

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
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

        public override bool IsFriendly
        {
            get { return true; }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected override bool CheckBusy
        {
            get { return false; } //  base.ShouldPush; }
        }

        public override bool ShouldPush
        {
            get
            {
                if (mFail)
                {
                    return mFailure.ShouldPush(base.ShouldPush);
                }
                else
                {
                    return mSuccess.ShouldPush(base.ShouldPush);
                }
            }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mAllowSteal = row.GetBool("AllowSteal");

            mSuccess = new WeightScenarioHelper(Origin.FromSocialization);
            if (!mSuccess.Parse(row, Manager, this, "Success", ref error))
            {
                return false;
            }

            mFailure = new WeightScenarioHelper(Origin.FromSocialization);
            if (!mFailure.Parse(row, Manager, this, "Failure", ref error))
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

        protected override bool TargetAllow(SimDescription sim)
        {
            SimPersonality clan = Manager as SimPersonality;

            if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (GetData(sim).IsClan(clan))
            {
                IncStat("Already Clan");
                return false;
            }
            else if (!clan.CanAdd(this, sim, mAllowSteal))
            {
                IncStat("Opposing");
                return false;
            }
            else if (!clan.TestMemberRetention(sim))
            {
                IncStat("Retention Fail");
                return false;
            }

            return base.TargetAllow(sim);
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

            if (!mFailure.TestBeforehand(Manager, Sim, Target))
            {
                IncStat("Failure TestBeforehand Fail");
                return false;
            }

            if (!base.PrivateUpdate(frame)) return false;

            SimPersonality clan = Manager as SimPersonality;
            if (clan == null)
            {
                IncStat("No Clan");
                return false;
            }

            mFail = !clan.AddToClan(this, Target, mAllowSteal);

            if (mFail)
            {
                mFailure.Perform(this, frame, "Failure", Sim, Target);
                return false;
            }
            else
            {
                mSuccess.Perform(this, frame, "Success", Sim, Target);
                return true;
            }
        }

        protected override bool Push()
        {
            return Situations.PushMeetUp(this, Sim, Target, ManagerSituation.MeetUpType.Commercial, FirstAction);
        }

        public override Scenario Clone()
        {
            return new RecruitScenario(this);
        }
    }
}
