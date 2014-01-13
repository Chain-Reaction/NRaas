using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class LotByID : HouseholdFromList, IHouseholdOption
    {
        public override string GetTitlePrefix()
        {
            return "LotByID";
        }

        protected override OptionResult Run(Lot myLot, Household house)
        {
            try
            {
                string text = StringInputDialog.Show(Name, "Enter the ID for the lot:", "");
                if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

                ulong lotID = ulong.Parse(text);

                Lot lot = LotManager.GetLot(lotID);
                if (lot == null)
                {
                    SimpleMessageDialog.Show(Name, "No lot found");
                }
                else
                {
                    Focus.Perform(lot);

                    SimpleMessageDialog.Show(Name, lot.Name + " " + lot.Address);
                }
            }
            catch (Exception e)
            {
                Common.Exception(myLot, e);
            }
            return OptionResult.SuccessClose;
        }
    }
}
