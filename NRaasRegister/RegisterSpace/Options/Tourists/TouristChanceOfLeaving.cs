using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RegisterSpace.Options.Tourists
{
    public class TouristChanceOfLeaving : IntegerSettingOption<GameObject>, ITouristOption
    {
        public override string GetTitlePrefix()
        {
            return "TouristChanceOfLeaving";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override int Validate(int value)
        {
            if (value < 0)
            {
                value = 0;
            }

            return base.Validate(value);
        }

        protected override int Value
        {
            get
            {
                return Register.Settings.mTouristChanceOfLeaving;
            }
            set
            {
                Register.Settings.mTouristChanceOfLeaving = value;
            }
        }
    }
}