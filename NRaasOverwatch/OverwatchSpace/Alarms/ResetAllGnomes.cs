using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Interfaces;
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
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Roles;
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
    public class ResetAllGnomes : AlarmOption, Common.IDelayedWorldLoadFinished
    {
        public override string GetTitlePrefix()
        {
            return "ResetAllGnomes";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mResetAllGnomes;
            }
            set
            {
                NRaas.Overwatch.Settings.mResetAllGnomes = value;
            }
        }

        public void OnDelayedWorldLoadFinished()
        {
            PerformAction(false);   
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log("Reset All Gnomes");

                int count = 0;

                foreach (MagicGnomeBase gnome in Sims3.Gameplay.Queries.GetObjects<MagicGnomeBase>())
                {
                    if (gnome.IsAngry) continue;

                    if (gnome.LotCurrent == null) continue;

                    if (gnome.LotCurrent.AlarmManager.GetTimeLeft(gnome.mTrickeryAlarm, TimeUnit.Minutes) > 0) continue;

                    gnome.IsAngry = false;
                    count++;
                }

                if ((prompt) || (count > 0))
                {
                    Overwatch.AlarmNotify(Common.Localize("ResetAllGnomes:Success", false, new object[] { count }));
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }
    }
}
