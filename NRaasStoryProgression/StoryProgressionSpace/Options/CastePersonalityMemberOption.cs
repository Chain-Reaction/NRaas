using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public class CastePersonalityMemberOption : GenericOptionBase.ListedOptionItem<SimPersonality, string>, IReadCasteLevelOption, IWriteCasteLevelOption, ISimCasteOption, ICasteFilterOption
    {
        public CastePersonalityMemberOption()
            : base(new List<string>(), new List<string>())
        { }

        public override string GetTitlePrefix()
        {
            return "CastePersonalityMember";
        }

        protected override string ValuePrefix
        {
            get { return "Boolean"; }
        }

        protected override string ConvertFromString(string value)
        {
            return value;
        }

        protected override string ConvertToValue(SimPersonality value, out bool valid)
        {
            if (value == null)
            {
                valid = false;
                return null;
            }
            else
            {
                valid = true;
                return value.UnlocalizedName;
            }
        }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }

        protected override string GetLocalizedValue(SimPersonality value, ref ThumbnailKey icon)
        {
            return value.GetLocalizedName();
        }

        protected override IEnumerable<SimPersonality> GetOptions()
        {
            return StoryProgression.Main.Personalities.AllPersonalities;
        }

        public override bool ShouldDisplay()
        {
            if (!GetValue<CasteAutoOption, bool>()) return false;

            return base.ShouldDisplay();
        }
    }
}

