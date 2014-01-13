using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Town
{
    public class RandomizeGenderPreference : OptionItem, ITownOption
    {
        public override string GetTitlePrefix()
        {
            return "RandomizeGenderPreference";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            string value = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt"), "0");
            if (string.IsNullOrEmpty(value)) return OptionResult.Failure;

            int chance;
            if (!int.TryParse(value, out chance))
            {
                Common.Notify(Common.Localize("Numeric:Error"));
                return OptionResult.Failure;
            }

            if (chance < 0)
            {
                chance = 0;
            }
            else if (chance > 100)
            {
                chance = 100;
            }

            int count = 0, gayCount = 0;

            foreach (SimDescription sim in Household.EverySimDescription())
            {
                if ((sim.mGenderPreferenceFemale == 0) && (sim.mGenderPreferenceMale == 0))
                {
                    if (RandomUtil.RandomChance(chance))
                    {
                        sim.mGenderPreferenceMale = sim.IsFemale ? -1 : 1;
                        sim.mGenderPreferenceFemale = sim.IsFemale ? 1 : -1;

                        gayCount++;
                    }
                    else
                    {
                        sim.mGenderPreferenceMale = sim.IsFemale ? 1 : -1;
                        sim.mGenderPreferenceFemale = sim.IsFemale ? -1 : 1;

                        count++;
                    }
                }
            }

            SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Result", false, new object[] { count, gayCount }));

            return OptionResult.SuccessClose;
        }
    }
}
