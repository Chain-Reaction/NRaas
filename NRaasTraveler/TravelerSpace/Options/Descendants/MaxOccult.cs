using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Options.Descendants
{
    public class MaxOccult : IntegerSettingOption<GameObject>, IDescendantOption
    {
        protected override int Value
        {
            get
            {
                return Traveler.Settings.mMaxOccult;
            }
            set
            {
                Traveler.Settings.mMaxOccult = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "MaxOccult";
        }        

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            /*
            int value = Traveler.Settings.mMaxOccult;

            string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt"), value.ToString(), 256, StringInputDialog.Validation.None);
            if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

            mCount = 0;
            if (!int.TryParse(text, out mCount))
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                return OptionResult.Failure;
            }

            Traveler.Settings.mMaxOccult = mCount;

            return OptionResult.SuccessRetain;
             * */
            return base.Run(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}