using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Interactions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Interactions
{
    public class PrimaryInteraction : CommonInteraction<IPrimaryOption<GameObject>, GameObject>
    {
        public static PrimaryDefinition Singleton = new PrimaryDefinition();

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            if ((!Singleton.IsVersion) || (!Common.AssemblyCheck.IsInstalled("NRaasOverwatch")))
            {
                interactions.AddRoot(Singleton);
            }
        }

        protected override OptionResult Perform(IActor actor, GameObject target, GameObjectHit hit)
        {
            throw new NotImplementedException();
        }

        public class PrimaryDefinition : CommonDefinition<PrimaryInteraction>
        {
            public PrimaryDefinition()
                : base(true,true)
            {
                List<IPrimaryOption<GameObject>> list = CommonOptionList<IPrimaryOption<GameObject>>.AllOptions();
                if ((list.Count == 1) && (list[0] is IVersionOption))
                {
                    mOption = list[0] as IInteractionOptionItem<IActor, GameObject, GameHitParameters<GameObject>>;
                }
            }

            public bool IsVersion
            {
                get { return (mOption != null); }
            }

            public override void AddInteractions(InteractionObjectPair iop, IActor actor, GameObject target, List<InteractionObjectPair> results)
            {
                if (mOption != null)
                {
                    results.Add(iop);
                }
                else
                {
                    base.AddInteractions(iop, actor, target, results);
                }
            }

            public override string GetInteractionName(IActor actor, GameObject target, InteractionObjectPair iop)
            {
                if (mOption != null)
                {
                    return Common.Localize("Root:MenuName").Replace("...", "") + " = " + EAText.GetNumberString(VersionStamp.sVersion);
                }
                else
                {
                    return base.GetInteractionName(actor, target, iop);
                }
            }

            public override string[] GetPath(bool isFemale)
            {
                if (mOption != null)
                {
                    return new string[] { "NRaas", mOption.Name };
                }
                else
                {
                    return base.GetPath(isFemale);
                }
            }
        }
    }
}
