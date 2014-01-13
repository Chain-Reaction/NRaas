using NRaas.CommonSpace.Booters;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Symptoms
{
    public class MoodletSymptom : SymptomBooter.Data
    {
        BuffNames mBuff;

        Origin mOrigin;

        int mMoodValue;

        int mDuration;

        public MoodletSymptom(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "BuffName", Guid))
            {
                if (!row.TryGetEnum<BuffNames>("BuffName", out mBuff, BuffNames.Undefined))
                {
                    mBuff = (BuffNames)row.GetUlong("BuffName", 0);
                    if (mBuff == 0)
                    {
                        mBuff = (BuffNames)ResourceUtils.HashString64(row.GetString("BuffName"));
                    }

                    if (!BuffManager.BuffDictionary.ContainsKey((ulong)mBuff))
                    {
                        BooterLogger.AddError(Guid + " Unknown BuffName: " + row.GetString("BuffName"));
                    }
                }
            }

            mMoodValue = row.GetInt("MoodValue", 0);

            mDuration = row.GetInt("Duration", 30);
            if (mDuration <= 0)
            {
                mDuration = -1;
            }

            mOrigin = (Origin)row.GetUlong("Origin", 0);
            if (mOrigin == Origin.None)
            {
                mOrigin = (Origin)ResourceUtils.HashString64(row.GetString("Origin"));
            }
        }

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            if (sim.BuffManager == null) return;

            sim.BuffManager.AddElement(mBuff, mMoodValue, mDuration, mOrigin);
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " BuffName: " + mBuff;
            result += Common.NewLine + " MoodValue: " + mMoodValue;
            result += Common.NewLine + " Duration: " + mDuration;
            result += Common.NewLine + " Origin: " + mOrigin;

            return result;
        }
    }
}
