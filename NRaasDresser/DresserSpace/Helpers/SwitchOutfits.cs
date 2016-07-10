using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.DresserSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.DresserSpace.Helpers
{
    public class SwitchOutfits : Common.IWorldLoadFinished, Common.IWorldQuit
    {
        static Dictionary<ulong, DateAndTime> sLastPerformed = new Dictionary<ulong, DateAndTime>();

        static Dictionary<ulong, InteractionInstance> sLastInteractions = new Dictionary<ulong, InteractionInstance>();

        public static OutfitCategories[] sCategories = new OutfitCategories[] { OutfitCategories.Athletic, OutfitCategories.Everyday, OutfitCategories.Formalwear, OutfitCategories.MartialArts, OutfitCategories.Sleepwear, OutfitCategories.Swimwear, OutfitCategories.Outerwear };

        public void OnWorldLoadFinished()
        {
            sLastPerformed.Clear();

            sLastInteractions.Clear();

            new Common.DelayedEventListener(EventTypeId.kUsedObect, OnUsedObject);
            new Common.DelayedEventListener(EventTypeId.kRoomChanged, OnRoomChanged);
            new Common.DelayedEventListener(EventTypeId.kEventSimMovedFromLot, OnLotChanged);
        }

        public void OnWorldQuit()
        {
            sLastPerformed.Clear();

            sLastInteractions.Clear();
        }

        protected static void OnLotChanged(Event e)
        {
            Sim sim = e.Actor as Sim;
            Lot lot = e.TargetObject as Lot;

            if (sim.IsHuman)
            {
                if (Dresser.Settings.mSwitchFormal)
                {
                    if ((!sim.IsSelectable) || (Dresser.Settings.mTransitionAffectActive))
                    {
                        if ((lot != null) && (lot != sim.LotHome) && (!lot.IsWorldLot) && (sim.CurrentOutfitCategory == OutfitCategories.Formalwear))
                        {
                            if (sim.IsOutside)
                            {
                                CommonSpace.Helpers.SwitchOutfits.SwitchNoSpin(sim, Sim.ClothesChangeReason.GoingOutside);
                            }
                            else
                            {
                                CommonSpace.Helpers.SwitchOutfits.SwitchNoSpin(sim, Sim.ClothesChangeReason.GoingOffLot);
                            }
                        }
                    }
                }

                SimDescription simDesc = sim.SimDescription;
                if ((sim.IsOutside) && (Dresser.Settings.ShouldCheck(simDesc)))
                {
                    int count = SavedOutfit.Cache.CountOutfits(simDesc, false);

                    if (count != Dresser.Settings.GetOutfitCount(simDesc))
                    {
                        CheckOutfitTask.Controller.PerformCheck(simDesc);

                        Dresser.Settings.SetOutfitCount(simDesc, count);
                    }
                }
            }
            else if ((sim.IsHorse) && (SimTypes.IsService(sim.SimDescription)))
            {
                if (Saddle.IsOutfitCategorySaddled(sim.CurrentOutfitCategory))
                {
                    BeingRiddenPosture posture = sim.Posture as BeingRiddenPosture;
                    if (posture == null)
                    {
                        CommonSpace.Helpers.SwitchOutfits.SwitchNoSpin(sim, Sim.ClothesChangeReason.GoingToBathe);
                    }
                }
            }
        }

        public static bool WasWithinTheDay(SimDescription sim)
        {
            ulong simId = sim.SimDescriptionId;

            DateAndTime lastTime;
            if (sLastPerformed.TryGetValue(simId, out lastTime))
            {
                if (SimClock.ElapsedTime(TimeUnit.Hours, lastTime) <= 23)
                {
                    return true;
                }
            }

            return false;
        }

        public static void SetLastSwitch(SimDescription sim)
        {
            ulong simId = sim.SimDescriptionId;

            sLastPerformed[simId] = SimClock.CurrentTime();
        }

        public static void OnUsedObject(Event e)
        {
            if ((e.TargetObject is IShower) || (e.TargetObject is IBathtub))
            {
                Sim actor = e.Actor as Sim;
                if (actor != null)
                {
                    if ((actor.CurrentInteraction is Shower.TakeShower) || (actor.CurrentInteraction is Bathtub.TakeBath))
                    {
                        bool allow = true;

                        InteractionInstance instance;
                        if (sLastInteractions.TryGetValue(actor.SimDescription.SimDescriptionId, out instance))
                        {
                            if (instance == actor.CurrentInteraction)
                            {
                                allow = false;
                            }
                        }

                        if (allow)
                        {
                            sLastInteractions[actor.SimDescription.SimDescriptionId] = actor.CurrentInteraction;

                            PerformRotation(actor, !actor.CurrentInteraction.Autonomous, false);
                        }
                    }
                }
            }
        }

        protected static void OnRoomChanged(Event e)
        {
            Sim sim = e.Actor as Sim;
            if (sim != null)
            {
                if (sim.HasBeenDestroyed) return;

                bool needsChange = false;

                if (sim.IsHuman)
                {
                    if ((!sim.IsSelectable) || (Dresser.Settings.mTransitionAffectActive))
                    {
                        if (sim.IsOutside)
                        {
                            try
                            {
                                if (Dresser.Settings.mSwitchOnOutside)
                                {
                                    switch (sim.CurrentOutfitCategory)
                                    {
                                        case OutfitCategories.Sleepwear:
                                            needsChange = true;
                                            break;
                                        case OutfitCategories.Naked:
                                            if (SkinnyDipClothingPile.FindClothingPile(sim) == null)
                                            {
                                                needsChange = true;
                                            }
                                            break;
                                        case OutfitCategories.Athletic:
                                            switch (sim.CurrentWalkStyle)
                                            {
                                                case Sim.WalkStyle.Jog:
                                                case Sim.WalkStyle.FastJog:
                                                    break;
                                                default:
                                                    needsChange = true;
                                                    break;
                                            }
                                            break;
                                    }
                                }
                            }
                            catch
                            { }
                        }
                        else
                        {
                            switch (sim.CurrentOutfitCategory)
                            {
                                case OutfitCategories.Sleepwear:
                                    if (Dresser.Settings.mSwitchSleepwear)
                                    {
                                        needsChange = true;
                                    }
                                    break;
                                case OutfitCategories.Naked:
                                    if (Dresser.Settings.mSwitchSleepwear)
                                    {
                                        if (SkinnyDipClothingPile.FindClothingPile(sim) == null && !(sim.CurrentInteraction is Streak))
                                        {
                                            needsChange = true;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }

                if (needsChange)
                {
                    if (sim.IsOutside)
                    {
                        CommonSpace.Helpers.SwitchOutfits.SwitchNoSpin(sim, Sim.ClothesChangeReason.GoingOutside);
                    }
                    else
                    {
                        CommonSpace.Helpers.SwitchOutfits.SwitchNoSpin(sim, Sim.ClothesChangeReason.LeavingRoom);
                    }
                }
            }
        }

        public static void PerformRotation(Sim sim, bool force, bool manual)
        {
            try
            {
                if (!sim.IsHuman) return;

                if (!manual)
                {
                    if (SimTypes.IsSkinJob(sim.SimDescription))
                    {
                        if (!Dresser.Settings.mRotateOccult) return;
                    }

                    if (sim.IsSelectable)
                    {
                        if (!Dresser.Settings.mRotationAffectActive) return;
                    }
                    else
                    {
                        if (!Dresser.Settings.mRotationAffectInactive) return;
                    }

                    if (!force)
                    {
                        if (WasWithinTheDay(sim.SimDescription)) return;
                    }
                }

                bool adjustForVacationOutfits = false;

                if (sim.IsSelectable)
                {
                    int vacationOutfitIndex = Dresser.Settings.mVacationOutfitIndex;
                    if (vacationOutfitIndex > 0)
                    {
                        adjustForVacationOutfits = true;
                    }
                }

                SetLastSwitch(sim.SimDescription);

                foreach (OutfitCategories category in sCategories)
                {
                    if (!Dresser.Settings.mAllowRotationOutfitCategories.Contains(category))
                    {
                        continue;
                    }

                    int count = sim.SimDescription.GetOutfitCount(category);
                    if (count < 2) continue;

                    if (!RandomUtil.RandomChance(Dresser.Settings.ChanceOfSwitch)) continue;

                    int startIndex = 0;
                    int endIndex = count - 2;

                    if (adjustForVacationOutfits)
                    {
                        if (GameUtils.IsOnVacation())
                        {
                            startIndex = Dresser.Settings.mVacationOutfitIndex - 1;
                        }
                        else
                        {
                            endIndex = Dresser.Settings.mVacationOutfitIndex - 1;
                        }
                    }

                    if (startIndex > endIndex) continue;

                    int index = RandomUtil.GetInt(startIndex, endIndex) + 1;

                    ArrayList list = sim.SimDescription.GetCurrentOutfits()[category] as ArrayList;

                    object a = list[0];
                    object b = list[index];

                    list[0] = b;
                    list[index] = a;

                    bool current = false;
                    try
                    {
                        if (sim.CurrentOutfitCategory == category)
                        {
                            current = true;
                        }
                    }
                    catch
                    { }

                    if (current)
                    {
                        CommonSpace.Helpers.SwitchOutfits.SwitchNoSpin(sim, new CASParts.Key(category, 0));
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
            }
        }
    }
}


