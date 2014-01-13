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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.AnimatorSpace.Options
{
    public class SelectNestedAnimation : OperationSettingOption<Sim>, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "AnimationLoopNested";
        }

        protected override bool Allow(GameHitParameters< Sim> parameters)
        {
            if (parameters.mTarget.InteractionQueue == null) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            return InteractionDefinitionOptionList.Perform(parameters.mActor, parameters.mHit, SelectLoopingAnimation.Item.GetInteractions(parameters.mTarget));
        }

        public class ItemSelection : ProtoSelection<SelectLoopingAnimation.Item>
        {
            public ItemSelection(Sim target)
                : base(Common.Localize("AnimationLoopNested:MenuName"), target.FullName, SelectLoopingAnimation.Item.GetOptions(target))
            { 
               AddColumn(new KeyColumn());
            }

            protected class KeyColumn : ObjectPickerDialogEx.CommonHeaderInfo<SelectLoopingAnimation.Item>
            {
                public KeyColumn()
                    : base("NRaas.Animator.OptionList:Key", "NRaas.Animator.OptionList:Key", 350)
                { }

                public override ObjectPicker.ColumnInfo GetValue(SelectLoopingAnimation.Item item)
                {
                    string name = item.Name;

                    if (item.Value.Contains(name))
                    {
                        name = item.Value;
                    }
                    if (name != item.Value)
                    {
                        name += " (" + item.Value + ")";
                    }

                    return new ObjectPicker.TextColumn(name);
                }
            }
        }
   }
}