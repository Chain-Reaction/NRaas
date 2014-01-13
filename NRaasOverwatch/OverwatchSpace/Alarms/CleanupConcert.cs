using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Lighting;
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
    public class CleanupConcert : AlarmOption
    {
        public override string GetTitlePrefix()
        {
            return "CleanupConcert";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mCleanupConcert;
            }
            set
            {
                NRaas.Overwatch.Settings.mCleanupConcert = value;
            }
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log("Cleanup Concert");

                foreach (ShowVenue obj in Sims3.Gameplay.Queries.GetObjects<ShowVenue>())
                {
                    Overwatch.Log("Reset " + obj.ShowType);

                    foreach (ISearchLight light in obj.LotCurrent.GetObjects<ISearchLight>())
                    {
                        try
                        {
                            light.TurnOff();
                        }
                        catch
                        { }

                        SearchLight searchLight = light as SearchLight;
                        if (searchLight != null)
                        {
                            try
                            {
                                searchLight.mSMC.Dispose();
                            }
                            catch
                            { }

                            searchLight.mSMC = null;
                        }
                    }

                    obj.EndPlayerConcert();
                }

                if (prompt)
                {
                    Overwatch.AlarmNotify(Common.Localize("CleanupConcert:Complete"));
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }
    }
}
