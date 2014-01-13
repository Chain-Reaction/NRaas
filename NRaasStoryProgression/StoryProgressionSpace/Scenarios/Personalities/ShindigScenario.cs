using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
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
    public class ShindigScenario : GatheringScenario, IHasPersonality, SimPersonality.IMustBeFirstChoiceOption
    {
        WeightOption.NameOption mName = null;

        WeightScenarioHelper mSuccess = null;

        int mChanceOfHomeLot = 50;

        OutfitCategories mPartyAttire = OutfitCategories.Everyday;

        public ShindigScenario()
        { }
        protected ShindigScenario(ShindigScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mChanceOfHomeLot = scenario.mChanceOfHomeLot;
            mPartyAttire = scenario.mPartyAttire;
            mSuccess = scenario.mSuccess;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "ChanceOfHomeLot=" + mChanceOfHomeLot;
            text += Common.NewLine + "PartyAttire=" + mPartyAttire;
            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;

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

        protected override bool CheckBusy
        {
            get { return false; } //  base.ShouldPush; }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        protected override OutfitCategories PartyAttire
        {
            get { return mPartyAttire; }
        }

        protected override int ChanceOfHomeLot
        {
            get { return mChanceOfHomeLot; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mChanceOfHomeLot = row.GetInt("ChanceOfHome", mChanceOfHomeLot);

            mPartyAttire = row.GetEnum<OutfitCategories>("Attire", OutfitCategories.None);
            if (mPartyAttire == OutfitCategories.None)
            {
                error = "Unknown Attire: " + row.GetString("Attire");
                return false;
            }

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

        protected override bool TargetAllow(SimDescription sim)
        {
            if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
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
            if (!mSuccess.TestBeforehand(Manager, Sim, Sim))
            {
                IncStat("Success TestBeforehand Fail");
                return false;
            }

            if (!base.PrivateUpdate(frame)) return false;

            mSuccess.Perform(this, frame, "Success", Sim, Sim);
            return true;
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new ShindigScenario(this);
        }
    }
}
