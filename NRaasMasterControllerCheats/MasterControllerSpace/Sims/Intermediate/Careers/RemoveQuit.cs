using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
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
    public class RemoveQuit : SimFromList, ICareerOption
    {
        public override string GetTitlePrefix()
        {
            return "RemoveQuit";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return false;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CareerManager == null) return false;

            if (me.CareerManager.QuitCareers == null) return false;

            return (me.CareerManager.QuitCareers.Count > 0);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            List<SimCareer.Item> allOptions = new List<SimCareer.Item>();

            allOptions.Add(new SimCareer.Item(OccupationNames.Undefined, Common.LocalizeEAString("Ui/Caption/ObjectPicker:All"), 0));

            foreach (Occupation career in me.CareerManager.QuitCareers.Values)
            {
                allOptions.Add(new SimCareer.Item(career.Guid, career.CareerName, 0));
            }

            CommonSelection<SimCareer.Item>.Results choices = new CommonSelection<SimCareer.Item>(Name, me.FullName, allOptions).SelectMultiple();
            if ((choices == null) || (choices.Count == 0)) return false;

            foreach (SimCareer.Item item in choices)
            {
                if (item.Value == OccupationNames.Undefined)
                {
                    me.CareerManager.QuitCareers.Clear();
                    break;
                }
                else
                {
                    me.CareerManager.QuitCareers.Remove(item.Value);
                }
            }

            return true;
        }
    }
}
