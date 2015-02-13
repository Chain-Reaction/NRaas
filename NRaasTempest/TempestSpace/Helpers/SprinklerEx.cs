using NRaas.CommonSpace;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Helpers
{
    public class SprinklerEx : Common.IExitBuildBuy, Common.IWorldLoadFinished
    {
        private static AlarmHandle processAlarm;           

        private void TurnOnUpgradedSprinklers()
        {
            foreach (ISprinkler sprinkler in Sims3.Gameplay.Queries.GetObjects<ISprinkler>())
            {
                Sprinkler sprinkler2 = sprinkler as Sprinkler;
                if (SeasonsManager.sInstance != null && SeasonsManager.CurrentSeason == Season.Winter && sprinkler.IsOutside)
                {
                    continue;
                }

                if (sprinkler2.Upgradable != null && sprinkler2.Upgradable.AutoWater && !sprinkler2.TurnedOn && !sprinkler2.OnOffChanging)
                {                    
                    sprinkler2.TurnOnSprinkler();
                    RemoveEAAlarms(sprinkler);
                    if (sprinkler2.TurnedOn)
                    {
                        sprinkler2.AutoTurnedOn = true;
                    }
                }
            }
        }

        private void ProcessSprinklersCallback()
        {            
            foreach (ISprinkler sprinkler in Sims3.Gameplay.Queries.GetObjects<ISprinkler>())
            {
                Sprinkler sprinkler2 = sprinkler as Sprinkler;
                if (sprinkler2.TurnedOn)
                {
                    SpawnPuddles(sprinkler2);
                    if (!sprinkler2.ShouldBeOn)
                    {
                        sprinkler2.TurnOffSprinkler();
                    }
                }
            }           
        }

        public static void SpawnPuddles(Sprinkler sprinkler)
        {           
            // this calls SetPuddles(true) in the core
            bool set = true;
            LotLocation location = new LotLocation();
            ulong lotLocation = World.GetLotLocation(sprinkler.PositionOnFloor, ref location);
            int num2 = (int)(Math.Sqrt(2.0) * Sprinkler.kRangeOfSpray);
            Vector2[] vectorArray = new Vector2[4];
            for (int i = 0; i <= 3; i++)
            {
                vectorArray[i].x = sprinkler.mDiscourageFootprintPoints[i].x;
                vectorArray[i].y = sprinkler.mDiscourageFootprintPoints[i].z;
            }
            Vector2 vector = new Vector2(vectorArray[1].x - vectorArray[0].x, vectorArray[1].y - vectorArray[0].y);
            Vector2 vector2 = new Vector2(vectorArray[3].x - vectorArray[0].x, vectorArray[3].y - vectorArray[0].y);
            float num4 = vector.Length();
            vector.Normalize();
            vector2.Normalize();
            Vector3 worldPos = new Vector3();
            Vector2 vector4 = new Vector2();
            LotLocation location2 = new LotLocation();
            short startingRoomId = World.GetRoomId(lotLocation, location, eRoomDefinition.LightBlocking);
            for (int j = location.mX - num2; j <= (location.mX + num2); j++)
            {
                for (int k = location.mZ - num2; k <= (location.mZ + num2); k++)
                {
                    location2.mX = j;
                    location2.mZ = k;
                    location2.mLevel = location.mLevel;
                    location2.mRoom = location.mRoom;
                    World.GetWorldPosition(lotLocation, location2, ref worldPos);
                    vector4.x = worldPos.x - vectorArray[0].x;
                    vector4.y = worldPos.z - vectorArray[0].y;
                    float num8 = (float)(vector4 * vector);
                    float num9 = (float)(vector4 * vector2);
                    if (((num8 >= 0f) && (num8 <= num4)) && ((num9 >= 0f) && (num9 <= num4)))
                    {
                        PlacePuddleIfSprayableEx(lotLocation, location2, worldPos, set, startingRoomId);
                    }
                }
            }
        }

        public static void PlacePuddleIfSprayableEx(ulong lotID, LotLocation loc, Vector3 pos, bool set, short startingRoomId)
        {            
            for (sbyte i = loc.mLevel; i >= 0; i = (sbyte)(i - 1))
            {
                LotLocation location = new LotLocation(loc.mX, loc.mZ, i, 0);
                if (World.GetRoomId(lotID, location, eRoomDefinition.LightBlocking) == startingRoomId)
                {
                    foreach (ObjectGuid guid in World.GetObjects(lotID, location))
                    {
                        ISprinklable sprinklable = guid.ObjectFromId<ISprinklable>();
                        if (sprinklable != null)
                        {
                            sprinklable.OnSprinkled();
                        }
                        IPlanterBowl bowl = guid.ObjectFromId<IPlanterBowl>();
                        if (bowl != null)
                        {
                            WaterContainedPlant(bowl);
                        }
                        ISoilRug rug = guid.ObjectFromId<ISoilRug>();
                        if (rug != null)
                        {
                            WaterContainedPlant(rug);
                        }
                    }
                    if (World.HasSolidFloor(lotID, location) && ((i != 0) || (World.GetTerrainType(pos.x, pos.z, 0) == 0x20)))
                    {
                        if (!Tempest.Settings.mSprinklersSpawnPuddles || !PuddleManager.IsValidPuddleLocation(lotID, location))
                        {
                            break;
                        }
                        World.SetPuddle(lotID, location, set);
                        return;
                    }
                }
            }
        }

        public static void WaterContainedPlant(IGameObject obj)
        {            
            if (!(obj is ISoilRug) && !(obj is IPlanterBowl))
            {
                return;
            }

            foreach (Slot slot in obj.GetContainmentSlots())
            {
                Soil soil = obj.GetContainedObject(slot) as Soil;
                if (soil != null)
                {
                    soil.OnSprinkled();
                }
            }
        }        

        public static void RemoveEAAlarms(ISprinkler sprinkler)
        {
            Sprinkler waterThingy = sprinkler as Sprinkler;
            AlarmManager.Global.RemoveAlarm(waterThingy.mProcessingAlarm);
            waterThingy.mProcessingAlarm = AlarmHandle.kInvalidHandle;

            AlarmManager.Global.RemoveAlarm(waterThingy.mAutoWaterAlarm);
            waterThingy.mAutoWaterAlarm = AlarmHandle.kInvalidHandle;                     
        }        

        public void OnWorldLoadFinished()
        {
            foreach (ISprinkler sprinkler in Sims3.Gameplay.Queries.GetObjects<ISprinkler>())
            {
                RemoveEAAlarms(sprinkler);
            }

            new Common.AlarmTask(Sprinkler.kAutoWaterTimeOfDay, ~DaysOfTheWeek.None, TurnOnUpgradedSprinklers);
            processAlarm = AlarmManager.Global.AddAlarmRepeating(Sprinkler.kProcessDelay, TimeUnit.Minutes, new AlarmTimerCallback(ProcessSprinklersCallback), Sprinkler.kProcessPeriod, TimeUnit.Minutes, "Sprinkler Processing Alarm", AlarmType.DeleteOnReset, null);
            AlarmManager.Global.AlarmWillYield(processAlarm);
        }

        public void OnExitBuildBuy(Lot lot)
        {
            foreach (ISprinkler sprinkler in lot.GetObjects<ISprinkler>())
            {
                RemoveEAAlarms(sprinkler);
            }
        }
    }
}
