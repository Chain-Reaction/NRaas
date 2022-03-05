using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.CAS;
using System;

namespace NRaas.CommonSpace.Options
{
    public enum OptionResult
    {
        Unset,
        SuccessRetain,
        SuccessLevelDown,
        SuccessClose,
        Failure,
    }

    public interface IInteractionOptionItem<TActor,TTarget,TParameters> : ICommonOptionItem
        where TParameters : InteractionOptionParameters<TActor,TTarget>
    {
        string GetTitlePrefix();

        bool Test(TParameters parameters);

        OptionResult Perform(TParameters parameters);
    }

    public interface IInteractionProxy<TActor, TTarget, TParameters>
        where TParameters : InteractionOptionParameters<TActor, TTarget>
    {
        OptionResult Perform(IInteractionOptionItem<TActor, TTarget, TParameters> option, TParameters parameters);
    }

    public interface ITitlePrefixOption
    {
        string ExportName
        {
            get;
        }
    }

    public abstract class InteractionOptionParameters<TActor,TTarget>
    {
        public readonly TActor mActor;
        public readonly TTarget mTarget;

        public InteractionOptionParameters(TActor actor, TTarget target)
        {
            mActor = actor;
            mTarget = target;
        }

        protected abstract void PrivateException(Common.StringBuilder msg, Exception e);

        public static void Exception(InteractionOptionParameters<TActor, TTarget> parameters, Exception e)
        {
            Exception(parameters, new Common.StringBuilder(), e);
        }
        public static void Exception(InteractionOptionParameters<TActor, TTarget> parameters, Common.StringBuilder msg, Exception e)
        {
            if (parameters == null)
            {
                Common.Exception(msg, e);
            }
            else
            {
                parameters.PrivateException(msg, e);
            }
        }
    }

    public class MiniSimDescriptionParameters : InteractionOptionParameters<IMiniSimDescription, IMiniSimDescription>
    {
        public MiniSimDescriptionParameters(IMiniSimDescription actor, IMiniSimDescription target)
            : base(actor, target)
        { }

        protected override void PrivateException(Common.StringBuilder msg, Exception e)
        {
            Common.Exception(mActor, mTarget, msg, e);
        }
    }

    public class SimDescriptionParameters : InteractionOptionParameters<SimDescription, SimDescription>
    {
        public SimDescriptionParameters(SimDescription actor, SimDescription target)
            : base(actor, target)
        { }

        protected override void PrivateException(Common.StringBuilder msg, Exception e)
        {
            Common.Exception(mActor, mTarget, msg, e);
        }
    }

    public class GameHitParameters<TTarget> : InteractionOptionParameters<IActor, TTarget>
        where TTarget : class, IGameObject
    {
        public readonly GameObjectHit mHit;

        public GameHitParameters(IActor actor, TTarget target, GameObjectHit hit)
            : base(actor, target)
        {
            mHit = hit;
        }

        protected override void PrivateException(Common.StringBuilder msg, Exception e)
        {
            Common.Exception(mActor, mTarget, msg, e);
        }
    }

    public abstract class InteractionOptionItem<TActor,TTarget,TParameters> : CommonOptionItem, IInteractionOptionItem<TActor,TTarget,TParameters>
        where TParameters : InteractionOptionParameters<TActor, TTarget>
    {
        public InteractionOptionItem()
        { }
        public InteractionOptionItem(string name, int count)
            : base(name, count)
        { }
        public InteractionOptionItem(string name, int count, string icon, ProductVersion version)
            : base(name, count, icon, version)
        { }
        public InteractionOptionItem(string name, int count, ResourceKey icon)
            : base(name, count, icon)
        { }
        public InteractionOptionItem(string name, int count, ThumbnailKey thumbnail)
            : base(name, count, thumbnail)
        { }

        public abstract string GetTitlePrefix();

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

        public bool Test(TParameters parameters)
        {
            try
            {
                if (string.IsNullOrEmpty(Name)) return false;

                return Allow(parameters);
            }
            catch (Exception e)
            {
                InteractionOptionParameters<TActor, TTarget>.Exception(parameters, e);
                return false;
            }
        }

        protected virtual bool Allow(TParameters parameters)
        {
            Reset();

            return true;
        }

        public OptionResult Perform(TParameters parameters)
        {
            Task.Result result = new Task.Result();

            // Used as a method of shortening the stack frame
            new Task(result, this, parameters).AddToSimulator();

            while (result.mResult == OptionResult.Unset)
            {
                Common.Sleep();
            }

            return result.mResult;
        }

        protected abstract OptionResult Run(TParameters parameters);

        public static Lot GetLot(GameObject target, GameObjectHit hit)
        {
            Lot lot = target as Lot;

            if (lot != null)
            {
                if (lot.IsBaseCampLotType) return null;

                return lot;
            }

            if (target is Sims3.Gameplay.Objects.Electronics.Computer) return null;

            if (target is RabbitHole) return null;

            if (target != null)
            {
                if (target.LotCurrent != null) return target.LotCurrent;
            }

            return LotManager.GetLotAtPoint(hit.mPoint);
        }

        public class Task : Common.FunctionTask
        {
            Result mResult;

            InteractionOptionItem<TActor, TTarget, TParameters> mItem;

            TParameters mParameters;

            public Task(Result result, InteractionOptionItem<TActor, TTarget, TParameters> item, TParameters parameters)
            {
                mResult = result;
                mItem = item;
                mParameters = parameters;
            }

            public class Result
            {
                public OptionResult mResult = OptionResult.Unset;
            }

            protected override void OnPerform()
            {
                try
                {
                    mResult.mResult = mItem.Run(mParameters);
                }
                catch (Exception e)
                {
                    InteractionOptionParameters<TActor, TTarget>.Exception(mParameters, new Common.StringBuilder(mItem.ToString()), e);
                    mResult.mResult = OptionResult.Failure;
                }
            }
        }
    }
}
