using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Interactions
{
    public class SetLotAddress : CommonInteraction<InteractionOptionItem<IActor, GameObject, GameHitParameters<GameObject>>, GameObject>
    {
        public class CustomInjector : Common.InteractionInjector<GameObject>
        {
            public CustomInjector()
                : base(Singleton)
            {
            }
            public override List<Type> GetTypes()
            {
                return new List<Type>
				{
					typeof(Lot),
					typeof(Terrain),
					typeof(BuildableShell)
				};
            }
        }

        public static InteractionDefinition Singleton = new CommonDefinition<SetLotAddress>();

        public Lot GetLot(GameObject target, GameObjectHit hit)
        {
            Lot lot = target as Lot;
            if (lot != null) return lot;

            if (target != null && target.LotCurrent != null) return target.LotCurrent;

            return hit != null ? LotManager.GetLotAtPoint(hit.mPoint) : null;
        }

        public string GetPrompt(Lot lot, Household house, string defAddress)
        {
            string prompt = "";
            if (house != null && !string.IsNullOrEmpty(house.Name)) prompt += house.Name;

            string lotName = lot.Name;
            if (!string.IsNullOrEmpty(lotName))
            {
                if (prompt != "") prompt += " - ";

                prompt += lotName;
            }
            if (prompt != "") prompt += "\n";

            prompt += /*Common.Localize ("LotAddress:DefaultAddress") +*/ defAddress;
            if (lot.Address != defAddress) prompt += "\n" /*+ Common.Localize ("LotAddress:CurrentAddress")*/ + lot.Address;

            return prompt;
        }

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddCustom(new CustomInjector());
        }

        protected override bool Test(IActor actor, GameObject target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            Lot lot = GetLot(target, hit);

            return lot != null && !lot.IsWorldLot && (lot.IsResidentialLot || lot.LastDisplayedLevel >= 0);
        }

        public override string GetInteractionName()
        {
            return "Set Lot Address";
        }

        protected override OptionResult Perform(IActor actor, GameObject target, GameObjectHit hit)
        {
            Lot lot = GetLot(target, hit);

            if (lot == null) return OptionResult.Failure;

            string backupAddress = "";
            string key = World.GetLotAddressKey(lot.LotId);
            if (!string.IsNullOrEmpty(key) && Localization.HasLocalizationString(key))
            {
                backupAddress = Common.LocalizeEAString(key);
            }
            string text = StringInputDialog.Show(GetInteractionName(), GetPrompt(lot, lot.Household, backupAddress), lot.Address, StringInputDialog.Validation.NoneAllowEmptyOK);

            if (text == null || text == lot.Address) return OptionResult.Failure;

            if (text == "" || text == backupAddress)
            {
                Tagger.Settings.mAddresses.Remove(lot.LotId);
            }
            else
            {
                Tagger.Settings.AddAddress(lot.LotId, text);
            }
            lot.mAddressLocalizationKey = text;
            return OptionResult.SuccessClose;
        }
    }
}