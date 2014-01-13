using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
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
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public class CasteScoringOption : GenericOptionBase.ChoiceOptionItem<string>, IReadCasteLevelOption, IWriteCasteLevelOption, ISimCasteOption, ICasteFilterOption
    {
        public CasteScoringOption()
            : base(null, null)
        { }

        public override string GetTitlePrefix()
        {
            return "CasteScoring";
        }

        protected override string GetLocalizationValueKey()
        {
            return null;
        }

        protected override string GetLocalizedValue(string value, ref bool matches, ref ThumbnailKey icon)
        {
            matches = (Value == value);

            if (string.IsNullOrEmpty(value))
            {
                return Common.Localize("None:MenuName");
            }
            else
            {
                return value;
            }
        }

        protected override IEnumerable<string> GetOptions()
        {
            List<string> results = new List<string>();

            results.Add("");

            foreach (KeyValuePair<string, IListedScoringMethod> pair in ScoringLookup.AllScoring)
            {
                if (pair.Value is IScoringMethod<SimDescription, SimScoringParameters>)
                {
                    results.Add(pair.Key);
                }
            }

            return results;
        }

        public override object PersistValue
        {
            set
            {
                SetValue(value as string);
            }
        }

        public override bool ShouldDisplay()
        {
            if (!GetValue<CasteAutoOption, bool>()) return false;

            return base.ShouldDisplay();
        }
    }
}

