using NRaas.MasterControllerSpace.Sims;
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
    public class AddCoworker : DualSimFromList, ICareerOption
    {
        public override string GetTitlePrefix()
        {
            return "AddCoworker";
        }

        protected override string GetTitleA()
        {
            return Common.Localize("ChooseBoss:Employee");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("ChangeCoworker:Coworker");
        }

        protected override int GetMaxSelectionB(IMiniSimDescription miniSim)
        {
            SimDescription sim = miniSim as SimDescription;
            if (sim == null) return 0;

            if (sim.Occupation == null) return 0;

            int count = 0;
            if (sim.Occupation.Coworkers != null)
            {
                count = sim.Occupation.Coworkers.Count;
            }

            return (sim.Occupation.MaxCoworkers - count);
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.Occupation == null) return false;

            if (IsFirst)
            {
                if (me.Occupation.Coworkers == null) return false;

                if (me.Occupation.Coworkers.Count >= me.Occupation.MaxCoworkers) return false;
            }

            return true;
        }

        protected override bool PrivateAllow(SimDescription a, SimDescription b)
        {
            if (!base.PrivateAllow(a, b)) return false;

            if (b.Occupation == null) return false;

            if (a.Occupation.Guid != b.Occupation.Guid) return false;

            if (a.Occupation.Boss == b) return false;

            if (a.Occupation.Coworkers != null)
            {
                if (a.Occupation.Coworkers.Contains(b)) return false;
            }

            return true;
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            a.Occupation.AddCoworker(b);

            if (a.CreatedSim == Sim.ActiveActor)
            {
                (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnCareerUpdated();
            }

            return true;
        }
    }
}
