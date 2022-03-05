using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using System.Collections.Generic;

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