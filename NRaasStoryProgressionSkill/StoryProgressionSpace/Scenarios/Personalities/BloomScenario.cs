using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Personalities
{
    public class BloomScenario : Money.HarvestScenario, IHasPersonality
    {
        public enum Locales : uint
        {
            Home = 0x1,
            Community = 0x2,
            Neighbors = 0x4,
        }

        WeightOption.NameOption mName = null;

        WeightScenarioHelper mSuccess = null;

        Locales mLocale = Locales.Home;

        Lot mLot = null;

        public BloomScenario()
        {}
        protected BloomScenario(BloomScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mSuccess = scenario.mSuccess;
            mLocale = scenario.mLocale;
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

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Locale=" + mLocale;
            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;

            return text;
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

        public override StoryProgressionObject Manager
        {
            set
            {
                base.Manager = value;

                if (mSuccess != null)
                {
                    mSuccess.UpdateManager(value);
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

            mSuccess = new WeightScenarioHelper(Origin.FromBurglar);
            if (!mSuccess.Parse(row, Manager, this, "Success", ref error))
            {
                return false;
            }

            if (!row.Exists("Locale"))
            {
                error = "Locale Missing";
                return false;
            }

            if (!row.TryGetEnum<Locales>("Locale", out mLocale, Locales.Home))
            {
                error = "Unknown Locale";
                return false;
            }

            return base.Parse(row, ref error);
        }

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Personalities.Allow(this, Sim))
            {
                IncStat("Personality Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return GetFilteredSims(Personalities.GetClanLeader(Manager));
        }

        protected override List<HarvestPlant> GatherPlants()
        {
            List<Lot> choices = new List<Lot>();

            foreach (Lot lot in LotManager.AllLots)
            {
                if (lot == Sim.LotHome)
                {
                    if ((mLocale & Locales.Home) != Locales.Home) continue;
                }
                else if (lot.IsCommunityLot)
                {
                    if ((mLocale & Locales.Community) != Locales.Community) continue;
                }
                else
                {
                    if ((mLocale & Locales.Neighbors) != Locales.Neighbors) continue;
                }

                if (lot.CountObjects<HarvestPlant> () == 0) continue;

                choices.Add(lot);
            }

            if (choices.Count == 0)
            {
                IncStat("No Lot Choices");
                return null;
            }

            mLot = RandomUtil.GetRandomObjectFromList(choices);

            return new List<HarvestPlant>(mLot.GetObjects<HarvestPlant>());
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mSuccess.TestBeforehand(Manager, Sim, Sim))
            {
                IncStat("Success TestBeforehand Fail");
                return false;
            }

            if (!base.PrivateUpdate(frame)) return false;

            mSuccess.Perform(this, frame, "Success", Sim, Sim);
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                SimDescription target = SimTypes.HeadOfFamily(mLot.Household);
                if (target == null) return null;

                parameters = new object[] { Sim, target };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new BloomScenario(this);
        }
    }
}
