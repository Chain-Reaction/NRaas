using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Options.General.Careers
{
    public class CareerSettingListingOption : InteractionOptionList<ICareerOption, GameObject>, IGeneralOption
    {
        public override string GetTitlePrefix()
        {
            return "CareerOptions";
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

        public override List<ICareerOption> GetOptions()
        {
            List<ICareerOption> results = new List<ICareerOption>();

            results.Add(new MinCoworkerSetting());
            results.Add(new MaxCoworkerSetting());

            return results;
        }
    }
}