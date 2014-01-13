using NRaas.CommonSpace.Options;
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
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Options.Move
{
    public abstract class MoveBase : OperationSettingOption<GameObject>, IMoveOption
    {
        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!DebugEnabler.Settings.mEnabled) return false;

            if (parameters.mTarget.InInventory) return false;

            if (parameters.mTarget is Terrain) return false;

            if (parameters.mTarget is Lot) return false;

            if (parameters.mTarget is RoomConnectionObject) return false;

            return parameters.mTarget.InWorld;
        }

        protected static bool Perform(GameObject target, float delta)
        {
            try
            {
                MoveUndo.Store(target);

                Vector3 position = target.Position;

                position.y += delta;

                target.SetPosition(position);
            }
            catch (Exception exception)
            {
                Common.Exception(target, exception);
            }
            return true;
        }
    }
}