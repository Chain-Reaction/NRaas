using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Sims.Status;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Tags
{
    public class AddTag : SimFromList, ITagsOption
    {
        public override string GetTitlePrefix()
        {
            return "AddTag";
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

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            MapTagManager manager = MapTagManager.ActiveMapTagManager;
            if (manager == null) return false;

            return base.Allow(parameters);
        }

        public override IEnumerable<SimSelection.ICriteria> AlterCriteria(IEnumerable<SimSelection.ICriteria> allCriteria, bool manual, bool canceled)
        {
            MasterController.Settings.mLastTagFilter = new SavedFilter("LastTag", allCriteria);

            return base.AlterCriteria(allCriteria, manual, canceled);
        }

        public static void Perform(MapTagManager manager, Sim me)
        {
            manager.RemoveTag(me);

            manager.AddTag(new TagItem(me, manager.Actor));
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.CreatedSim == null) return false;

            MapTagManager manager = MapTagManager.ActiveMapTagManager;
            if (manager == null) return false;

            TagItem tag = manager.GetTag(me.CreatedSim) as TagItem;
            return (tag == null);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            foreach (Sim sim in CommonSpace.Helpers.Households.AllSims(Household.ActiveHousehold))
            {
                MapTagManager manager = sim.MapTagManager;
                if (manager == null) continue;

                Perform(manager, me.CreatedSim);
            }

            return true;
        }

        public class TagItem : MapTag
        {
            public TagItem(Sim sim, Sim owner)
                : base(sim, owner)
            { }

            public override MapTagType TagType
            {
                get
                {
                    if (MasterController.Settings.mSubtleMapTags)
                    {
                        return MapTagType.NPCSim;
                    }
                    else
                    {
                        return MapTagType.PrivateEyeCase;
                    }
                }
            }

            public override MapTagFilterType FilterType
            {
                get
                {
                    return ~MapTagFilterType.None;
                }
            }

            public override string HoverText
            {
                get
                {
                    try
                    {
                        Sim target = Target as Sim;

                        IStatusOption option = null;

                        switch (MasterController.Settings.mTagInfo)
                        {
                            case PersistedSettings.TagInfo.Personal:
                                option = new PersonalStatus();
                                break;
                            case PersistedSettings.TagInfo.Career:
                                option = new CareerStatus();
                                break;
                            case PersistedSettings.TagInfo.Relationship:
                                option = new RelationshipStatus();
                                break;
                            case PersistedSettings.TagInfo.Household:
                                option = new HouseholdStatus();
                                break;
                        }

                        if (option != null)
                        {
                            return option.GetDetails(target.SimDescription).Trim();
                        }
                        else
                        {
                            return PersonalStatus.GetHeader(target.SimDescription).Trim();
                        }
                    }
                    catch(Exception e)
                    {
                        Common.Exception(Target, e);
                        return null;
                    }
                }
            }
        }
    }
}
