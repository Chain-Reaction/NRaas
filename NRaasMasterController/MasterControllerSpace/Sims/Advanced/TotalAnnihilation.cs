using NRaas.CommonSpace.Helpers;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class TotalAnnihilation : SimFromList, IAdvancedOption
    {
        public override string GetTitlePrefix()
        {
            return "TotalAnnihilation";
        }

        protected override bool TestValid
        {
            get { return false; }
        }

        protected override bool ShowProgress
        {
            get { return true; }
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.CreatedSim == Sim.ActiveActor)
            {
                return false;
            }

            return true;
        }
        protected override bool PrivateAllow(MiniSimDescription me)
        {
            return true;
        }

        protected bool Prompt (IMiniSimDescription me)
        {
            if (!ApplyAll)
            {
                if (!AcceptCancelDialog.Show(Common.Localize("TotalAnnihilation:Prompt", me.IsFemale, new object[] { me })))
                {
                    return false;
                }
            }

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!Prompt(me)) return false;

            Annihilation.Cleanse(me);
            return true;
        }
        protected override bool Run(MiniSimDescription me, bool singleSelection)
        {
            if (!Prompt(me)) return false;

            Annihilation.Cleanse(me);
            return true;
        }
    }
}
