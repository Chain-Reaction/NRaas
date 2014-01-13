using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Utilities;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences.Controllers
{
    public class DereferenceMapTag : DereferenceController<MapTag>
    {
        Dictionary<IMapTag, bool> mMapTags = new Dictionary<IMapTag, bool>();

        public DereferenceMapTag()
        {
            MapTagManager mapTagManager = MapTagManager.ActiveMapTagManager;
            if (mapTagManager != null)
            {
                foreach (IMapTag tag in mapTagManager.GetCurrentMapTags())
                {
                    mMapTags.Add(tag, true);
                }
            }
        }

        protected override void PreProcess(MapTag obj, object parent, FieldInfo field)
        { }

        protected override void Perform(MapTag obj, object parent, FieldInfo field)
        {
            if (!mMapTags.ContainsKey(obj))
            {
                obj.OnRemovedFromManager();

                ErrorTrap.DebugLogCorrection("Bogus Map Tag Dropped: " + obj.GetType());
            }
        }
    }
}
