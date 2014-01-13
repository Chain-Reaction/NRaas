using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Symptoms
{
    public class SocialSymptom : SymptomBooter.Data
    {
        string mSocial;

        string mScoring;

        int mMinimum;

        bool mRoomOnly;

        public SocialSymptom(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Social", Guid))
            {
                mSocial = row.GetString("Social");

                if (mSocial != "Braaaiiins")
                {
                    if (ActionData.Get(mSocial) == null)
                    {
                        BooterLogger.AddError(Guid + " Invalid Social: " + mSocial);
                    }
                }
            }

            if (BooterLogger.Exists(row, "RoomOnly", Guid))
            {
                mRoomOnly = row.GetBool("RoomOnly");
            }

            mScoring = row.GetString("Scoring");

            if (!string.IsNullOrEmpty(mScoring))
            {
                if (ScoringLookup.GetScoring(mScoring) == null)
                {
                    BooterLogger.AddError(Guid + " Invalid Scoring: " + mScoring);
                }

                mMinimum = row.GetInt("Minimum", 0);
            }
        }

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            if (SimTypes.IsDead(sim.SimDescription)) return;

            List<Sim> sims = new List<Sim>();

            foreach (Sim other in sim.LotCurrent.GetSims())
            {
                if (other == sim) continue;

                if (mRoomOnly)
                {
                    if (other.RoomId != sim.RoomId) continue;
                }

                sims.Add(other);
            }

            if (sims.Count == 0) return;

            Sim choice = RandomUtil.GetRandomObjectFromList(sims);

            try
            {
                if (sim.SimDescription.IsZombie)
                {
                    InteractionInstance entry = new Sim.ZombifiedSocials.Definition(Sim.ZombifiedSocials.Definition.eInteractionType.Braaaiiins).CreateInstance(choice, sim, new InteractionPriority(InteractionPriorityLevel.High), true, true);
                    sim.InteractionQueue.AddNext(entry);
                }
                else if (mSocial != "Braaaiiins")
                {
                    InteractionInstance entry = new SocialInteractionA.Definition(mSocial, null, null, false).CreateInstance(choice, sim, new InteractionPriority(InteractionPriorityLevel.High), true, true);

                    sim.InteractionQueue.AddNext(entry);
                }
            }
            catch (Exception e)
            {
                Common.DebugException(sim, choice, e);
            }
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Social: " + mSocial;
            result += Common.NewLine + " RoomOnly: " + mRoomOnly;

            return result;
        }
    }
}
