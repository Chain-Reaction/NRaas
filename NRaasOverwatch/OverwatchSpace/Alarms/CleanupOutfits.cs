using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Loadup;
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
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public class CleanupOutfits : AlarmOption
    {
        public override string GetTitlePrefix()
        {
            return "CleanupOutfits";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mCleanupSinged;
            }
            set
            {
                NRaas.Overwatch.Settings.mCleanupSinged = value;
            }
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log("Cleanup Singed");

                int count = 0;

                foreach (SimDescription sim in Household.EverySimDescription())
                {
                    int protection = 0;
                    while ((sim.GetOutfitCount(OutfitCategories.Singed) > 1) && (protection < 100))
                    {
                        sim.RemoveOutfit(OutfitCategories.Singed, sim.GetOutfitCount(OutfitCategories.Singed)-1, true);

                        protection++;
                        count++;

                        Overwatch.Log("Singed Removed " + sim.FullName);
                    }

                    protection = 0;
                    while ((sim.GetOutfitCount(OutfitCategories.Career) > 3) && (protection < 100))
                    {
                        // this handles the out of control lab coat's from the generation table
                        sim.RemoveOutfit(OutfitCategories.Career, sim.GetOutfitCount(OutfitCategories.Career) - 1, true);

                        protection++;
                        count++;

                        Overwatch.Log("Career Removed " + sim.FullName);
                    }

                    if (sim.IsBonehilda)
                    {
                        protection = 0;
                        while ((sim.GetOutfitCount(OutfitCategories.Everyday) > 1) && (protection < 100))
                        {
                            sim.RemoveOutfit(OutfitCategories.Everyday, sim.GetOutfitCount(OutfitCategories.Everyday) - 1, true);

                            protection++;
                            count++;

                            Overwatch.Log("Bonehilda Removed " + sim.FullName);
                        }
                    }

                    // fix corrupt generations outfits
                    ArrayList outfits = sim.GetOutfits(OutfitCategories.ChildImagination);
                    if (outfits != null)
                    {
                        int index = 0;
                        while (index < outfits.Count)
                        {
                            SimOutfit simOutfit = outfits[index] as SimOutfit;
                            if (simOutfit == null)
                            {
                                outfits.RemoveAt(index);
                            }
                            else if (!simOutfit.IsValid)
                            {
                                outfits.RemoveAt(index);
                            }
                            else
                            {
                                index++;
                            }
                        }                     
                    }

                    // fix corrupt diving outfits
                    ArrayList specialOutfits = sim.GetOutfits(OutfitCategories.Special);
                    if (specialOutfits != null)
                    {
                        int index = 0;
                        while (index < specialOutfits.Count)
                        {
                            SimOutfit simOutfit = specialOutfits[index] as SimOutfit;
                            if (simOutfit == null)
                            {
                                sim.RemoveSpecialOutfitAtIndex(index);
                            }
                            else if (!simOutfit.IsValid)
                            {
                                sim.RemoveSpecialOutfitAtIndex(index);
                            }
                            else
                            {
                                index++;
                            }
                        }
                    }

                    CASParts.CheckIndex(sim, Overwatch.Log);
                }

                if ((prompt) && (count > 0))
                {
                    Overwatch.AlarmNotify(Common.Localize("CleanupOutfits:Complete", false, new object[] { count }));
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }
    }
}
