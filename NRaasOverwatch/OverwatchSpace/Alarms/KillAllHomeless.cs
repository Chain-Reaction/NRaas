using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public class KillAllHomeless : AlarmOption
    {
        public KillAllHomeless()
        { }

        public override string GetTitlePrefix()
        {
            return "KillAllHomeless";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mKillAllHomeless;
            }
            set
            {
                NRaas.Overwatch.Settings.mKillAllHomeless = value;
            }
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log("Kill All Homeless");

                if ((!prompt) || (AcceptCancelDialog.Show(Common.Localize("KillAllHomeless:Prompt"))))
                {
                    List<Household> list = new List<Household>();
                    int iSimCount = 0;

                    foreach (Household household in Household.sHouseholdList)
                    {
                        if (household.IsSpecialHousehold) continue;

                        if (household.InWorld) continue;

                        if (Households.IsPassport(household)) continue;

                        if (Households.IsRole(household)) continue;

                        if (Households.IsLunarCycleZombie(household)) continue;

                        if (Households.IsActiveDaycare(household)) continue;

                        iSimCount += Households.NumSims(household);
                        list.Add(household);
                    }

                    foreach (Household household in list)
                    {
                        Annihilation.Cleanse(household);
                    }

                    if (iSimCount > 0)
                    {
                        Overwatch.AlarmNotify(Common.Localize("KillAllHomeless:Success", false, new object[] { iSimCount }));
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Name, exception);
            }
        }
    }
}
