using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Symptoms
{
    public class DeathSymptom : SymptomBooter.Data
    {
        SimDescription.DeathType mType;

        bool mAllowActive;

        public DeathSymptom(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Type", Guid))
            {
                if (!row.TryGetEnum<SimDescription.DeathType>("Type", out mType, SimDescription.DeathType.None))
                {
                    BooterLogger.AddError(" Unknown Type: " + row.GetString("Type"));
                }
            }

            if (BooterLogger.Exists(row, "AllowActive", Guid))
            {
                mAllowActive = row.GetBool("AllowActive");
            }
        }

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            if (SimTypes.IsDead(sim.SimDescription)) return;

            if (!mAllowActive)
            {
                if (SimTypes.IsSelectable(sim)) return;
            }

            if (sim.InteractionQueue == null) return;

            SimDescription.DeathType type = mType;
            if (type == SimDescription.DeathType.None)
            {
                List<SimDescription.DeathType> choices = new List<SimDescription.DeathType>();
                foreach (SimDescription.DeathType choice in Enum.GetValues(typeof(SimDescription.DeathType)))
                {
                    if (!OccultTypeHelper.IsInstalled(choice)) continue;

                    choices.Add(choice);
                }

                if (choices.Count == 0) return;

                type = RandomUtil.GetRandomObjectFromList(choices);
            }

            InteractionInstance entry = Urnstone.KillSim.Singleton.CreateInstance(sim, sim, new InteractionPriority(InteractionPriorityLevel.MaxDeath, 0f), false, false);
            (entry as Urnstone.KillSim).simDeathType = type;
            sim.InteractionQueue.Add(entry);
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Type: " + mType;
            result += Common.NewLine + " AllowActive: " + mAllowActive;

            return result;
        }
    }
}
