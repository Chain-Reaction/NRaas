using NRaas.CommonSpace.Helpers;
using NRaas.RegisterSpace;
using NRaas.RegisterSpace.Helpers;
using NRaas.RegisterSpace.Options.Tourists;
using NRaas.RegisterSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.PetSystems;
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

namespace NRaas
{
    public class Register : Common, Common.IPreLoad, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static List<Role> sStoredActiveRoles = new List<Role>();

        static MethodStore sStoryProgressionRetire = new MethodStore("NRaasStoryProgression", "NRaas.StoryProgressionSpace.Managers.ManagerCareer", "ApplyRetiredCareer", new Type[] { typeof(SimDescription) });

        static Register()
        {
            Bootstrap();
        }

        public static PersistedSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new PersistedSettings();
                }

                return sSettings;
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;
        }

        public void OnPreLoad()
        {
            // Change all Resident requirements to townies (this stops the game from immigrating sims directly into lots)
            foreach (ICollection<Dictionary<Role.RoleType, RoleData>> worldRoleList in new ICollection<Dictionary<Role.RoleType, RoleData>>[] { RoleData.sData.Values, RoleData.sWorldTypeData.Values })
            {
                foreach (Dictionary<Role.RoleType, RoleData> worldRoles in worldRoleList)
                {
                    foreach (RoleData data in worldRoles.Values)
                    {
                        if (data.FillRoleFrom == Role.RoleFillFrom.Residents)
                        {
                            data.mFillRollFrom = Role.RoleFillFrom.Townies;
                        }
                    }
                }
            }

            // Change the defaults for tourists and explorers to the Sunset
            Dictionary<Role.RoleType, RoleData> defaults;
            if (RoleData.sData.TryGetValue(WorldName.Undefined, out defaults))
            {
                foreach (Role.RoleType type in new Role.RoleType[] { Role.RoleType.Tourist, Role.RoleType.Explorer })
                {
                    RoleData defaultData;
                    defaults.TryGetValue(type, out defaultData);

                    if ((defaultData == null) || (defaultData.Number == 0))
                    {
                        RoleData newDefault = RoleData.GetData(type, WorldName.SunsetValley, false);
                        if (newDefault == null)
                        {
                            newDefault = RoleData.GetData(type, WorldName.France, false);
                        }

                        if (newDefault != null)
                        {
                            defaults.Remove(type);
                            defaults.Add(type, CloneRole(newDefault, WorldName.Undefined, newDefault.FillRoleFrom));
                        }
                    }
                }
            }

            // Reset all the roles for these worlds back to being residents
            foreach (WorldName world in new WorldName[] { WorldName.China, WorldName.France, WorldName.Egypt })
            {
                foreach (Role.RoleType type in Enum.GetValues(typeof(Role.RoleType)))
                {
                    CloneResidentRole(type, world);
                }
            }
        }

        public void OnWorldLoadFinished()
        {
            kDebugging = Settings.Debugging;

            StoreRoles();

            foreach (Role.RoleType type in Enum.GetValues(typeof(Role.RoleType)))
            {
                CleanupBadRole(type);
            }

            Settings.ValidateObjects();

            AlterPetPool();

            TouristAmount.ApplySize();

            new AlarmTask(1, TimeUnit.Seconds, OnRestoreRoles);

            // Must be performed after CleanupBadRole
            new RoleManagerTaskEx.StartupTask();
        }

        protected static void CloneResidentRole(Role.RoleType type, WorldName world)
        {
            RoleData data = RoleData.GetData(type, world, true);
            if (data == null) return;

            if (data.FillRoleFrom != Role.RoleFillFrom.Townies) return;

            RoleData r = CloneRole(data, world, Role.RoleFillFrom.Residents);
            if (r == null) return;

            Dictionary<Role.RoleType, RoleData> dictionary;
            if (!RoleData.sData.TryGetValue(r.World, out dictionary))
            {
                dictionary = new Dictionary<Role.RoleType, RoleData>();
                RoleData.sData.Add(r.World, dictionary);
            }

            dictionary.Remove(r.Type);
            dictionary.Add(r.Type, r);
        }

        protected static RoleData CloneRole(RoleData data, WorldName world, Role.RoleFillFrom fillType)
        {
            RoleData.ConstructorData conData = new RoleData.ConstructorData();
            conData.World = world;
            conData.Type = data.mType;
            conData.MaxSpecCount = data.mMaxSpecCount;
            conData.MidSpecCount = data.mMidSpeCount;
            conData.MinSpecCount = data.mMinSpecCount;
            conData.StartTime = data.mStartTime;
            conData.EndTime = data.mEndTime;
            conData.AgeSpecies = data.mAgeSpecies;// | CASAgeGenderFlags.YoungAdult | CASAgeGenderFlags.Adult | CASAgeGenderFlags.Elder;
            conData.Motives = data.mMotives;
            conData.FillRollFrom = fillType;
            conData.ValidProductVersion = data.mValidProductVersion;
            conData.MotivesToFreeze = data.mMotivesToFreeze;
            conData.UseHoverbot = data.mUseHoverbot;
            conData.UseServobot = data.mUseServobot;

            conData.MaleUniform = null;
            conData.FemaleUniform = null;
            conData.MaleUniformElder = null;
            conData.FemaleUniformElder = null;

            conData.FutureWorldMaleUniform = null;
            conData.FutureWorldFemaleUniform = null;
            conData.FutureWorldMaleUniformElder = null;
            conData.FutureWorldFemaleUniformElder = null;

            RoleData r = new RoleData(conData);

            r.mFemaleUniform = data.mFemaleUniform;
            r.mFemaleUniformElder = data.mFemaleUniformElder;
            r.mMaleUniform = data.mMaleUniform;
            r.mMaleUniformElder = data.mMaleUniformElder;

            r.mFutureWorldFemaleUniform = data.mFutureWorldFemaleUniform;
            r.mFutureWorldFemaleUniformElder = data.mFutureWorldFemaleUniformElder;
            r.mFutureWorldMaleUniform = data.mFutureWorldMaleUniform;
            r.mFutureWorldMaleUniformElder = data.mFutureWorldMaleUniformElder;

            return r;
        }

        protected static void CleanupBadRole(Role.RoleType type)
        {
            try
            {
                List<Role> roles = RoleManager.GetRolesOfType(type);
                if (roles != null)
                {
                    for (int i = roles.Count - 1; i >= 0; i--)
                    {
                        if (roles[i] == null)
                        {
                            roles.RemoveAt(i);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(type.ToString(), e);
            }
        }

        public static void StoreRoles()
        {
            sStoredActiveRoles.Clear();

            if (Household.ActiveHousehold != null)
            {
                foreach (SimDescription sim in Household.ActiveHousehold.AllSimDescriptions)
                {
                    if (sim.AssignedRole == null) continue;

                    sStoredActiveRoles.Add(sim.AssignedRole);

                    sim.AssignedRole = null;
                }
            }
        }

        public static void OnRestoreRoles()
        {
            foreach (Role role in sStoredActiveRoles)
            {
                role.mSim.AssignedRole = role;
            }

            sStoredActiveRoles.Clear();

            if (!Register.Settings.mDisableServiceCleanup)
            {
                // Must be delayed until after the services are initialized
                ServiceCleanup.Task.Perform();
                ServicePoolCleanup.Task.Perform();
            }
        }

        public static string GetRoleName(Role.RoleType type)
        {
            string key = null;

            switch (type)
            {
                case Role.RoleType.Bouncer:
                    key = "Gameplay/Objects/Miscellaneous/VelvetRopes:BouncerRoleName";
                    break;
                case Role.RoleType.GenericMerchant:
                    key = "Gameplay/Objects/Register/ShoppingRegister:ConsignmentRoleRegister";
                    break;
                case Role.RoleType.PetStoreMerchant:
                    key = "Gameplay/Objects/Register/ShoppingRegister:PetstoreRoleRegister";
                    break;
                /* This has the same translation as the Consignment Role
                case Role.RoleType.PotionShopMerchant:
                    key = "Gameplay/Objects/Register/ShoppingRegister:PotionShopRoleRegister";
                    break;
                */
                default:
                    key = "Gameplay/Roles/Role" + type + ":" + type + "CareerTitle";
                    break;
            }

            return Common.LocalizeEAString(key);
        }

        public static void AlterPetPool()
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP5)) return;

            Common.StringBuilder msg = new Common.StringBuilder();

            foreach (KeyValuePair<PetPoolType,PetPoolConfig> element in PetPoolManager.sPetConfigManager)
            {
                msg += Common.NewLine + element.Key;

                int poolSize = Settings.GetMaximumPoolSize(element.Key);
                if (poolSize == -1) continue;

                msg += Common.NewLine + " Maximum: " + poolSize;

                element.Value.mEP5Range[1] = poolSize;
                element.Value.mEP5Range[2] = poolSize;
                element.Value.mOtherWorldRange[1] = poolSize;
                element.Value.mOtherWorldRange[2] = poolSize;

                List<SimDescription> destroyed = PetPoolManager.GetPetsByType(element.Key);

                int remove = PetPoolManager.GetPoolSize(element.Key) - poolSize;

                msg += Common.NewLine + " Remove: " + remove;

                if (remove > 0)
                {
                    PetPoolManager.RefreshPool(element.Key, remove);

                    List<SimDescription> remaining = PetPoolManager.GetPetsByType(element.Key);
                    if (remaining != null)
                    {
                        foreach (SimDescription remain in remaining)
                        {
                            destroyed.Remove(remain);
                        }
                    }

                    int destroyedCount = 0;
                    foreach (SimDescription destroy in destroyed)
                    {
                        if (ServiceCleanup.AttemptServiceDisposal(destroy, false, "Too Many " + element.Key))
                        {
                            destroyedCount++;
                        }
                        else
                        {
                            Household.PetHousehold.Add(destroy);
                        }
                    }

                    msg += Common.NewLine + " Destroyed: " + destroyedCount;
                }

                PetPool pool;
                if (PetPoolManager.TryGetPetPool(element.Key, out pool))
                {
                    if (pool.mSimDescriptionIds == null)
                    {
                        // Setting this off null will ensure that MaximumThresholdReached() returns TRUE for a size of "0"
                        pool.mSimDescriptionIds = new List<ulong>();

                        msg += Common.NewLine + " Repooled";
                    }
                }
            }

            Common.DebugNotify(msg);
        }

        public static bool DropRole(SimDescription sim, IRoleGiver target)
        {
            if (RoleManager.sRoleManager == null) return false;

            if (target != null)
            {
                switch (target.RoleType)
                {
                    case Role.RoleType.SpecialMerchant:
                    case Role.RoleType.Explorer:
                    case Role.RoleType.Tourist:
                        return false;
                }

                if (target.CurrentRole != null)
                {
                    target.CurrentRole.RemoveSimFromRole();
                }
            }

            if (sim.AssignedRole != null)
            {
                sim.AssignedRole.RemoveSimFromRole();
            }

            return true;
        }

        public static void ShowNotice(Role role)
        {
            sStoryProgressionRetire.Invoke<bool>(new object[] { role.mSim });

            if (Settings.mShowNotices)
            {
                if (string.IsNullOrEmpty(role.CareerTitleKey)) return;

                ObjectGuid id1 = ObjectGuid.InvalidObjectGuid;
                if (role.SimInRole != null)
                {
                    id1 = role.SimInRole.ObjectId;
                }

                ObjectGuid id2 = ObjectGuid.InvalidObjectGuid;
                if (role.RoleGivingObject != null)
                {
                    id2 = role.RoleGivingObject.ObjectId;
                }

                Notify(Common.LocalizeEAString(role.mSim.IsFemale, role.CareerTitleKey, new object[0]) + ": " + role.mSim.FullName, id1, id2, StyledNotification.NotificationStyle.kDebugAlert);
            }
        }

        public static bool AssignRole(SimDescription sim, Role.RoleType type, IRoleGiver target)
        {
            try
            {
                RoleData data = RoleData.GetDataForCurrentWorld(type, true);
                if (data == null) return false;

                Role role = Role.CreateRole(data, sim, target);
                if (role == null) return false;

                if ((role.Data.StartTime == role.Data.EndTime) || (data.IsValidTimeForRole()))
                {
                    role.StartRoleAlarmHandler();
                }

                RoleManager.sRoleManager.AddRole(role);

                if (target != null)
                {
                    target.CurrentRole = role;
                }

                ShowNotice(role);
            }
            catch(Exception e)
            {
                Exception(sim, e);
            }

            return true;
        }

        public static void ValidateOutfit(SimDescription sim, ref SimOutfit outfit, ref int index)
        {
            if ((outfit == null) || !outfit.IsValid)
            {
                int count = sim.GetOutfitCount(OutfitCategories.Career);
                if (count > 0)
                {
                    outfit = sim.GetOutfit(OutfitCategories.Career, 0);
                    index = 0;
                }
            }
        }
    }
}
