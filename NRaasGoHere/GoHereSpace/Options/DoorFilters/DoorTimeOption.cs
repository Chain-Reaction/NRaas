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
    public class DoorTimeOption : OperationSettingOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "OpenCloseTime";
        }        

        public override string DisplayValue
        {
            get
            {
                return null;
            }
        }        

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (parameters.mTarget != null)
            {
                DoorPortalComponentEx.DoorSettings settings = GoHere.Settings.GetDoorSettings(parameters.mTarget.ObjectId);

                string open = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":PromptOpen"), settings.mDoorOpen.ToString());
                if (string.IsNullOrEmpty(open)) return OptionResult.Failure;

                string close = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":PromptClose"), settings.mDoorClose.ToString());
                if (string.IsNullOrEmpty(close)) return OptionResult.Failure;

                int openTime = -1;
                int closeTime = -1;
                if (!int.TryParse(open, out openTime) || !int.TryParse(close, out closeTime))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return OptionResult.Failure;
                }

                if ((openTime < 1 || openTime > 23) || (closeTime < 1 || closeTime > 23))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:InvalidInput"));
                    return OptionResult.Failure;
                }

                settings.mDoorOpen = openTime;
                settings.mDoorClose = closeTime;
                GoHere.Settings.AddOrUpdateDoorSettings(parameters.mTarget.ObjectId, settings);

                Common.Notify(Common.Localize("Generic:Success"));

                return OptionResult.SuccessClose;
            }            

            return OptionResult.Failure;
        }
    }
}