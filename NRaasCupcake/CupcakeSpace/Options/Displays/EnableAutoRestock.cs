using NRaas.CommonSpace.Options;
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
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.Store.Objects;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CupcakeSpace.Options.Displays
{
    public class EnableAutoRestock : OperationSettingOption<GameObject>, ICaseOption
    {
        IGameObject mTarget;

        public override string GetTitlePrefix()
        {
            return "DisableAutoRestock";
        }

        public override string Name
        {
            get
            {
                if (mTarget != null)
                {
                    if (Cupcake.Settings.IsDisplayExempt(mTarget.ObjectId))
                    {
                        return Cupcake.Localize("EnableAutoRestock:MenuName");
                    }
                    else
                    {
                        return Cupcake.Localize("DisableAutoRestock:MenuName");
                    }
                }

                return "Debug: EnableAutoRestock";
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;
            if (mTarget == null) return false;

            if (!(mTarget is CraftersConsignmentDisplay)) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (Cupcake.Settings.IsDisplayExempt(mTarget.ObjectId))
            {
                Cupcake.Settings.ClearExemptStatus(mTarget.ObjectId);
            }
            else
            {
                Cupcake.Settings.AddExemptStatus(mTarget.ObjectId);
            }

            Common.Notify(Common.Localize("General:Success"));

            return OptionResult.SuccessClose;
        }
    }
}