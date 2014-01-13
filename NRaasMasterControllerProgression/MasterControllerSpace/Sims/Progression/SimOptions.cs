extern alias SP;

using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Progression
{
    public class SimOptions : SimFromList, IProgressionOption
    {
        List<SP.NRaas.StoryProgressionSpace.GenericOptionBase.DefaultableOption> mOptions = null;

        public override string GetTitlePrefix()
        {
            return "ProgressionSimOptions";
        }

        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        public override void Reset()
        {
            mOptions = null;

            base.Reset();
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (SP.NRaas.StoryProgression.Main == null) return false;

            return base.Allow(parameters);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            SP.NRaas.StoryProgressionSpace.GenericOptionBase data = SP.NRaas.StoryProgression.Main.GetData(me);

            if (!ApplyAll)
            {
                mOptions = data.ListOptions(SP.NRaas.StoryProgression.Main, Common.Localize("SimOptions:MenuName"), singleSelection);
                if (mOptions == null) return false;

                foreach (SP.NRaas.StoryProgressionSpace.GenericOptionBase.DefaultableOption option in mOptions)
                {
                    option.Perform();
                }
            }
            else
            {
                if (mOptions == null) return false;

                foreach (SP.NRaas.StoryProgressionSpace.GenericOptionBase.DefaultableOption option in mOptions)
                {
                    option.Persist(data);
                }
            }
            return true;
        }
    }
}
