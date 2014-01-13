using NRaas.CommonSpace.Interactions;
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
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class CurrentInteraction : DebugEnablerInteraction<GameObject>
    {
        // Fields
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if ((obj is Sim) || (obj is CityHall))
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public override bool Run()
        {
            try
            {
                Sim sim = Target as Sim;
                if (sim != null)
                {
                    string msg = null;
                    foreach (InteractionInstance instance in sim.InteractionQueue.InteractionList)
                    {
                        msg += Common.NewLine + Common.NewLine + instance.InteractionDefinition.GetType().AssemblyQualifiedName;

                        if (instance.InteractionDefinition is SocialInteractionA)
                        {
                            msg += Common.NewLine + "(" + instance.InteractionDefinition.ToString() + ")";
                        }
                    }

                    DebugEnabler.WriteLog(msg);

                    SimpleMessageDialog.Show(Common.Localize("CurrentInteraction:MenuName"), msg);
                }
                else
                {
                    string msg = null;

                    Sims3.Gameplay.Objects.RabbitHoles.CityHall cityHall = Target as Sims3.Gameplay.Objects.RabbitHoles.CityHall;
                    if (cityHall != null)
                    {
                        foreach (Sim member in LotManager.Actors)
                        {
                            if ((member.InteractionQueue != null) && (member.InteractionQueue.GetHeadInteraction() != null))
                            {
                                msg += Common.NewLine + member.SimDescription.FullName;
                                foreach (InteractionInstance instance in member.InteractionQueue.InteractionList)
                                {
                                    msg += Common.NewLine + Common.NewLine + instance.InteractionDefinition.GetType().AssemblyQualifiedName;

                                    if (instance.InteractionDefinition is SocialInteractionA.Definition)
                                    {
                                        msg += Common.NewLine + "(" + instance.InteractionDefinition.ToString() + ")";
                                    }
                                }
                            }
                        }
                    }

                    DebugEnabler.WriteLog(msg);
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<CurrentInteraction>
        {
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("CurrentInteraction:MenuName");
            }

            public override bool Test(IActor a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                Sim sim = target as Sim;
                if (sim != null)
                {
                    if (sim.InteractionQueue == null) return false;

                    if (sim.InteractionQueue.Count == 0) return false;

                    return true;
                }
                else
                {
                    Sims3.Gameplay.Objects.RabbitHoles.CityHall cityHall = target as Sims3.Gameplay.Objects.RabbitHoles.CityHall;
                    return (cityHall != null);
                }
            }
        }
    }
}