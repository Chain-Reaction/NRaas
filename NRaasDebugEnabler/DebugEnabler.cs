using NRaas.CommonSpace.Tasks;
using NRaas.DebugEnablerSpace;
using NRaas.DebugEnablerSpace.Interactions;
using NRaas.DebugEnablerSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class DebugEnabler : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static DebugEnabler()
        {
            Bootstrap();
        }

        public static PersistedSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new PersistedSettings();
                }

                return sSettings;
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;
        }

        public void OnWorldLoadFinished()
        {
            Common.kDisableLotMenu = false;

            Simulator.AddObject(new RegisterCommandsTask());

            new Common.DelayedEventListener(EventTypeId.kInventoryObjectAdded, OnNewObject);
            new Common.DelayedEventListener(EventTypeId.kObjectStateChanged, OnNewObject);

            // Corrects for an error in SetGraduatedBoardingSchool.Definition AddInteractions when running in CAW
            if (BoardingSchool.BoardingSchoolData.sBoardingSchoolDataList == null)
            {
                BoardingSchool.BoardingSchoolData.sBoardingSchoolDataList = new PairedListDictionary<BoardingSchool.BoardingSchoolTypes, BoardingSchool.BoardingSchoolData>();

                StubXMLRow stubRow = new StubXMLRow();
                foreach (BoardingSchool.BoardingSchoolTypes types in Enum.GetValues(typeof(BoardingSchool.BoardingSchoolTypes)))
                {
                    BoardingSchool.BoardingSchoolData.sBoardingSchoolDataList.Add(types, new BoardingSchool.BoardingSchoolData(stubRow));
                }
            }
        }

        public class StubXMLRow : XmlDbData.XmlDbRowFast
        {
            public StubXMLRow()
            {
                mData = new Dictionary<string, string>();
            }

            public override bool TryGetValueInternal(string column, out string value)
            {
                value = "";
                return true;
            }
        }

        protected class RegisterCommandsTask : RepeatingTask
        {
            public RegisterCommandsTask()
            { }

            protected override bool OnPerform()
            {
                if (RegisterCommands.Perform())
                {
                    // Stop the task
                    return false;
                }
                else
                {
                    // Continue the task
                    return true;
                }
            }
        }
    }
}
