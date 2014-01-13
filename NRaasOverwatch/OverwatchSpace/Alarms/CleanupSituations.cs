using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public class CleanupSituations : AlarmOption
    {
        public override string GetTitlePrefix()
        {
            return "CleanupSituations";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mCleanupSituations;
            }
            set
            {
                NRaas.Overwatch.Settings.mCleanupSituations = value;
            }
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            Overwatch.Log(Name);

            int count = 0;

            for (int i=Situation.sAllSituations.Count-1; i>=0; i--)
            {
                Situation situation = Situation.sAllSituations[i];

                try
                {
                    bool tested = false, remove = false;

                    if (situation is FieldTripSituation)
                    {
                        tested = true;
                        remove = true;
                    }

                    if ((tested) && (prompt))
                    {
                        remove = true;
                    }

                    if (remove)
                    {
                        try
                        {
                            situation.Exit();
                        }
                        catch(Exception e)
                        {
                            Common.Exception("Situation: " + situation.GetType().ToString(), e);
                        }

                        foreach (Sim sim in LotManager.Actors)
                        {
                            sim.RemoveRole(situation);
                        }

                        if ((Situation.sAllSituations.Count > i) && (Situation.sAllSituations[i] == situation))
                        {
                            Situation.sAllSituations.RemoveAt(i);
                        }

                        count++;

                        Overwatch.Log("  Removed: " + situation.GetType().ToString());
                    }
                }
                catch (Exception e)
                {
                    Common.Exception("Situation: " + situation.GetType().ToString(), e);
                }
            }

            if ((prompt) || (count > 0))
            {
                Overwatch.AlarmNotify(Common.Localize(GetTitlePrefix() + ":Success", false, new object[] { count }));
            }
        }
    }
}
