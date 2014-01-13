using NRaas.CommonSpace.Helpers;
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
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.Seating;
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
    public class PrankScenario : RelationshipScenario, IHasPersonality
    {
        static Origin[] sTypes = new Origin[] { Origin.FromPrankComputer, Origin.FromPrankDoorbellDitch, Origin.FromPrankFlamingBag, Origin.FromPrankShower, Origin.FromPrankSink, Origin.FromPrankThrowingEggs, Origin.FromPrankToilet, Origin.FromPrankWhoopeeCushion };

        WeightOption.NameOption mName = null;

        string mSneakinessScoring = null;

        bool mChildStory = false;

        bool mAllowDoorPranks = false;

        WeightScenarioHelper mSuccess = null;
        WeightScenarioHelper mFailure = null;

        bool mFail = false;

        public PrankScenario()
            : base(50)
        { }
        protected PrankScenario(PrankScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mSuccess = scenario.mSuccess;
            mFailure = scenario.mFailure;
            mChildStory = scenario.mChildStory;
            mSneakinessScoring = scenario.mSneakinessScoring;
            mAllowDoorPranks = scenario.mAllowDoorPranks;
            // mFail = scenario.mFail;
        }

        protected override bool TargetAllowActive
        {
            get
            {
                return GetValue<AllowActiveTargetOption,bool>();
            }
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "ChildStory=" + mChildStory;
            text += Common.NewLine + "SneakinessScoring=" + mSneakinessScoring;
            text += Common.NewLine + "AllowDoorPranks=" + mAllowDoorPranks;
            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;
            text += Common.NewLine + "Failure" + Common.NewLine + mFailure;            

            return text;
        }

        protected override bool Allow()
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP4)) return false;

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
                if (mFail)
                {
                    return mName + "Fail";
                }
                else if ((mChildStory) && (Target != null) && (Target.Child))
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

            if (row.Exists("ChildStory"))
            {
                mChildStory = row.GetBool("ChildStory");
            }

            if (row.Exists("AllowDoorPranks"))
            {
                mAllowDoorPranks = row.GetBool("AllowDoorPranks");
            }            

            mSneakinessScoring = row.GetString("SneakinessScoring");

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
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("No LotHome");
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

            mFail = false;
            if (!string.IsNullOrEmpty(mSneakinessScoring))
            {
                foreach (SimDescription member in CommonSpace.Helpers.Households.All(Target.Household))
                {
                    if (AddScoring("Target Sneak", ScoringLookup.GetScore(mSneakinessScoring, member)) > AddScoring("Sim Sneak", ScoringLookup.GetScore(mSneakinessScoring, Sim)))
                    {
                        mFail = true;
                        break;
                    }
                }
            }

            if (!base.PrivateUpdate(frame)) return false;

            if (mFail)
            {
                if (!Situations.PushVisit(this, Sim, Target.LotHome))
                {
                    IncStat("Push Fail");
                    return false;
                }

                mFailure.Perform(this, frame, "Failure", Sim, Target);
                return true;
            }
            else
            {
                List<Origin> types = new List<Origin>(sTypes);

                while (types.Count > 0)
                {
                    Origin type = RandomUtil.GetRandomObjectFromList(types);
                    types.Remove(type);

                    IncStat("Try " + type);

                    GameObject boobyTrap = null;

                    switch (type)
                    {
                        case Origin.FromPrankDoorbellDitch:
                            if (!mAllowDoorPranks) continue;

                            if (Situations.PushInteraction(this, Sim, Target.LotHome, Door.PrankDoorbellDitch.Singleton))
                            {
                                return true;
                            }
                            break;
                        case Origin.FromPrankFlamingBag:
                            if (!mAllowDoorPranks) continue;

                            if (Situations.PushInteraction(this, Sim, Target.LotHome, Door.PrankFlamingBag.Singleton))
                            {
                                return true;
                            }
                            break;
                        case Origin.FromPrankThrowingEggs:
                            if (!mAllowDoorPranks) continue;

                            if (Situations.PushInteraction(this, Sim, Target.LotHome, Door.PrankThrowingEggs.Singleton))
                            {
                                return true;
                            }
                            break;
                        case Origin.FromPrankToilet:
                            Toilet[] toilets = Target.LotHome.GetObjects<Toilet>();
                            if (toilets.Length > 0)
                            {
                                boobyTrap = RandomUtil.GetRandomObjectFromList(toilets);
                            }
                            break;
                        case Origin.FromPrankSink:
                            Sink[] sinks = Target.LotHome.GetObjects<Sink>();
                            if (sinks.Length > 0)
                            {
                                boobyTrap = RandomUtil.GetRandomObjectFromList(sinks);
                            }
                            break;
                        case Origin.FromPrankShower:
                            Shower[] showers = Target.LotHome.GetObjects<Shower>();
                            if (showers.Length > 0)
                            {
                                boobyTrap = RandomUtil.GetRandomObjectFromList(showers);
                            }
                            break;
                        case Origin.FromPrankWhoopeeCushion:
                            ChairDining[] chairs = Target.LotHome.GetObjects<ChairDining>();
                            if (chairs.Length > 0)
                            {
                                boobyTrap = RandomUtil.GetRandomObjectFromList(chairs);
                            }
                            break;
                        case Origin.FromPrankComputer:
                            Computer[] computers = Target.LotHome.GetObjects<Computer>();
                            if (computers.Length > 0)
                            {
                                if (Situations.PushInteraction(this, Sim, RandomUtil.GetRandomObjectFromList(computers), Computer.SetBoobyTrap.Singleton))
                                {
                                    mSuccess.Perform(this, frame, "Success", Sim, Target);
                                    return true;
                                }
                            }
                            break;
                    }

                    if (boobyTrap != null)
                    {
                        if (Situations.PushInteraction(this, Sim, boobyTrap, SetBoobyTrap.Singleton))
                        {
                            mSuccess.Perform(this, frame, "Success", Sim, Target);
                            return true;
                        }
                    }
                }

                IncStat("No Choices");
                return false;
            }
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new PrankScenario(this);
        }

        public class AllowActiveTargetOption : BooleanManagerOptionItem<ManagerPersonality>
        {
            public AllowActiveTargetOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowActiveTargetPrank";
            }

            public override bool Install(ManagerPersonality main, bool initial)
            {
                if (initial)
                {
                    InteractionTuning tuning = Tunings.GetTuning<IBoobyTrap, SetBoobyTrap.Definition>();
                    if (tuning != null)
                    {
                        tuning.Availability.Adults = true;
                        tuning.Availability.Elders = true;
                    }

                    tuning = Tunings.GetTuning<Computer, Computer.SetBoobyTrap.Definition>();
                    if (tuning != null)
                    {
                        tuning.Availability.Adults = true;
                        tuning.Availability.Elders = true;
                    }
                }

                return base.Install(main, initial);
            }
        }
    }
}
