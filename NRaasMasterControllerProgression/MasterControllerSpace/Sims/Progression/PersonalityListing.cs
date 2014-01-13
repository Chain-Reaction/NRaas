extern alias SP;

using SimPersonality = SP::NRaas.StoryProgressionSpace.Personalities.SimPersonality;

using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Progression
{
    public abstract class PersonalityListing<T> : OptionList<PersonalityBase>, IProgressionOption
        where T : PersonalityBase, new()
    {
        public override List<PersonalityBase> GetOptions()
        {
            List<PersonalityBase> results = new List<PersonalityBase>();

            if (SP::NRaas.StoryProgression.Main != null)
            {
                foreach (SimPersonality personality in SP::NRaas.StoryProgression.Main.Personalities.AllPersonalities)
                {
                    T value = new T();
                    value.Personality = personality;

                    results.Add(value);
                }
            }

            return results;
        }
    }
}
