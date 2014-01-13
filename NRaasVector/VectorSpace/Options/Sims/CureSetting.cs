using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.VectorSpace.Options.Sims
{
    public class CureSetting : OperationSettingOption<Sim>, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "Cure";
        }

        protected override bool Allow(GameHitParameters<Sim> parameters)
        {
            if (!Common.kDebugging) return false;

            return base.Allow(parameters);
        }

        public static bool Perform(SimDescription sim)
        {
            List<VectorBooter.Item> items = new List<VectorBooter.Item>();

            foreach (DiseaseVector vector in Vector.Settings.GetVectors(sim))
            {
                items.Add(new VectorBooter.Item(vector, sim.IsFemale));
            }

            CommonSelection<VectorBooter.Item>.Results choices = new CommonSelection<VectorBooter.Item>("Cure:MenuName", items).SelectMultiple();
            if ((choices == null) || (choices.Count == 0)) return false;

            foreach (VectorBooter.Item item in choices)
            {
                Vector.Settings.RemoveVector(sim, item.Value.Guid);

                Common.Notify(Common.Localize("Cure:Success", sim.IsFemale, new object[] { sim, item.Value.GetLocalizedName(sim.IsFemale) }));
            }

            return true;
        }

        protected override OptionResult Run(GameHitParameters<Sim> parameters)
        {
            if (Perform(parameters.mTarget.SimDescription))
            {
                return OptionResult.SuccessClose;
            }
            else
            {
                return OptionResult.Failure;
            }
        }
    }
}
