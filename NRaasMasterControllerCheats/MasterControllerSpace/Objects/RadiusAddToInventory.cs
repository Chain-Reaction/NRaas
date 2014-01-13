using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Helpers;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Town;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Object
{
    public class RadiusAddToInventory : OptionItem, IObjectOption
    {
        public override string GetTitlePrefix()
        {
            return "RadiusAddToInventory";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!MasterController.Settings.mMenuVisibleRadiusAddToInventory) return false;

            Terrain terrain = parameters.mTarget as Terrain;
            if (terrain == null) return false;

            // Normally called from the base class
            Reset();
 	        return true;
        }

        protected override CommonSpace.Options.OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            return new VectorProcessor(GetTitlePrefix(), parameters.mHit.mPoint).Perform(ObjectAddToInventory.OnAddToInventory);
        }
    }
}
