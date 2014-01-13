using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Dereferences
{
    public class DerefDreamsAndPromisesManager : Dereference<DreamsAndPromisesManager>
    {
        protected override DereferenceResult Perform(DreamsAndPromisesManager reference, FieldInfo field, List<ReferenceWrapper> objects)
        {
            if (Matches(reference, "mPotentialRemovalNodes", field, objects))
            {
                if (Performing)
                {
                    foreach (ReferenceWrapper obj in objects)
                    {
                        List<ActiveDreamNode> nodes;
                        if (reference.mPotentialRemovalNodes.TryGetValue(obj, out nodes))
                        {
                            foreach (ActiveDreamNode node in nodes)
                            {
                                reference.RemoveActiveNode(node, false);
                                reference.mSleepingNodes.Remove(node);

                                reference.RemoveDisplayedPromiseNode(node);
                            }
                        }

                        reference.mPotentialRemovalNodes.Remove(obj);
                    }
                }

                return DereferenceResult.End;
            }

            if (Matches(reference, "mActor", field, objects))
            {
                Remove(ref reference.mActor);
                return DereferenceResult.End;
            }

            if (Matches(reference, "mActiveNodes", field, objects))
            {
                if (Performing)
                {
                    ActiveNodeBase node = Find<ActiveNodeBase>(objects);
                    if (node != null)
                    {
                        try
                        {
                            reference.RemoveActiveNode(node);
                        }
                        catch
                        { }
                    }
                }

                return DereferenceResult.End;
            }

            return DereferenceResult.Failure;
        }
    }
}
