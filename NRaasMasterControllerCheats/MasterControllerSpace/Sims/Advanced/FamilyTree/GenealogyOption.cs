using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced.FamilyTree
{
    public abstract class GenealogyOption : DualSimFromList, IGenealogyOption
    {
        public GenealogyOption()
        {}

        protected override bool TestValid
        {
            get { return false; }
        }

        protected override bool PrivateAllow(IMiniSimDescription a, SimDescription b)
        {
            if (!base.PrivateAllow(a, b)) return false;

            if (!Fixup(a as SimDescription)) return false;
            if (!Fixup(b)) return false;

            return Allow(a.CASGenealogy as Genealogy, b.Genealogy);
        }

        protected override bool PrivateAllow(IMiniSimDescription a, MiniSimDescription b)
        {
            if (!Fixup(a as SimDescription)) return false;

            return Allow(a.CASGenealogy as Genealogy, b.Genealogy);
        }

        protected abstract bool Allow(Genealogy a, Genealogy b);

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            return Allow(me.Genealogy);
        }

        protected override bool PrivateAllow(MiniSimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            return Allow(me.Genealogy);
        }

        protected abstract bool Allow(Genealogy me);

        protected override bool PrivateRun(IMiniSimDescription a, SimDescription b)
        {
            if (!Fixup(a as SimDescription)) return false;
            if (!Fixup(b)) return false;

            return Run(a.CASGenealogy as Genealogy, b.Genealogy);
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            if (!Fixup(a)) return false;
            if (!Fixup(b)) return false;

            return Run(a.Genealogy, b.Genealogy);
        }

        protected override bool PrivateRun(IMiniSimDescription a, MiniSimDescription b)
        {
            if (!Fixup(a as SimDescription)) return false;

            return Run(a.CASGenealogy as Genealogy, b.Genealogy);
        }

        protected override bool Run(SimDescription a, MiniSimDescription b)
        {
            if (!Fixup(a)) return false;

            return Run(a.Genealogy, b.Genealogy);
        }

        protected abstract bool Run(Genealogy a, Genealogy b);

        protected static bool Fixup(SimDescription me)
        {
            if (me == null) return true;

            if (!me.IsValidDescription)
            {
                me.Fixup();
            }

            return (me.Genealogy != null);
        }
    }
}
