﻿using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public abstract class VisitorScenario : DualSimScenario
    {
        public VisitorScenario()
        { }
        protected VisitorScenario(VisitorScenario scenario)
            : base (scenario)
        { }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool TargetAllowActive
        {
            get { return false; }
        }

        protected override int ContinueChance
        {
            get { return 25; }
        }

        protected override int ContinueReportChance
        {
            get { return 50; }
        }

        protected override int Rescheduling
        {
            get { return 120; }
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.CreatedSim.LotCurrent != sim.LotHome)
            {
                IncStat("Not Home");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (Sim.Household == Target.Household)
            {
                IncStat("Same Home");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return true;
        }

        protected override bool Push()
        {
            return Situations.PushVisit(this, Target, Sim.LotHome);
        }
    }
}
