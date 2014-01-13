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
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public class CasteNameOption : GenericOptionBase.StringOption, IReadCasteLevelOption, IWriteCasteLevelOption, ICasteFilterOption
    {
        public CasteNameOption()
            : base("")
        { }

        public override string GetTitlePrefix()
        {
            return "CasteName";
        }

        protected override string Validate(string value)
        {
            string origValue = value;

            int count = 1;

            bool found = true;
            while (found)
            {
                found = false;

                foreach (CasteOptions option in StoryProgression.Main.Options.AllCastes)
                {
                    if (Manager == option) continue;

                    if (option.Name == value)
                    {
                        found = true;

                        value = origValue + " " + count;
                        count++;
                        break;
                    }
                }
            }

            return base.Validate(value);
        }
    }
}

