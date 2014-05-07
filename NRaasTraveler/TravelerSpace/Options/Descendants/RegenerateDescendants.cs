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
    public class RegenerateDescendants : OperationSettingNestledOption<GameObject>, IDescendantOption
    {
        public override string GetTitlePrefix()
        {
            return "RegenerateDescendants";
        }        

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            // I spent <ages> on this and can't get it to be 100% reliable as it keeps erroring at random
            // times for no apparent reason when you don't take all the progenators to the Future. The game
            // will even generate descendants fine on world arrival then error here. Something in EA's craptastic engine 
            // doesn't like it when you keep unpacking mini sims apparently. Left it here as it is fixable with a lot of 
            // dancing around (such as cloning the SimDescription fields needed for generation) but I don't have time right now
            if (!GameUtils.IsFutureWorld() || Traveler.Settings.mDisableDescendants) return false;

            if (!Common.kDebugging) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt"))) return OptionResult.Failure;

            FutureDescendantServiceEx.RegenerateDescendants();

            return OptionResult.SuccessClose;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}