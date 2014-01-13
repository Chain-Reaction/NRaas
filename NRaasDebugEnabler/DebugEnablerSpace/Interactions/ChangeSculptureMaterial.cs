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
using Sims3.Gameplay.ObjectComponents;
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
    public class ChangeSculptureMaterial : DebugEnablerInteraction<GameObject>
    {
        // Fields
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if (obj.SculptureComponent != null)
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        [DoesntRequireTuning]
        private class Definition : DebugEnablerDefinition<ChangeSculptureMaterial>
        {
            public SculptureComponent.SculptureMaterial mMaterial = SculptureComponent.SculptureMaterial.None;

            public Definition()
            {}
            protected Definition(SculptureComponent.SculptureMaterial material)
            {
                mMaterial = material;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { Common.Localize("ChangeMaterial:MenuName") };
            }

            public override void AddInteractions(InteractionObjectPair iop, IActor actor, GameObject target, List<InteractionObjectPair> results)
            {
                foreach (SculptureComponent.SculptureMaterial material in Enum.GetValues(typeof(SculptureComponent.SculptureMaterial)))
                {
                    if (material == SculptureComponent.SculptureMaterial.None) continue;

                    if (material == SculptureComponent.SculptureMaterial.Metal) continue;

                    if (material == SculptureComponent.SculptureMaterial.Topiary) continue;

                    results.Add(new InteractionObjectPair(new Definition(material), target));
                }
            }

            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return mMaterial.ToString();
            }

            public override bool Test(IActor a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                SculptureComponent component = target.SculptureComponent;

                if (component.Material == SculptureComponent.SculptureMaterial.Topiary) return false;

                if (component.Material == SculptureComponent.SculptureMaterial.Metal) return false;

                return (component != null);
            }
        }

        public override bool Run()
        {
            try
            {
                SculptureComponent component = Target.SculptureComponent;
                if (component == null) return false;

                Definition definition = InteractionDefinition as Definition;
                
                component.mSculptureMaterial = (uint)definition.mMaterial;
                if ((definition.mMaterial == SculptureComponent.SculptureMaterial.Stone) && RandomUtil.CoinFlip())
                {
                    component.mSculptureMaterial |= 0x80;
                }
                component.mSculptureMaterial |= (uint)(RandomUtil.GetInt(0x0, SculptureComponent.kNumSculptureTexturesPerMaterial - 0x1) << 0x8);
                component.SetShaderIfNeeded();

                component.mScriptObject.RemoveAlarm(component.mMeltingAlarm);
                component.mMeltingAlarm = AlarmHandle.kInvalidHandle;
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }
    }
}