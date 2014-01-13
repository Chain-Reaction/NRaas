using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class HotkeyCheatInteraction : ImmediateInteraction<Sim,Sim>, Common.IPreLoad
    {
        public void OnPreLoad()
        {
            Sim.EditSimInCAS.Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                Definition definition = InteractionDefinition as Definition;
                if (definition == null) return false;

                return (definition.mOption.Perform(new GameHitParameters<GameObject> (Actor, Target, GameObjectHit.NoHit)) != OptionResult.Failure);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }

            return false;
        }

        public class Definition : ActorlessInteractionDefinition<Sim, Sim, HotkeyCheatInteraction>
        {
            public OptionItem mOption;

            public Definition()
            { }
            public Definition(OptionItem item)
            {
                mOption = item;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Sim target, List<InteractionObjectPair> results)
            {
                results.Add(new InteractionObjectPair(EditSimInCASEx.Singleton, target));

                foreach (OptionItem item in HotkeyInteraction.HotkeyOption.GetHotkeyOptions())
                {
                    results.Add(new InteractionObjectPair(new Definition(item), target));
                }
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                if (mOption == null) return null;

                return mOption.Name;
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (mOption == null) return true;

                    return mOption.Test(new GameHitParameters<GameObject>(actor, target, GameObjectHit.NoHit));
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
