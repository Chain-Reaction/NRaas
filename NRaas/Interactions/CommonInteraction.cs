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
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Interactions
{
    public abstract class CommonInteraction<TOption, TTarget> : BaseInteraction<TTarget>
        where TOption : class, IInteractionOptionItem<IActor,TTarget,GameHitParameters<TTarget>>
        where TTarget : class, IGameObject
    {
        protected virtual bool SingleSelection
        {
            get { return true; }
        }

        protected abstract OptionResult Perform(IActor actor, TTarget target, GameObjectHit hit);

        public override bool Run()
        {
            try
            {
                IPerform definition = InteractionDefinition as IPerform;

                return (definition.Perform(this, Actor, Target, Hit) != OptionResult.Failure);
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        protected interface IPerform
        {
            OptionResult Perform(CommonInteraction<TOption, TTarget> interaction, IActor actor, TTarget target, GameObjectHit hit);
        }

        public class UIMouseEventArgsEx : UIMouseEventArgs
        {
            public UIMouseEventArgsEx()
            {
                Vector2 position = UIManager.GetCursorPosition();

                base.mF1 = position.x;
                base.mF2 = position.y;
            }
        }

        [DoesntRequireTuning]
        public class CommonDefinition<INTERACTION> : BaseDefinition<INTERACTION>, IPerform
            where INTERACTION : CommonInteraction<TOption, TTarget>, IImmediateInteraction, new()
        {
            protected IInteractionOptionItem<IActor, TTarget, GameHitParameters< TTarget>> mOption;

            static readonly INTERACTION sTest = new INTERACTION();

            static List<IInteractionOptionItem<IActor, TTarget, GameHitParameters<TTarget>>> sPopupOptions;

            IInteractionProxy<IActor, TTarget, GameHitParameters< TTarget>> mProxy;

            bool mPopup;

            bool mAddRoot;

            public CommonDefinition()
                : this(false, false)
            { }
            public CommonDefinition(bool popup, bool addRoot)
                : this(null, null, popup, new string[0])
            {
                mAddRoot = addRoot;
            }
            private CommonDefinition(IInteractionProxy<IActor, TTarget, GameHitParameters<TTarget>> proxy, IInteractionOptionItem<IActor, TTarget, GameHitParameters<TTarget>> option, bool popup, string[] path)
                : base(path)
            {
                mProxy = proxy;
                mOption = option;
                mPopup = popup;
            }

            public static void UnloadPopupOptions()
            {
                if (sPopupOptions != null)
                {
                    sPopupOptions.Clear();
                }
                sPopupOptions = null;
            }

            public override void AddInteractions(InteractionObjectPair iop, IActor actor, TTarget target, List<InteractionObjectPair> results)
            {
                try
                {
                    string[] defPath = mPath;
                    if ((defPath == null) || (defPath.Length == 0))
                    {
                        if (mAddRoot)
                        {
                            defPath = new string[] { "NRaas", sTest.GetInteractionName(actor, target, mHit) };
                        }
                        else
                        {
                            defPath = new string[] { "NRaas" };
                        }
                    }

                    if ((mPopup) || ((VersionStamp.sPopupMenuStyle)) && (iop == null))
                    {
                        List<string> path = null;
                        if (mPopup)
                        {
                            path = new List<string>(defPath);
                        }
                        else
                        {
                            path = new List<string>();
                        }

                        if (sPopupOptions == null)
                        {
                            sPopupOptions = new List<IInteractionOptionItem<IActor, TTarget, GameHitParameters<TTarget>>>();

                            foreach (TOption option in CommonOptionList<TOption>.AllOptions())
                            {
                                sPopupOptions.Add(option.Clone() as TOption);
                            }
                        }

                        ListOptions(null, sPopupOptions, actor, target, mHit, mPopup, path, results);
                    }
                    else
                    {
                        mPath = defPath;

                        base.AddInteractions(iop, actor, target, results);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(actor, target, e);
                }
            }

            private static void ListOptions(IInteractionProxy<IActor, TTarget, GameHitParameters<TTarget>> proxy, IEnumerable<IInteractionOptionItem<IActor, TTarget, GameHitParameters< TTarget>>> options, IActor actor, TTarget target, GameObjectHit hit, bool popup, List<string> path, List<InteractionObjectPair> results)
            {
                GameHitParameters< TTarget> parameters = new GameHitParameters< TTarget>(actor, target, hit);

                foreach (IInteractionOptionItem<IActor, TTarget, GameHitParameters<TTarget>> option in options)
                {
                    if (!option.Test(parameters)) continue;

                    IInteractionOptionList<TTarget> list = option as IInteractionOptionList<TTarget>;
                    if (list != null)
                    {
                        List<string> newPath = new List<string>(path);
                        newPath.Add(option.Name);

                        ListOptions(list, list.IOptions(), actor, target, hit, popup, newPath, results);
                    }
                    else
                    {
                        results.Add(new InteractionObjectPair(new CommonDefinition<INTERACTION>(proxy, option, popup, path.ToArray()), target));
                    }
                }
            }

            public override bool Test(IActor a, TTarget target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!sTest.Test(a, target, mHit, ref greyedOutTooltipCallback)) return false;

                return (!isAutonomous);
            }

            public OptionResult Perform(CommonInteraction<TOption, TTarget> interaction, IActor actor, TTarget target, GameObjectHit hit)
            {
                if (mOption == null)
                {
                    if ((mPopup) || (VersionStamp.sPopupMenuStyle))
                    {
                        List<InteractionObjectPair> interactions = new List<InteractionObjectPair>();
                        AddInteractions(null, actor, target, interactions);

                        Sims3.Gameplay.UI.PieMenu.TestAndBringUpPieMenu(actor, new UIMouseEventArgsEx(), hit, interactions, InteractionMenuTypes.Normal);
                        return OptionResult.SuccessClose;
                    }
                    else
                    {
                        return interaction.Perform(actor, target, hit);
                    }
                }
                else if (mProxy != null)
                {
                    return mProxy.Perform(mOption, new GameHitParameters< TTarget>(actor, target, hit));
                }
                else
                {
                    return mOption.Perform(new GameHitParameters< TTarget>(actor, target, hit));
                }
            }

            public override string GetInteractionName(IActor actor, TTarget target, InteractionObjectPair iop)
            {
                try
                {
                    if (mOption == null)
                    {
                        return sTest.GetInteractionName(actor, target, mHit);
                    }
                    else
                    {
                        return mOption.ToString();
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(actor, target, e);
                    return "Exception";
                }
            }
        }
    }
}
