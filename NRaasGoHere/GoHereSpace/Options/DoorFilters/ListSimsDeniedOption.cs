using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
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
    public class ListSimsDeniedOption : OperationSettingOption<GameObject>, IDoorOption
    {
        public override string GetTitlePrefix()
        {
            return "ListDeniedSims";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (GoHere.Settings.GetDoorSettings(parameters.mTarget.ObjectId).FiltersEnabled == 0) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Sim actorSim = parameters.mActor as Sim;

            if (parameters.mTarget != null)
            {
                SimSelection sims = SimSelection.Create("", Name, actorSim.SimDescription, parameters.mTarget);
                if (sims.IsEmpty)
                {
                    return OptionResult.Failure;
                }

                sims.SelectSingle();
            }

            return OptionResult.SuccessClose;
        }

        protected class SimSelection : ProtoSimSelection<SimDescription>
        {
            static GameObject mTarget;

            private SimSelection(string title, string subTitle, SimDescription me)
                : base(title, subTitle, me, true, true)
            {
            }

            public static SimSelection Create(string title, string subTitle, SimDescription me, GameObject target)
            {
                mTarget = target;

                SimSelection selection = new SimSelection(title, subTitle, me);

                bool canceled = false;
                selection.FilterSims(null, null, false, out canceled);

                return selection;
            }

            protected override bool Allow(SimDescription sim)
            {
                if (mTarget == null) return false;

                return !GoHere.Settings.GetDoorSettings(mTarget.ObjectId).IsSimAllowedThrough(sim.SimDescriptionId);
            }
        }
    }
}