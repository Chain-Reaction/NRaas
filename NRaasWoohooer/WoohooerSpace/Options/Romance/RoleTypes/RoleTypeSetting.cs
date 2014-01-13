using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Romance.RoleTypes
{
    public class RoleTypeSetting : BooleanSettingOption<GameObject>, IOptionItem
    {
        Role.RoleType mType;

        public RoleTypeSetting(Role.RoleType type)
        {
            mType = type;
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mDisallowAutonomousRoleTypes.Contains(mType);
            }
            set
            {
                if (value)
                {
                    if (!NRaas.Woohooer.Settings.mDisallowAutonomousRoleTypes.Contains(mType))
                    {
                        NRaas.Woohooer.Settings.mDisallowAutonomousRoleTypes.Add(mType);
                    }
                }
                else
                {
                    NRaas.Woohooer.Settings.mDisallowAutonomousRoleTypes.Remove(mType);
                }
            }
        }

        public override string GetTitlePrefix()
        {
            return "DisallowRole";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public static string GetRoleName(Role.RoleType type)
        {
            string key = null;
            switch (type)
            {
                case Role.RoleType.Bartender:
                    key = "Gameplay/Roles/RoleBartender:BartenderCareerTitle";
                    break;
                case Role.RoleType.Bouncer:
                    key = "Gameplay/Objects/Miscellaneous/VelvetRopes:BouncerRoleName";
                    break;
                case Role.RoleType.Explorer:
                    key = "Gameplay/Roles/RoleExplorer:Explorer";
                    break;
                case Role.RoleType.GenericMerchant:
                    key = "Gameplay/Objects/Register/ShoppingRegister:ConsignmentRoleRegister";
                    break;
                case Role.RoleType.LocationMerchant:
                    key = "Gameplay/Roles/RoleLocationMerchant:LocationMerchantCareerTitle";
                    break;
                case Role.RoleType.Paparazzi:
                    key = "Gameplay/Roles/RolePaparazzi:PaparazziCareerTitle";
                    break;
                case Role.RoleType.Pianist:
                    key = "Gameplay/Roles/RolePianist:PianistCareerTitle";
                    break;
                case Role.RoleType.SpecialMerchant:
                    key = "Gameplay/Roles/RoleSpecialMerchant:SpecialMerchantCareerTitle";
                    break;
                case Role.RoleType.Stylist:
                    key = "Gameplay/Roles/RoleStylist:StylistCareerTitle";
                    break;
                case Role.RoleType.TattooArtist:
                    key = "Gameplay/Roles/RoleTattooArtist:TattooArtistCareerTitle";
                    break;
                case Role.RoleType.Tourist:
                    key = "Gameplay/Roles/RoleTourist:Tourist";
                    break;
            }

            if (key == null) return null;

            string title;
            if (!Localization.GetLocalizedString(key, out title))
            {
                title = key;
            }

            return title;
        }

        public override string Name
        {
            get
            {
                return GetRoleName(mType);
            }
        }
    }
}
