using NRaas.CommonSpace.Booters;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Symptoms
{
    public class IdleAnimationSymptom : SymptomBooter.Data
    {
        MoodFlavor mMood;

        IdleAnimationPriority mPriority;

        public IdleAnimationSymptom(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Mood", Guid))
            {
                if (!row.TryGetEnum<MoodFlavor>("Mood", out mMood, MoodFlavor.Uncomfortable))
                {
                    BooterLogger.AddError(" Unknown Mood: " + row.GetString("Mood"));
                }
            }

            if (BooterLogger.Exists(row, "Priority", Guid))
            {
                if (!row.TryGetEnum<IdleAnimationPriority>("Priority", out mPriority, IdleAnimationPriority.NonDistress))
                {
                    BooterLogger.AddError(" Unknown Priority: " + row.GetString("Priority"));
                }
            }
        }

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            if (sim.IdleManager == null) return;

            sim.IdleManager.ScheduleIdle(mPriority, mMood);
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Mood: " + mMood;
            result += Common.NewLine + " Priority: " + mPriority;

            return result;
        }
    }
}
