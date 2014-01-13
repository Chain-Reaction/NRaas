using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TempestSpace.Interactions
{
    public class PlantObjectHereEx : PlantObjectHere, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<GameObject, PlantObjectHere.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<GameObject, PlantObjectHere.Definition>(Singleton);
        }

        public override bool PlaceSoil()
        {
            //if (CanPlaceSoil(base.Actor, base.mCurrentTarget, base.mCurrentSoil))
            {
                mCurrentSoil.SetPosition(base.mCurrentTarget.Position);
                return true;
            }
            //return false;
        }

        public new class Definition : PlantObjectHere.Definition
        {
            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PlantObjectHereEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target.InUse)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("InUse");
                    return false;
                }

                PlantableComponent plantable = target.Plantable;
                if ((plantable == null) || plantable.InSeedSpawner)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("InSeedSpawner");
                    return false;
                }

                /*
                TerrainType type = (TerrainType)World.GetTerrainType(target.Position.x, target.Position.z, 0x0);
                if ((type != TerrainType.LotTerrain) && (type != TerrainType.WorldTerrain))
                {
                    return false;
                }
                */
                if (target.Parent != null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Parent");
                    return false;
                }
                /*
                else if (!target.IsOutside)
                {
                    return false;
                }
                */
                else if (!BaseCommonPlantingTest(a, target, target.LotCurrent, ref greyedOutTooltipCallback))
                {
                    return false;
                }

                bool flag = true;
                /*
                Soil soilToPlace = GlobalFunctions.CreateObjectOutOfWorld("GardenSoil") as Soil;
                if (soilToPlace == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("GardenSoil");
                    return false;
                }

                soilToPlace.AddToWorld();
                soilToPlace.Ghost();
                flag = PlantObjectHere.CanPlaceSoil(a, target, soilToPlace);
                soilToPlace.Destroy();

                if (!flag)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("CanPlaceSoil");
                }
                */
                return flag;
            }

            protected static bool BaseCommonPlantingTest(Sim a, GameObject target, Lot lotTryingToPlantingOn, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!PlantableComponent.PlantInteractionOpportunityTest(a, target))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("PlantInteractionOpportunityTest");
                    return false;
                }

                if (CameraController.IsMapViewModeEnabled())
                {
                    greyedOutTooltipCallback = new GreyedOutTooltipCallback(PlantObject.MapViewGreyedTooltip);
                    return false;
                }
                /*
                if (!PlantingLotTest(lotTryingToPlantingOn, a))
                {
                    greyedOutTooltipCallback = new GreyedOutTooltipCallback(PlantObject.CanOnlyPlantOnHomeLot);
                    return false;
                }
                */
                if (!PlantableComponent.PlantInteractionGardeningSkillTest(a, target, ref greyedOutTooltipCallback))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("PlantInteractionGardeningSkillTest");
                    return false;
                }
                return true;
            }
        }
    }
}
