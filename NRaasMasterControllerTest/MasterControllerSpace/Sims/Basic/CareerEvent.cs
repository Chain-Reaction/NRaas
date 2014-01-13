using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class CareerEvent : SimFromList, IBasicOption
    {
        protected class Item : CommonOptionItem
        {
            public Sims3.Gameplay.Careers.Career.EventDaily mEvent;

            public Sims3.Gameplay.Careers.Career mCareer;

            public Item(Sims3.Gameplay.Careers.Career.EventDaily item, Sims3.Gameplay.Careers.Career career, string name)
                : base (name, 0)
            {
                mEvent = item;
                mCareer = career;
            }

            public override string DisplayValue
            {
                get { return null; }
            }
        }

        public override string GetTitlePrefix()
        {
            return "CareerEvent";
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            if (me.CareerManager == null) return false;

            Sims3.Gameplay.Careers.Career career = me.Occupation as Sims3.Gameplay.Careers.Career;
            if ((career == null) && (me.CareerManager.School == null)) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            try
            {
                List<Item> allOptions = new List<Item>();

                Sims3.Gameplay.Careers.Career career = me.Occupation as Sims3.Gameplay.Careers.Career;
                if (career != null)
                {
                    foreach (Sims3.Gameplay.Careers.Career.EventDaily item in career.CareerEventManager.Events)
                    {
                        if (item is Sims3.Gameplay.Careers.Career.EventOpportunity) continue;

                        if (!item.IsAvailable(career)) continue;

                        allOptions.Add(new Item(item, career, item.EventType.ToString()));
                    }
                }

                if (me.CareerManager.School != null)
                {
                    foreach (Sims3.Gameplay.Careers.Career.EventDaily item in me.CareerManager.School.CareerEventManager.Events)
                    {
                        if (item is Sims3.Gameplay.Careers.Career.EventOpportunity) continue;

                        if (!item.IsAvailable(me.CareerManager.School)) continue;

                        allOptions.Add(new Item(item, me.CareerManager.School, item.EventType.ToString()));
                    }
                }

                if (allOptions.Count == 0)
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("CareerEvent:None"));
                    return false;
                }

                Item selection = new CommonSelection<Item>(Name, me.FullName, allOptions).SelectSingle();
                if (selection == null) return false;

                selection.mEvent.RunEvent(selection.mCareer);
            }
            catch (Exception e)
            {
                Common.Exception(me, e);
            }
            return true;
        }
    }
}
