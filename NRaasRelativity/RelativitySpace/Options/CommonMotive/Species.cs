using NRaas.CommonSpace.Options;
using NRaas.RelativitySpace.Helpers;
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
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace.Options.CommonMotive
{
    public abstract class Species : ListedSettingOption<CASAgeGenderFlags, GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "FactorSpecies";
        }

        public override string GetLocalizedValue(CASAgeGenderFlags value)
        {
            return Common.Localize("Species:" + value);
        }

        protected override List<Item> GetOptions()
        {
            List<Item> results = new List<Item>();

            foreach (CASAgeGenderFlags species in TuningAlterations.sSpecies)
            {
                results.Add(new Item(this, species));
            }

            return results;
        }

        protected override bool Allow(CASAgeGenderFlags value)
        {
            if (value == CASAgeGenderFlags.SpeciesMask) return false;

            return ((value & CASAgeGenderFlags.SpeciesMask) == value);
        }

        public override string ConvertToString(CASAgeGenderFlags value)
        {
            return value.ToString();
        }

        public override bool ConvertFromString(string value, out CASAgeGenderFlags newValue)
        {
            return ParserFunctions.TryParseEnum<CASAgeGenderFlags>(value, out newValue, CASAgeGenderFlags.None);
        }

        public override string ExportName
        {
            get { return null; }
        }
    }
}
