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
    public class GemEx
    {
        public static void CutGem(Gem ths, RockGemMetalBase.CutData data, SimDescription actor)
        {
            ths.mCutName = data.CutName;
            ths.mLocalizedCutName = data.Name;
            ths.UpdateGemVisual();
            float multiplier = data.Multiplier;
            if ((actor != null) && actor.TraitManager.HasElement(TraitNames.GathererTrait))
            {
                multiplier += TraitTuning.GathererTraitGemCutQualityModifier;
            }

            ths.mStoredCost = (int)(ths.Cost * multiplier);
        }
    }
}

