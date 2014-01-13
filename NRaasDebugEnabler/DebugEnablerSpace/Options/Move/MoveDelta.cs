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
    public class MoveDelta : MoveBase
    {
        public override string GetTitlePrefix()
        {
            return "MoveDelta";
        }

        protected override CommonSpace.Options.OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Vector3 position = parameters.mTarget.Position;

            string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", false, new object[] { position.z }), "0");
            if ((text == null) || (text == "")) return CommonSpace.Options.OptionResult.Failure;

            float value = 0;
            if (!float.TryParse(text, out value))
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error", false, new object[] { text }));
                return CommonSpace.Options.OptionResult.Failure;
            }

            Perform(parameters.mTarget, value);
            return CommonSpace.Options.OptionResult.SuccessClose;
        }
    }
}