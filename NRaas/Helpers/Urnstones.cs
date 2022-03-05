using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class Urnstones
    {
        public static string GetLocalizedString(bool isFemale, SimDescription.DeathType type)
        {
            switch (type)
            {
                case SimDescription.DeathType.None:
                    return "";
                case SimDescription.DeathType.PetOldAgeBad:
                case SimDescription.DeathType.PetOldAgeGood:
                    return Common.LocalizeEAString(isFemale, "Gameplay/Objects/Urnstone:" + type.ToString());
            }

            return Urnstone.DeathTypeToLocalizedString(type);
        }

        // Lacks the "greater than eight" restriction
        public static bool GhostToPlayableGhost(Urnstone ths, Household newHousehold, Vector3 ghostPosition)
        {
            SimDescription simDescription = ths.DeadSimsDescription;

            if (!simDescription.IsValidDescription)
            {
                simDescription.Fixup();
            }

            if (simDescription.Household != null)
            {
                simDescription.Household.Remove(simDescription, !simDescription.Household.IsSpecialHousehold);
            }

            if (!newHousehold.Contains(simDescription))
            {
                newHousehold.Add(simDescription);
            }

            Sim ghost = Instantiation.Perform(simDescription, ghostPosition, null, null);
            if (ghost == null) return false;

            ths.GhostSetup(ghost, true);

            ths.RemoveMourningRelatedBuffs(ghost);

            simDescription.ShowSocialsOnSim = true;
            simDescription.IsNeverSelectable = false;
            simDescription.Marryable = true;
            simDescription.Contactable = true;

            if (!simDescription.IsEP11Bot)
            {
                simDescription.AgingEnabled = true;
                simDescription.AgingState.ResetAndExtendAgingStage(0f);
                simDescription.PushAgingEnabledToAgingManager();
            }

            string failureReason;
            if (!Inventories.TryToMove(ths, ghost.Inventory, true, out failureReason))
            {
                Common.DebugNotify(failureReason);
            }
            //Inventories.TryToMove(ths, ghost);

            if (simDescription.Child || simDescription.Teen)
            {
                simDescription.AssignSchool();
            }

            if (ghost.IsSelectable)
            {
                ghost.OnBecameSelectable();
            }

            return true;
        }

        public static Urnstone FindGhostsGrave(SimDescription sim)
        {
            foreach (Urnstone urnstone in Sims3.Gameplay.Queries.GetObjects<Urnstone>())
            {
                if (object.ReferenceEquals(urnstone.DeadSimsDescription, sim))
                {
                    return urnstone;
                }
            }

            return null;
        }

        public static bool MoveToMausoleum(Urnstone urnstone)
        {
            List<IMausoleum> mausoleums = new List<IMausoleum>(Sims3.Gameplay.Queries.GetObjects<IMausoleum>());
            if (mausoleums.Count > 0)
            {
                IMausoleum mausoleum = RandomUtil.GetRandomObjectFromList(mausoleums);
                if (mausoleum != null)
                {
                    foreach (IActor actor in urnstone.ReferenceList)
                    {
                        actor.InteractionQueue.PurgeInteractions(urnstone);
                    }

                    if (MoveToMausoleum(mausoleum, urnstone))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public static bool MoveToMausoleum(IMausoleum mausoleum, Urnstone urnstone)
        {
            bool bOriginalValue = mausoleum.Inventory.IgnoreInventoryValidation;

            try
            {
                mausoleum.Inventory.IgnoreInventoryValidation = true;

                if (Inventories.TryToMove(urnstone, mausoleum.Inventory))
                {
                    return true;
                }
            }
            finally
            {
                mausoleum.Inventory.IgnoreInventoryValidation = bOriginalValue;
            }

            return false;
        }

        public static bool GhostToSim(SimDescription me)
        {
            Urnstone stone = Urnstones.FindGhostsGrave(me);
            if (stone != null)
            {
                if (me.CreatedSim != null)
                {
                    stone.GhostToSim(me.CreatedSim, false, false);
                    return true;
                }
            }

            return false;
        }

        public static Urnstone SimToPlayableGhost(SimDescription me, SimDescription.DeathType deathType)
        {
            Urnstone urnstone = FindGhostsGrave(me);
            if (urnstone == null)
            {
                urnstone = PrivateCreateGrave(me);
                if (urnstone == null)
                {
                    return null;
                }
            }

            if (!Inventories.TryToMove(urnstone, me.CreatedSim))
            {
                MoveToMausoleum(urnstone);
            }

            me.IsGhost = true;
            me.SetDeathStyle(deathType, true);

            if (me.CreatedSim != null)
            {
                urnstone.GhostSetup(me.CreatedSim, true);
                if (!me.IsEP11Bot)
                {
                    me.AgingEnabled = true;
                    me.PushAgingEnabledToAgingManager();
                }
            }

            return urnstone;
        }

        public static Urnstone CreateGrave(SimDescription me, bool ignoreExisting)
        {
            return CreateGrave(me, SimDescription.DeathType.OldAge, ignoreExisting, false);
        }
        public static Urnstone CreateGrave(SimDescription me, SimDescription.DeathType deathType, bool ignoreExisting, bool report)
        {
            Urnstone urnstone = FindGhostsGrave(me);
            if ((urnstone != null) && (!ignoreExisting))
            {
                if ((urnstone.InInventory) || (urnstone.InWorld))
                {
                    return urnstone;
                }
            }

            Household originalHousehold = me.Household;

            bool addOnly = false;
            if (urnstone == null)
            {
                urnstone = PrivateCreateGrave(me);
                if (urnstone == null)
                {
                    return null;
                }
            }
            else
            {
                addOnly = true;
            }

            bool success = MoveToMausoleum(urnstone);          

            if (!success)
            {
                if (me.CreatedSim != null)
                {
                    if (Inventories.TryToMove(urnstone, me.CreatedSim.Inventory))
                    {
                        success = true;
                    }
                }

                if ((!success) && (Sim.ActiveActor != null))
                {
                    if (Inventories.TryToMove(urnstone, Sim.ActiveActor.Inventory))
                    {
                        success = true;
                    }
                }

                if (!success) return null;
            }

            if (report)
            {
                Common.Notify(Common.Localize("ForceKill:Success", me.IsFemale, new object[] { me }));
            }

            if ((addOnly) && (!ignoreExisting))
            {
                return urnstone;
            }

            if ((originalHousehold == Household.ActiveHousehold) && (me.CreatedSim != null))
            {
                HudModel model = HudController.Instance.Model as HudModel;

                foreach (SimInfo info in model.mSimList)
                {
                    if (info.mGuid == me.CreatedSim.ObjectId)
                    {
                        model.RemoveSimInfo(info);
                        model.mSimList.Remove(info);
                        break;
                    }
                }
            }

            if (me.CreatedSim != null)
            {
                me.CreatedSim.Destroy();
            }

            if (originalHousehold != null)
            {
                originalHousehold.Remove(me, !originalHousehold.IsSpecialHousehold);
            }

            if (me.DeathStyle == SimDescription.DeathType.None)
            {
                if (me.IsHuman)
                {
                    switch (deathType)
                    {
                        case SimDescription.DeathType.None:
                        case SimDescription.DeathType.PetOldAgeBad:
                        case SimDescription.DeathType.PetOldAgeGood:
                            deathType = SimDescription.DeathType.OldAge;
                            break;
                    }
                }
                else
                {
                    switch (deathType)
                    {
                        case SimDescription.DeathType.None:
                        case SimDescription.DeathType.OldAge:
                            deathType = SimDescription.DeathType.PetOldAgeGood;
                            break;
                    }
                }

                me.SetDeathStyle(deathType, true);
            }

            me.IsNeverSelectable = true;
            me.Contactable = false;
            me.Marryable = false;

            if (me.CreatedSim == PlumbBob.SelectedActor)
            {
                LotManager.SelectNextSim();
            }

            if (me.CareerManager != null)
            {
                me.CareerManager.LeaveAllJobs(Sims3.Gameplay.Careers.Career.LeaveJobReason.kDied);
            }

            urnstone.OnHandToolMovement();
            Urnstone.FinalizeSimDeath(me, originalHousehold);
            int num = ((int)Math.Floor((double)SimClock.ConvertFromTicks(SimClock.CurrentTime().Ticks, TimeUnit.Minutes))) % 60;
            urnstone.MinuteOfDeath = num;

            return urnstone;
        }

        protected static Urnstone PrivateCreateGrave(SimDescription corpse)
        {
            string style;
            ProductVersion version = ProductVersion.BaseGame;
            switch (corpse.Species)
            {
                case CASAgeGenderFlags.Dog:
                case CASAgeGenderFlags.LittleDog:
                    style = "tombstoneDog";
                    version = ProductVersion.EP5;
                    break;

                case CASAgeGenderFlags.Horse:
                    if (corpse.IsUnicorn)
                    {
                        style = "tombstoneUnicorn";
                    }
                    else
                    {
                        style = "tombstoneHorse";
                    }
                    version = ProductVersion.EP5;
                    break;

                case CASAgeGenderFlags.Cat:
                    style = "tombstoneCat";
                    version = ProductVersion.EP5;
                    break;

                default:
                    ulong lifetimeHappiness = corpse.LifetimeHappiness;
                    if (lifetimeHappiness >= Urnstone.LifetimeHappinessWealthyTombstone)
                    {
                        style = "UrnstoneHumanWealthy";
                    }
                    else if (lifetimeHappiness < Urnstone.LifetimeHappinessPoorTombstone)
                    {
                        style = "UrnstoneHumanPoor";
                    }
                    else
                    {
                        style = "UrnstoneHuman";
                    }
                    break;
            }

            Urnstone stone = GlobalFunctions.CreateObject(style, version, Vector3.OutOfWorld, 0, Vector3.UnitZ, null, null) as Urnstone;
            if (stone == null)
            {
                return null;
            }

            corpse.Fixup();

            stone.SetDeadSimDescription(corpse);

            stone.mPlayerMoveable = true;

            return stone;
        }

        public static bool GhostSpawn(Urnstone me, Lot lot)
        {
            if (me.DeadSimsDescription == null)
            {
                return false;
            }

            if (!me.DeadSimsDescription.IsValidDescription)
            {
                me.DeadSimsDescription.Fixup();
            }

            Vector3 position;
            if (me.DeadSimsDescription.ToddlerOrBelow)
            {
                position = lot.EntryPoint();
            }
            else if (!me.InInventory)
            {
                position = me.Position;
            }
            else
            {
                position = Service.GetPositionInRandomLot(lot);
            }

            Household.NpcHousehold.Add(me.DeadSimsDescription);
            Sim sim = Instantiation.Perform(me.DeadSimsDescription, position, null, null);
            sim.SetOpacity(0f, 0f);

            ActiveTopic.AddToSim(sim, "Ghost");
            me.GhostSetup(sim, true);

            if (!me.InInventory)
            {
                sim.GreetSimOnLot(me.LotCurrent);
                Audio.StartObjectSound(me.ObjectId, "sting_ghost_appear", false);
            }

            sim.FadeIn();
            me.CreateAlarmReturnToGrave(false);
            return true;
        }
    }
}

