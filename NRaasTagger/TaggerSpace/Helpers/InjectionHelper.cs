using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Helpers
{
    public class InjectionHelper
    {
        public static void InjectMapTag(string name)
        {
            MapTagController.MapTagData data = default(MapTagController.MapTagData);
            if (ParserFunctions.TryParseEnum<MapTagType>(name, out data.TagType, MapTagType.Count, true))
            {
                data.AllowedBoundsOutside = 1f;
                data.OverrideAllowedBounds = false;
                data.MinCamDistance = 100f;
                data.MaxCamDistance = 100000f;                
                data.CamZoomValue = 50f;
                data.CamPitchValue = -60f;
                data.ShadeColor = uint.Parse("FFFFFF", System.Globalization.NumberStyles.HexNumber); // default, overwrote in the class
                // no icon, overwrote in class
                data.FilterType = MapTagFilterType.PublicSpacesAndActivities; // types tacked on in class

                if (!MapTagController.sSpreadsheetData.ContainsKey(data.TagType))
                {
                    MapTagController.sSpreadsheetData.Add(data.TagType, data);
                }
            }
        }        

        public static void InjectCommunityType(string varname)
        {
            CommercialLotSubType type;
            if (ParserFunctions.TryParseEnum<CommercialLotSubType>(varname, out type, CommercialLotSubType.kCommercialUndefined))
            {
                XmlDbTable table;
                XmlDbData data = XmlDbData.ReadData("Venues");
                if (data.Tables.TryGetValue("CommuntiyTypes", out table))
                {
                    XmlDbRow row = table.Rows[10];
                    Lot.CommercialSubTypeData rowData = new Lot.CommercialSubTypeData(row);
                    rowData.WorldAllowed = null;
                    rowData.WorldTypeAllowed = null;
                    rowData.IsVisible = true;
                    rowData.LocalizationStringKey = "Gameplay/Excel/Venues/CommunityTypes:" + varname;
                    rowData.HouseboatValid = false;
                    rowData.AutoPlaceable = false;
                    rowData.CommercialLotSubType = type;
                    Lot.sCommnunityTypeData.Add(rowData);
                }
            }
        }

        // This isn't used currently because EA hard coded the function that matches this type to the lot type...
        // but it's here in case I have a lightbulb moment
        public static void InjectMetaAutonomyVenueType(string str, string icon, string maptag)
        {
            MetaAutonomyVenueType type;
            MetaAutonomyTuning sCurrentTuning;
            if (ParserFunctions.TryParseEnum<MetaAutonomyVenueType>(str, out type, MetaAutonomyVenueType.Restaurant))
            {                
                List<MetaAutonomyTuning> list;
                sCurrentTuning = new MetaAutonomyTuning(type);
                if (!MetaAutonomyManager.sTunings.TryGetValue(type, out list))
                {
                    MetaAutonomyManager.sTunings[type] = list = new List<MetaAutonomyTuning>();
                }
                list.Add(sCurrentTuning);

                // w_simoleon           
                if (!string.IsNullOrEmpty(icon))
                {
                    sCurrentTuning.IconKey = ResourceKey.CreatePNGKey(icon, ResourceUtils.ProductVersionToGroupId(ProductVersion.Undefined));
                    if (!World.ResourceExists(sCurrentTuning.IconKey))
                    {
                        sCurrentTuning.IconKey = ResourceKey.CreatePNGKey(icon, 0);
                    }
                }

                // hud_mt_i_consignment            
                if (!string.IsNullOrEmpty(maptag))
                {
                    sCurrentTuning.MaptagIconKey = ResourceKey.CreatePNGKey(maptag, 0);
                }

                /*
                string str4 = row.GetString("OneShotMotives");
                if (!string.IsNullOrEmpty(str4))
                {
                    ParserFunctions.TryParseCommaSeparatedList<CommodityKind>(str4, out sCurrentTuning.OneShotMotives, CommodityKind.None);
                }
                string str5 = row.GetString("ContinuousMotive");
                if (!string.IsNullOrEmpty(str5))
                {
                    CommodityKind none = CommodityKind.None;
                    if (ParserFunctions.TryParseEnum<CommodityKind>(str5, out none, CommodityKind.None))
                    {
                        sCurrentTuning.ContinuousMotive = none;
                    }
                }
                 */

                sCurrentTuning.Intensity = 0.75f;
                sCurrentTuning.Intensity_Downtown = 0.75f;

                /*
                if (!string.IsNullOrEmpty(row.GetString("HourOpen")))
                {
                    sCurrentTuning.HourOpen = row.GetFloat("HourOpen");
                }
                if (!string.IsNullOrEmpty(row.GetString("HourClose")))
                {
                    sCurrentTuning.HourClose = row.GetFloat("HourClose");
                }
                 */

                sCurrentTuning.MinLength = 0.1f;
                sCurrentTuning.MaxLength = 0.3f;

                sCurrentTuning.ColdSpotPriority = 4;
                sCurrentTuning.HowLongSimsWaitOutsideBeforeEntering = 0;

                sCurrentTuning.mMiscellaneousFlags |= MetaAutonomyTuning.TuningAttribute.ChildCanVisit;

                /*
                if (!string.IsNullOrEmpty(row.GetString("SpecialMotiveLock")) && row.GetBool("SpecialMotiveLock"))
                {
                    sCurrentTuning.mMiscellaneousFlags |= MetaAutonomyTuning.TuningAttribute.HasSpecialMotiveLockForNpcs;
                }
                 */

                sCurrentTuning.mAllowedPetSpecies = CASAGSAvailabilityFlags.None;

                sCurrentTuning.mBookGigModifier = 0f;
                sCurrentTuning.mPerformanceMeterMax = 1f;
            }
        }
    }
}
