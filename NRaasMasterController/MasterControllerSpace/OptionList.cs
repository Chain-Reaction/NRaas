using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Dialogs;
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

namespace NRaas.MasterControllerSpace
{
    public interface IOptionList : IOptionItem
    { }

    [Persistable]
    public abstract class OptionList<T> : InteractionOptionList<T, GameObject>, IOptionList, IDescriptionOptionItem
        where T : class, IInteractionOptionItem<IActor,GameObject,GameHitParameters<GameObject>>
    {
        public OptionList()
        { }

        public string HotkeyID
        {
            get { return ""; }
        }

        public bool Test(GameHitParameters<SimDescriptionObject> parameters)
        {
            return base.Test(new GameHitParameters<GameObject>(parameters.mActor, parameters.mTarget, parameters.mHit));
        }

        public OptionResult Perform(GameHitParameters<SimDescriptionObject> parameters)
        {
            return base.Perform(new GameHitParameters<GameObject>(parameters.mActor, parameters.mTarget, parameters.mHit));
        }
    }
}
