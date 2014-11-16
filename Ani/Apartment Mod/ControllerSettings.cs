using Sims3.SimIFace;
using System;
using Sims3.Gameplay.Utilities;

namespace TS3Apartments
{
    [Persistable]
    public class ControllerSettings
    {
        public static float timeOfRent = 9f;
        public static DaysOfTheWeek rentDay = DaysOfTheWeek.Monday;
        public static int numberOfRoommate = 0;
        public static bool alwaysDisableRoommateService = true;
    }
}
