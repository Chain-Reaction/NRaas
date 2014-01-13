using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class ConvertTranslation : OptionItem, IHouseholdOption
    {
        public override string GetTitlePrefix()
        {
            return "ConvertTranslation";
        }

        public bool Test(GameHitParameters<SimDescriptionObject> parameters)
        {
            return base.Test(new GameHitParameters<GameObject>(parameters.mActor, parameters.mTarget, parameters.mHit));
        }

        public OptionResult Perform(GameHitParameters<SimDescriptionObject> parameters)
        {
            return base.Perform(new GameHitParameters<GameObject>(parameters.mActor, parameters.mTarget, parameters.mHit));
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            try
            {
                string key = "";

                while (true)
                {
                    key = StringInputDialog.Show(Name, "Enter Translation Key", key);
                    if (string.IsNullOrEmpty(key)) return OptionResult.Failure;

                    SimpleMessageDialog.Show(key, Common.LocalizeEAString(key));
                }
            }
            catch (Exception e)
            {
                GameHitParameters<GameObject>.Exception(parameters, e);
            }
            return OptionResult.SuccessClose;
        }
    }
}
