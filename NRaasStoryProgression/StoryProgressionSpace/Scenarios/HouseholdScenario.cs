using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
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
    public abstract class HouseholdScenario : Scenario 
    {
        Household mHouse = null;

        protected HouseholdScenario()
        { }
        protected HouseholdScenario(Household house)
        {
            mHouse = house;
        }
        protected HouseholdScenario(HouseholdScenario scenario)
            : base (scenario)
        {
            mHouse = scenario.House;
        }

        public override string ToString()
        {
            string text = base.ToString();

            if (House != null)
            {
                text += Common.NewLine + "H: " + House.Name;
            }

            return text;
        }

        protected virtual List<Household> GetHouses()
        {
            List<Household> list = new List<Household>();
            foreach (Household house in Household.sHouseholdList)
            {
                if (SimTypes.IsSpecial(house)) continue;

                list.Add(house);
            }

            return list;
        }

        protected virtual bool AllowActive
        {
            get { return false; }
        }

        public Household House
        {
            get { return mHouse; }
            set { mHouse = value; }
        }

        public override IAlarmOwner Owner
        {
            get { return House; }
        }

        protected override bool Allow()
        {
            if (!base.Allow()) return false;

            if (House != null)
            {
                return Allow(House);
            }
            else
            {
                return true;
            }
        }

        public override string GetIDName()
        {
            string result = base.GetIDName();

            if (House != null)
            {
                result += Common.NewLine + "H: " + House.Name;

                SimDescription head = SimTypes.HeadOfFamily(House);
                if (head != null)
                {
                    result += " (" + head.FullName + ")";
                }
            }

            return result;
        }

        protected virtual bool Allow(Household house)
        {
            if (HouseholdsEx.NumSims(house) == 0)
            {
                IncStat("Empty");
                return false;
            }

            if (!AllowActive)
            {
                if (house == Household.ActiveHousehold)
                {
                    IncStat("Active");
                    return false;
                }
            }
            return true;
        }

        protected override bool Matches(Scenario scenario)
        {
            if (!base.Matches(scenario)) return false;

            HouseholdScenario houseScenario = scenario as HouseholdScenario;
            if (houseScenario == null) return false;

            return (House == houseScenario.House);
        }

        protected override bool UsesSim(ulong sim)
        {
            if (House == null) return false;

            foreach (SimDescription resident in HouseholdsEx.All(House))
            {
                if (resident.SimDescriptionId == sim) return true;
            }

            return false;
        }

        protected virtual bool Sort(List<Household> houses)
        {
            return false;
        }

        protected override GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            if (House != null)
            {
                if (Allow(House))
                {
                    return GatherResult.Update;
                }
            }
            else
            {
                List<Household> houses = GetHouses();
                if (houses != null)
                {
                    List<Household> allowed = new List<Household>();

                    foreach (Household house in houses)
                    {
                        House = house;
                        if (Allow(house))
                        {
                            allowed.Add(house);
                        }
                    }

                    House = null;
                    random = !Sort(allowed);

                    foreach (Household house in allowed)
                    {
                        House = house;

                        Scenario scenario = Clone();
                        if (scenario != null)
                        {
                            list.Add(scenario);
                        }
                    }

                    House = null;

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
                if (House == null) return null;

                if (name == null) return null;

                if (manager == null)
                {
                    manager = Manager;
                }

                return Stories.PrintStory(manager, name, House, extended, logging);
            }
            else
            {
                return base.PrintStory(manager, name, parameters, extended, logging);
            }
        }

        protected override ManagerStory.Story PrintDebuggingStory(object[] parameters)
        {
            if (parameters == null)
            {
                if (House != null)
                {
                    parameters = new object[] { House };
                }
            }

            return base.PrintDebuggingStory(parameters);
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                if (text == null) return null;

                if (House == null) return null;

                parameters = new object[] { House };
            }

            return base.PrintFormattedStory(manager, text, summaryKey, parameters, extended, logging);
        }
    }
}
