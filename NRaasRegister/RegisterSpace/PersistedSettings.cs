using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RegisterSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Whether to display the menu when clicking on a Lot")]
        public static bool kShowLotMenu = true;
        [Tunable, TunableComment("Whether to display the registration notifications")]
        public static bool kShowNotices = true;
        [Tunable, TunableComment("How much to pay each role assigned sim per hour they are working")]
        public static int kPayPerHour = 100;
        [Tunable, TunableComment("Whether to allow tourists to spawn in the town")]
        public static bool kAllowTourists = true;
        [Tunable, TunableComment("Whether to allow paparazzi to spawn in the town")]
        public static bool kAllowPaparazzi = true;
        [Tunable, TunableComment("Whether to allow immigrants to spawn to fill role positions")]
        public static bool kAllowImmigration = true;

        [Tunable, TunableComment("The maximum number of wild horses that can be spawned by the Pet Manager")]
        public static int kMaximumWildHorses = 4;
        [Tunable, TunableComment("The maximum number of unicorns that can be spawned by the Pet Manager")]
        public static int kMaximumUnicorns = 1;
        [Tunable, TunableComment("The maximum number of stray cats that can be spawned by the Pet Manager")]
        public static int kMaximumStrayDogs = 6;
        [Tunable, TunableComment("The maximum number of stray dogs that can be spawned by the Pet Manager")]
        public static int kMaximumStrayCats = 6;
        [Tunable, TunableComment("The maximum number of deer that can be spawned by the Pet Manager")]
        public static int kMaximumDeer = 3;
        [Tunable, TunableComment("The maximum number of raccoon that can be spawned by the Pet Manager")]
        public static int kMaximumRaccoon = 2;
        [Tunable, TunableComment("Whether to display debugging notices")]
        public static bool kDebugging = false;
        [Tunable, TunableComment("Whether to display a notice when immigration is required for a role")]
        public static bool kShowImmigrationNotice = true;
        [Tunable, TunableComment("Whether to allow residents to be automatically assigned to role positions")]
        public static bool kAllowResidentAssignment = false;

        [Tunable, TunableComment("How much to pay generic cashiers per hour they are working")]
        public static int kPayPerGenericMerchant = 100;
        [Tunable, TunableComment("How much to pay location cashiers per hour they are working")]
        public static int kPayPerLocationMerchant = 100;
        [Tunable, TunableComment("How much to pay pet store cashiers per hour they are working")]
        public static int kPayPerPetRegister = 100;
        [Tunable, TunableComment("How much to pay bartenders per hour they are working")]
        public static int kPayPerBartender = 100;
        [Tunable, TunableComment("How much to pay bouncers per hour they are working")]
        public static int kPayPerBouncer = 100;
        [Tunable, TunableComment("How much to pay paparazzi per hour they are working")]
        public static int kPayPerPaparazzi = 100;
        [Tunable, TunableComment("How much to pay special merchant per hour they are working")]
        public static int kPayPerSpecialMerchant = 100;
        [Tunable, TunableComment("How much to pay stylists per hour they are working")]
        public static int kPayPerStylist = 100;
        [Tunable, TunableComment("How much to pay tattoo artists per hour they are working")]
        public static int kPayPerTattooArtist = 100;
        [Tunable, TunableComment("How much to pay pianists per hour they are working")]
        public static int kPayPerPianist = 100;

        [Tunable, TunableComment("Chance of a tourist leaving every 75 minutes")]
        public static int kTouristChanceOfLeaving = 25;
        [Tunable, TunableComment("Size of the tourist pool")]
        public static int kTouristAmount = 10;
