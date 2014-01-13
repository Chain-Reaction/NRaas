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
    public class ReactionSymptom : SymptomBooter.Data
    {
        ReactionTypes mType;

        ReactionSpeed mSpeed;

        public ReactionSymptom(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Type", Guid))
            {
                if (!row.TryGetEnum<ReactionTypes>("Type", out mType, ReactionTypes.None))
                {
                    BooterLogger.AddError(Guid + " Unknown Type: " + row.GetString("Type"));
                }
            }

            if (BooterLogger.Exists(row, "Speed", Guid))
            {
                if (!row.TryGetEnum<ReactionSpeed>("Speed", out mSpeed, ReactionSpeed.None))
                {
                    BooterLogger.AddError(Guid + " Unknown Speed: " + row.GetString("Speed"));
                }
            }
        }

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            sim.PlayReaction(mType, mSpeed);
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Type: " + mType;
            result += Common.NewLine + " Speed: " + mSpeed;

            return result;
        }
    }
}
