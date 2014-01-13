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
    public class BreakSymptom : SymptomBooter.Data
    {
        int mMaximum;

        public BreakSymptom(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Maximum", Guid))
            {
                mMaximum = row.GetInt("Maximum");
            }
        }

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            int count = 0;

            List<GameObject> objs = sim.LotCurrent.GetObjectsInRoom<GameObject>(sim.RoomId);

            while ((count < mMaximum) && (objs.Count > 0))
            {
                GameObject obj = RandomUtil.GetRandomObjectFromList(objs);
                count++;

                RepairableComponent repairable = obj.Repairable;
                if (repairable == null) continue;

                if (!repairable.CanBreak()) continue;

                repairable.BreakObject(sim, true);
            }
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Maximum: " + mMaximum;

            return result;
        }
    }
}
