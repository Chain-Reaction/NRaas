using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.ChildrenObjects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace.Helpers
{
    public class SimEx
    {
        public static float ScoreMotiveTuning(MotiveTuning tuning, MotiveKey key)
        {
            float num = 0f;
            CASAgeGenderFlags speciesSpecificity = tuning.SpeciesSpecificity;
            if (speciesSpecificity != CASAgeGenderFlags.None)
            {
                CASAgeGenderFlags species = key.mAgeSpecies & CASAgeGenderFlags.SpeciesMask;
                if (species == CASAgeGenderFlags.None)
                {
                    species = CASAgeGenderFlags.Human;
                }

                if (species != speciesSpecificity)
                {
                    return float.MinValue;
                }

                num += Sim.kMotiveTuningScoreForSpecies;
            }

            CASAgeGenderFlags ageSpecificity = tuning.AgeSpecificity;
            if (ageSpecificity != CASAgeGenderFlags.AgeMask)
            {
                if ((key.mAgeSpecies & ageSpecificity) == CASAgeGenderFlags.None)
                {
                    return float.MinValue;
                }

                num += Sim.kMotiveTuningScoreForAge;
            }

            if (tuning.TraitSpecificity != null)
            {
                OccultTypes occult = OccultTypes.None;

                bool flag = true;
                foreach (TraitNames names in tuning.TraitSpecificity)
                {
                    occult = OccultTypeHelper.OccultFromTrait(names);
                    if (occult != key.mOccult)
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    num += Sim.kMotiveTuningScoreForTrait * tuning.TraitSpecificity.Count;
                }
                else
                {
                    return float.MinValue;
                }
            }

            if (tuning.WorldRestrictionType == WorldRestrictionType.Allow)
            {
                if (!tuning.WorldRestrictionWorldTypes.Contains(GameUtils.GetCurrentWorldType()) && !tuning.WorldRestrictionWorldNames.Contains(GameUtils.GetCurrentWorld()))
                {
                    return float.MinValue;
                }
                num += Sim.kMotiveTuningScoreForWorld;
            }

            if (tuning.WorldRestrictionType != WorldRestrictionType.Disallow)
            {
                return num;
            }

            if (tuning.WorldRestrictionWorldTypes.Contains(GameUtils.GetCurrentWorldType()) || tuning.WorldRestrictionWorldNames.Contains(GameUtils.GetCurrentWorld()))
            {
                return float.MinValue;
            }

            return (num + Sim.kMotiveTuningScoreForWorld);
        }
    }
}
