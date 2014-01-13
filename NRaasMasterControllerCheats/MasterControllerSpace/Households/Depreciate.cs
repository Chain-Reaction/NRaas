using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Fireplaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Insect;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class Depreciate : HouseholdFromList, IHouseholdOption
    {
        int mValue = 0;

        public override string GetTitlePrefix()
        {
            return "DepreciateHouse";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            if (me == Household.ActiveHousehold) return false;

            if (lot == null) return false;

            if (lot is WorldLot) return false;

            return (true);
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            if (lot == null) return OptionResult.Failure;

            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize("DepreciateHouse:Prompt"), "0", 256, StringInputDialog.Validation.None);
                if ((text == null) || (text == "")) return OptionResult.Failure;

                mValue = 0;
                if (!int.TryParse(text, out mValue))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return OptionResult.Failure;
                }
            }

            if ((lot != null) && (lot.IsResidentialLot))
            {
                int oldCost = lot.CalculateFurnitureWorth ();

                int baseCost = lot.Cost - oldCost;

                Dictionary<string,int> objects = new Dictionary<string,int>();

                foreach (GameObject obj in lot.GetObjects<GameObject> ())
                {
                    if (obj is AbstractArtObject) continue;
                    
                    if (obj is Fireplace) continue;
                    
                    if (obj is ImageObject) continue;

                    if (obj is Terrarium) continue;

                    obj.ValueModifier -= (int) (obj.PurchasedPrice * (mValue / 100f));

                    if (!objects.ContainsKey(obj.CatalogName))
                    {
                        objects.Add(obj.CatalogName, 1);
                    }
                    else
                    {
                        objects[obj.CatalogName]++;
                    }
                }

                int newCost = lot.CalculateFurnitureWorth();

                Common.Notify(Common.Localize("DepreciateHouse:Success", false, new object[] { lot.Name, baseCost + oldCost, baseCost + newCost }));
            }

            return OptionResult.SuccessClose;
        }
    }
}
