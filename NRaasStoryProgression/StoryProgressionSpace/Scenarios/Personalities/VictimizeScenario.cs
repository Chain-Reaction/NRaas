using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
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
    public class VictimizeScenario : RelationshipScenario, IHasPersonality
    {
        WeightOption.NameOption mName = null;

        string mReason;

        bool mFail;

        FightScenarioHelper mFight = null;

        Dictionary<TraitNames, string> mTraitReasons = null;

        Dictionary<CASAgeGenderFlags, string> mAgeGenderReasons = null;

        public VictimizeScenario()
            : base(-80)
        { }
        protected VictimizeScenario(VictimizeScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mReason = scenario.mReason;
            mFight = scenario.mFight;
            mTraitReasons = scenario.mTraitReasons;
            mAgeGenderReasons = scenario.mAgeGenderReasons;
            //mFail = scenario.mFail;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Reason=" + mReason;

            text += Common.NewLine + "TraitReasons";
            foreach (KeyValuePair<TraitNames, string> traits in mTraitReasons)
            {
                text += Common.NewLine + traits.Key + ":" + traits.Value;
            }

            text += Common.NewLine + "AgeGenderReasons";
            foreach (KeyValuePair<CASAgeGenderFlags, string> traits in mAgeGenderReasons)
            {
                text += Common.NewLine + traits.Key + ":" + traits.Value;
            }

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
                return mReason;
            }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        public override bool IsFriendly
        {
            get { return false; }
        }

        protected override bool CheckBusy
        {
            get { return base.ShouldPush; }
        }

        public override bool ShouldPush
        {
            get
            {
                return mFight.ShouldPush(mFail, base.ShouldPush);
            }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mFight = new FightScenarioHelper(Origin.FromWatchingSimSuffer, SimDescription.DeathType.OldAge);
            if (!mFight.Parse(row, Manager, this, ref error))
            {
                return false;
            }

            mTraitReasons = new Dictionary<TraitNames, string>();

            int index = 0;
            while (true)
            {
                if (!row.Exists("ReasonTrait" + index)) break;

                TraitNames trait;
                if (!ParserFunctions.TryParseEnum<TraitNames>(row.GetString("ReasonTrait" + index), out trait, TraitNames.Unknown))
                {
                    error = "ReasonTrait" + index + " unknown";
                    return false;
                }

                if (mTraitReasons.ContainsKey(trait))
                {
                    error = "ReasonTrait " + trait + " already found";
                    return false;
                }

                if (!row.Exists("ReasonTraitName" + index))
                {
                    error = "ReasonTraitName" + index + " missing";
                    return false;
                }

                string text = row.GetString("ReasonTraitName" + index);

                mTraitReasons.Add(trait, text);

                index++;
            }

            mAgeGenderReasons = new Dictionary<CASAgeGenderFlags, string>();

            index = 0;
            while (true)
            {
                if (!row.Exists("ReasonAgeGender" + index)) break;

                CASAgeGenderFlags ageGender;
                if (!ParserFunctions.TryParseEnum<CASAgeGenderFlags>(row.GetString("ReasonAgeGender" + index), out ageGender, CASAgeGenderFlags.None))
                {
                    error = "ReasonAgeGender" + index + " unknown";
                    return false;
                }

                if (mAgeGenderReasons.ContainsKey(ageGender))
                {
                    error = "ReasonAgeGender " + ageGender + " already found";
                    return false;
                }

                if (!row.Exists("ReasonAgeGenderName" + index))
                {
                    error = "ReasonAgeGenderName" + index + " missing";
                    return false;
                }

                string text = row.GetString("ReasonAgeGenderName" + index);

                mAgeGenderReasons.Add(ageGender, text);

                index++;
            }

            if ((mTraitReasons.Count == 0) && (mAgeGenderReasons.Count == 0))
            {
                error = "No Reasons found";
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
            ICollection<SimDescription> potentials = GetFilteredTargets(sim);

            List<SimDescription> targets = new List<SimDescription>();
            foreach (SimDescription potential in potentials)
            {
                if (GetReason(potential) == null) continue;

                targets.Add(potential);
            }

            return targets;
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

        protected override bool TargetAllow(SimDescription target)
        {
            if ((target.Gender == CASAgeGenderFlags.Female) && (Sim.Gender == CASAgeGenderFlags.Male) && (!GetValue<AntagonizeScenario.AllowMaleFemaleVictimOptionV2, bool>()))
            {
                IncStat("Male-Female Denied");
                return false;
            }

            return base.TargetAllow(target);
        }

        protected string GetReason(SimDescription sim)
        {
            List<string> choices = new List<string>();

            foreach (KeyValuePair<TraitNames, string> reason in mTraitReasons)
            {
                if (reason.Key == TraitNames.Unknown)
                {
                    choices.Add(reason.Value);
                }
                else if (sim.TraitManager.HasElement(reason.Key))
                {
                    choices.Add(reason.Value);
                }
            }

            foreach (KeyValuePair<CASAgeGenderFlags, string> reason in mAgeGenderReasons)
            {
                if (reason.Key == CASAgeGenderFlags.None)
                {
                    choices.Add(reason.Value);
                }
                else
                {
                    bool match = true;

                    if ((reason.Key & CASAgeGenderFlags.AgeMask) != CASAgeGenderFlags.None)
                    {
                        if ((reason.Key & sim.Age) != sim.Age)
                        {
                            match = false;
                        }
                    }

                    if ((reason.Key & CASAgeGenderFlags.GenderMask) != CASAgeGenderFlags.None)
                    {
                        if ((reason.Key & sim.Gender) != sim.Gender)
                        {
                            match = false;
                        }
                    }

                    if ((reason.Key & CASAgeGenderFlags.SpeciesMask) != CASAgeGenderFlags.None)
                    {
                        if ((reason.Key & sim.Species) != sim.Species)
                        {
                            match = false;
                        }
                    }

                    if (match)
                    {
                        choices.Add(reason.Value);
                    }
                }
            }

            if (choices.Count == 0)
            {
                return null;
            }

            return RandomUtil.GetRandomObjectFromList(choices);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            mReason = GetReason(Target);

            if (mReason == null)
            {
                IncStat("No Reason");
                return false;
            }

            if (!base.PrivateUpdate(frame)) return false;

            bool result = mFight.Perform(this, frame, Sim, Target, null, out mFail);
            if (mFail)
            {
                mReason = mName + "Fail";
            }

            return result;
        }

        protected override bool Push()
        {
            return Situations.PushMeetUp(this, Sim, Target, ManagerSituation.MeetUpType.Commercial, FirstAction);
        }

        public override Scenario Clone()
        {
            return new VictimizeScenario(this);
        }
    }
}
