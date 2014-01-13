using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Friends
{
    public abstract class MeetTheWorkerScenario : RelationshipScenario
    {
        public MeetTheWorkerScenario()
            : base(10)
        { }
        protected MeetTheWorkerScenario(MeetTheWorkerScenario scenario)
            : base (scenario)
        { }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public override bool IsFriendly
        {
            get { return true; }
        }

        protected override bool TestRelationship
        {
            get { return true; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected abstract Career GetCareer(SimDescription sim);

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            Dictionary<SimDescription,bool> coworkers = new Dictionary<SimDescription,bool>();

            Career career = GetCareer(sim);                
            if (career == null) 
            {
                IncStat("No Career");
                return null;
            }

            if (career.CareerLoc == null)
            {
                IncStat("No Loc");
                return null;
            }

            RabbitHole hole = career.CareerLoc.Owner;
            if (hole == null) 
            {
                IncStat("No Hole");
                return null;
            }

            if (hole.CareerLocations == null)
            {
                IncStat("No CareerLocations");
                return null;
            }

            foreach (CareerLocation location in hole.CareerLocations.Values)
            {
                if (location == career.CareerLoc) continue;

                Career other = location.Career;
                if (other == null) continue;

                if (other.IsPartTime) continue;

                if (other is School) continue;

                if (other.Coworkers == null) continue;

                foreach (SimDescription coworker in other.Coworkers)
                {
                    if (coworkers.ContainsKey(coworker)) continue;

                    if (Relationship.Get(sim, coworker, false) != null) continue;

                    coworkers.Add(sim, true);
                }
            }

            return coworkers.Keys;
        }

        protected override bool Allow()
        {
            if (!GetValue<ScheduledFriendScenario.AllowFriendOption, bool>()) return false;

            return base.Allow();
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Situations.Allow(this, sim))
            {
                IncStat("Situations Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (GetCareer (sim) == null)
            {
                IncStat("No Career");
                return false;
            }
            else if (AddScoring("MeetTheWorker", sim) < 0)
            {
                IncStat("Scoring Fail");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool Push()
        {
            return Situations.PushVisit(this, Sim, Target.LotHome);
        }
    }
}
