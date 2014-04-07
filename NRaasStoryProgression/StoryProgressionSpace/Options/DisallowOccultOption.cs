using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public class DisallowOccultOption : OccultTypeOption, IReadSimLevelOption, IWriteSimLevelOption, IHouseLevelSimOption
    {
        public DisallowOccultOption()
            : base(new OccultTypes[0])
        { }

        public override string GetTitlePrefix()
        {
            return "DisallowOccult";
        }

        protected override string ValuePrefix
        {
            get { return "Disallowed"; }
        }

        public override bool ShouldDisplay()
        {
            if (!StoryProgression.Main.Personalities.HasPersonalities) return false;

            return base.ShouldDisplay();
        }        
    }
}


