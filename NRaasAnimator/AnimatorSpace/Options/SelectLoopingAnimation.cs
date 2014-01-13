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
    public class SelectLoopingAnimation : OperationSettingOption<Sim>, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "AnimationLoopByKey";
        }

        protected override bool Allow(GameHitParameters< Sim> parameters)
        {
            if (parameters.mTarget.InteractionQueue == null) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            Item item = new ItemSelection(parameters.mTarget).SelectSingle();
            if (item == null) return OptionResult.Failure;

            parameters.mTarget.InteractionQueue.AddNext(item.mDefinition.CreateInstance(parameters.mTarget, parameters.mTarget, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));
            return OptionResult.SuccessClose;
        }

        public class ItemSelection : ProtoSelection<Item>
        {
            public ItemSelection(Sim target)
                : base(Common.Localize("AnimationLoopByKey:MenuName"), target.FullName, Item.GetOptions(target))
            { 
               AddColumn(new KeyColumn());
               AddColumn(new TypeColumn());
            }

            protected class KeyColumn : ObjectPickerDialogEx.CommonHeaderInfo<Item>
            {
                public KeyColumn()
                    : base("NRaas.Animator.OptionList:OptionTitle", "NRaas.Animator.OptionList:OptionTooltip", 350)
                { }

                public override ObjectPicker.ColumnInfo GetValue(Item item)
                {
                    string name = item.Value;

                    if (name.Contains(item.Name))
                    {
                        name = item.Name;
                    }
                    if (name != item.Name)
                    {
                        name += " (" + item.Name + ")";
                    }

                    return new ObjectPicker.TextColumn(name);
                }
            }

            protected class TypeColumn : ObjectPickerDialogEx.CommonHeaderInfo<Item>
            {
                public TypeColumn()
                    : base("NRaas.Animator.OptionList:Type", "NRaas.Animator.OptionList:Type", 25)
                { }

                public override ObjectPicker.ColumnInfo GetValue(Item item)
                {
                    return new ObjectPicker.TextColumn(item.mType);
                }
            }
        }

        public class Item : ValueSettingOption<string>
        {
            public readonly string mType;

            public readonly InteractionDefinition mDefinition;

            public Item(string name, string key, string type, InteractionDefinition definition)
                : base(key, Convert(name), 0)
            {
                mType = type;
                mDefinition = definition;
            }

            public static string Convert(string value)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    value = value.Replace(Common.NewLine, " ");
                    
                    if (value.Length > 50)
                    {
                        value = value.Substring(0, 50);
                    }
                }

                return value;
            }

            public static List<InteractionObjectPair> GetInteractions(Sim target)
            {
                List<InteractionObjectPair> results = new List<InteractionObjectPair>();

                List<InteractionObjectPair> list = new List<InteractionObjectPair>();

                if (target.IsHuman)
                {
                    new Sim.PlaySpecificLoopingAnimation.Definition().AddInteractions(null, target, target, list);

                    foreach (InteractionObjectPair pair in list)
                    {
                        PlaySpecificLoopingAnimationEx.Definition definition = new PlaySpecificLoopingAnimationEx.Definition(pair.InteractionDefinition as Sim.PlaySpecificLoopingAnimation.Definition);

                        if (!GameUtils.IsInstalled(definition.mProductVersion)) continue;

                        if (!Sim.AnimationClipDataForCAS.SimCanPlayAnimation(target, definition.ClipName)) continue;

                        results.Add(new InteractionObjectPair(definition, target));
                    }
                }

                list.Clear();
                new Sim.PlaySpecificIdle.Definition().AddInteractions(null, target, target, list);

                CASAGSAvailabilityFlags flags = target.SimDescription.GetCASAGSAvailabilityFlags();

                foreach (InteractionObjectPair pair in list)
                {
                    PlaySpecificIdleEx.Definition definition = new PlaySpecificIdleEx.Definition(pair.InteractionDefinition as Sim.PlaySpecificIdle.Definition);

                    if ((definition.Idle.AgeSpeciesRestrictions & flags) == flags) continue;

                    results.Add(new InteractionObjectPair(definition, target));
                }

                if (target.IsHuman)
                {
                    foreach (KeyValuePair<string, Sim.AnimationClipDataForCAS[]> animations in Sim.AnimationClipDataForCAS.sCasAnimations)
                    {
                        string name = null;

                        switch (animations.Key)
                        {
                            case "CasFullBodyAnimations":
                                name = Common.LocalizeEAString("Gameplay/Actors/Sim:Cheat_FullBodyAnimationSuite");
                                break;
                            case "CasFaceAnimations":
                                name = Common.LocalizeEAString("Gameplay/Actors/Sim:Cheat_FaceAnimationSuite");
                                break;
                            default:
                                name = animations.Key;
                                break;
                        }

                        foreach (Sim.AnimationClipDataForCAS rcas in animations.Value)
                        {
                            if (!Sim.AnimationClipDataForCAS.SimCanPlayAnimation(target, rcas.AnimationClipName)) continue;

                            PlaySuiteLoopingAnimationEx.Definition definition = new PlaySuiteLoopingAnimationEx.Definition(rcas, name);

                            results.Add(new InteractionObjectPair(definition, target));
                        }
                    }
                }

                return results;
            }

            public static List<Item> GetOptions(Sim target)
            {
                List<Item> items = new List<Item>();

                Dictionary<string, bool> existing = new Dictionary<string, bool>();

                List<InteractionObjectPair> results = GetInteractions(target);

                foreach (InteractionObjectPair pair in results)
                {
                    IAnimationDefinition animation = pair.InteractionDefinition as IAnimationDefinition;
                    if (string.IsNullOrEmpty(animation.ClipName))
                    {
                        continue;
                    }

                    if (existing.ContainsKey(animation.ClipName))
                    {
                        continue;
                    }

                    existing.Add(animation.ClipName, true);

                    items.Add(new Item(animation.InteractionName, animation.ClipName, animation.Type, pair.InteractionDefinition));
                }

                return items;
            }
        }
   }
}