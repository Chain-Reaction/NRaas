using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace.Metrics;
using NRaas.Gameplay.Tones;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;

namespace NRaas.Gameplay.OmniSpace.Tones
{
    [Persistable]
    public class PerformTone : CareerToneEx
    {
        // Methods
        private static string LocalizeString(SimDescription sim, string name, params object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, "PerformTone:" + name, "Gameplay/Careers/Music/PerformTone:" + name, parameters);
        }

        public override void OnTimePassed(InteractionInstance interactionInstance, float totalTime, float deltaTime)
        {
            base.OnTimePassed(interactionInstance, totalTime, deltaTime);

            Music career = base.Career as Music;
            if ((career != null) && (career.CurLevel != null))
            {
                career.mPreparationForConcert += (deltaTime * Music.PerformTone.kPrepareForConcertPerSimHour) / 60f;
            }
            if (career.mPreparationForConcert >= 100f)
            {
                career.mPreparationForConcert = 0f;
                career.ConcertsPerformed++;
                career.ShowOccupationTNS(LocalizeString(interactionInstance.InstanceActor.SimDescription, "ConcertPerformed", new object[] { career.OwnerDescription }));
            }
            this.OnProgressChanged(this);
        }

        public override bool ShouldAddTone(Career career)
        {
            if (!base.ShouldAddTone(career)) return false;

            foreach (PerfMetric metric in career.CurLevel.Metrics)
            {
                if (metric is MetricConcertsPerformed)
                {
                    return true;
                }
            }
            return false;
        }

        // Properties
        public override bool IsProgressTone
        {
            get
            {
                return true;
            }
        }

        public override float Progress
        {
            get
            {
                Music career = OmniCareer.Career<Music> (Career);
                if ((career != null) && (career.CurLevel != null))
                {
                    return career.mPreparationForConcert;
                }
                return 0f;
            }
        }
    }
}
