using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.SleepFreedom
{
    public static class BedController
    {
        public static bool CanShareBed(BedMultiPart bed, Sim newSim, CommodityKind use, out Sim incompatibleSim)
        {
            incompatibleSim = null;

            try
            {
                foreach (BedData data in bed.PartComp.PartDataList.Values)
                {
                    if (!BedController.CanShareBed(newSim, data.ContainedSim, use, bed.IsTent))
                    {
                        incompatibleSim = data.ContainedSim;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(bed, e);
                return false;
            }
       }

        public static bool CanShareBed(Sim newSim, Sim simUsingBed, CommodityKind use, bool isTent)
        {
            try
            {
                if ((simUsingBed != null) && (simUsingBed != newSim))
                {
                    Relationship relationship = newSim.GetRelationship(simUsingBed, false);
                    if (relationship == null) return false;

                    if ((relationship.LTR == null) || (relationship.LTR.SimWasBetrayed(newSim)) || (relationship.LTR.SimWasBetrayed(simUsingBed)))
                    {
                        return false;
                    }

                    if (simUsingBed.InteractionQueue != null)
                    {
                        WooHoo runningInteraction = simUsingBed.InteractionQueue.RunningInteraction as WooHoo;
                        if ((runningInteraction != null) && (newSim != runningInteraction.Target))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Common.Exception(newSim, e);
                return false;
            }
        }
    }
}
