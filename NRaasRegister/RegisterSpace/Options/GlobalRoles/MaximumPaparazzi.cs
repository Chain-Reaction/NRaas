using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
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

namespace NRaas.RegisterSpace.Options.GlobalRoles
{
    public class MaximumPaparazzi : IntegerSettingOption<GameObject>, IGlobalRolesOption
    {
        public override string GetTitlePrefix()
        {
            return "MaximumPaparazzi";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP3)) return false;

            return base.Allow(parameters);
        }

        protected override string GetPrompt()
        {
            return Common.Localize("Maximum:Prompt", false, new object[] { Name });
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
                return Register.Settings.mMaximumPaparazzi;
            }
            set
            {
                Register.Settings.mMaximumPaparazzi = value;

                RoleData data = RoleData.GetData(Role.RoleType.Paparazzi, WorldName.Undefined);
                if (data != null)
                {
                    data.mMinSpecCount = value;
                    data.mMidSpeCount = value;
                    data.mMaxSpecCount = value;
                }

                List<Role> roles =  new List<Role>(RoleManager.GetRolesOfType(Role.RoleType.Paparazzi));
                int count = 0;
                foreach(Role role in roles)
                {
                    count++;
                    if (count > value)
                    {
                        if (role.mSim != null)
                        {
                            Annihilation.Perform(role.mSim, true);
                        }
                    }
                }
            }
        }
    }
}