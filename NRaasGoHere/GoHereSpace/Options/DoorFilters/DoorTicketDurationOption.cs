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
    public class DoorTicketDurationOption : OperationSettingOption<GameObject>, IDoorOption
    {
        GameObject mTarget;

        public override string GetTitlePrefix()
        {
            return "DoorTicketDuration";
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
                    return GoHere.Settings.GetDoorSettings(mTarget.ObjectId).mDoorTicketDuration.ToString();

                return null;
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (parameters.mTarget != null)
            {
                DoorPortalComponentEx.DoorSettings settings = GoHere.Settings.GetDoorSettings(parameters.mTarget.ObjectId);

                string sDuration = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":PromptCost"), settings.mDoorTicketDuration.ToString());
                if (string.IsNullOrEmpty(sDuration)) return OptionResult.Failure;

                int duration = 0;
                if (!int.TryParse(sDuration, out duration))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return OptionResult.Failure;
                }

                if (duration < 0 || duration > int.MaxValue)
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:InvalidInput"));
                    return OptionResult.Failure;
                }

                settings.mDoorTicketDuration = duration;
                GoHere.Settings.AddOrUpdateDoorSettings(parameters.mTarget.ObjectId, settings, false);

                Common.Notify(Common.Localize("Generic:Success"));

                return OptionResult.SuccessClose;
            }

            return OptionResult.Failure;
        }
    }
}