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
    public class RandomPianoSongScenario : RandomSongScenario<Piano>
    {
        public RandomPianoSongScenario()
        { }
        protected RandomPianoSongScenario(RandomPianoSongScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "RandomPianoSong";
        }
        
        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kPlayedPiano);
        }

        public override Scenario Clone()
        {
            return new RandomPianoSongScenario(this);
        }

        public class EventOption : BooleanEventOptionItem<ManagerSkill, RandomPianoSongScenario>
        {
            public EventOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "RandomizePianoSongs";
            }
        }
    }
}
