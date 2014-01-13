using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
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
    public abstract class CareerOption : SimFromList, ICareerOption
    {
        protected void EnsureCoworkerLists(SimDescription sim)
        {
            if (sim.Occupation == null) return;

            if (sim.Occupation.CareerLoc != null)
            {
                foreach (SimDescription worker in sim.Occupation.CareerLoc.Workers)
                {
                    if (worker.Occupation == null) continue;

                    if (worker.Occupation.Coworkers == null)
                    {
                        worker.Occupation.Coworkers = new List<SimDescription>();
                    }
                }
            }
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            return (me.CareerManager != null);
        }
    }
}
