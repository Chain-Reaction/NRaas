using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
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
    public class InfectSetting : OperationSettingOption<Sim>, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "Infect";
        }

        protected override bool Allow(GameHitParameters<Sim> parameters)
        {
            if (!Common.kDebugging) return false;

            return base.Allow(parameters);
        }

        public static bool Perform(SimDescription sim, bool promptStrength, bool random)
        {
            List<VectorBooter.Item> choices = VectorBooter.GetVectorItems(sim);

            int maxSelection = 0;
            if (!promptStrength)
            {
                maxSelection = 1;
            }

            List<VectorBooter.Item> selection = new List<VectorBooter.Item>();

            if (random)
            {
                if (choices.Count == 0) return false;

                selection.Add(RandomUtil.GetRandomObjectFromList(choices));
            }
            else
            {
                CommonSelection<VectorBooter.Item>.Results items = new CommonSelection<VectorBooter.Item>(Common.Localize("Infect:MenuName"), choices).SelectMultiple(maxSelection);
                if ((items == null) || (items.Count == 0)) return false;

                selection.AddRange(items);
            }

            foreach (VectorBooter.Item item in selection)
            {
                DiseaseVector.Variant strain = Vector.Settings.GetCurrentStrain(item.Value);

                int strength = strain.Strength;

                if (promptStrength)
                {
                    string text = StringInputDialog.Show(Common.Localize("Infect:MenuName"), Common.Localize("Infect:Prompt", false, new object[] { item.Value.GetLocalizedName(false) }), strain.Strength.ToString());

                    if (!int.TryParse(text, out strength))
                    {
                        Common.Notify(Common.Localize("Numeric:Error"));
                    }
                }

                strain.Strength = strength;

                DiseaseVector disease = new DiseaseVector(item.Value, strain);

                disease.Infect(sim, true);
            }

            return true;
        }

        protected override OptionResult Run(GameHitParameters<Sim> parameters)
        {
            if (Perform(parameters.mTarget.SimDescription, true, false))
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
