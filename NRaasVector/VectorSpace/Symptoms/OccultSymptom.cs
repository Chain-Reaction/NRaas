using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Symptoms
{
    public class OccultSymptom : SymptomBooter.Data
    {
        OccultTypes mOccult;

        bool mRemove;

        bool mDropOthers;

        bool mAllowIfOthers;

        public OccultSymptom(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Occult", Guid))
            {
                if (!row.TryGetEnum<OccultTypes>("Occult", out mOccult, OccultTypes.None))
                {
                    BooterLogger.AddError(" Unknown Occult: " + row.GetString("Occult"));
                }
            }

            if (BooterLogger.Exists(row, "Remove", Guid))
            {
                mRemove = row.GetBool("Remove");
            }

            if (BooterLogger.Exists(row, "DropOthers", Guid))
            {
                mDropOthers = row.GetBool("DropOthers");
            }

            if (BooterLogger.Exists(row, "AllowIfOthers", Guid))
            {
                mAllowIfOthers = row.GetBool("AllowIfOthers");
            }
        }

        public override void Perform(Sim sim, DiseaseVector vector)
        {
            if (sim.OccultManager== null) return;

            if (mRemove)
            {
                OccultTypeHelper.Remove(sim.SimDescription, mOccult, true);
            }
            else
            {
                if (sim.OccultManager.HasOccultType(mOccult)) return;

                if (mDropOthers)
                {
                    foreach (OccultTypes type in Enum.GetValues(typeof(OccultTypes)))
                    {
                        if (type == OccultTypes.None) continue;

                        OccultTypeHelper.Remove(sim.SimDescription, type, true);
                    }
                }

                OccultTypeHelper.Add(sim.SimDescription, mOccult, false, true);
            }
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Occult: " + mOccult;
            result += Common.NewLine + " Remove: " + mRemove;
            result += Common.NewLine + " DropOthers: " + mDropOthers;
            result += Common.NewLine + " AllowIfOthers: " + mAllowIfOthers;

            return result;
        }
    }
}
