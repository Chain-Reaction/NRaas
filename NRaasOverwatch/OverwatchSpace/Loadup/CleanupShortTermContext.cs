using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace.Interfaces;
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
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupShortTermContext : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupShortTermContext");

            foreach (SimDescription sim in SimListing.GetResidents(true).Values)
            {
                try
                {
                    foreach (Relationship relation in Relationship.Get(sim))
                    {
                        List<CommodityTypes> removeCommodity = new List<CommodityTypes>();

                        foreach (KeyValuePair<CommodityTypes, Dictionary<SimDescription, float>> commodity in relation.STC.mAsymmetricStcProgress)
                        {
                            List<SimDescription> remove = new List<SimDescription>();

                            foreach (KeyValuePair<SimDescription, float> value in commodity.Value)
                            {
                                if (value.Value == 0)
                                {
                                    remove.Add(value.Key);
                                }
                            }

                            if (remove.Count == commodity.Value.Count)
                            {
                                commodity.Value.Clear();

                                removeCommodity.Add(commodity.Key);

                                Overwatch.Log("Zero Size Commodity Dropped: " + sim.FullName);
                            }
                            else if (remove.Count > 0)
                            {
                                Overwatch.Log(remove.Count + " Zero Value STC Dropped: " + sim.FullName);

                                foreach (SimDescription other in remove)
                                {
                                    commodity.Value.Remove(other);
                                }
                            }
                        }

                        if (removeCommodity.Count == relation.STC.mAsymmetricStcProgress.Count)
                        {
                            relation.STC.mAsymmetricStcProgress.Clear();
                        }
                        else
                        {
                            foreach (CommodityTypes commodity in removeCommodity)
                            {
                                relation.STC.mAsymmetricStcProgress.Remove(commodity);
                            }
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
}
