using NRaas.CommonSpace.Options;
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
    public class ChangeCasteOptions : OperationSettingOption<GameObject>, ICasteOption
    {
        CasteOptions mOption;

        public ChangeCasteOptions(CasteOptions option)
        {
            mOption = option;
        }

        public override string GetTitlePrefix()
        {
            return "ChangeCaste";
        }

        public override string Name
        {
            get
            {
                return mOption.Name;
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!base.Allow(parameters)) return false;

            if (Common.IsRootMenuObject(parameters.mTarget)) return true;

            Sim sim = parameters.mTarget as Sim;
            if (sim != null) return true;

            return false;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            mOption.ShowOptions(StoryProgression.Main, Name);
            return OptionResult.SuccessRetain;
        }
    }
}

