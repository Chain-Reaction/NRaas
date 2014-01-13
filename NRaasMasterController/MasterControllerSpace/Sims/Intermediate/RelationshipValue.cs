using NRaas.MasterControllerSpace.Dialogs;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class RelationshipValue : DualSimFromList, IIntermediateOption
    {
        float mValue = 0;

        public override string GetTitlePrefix()
        {
            return "RelationshipByValue";
        }

        protected override string GetTitleA()
        {
            return Common.Localize("Relationship:SimA");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("Relationship:SimB");
        }

        protected override int GetMaxSelectionA()
        {
            return 0;
        }

        protected override int GetMaxSelectionB(IMiniSimDescription sim)
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AllowSpecies(IMiniSimDescription a, IMiniSimDescription b)
        {
            return true;
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            Relationship relation = Relationship.Get(a, b, true);
            if (relation == null) return true;

            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize("RelationshipByValue:Prompt", a.IsFemale, new object[] { a, b }), relation.LTR.Liking.ToString ());
                if (string.IsNullOrEmpty(text)) return false;

                mValue = 0;
                if (!float.TryParse(text, out mValue))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }
            }

            relation.LTR.SetLiking(mValue);
            return true;
        }
    }
}
