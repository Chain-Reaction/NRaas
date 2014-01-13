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
using Sims3.UI.Hud;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Options
{
    public interface IInteractionDefinitionOption : IInteractionOptionItem<IActor,GameObject,GameHitParameters<GameObject>>
    { }

    public class InteractionDefinitionOption : OperationSettingOption<GameObject>, IInteractionDefinitionOption
    {
        protected InteractionObjectPair mPair;

        public InteractionDefinitionOption(InteractionObjectPair pair, IActor actor)
            : base(GetInteractionName(pair, actor))
        {
            mPair = pair;

            if (string.IsNullOrEmpty(mName))
            {
                string key = mPair.InteractionDefinition.GetType().FullName;

                mName = Localization.GetLocalizedString(key);
                if (string.IsNullOrEmpty(mName))
                {
                    mName = key.Replace(mPair.InteractionDefinition.GetType().Namespace + ".", "");
                }
            }
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override string DisplayValue
        {
            get { return null; }
        }

        protected static string GetInteractionName(InteractionObjectPair pair, IActor actor)
        {
            try
            {
                InteractionInstanceParameters parameters = new InteractionInstanceParameters(pair, actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);

                return pair.InteractionDefinition.GetInteractionName(ref parameters);
            }
            catch (Exception e)
            {
                Common.Exception(actor, pair.Target, pair.InteractionDefinition.GetType().ToString(), e);
                return pair.InteractionDefinition.GetType().ToString();
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> hitParameters)
        {
            InteractionInstanceParameters parameters = new InteractionInstanceParameters(mPair, hitParameters.mActor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, hitParameters.mHit);

            GreyedOutTooltipCallback greyedOutTooltipCallback = null;
            return IUtil.IsPass(mPair.InteractionDefinition.Test(ref parameters, ref greyedOutTooltipCallback));
        }

        protected override OptionResult Run(GameHitParameters< GameObject> hitParameters)
        {
            InteractionInstanceParameters parameters = new InteractionInstanceParameters(mPair, hitParameters.mActor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true, hitParameters.mHit);

            MenuItem item = new MenuItem();
            mPair.InteractionDefinition.PopulatePieMenuPicker(ref parameters, out item.mListObjs, out item.mHeaders, out item.mNumSelectableRows);

            if ((item.mListObjs != null) && (item.mHeaders != null) && (item.mNumSelectableRows != 0))
            {
                bool okayed = false;
                parameters.mSelectedObjects = Dialogs.ObjectPickerDialogEx.Show<object>(Name, item.mListObjs, item.mHeaders, item.mNumSelectableRows, new List<ObjectPicker.RowInfo>(), out okayed);
                if ((parameters.SelectedObjects == null) || (parameters.SelectedObjects.Count == 0))
                {
                    if (okayed)
                    {
                        return OptionResult.SuccessClose;
                    }
                    else
                    {
                        return OptionResult.Failure;
                    }
                }
            }

            InteractionInstance interaction = mPair.InteractionDefinition.CreateInstanceFromParameters(ref parameters);

            if (interaction is IImmediateInteraction)
            {
                Common.FunctionTask.Perform(new RunTask(interaction).Run);
                return OptionResult.SuccessClose;
            }
            else
            {
                if (hitParameters.mActor.InteractionQueue.Add(interaction))
                {
                    return OptionResult.SuccessClose;
                }
                else
                {
                    return OptionResult.Failure;
                }
            }
        }

        public class RunTask
        {
            InteractionInstance mInstance;

            public RunTask(InteractionInstance instance)
            {
                mInstance = instance;
            }

            public void Run()
            {
                mInstance.Run();
            }
        }
    }
}
