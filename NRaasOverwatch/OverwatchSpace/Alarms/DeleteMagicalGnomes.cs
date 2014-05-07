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
    public class DeleteMagicalGnomes : AlarmOption, Common.IDelayedWorldLoadFinished
    {
        public override string GetTitlePrefix()
        {
            return "DeleteAllMagicalGnomes";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mDeleteAllMagicGnomes;
            }
            set
            {
                NRaas.Overwatch.Settings.mDeleteAllMagicGnomes = value;
            }
        }

        public void OnDelayedWorldLoadFinished()
        {
            PerformAction(false);
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            Overwatch.Log("Delete All Magical Gnomes");

            int count = 0;

            try
            {    
                foreach (MagicGnomeBase gnome in Sims3.Gameplay.Queries.GetObjects<MagicGnomeBase>())
                {
                    if (!gnome.DestroyOnGrab)
                    {
                        gnome.Dispose();
                        gnome.Destroy();
                        count++;                        
                    }
                }

                if ((prompt) || (count > 0))
                {
                    Overwatch.AlarmNotify(Common.Localize("DeleteAllMagicalGnomes:Success", false, new object[] { count }));
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }
    }
}