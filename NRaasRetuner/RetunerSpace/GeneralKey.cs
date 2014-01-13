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
    public class GeneralKey : SettingsKey
    {
        public GeneralKey()
        { }
        public GeneralKey(Vector2 hours)
            : base(hours)
        { }

        public override string LocalizedName
        {
            get
            {
                return Common.Localize("Settings:General") + base.LocalizedName;
            }
        }

        public override bool IsEqual(SettingsKey o)
        {
            if (o is GeneralKey)
            {
                return base.IsEqual(o);
            }
            else
            {
                return false;
            }
        }
    }
}
