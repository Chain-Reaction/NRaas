using NRaas.CommonSpace.Options;
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

namespace NRaas.MasterControllerSpace.Sims
{
    public class IsSimOption : IDisposable
    {
        public static bool sSet;

        public IsSimOption()
        {
            sSet = true;
        }

        public void Dispose()
        {
            sSet = false;
        }
    }

    public class ListingOption : OptionList<ISimOption>, IPrimaryOption<GameObject>, ILotOption
    {
        public override string GetTitlePrefix()
        {
            return "SimInteraction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override OptionResult Run(ISimOption option, GameHitParameters<GameObject> parameters)
        {
            using (IsSimOption isSim = new IsSimOption())
            {
                return base.Run(option, parameters);
            }
        }
    }
}
