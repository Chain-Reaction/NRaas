using NRaas.CommonSpace.Helpers;
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
using Sims3.Gameplay.Objects.Appliances;
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
    public class CleanseTheDead : AlarmOption
    {
        public override string GetTitlePrefix()
        {
            return "CleanseTheDead";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mCleanseTheDead;
            }
            set
            {
                NRaas.Overwatch.Settings.mCleanseTheDead = value;
            }
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log("Cleanse The Dead");

                Dictionary<SimDescription, Pair<IMausoleum, Urnstone>> urnstones = new Dictionary<SimDescription, Pair<IMausoleum, Urnstone>>();
                CommonSpace.Helpers.CleanseTheDead.Retrieve(urnstones);

                List<SimDescription> choices = new List<SimDescription>();

                if (Overwatch.Settings.mCompressFamilyLevel > 0)
                {
                    foreach (SimDescription choice in urnstones.Keys)
                    {
                        if (Genealogies.GetFamilyLevel(choice.Genealogy) >= Overwatch.Settings.mCompressFamilyLevel)
                        {
                            choices.Add(choice);
                        }
                    }
                }
                else
                {
                    choices.AddRange(urnstones.Keys);
                }

                CommonSpace.Helpers.CleanseTheDead.Cleanse(choices, urnstones, true, Overwatch.Log);

                if ((choices.Count > 0) || (prompt))
                {
                    Overwatch.AlarmNotify(Common.Localize("CleanseTheDead:Complete", false, new object[] { choices.Count }));
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }
    }
}
