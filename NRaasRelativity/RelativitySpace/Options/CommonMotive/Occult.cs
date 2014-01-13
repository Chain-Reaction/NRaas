using NRaas.CommonSpace.Helpers;
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
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace.Options.CommonMotive
{
    public abstract class Occult : ListedSettingOption<OccultTypes, GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "FactorOccult";
        }

        public override string GetLocalizedValue(OccultTypes value)
        {
            return OccultTypeHelper.GetLocalizedName(value);
        }

        protected override List<Item> GetOptions()
        {
            List<Item> results = new List<Item>();

            foreach (OccultTypes species in TuningAlterations.sHumanOccults)
            {
                results.Add(new Item(this, species));
            }

            return results;
        }

        public override string ConvertToString(OccultTypes value)
        {
            return value.ToString();
        }

        public override bool ConvertFromString(string value, out OccultTypes newValue)
        {
            return ParserFunctions.TryParseEnum<OccultTypes>(value, out newValue, OccultTypes.None);
        }

        public override string ExportName
        {
            get { return null; }
        }
    }
}
