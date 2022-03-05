using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Objects.RabbitHoles;

namespace NRaas.CareerSpace.Options.General
{
    public class HomeworldUniversityTermLengthSetting : IntegerSettingOption<GameObject>, IGeneralOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.Careers.Settings.mHomeworldUniversityTermLength;
            }
            set
            {
                NRaas.Careers.Settings.mHomeworldUniversityTermLength = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "UniversityTermLength";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override int Validate(int value)
        {
            if (value < 7)
            {
                return 7;
            }

            return base.Validate(value);
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (GameStates.IsOnVacation) return false;

            if (Sims3.Gameplay.Queries.CountObjects<AdminstrationCenter>() == 0) return false;

            return base.Allow(parameters);
        }
    }
}
