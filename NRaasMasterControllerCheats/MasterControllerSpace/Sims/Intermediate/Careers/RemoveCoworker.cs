using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
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
using Sims3.Gameplay.Interfaces;
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
    public class RemoveCoworker : DualSimFromList, ICareerOption
    {
        public override string GetTitlePrefix()
        {
            return "RemoveCoworker";
        }

        protected override string GetTitleA()
        {
            return Common.Localize("ChooseBoss:Employee");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("ChangeCoworker:Coworker");
        }

        protected override ICollection<SimSelection.ICriteria> GetCriteriaB(GameHitParameters<GameObject> parameters)
        {
            return null;
        }

        protected override int GetMaxSelectionB(IMiniSimDescription sim)
        {
            return 0;
        }

        protected override bool TestValid
        {
            get { return false; }
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (IsFirst)
            {
                if (me.Occupation == null) return false;

                if (me.Occupation.Coworkers == null) return false;

                if (me.Occupation.Coworkers.Count <= 0) return false;
            }

            return true;
        }

        protected override bool PrivateAllow(SimDescription a, SimDescription b)
        {
            if (!base.PrivateAllow(a, b)) return false;

            if (a.Occupation.Coworkers != null)
            {
                if (!a.Occupation.Coworkers.Contains(b)) return false;
            }

            return true;
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            a.Occupation.RemoveCoworker(b);

            if (a.CreatedSim == Sim.ActiveActor)
            {
                (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnCareerUpdated();
            }

            return true;
        }
    }
}
