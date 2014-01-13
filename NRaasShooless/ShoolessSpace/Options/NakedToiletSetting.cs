using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.ShoolessSpace.Options
{
    public class NakedToiletSetting : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return Shooless.Settings.mNakedToilet;
            }
            set
            {
                Shooless.Settings.mNakedToilet = value;
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Common.IsRootMenuObject(parameters.mTarget)) return false;

            return base.Allow(parameters);
        }

        public override string GetTitlePrefix()
        {
            return "NakedToilet";
        }
    }
}


