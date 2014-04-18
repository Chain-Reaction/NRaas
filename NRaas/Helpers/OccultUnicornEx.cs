using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class OccultUnicornEx
    {
        static Dictionary<ulong, bool> sUnicornParts = new Dictionary<ulong, bool>();
        static List<CASParts.Wrapper> sParts = null;

        static OccultUnicornEx()
        {
            sUnicornParts.Add(0x4740A099D50755C0, true); // Adult/Elder Male/Female horn
            sUnicornParts.Add(0xE18CF0CE08B508A8, true); // Adult/Elder Male/Female beard
            sUnicornParts.Add(0x568ED1E1E98F05CE, true); // Child Male/Female horn
            sUnicornParts.Add(0xF000A911F12FE0FA, true); // Child Male/Female beard
        }

        protected static bool PartMatches(CASParts.Wrapper part)
        {
            if (part.mPart.Key.GroupId != 0x48000000) return false;

            return sUnicornParts.ContainsKey(part.mPart.Key.InstanceId);
        }

        public static void OnAddition(OccultUnicorn ths, SimDescription simDes, bool alterOutfit)
        {
            if (alterOutfit)
            {
                if (simDes.HorseManager == null)
                {
                    return;
                }

                Color[] maneColors = simDes.HorseManager.ActiveManeHairColors;
                simDes.HorseManager.ActiveUnicornBeardHairColors = maneColors;

                if (sParts == null)
                {
                    sParts = CASParts.GetParts(PartMatches);
                }

                List<CASParts.PartPreset> parts = new List<CASParts.PartPreset>();
                foreach (CASParts.Wrapper part in sParts)
                {
                    if (!part.ValidFor(simDes)) continue;

                    CASParts.PartPreset preset = part.GetRandomPreset();
                    if (preset == null) continue;

                    parts.Add(preset);
                }

                if (parts.Count > 0)
                {
                    GeneticsPet.SpeciesSpecificData speciesData = new GeneticsPet.SpeciesSpecificData();
                    speciesData.UnicornBeardHairColors = simDes.HorseManager.ActiveUnicornBeardHairColors;

                    foreach (OutfitCategories category in simDes.ListOfCategories)
                    {
                        switch (category)
                        {
                            case OutfitCategories.All:
                            case OutfitCategories.CategoryMask:
                            case OutfitCategories.None:
                            case OutfitCategories.PrimaryCategories:
                            case OutfitCategories.PrimaryHorseCategories:
                            case OutfitCategories.Special:
                                continue;
                            default:
                                for (int i = 0x0; i < simDes.GetOutfitCount(category); i++)
                                {
                                    using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(simDes, new CASParts.Key(category, i)))
                                    {
                                        foreach (CASParts.PartPreset part in parts)
                                        {
                                            builder.Builder.RemoveParts(new BodyTypes[] { part.mPart.BodyType });
                                            builder.ApplyPartPreset(part);
                                            if (part.mPart.BodyType == BodyTypes.PetBeard)
                                            {
                                                OutfitUtils.AdjustPresetForHorseHairColor(builder.Builder, part.mPart, speciesData);
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }

                    if (simDes.CreatedSim != null)
                    {
                        simDes.CreatedSim.UpdateOutfitInfo();
                        simDes.CreatedSim.RefreshCurrentOutfit(false);
                    }
                }
            }
        }

        public static void OnRemoval(SimDescription sim)
        {
            foreach (OutfitCategories category in sim.ListOfCategories)
            {
                switch (category)
                {
                    case OutfitCategories.All:
                    case OutfitCategories.CategoryMask:
                    case OutfitCategories.None:
                    case OutfitCategories.PrimaryCategories:
                    case OutfitCategories.PrimaryHorseCategories:
                    case OutfitCategories.Special:
                        continue;
                    default:
                        for (int i = 0x0; i < sim.GetOutfitCount(category); i++)
                        {
                            using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sim, new CASParts.Key(category, i)))
                            {
                                builder.Builder.RemoveParts(new BodyTypes[] { BodyTypes.PetBeard });
                                builder.Builder.RemoveParts(new BodyTypes[] { BodyTypes.PetHorn });   
                            }
                        }
                        break;
                }
            }

            if (sim.CreatedSim != null)
            {
                sim.CreatedSim.UpdateOutfitInfo();
                sim.CreatedSim.RefreshCurrentOutfit(false);
            }
        }
    }
}
  
