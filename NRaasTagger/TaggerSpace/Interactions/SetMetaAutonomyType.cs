using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.TaggerSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Interactions
{
    public class SetMetaAutonomyType : CommonInteraction<InteractionOptionItem<IActor, GameObject, GameHitParameters<GameObject>>, GameObject>
    {
        public class Item : ValueSettingOption<Lot.MetaAutonomyType>
        {
            public Item()
            { }
            public Item(Lot.MetaAutonomyType type)
                : base(type, type.ToString(), 0)
            { }            

            public override bool UsingCount
            {
                get
                {
                    return false;
                }
            }
        }

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

        public static InteractionDefinition Singleton = new CommonDefinition<SetMetaAutonomyType>();

        public Lot GetLot(GameObject target, GameObjectHit hit)
        {
            Lot lot = target as Lot;
            if (lot != null) return lot;

            if (target != null && target.LotCurrent != null) return target.LotCurrent;

            return hit != null ? LotManager.GetLotAtPoint(hit.mPoint) : null;
        }       

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddCustom(new CustomInjector());
        }

        protected override bool Test(IActor actor, GameObject target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            Lot lot = GetLot(target, hit);

            return Tagger.Settings.mEnableLotInteractions && lot != null && !lot.IsWorldLot && lot.IsCommunityLot;            
        }

        public override string GetInteractionName()
        {
            return Common.Localize("SetMetaAutonomyType:MenuName");
        }

        protected override OptionResult Perform(IActor actor, GameObject target, GameObjectHit hit)
        {
            Lot lot = GetLot(target, hit);

            if (lot == null) return OptionResult.Failure;

            uint type = (uint)lot.CommercialLotSubType;

            Common.Notify("Current Type: " + lot.mMetaAutonomyType.ToString());

            List<Item> allOptions = new List<Item>();

            foreach (Lot.MetaAutonomyType metaAutoType in Enum.GetValues(typeof(Lot.MetaAutonomyType)))
            {
                allOptions.Add(new Item(metaAutoType));
            }

            Item selection = new CommonSelection<Item>(lot.GetLocalizedName(), allOptions).SelectSingle();
            if (selection == null) return OptionResult.Failure;            

            if (!Tagger.Settings.TypeHasCustomSettings(type))
            {
                TagSettingKey key = new TagSettingKey();
                key.MetaAutonomyType = (uint)selection.Value;
                Tagger.Settings.mCustomTagSettings.Add(type, key);
            }
            else
            {
                Tagger.Settings.mCustomTagSettings[type].MetaAutonomyType = (uint)selection.Value;
            }

            Tagger.SetMetaAutonomyType(lot, selection.Value);

            Common.Notify(GetInteractionName() + ": " + selection.Value);
            return OptionResult.SuccessClose;
        }
    }
}