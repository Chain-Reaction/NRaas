using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Fireplaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class CleanAll : HouseholdFromList, IHouseholdOption
    {
        public override string GetTitlePrefix()
        {
            return "CleanAll";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            if (lot == null) return false;

            return (true);
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            if (lot == null) return OptionResult.Failure;

            World.DecayLeaves(lot.LotId, 1f);

            foreach (GameObject obj in lot.GetObjects<GameObject>())
            {
                if (obj.InUse) continue;

                if (obj.InInventory) continue;

                if (obj.IsCleanable)
                {
                    obj.Cleanable.ForceClean();
                }

                Hamper hamper = obj as Hamper;
                if (hamper != null)
                {
                    hamper.RemoveClothingPiles();
                }

                IThrowAwayable awayable = obj as IThrowAwayable;
                if ((awayable != null) &&
                    (!awayable.InUse) &&
                    (awayable.HandToolAllowUserPickupBase()) &&
                    (awayable.ShouldBeThrownAway()) &&
                    ((awayable.Parent == null) || (!awayable.Parent.InUse)) &&
                    (!(awayable is Bar.Glass)) &&
                    (!(awayable is Bill)))
                {
                    bool flag = false;
                    if (awayable is BarTray)
                    {
                        foreach (Slot slot in awayable.GetContainmentSlots())
                        {
                            if (awayable.GetContainedObject(slot) is Bar.Glass)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        awayable.ThrowAwayImmediately();
                    }
                }

                if (obj is IDestroyOnMagicalCleanup)
                {
                    obj.FadeOut(false, true);
                }
            }

            lot.FireManager.RestoreObjects();

            LotLocation[] puddles = World.GetPuddles(lot.LotId, LotLocation.Invalid);
            if (puddles.Length > 0x0)
            {
                foreach (LotLocation location in puddles)
                {
                    if ((lot.TombRoomManager == null) || !lot.TombRoomManager.IsObjectInATombRoom(location))
                    {
                        PuddleManager.RemovePuddle(lot.LotId, location);
                    }
                }
            }

            LotLocation[] burntTiles = World.GetBurntTiles(lot.LotId, LotLocation.Invalid);
            if (burntTiles.Length > 0x0)
            {
                foreach (LotLocation location2 in burntTiles)
                {
                    if ((lot.LotLocationIsPublicResidential(location2)) && ((lot.TombRoomManager == null) || !lot.TombRoomManager.IsObjectInATombRoom(location2)))
                    {
                        World.SetBurnt(lot.LotId, location2, false);
                    }
                }
            }

            if (me != null)
            {
                List<Fridge> fridges = new List<Fridge>(lot.GetObjects<Fridge>());
                if (fridges.Count > 0)
                {
                    Fridge fridge = fridges[0];
                    if ((fridge != null) &&
                        (me.SharedFridgeInventory != null) &&
                        (me.SharedFamilyInventory.Inventory != null))
                    {
                        foreach (ServingContainer container in Sims3.Gameplay.Queries.GetObjects<ServingContainer>(lot))
                        {
                            if ((!container.InUse) &&
                                (fridge.HandToolAllowDragDrop(container)) &&
                                (container.HasFood) && 
                                (container.HasFoodLeft()) &&
                                (!container.IsSpoiled) &&
                                (container.GetQuality() >= Quality.Neutral))
                            {
                                me.SharedFridgeInventory.Inventory.TryToAdd(container, false);
                            }
                        }
                    }
                }
            }

            foreach (Sim sim in lot.GetAllActors())
            {
                if (sim.Inventory == null) continue;

                foreach (IThrowAwayable awayable in sim.Inventory.FindAll<IThrowAwayable>(true))
                {
                    if (awayable == null) continue;

                    if (!awayable.HandToolAllowUserPickupBase()) continue;

                    if (!awayable.ShouldBeThrownAway()) continue;

                    if (awayable.InUse) continue;

                    if ((awayable is Newspaper) && !(awayable as Newspaper).IsOld) continue;

                    if (awayable is TrashPileOpportunity) continue;

                    if ((awayable is PreparedFood) && !(awayable as PreparedFood).IsSpoiled) continue;

                    awayable.ThrowAwayImmediately();
                }
            }

            return OptionResult.SuccessClose;
        }
    }
}
