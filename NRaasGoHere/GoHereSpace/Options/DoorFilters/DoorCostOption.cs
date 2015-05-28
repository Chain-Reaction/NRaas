using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.GoHereSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.GoHereSpace.Options.DoorFilters
{
    public class DoorCostOption : OperationSettingOption<GameObject>, IDoorOption
    {
        GameObject mTarget;

        public override string GetTitlePrefix()
        {
            return "DoorCost";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;
            return base.Allow(parameters);
        }

        public override string DisplayValue
        {
            get
            {
                if (mTarget != null)
                    return GoHere.Settings.GetDoorSettings(mTarget.ObjectId).mDoorCost.ToString();

                return null;
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (parameters.mTarget != null)
            {
                DoorPortalComponentEx.DoorSettings settings = GoHere.Settings.GetDoorSettings(parameters.mTarget.ObjectId);

                string sCost = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":PromptCost"), settings.mDoorCost.ToString());
                if (string.IsNullOrEmpty(sCost)) return OptionResult.Failure;                

                int cost = 0;                
                if (!int.TryParse(sCost, out cost))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return OptionResult.Failure;
                }

                if (cost < 0 || cost > int.MaxValue)
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:InvalidInput"));
                    return OptionResult.Failure;
                }

                settings.mDoorCost = cost;                
                GoHere.Settings.AddOrUpdateDoorSettings(parameters.mTarget.ObjectId, settings, false);

                Common.Notify(Common.Localize("Generic:Success"));

                return OptionResult.SuccessClose;
            }

            return OptionResult.Failure;
        }
    }
}