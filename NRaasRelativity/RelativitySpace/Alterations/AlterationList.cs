using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.ChildrenObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace.Alterations
{
    public abstract class AlterationList : IAlteration
    {
        List<IAlteration> mAlterations = new List<IAlteration>();

        public AlterationList()
        { }

        public void Add(IAlteration alteration)
        {
            mAlterations.Add(alteration);
        }

        public void Store()
        {
            foreach (IAlteration alteration in mAlterations)
            {
                alteration.Store();
            }
        }

        public void Revert()
        {
            foreach (IAlteration alteration in mAlterations)
            {
                alteration.Revert();
            }
        }
    }
}
