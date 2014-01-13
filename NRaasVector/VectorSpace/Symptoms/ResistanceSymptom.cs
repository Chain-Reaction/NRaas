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
using System.Collections.Generic;

namespace NRaas.VectorSpace.Symptoms
{
    public class ResistanceSymptom : SymptomBooter.Data
    {
        bool mAffectOther;
        bool mAffectSelf;

        int mMinimum;
        int mMaximum;

        public ResistanceSymptom(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "AffectSelf", Guid))
            {
                mAffectSelf = row.GetBool("AffectSelf");
            }

            if (BooterLogger.Exists(row, "AffectOther", Guid))
            {
                mAffectOther = row.GetBool("AffectOther");
            }

            if (BooterLogger.Exists(row, "Minimum", Guid))
            {
                mMinimum = row.GetInt("Minimum", 0);
            }

            if (BooterLogger.Exists(row, "Maximum", Guid))
            {
                mMaximum = row.GetInt("Maximum", 0);
            }
        }

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            if (mAffectOther)
            {
                foreach (DiseaseVector disease in Vector.Settings.GetVectors(sim))
                {
                    if (!mAffectSelf)
                    {
                        if (disease == vector) continue;
                    }

                    disease.AlterResistance(RandomUtil.GetInt(mMinimum, mMaximum));
                }
            }            
            else if (mAffectSelf)
            {
                vector.AlterResistance(RandomUtil.GetInt(mMinimum, mMaximum));
            }
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " AffectOther: " + mAffectOther;
            result += Common.NewLine + " AffectSelf: " + mAffectSelf;
            result += Common.NewLine + " Minimum: " + mMinimum;
            result += Common.NewLine + " Maximum: " + mMaximum;

            return result;
        }
    }
}
