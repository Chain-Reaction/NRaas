using NRaas.StoryProgressionSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class MetalEx
    {
        public static void SmeltMetal(Metal ths, SimDescription actor)
        {
            ths.mHasBeenSmelt = true;
            ths.UpdateMetalVisual();
            float kSmeltMultiplier = Metal.kSmeltMultiplier;
            if ((actor != null) && actor.TraitManager.HasElement(TraitNames.GathererTrait))
            {
                kSmeltMultiplier += TraitTuning.GathererTraitIngotQualityModifier;
            }
            ths.mStoredCost = (int)(ths.Cost * kSmeltMultiplier);
        }
    }
}

