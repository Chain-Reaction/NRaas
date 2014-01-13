using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Options
{
    public abstract class InteractionOptionList<T, TTarget> : PopupOptionList<T, TTarget>
        where T : class, IInteractionOptionItem<IActor, TTarget, GameHitParameters< TTarget>>
        where TTarget : class, IGameObject
    {
        public InteractionOptionList()
        { }
        public InteractionOptionList(string name)
            : base(name)
        { }

        public override OptionResult Perform(GameHitParameters< TTarget> parameters)
        {
            try
            {
                while (true)
                {
                    IEnumerable<T> allOptions = GetOptions();
                    if (allOptions == null) return OptionResult.Failure;

                    List<T> options = new List<T>();
                    foreach (T option in allOptions)
                    {
                        if (!option.Test(parameters)) continue;

                        options.Add(option);
                    }

                    CommonSelection<T>.Results selection = new CommonSelection<T>(Name, options).SelectMultiple(NumSelectable);
                    if (selection.Count == 0)
                    {
                        if (selection.mOkayed)
                        {
                            return OptionResult.SuccessClose;
                        }
                        else
                        {
                            return OptionResult.Failure;
                        }
                    }

                    bool success = false;
                    bool retain = false;
                    bool levelDown = false;

                    foreach (T item in selection)
                    {
                        switch (Run(item, parameters))
                        {
                            case OptionResult.SuccessClose:
                                success = true;
                                break;
                            case OptionResult.SuccessRetain:
                                retain = true;
                                break;
                            case OptionResult.SuccessLevelDown:
                                levelDown = true;
                                break;
                        }
                    }

                    if (levelDown)
                    {
                        return OptionResult.SuccessRetain;
                    }
                    else if ((success) && (!retain))
                    {
                        return OptionResult.SuccessClose;
                    }
                }
            }
            catch (Exception e)
            {
                GameHitParameters< TTarget>.Exception(parameters, e);
                return OptionResult.Failure;
            }
        }

        public new class AllList : InteractionOptionList<T, TTarget>
        {
            bool mSingleSelection;

            OnGetOptions mOnGetOptions;

            public delegate List<T> OnGetOptions(List<T> options);

            public AllList(string name, bool singleSelection)
                : base(name)
            {
                mSingleSelection = singleSelection;
            }
            public AllList(string name, bool singleSelection, OnGetOptions onGetOptions)
                : base(name)
            {
                mSingleSelection = singleSelection;
                mOnGetOptions = onGetOptions;
            }

            public override List<T> GetOptions()
            {
                if (mOnGetOptions != null)
                {
                    return mOnGetOptions(base.GetOptions());
                }
                else
                {
                    return base.GetOptions();
                }
            }

            public override string GetTitlePrefix()
            {
                return null;
            }

            public override ITitlePrefixOption ParentListingOption
            {
                get { return null; }
            }

            protected override int NumSelectable
            {
                get
                {
                    if (mSingleSelection)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
    }
}
