using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Options.Immigration;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class ImmigrateScenario : ScheduledSoloScenario
    {
        ManagerLot.ImmigrationRequirement mRequirement;

        bool mCheckRequired;

        public ImmigrateScenario(ManagerLot.ImmigrationRequirement requirement, bool checkRequired)
        {
            mRequirement = requirement;
            mCheckRequired = checkRequired;
        }
        protected ImmigrateScenario(ImmigrateScenario scenario)
            : base (scenario)
        {
            mRequirement = scenario.mRequirement;
            mCheckRequired = scenario.mCheckRequired;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Immigrate";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected ManagerLot.CheckResult OnLotPriceCheck(Common.IStatGenerator stats, Lot lot, int currentLotCost, int availableFunds)
        {
            Vector2 range = GetValue<ImmigrantMoveInScenario.LotPriceRangeOption, Vector2>();
            if ((range.x == 0) && (range.y == 0)) return ManagerLot.CheckResult.IgnoreCost;

            int cost = Lots.GetLotCost(lot);
            if ((range.x <= cost) && (cost <= range.y))
            {
                return ManagerLot.CheckResult.IgnoreCost;
            }
            else
            {
                stats.IncStat("Out of Range");
                return ManagerLot.CheckResult.Failure;
            }
        }

        public static int CountEmptyHomes()
        {
            int count = 0;

            foreach (Lot lot in LotManager.AllLots)
            {
                if (!lot.IsResidentialLot) continue;

                Lot.LotMetrics metrics = new Lot.LotMetrics();
                lot.GetLotMetrics(ref metrics);

                if (metrics.FridgeCount <= 0) continue;

                if (metrics.BedCount <= 0) continue;

                count++;
            }

            return count;
        }

        public static bool TestEmptyHomes(Scenario manager)
        {
            return (CountEmptyHomes() >= manager.GetValue<MinimumEmptyHomesOption, int>());
        }

        protected override bool Allow()
        {
            if (Sims.HasEnough(this, CASAgeGenderFlags.Human))
            {
                IncStat("Town Full");
                return false;
            }
            else if (!TestEmptyHomes(this))
            {
                IncStat("Too Few Empty");
                return false;
            }
            else if (Lots.FindLot(this, null, 0, ManagerLot.FindLotFlags.Inspect | ManagerLot.FindLotFlags.CheapestHome, OnLotPriceCheck) == null)
            {
                IncStat("No Empty");
                return false;
            }

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (mCheckRequired)
            {
                if (!mRequirement.mRequired) return false;
            }

            List<CASAgeGenderFlags> ageGenderRequirements = new List<CASAgeGenderFlags>();

            if ((!mRequirement.mNeedMale) && (!mRequirement.mNeedFemale))
            {
                if (BabyGenderScenario.GetGenderByFirstBorn(Manager, GetValue<GenderOption, BabyGenderScenario.FirstBornGender>(), false) == CASAgeGenderFlags.Male)
                {
                    mRequirement.mNeedMale = true;
                }
                else
                {
                    mRequirement.mNeedFemale = true;
                }
            }

            bool teenMoveIn = false;
            if (mRequirement.mMate == null)
            {
                if (RandomUtil.RandomChance(GetValue<TeenChanceOption,int>()))
                {
                    teenMoveIn = true;
                }
            }
            else if (mRequirement.mMate.Teen)
            {
                teenMoveIn = true;
            }

            if (mRequirement.mNeedMale)
            {
                CASAgeGenderFlags age = CASAgeGenderFlags.Male | CASAgeGenderFlags.Human;
                if (teenMoveIn)
                {
                    age |= CASAgeGenderFlags.Teen;
                }
                else //if ((mRequirement.CareerLevel != null) && (mRequirement.CareerLevel.Level > 5))
                {
                    age |= CASAgeGenderFlags.YoungAdult;
                    age |= CASAgeGenderFlags.Adult;
                }

                ageGenderRequirements.Add(age);
            }

            if (mRequirement.mNeedFemale)
            {
                CASAgeGenderFlags age = CASAgeGenderFlags.Female | CASAgeGenderFlags.Human;
                if (teenMoveIn)
                {
                    age |= CASAgeGenderFlags.Teen;
                }
                else if (mRequirement.mFertile)
                {
                    age |= CASAgeGenderFlags.YoungAdult;                    
                }
                else //((mRequirement.CareerLevel != null) && (mRequirement.CareerLevel.Level > 5) && )
                {
                    age |= CASAgeGenderFlags.YoungAdult;
                    age |= CASAgeGenderFlags.Adult;
                }

                ageGenderRequirements.Add(age);
            }

            bool singleParent = false;

            Lot familyLot = null;

            if (((mRequirement.mMate == null) || (!mCheckRequired)) && (GetValue<WholeFamilyOption,int>() > 0) && (!RandomUtil.RandomChance(GetValue<SingleSimOption,int>())))
            {
                IncStat("WholeFamily");

                familyLot = Lots.FindLot(this, null, 0, ManagerLot.FindLotFlags.CheapestHome | ManagerLot.FindLotFlags.Inspect, OnLotPriceCheck);
                if (familyLot == null) return false;

                if (RandomUtil.RandomChance(GetValue<ChanceOfSingleOption, int>()))
                {
                    IncStat("Single Parent");
                    singleParent = true;
                }
                else if (familyLot.CountObjects<IBedDouble>() == 0)
                {
                    if (familyLot.CountObjects<IBedSingle>() > 1)
                    {
                        IncStat("Single Parent");
                        singleParent = true;
                    }
                    else
                    {
                        IncStat("No Double Bed");
                        familyLot = null;
                    }
                }
            }

            using (SimFromBin<ManagerLot> simBin = new SimFromBin<ManagerLot>(this, Lots))
            {
                List<SimDescription> sims = new List<SimDescription>();

                CASAgeGenderFlags genders = CASAgeGenderFlags.Male | CASAgeGenderFlags.Female;

                if (familyLot != null)
                {
                    BabyGenderScenario.FirstBornGender genderPref = GetValue<GenderOption, BabyGenderScenario.FirstBornGender>();

                    bool bothGenders = true;
                    if ((genderPref == BabyGenderScenario.FirstBornGender.Male) ||
                        (genderPref == BabyGenderScenario.FirstBornGender.Female))
                    {
                        bothGenders = false;
                    }
                    else if (RandomUtil.RandomChance(GetValue<ManagerFlirt.ChanceOfGaySim, int>()))
                    {
                        bothGenders = false;
                    }

                    if (ageGenderRequirements.Count == 1)
                    {
                        if (bothGenders)
                        {
                            if ((ageGenderRequirements[0] & CASAgeGenderFlags.Male) == CASAgeGenderFlags.Male)
                            {
                                ageGenderRequirements.Add(CASAgeGenderFlags.Female | (ageGenderRequirements[0] & CASAgeGenderFlags.AgeMask));
                            }
                            else
                            {
                                ageGenderRequirements.Add(CASAgeGenderFlags.Male | (ageGenderRequirements[0] & CASAgeGenderFlags.AgeMask));
                            }
                        }
                        else
                        {
                            genders = ageGenderRequirements[0] & CASAgeGenderFlags.GenderMask;

                            ageGenderRequirements.Add(ageGenderRequirements[0]);
                        }
                    }
                }

                foreach (CASAgeGenderFlags ageGender in ageGenderRequirements)
                {
                    SimDescription sim = simBin.CreateNewSim(ageGender & CASAgeGenderFlags.AgeMask, ageGender & CASAgeGenderFlags.GenderMask, CASAgeGenderFlags.Human);
                    if (sim == null)
                    {
                        IncStat("Creation Failure");
                    }
                    else
                    {
                        sims.Add(sim);
                    }
                }

                if (familyLot == null)
                {
                    if (sims.Count > 0)
                    {
                        Add(frame, new ImmigrantMailOrderScenario(mRequirement, sims), ScenarioResult.Start);
                    }
                }
                else
                {
                    bool createChildren = true;

                    SimDescription otherParent = null;

                    List<SimDescription> parents = new List<SimDescription>(sims);
                    if (parents.Count > 1)
                    {
                        otherParent = parents[1];

                        if (parents[1].IsMale)
                        {
                            parents[0].InternalIncreaseGenderPreferenceMale();
                        }
                        else
                        {
                            parents[0].InternalIncreaseGenderPreferenceFemale();
                        }

                        if (parents[0].IsMale)
                        {
                            parents[1].InternalIncreaseGenderPreferenceMale();
                        }
                        else
                        {
                            parents[1].InternalIncreaseGenderPreferenceFemale();
                        }

                        Relationship relation = Relationship.Get(parents[0], parents[1], true);
                        if (relation != null)
                        {
                            relation.MakeAcquaintances();
                        }

                        if (GameUtils.IsUniversityWorld())
                        {
                            createChildren = true;
                            IncStat("Immigrant Friends");
                        }
                        else if (!RandomUtil.RandomChance(GetValue<ChanceOfFriendsOption, int>()))
                        {
                            Dictionary<string, List<News.NewsTuning.ArticleTuning>> namedArticles = News.sNewsTuning.mNamedArticles;

                            try
                            {
                                // Doing so stops Marriage notices of imported sims from appearing in the newspaper
                                News.sNewsTuning.mNamedArticles = new Dictionary<string, List<News.NewsTuning.ArticleTuning>>();

                                if (RandomUtil.RandomChance(GetValue<ChanceOfPartnerOption, int>()))
                                {
                                    IncStat("Immigrant Partners");

                                    while (relation.CurrentLTR != LongTermRelationshipTypes.Partner)
                                    {
                                        if (!Romances.BumpToHigherState(this, parents[0], parents[1])) break;
                                    }
                                }
                                else
                                {
                                    if (!Romances.BumpToHighestState(this, parents[0], parents[1]))
                                    {
                                        IncStat("Unmarriable");
                                        createChildren = false;
                                    }
                                    else
                                    {
                                        parents[1].LastName = parents[0].LastName;
                                    }
                                }
                            }
                            finally
                            {
                                News.sNewsTuning.mNamedArticles = namedArticles;
                            }
                        }
                        else
                        {
                            createChildren = false;
                            IncStat("Immigrant Friends");
                        }
                    }

                    if (createChildren)
                    {
                        Lot.LotMetrics metrics = new Lot.LotMetrics();
                        familyLot.GetLotMetrics(ref metrics);

                        int totalChildren = GetValue<WholeFamilyOption, int>();
                        if (totalChildren > Options.GetValue<MaximumSizeOption, int>())
                        {
                            totalChildren = Options.GetValue<MaximumSizeOption, int>();
                        }

                        totalChildren -= parents.Count;

                        totalChildren = RandomUtil.GetInt(totalChildren - 1) + 1;

                        AddStat("Allowed Children", totalChildren);

                        int totalBeds = metrics.BedCount - parents.Count;
                        if (totalBeds > totalChildren)
                        {
                            totalBeds = totalChildren;
                        }

                        AddStat("Available Beds", totalBeds);

                        if (!GameUtils.IsUniversityWorld())
                        {
                            int numCribs = RandomUtil.GetInt((int)familyLot.CountObjects<Sims3.Gameplay.Objects.Beds.Crib>());

                            // Create the children
                            if (numCribs > 2)
                            {
                                numCribs = 2;
                            }

                            if (numCribs > totalChildren)
                            {
                                numCribs = totalChildren;
                            }

                            AddStat("Available Cribs", numCribs);

                            for (int i = 0; i < numCribs; i++)
                            {
                                SimDescription sim = simBin.CreateNewSim(parents[0], otherParent, CASAgeGenderFlags.Toddler, genders, parents[0].Species, true);
                                if (sim == null)
                                {
                                    IncStat("Creation Failure");
                                }
                                else
                                {
                                    sims.Add(sim);

                                    totalBeds--;
                                }
                            }

                            if ((!parents[0].Teen) && ((otherParent == null) || (!otherParent.Teen)))
                            {
                                for (int i = 0; i < totalBeds; i++)
                                {
                                    SimDescription sim = simBin.CreateNewSim(parents[0], otherParent, CASAgeGenderFlags.Child | CASAgeGenderFlags.Teen, genders, parents[0].Species, true);
                                    if (sim == null)
                                    {
                                        IncStat("Creation Failure");
                                    }
                                    else
                                    {
                                        sims.Add(sim);
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < totalBeds; i++)
                            {
                                SimDescription sim = simBin.CreateNewSim(CASAgeGenderFlags.YoungAdult | CASAgeGenderFlags.Adult, genders, parents[0].Species);
                                if (sim == null)
                                {
                                    IncStat("Creation Failure");
                                }
                                else
                                {
                                    sims.Add(sim);
                                }
                            }
                        }
                        
                        if ((singleParent) && (otherParent != null))
                        {
                            sims.Remove(otherParent);

                            parents.Remove(otherParent);

                            if (otherParent.Partner != null)
                            {
                                otherParent.Partner.Partner = null;
                                otherParent.Partner = null;
                            }

                            otherParent.Dispose(true, false, true);
                        }
                    }
                }

                if ((GameUtils.IsInstalled(ProductVersion.EP5)) &&
                    (RandomUtil.RandomChance(GetValue<PetChanceOption, int>())))
                {
                    List<CASAgeGenderFlags> choices = new List<CASAgeGenderFlags>();

                    foreach (SimDescription sim in sims)
                    {
                        PetAdoptionBaseScenario.GetPossibleSpecies(this, familyLot, sim, false, choices);
                    }

                    AddStat("Pet Choices", choices.Count);

                    if (choices.Count > 0)
                    {
                        int numberPets = RandomUtil.GetInt(1, GetValue<MaximumPetsOption, int>() - 1);

                        AddStat("Pet Immigrants", numberPets);

                        for (int i = 0; i < numberPets; i++)
                        {
                            CASAgeGenderFlags species = RandomUtil.GetRandomObjectFromList(choices);
                            if (Sims.HasEnough(this, species)) continue;

                            SimDescription pet = simBin.CreateNewSim(CASAgeGenderFlags.Child | CASAgeGenderFlags.Adult | CASAgeGenderFlags.Elder, CASAgeGenderFlags.Male | CASAgeGenderFlags.Female, species);
                            if (pet != null)
                            {
                                pet.LastName = sims[0].LastName;

                                sims.Add(pet);
                            }
                            else
                            {
                                IncStat("Pet Creation Fail");
                            }
                        }
                    }
                }

                Add(frame, new ImmigrantMoveInScenario(sims), ScenarioResult.Start);
                Add(frame, new NewImmigrantScenario(sims, mRequirement), ScenarioResult.Success);
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new ImmigrateScenario(this);
        }

        public class MinimumEmptyHomesOption : IntegerManagerOptionItem<ManagerLot>, ManagerLot.IImmigrationEmigrationOption, IAdjustForVacationOption
        {
            public MinimumEmptyHomesOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "MinimumEmptyHomes";
            }

            public bool AdjustForVacationTown()
            {
                if (GameUtils.IsUniversityWorld())
                {
                    SetValue(5);
                    return true;
                }

                return false;
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class WholeFamilyOption : IntegerManagerOptionItem<ManagerLot>, ManagerLot.IImmigrationEmigrationOption
        {
            public WholeFamilyOption()
                : base(8)
            { }

            public override string GetTitlePrefix()
            {
                return "ImmigrateWholeFamily";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class PetChanceOption : IntegerManagerOptionItem<ManagerLot>, ManagerLot.IImmigrationEmigrationOption, IAdjustForVacationOption
        {
            public PetChanceOption()
                : base(25)
            { }

            public override string GetTitlePrefix()
            {
                return "ImmigrantPetChance";
            }

            public bool AdjustForVacationTown()
            {
                if (GameUtils.IsUniversityWorld())
                {
                    SetValue(0);
                }

                return true;
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP5);
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class MaximumPetsOption : IntegerManagerOptionItem<ManagerLot>, ManagerLot.IImmigrationEmigrationOption
        {
            public MaximumPetsOption()
                : base(2)
            { }

            public override string GetTitlePrefix()
            {
                return "MaximumPetImmigrants";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class ChanceOfFriendsOption : IntegerManagerOptionItem<ManagerLot>, ManagerLot.IImmigrationEmigrationOption
        {
            public ChanceOfFriendsOption()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "ChanceOfFriendsImmigrant";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class ChanceOfPartnerOption : IntegerManagerOptionItem<ManagerLot>, ManagerLot.IImmigrationEmigrationOption
        {
            public ChanceOfPartnerOption()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "ChanceOfPartnerImmigrant";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class SingleSimOption : IntegerManagerOptionItem<ManagerLot>, ManagerLot.IImmigrationEmigrationOption
        {
            public SingleSimOption()
                : base(25)
            { }

            public override string GetTitlePrefix()
            {
                return "ImmigrateSingleSim";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                if (Manager.GetValue<WholeFamilyOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class TeenChanceOption : IntegerManagerOptionItem<ManagerLot>, ManagerLot.IImmigrationEmigrationOption
        {
            public TeenChanceOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "ChanceofTeenImmigrant";
            }

            public override int Value
            {
                get
                {
                    if (!ShouldDisplay()) return 0;

                    return base.Value;
                }
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class ChanceOfSingleOption : IntegerManagerOptionItem<ManagerLot>, ManagerLot.IImmigrationEmigrationOption
        {
            public ChanceOfSingleOption()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "ChanceOfSingleParentImmigrant";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class GenderOption : BabyGenderScenario.FirstBornGenderOptionBase<ManagerLot>, ManagerLot.IImmigrationEmigrationOption
        {
            public GenderOption()
            { }

            public override string GetTitlePrefix()
            {
                return "ImmigrantGender";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<ScheduledImmigrationScenario.GaugeOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
