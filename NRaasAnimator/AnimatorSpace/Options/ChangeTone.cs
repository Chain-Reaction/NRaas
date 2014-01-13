using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.AnimatorSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.AnimatorSpace.Options
{
    public class ChangeTone : OperationSettingOption<Sim>, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "ChangeTone";
        }

        protected override bool Allow(GameHitParameters< Sim> parameters)
        {
            if (!base.Allow(parameters)) return false;

            if (parameters.mTarget.InteractionQueue == null) return false;

            return (parameters.mTarget.InteractionQueue.GetCurrentInteraction() is LoopingAnimationBase);
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            Item item = new ItemSelection(parameters.mTarget).SelectSingle();
            if (item == null) return OptionResult.Failure;

            parameters.mTarget.InteractionQueue.GetCurrentInteraction().CurrentTone = item.Value;
            return OptionResult.SuccessClose;
        }

        public class ItemSelection : ProtoSelection<Item>
        {
            public ItemSelection(Sim target)
                : base(Common.Localize("ChangeTone:MenuName"), target.FullName, Item.GetOptions(target))
            { 
               AddColumn(new NameColumn());
            }

            protected class NameColumn : ObjectPickerDialogEx.CommonHeaderInfo<Item>
            {
                public NameColumn()
                    : base("NRaas.Animator.OptionList:Name", "NRaas.Animator.OptionList:Name", 350)
                { }

                public override ObjectPicker.ColumnInfo GetValue(Item item)
                {
                    return new ObjectPicker.TextColumn(item.Name);
                }
            }
        }

        public class Item : ValueSettingOption<Tone>
        {
            public Item(string name, Tone tone)
                : base(tone, name, 0)
            { }

            public static List<Item> GetOptions(Sim target)
            {
                if (target.InteractionQueue == null) return null;

                if (target.InteractionQueue.GetCurrentInteraction() == null) return null;

                List<Item> items = new List<Item>();

                List<InteractionToneDisplay> tones = target.InteractionQueue.GetCurrentInteraction().AvailableTonesForDisplay();

                foreach (InteractionToneDisplay tone in tones)
                {
                    items.Add(new Item(tone.InteractionTone.Name(), tone.InteractionTone as Tone));
                }

                return items;
            }
        }
    }
}