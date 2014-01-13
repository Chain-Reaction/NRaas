using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Settings
{
    public class AddCasteOption : OperationSettingOption<GameObject>, ICasteOption
    {
        public override string GetTitlePrefix()
        {
            return "AddCaste";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            string text = StringInputDialog.Show(Name, Common.Localize("CasteName:Prompt"), "");
            if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

            bool created;
            CasteOptions options = StoryProgression.Main.Options.GetNewCasteOptions(null, text, out created);
            if (options == null) return OptionResult.Failure;

            List<SimDescription> sims = new List<SimDescription>();

            Sim sim = parameters.mTarget as Sim;
            if (sim != null)
            {
                StoryProgression.Main.AddValue<ManualCasteOption,CasteOptions>(sim.SimDescription, options);
            }
            else
            {
                Lot lot = parameters.mTarget as Lot;
                if (lot != null)
                {
                    foreach (SimDescription member in Households.All(lot.Household))
                    {
                        StoryProgression.Main.AddValue<ManualCasteOption,CasteOptions>(member, options);
                    }
                }
            }

            return OptionResult.SuccessRetain;
        }
    }
}

