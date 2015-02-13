using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Options
{
    public class SprinklersSpawnPuddles : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return Tempest.Settings.mSprinklersSpawnPuddles;
            }
            set
            {
                Tempest.Settings.mSprinklersSpawnPuddles = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "SprinklersSpawnPuddlesInside";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return Tempest.Settings.Debugging;
        }
    }
}