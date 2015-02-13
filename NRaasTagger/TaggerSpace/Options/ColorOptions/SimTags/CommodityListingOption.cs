using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TaggerSpace.Options.ColorOptions.SimTags.Commodity;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options.ColorOptions.SimTags
{
    public class CommodityListingOption : InteractionOptionList<ISimTagCommodityColorOption, GameObject>, ISimTagColorOption
    {
        public override string GetTitlePrefix()
        {
            return "MotiveRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return base.Allow(parameters);
        }

        public override List<ISimTagCommodityColorOption> GetOptions()
        {
            List<ISimTagCommodityColorOption> results = new List<ISimTagCommodityColorOption>();

            foreach (CommodityKind flag in Enum.GetValues(typeof(CommodityKind)))
            {
                switch (flag)
                {
                    case CommodityKind.BatteryPower:
                    case CommodityKind.Bladder:
                    case CommodityKind.CatScratch:
                    case CommodityKind.DogDestruction:
                    case CommodityKind.Energy:
                    case CommodityKind.Fun:
                    case CommodityKind.HorseExercise:
                    case CommodityKind.HorseThirst:
                    case CommodityKind.Hunger:
                    case CommodityKind.Hygiene:
                    case CommodityKind.Maintenence:
                    case CommodityKind.MermaidDermalHydration:
                    case CommodityKind.Social:
                    case CommodityKind.Temperature:
                    case CommodityKind.VampireThirst:
                        results.Add(new CommodityOption(flag));
                        break;
                    default:
                        break;
                }
            }

            return results;
        }
    }
}