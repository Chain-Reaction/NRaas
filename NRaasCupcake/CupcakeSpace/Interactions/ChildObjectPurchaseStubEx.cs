using NRaas.CommonSpace.Helpers;
using NRaas.CupcakeSpace.Helpers;
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
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.Store.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CupcakeSpace.Interactions
{
    public class ChildObjectPurchaseStubEx : CraftersConsignment.ChildObjectPurchaseStub, Common.IPreLoad
    {
        //static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            if (Common.AssemblyCheck.IsInstalled("NRaasEconomizer")) return;

            //sOldSingleton = Singleton;
            Singleton = new Definition();
        }        

        public new class Definition : CraftersConsignment.ChildObjectPurchaseStub.Definition
        {
            public new ObjectGuid mParentRug;           

            public Definition()
            {
                this.mParentRug = ObjectGuid.InvalidObjectGuid;
            }

            public Definition(ObjectGuid mParentRug)
            {
                this.mParentRug = ObjectGuid.InvalidObjectGuid;
                this.mParentRug = mParentRug;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, IGameObject target, List<InteractionObjectPair> results)
            {
                CraftersConsignment targetObject = null;
                if (this.mParentRug != ObjectGuid.InvalidObjectGuid)
                {
                    targetObject = GlobalFunctions.ConvertGuidToObject<CraftersConsignment>(this.mParentRug);
                }
                for (IGameObject obj2 = target.Parent; (targetObject == null) && (obj2 != null); obj2 = obj2.Parent)
                {
                    targetObject = obj2 as CraftersConsignment;
                }
                if ((targetObject != null) && DisplayHelper.TestIfObjectCanBeBoughtByActor(target, actor)) //DisplayHelper.GetObjectsICanBuyInDisplay(actor, targetObject).Contains(target.ObjectId))
                {
                    ServingContainerGroup groupServing = target as ServingContainerGroup;
                    if (groupServing != null)
                    {
                        results.Add(new InteractionObjectPair(new PurchaseItemEx.Definition(groupServing, true), targetObject));
                        results.Add(new InteractionObjectPair(new PurchaseItemEx.Definition(groupServing, false), targetObject));
                        return;
                    }
                    results.Add(new InteractionObjectPair(new PurchaseItemEx.Definition(target.ObjectId, false), targetObject));
                }
            }

            /*public override string GetInteractionName(Sim actor, IGameObject target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new ChildObjectPurchaseStubEx();
                na.Init(ref parameters);
                return na;
            }*/
        }
    }
}