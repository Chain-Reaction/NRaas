using NRaas.CommonSpace.Helpers;
using NRaas.TaggerSpace.MapTags;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Roles;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Helpers
{
    public class MapTagHelper
    {
        public static void OnMapTagAdded(IMapTag tag)
        {
            if (Sim.ActiveActor == null || Sim.ActiveActor.MapTagManager == null || !CameraController.IsMapViewModeEnabled())
            {
                return;
            }

            if (tag.TagType == MapTagType.Venue)
            {
                MapTagsModel model = MapTagsModel.Singleton;
                MapTagManager manager = Sim.ActiveActor.MapTagManager;
                MapTag mTag = tag as MapTag;
                if (model != null && manager != null && mTag != null)
                {
                    Lot lot = LotManager.GetLot(mTag.LotId);
                    if (lot != null && ShouldReplace(lot))
                    {                        
                        CustomTagNRaas customTag = new CustomTagNRaas(lot, mTag.Owner);
                        manager.RemoveTag(tag.ObjectGuid);
                        Tagger.sReplaced.Add(lot.LotId);
                        manager.AddTag(customTag);
                    }
                }
            }
        }

        public static void SetupSimTag(Sim sim)
        {
            Sim active = Sims3.Gameplay.Actors.Sim.ActiveActor;
            if (active == null) return;

            MapTagManager mtm = active.MapTagManager;
            if (mtm == null) return;

            try
            {
                if (sim == null) return;

                MapTag tag = mtm.GetTag(sim);                

                if ((sim.Household == Household.ActiveHousehold) ||
                    (sim.SimDescription.AssignedRole is RoleSpecialMerchant) ||
                    (sim.SimDescription.AssignedRole is Proprietor) ||
                    (Tagger.Settings.mEnableSimTags && (!Tagger.Settings.HasSimFilterActive() || Tagger.Settings.DoesSimMatchSimFilters(sim.SimDescription.SimDescriptionId) || Tagger.Settings.mTaggedSims.Contains(sim.SimDescription.SimDescriptionId))))
                {
                    if (((tag is NPCSimMapTag) || (tag is SelectedSimMapTag)) || (tag is FamilySimMapTag))
                    {
                        mtm.RemoveTag(tag);
                    }                   

                    if (!mtm.HasTag(sim))
                    {
                        mtm.AddTag(new TrackedSim(sim, mtm.Actor));                        
                    }
                }
                else if (tag is TrackedSim)
                {
                    mtm.RemoveTag(tag);                    
                }
            }
            catch (Exception exception)
            {
                Common.DebugException(sim, exception);
            }
        }

        public static void SetupLotTag(Lot lot)
        {
            Sim active = Sims3.Gameplay.Actors.Sim.ActiveActor;
            if (active == null) return;

            MapTagManager mtm = active.MapTagManager;
            if (mtm == null) return;

            try
            {
                MapTag tag = mtm.GetTag(lot);

                if ((!Tagger.Settings.HasLotFilterActive() || Tagger.Settings.DoesHouseholdMatchLotFilters(lot.Household.AllSimDescriptions)) && Tagger.Settings.mEnableLotTags)
                {
                    if ((tag != null) && (!(tag is TrackedLot)) && (!(tag is HomeLotMapTag)))
                    {
                        mtm.RemoveTag(tag);
                    }
                    if (!mtm.HasTag(lot))
                    {
                        mtm.AddTag(new TrackedLot(lot, mtm.Actor));                        
                    }
                }
                else if (tag is TrackedLot)
                {
                    mtm.RemoveTag(tag);                    
                }
            }
            catch (Exception exception)
            {
                Common.DebugException(lot, exception);
            }
        }

        public static bool ShouldReplace(Lot lot)
        {
            if (lot != null && !Tagger.sReplaced.Contains(lot.LotId))
            {
                return Tagger.staticData.ContainsKey((uint)lot.CommercialLotSubType);
            }

            return false;
        }
    }
}
