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
using Sims3.Gameplay.Skills;
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
    public abstract class RandomSongScenario<TInstrument> : SimEventScenario<Event>
        where TInstrument : BandInstrument
    {
        public RandomSongScenario()
        { }
        protected RandomSongScenario(RandomSongScenario<TInstrument> scenario)
            : base (scenario)
        { }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            Sim createdSim = sim.CreatedSim;
            if (createdSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            
            InteractionQueue queue = createdSim.InteractionQueue;
            if (queue == null)
            {
                IncStat("No Queue");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Sim sim = Sim.CreatedSim;

            BandInstrument.PlayBandInstrument<TInstrument> interaction = sim.InteractionQueue.GetCurrentInteraction() as BandInstrument.PlayBandInstrument<TInstrument>;
            if (interaction != null)
            {
                int elapsedCalendarDays = SimClock.ElapsedCalendarDays();

                SimData data = GetData<SimData>(Sim);

                DateAndTime lastChange = data.mLast;
                lastChange.Ticks += SimClock.ConvertToTicks(2f, TimeUnit.Hours);

                if (lastChange > SimClock.CurrentTime())
                {
                    data.mLast = SimClock.CurrentTime();

                    interaction.TriggerAudio(false);

                    if (interaction.mInstrumentSound == null)
                    {
                        Sims3.Gameplay.Skills.Guitar.Composition PlayingComposition = null;

                        try
                        {
                            PlayingComposition = (interaction.mSkill as BandSkill).GetRandomComposition(false);
                        }
                        catch (Exception)
                        {
                            // If the known compositions is zero, GetRandom will except
                        }

                        if (PlayingComposition != null)
                        {
                            interaction.mInstrumentSound = new ObjectSound(interaction.Target.ObjectId, PlayingComposition.AudioClip);
                            interaction.mInstrumentSound.StartLoop();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        protected class SimData : ElementalSimData
        {
            public DateAndTime mLast = new DateAndTime();

            public SimData()
            { }

            public override string ToString()
            {
                return Common.StringBuilder.XML("Last", mLast);
            }
        }

        public class Option : BooleanManagerOptionItem<ManagerSkill>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "RandomizeInstrumentSongs";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
