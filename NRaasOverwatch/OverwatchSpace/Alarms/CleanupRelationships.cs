using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public class CleanupRelationships : AlarmOption
    {
        public CleanupRelationships()
            : base ()
        { }

        public override string GetTitlePrefix()
        {
            return "CleanupRelationships";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mCleanupRelationships;
            }
            set
            {
                NRaas.Overwatch.Settings.mCleanupRelationships = value;
            }
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            Overwatch.Log(Name);

            List<Relationship> fullList = new List<Relationship>();

            int count = 0;

            foreach (SimDescription sim in new List<SimDescription>(Relationship.sAllRelationships.Keys))
            {
                count += Corrections.CleanupRelationship(sim, Overwatch.Log);
            }

            foreach(Relationship relation in fullList)
            {
                if (Relationships.RepairRelationship(relation, Overwatch.Log))
                {
                    count++;
                }
            }

            foreach (SimDescription sim in Household.EverySimDescription())
            {
                if (sim.Partner == null) continue;

                if (!sim.Partner.IsValidDescription)
                {
                    sim.mPartner = null;

                    Overwatch.Log("Partner Relationship Dropped: " + sim.FullName);

                    count++;
                }
                else
                {
                    Relationship relation = Relationship.Get(sim, sim.Partner, false);
                    if (relation == null)
                    {
                        relation = Relationship.Get(sim, sim.Partner, true);

                        relation.MakeAcquaintances();

                        Overwatch.Log("Partner Relationship Corrected: " + sim.FullName);

                        count++;
                    }
                }
            }

            if ((prompt) || (count > 0))
            {
                Overwatch.AlarmNotify(Common.Localize(GetTitlePrefix() + ":Success", false, new object[] { count }));
            }
        }
    }
}
