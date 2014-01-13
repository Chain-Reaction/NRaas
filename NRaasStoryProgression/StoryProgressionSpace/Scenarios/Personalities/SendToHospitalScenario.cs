using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
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
    public class SendToHospitalScenario : DualSimScenario
    {
        WeightOption.NameOption mName = null;

        SimDescription.DeathType mType = SimDescription.DeathType.None;

        string mInjuredStory = null;

        WeightScenarioHelper mSuccess = null;

        public SendToHospitalScenario()
        { }
        protected SendToHospitalScenario(SendToHospitalScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mType = scenario.mType;
            mInjuredStory = scenario.mInjuredStory;
            mSuccess = scenario.mSuccess;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Type=" + mType;
            text += Common.NewLine + "InjuredStory=" + mInjuredStory;
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
                return null;
            }
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mInjuredStory = row.GetString("InjuredStory");

            if (!ParserFunctions.TryParseEnum<SimDescription.DeathType>(row.GetString("Type"), out mType, SimDescription.DeathType.None))
            {
                error = "Type not valid";
                return false;
            }

            if (mType == SimDescription.DeathType.None)
            {
                error = "Type cannot be None";
                return false;
            }

            mSuccess = new WeightScenarioHelper(Origin.FromWatchingSimSuffer);
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

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(UnlocalizedName) <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Not Resident");
                return false;
            }
            else if (sim.OccultManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (!sim.OccultManager.HasOccultType(OccultTypes.Mummy))
            {
                IncStat("Not Mummy");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (!Households.AllowGuardian(sim))
            {
                IncStat("Age Denied");
                return false;
            }
            else if (!Deaths.Allow(this, sim))
            {
                IncStat("Deaths Denied");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situations Denied");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mSuccess.TestBeforehand(Manager, Sim, Target))
            {
                IncStat("Success TestBeforehand Fail");
                return false;
            }

            Add(frame, new GoToHospitalScenario(Target, Sim, mInjuredStory, mType), ScenarioResult.Start);

            mSuccess.Perform(this, frame, "Success", Sim, Target);
            return false;
        }

        public override Scenario Clone()
        {
            return new SendToHospitalScenario(this);
        }
    }
}
