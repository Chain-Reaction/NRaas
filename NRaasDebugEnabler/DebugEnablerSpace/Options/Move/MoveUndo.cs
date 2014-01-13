using NRaas.CommonSpace.Options;
using NRaas.DebugEnablerSpace.Interfaces;
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
    public class MoveUndo : OperationSettingOption<GameObject>, IMoveOption
    {
        private static GameObject mObject;
        private static List<Vector3> mPositions = new List<Vector3>();

        public override string GetTitlePrefix()
        {
            return "MoveUndo";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!DebugEnabler.Settings.mEnabled) return false;

            if (mObject == null) return false;

            return (mPositions.Count > 0);
        }

        public static void Store(GameObject obj)
        {
            if (mObject != obj)
            {
                mPositions.Clear();
            }

            mObject = obj;
            mPositions.Add(obj.Position);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            mObject.SetPosition(mPositions[mPositions.Count-1]);

            mPositions.RemoveAt(mPositions.Count - 1);
            return OptionResult.SuccessClose;
        }
    }
}