extern alias SP;

using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
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

namespace NRaas.MasterControllerSpace.Sims.Status
{
    public class ProgressionStatus : SimFromList, IStatusOption
    {
        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        public override string GetTitlePrefix()
        {
            return "ProgressionStatus";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return Perform(me);
        }

        public string GetDetails(IMiniSimDescription me)
        {
            string msg = null;

            try
            {
                msg = PersonalStatus.GetHeader(me);

                SimDescription simDesc = me as SimDescription;

                msg += Common.NewLine + SP::NRaas.StoryProgression.Main.Sims.GetStatus(simDesc);
            }
            catch (Exception e)
            {
                Common.Exception(me.FullName, e);

                msg += Common.NewLine + "END OF LINE";
            }

            return msg;
        }

        protected bool Perform(IMiniSimDescription me)
        {
            SimpleMessageDialog.Show(Name, GetDetails(me));
            return true;
        }
    }
}