//--- New
        [Tunable, TunableComment("How much to pay proprietors per hour they are working")]
        public static int kPayPerProprietor = 100;
        [Tunable, TunableComment("How much to pay concession stand merchants per hour they are working")]
        public static int kPayPerConcessionsStandMerchant = 100;
        [Tunable, TunableComment("How much to pay kissing booth merchants per hour they are working")]
        public static int kPayPerKissingBoothAttendant = 100;
        [Tunable, TunableComment("How much to pay potion merchants per hour they are working")]
        public static int kPayPerPotionShopMerchant = 100;
        [Tunable, TunableComment("How much to pay University Mascots per hour they are working")]
        public static int kPayPerUniversityMascot = 100;
        [Tunable, TunableComment("How much to pay Barista Bar Tenders per hour they are working")]
        public static int kPayPerBaristaBarTender = 100;
        [Tunable, TunableComment("How much to pay Hobby Shop Merchants per hour they are working")]
        public static int kPayPerHobbyShopMerchant = 100;
        [Tunable, TunableComment("How much to pay Cafeteria Waiters per hour they are working")]
        public static int kPayPerCafeteriaWaiter = 100;
        [Tunable, TunableComment("How much to pay Bot Shop Merchants per hour they are working")]
        public static int kPayPerBotShopMerchant = 100;

        [Tunable, TunableComment("Whether to allow homeworld residents to be used as tourists")]
        public static bool kAllowHomeworldTourists = true;
        
        public bool mShowLotMenu = kShowLotMenu;
        public bool mShowNotices = kShowNotices;
        public int mPayPerHour = kPayPerHour;
        public bool mAllowTourists = kAllowTourists;
        public bool mAllowPaparazzi = kAllowPaparazzi;
        public bool mAllowImmigration = kAllowImmigration;
        public bool mShowImmigrationNotice = kShowImmigrationNotice;
        public bool mAllowResidentAssignment = kAllowResidentAssignment;
        
        private bool mDebugging = kDebugging;

        public int mMaximumWildHorses = kMaximumWildHorses;
        public int mMaximumUnicorns = kMaximumUnicorns;
        public int mMaximumStrayDogs = kMaximumStrayDogs;
        public int mMaximumStrayCats = kMaximumStrayCats;
        public int mMaximumDeer = kMaximumDeer;
        public int mMaximumRaccoon = kMaximumRaccoon;

        public int mTouristChanceOfLeaving = kTouristChanceOfLeaving;
        public int mTouristAmount = kTouristAmount;

        public bool mDisableServiceCleanup = false;

        public Dictionary<Role.RoleType, int> mPayPerRole = new Dictionary<Role.RoleType, int>();

        public Dictionary<ObjectGuid, bool> mDisabledAssignment = new Dictionary<ObjectGuid, bool>();

        public bool mAllowHomeworldTourists = kAllowHomeworldTourists;

        public Dictionary<WorldName, bool> mDisabledTouristWorlds = new Dictionary<WorldName, bool>();

        public PersistedSettings()
        {
            mPayPerRole.Add(Role.RoleType.GenericMerchant, kPayPerGenericMerchant);
            mPayPerRole.Add(Role.RoleType.LocationMerchant, kPayPerLocationMerchant);
            mPayPerRole.Add(Role.RoleType.PetStoreMerchant, kPayPerPetRegister);
            mPayPerRole.Add(Role.RoleType.Bartender, kPayPerBartender);
            mPayPerRole.Add(Role.RoleType.Bouncer, kPayPerBouncer);
            mPayPerRole.Add(Role.RoleType.Paparazzi, kPayPerPaparazzi);
            mPayPerRole.Add(Role.RoleType.SpecialMerchant, kPayPerSpecialMerchant);
            mPayPerRole.Add(Role.RoleType.Stylist, kPayPerStylist);
            mPayPerRole.Add(Role.RoleType.TattooArtist, kPayPerTattooArtist);
            mPayPerRole.Add(Role.RoleType.Pianist, kPayPerPianist);
            mPayPerRole.Add(Role.RoleType.Proprietor, kPayPerProprietor);
            mPayPerRole.Add(Role.RoleType.ConcessionsStandMerchant, kPayPerConcessionsStandMerchant);
            mPayPerRole.Add(Role.RoleType.KissingBoothAttendantMale, kPayPerKissingBoothAttendant);
            mPayPerRole.Add(Role.RoleType.KissingBoothAttendantFemale, kPayPerKissingBoothAttendant);
            mPayPerRole.Add(Role.RoleType.PotionShopMerchant, kPayPerPotionShopMerchant);
            mPayPerRole.Add(Role.RoleType.UniversityMascot, kPayPerUniversityMascot);
            mPayPerRole.Add(Role.RoleType.BaristaBarTender, kPayPerBaristaBarTender);
            mPayPerRole.Add(Role.RoleType.HobbyShopMerchant, kPayPerHobbyShopMerchant);
            mPayPerRole.Add(Role.RoleType.CafeteriaWaiter, kPayPerCafeteriaWaiter);
            mPayPerRole.Add(Role.RoleType.BotShopMerchant, kPayPerBotShopMerchant);
        }

        public void ValidateObjects()
        {
            List<ObjectGuid> remove = new List<ObjectGuid>();

            foreach (ObjectGuid guid in mDisabledAssignment.Keys)
            {
                if (Simulator.GetProxy(guid) != null) continue;

                remove.Add(guid);
            }

            foreach (ObjectGuid guid in remove)
            {
                mDisabledAssignment.Remove(guid);
            }
        }

        public int GetPayPerRole(Role.RoleType type)
        {
            int value;
            if (!mPayPerRole.TryGetValue(type, out value))
            {
                return 0;
            }

            return value;
        }

        public void SetPayPerRole(Role.RoleType type, int value)
        {
            mPayPerRole[type] = value;
        }

        public int GetPayPerHour(Role role)
        {
            int result = mPayPerHour;

            int rolePay = GetPayPerRole(role.mType);

            result = Math.Max(result, rolePay);

            return result;
        }

        public int GetMaximumPoolSize(PetPoolType type)
        {
            switch (type)
            {
                case PetPoolType.NPCDeer:
                    return mMaximumDeer;
                case PetPoolType.NPCRaccoon:
                    return mMaximumRaccoon;
                case PetPoolType.StrayCat:
                    return mMaximumStrayCats;
                case PetPoolType.StrayDog:
                    return mMaximumStrayDogs;
                case PetPoolType.Unicorn:
                    return mMaximumUnicorns;
                case PetPoolType.WildHorse:
                    return mMaximumWildHorses;
                default:
                    return -1;
            }
        }

        public bool Debugging
        {
            get
            {
                return mDebugging;
            }
            set
            {
                mDebugging = value;

                Common.kDebugging = value;
            }
        }
    }
}
