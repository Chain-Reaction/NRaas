using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public abstract class AgeBaseOption : AgeGenderSpeciesOption
    {
        protected static CASAgeGenderFlags[] sAges = new CASAgeGenderFlags[] { CASAgeGenderFlags.Baby, CASAgeGenderFlags.Toddler, CASAgeGenderFlags.Child, CASAgeGenderFlags.Teen, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Adult, CASAgeGenderFlags.Elder };

        public AgeBaseOption(CASAgeGenderFlags[] defValue)
            : base(defValue)
        { }

        protected override bool Allow(CASAgeGenderFlags option)
        {
            if ((option & CASAgeGenderFlags.AgeMask) == CASAgeGenderFlags.None) return false;

            if (option == CASAgeGenderFlags.AgeMask) return false;

            return true;
        }

        protected override string GetLocalizationUIKey()
        {
            return "AgingByStage";
        }

        protected override string GetLocalizedValue(CASAgeGenderFlags value, ref ThumbnailKey icon)
        {
            return Common.LocalizeEAString("UI/Feedback/CAS:" + value);
        }
    }
}

