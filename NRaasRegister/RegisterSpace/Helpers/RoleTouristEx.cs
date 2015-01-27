using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.RegisterSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RegisterSpace.Helpers
{
    public class RoleTouristEx
    {
        public static void SimulateRole(Role ths, float minPassed)
        {
            if (!Register.Settings.mAllowTourists)
            {
                ths.EndRole();
                return;
            }

            if (ths.IsActive)
            {
                List<Lot> baseCamps = new List<Lot>();

                foreach (Lot lot in LotManager.AllLots)
                {
                    if ((lot.IsBaseCampLotType) || (lot.CommercialLotSubType == CommercialLotSubType.kEP10_Resort))
                    {
                        baseCamps.Add(lot);
                    }
                }

                // Drop the EndRole alarm
                foreach (AlarmHandle handle in ths.mAlarmHandles)
                {
                    AlarmManager.Global.RemoveAlarm(handle);
                }

                ths.mAlarmHandles.Clear();

                if (baseCamps.Count > 0)
                {
                    Sim createdSim = ths.mSim != null ? ths.mSim.CreatedSim : null;
                    if (createdSim != null)
                    {
                        if (ths.mMinInLot >= RoleExplorer.kMinPassToAllowSwitchingLots)
                        {
                            if (SimClock.IsTimeBetweenTimes((float)SimClock.Hours24, RoleExplorer.kTimeToGoBackToBaseCamp, RoleExplorer.kTimeToReleaseFromBaseCamp) && (!createdSim.LotCurrent.IsBaseCampLotType))
                            {
                                if (RandomUtil.RandomChance(Register.Settings.mTouristChanceOfLeaving))
                                {
                                    RestoreFutureTrait(ths.mSim);

                                    ths.EndRole();
                                    return;
                                }
                            }

                            if (RandomUtil.CoinFlip())
                            {
                                if (!createdSim.InteractionQueue.Add(GoToLotThatSatisfiesMyRoleEx.Singleton.CreateInstance(createdSim, createdSim, new InteractionPriority(InteractionPriorityLevel.High), true, true)))
                                {
                                    Lot baseCamp = RandomUtil.GetRandomObjectFromList(baseCamps);

                                    createdSim.InteractionQueue.CancelAllInteractions();
                                    InteractionInstance instance = VisitCommunityLot.Singleton.CreateInstance(baseCamp, createdSim, new InteractionPriority(InteractionPriorityLevel.CriticalNPCBehavior), false, true);
                                    createdSim.InteractionQueue.AddAfterCheckingForDuplicates(instance);
                                    ths.UpdateFulfillingLot(baseCamp.LotId);
                                    return;
                                }
                            }
                        }

                        if (createdSim.LotCurrent != null)
                        {
                            if (ths.mFulfillingLotId == 0x0L)
                            {
                                ths.mFulfillingLotId = createdSim.LotCurrent.LotId;
                                ths.mMinInLot += minPassed;
                            }
                            else if (ths.mFulfillingLotId == createdSim.LotCurrent.LotId)
                            {
                                ths.mMinInLot += minPassed;
                            }
                            else if (!createdSim.IsRouting)
                            {
                                ths.UpdateFulfillingLot(createdSim.LotCurrent.LotId);
                            }
                        }
                    }
                }                
                else
                {
                    // needs tuning check
                    if (SimClock.HoursUntil(16) <= 6)
                    {
                        if (ths.mSim != null && ths.mSim.CreatedSim != null && ths.mSim.CreatedSim.CurrentInteraction == null)
                        {
                            if (ths.mSim.HomeWorld == WorldName.FutureWorld && ths.mSim.CreatedSim.Posture != null && ths.mSim.CreatedSim.Posture.ReactionAllowed())
                            {
                                // this is fun but naturally it breaks because EA
                               // ths.mSim.CreatedSim.PlayReaction(ReactionTypes.StandingAwe, ReactionSpeed.NowOrLater);
                            }
                        }

                        ths.SimulateRole(minPassed);
                    }
                    else
                    {
                        if (ths.mSim != null)
                        {
                            RestoreFutureTrait(ths.mSim);
                        }

                        ths.EndRole();
                        return;
                    }
                }               
            }
        }

        public static void StripFutureTraitFromBots()
        {
            Household tourists = Household.TouristHousehold;
            if (tourists != null)
            {
                foreach (SimDescription sim in tourists.AllSimDescriptions)
                {
                    StripFutureTrait(sim);
                }
            }
        }

        public static void RestoreFutureTrait(SimDescription desc)
        {
            if (desc == null)
            {
                return;
            }

            if (Register.Settings.mFutureSims.Contains(desc.SimDescriptionId) && desc.TraitManager != null && !desc.HasTrait(TraitNames.FutureSim))
            {
                Trait trait = TraitManager.GetTraitFromDictionary(TraitNames.FutureSim);
                if (trait != null)
                {
                    desc.AddTrait(trait);
                }
            }
        }

        public static void StripFutureTrait(SimDescription simDescription)
        {
            if (simDescription == null)
            {
                return;
            }

            // Stops an issue in "GrantFutureObjects" regarding the use of sIsChangingWorlds=true                            
            if (simDescription.TraitManager != null)
            {
                if (simDescription.TraitManager.HasElement(TraitNames.FutureSim))
                {
                    simDescription.TraitManager.RemoveElement(TraitNames.FutureSim);
                    Register.Settings.mFutureSims.Add(simDescription.SimDescriptionId);
                }
            } 
        }

        public static void SpawnInPortal(SimDescription sim)
        {
            if (sim == null || sim.CreatedSim == null)
            {
                return;
            }

            ITimePortal[] portals = Sims3.Gameplay.Queries.GetObjects<ITimePortal>();
            foreach (ITimePortal portal in portals)
            {
                TimePortal usePortal = portal as TimePortal;
                if (usePortal != null && usePortal.HasTimeTravelerBeenSummoned() && usePortal.LotCurrent != null && usePortal.InWorld && !usePortal.InUse)
                {
                    bool wasInactive = false;
                    if (usePortal.State == TimePortal.PortalState.Inactive)
                    {
                        wasInactive = true;
                        usePortal.UpdateState(TimePortal.PortalState.Active);
                    }

                    if (sim.CreatedSim.InteractionQueue != null)
                    {
                        sim.CreatedSim.InteractionQueue.CancelAllInteractions();

                        /*
                        while(sim != null && sim.CreatedSim != null && sim.CreatedSim.CurrentInteraction != null)
                        {
                            Common.Sleep(5);
                        }
                         */
                    }
                    
                    usePortal.PushArriveInteraction(new List<SimDescription>{ sim });                    

                    if (wasInactive)
                    {
                        while (sim != null && sim.CreatedSim != null && sim.CreatedSim.CurrentInteraction is TimePortal.Arrive)
                        {
                            Common.Sleep(10);
                        }
                        usePortal.UpdateState(TimePortal.PortalState.Inactive);
                    }

                    //usePortal.PushDeactivatePortal(usePortal, sim.CreatedSim);

                    if (sim.CreatedSim != null)
                    {
                        List<Lot> randomList = new List<Lot>();
                        foreach (Lot lot in LotManager.AllLots)
                        {
                            if ((((lot != sim.CreatedSim.LotCurrent) && !lot.IsWorldLot) && (lot.IsCommunityLot && !lot.IsNeighborOfPlayer())))
                            {
                                randomList.Add(lot);
                            }
                        }

                        Lot choosen = null;
                        if (randomList.Count != 0)
                        {
                            choosen = RandomUtil.GetRandomObjectFromList<Lot>(randomList);
                        }

                        if (choosen != null)
                        {                            
                            sim.CreatedSim.InteractionQueue.Add(VisitCommunityLot.Singleton.CreateInstance(choosen, sim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.Autonomous), false, true));                            
                        }                        
                    }
                }
                break;
            }
        }       
    }
}