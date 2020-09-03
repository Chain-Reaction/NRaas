﻿using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Options
{
    public class AllowBoatRoutingSetting : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return GoHere.Settings.mAllowBoatRouting;
            }
            set
            {
                GoHere.Settings.mAllowBoatRouting = value;

                if (value)
                {
                    Boat.kDistanceToDestinationSoSimWillBoat = 20000;
                }
                else
                {
                    Boat.kDistanceToDestinationSoSimWillBoat = GoHere.Settings.mOrigBoatRoutingDistance;
                }
            }
        }

        public override string GetTitlePrefix()
        {
            return "AllowBoatRouting";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}