using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
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
    public interface IInteractionOptionList<TTarget> : IInteractionOptionItem<IActor, TTarget, GameHitParameters< TTarget>>, IInteractionProxy<IActor, TTarget, GameHitParameters< TTarget>>
        where TTarget : class, IGameObject
    {
        List<IInteractionOptionItem<IActor, TTarget, GameHitParameters< TTarget>>> IOptions();
    }

    public abstract class PopupOptionList<TOption, TTarget> : CommonOptionList<TOption>, IInteractionOptionList<TTarget>, ITitlePrefixOption
        where TOption : class, IInteractionOptionItem<IActor, TTarget, GameHitParameters< TTarget>>
        where TTarget : class, IGameObject
    {
        public PopupOptionList()
        { }
        public PopupOptionList(string name)
            : base(name)
        { }

        public abstract string GetTitlePrefix();

        public abstract ITitlePrefixOption ParentListingOption
        {
            get;
        }

        public override string DisplayValue
        {
            get { return null; }
        }

        public virtual string ExportName
        {
            get
            {
                string name = GetTitlePrefix();

                ITitlePrefixOption parent = ParentListingOption;
                if (parent != null)
                {
                    name = parent.ExportName + name;
                }

                return name;
            }
        }

        public override string Name
        {
            get
            {
                string title = GetTitlePrefix();
                if (string.IsNullOrEmpty(title))
                {
                    return mName;
                }

                if (Localization.HasLocalizationString(title + ":Title"))
                {
                    title += ":Title";
                }
                else
                {
                    title += ":MenuName";
                }

                return Common.Localize(title);
            }
        }

        public static void Exception(IActor a, object b, Exception e)
        {
            if (b is SimDescription)
            {
                Sim sim = a as Sim;
                if (sim != null)
                {
                    Common.Exception(sim.SimDescription, b as SimDescription, e);
                    return;
                }
            }
            else if (b is IScriptLogic)
            {
                Common.Exception(a, b as IScriptLogic, e);
                return;
            }

            Common.StringBuilder text = new Common.StringBuilder();
            if (b != null)
            {
                text += b.ToString();
            }

            Common.Exception(a, null, text, e);
        }

        public List<IInteractionOptionItem<IActor, TTarget, GameHitParameters< TTarget>>> IOptions()
        {
            List<IInteractionOptionItem<IActor, TTarget, GameHitParameters< TTarget>>> results = new List<IInteractionOptionItem<IActor, TTarget, GameHitParameters< TTarget>>>();

            foreach (TOption option in GetOptions())
            {
                results.Add(option);
            }

            return results;
        }

        public override List<TOption> GetOptions()
        {
            return AllOptions();
        }

        public bool Test(GameHitParameters< TTarget> parameters)
        {
            try
            {
                return Allow(parameters);
            }
            catch (Exception e)
            {
                GameHitParameters< TTarget>.Exception(parameters, e);
                return false;
            }
        }

        protected virtual bool Allow(GameHitParameters< TTarget> parameters)
        {
            IEnumerable<TOption> options = GetOptions();
            if (options == null) return true;

            foreach (TOption option in options)
            {
                if (option.Test(parameters)) return true;
            }

            return false;
        }

        public virtual OptionResult Perform(GameHitParameters< TTarget> parameters)
        {
            throw new NotImplementedException();
        }

        protected virtual OptionResult Run(TOption option, GameHitParameters< TTarget> parameters)
        {
            return option.Perform(parameters);
        }

        public OptionResult Perform(IInteractionOptionItem<IActor, TTarget, GameHitParameters< TTarget>> option, GameHitParameters< TTarget> parameters)
        {
            try
            {
                return Run(option as TOption, parameters);
            }
            catch (Exception e)
            {
                GameHitParameters< TTarget>.Exception(parameters, e);
                return OptionResult.Failure;
            }
        }

        public class AllList : PopupOptionList<TOption, TTarget>
        {
            bool mSingleSelection;

            public AllList(string name, bool singleSelection)
                : base(name)
            {
                mSingleSelection = singleSelection;
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
