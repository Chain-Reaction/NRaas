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
    public class ChooseBoss : DualSimFromList, ICareerOption
    {
        public override string GetTitlePrefix()
        {
            return "ChooseBoss";
        }

        protected override string GetTitleA()
        {
            return Common.Localize("ChooseBoss:Employee");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("ChooseBoss:Boss");
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.Occupation == null) return false;

            if (IsFirst)
            {
                Career career = me.Occupation as Career;
                if (career != null)
                {
                    if (career.CurLevel == null) return false;

                    if (!career.CurLevel.HasBoss) return false;
                }
            }

            return true;
        }

        protected override bool PrivateAllow(SimDescription a, SimDescription b)
        {
            if (!base.PrivateAllow(a, b)) return false;

            if (b.Occupation == null) return false;

            if (a.Occupation.Guid != b.Occupation.Guid) return false;

            if (b.Occupation.CareerLevel < Career.kMinimumBossLevel) return false;

            return true;
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            a.Occupation.RemoveCoworker(b);

            a.Occupation.Boss = b;

            if (a.CreatedSim == Sim.ActiveActor)
            {
                (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnCareerUpdated();
            }

            return true;
        }
    }
}
