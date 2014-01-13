using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class FamilyTree : SimFromList, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "FamilyTree";
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

        protected override bool PrivateAllow(SimDescription me)
        {
            return true;
        }
        protected override bool PrivateAllow(MiniSimDescription me)
        {
            return true;
        }

        public static void Perform(IMiniSimDescription me)
        {
            if (!Responder.Instance.IsGameStatePending || !Responder.Instance.IsGameStateShuttingDown)
            {
                Dialogs.FamilyTreeDialog.Show(me);
            }
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Perform(me);
            return true;
        }
        protected override bool Run(MiniSimDescription me, bool singleSelection)
        {
            Perform(me);
            return true;
        }
    }
}
