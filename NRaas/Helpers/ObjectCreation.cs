using Sims3.Gameplay;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections;

namespace NRaas.CommonSpace.Helpers
{
    public class ObjectCreation
    {
        private static IGameObject CreateObjectInternal(ulong instance, ProductVersion version, Hashtable data, Simulator.ObjectInitParameters initData)
        {
            ResourceKey key = new ResourceKey(instance, 0x319e4f1d, ResourceUtils.ProductVersionToGroupId(version));

            return GlobalFunctions.CreateObjectInternal(key, data, initData);
        }

        private static IGameObject CreateObjectWithOverrides(ulong instance, ProductVersion version, Vector3 initPos, int level, Vector3 initFwd, Hashtable overrides, Simulator.ObjectInitParameters initData)
        {
            GlobalFunctions.FillInInitData(initPos, level, initFwd, ref initData);
            IGameObject createdObject = CreateObjectInternal(instance, version, overrides, initData);
            GlobalFunctions.CheckForFailure(createdObject, "Missing object resource instance " + instance + Common.NewLine);
            return createdObject;
        }

        public static IGameObject CreateObject(ulong instance, ProductVersion version, Simulator.ObjectInitParameters initData)
        {
            Hashtable overrides = new Hashtable(0x1);

            IGameObject obj = null;
            try
            {
                obj = CreateObjectWithOverrides(instance, version, Vector3.OutOfWorld, 0, Vector3.UnitZ, overrides, initData);

                if (obj is FailureObject)
                {
                    Common.DebugNotify((obj as FailureObject).mErrorText);
                }
            }
            catch (NullReferenceException exception)
            {
                string str = "Instance name was " + instance + Common.NewLine;
                throw new NullReferenceException(exception.Message + str);
            }
            return obj;
        }
    }
}

