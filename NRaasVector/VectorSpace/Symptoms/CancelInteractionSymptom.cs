using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Symptoms
{
    public class CancelInteractionSymptom : SymptomBooter.Data
    {
        bool mAll;

        bool mAffectSleep;

        public CancelInteractionSymptom(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "All", Guid))
            {
                mAll = row.GetBool("All");
            }

            if (BooterLogger.Exists(row, "AffectSleep", Guid))
            {
                mAffectSleep = row.GetBool("AffectSleep");
            }
        }

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            if (sim.InteractionQueue == null) return;

            if (sim.CurrentInteraction is ICountsAsWorking) return;

            if (SimTypes.IsServiceOrRole(sim.SimDescription, false))
            {
                if (!SimTypes.IsSelectable(sim)) return;
            }

            if (!mAffectSleep)
            {
                if (sim.CurrentInteraction is ISleeping) return;
            }

            if (sim.CurrentInteraction != null)
            {
                if (sim.CurrentInteraction.Target is RabbitHole) return;
            }

            try
            {
                if (mAll)
                {
                    sim.InteractionQueue.CancelAllInteractions();
                }
                else if (sim.CurrentInteraction != null)
                {
                    sim.InteractionQueue.CancelInteraction(sim.CurrentInteraction, false);
                }
            }
            catch (Exception e)
            {
                Common.DebugException(sim, e);
            }
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " All: " + mAll;
            result += Common.NewLine + " AffectSleep: " + mAffectSleep;

            return result;
        }
    }
}
