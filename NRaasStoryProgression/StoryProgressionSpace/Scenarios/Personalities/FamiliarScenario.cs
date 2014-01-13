using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
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
    public class FamiliarScenario : PetAdoptionBaseScenario, IHasPersonality
    {
        WeightOption.NameOption mName = null;

        WeightScenarioHelper mSuccess = null;

        int mDelta = 0;

        bool mCheckAny = false;

        bool mCheckExisting = true;

        List<CASAgeGenderFlags> mSpecies;

        CASAgeGenderFlags mAges = CASAgeGenderFlags.Child;

        public FamiliarScenario()
        { }
        protected FamiliarScenario(FamiliarScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mCheckAny = scenario.mCheckAny;
            mCheckExisting = scenario.mCheckExisting;
            mSpecies = scenario.mSpecies;
            mDelta = scenario.mDelta;
            mAges = scenario.mAges;
            mSuccess = scenario.mSuccess;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Delta=" + mDelta;
            text += Common.NewLine + "CheckAny=" + mCheckAny;
            text += Common.NewLine + "CheckExisting=" + mCheckExisting;
            text += Common.NewLine + "Ages=" + mAges;

            text += Common.NewLine + "Species=";

            foreach (CASAgeGenderFlags species in mSpecies)
            {
                text += species + ",";
            }

            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;

            return text;
        }

        protected override bool Allow()
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP5))
            {
                return false;
            }

            if (!GetValue<PetAdoptionScenario.Option, bool>()) return false;

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

        protected override bool CheckBusy
        {
            get { return false; } //  base.ShouldPush; }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        protected override int Delta
        {
            get { return mDelta; }
        }

        protected override CASAgeGenderFlags Ages
        {
            get { return mAges; }
        }

        protected override void GetPossibleSpecies(Household house, List<CASAgeGenderFlags> species)
        {
            if ((mSpecies == null) || (mSpecies.Count == 0))
            {
                base.GetPossibleSpecies(house, species);
            }
            else
            {
                species.AddRange(mSpecies);
            }

            if (mCheckExisting)
            {
                List<CASAgeGenderFlags> existing = new List<CASAgeGenderFlags>();
                foreach(SimDescription sim in HouseholdsEx.Pets(house))
                {
                    existing.Add(sim.Species);
                }

                for (int i = species.Count - 1; i >= 0; i--)
                {
                    if (existing.Contains(species[i]))
                    {
                        species.RemoveAt(i);
                    }
                }
            }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mCheckAny = row.GetBool("CheckAny");

            mDelta = row.GetInt("Delta", mDelta);

            if (row.Exists("Ages"))
            {
                mAges = row.GetEnum<CASAgeGenderFlags>("Ages", CASAgeGenderFlags.None);
                if (mAges == CASAgeGenderFlags.None)
                {
                    error = "Invalid Ages: " + row.GetString("Ages");
                    return false;
                }
            }

            if (row.Exists("CheckExisting"))
            {
                mCheckExisting = row.GetBool("CheckExisting");
            }

            if (row.Exists("Species"))
            {
                ToCASAgeGenderFlags converter = new ToCASAgeGenderFlags();

                mSpecies = converter.Convert(row.GetString("Species"));
                if (mSpecies == null)
                {
                    error = converter.mError;
                    return false;
                }
            }

            mSuccess = new WeightScenarioHelper(Origin.FromNewLitter);
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

        protected override bool Allow(SimDescription sim)
        {
            if (!Personalities.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if ((mCheckAny) && (HouseholdsEx.NumPets(sim.Household) > 0))
            {
                IncStat("Has Pets");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mSuccess.TestBeforehand(Manager, Sim, Sim))
            {
                IncStat("Success TestBeforehand Fail");
                return false;
            }

            if (!base.PrivateUpdate(frame)) return false;

            mSuccess.Perform(this, frame, "Success", Sim, null);
            return true;
        }

        public override Scenario Clone()
        {
            return new FamiliarScenario(this);
        }

        public class ToCASAgeGenderFlags : StringToList<CASAgeGenderFlags>
        {
            public string mError;

            protected override bool PrivateConvert(string value, out CASAgeGenderFlags result)
            {
                if (ParserFunctions.TryParseEnum<CASAgeGenderFlags>(value, out result, CASAgeGenderFlags.None)) return true;

                mError = "Unknown Species: " + value;
                return false;
            }
        }
    }
}
