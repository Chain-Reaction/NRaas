using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class BassGuitarWatch : MusicalInstrumentWatchBase<BassGuitar>
    {
        static InteractionDefinition sOldSingleton;

        public override void OnPreLoad()
        {
            Tunings.Inject<BassGuitar, BassGuitar.Watch.Definition, Definition>(false);

            sOldSingleton = BassGuitar.Watch.Singleton;
            BassGuitar.Watch.Singleton = new Definition();
        }

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<BassGuitar, BassGuitar.Watch.Definition>(BassGuitar.Watch.Singleton);
        }

        public override MusicalInstrument.WatchBase<BassGuitar>.WatchTuning Tuning
        {
            get { return BassGuitar.Watch.kWatchTuning; }
        }

        protected class Definition : MusicalInstrumentWatchBase<BassGuitar>.WatchDefinition<BassGuitarWatch>
        {
            public override string GetInteractionName(Sim actor, MusicalInstrument target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
