using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Actions;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Locations;
using Sims3.Gameplay.RealEstate;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Settings
{
    public class DropInvalidVacationHomes : OperationSettingOption<GameObject>, IActionOption
    {
        public override string GetTitlePrefix()
        {
            return "DropInvalidVacationHomes";
        }        

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Overwatch.Log("DropInvalidVactionHomes");

            WorldName currentWorld = GameUtils.GetCurrentWorld();
            List<WorldName> found = new List<WorldName>();

            foreach (MiniSimDescription desc in MiniSimDescription.sMiniSims.Values)
            {
                if (desc.HomeWorld != WorldName.UserCreated && desc.HomeWorld != currentWorld && !found.Contains(desc.HomeWorld))
                {
                    found.Add(desc.HomeWorld);
                }
            }

            if(Household.ActiveHousehold == null || Household.ActiveHousehold.RealEstateManager == null)
            {
                Common.Notify("Fail");
            }

            foreach (PropertyData data in Household.ActiveHousehold.RealEstateManager.AllProperties)
            {
                if (data.PropertyType != RealEstatePropertyType.VacationHome) continue;

                if (found.Contains(data.World))
                {
                    Overwatch.Log("Skipping " + data.World);
                    continue;
                }

                string na = null;
                string msg = data.World == WorldName.UserCreated ? "Unhandable PropertyData found" : "Possible invalid PropertyData found";

                if(TwoButtonDialog.Show(msg + ": PropertyName: " + (data.LocalizedName != string.Empty ? data.LocalizedName : "N/A") + " Value: " + data.StoredValue + " World: " + Sims3.Gameplay.Objects.HobbiesSkills.Photograph.GameUtilsGetLocalizedWorldName(data.World, ref na), "Yes", "No"))
                {
                    try
                    {
                        data.Owner.SellProperty(data, false);

                        LocationHomeDeed[] deeds = Sims3.Gameplay.Queries.GetObjects<LocationHomeDeed>();
                        foreach (LocationHomeDeed deed in deeds)
                        {
                            if (deed.LotId == data.LotId && deed.World == data.World)
                            {
                                Sim owner = deed.ItemComp.InventoryParent.Owner as Sim;
                                if (owner != null)
                                {
                                    owner.Inventory.RemoveByForce(deed);
                                }
                                deed.Destroy();
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        Common.Exception("", e);
                    }
                }
            }

            return OptionResult.SuccessRetain;
        }
    }
}