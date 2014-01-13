using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Careers
{
    public class Retire : CareerOption
    {
        public override string GetTitlePrefix()
        {
            return "Retire";
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

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.Occupation == null) return false;

            //if (!me.Occupation.CanRetire()) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                if (!AcceptCancelDialog.Show(Common.Localize("Retire:Prompt", me.IsFemale, new object[] { me })))
                {
                    return false;
                }
            }

            EnsureCoworkerLists(me);

            me.Occupation.RetireNoConfirmation();

            if (me.CreatedSim != null)
            {
                me.CreatedSim.InteractionQueue.CancelAllInteractionsByType(WorkInRabbitHole.Singleton);
            }

            return true;
        }
    }
}
