using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Symptoms
{
    public class FirestarterSymptom : SymptomBooter.Data
    {
        int mMaximum;

        public FirestarterSymptom(XmlDbRow row)
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

                if (obj.GetFireType() == FireType.DoesNotBurn) continue;

                LotLocation loc = LotLocation.Invalid;
                ulong lotLocation = World.GetLotLocation(obj.PositionOnFloor, ref loc);

                if (!World.HasSolidFloor(obj.LotCurrent.mLotId, loc)) continue;

                if (lotLocation == 0x0L) continue;

                Sims3.Gameplay.Objects.Fire.CreateFire(lotLocation, loc);
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
