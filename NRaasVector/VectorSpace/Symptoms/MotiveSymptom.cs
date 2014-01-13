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
    public class MotiveSymptom : SymptomBooter.Data
    {
        CommodityKind mMotive;

        int mDeltaValue;

        int mLowerBound;

        public MotiveSymptom(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Motive", Guid))
            {
                if (!row.TryGetEnum<CommodityKind>("Motive", out mMotive, CommodityKind.None))
                {
                    BooterLogger.AddError(" Unknown Motive: " + row.GetString("Motive"));
                }
            }

            if (BooterLogger.Exists(row, "DeltaValue", Guid))
            {
                mDeltaValue = row.GetInt("DeltaValue", 0);
            }

            if (BooterLogger.Exists(row, "LowerBound", Guid))
            {
                mLowerBound = row.GetInt("LowerBound", 0);
            }
        }

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            if (sim.Autonomy == null) return;

            if (sim.Autonomy.Motives == null) return;            

            if (sim.Autonomy.Motives.HasMotive(mMotive))
            {
                int delta = (int)(mDeltaValue * (Vector.Settings.GetMotiveAdjustmentRatio(vector.Guid) / 100f));

                if (delta < 0)
                {
                    float current = sim.Autonomy.Motives.GetValue(mMotive);

                    if (current < mLowerBound)
                    {
                        delta = 0;
                    }
                    if ((current + delta) < mLowerBound)
                    {
                        delta = (int)(mLowerBound - current);
                    }
                }

                sim.Autonomy.Motives.ChangeValue(mMotive, delta);
            }
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Motive: " + mMotive;
            result += Common.NewLine + " DeltaValue: " + mDeltaValue;
            result += Common.NewLine + " LowerBound: " + mLowerBound;

            return result;
        }
    }
}
