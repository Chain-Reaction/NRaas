using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Symptoms
{
    public class ImpregnateSymptom : SymptomBooter.Data
    {
        bool mClone;

        bool mAllowCloseRelations;

        bool mHandlePlantSim = true;

        public ImpregnateSymptom(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Clone", Guid))
            {
                mClone = row.GetBool("Clone");
            }

            if (BooterLogger.Exists(row, "AllowCloseRelations", Guid))
            {
                mAllowCloseRelations = row.GetBool("AllowCloseRelations");
            }

            if (row.Exists("HandlePlantSim"))
            {
                mHandlePlantSim = row.GetBool("HandlePlantSim");
            }
        }

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            if (sim.SimDescription.IsPregnant) return;

            if (mClone)
            {
                Pregnancies.Start(sim, sim.SimDescription, mHandlePlantSim);
            }
            else if (vector.Infector != 0)
            {
                SimDescription other = SimDescription.Find(vector.Infector);
                if (other != null)
                {
                    if (!mAllowCloseRelations)
                    {
                        if (Relationships.IsCloselyRelated(sim.SimDescription, other, false)) return;
                    }

                    Pregnancies.Start(sim, other, mHandlePlantSim);
                }
            }
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Clone: " + mClone;
            result += Common.NewLine + " AllowCloseRelations: " + mClone;

            return result;
        }
    }
}
