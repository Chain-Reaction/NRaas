using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class ExtinguishScenario : LotScenario
    {
        public ExtinguishScenario(Lot lot)
            : base(lot)
        { }
        protected ExtinguishScenario(ExtinguishScenario scenario)
            : base(scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Extinguish";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow(Lot lot)
        {
            if (FindSituationInvolving(lot))
            {
                IncStat("Already handled");
                return false;
            }

            return base.Allow(lot);
        }

        public static bool FindSituationInvolving(Lot lot)
        {
            foreach (Situation situation in Situation.sAllSituations)
            {
                Sims3.Gameplay.Services.FirefighterSituation sit2 = situation as Sims3.Gameplay.Services.FirefighterSituation;
                if (sit2 != null)
                {
                    if (sit2.Lot == lot) return true;
                }
                else
                {
                    NRaas.StoryProgressionSpace.Situations.FirefighterSituation sit3 = situation as NRaas.StoryProgressionSpace.Situations.FirefighterSituation;
                    if (sit3 != null)
                    {
                        if (sit3.Lot == lot) return true;
                    }
                }
            }
            return false;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Firefighter instance = Firefighter.Instance;
            if (instance == null)
            {
                IncStat("No Service");
                return false;
            }

            List<SimDescription> sims = new List<SimDescription>();
            foreach (SimDescription sim in Sims.All)
            {
                if (SimTypes.IsSelectable(sim))
                {
                    IncStat("Active");
                }
                else if (sim.CreatedSim == null)
                {
                    IncStat("Hibernating");
                }
                else if (sim.LotHome == null)
                {
                    IncStat("Not Resident");
                }
                else if (!(sim.Occupation is ActiveFireFighter))
                {
                    IncStat("Not Firefighter");
                }
                else if (!Situations.Allow(this, sim, Managers.Manager.AllowCheck.None))
                {
                    IncStat("Situation Denied");
                }
                else
                {
                    sims.Add(sim);
                }
            }

            if (sims.Count > 0)
            {
                AddStat("Choices", sims.Count);

                SimDescription choice = RandomUtil.GetRandomObjectFromList(sims);

                if (Situations.GreetSimOnLot(choice, Lot))
                {
                    IncStat("Active Firefighter");

                    new NRaas.StoryProgressionSpace.Situations.FirefighterSituation(Lot, choice.CreatedSim);
                    return true;
                }
            }

            IncStat("Service Firefighter");

            instance.MakeServiceRequest(Lot, true, ObjectGuid.InvalidObjectGuid);
            return true;
        }

        public override Scenario Clone()
        {
            return new ExtinguishScenario(this);
        }
    }
}
