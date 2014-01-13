using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.Households;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Object
{
    public class RabbitHoleOwnership : OptionItem, IObjectOption
    {
        public override string GetTitlePrefix()
        {
            return "Ownership";
        }

        public override string HotkeyID
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!MasterController.Settings.mMenuVisibleOwnership) return false;

            RabbitHole hole = parameters.mTarget as RabbitHole;
            if (hole == null) return false;

            if (!hole.RabbitHoleTuning.kCanInvestHere) return false;

 	        return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            RabbitHole hole = parameters.mTarget as RabbitHole;

            List<Item> allOptions = new List<Item>();

            foreach (Household house in Household.sHouseholdList)
            {
                if (house.RealEstateManager == null) continue;

                PropertyData data = house.RealEstateManager.FindProperty(hole);
                if (data == null) continue;

                RealEstate.OwnerType type = RealEstate.OwnerType.Partial;

                if (data.IsFullOwner)
                {
                    type = RealEstate.OwnerType.Full;
                }

                allOptions.Add(new Item(house, hole, type));
            }

            if (allOptions.Count == 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Failure"));
                return OptionResult.Failure;
            }

            CommonSelection<Item>.Results choices = new CommonSelection<Item>(Name, allOptions).SelectMultiple();
            if ((choices == null) || (choices.Count == 0)) return OptionResult.Failure;

            foreach (Item item in choices)
            {
                item.Perform();
            }

            return OptionResult.SuccessClose;
        }

        public class Item : RealEstate.RabbitHoleItem
        {
            Household mHouse;

            public Item(Household house, RabbitHole hole, RealEstate.OwnerType type)
                : base(hole, type)
            {
                mHouse = house;
                mName = TownFamily.GetQualifiedName(house);
            }

            public bool Perform()
            {
                return Perform(mHouse);
            }
        }
    }
}
