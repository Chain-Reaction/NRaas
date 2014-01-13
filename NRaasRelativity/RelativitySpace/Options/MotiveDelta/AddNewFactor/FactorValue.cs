using NRaas.CommonSpace.Options;
using NRaas.RelativitySpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;

namespace NRaas.RelativitySpace.Options.MotiveDelta.AddNewFactor
{
    public class FactorValue : FloatSettingOption<GameObject>, IAddNewFactorOption
    {
        public static float sFactor = 1f;

        protected override float Value
        {
            get
            {
                return sFactor;
            }
            set
            {
                sFactor = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "FactorValue";
        }

        protected override float Validate(float value)
        {
            if (value < 0)
            {
                value = 0;
            }

            return base.Validate(value);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (base.Run(parameters) == OptionResult.Failure) return OptionResult.Failure;

            if (Age.sCurrent.Count == 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("CreateNewFactor:NoAge"));
                return OptionResult.Failure;
            }
            else if (Species.sCurrent.Count == 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("CreateNewFactor:NoSpecies"));
                return OptionResult.Failure;
            }
            else if (Occult.sCurrent.Count == 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("CreateNewFactor:NoOccult"));
                return OptionResult.Failure;
            }
            else if (Commodity.sCurrent.Count == 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("CreateNewFactor:NoCommodity"));
                return OptionResult.Failure;
            }

            foreach (CASAgeGenderFlags species in Species.sCurrent)
            {
                foreach (CASAgeGenderFlags age in Age.sCurrent)
                {
                    foreach (OccultTypes occult in Occult.sCurrent)
                    {
                        foreach (CommodityKind kind in Commodity.sCurrent)
                        {
                            MotiveKey key = new MotiveKey(age | species, occult, kind);

                            Relativity.Settings.SetMotiveFactor(key, Value);
                        }
                    }
                }
            }

            PriorValues.sFactorChanged = true;
            return OptionResult.SuccessLevelDown;
        }

        public override string ExportName
        {
            get { return null; }
        }
    }
}
