using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Options.Immigration;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class BirthScenario : SimEventScenario<PregnancyEvent>, IFormattedStoryScenario
    {
        SimDescription mDad;

        List<SimDescription> mBabies = null;

        public BirthScenario()
        { }
        protected BirthScenario(BirthScenario scenario)
            : base (scenario)
        {
            mDad = scenario.mDad;

            if (scenario.mBabies != null)
            {
                mBabies = new List<SimDescription>(scenario.mBabies);
            }
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Spawn";
        }

        public Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Pregnancies;
        }

        public SimDescription Dad
        {
            get
            {
                return mDad;
            }
        }

        public List<SimDescription> Babies
        {
            get { return mBabies; }
        }

        protected override int ReportDelay
        {
            get { return 30; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            events.AddListener(this, EventTypeId.kHadBaby);
            events.AddListener(this, EventTypeId.kPetHadOffspring);
            return true;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            PregnancyEvent pe = e as PregnancyEvent;
            if (pe == null)
            {
                return null;
            }

            if ((pe.Babies == null) || (pe.Babies.Count == 0))
            {
                return null;
            }

            Sim mom = pe.Actor as Sim;
            if (mom == null)
            {
                return null;
            }

            if (mom.SimDescription == null)
            {
                return null;
            }

            List<SimDescription> parents = Relationships.GetParents(pe.Babies[0].SimDescription);
            if (parents.Count == 0)
            {
                return null;
            }

            SimDescription actualMom = null;
            foreach (SimDescription parent in parents)
            {
                if (parent.Pregnancy == pe.Pregnancy)
                {
                    actualMom = parent;
                    break;
                }
            }

            if (actualMom == null)
            {
                actualMom = parents[0];
            }

            // Stops the second PregnancyEvent fired for the father from being picked up
            if (actualMom != mom.SimDescription)
            {
                return null;
            }

            mBabies = new List<SimDescription>();
            foreach (Sim baby in pe.Babies)
            {
                mBabies.Add(baby.SimDescription);
            }

            Scenario scenario = base.Handle(e, ref result);
            if (scenario == null)
            {
                return null;
            }

            return scenario;
        }

        public static event UpdateDelegate OnRenameNewbornsScenario;
        public static event UpdateDelegate OnBirthScenario;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Common.StringBuilder msg = new Common.StringBuilder("PrivateUpdate");

            try
            {
                bool fullList = !GetValue<CustomNamesOnlyOption<ManagerLot>, bool>();

                foreach (SimDescription baby in mBabies)
                {
                    msg += Common.NewLine + baby.FullName;

                    if (!SimTypes.IsSelectable(baby))
                    {
                        baby.FirstName = Sims.EnsureUniqueName(baby);
                    }

                    List<SimDescription> parents = Relationships.GetParents(baby);

                    parents.Remove(Sim);
                    if ((mDad == null) && (parents.Count > 0))
                    {
                        mDad = parents[0];
                    }

                    msg += Common.NewLine + "A";

                    List<OccultTypes> choices = OccultTypeHelper.CreateList(Sim.OccultManager.CurrentOccultTypes, true);

                    // Grandparent occult inheritance
                    foreach (SimDescription parent in Relationships.GetParents(Sim))
                    {
                        if (parent.OccultManager == null) continue;

                        choices.AddRange(OccultTypeHelper.CreateList(parent.OccultManager.CurrentOccultTypes, true));
                    }

                    msg += Common.NewLine + "B";

                    if ((mDad != null) && (mDad.OccultManager != null))
                    {
                        choices.AddRange(OccultTypeHelper.CreateList(mDad.OccultManager.CurrentOccultTypes, true));

                        // Grandparent occult inheritance
                        foreach (SimDescription parent in Relationships.GetParents(mDad))
                        {
                            if (parent.OccultManager == null) continue;

                            choices.AddRange(OccultTypeHelper.CreateList(parent.OccultManager.CurrentOccultTypes, true));
                        }
                    }

                    msg += Common.NewLine + "C";

                    Sims.ApplyOccultChance(this, baby, choices, GetValue<ChanceOfOccultBabyOptionV2, int>(), GetValue<MaximumNewbornOccultOption, int>());

                    msg += Common.NewLine + "D";

                    if ((SimTypes.IsServiceAlien(mDad)) || (SimTypes.IsServiceAlien(Sim)))
                    {
                        baby.SetAlienDNAPercentage(1f);
                    }
                    else
                    {
                        baby.SetAlienDNAPercentage(SimDescription.GetAlienDNAPercentage(mDad, Sim, true));
                    }
                }

                msg += Common.NewLine + "E";

                if (OnRenameNewbornsScenario != null)
                {
                    OnRenameNewbornsScenario(this, frame);
                }

                if (!SimTypes.IsSelectable(Sim))
                {
                    ManagerSim.IncrementLifetimeHappiness(Sim, Sims3.Gameplay.Rewards.LifeEventRewards.kLifetimeHappinessRewardForBaby);
                }

                SetElapsedTime<DayOfLastBabyOption>(Sim);

                msg += Common.NewLine + "F";

                if (mDad != null)
                {
                    if (!SimTypes.IsSelectable(mDad))
                    {
                        ManagerSim.IncrementLifetimeHappiness(mDad, Sims3.Gameplay.Rewards.LifeEventRewards.kLifetimeHappinessRewardForBaby);
                    }

                    SetElapsedTime<DayOfLastBabyOption>(mDad);
                }

                msg += Common.NewLine + "G";

                foreach (SimDescription baby in mBabies)
                {
                    Manager.AddAlarm(new NormalBabyAgingScenario(baby));
                }

                msg += Common.NewLine + "H";

                if (OnBirthScenario != null)
                {
                    OnBirthScenario(this, frame);
                }

                msg += Common.NewLine + "I";

                // Do this at the end once everything else has done its thing
                foreach (SimDescription baby in mBabies)
                {
                    msg += Common.NewLine + baby.FullName;

                    HouseholdOptions houseData = GetHouseOptions(baby.Household);
                    if (houseData != null)
                    {
                        int netWorth = houseData.GetValue<NetWorthOption, int>();

                        foreach (SimDescription parent in Relationships.GetParents(baby))
                        {
                            SimData parentData = GetData(parent);
                            if (parentData != null)
                            {
                                foreach (CasteOptions caste in parentData.Castes)
                                {
                                    if (!caste.GetValue<CasteInheritedOption, bool>()) continue;

                                    if (!caste.Matches(baby, netWorth)) continue;

                                    SimData babyData = GetData(baby);
                                    if (babyData != null)
                                    {
                                        babyData.AddValue<ManualCasteOption, CasteOptions>(caste);
                                    }
                                }
                            }
                        }
                    }
                }

                msg += Common.NewLine + "J";

                Add(frame, new SuccessScenario(), ScenarioResult.Start);
            }
            catch(Exception e)
            {
                Common.Exception(ToString() + Common.NewLine + msg.ToString(), e);
            }
            return false;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (mDad != null) return null;

            if (manager == null)
            {
                manager = Pregnancies;
            }

            ManagerStory.Story story = null;
            foreach (SimDescription baby in mBabies)
            {
                story = base.PrintStory(manager, name, new object[] { baby, Sim }, extended, logging);

                if (story != null)
                {
                    story.mOverrideImage = "glb_tns_baby_coming_r2";
                }
            }

            return story;
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (mDad == null) return null;

            if (manager == null)
            {
                manager = Pregnancies;
            }

            if (extended == null)
            {
                List<string> extendedArray = new List<string>();
                foreach(SimDescription sim in mBabies)
                {
                    extendedArray.Add(sim.FullName);
                }

                extended = extendedArray.ToArray();
            }

            if (parameters == null)
            {
                parameters = new object[] { Sim, mDad };
            }

            if (Sim.IsHuman)
            {
                if (mBabies.Count == 4)
                {
                    text = Common.Localize("Pregnancy:Quadruplets", false, new object[] { mDad, Sim, mBabies[0], mBabies[1], mBabies[2], mBabies[3] });
                }
                else if (mBabies.Count == 3)
                {
                    text = Common.LocalizeEAString(false, "Gameplay/ActorSystems/Pregnancy:DadBabyBuffTriplets", new object[] { mDad, Sim, mBabies[0], mBabies[1], mBabies[2] });
                }
                else if (mBabies.Count == 2)
                {
                    text = Common.LocalizeEAString(false, "Gameplay/ActorSystems/Pregnancy:DadBabyBuffTwins", new object[] { mDad, Sim, mBabies[0], mBabies[1] });
                }
                else if (mBabies[0].IsMale)
                {
                    text = Common.LocalizeEAString(false, "Gameplay/ActorSystems/Pregnancy:DadBabyBuffBoy", new object[] { mDad, Sim, mBabies[0] });
                }
                else if (mBabies[0].IsFemale)
                {
                    text = Common.LocalizeEAString(false, "Gameplay/ActorSystems/Pregnancy:DadBabyBuffGirl", new object[] { mDad, Sim, mBabies[0] });
                }
            }
            else
            {
                if (mBabies.Count == 4)
                {
                    text = Common.Localize("PetPregnancy:Quadruplets" + Sim.Species, false, new object[] { mDad, Sim, Sim.Household.Name });
                }
                else if (mBabies.Count == 3)
                {
                    text = Common.Localize("PetPregnancy:Triplets" + Sim.Species, false, new object[] { mDad, Sim, Sim.Household.Name });
                }
                else if (mBabies.Count == 2)
                {
                    text = Common.Localize("PetPregnancy:Twins" + Sim.Species, mBabies[0].IsFemale, mBabies[1].IsFemale, new object[] { mDad, Sim, Sim.Household.Name });
                }
                else
                {
                    text = Common.Localize("PetPregnancy:Single" + Sim.Species, mBabies[0].IsFemale, new object[] { mDad, Sim, Sim.Household.Name });
                }
            }

            ManagerStory.Story story = base.PrintFormattedStory(manager, text, summaryKey, parameters, extended, logging);

            if (story != null)
            {
                story.mOverrideImage = "glb_tns_baby_coming_r2";
            }

            return story;
        }

        public override Scenario Clone()
        {
            return new BirthScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerPregnancy, BirthScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "Birth";
            }
        }

        public class ChanceOfOccultBabyOptionV2 : IntegerBaseManagerOptionItem<ManagerPregnancy>, IGeneralOption
        {
            public ChanceOfOccultBabyOptionV2()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "ChanceOfHybridOffspring";
            }
        }

        public class MaximumNewbornOccultOption : IntegerBaseManagerOptionItem<ManagerPregnancy>, IGeneralOption
        {
            public MaximumNewbornOccultOption()
                : base(2)
            { }

            public override string GetTitlePrefix()
            {
                return "MaximumNewbornOccult";
            }
        }
    }
}
