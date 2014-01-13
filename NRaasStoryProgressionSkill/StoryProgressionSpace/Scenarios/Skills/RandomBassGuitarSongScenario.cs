using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class RandomBassGuitarSongScenario : RandomSongScenario<BassGuitar>
    {
        public RandomBassGuitarSongScenario()
        { }
        protected RandomBassGuitarSongScenario(RandomBassGuitarSongScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "RandomBassGuitarSong";
        }
        
        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kPlayedBassGuitar);
        }

        public override Scenario Clone()
        {
            return new RandomBassGuitarSongScenario(this);
        }

        public class EventOption : BooleanEventOptionItem<ManagerSkill, RandomBassGuitarSongScenario>
        {
            public EventOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "RandomizeBassGuitarSongs";
            }
        }
    }
}
