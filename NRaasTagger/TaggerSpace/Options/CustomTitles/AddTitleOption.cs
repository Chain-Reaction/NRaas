using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TaggerSpace.Options.CustomTitles
{
    public class AddTitleOption : OperationSettingOption<GameObject>, ICustomTitleOption
    {
        public override string GetTitlePrefix()
        {
            return "AddTitle";
        }        

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Sim target = parameters.mActor as Sim;
            if (target != null)
            {
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", false), "");
                if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

                if (!Tagger.Settings.mCustomSimTitles.ContainsKey(target.SimDescription.SimDescriptionId))
                {
                    Tagger.Settings.mCustomSimTitles.Add(target.SimDescription.SimDescriptionId, new List<string>());
                }

                if (!Tagger.Settings.mCustomSimTitles[target.SimDescription.SimDescriptionId].Contains(text))
                {
                    Tagger.Settings.mCustomSimTitles[target.SimDescription.SimDescriptionId].Add(text);
                }

                Tagger.Settings.SetCustomTitles(target);

                Common.Notify(Common.Localize("General:Success"));
            }

            return OptionResult.SuccessClose;
        }
    }
}