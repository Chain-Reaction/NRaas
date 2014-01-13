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
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace.Options.CommonMotive
{
    public abstract class Commodity : ListedSettingOption<CommodityKind, GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "FactorCommodity";
        }

        public override string GetLocalizedValue(CommodityKind value)
        {
            return CommoditiesEx.GetMotiveLocalizedName(value);
        }

        protected override List<Item> GetOptions()
        {
            List<Item> results = new List<Item>();

            foreach (CommodityKind kind in TuningAlterations.sCommodities)
            {
                results.Add(new Item(this, kind));
            }

            return results;
        }

        public override string ConvertToString(CommodityKind value)
        {
            return value.ToString();
        }

        public override bool ConvertFromString(string value, out CommodityKind newValue)
        {
            return ParserFunctions.TryParseEnum<CommodityKind>(value, out newValue, CommodityKind.None);
        }

        public override string ExportName
        {
            get { return null; }
        }
    }
}
