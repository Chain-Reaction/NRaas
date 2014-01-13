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
using Sims3.Gameplay.Objects.Appliances;
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

                if (baseCamps.Count > 0)
                {
                    // Drop the EndRole alarm
                    foreach (AlarmHandle handle in ths.mAlarmHandles)
                    {
                        AlarmManager.Global.RemoveAlarm(handle);
                    }

                    ths.mAlarmHandles.Clear();

                    Sim createdSim = ths.mSim.CreatedSim;
                    if (createdSim != null)
                    {
                        if (ths.mMinInLot >= RoleExplorer.kMinPassToAllowSwitchingLots)
                        {
                            if (SimClock.IsTimeBetweenTimes((float)SimClock.Hours24, RoleExplorer.kTimeToGoBackToBaseCamp, RoleExplorer.kTimeToReleaseFromBaseCamp) && (!createdSim.LotCurrent.IsBaseCampLotType))
                            {
                                if (RandomUtil.RandomChance(Register.Settings.mTouristChanceOfLeaving))
                                {
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
                    ths.SimulateRole(minPassed);
                }
            }
        }
    }
}