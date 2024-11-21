using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class TakeRomanticSelfPhotoWithAEx : Sim.TakeRomanticSelfPhotoWithA, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Sim.TakeRomanticSelfPhotoWithA.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Sim, Sim.TakeRomanticSelfPhotoWithA.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public new class Definition : Sim.TakeRomanticSelfPhotoWithA.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new TakeRomanticSelfPhotoWithAEx();
                result.Init(ref parameters);
                return result;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim actor, Sim target, bool autonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!Woohooer.Settings.mUnlockTeenActions)
                    {
                        if ((target.SimDescription != null) && target.SimDescription.TeenOrBelow)
                        {
                            return false;
                        }
                    }

                    if((actor != null && target != null) && (actor.SimDescription.TeenOrBelow || target.SimDescription.TeenOrBelow))
                    {
                        if(!Woohooer.Settings.AllowTeen(false))
                        {
                            return false;
                        }

                        if((actor.SimDescription.YoungAdultOrAbove || target.SimDescription.YoungAdultOrAbove))
                        {
                            if(!Woohooer.Settings.AllowTeenAdult(false))
                            {
                                return false;
                            }
                        }
                    }

                    if (((actor == target) || (actor == null)) || (((target == null) || (target.SimDescription == null)) || target.SimDescription.ChildOrBelow))
                    {
                        return false;
                    }
                    if (!target.CanBeSocializedWith || ((target.SimDescription.DeathStyle != SimDescription.DeathType.None) && !target.SimDescription.IsPlayableGhost))
                    {
                        return false;
                    }
                    if (!actor.Posture.AllowsNormalSocials())
                    {
                        return false;
                    }
                    if (!target.Posture.AllowsNormalSocials())
                    {
                        return false;
                    }

                    Relationship relationship = Relationship.Get(actor, target, false);
                    return ((relationship != null) && relationship.AreRomantic());
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.Exception(actor, target, e);
                    return false;
                }
            }
        }
    }
}
