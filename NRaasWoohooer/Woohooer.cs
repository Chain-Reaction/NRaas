using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.WoohooerSpace;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Options.General;
using NRaas.WoohooerSpace.Scoring;
using NRaas.WoohooerSpace.Skills;
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
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.TombObjects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class Woohooer : Common, Common.IPreLoad, Common.IWorldLoadFinished, Common.IWorldQuit
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        public static bool sWasAffectBroadcasterRoomOnly = false;

        [PersistableStatic]
        protected static PersistedSettings sSettings = null;

        static float sOriginalAttractionThreshold = Relationship.kAttractionThreshold;

        static Woohooer()
        {
            sEnableLoadLog = true;

            StatValueCount.sFullLog = true;

            Bootstrap();

            BooterHelper.Add(new BuffBooter());
            BooterHelper.Add(new SkillBooter());
            BooterHelper.Add(new ScoringBooter("MethodFile", "NRaas.WoohooerModule", false));
            BooterHelper.Add(new SocializingBooter());
        }

        public Woohooer()
        { }

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

            KamaSimtra.ResetSettings();

            AdjustAttractionTuning();
        }

        public void OnPreLoad()
        {
            sOriginalAttractionThreshold = Relationship.kAttractionThreshold;
        }

        public void OnWorldLoadFinished()
        {
            kDebugging = Settings.Debugging;

            new Common.ImmediateEventListener(EventTypeId.kBuiltIgloo, OnNewObject);
            new Common.ImmediateEventListener(EventTypeId.kRakeLeafPile, OnNewObject);
            
            MustBeRomanticForAutonomousSetting.AdjustVisitorPrivilege(!Settings.mMustBeRomanticForAutonomousV2[PersistedSettings.GetSpeciesIndex(CASAgeGenderFlags.Human)]);

            sWasAffectBroadcasterRoomOnly = Conversation.ReactToSocialParams.AffectBroadcasterRoomOnly;

            Conversation.ReactToSocialParams.AffectBroadcasterRoomOnly = true;

            AdjustAttractionTuning();
        }

        protected static new void OnNewObject(Event e)
        {
            if (e.Actor.LotCurrent == null) return;

            Common.AddAllInteractions(e.Actor.LotCurrent.LotId, true);
        }

        public void OnWorldQuit()
        {
            if (Settings.Debugging)
            {
                WoohooScoring.Dump(true);
            }
        }

        public static void AdjustAttractionTuning()
        {
            if (Settings.mAttractionBaseChanceScoringV3[0] <= -1000)
            {
                Relationship.kAttractionThreshold = float.MaxValue;
            }
            else
            {
                Relationship.kAttractionThreshold = sOriginalAttractionThreshold;
            }
        }

        // Externalized to MasterController
        public static float GetChanceOfQuads()
        {
            try
            {
                return Woohooer.Settings.mChanceOfQuads;
            }
            catch (Exception exception)
            {
                Common.Exception("GetChanceOfQuads", exception);
                return 0;
            }
        }

        // Externalized to Vector
        public static bool IsRiskyOrTryForBaby(Event e)
        {
            try
            {
                CommonWoohoo.NRaasWooHooEvent woohooEvent = e as CommonWoohoo.NRaasWooHooEvent;
                if (woohooEvent == null) return true;

                switch(woohooEvent.Style)
                {
                    case CommonWoohoo.WoohooStyle.Risky:
                    case CommonWoohoo.WoohooStyle.TryForBaby:
                        return true;
                }

                return false;
            }
            catch(Exception exception)
            {
                Common.Exception(e.Actor, e.TargetObject, exception);
                return false;
            }
        }

        public static GreyedOutTooltipCallback StoryProgressionTooltip(string debuggingReason, bool debuggingOnly)
        {
            if (Common.kDebugging)
            {
                return Common.DebugTooltip("StoryProgression " + debuggingReason);
            }
            else 
            {
                if (debuggingOnly) return null;

                return delegate { return Localize("Socials:ProgressionDenied"); };
            }
        }

        public static InteractionTuning InjectAndReset<Target, OldType, NewType>(bool clone)
            where Target : IGameObject
            where OldType : InteractionDefinition
            where NewType : InteractionDefinition
        {
            return WoohooTuningControl.ResetTuning(Tunings.Inject<Target, OldType, NewType>(clone), false, false);
        }
    }
}
