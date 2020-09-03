﻿using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Options.General.CareerLevel
{
    public class CareerSettingListingOption : InteractionOptionList<ICareerLevelOption, GameObject>, IGeneralOption
    {
        public override string GetTitlePrefix()
        {
            return "CareerLevelOptions";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (Sims3.Gameplay.Queries.CountObjects<RabbitHole>() == 0) return false;

            return base.Allow(parameters);
        }

        public override List<ICareerLevelOption> GetOptions()
        {
            List<ICareerLevelOption> results = new List<ICareerLevelOption>();

            results.Add(new CarpoolTypeSetting());
            results.Add(new PayPerHourBaseSetting());

            return results;
        }
    }
}