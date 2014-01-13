using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public class TurnOffStereos : AlarmOption
    {
        public TurnOffStereos()
        { }

        public override string GetTitlePrefix()
        {
            return "TurnOffStereos";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mTurnOffStereos;
            }
            set
            {
                NRaas.Overwatch.Settings.mTurnOffStereos = value;
            }
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log("Turn Off Stereos");

                int count = 0;

                List<Stereo> stereos = new List<Stereo>(Sims3.Gameplay.Queries.GetObjects<Stereo>());
                foreach (Stereo obj in stereos)
                {
                    if (obj is StereoCommunity) continue;

                    if ((!NRaas.Overwatch.Settings.mAffectActiveLot) && (Household.ActiveHousehold != null))
                    {
                        if (obj.LotCurrent == null) continue;

                        if (obj.LotCurrent.Household == Household.ActiveHousehold) continue;

                        bool found = false;
                        foreach (Sim sim in Households.AllSims(Household.ActiveHousehold))
                        {
                            if (sim.LotCurrent == obj.LotCurrent)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found) continue;
                    }

                    if (obj.TurnedOn)
                    {
                        try
                        {
                            obj.TurnOff();
                            count++;
                        }
                        catch
                        {
                            obj.SetObjectToReset();
                        }
                    }
                }

                if (count > 0)
                {
                    Overwatch.AlarmNotify(Common.Localize("TurnOffStereos:Success", false, new object[] { count }));
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Name, exception);
            }
        }
    }
}
