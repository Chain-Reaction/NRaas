using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class WooHooControlInteraction<TTarget> : Interaction<Sim, TTarget>
        where TTarget : class, IGameObject
    {
        public override bool Run()
        {
            try
            {
                IWooHooProxyDefinition definition = InteractionDefinition as IWooHooProxyDefinition;

                // Initiate from standing social
                Sim selectedTarget = definition.ITarget(this);

                // Join in progress
                if (definition.IJoinInProgress(Actor, selectedTarget, Target, this))
                {
                    return true;
                }

                if (selectedTarget == null)
                {
                    return false;
                }

                bool allowSocial = (definition.PushSocial) && (NRaas.Woohooer.Settings.mAllowForeplay);
                if (allowSocial)
                {
                    if ((Actor.SimDescription.IsZombie) || (selectedTarget.SimDescription.IsZombie))
                    {
                        allowSocial = false;

                    }
                    else if ((Actor.SimDescription.IsRobot) || (selectedTarget.SimDescription.IsRobot))
                    {
                        allowSocial = false;
                    }
                }

                if (allowSocial)
                {
                    InteractionInstance entry = new WooHooSocialInteraction.ProxyDefinition(definition, Actor, Target).CreateInstance(selectedTarget, Actor, GetPriority(), Autonomous, CancellableByPlayer);
                    
                    return Actor.InteractionQueue.Add(entry);
                }
                else
                {
                    definition.PushWooHoo(Actor, selectedTarget, Target);
                    return true;
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
                return false;
            }
        }
    }
}
