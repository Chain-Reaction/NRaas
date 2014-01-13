using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.VectorSpace.Interactions
{
    public class AskContagious
    {
        public static bool OnTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                foreach (DiseaseVector vector in Vector.Settings.GetVectors(target))
                {
                    if (vector.ShowingSigns) return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static void OnAccept(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                foreach (DiseaseVector vector in Vector.Settings.GetVectors(target))
                {
                    if (vector.ShowingSigns)
                    {
                        if (vector.IsContagious)
                        {
                            Common.Notify(target, Common.Localize("AskContagious:Success", target.IsFemale, new object[] { target }));
                            return;
                        }
                    }
                }

                Common.Notify(target, Common.Localize("AskContagious:Failure", target.IsFemale, new object[] { target }));
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
