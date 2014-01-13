using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class CollectEx : RockGemMetalBase.Collect, Common.IPreLoad
    {
        public new static readonly InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<RockGemMetalBase, RockGemMetalBase.Collect.Definition, Definition>(false);
        }

        public override bool Run()
        {
            try
            {
                float kRouteDistance = Target.Tuning.kRouteDistance;
                if (Target.Parent is ISculptingStation)
                {
                    kRouteDistance = 1.3f;
                }

                try
                {
                    if (!Actor.RouteToObjectRadius(Target, kRouteDistance))
                    {
                        return false;
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.DebugException(Actor, Target, e);
                    return false;
                }

                StandardEntry();
                Actor.SkillManager.AddElement(SkillNames.Collecting);
                BeginCommodityUpdates();

                bool succeeded = false;
                try
                {
                    Actor.PlaySoloAnimation("a2o_object_genericSwipe_x", true);
                    Target.FadeOut(true);
                    succeeded = Inventories.TryToMove(Target, Actor);
                    if (succeeded)
                    {
                        Target.RegisterCollected(Actor, true);
                        Target.RemoveFromWorld();
                    }
                    Target.SetOpacity(1f, 0f);
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                    StandardExit();
                }

                return succeeded;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        private new class Definition : RockGemMetalBase.Collect.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CollectEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}

