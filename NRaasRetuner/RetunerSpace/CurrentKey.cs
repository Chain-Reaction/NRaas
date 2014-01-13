using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.RetunerSpace.Booters;
using NRaas.RetunerSpace.Helpers.Stores;
using NRaas.RetunerSpace.Options.Tunable;
using NRaas.RetunerSpace.Options.Tunable.Fields;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace
{
    public class CurrentKey : SettingsKey
    {
        public CurrentKey()
        { }

        public override bool IsActive
        {
            get { return false; }
        }

        public override string LocalizedName
        {
            get
            {
                return Common.Localize("Settings:Current");
            }
        }

        public override Season Season
        {
            get
            {
                return sAllSeasons;
            }
        }

        public override bool IsEqual(SettingsKey o)
        {
            return (o is CurrentKey);
        }

        public override void Export(Persistence.Lookup settings)
        { }

        public override void Import(Persistence.Lookup settings)
        { }

        public override string ToXMLString()
        {
            return null;
        }
    }
}
