using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace.Options.Settings
{
    public class SettingsOption : InteractionOptionList<ITuningOption, GameObject>, ISettingsOption
    {
        SettingsKey mSeason;

        public SettingsOption(SettingsKey season)
            : base(season.LocalizedName)
        {
            mSeason = season;
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            // Don't call the base class it takes awhile to process
            return true;
        }

        public override OptionResult Perform(GameHitParameters<GameObject> parameters)
        {
            using (Retuner.ActiveSettingsToggle activeSeason = new Retuner.ActiveSettingsToggle(mSeason))
            {
                return base.Perform(parameters);
            }
        }
    }
}
