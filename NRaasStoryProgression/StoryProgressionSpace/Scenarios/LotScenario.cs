using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public abstract class LotScenario : Scenario 
    {
        Lot mLot = null;

        protected LotScenario()
        { }
        protected LotScenario(Lot lot)
        {
            mLot = lot;
        }
        protected LotScenario(LotScenario scenario)
            : base (scenario)
        {
            mLot = scenario.Lot;
        }

        protected virtual List<Lot> GetLots()
        {
            List<Lot> lots = new List<Lot>();
            foreach (Lot lot in LotManager.AllLots)
            {
                lots.Add(lot);
            }
            return lots;
        }

        public override string ToString()
        {
            string text = base.ToString();

            if (Lot != null)
            {
                text += Common.NewLine + "L: " + Lot.Name;
            }

            return text;
        }

        public override string GetIDName()
        {
            string text = base.GetIDName();

            if (Lot != null)
            {
                text += Common.NewLine + "L: " + Lot.Name;
            }

            return text;
        }

        protected virtual bool AllowActive
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!base.Allow()) return false;

            if (Lot != null)
            {
                return Allow(Lot);
            }
            else
            {
                return true;
            }
        }

        public Lot Lot
        {
            get { return mLot; }
            set { mLot = value; }
        }

        public override IAlarmOwner Owner
        {
            get { return Lot; }
        }

        protected virtual bool Allow(Lot lot)
        {
            if (!AllowActive)
            {
                if (lot.Household == Household.ActiveHousehold) return false;
            }

            return true;
        }

        protected override bool Matches(Scenario scenario)
        {
            if (!base.Matches(scenario)) return false;

            LotScenario lotScenario = scenario as LotScenario;
            if (lotScenario == null) return false;

            return (Lot == lotScenario.Lot);
        }

        protected override bool UsesSim(ulong sim)
        {
            if (Lot == null) return false;

            Household house = Lot.Household;
            if (house == null) return false;

            foreach (SimDescription resident in HouseholdsEx.All(house))
            {
                if (resident.SimDescriptionId == sim) return true;
            }

            return false;
        }

        protected virtual bool Sort(List<Lot> houses)
        {
            return false;
        }

        protected override GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            if (Lot != null)
            {
                if (Allow(Lot))
                {
                    return GatherResult.Update;
                }
            }
            else
            {
                List<Lot> lots = GetLots();
                if (lots != null)
                {
                    List<Lot> allowed = new List<Lot>();

                    foreach (Lot lot in lots)
                    {
                        Lot = lot;
                        if (Allow(lot))
                        {
                            allowed.Add(lot);
                        }
                    }

                    Lot = null;
                    random = !Sort(allowed);

                    foreach (Lot lot in allowed)
                    {
                        Lot = lot;

                        Scenario scenario = Clone();
                        if (scenario != null)
                        {
                            list.Add(scenario);
                        }
                    }

                    Lot = null;

                    if (allowed.Count > 0)
                    {
                        return GatherResult.Success;
                    }
                }
            }

            return GatherResult.Failure;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                if (Lot == null) return null;

                if (Lot.Name == null) return null;

                parameters = new object[] { Lot };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        protected override ManagerStory.Story PrintDebuggingStory(object[] parameters)
        {
            if (parameters == null)
            {
                if (Lot != null)
                {
                    parameters = new object[] { Lot };
                }
            }

            return base.PrintDebuggingStory(parameters);
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                if (Lot == null) return null;

                if (Lot.Name == null) return null;

                parameters = new object[] { Lot };
            }

            return base.PrintFormattedStory(manager, text, summaryKey, parameters, extended, logging);
        }
    }
}
