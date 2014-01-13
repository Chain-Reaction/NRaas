using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Personalities
{
    public class HoosegowScenario : GoToJailBaseScenario, IHasPersonality
    {
        WeightOption.NameOption mName = null;

        IntegerOption.OptionValue mChance = null;

        IntegerOption.OptionValue mBail = null;

        WeightScenarioHelper mSuccess = null;

        public HoosegowScenario()
        { }
        protected HoosegowScenario(HoosegowScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mChance = scenario.mChance;
            mBail = scenario.mBail;
            mSuccess = scenario.mSuccess;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Chance=" + mChance;
            text += Common.NewLine + "Bail=" + mBail;
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

        protected override int Bail
        {
            get 
            {
                if (mBail.Value > 0)
                {
                    return mBail.Value;
                }
                else
                {
                    return base.Bail;
                }
            }
        }

        protected override int Chance
        {
            get { return mChance.Value; }
        }

        protected override string StoryName
        {
            get { return mName.ToString(); }
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

                if (mBail != null)
                {
                    mBail.UpdateManager(value);
                }

                if (mChance != null)
                {
                    mChance.UpdateManager(value);
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

            mBail = new IntegerOption.OptionValue(-1);
            if (row.Exists("Bail"))
            {
                if (!mBail.Parse(row, "Bail", Manager, this, ref error))
                {
                    return false;
                }
            }

            mChance = new IntegerOption.OptionValue();
            if (!mChance.Parse(row, "Chance", Manager, this, ref error))
            {
                return false;
            }

            mSuccess = new WeightScenarioHelper(Origin.FromBurglar);
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

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Personalities.Allow(this, sim))
            {
                IncStat("Personality Denied");
                return false;
            }

            return (base.Allow(sim));
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

        public override Scenario Clone()
        {
            return new HoosegowScenario(this);
        }
    }
}
