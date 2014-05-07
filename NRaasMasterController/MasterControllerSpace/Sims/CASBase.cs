using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.MasterControllerSpace.CAS;
using NRaas.MasterControllerSpace.Proxies;
using NRaas.MasterControllerSpace.Sims.Basic;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using Sims3.Metadata;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.CAS.CAB;
using Sims3.UI.CAS.CAP;
using Sims3.UI.Controller.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims
{
    public abstract class CASBase : SimFromList
    {
        public enum EditType
        {
            None,
            Uniform,
            Mannequin
        }

        public enum SortOrder
        {
            EAStandard = 0,
            CustomContentTop,
            CustomContentBottom,
        }

        static readonly List<BodyTypes> sCachedTypes = new List<BodyTypes>(new BodyTypes[] { BodyTypes.FullBody, BodyTypes.UpperBody, BodyTypes.LowerBody, BodyTypes.Shoes, BodyTypes.RightGarter, BodyTypes.LeftGarter });

        static readonly List<BodyTypes> sApplyAllNotTypes = new List<BodyTypes>(new BodyTypes[] { BodyTypes.PetSaddle, BodyTypes.PetBridle, BodyTypes.PetCollar, BodyTypes.PetBlanket, BodyTypes.PetBreastCollar });

        static readonly OutfitCategories[] sValidChoices = new OutfitCategories[] { OutfitCategories.Everyday, OutfitCategories.Naked, OutfitCategories.Formalwear, OutfitCategories.Athletic, OutfitCategories.Sleepwear, OutfitCategories.Swimwear, OutfitCategories.Bridle, OutfitCategories.Jumping, OutfitCategories.Racing };

        static bool sFemaleBodyHairInstalled;

        static bool sInitialized;
        static SimDescription.DeathType sDeathType;
        static SimDescription sLastSim;
        static float sPregnant;
        static bool sNoHouse;
        static bool sInstantiated;
        static CASAgeGenderFlags sAgeStage;
        static float sYearsUntilTransition;
        static bool sAgingEnabled;
        static OutfitCategories sActualCategory = OutfitCategories.None;
        static int sActualIndex = 0;

        public static List<CASParts.Wrapper> sUnboxedParts = new List<CASParts.Wrapper>();

        static Household sHousehold = null;

        public static bool sWasCanceled = false;

        static string sFirstName;
        static string sLastName;

        public delegate CASMode OnGetMode(SimDescription sim, ref OutfitCategories startCategory, ref int startIndex, ref EditType editType);

        static OnGetMode sMode = null;

        static CASTask sTask = null;

        static CASTattoo sTattoo = null;
        static CASBodyHair sBodyHair = null;

        static List<TraitNames> sRewardTraits = null;

        static List<SimDescription> sQueuedSims = null;

        static bool sSwapOutfits = false;
        static OutfitCategoryMap sNonPregnancyOutfits = null;
        static int sPreTattooCount;
        static List<int> sAddedOutfits = new List<int>();

        static List<ICASFacialBlendPanelAdjustment> sFacialPanels = null;

        static SimDescriptionCore sPreviousSim = null;
        static CASParts.Key sPreviousKey = new CASParts.Key(OutfitCategories.None, 0);
        static Dictionary<SimDescriptionCore,SavedOutfit.Cache> sOutfits = new Dictionary<SimDescriptionCore,SavedOutfit.Cache>();

        static List<CASParts.Wrapper> sHairParts = null;

        public delegate void ExternalShowUI(bool initialized);
        public delegate void ExternalClosingDown();

        public static ExternalShowUI OnExternalShowUI;
        public static ExternalClosingDown OnExternalClosingDown;

        protected abstract CASMode GetMode(SimDescription sim, ref OutfitCategories startCategory, ref int startIndex, ref EditType editType);

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool AllowSpecies(IMiniSimDescription me)
        {
            SimDescription sim = me as SimDescription;
            if (sim != null)
            {
                if ((sim.IsDeer) || (sim.IsRaccoon)) return false;
            }

            return base.AllowSpecies(me);
        }

        public static bool PublicAllow(SimDescription me, ref GreyedOutTooltipCallback callback)
        {
            if (me.CreatedSim != null)
            {
                if ((me.CreatedSim.BuffManager != null) && me.CreatedSim.BuffManager.HasTransformBuff())
                {
                    callback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Gameplay/Buffs/BuffTransformation:CantChangeClothesTooltip", new object[] { me }));
                    return false;
                }
            }
            else
            {
                if (!RecoverMissingSimTask.Allowed(me, false, ref callback))
                {
                    return false;
                }
            }

            if (me.GetOutfit(OutfitCategories.Everyday, 0) == null)
            {
                callback = Common.DebugTooltip("No Everyday Outfit");
                return false;
            }

            if (me.Household == Household.ActiveHousehold) return true;

            if (me.Household == null)
            {
                callback = Common.DebugTooltip("No Household");
                return false;
            }

            if (SimTypes.IsTourist(me))
            {
                callback = Common.DebugTooltip("Tourist Fail");
                return false;
            }

            return true;
        }

        public static SavedOutfit GetOutfit(SimDescriptionCore sim, OutfitCategories category, int index)
        {
            SavedOutfit.Cache cache;
            if (!sOutfits.TryGetValue(sim, out cache)) return null;

            return cache.Load(new CASParts.Key(category, index));
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            GreyedOutTooltipCallback callback = null;
            if (!PublicAllow(me, ref callback))
            {
                if (callback != null)
                {
                    Common.DebugNotify(callback());
                }
                return false;
            }

            return true;
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            List<SimDescription> fullSims = new List<SimDescription>();

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription sim = miniSim as SimDescription;
                if (sim == null) continue;

                fullSims.Add(sim);
            }

            if (fullSims.Count == 1)
            {
                if (Perform(fullSims[0], GetMode))
                {
                    return OptionResult.SuccessClose;
                }
                else
                {
                    return OptionResult.Failure;
                }
            }
            else
            {
                Perform(fullSims, GetMode);
                return OptionResult.SuccessClose;
            }
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return Perform(me, GetMode);
        }

        public static IEnumerable<CASParts.Wrapper> HairParts
        {
            get
            {
                return sHairParts;
            }
        }

        public static void Perform(List<SimDescription> sims, OnGetMode mode)
        {
            if ((sims == null) || (sims.Count == 0)) return;

            sQueuedSims = sims;

            new RunQueueTask(mode).AddToSimulator();
        }

        protected static bool IsSpecialEdit()
        {
            switch(sActualCategory)
            {
                case OutfitCategories.MartialArts:
                case OutfitCategories.Naked:
                case OutfitCategories.Special:
                case OutfitCategories.AuxCareer:
                    return true;
            }

            return false;
        }

        public static bool Perform(SimDescription me, OnGetMode getMode)
        {
            CASChangeReporter.Instance.ClearChanges();

            sWasCanceled = false;
            sMode = getMode;

            CASTask.Create(ref sTask);

            sFemaleBodyHairInstalled = false;
            sFacialPanels = null;

            sLastSim = me;

            sFirstName = null;
            sLastName = null;

            sNonPregnancyOutfits = null;
            sSwapOutfits = false;

            sActualCategory = OutfitCategories.Everyday;
            sActualIndex = 0;

            sOutfits.Clear();

            sPreTattooCount = -1;
            sAddedOutfits.Clear();

            OutfitCategories displayCategory = sActualCategory;
            int displayIndex = sActualIndex;

            EditType editType = EditType.None;
            CASMode mode = getMode(me, ref displayCategory, ref displayIndex, ref editType);

            if (me != null)
            {
                CASController controller = CASController.Singleton;
                if (controller != null)
                {
                    controller.mAccessCareer = (me.GetOutfitCount(OutfitCategories.Career) > 0);
                }

                sRewardTraits = new List<TraitNames>();

                foreach (Trait trait in me.TraitManager.RewardTraits)
                {
                    sRewardTraits.Add(trait.Guid);
                }

                sPreTattooCount = me.GetOutfitCount(OutfitCategories.Everyday);

                sYearsUntilTransition = me.YearsSinceLastAgeTransition;
                sAgingEnabled = me.AgingEnabled;
                sAgeStage = me.Age;
                sDeathType = me.DeathStyle;

                sFirstName = me.FirstName;
                sLastName = me.LastName;

                if (mode != CASMode.Full)
                {
                    me.SetDeathStyle(SimDescription.DeathType.None, true);
                }

                sNoHouse = (me.Household == null);
                if (sNoHouse)
                {
                    Household.NpcHousehold.Add(sLastSim);
                }

                sInstantiated = (me.CreatedSim != null);
                if (!sInstantiated)
                {
                    if (!Instantiation.EnsureInstantiate(me, LotManager.GetWorldLot()))
                    {
                        return false;
                    }
                }

                sPregnant = me.mCurrentShape.Pregnant;
                me.mCurrentShape.Pregnant = 0;

                if (sPregnant > 0)
                {
                    sNonPregnancyOutfits = me.Outfits;
                    me.mOutfits = me.mMaternityOutfits;
                    me.mMaternityOutfits = new OutfitCategoryMap();
                }

                foreach (OutfitCategories category in Enum.GetValues(typeof(OutfitCategories)))
                {
                    if ((category != OutfitCategories.None) && (category < OutfitCategories.CategoryMask))
                    {
                        try
                        {
                            ArrayList list = me.GetOutfits(category);

                            int index = 0;
                            while (index < list.Count)
                            {
                                SimOutfit outfit = list[index] as SimOutfit;
                                if ((outfit == null) || (!outfit.IsValid))
                                {
                                    list.RemoveAt(index);
                                }
                                else
                                {
                                    index++;

                                    outfit.MorphWeightD = 0;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(category.ToString(), e);

                            RestoreOutfits();
                            return false;
                        }
                    }
                }

                if (me.CreatedSim != null)
                {
                    sActualCategory = me.CreatedSim.CurrentOutfitCategory;
                    sActualIndex = me.CreatedSim.CurrentOutfitIndex;
                }

                displayCategory = sActualCategory;
                displayIndex = sActualIndex;

                if (editType == EditType.Uniform)
                {
                    displayCategory = OutfitCategories.Career;
                    displayIndex = 0;
                }
                else if (mode == CASMode.Tattoo)
                {
                    // Tattoo will create its own outfit to use
                }
                else if ((sActualCategory == OutfitCategories.Singed) ||
                         (sActualCategory == OutfitCategories.Makeover) ||
                         (sActualCategory == OutfitCategories.SkinnyDippingTowel))
                {
                    displayCategory = OutfitCategories.Everyday;
                    displayIndex = 0;
                }
                else if (!IsSpecialEdit())
                {
                    // Do Nothing
                }
                else if (me.Outfits == null)
                {
                    RestoreOutfits();
                    return false;
                }
                else if ((mode != CASMode.Full) || (sActualCategory == OutfitCategories.Naked))
                {
                    if (me.IsHuman)
                    {
                        ArrayList list = me.Outfits[sActualCategory] as ArrayList;
                        if ((list != null) && (list.Count > 0))
                        {
                            me.Outfits[sActualCategory] = me.Outfits[OutfitCategories.Everyday] as ArrayList;
                            me.Outfits[OutfitCategories.Everyday] = list;

                            sSwapOutfits = true;
                        }
                    }

                    displayCategory = OutfitCategories.Everyday;
                    displayIndex = 0;
                }
                else
                {
                    sActualCategory = OutfitCategories.Everyday;
                    sActualIndex = 0;

                    displayCategory = OutfitCategories.Everyday;
                    displayIndex = 0;
                }

                if (me.GetOutfit(displayCategory, displayIndex) == null)
                {
                    RestoreOutfits();
                    return false;
                }

                OccultTypeHelper.TestAndRebuildWerewolfOutfit(me);
            }

            sInitialized = false;

            CASLogic singleton = CASLogic.GetSingleton();
            singleton.CASMode = mode;
            singleton.PreviewSimIndex = 0;// -1;
            singleton.UseTempSimDesc = true;
            singleton.IsEditingUniform = (editType == EditType.Uniform);
            singleton.IsEditingMannequin = (editType == EditType.Mannequin);

            if (me != null)
            {
                singleton.LoadSim(me, displayCategory, displayIndex);
            }

            singleton.ClosingDown += OnClosingDown;
            singleton.ShowUI += OnShowUI;
            singleton.OnSimReplaced += OnSimReplaced;         
            singleton.OnSimOutfitCategoryChanged += OnSimOutfitCategoryChanged;
            singleton.OnSimOutfitIndexChanged += OnSimOutfitIndexChanged;
            singleton.OnCASPartAdded += OnCASPartAdded;
            singleton.OnCASPartRemoved += OnCASPartRemoved;
            singleton.OnSimAgeGenderChanged += OnSimAgeGenderChanged;
            singleton.OnSimSpeciesChanged += OnSimSpeciesChanged;
            singleton.OccultTypeSelected += OnOccultTypeChanged;
            singleton.OnSimLoaded += OnSimLoaded;
            //singleton.OnSimUpdated += OnSimUpdated;
            singleton.OnPresetAppliedToPart += OnPresetAppliedToPart;
            singleton.OnSimAddedToHousehold += OnSimAddedToHousehold;
            singleton.OnSimPreviewChange += OnSimPreviewChange;
            singleton.OnHairPresetApplied += OnHairPresetApplied;
            singleton.OnSetHairColor += OnSetHairColor;
            singleton.UndoSelected += OnUndoSelected;
            singleton.RedoSelected += OnRedoSelected;
            CASLogic.EditPart += OnEditPart;

            switch(mode)
            {
                case CASMode.Dresser:
                    GameStates.TransitionToCASDresserMode();
                    break;
                case CASMode.Mirror:            
                    GameStates.TransitionToCASMirrorMode();
                    break;
                case CASMode.Tattoo:
                    GameStates.TransitionToCASTattooMode();
                    break;
                case CASMode.Stylist:
                    GameStates.TransitionToCASStylistMode();
                    break;
                case CASMode.Full:
                    GameStates.TransitionToCASMode();
                    break;
                case CASMode.Collar:
                    GameStates.TransitionToCASCollarMode();
                    break;
                case CASMode.Tack:
                    GameStates.TransitionToCASTackMode();
                    break;
                case CASMode.PlasticSurgeryFace:
                    GameStates.TransitionToCASSurgeryFaceMode();
                    break;
                case CASMode.PlasticSurgeryBody:
                    GameStates.TransitionToCASSurgeryBodyMode();
                    break;
                case CASMode.Mermaid:
                    GameStates.TransitionToCASMermaidMode();
                    break;
                case CASMode.CreateABot:
                    GameStates.TransitionToCABotMode(false);
                    break;
                case CASMode.EditABot:
                    GameStates.TransitionToCABotMode(true);
                    break;
            }

            return true;
        }

        protected static void OnSetHairColor(BodyTypes type)
        {
            try
            {
                StoreChanges();
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static void OnHairPresetApplied(BodyTypes type, ColorInfo hairColors)
        {
            try
            {
                StoreChanges();
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static void ReinjectActiveColoring(BodyTypes type)
        {
            CASLogic singleton = CASLogic.Instance;

            switch (type)
            {
                case BodyTypes.Hair:
                    OutfitUtils.InjectHairColor(singleton.mBuilder, singleton.mCurrentSimData.ActiveHairColors, BodyTypes.Hair);
                    break;
                case BodyTypes.Beard:
                    OutfitUtils.InjectHairColor(singleton.mBuilder, singleton.mCurrentSimData.ActiveFacialHairColors, BodyTypes.Beard);
                    break;
                case BodyTypes.Eyebrows:
                    OutfitUtils.InjectEyeBrowHairColor(singleton.mBuilder, singleton.mCurrentSimData.ActiveEyebrowColor);
                    break;
                default:
                    if (CASParts.BodyHairTypes.Contains(type))
                    {
                        OutfitUtils.InjectBodyHairColor(singleton.mBuilder, singleton.mCurrentSimData.ActiveBodyHairColor);
                    }
                    break;
            }
        }

        protected static void OnPresetAppliedToPart(CASPart part, string preset)
        {
            try
            {
                ReinjectActiveColoring(part.BodyType);

                StoreChanges();
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        } 

        protected static void OnUndoSelected()
        {
            try
            {
                StoreChanges();
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static void OnRedoSelected()
        {
            try
            {
                StoreChanges();
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static void OnEditPart()
        {
            try
            {
                StoreChanges();
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static void OnSimOutfitIndexChanged(int index)
        {
            try
            {
                if ((CASTattoo.gSingleton == null) && (CASBodyHair.gSingleton == null))
                {
                    if (CASLogic.GetSingleton().mCurrentOutfitCategory == OutfitCategories.Everyday)
                    {
                        sAddedOutfits.Remove(index);
                    }
                }

                if (index == sPreviousKey.GetIndex()) return;

                ChangedOutfit();
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static void OnSimOutfitCategoryChanged(OutfitCategories outfitCategory)
        {
            try
            {
                ChangedOutfit();
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static void OnCASPartAdded(CASPart part)
        {
            try
            {
                ReinjectActiveColoring(part.BodyType);

                StoreChanges();
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static void OnCASPartRemoved(CASPart part)
        {
            try
            {
                //Common.DebugStackLog(part.ToString());

                StoreChanges(false);
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static void OnSimReplaced(int simIndex)
        {
            try
            {
                CASPuck.Instance.mSimBinButton.Visible = true;

                CASLogic singleton = CASLogic.Instance;

                SimDescriptionCore sim = singleton.mCurrentSimData;

                if (sLastSim != null)
                {
                    sim.mFirstName = sLastSim.mFirstName;
                    sim.mLastName = sLastSim.mLastName;
                    sim.TraitManager = new TraitManager(sLastSim.TraitManager);
                    sim.LifetimeWish = sLastSim.LifetimeWish;
                    sim.Zodiac = sLastSim.Zodiac;
                    sim.mFavouriteColor = sLastSim.mFavouriteColor;
                    sim.mFavouriteFoodType = sLastSim.mFavouriteFoodType;
                    sim.mFavouriteMusicType = sLastSim.mFavouriteMusicType;
                }
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static void OnSimAddedToHousehold(int simIndex)
        {
            try
            {
                sPreviousSim = CASLogic.Instance.mCurrentSimData;

                ChangedOutfit();
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        public static void RemoveOutfits(SimDescriptionCore sim)
        {
            if ((!sSwapOutfits) && (CASLogic.Instance.CASMode == CASMode.Full))
            {
                sOutfits.Remove(sim);

                //Common.DebugStackLog(delegate { return "RemoveOutfits"; });
            }
        }

        protected static void OnSimPreviewChange(int simIndex)
        {
            try
            {
                if (sPreviousSim != null)
                {
                    sPreviousSim.FirstName = sFirstName;
                    sPreviousSim.LastName = sLastName;
                }

                sPreviousSim = CASLogic.Instance.mCurrentSimData;

                sFirstName = sPreviousSim.FirstName;
                sLastName = sPreviousSim.LastName;

                ChangedOutfit();
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        public static void ChangeName(string firstName, string lastName)
        {
            sFirstName = firstName;
            sLastName = lastName;
        }

        /*
        protected static void OnSimUpdated(int simIndex)
        {
            try
            {
                if (simIndex != -1)
                {
                    sPreviousSim = CASLogic.Instance.GetSimsInHousehold()[simIndex] as SimDescriptionCore;

                    RemoveOutfits(sPreviousSim);
                }
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }
        */
        protected static void OnSimLoaded(ResourceKey key)
        {
            try
            {
                sPreviousSim = CASLogic.Instance.mCurrentSimData;

                RemoveOutfits(sPreviousSim);
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static void OnSimAgeGenderChanged(CASAgeGenderFlags age, CASAgeGenderFlags gender)
        {
            try
            {
                RemoveOutfits(CASLogic.Instance.mCurrentSimData);
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static void OnSimSpeciesChanged(CASAgeGenderFlags species)
        {
            try
            {
                RemoveOutfits(CASLogic.Instance.mCurrentSimData);
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static void OnOccultTypeChanged(OccultTypes type)
        {
            try
            {
                RemoveOutfits(CASLogic.Instance.mCurrentSimData);
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        public static void RecacheOutfits()
        {
            CASLogic logic = CASLogic.Instance;

            sOutfits.Remove(logic.mCurrentSimData);
            GetOutfits(logic.mCurrentSimData);
        }

        protected static SavedOutfit.Cache GetOutfits()
        {
            return GetOutfits(CASLogic.Instance.mCurrentSimData);
        }
        protected static SavedOutfit.Cache GetOutfits(SimDescriptionCore sim)
        {
            SavedOutfit.Cache outfits;
            if (!sOutfits.TryGetValue(sim, out outfits))
            {
                outfits = new SavedOutfit.Cache();
                sOutfits.Add(sim, outfits);

                //Common.DebugStackLog(delegate { return "GetOutfits"; });
            }

            return outfits;
        }

        protected static void OnSimNameChanged(string firstName, string lastName, CASAgeGenderFlags age, CASAgeGenderFlags gender, string astrologySign)
        {
            try
            {
                try
                {
                    CASPuck puck = CASPuck.Instance;
                    if (puck != null)
                    {
                        puck.UpdateCurrentSimTooltip();
                    }
                }
                catch (Exception e)
                {
                    Common.DebugException(firstName + " " + lastName + Common.NewLine + "PreviewSimIndex: " + Responder.Instance.CASModel.PreviewSimIndex + Common.NewLine + "NumInHousehold: " + Responder.Instance.CASModel.NumInHousehold, e);
                }

                CASBasics basics = CASBasics.gSingleton;
                if (basics != null)
                {
                    if (basics.mFirstNameTextEdit.MaxTextLength == 256)
                    {
                        if (basics.mFirstNameTextEdit != null)
                        {
                            sFirstName = firstName;
                        }
                    }
                    else
                    {
                        basics.mFirstNameTextEdit.MaxTextLength = 256;

                        if ((!string.IsNullOrEmpty(sFirstName)) && (!string.IsNullOrEmpty(firstName)) && (sFirstName.StartsWith(firstName)))
                        {
                            basics.mFirstNameTextEdit.Caption = sFirstName;
                        }
                    }

                    if (basics.mLastNameTextEdit.MaxTextLength == 256)
                    {
                        if (basics.mLastNameTextEdit != null)
                        {
                            sLastName = lastName;
                        }
                    }
                    else
                    {
                        basics.mLastNameTextEdit.MaxTextLength = 256;

                        if ((!string.IsNullOrEmpty(sLastName)) && (!string.IsNullOrEmpty(lastName)) && (sLastName.StartsWith(lastName)))
                        {
                            basics.mLastNameTextEdit.Caption = sLastName;
                        }
                    }
                }
                else
                {
                    CAPBasics capBasics = CAPBasics.gSingleton;
                    if (capBasics != null)
                    {
                        if (capBasics.mFirstNameTextEdit.MaxTextLength == 256)
                        {
                            if (capBasics.mFirstNameTextEdit != null)
                            {
                                sFirstName = firstName;
                            }
                        }
                        else
                        {
                            capBasics.mFirstNameTextEdit.MaxTextLength = 256;
                               
                            if ((!string.IsNullOrEmpty(sFirstName)) && (!string.IsNullOrEmpty(firstName)) && (sFirstName.StartsWith(firstName)))
                            {
                                capBasics.mFirstNameTextEdit.Caption = sFirstName;
                            }
                        }

                        if (capBasics.mLastNameTextEdit.MaxTextLength == 256)
                        {
                            if (capBasics.mLastNameTextEdit != null)
                            {
                                sLastName = lastName;
                            }
                        }
                        else
                        {
                            capBasics.mLastNameTextEdit.MaxTextLength = 256;

                            if ((!string.IsNullOrEmpty(sLastName)) && (!string.IsNullOrEmpty(lastName)) && (sLastName.StartsWith(lastName)))
                            {
                                capBasics.mLastNameTextEdit.Caption = sLastName;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        } 

        protected static bool ApplyGlobalChanges(SimBuilder builder, SavedOutfit.Cache outfits, OutfitCategories category)
        {
            List<BodyTypes> types = new List<BodyTypes>();
            types.Add(BodyTypes.Tattoo);

            types.AddRange(CASParts.GeneticBodyTypes);

            if (MasterController.Settings.mEyeColorByCategory)
            {
                types.Remove(BodyTypes.EyeColor);
            }

            if (Sims3.UI.Responder.Instance.CASModel.PropagateHairStyles)
            {
                switch (category)
                {
                    case OutfitCategories.Career:
                    case OutfitCategories.Singed:
                    case OutfitCategories.ChildImagination:
                    case OutfitCategories.Special:
                        break;
                    default:
                        //if (MasterController.Settings.mUniGenderHair)
                        {
                            types.Add(BodyTypes.Hair);
                        }
                        break;
                }

                //if (MasterController.Settings.mBeardsForAll)
                {
                    types.Add(BodyTypes.Beard);
                }
            }

            if (Sims3.UI.Responder.Instance.CASModel.PropagateMakeUpStyles)
            {
                types.AddRange(CASParts.sMakeup);
            }

            return outfits.Apply(builder, sPreviousKey, !MasterController.Settings.mHairColorByOutfit, types, null);
        }

        protected static bool ApplyFullChanges(SimBuilder builder, SavedOutfit.Cache outfits, CASParts.Key key, bool final)
        {
            // Cross-Category parts
            bool result = ApplyGlobalChanges(builder, outfits, key.mCategory);

            // Everything else
            if ((CASLogic.Instance.CASMode != CASMode.Tattoo) || (final))
            {
                List<BodyTypes> types = new List<BodyTypes>();

                if (MasterController.Settings.mEyeColorByCategory)
                {
                    types.Add(BodyTypes.EyeColor);
                }

                types.AddRange(sCachedTypes);

                if (!Sims3.UI.Responder.Instance.CASModel.PropagateMakeUpStyles)
                {
                    types.AddRange(CASParts.sMakeup);
                }

                if (!Sims3.UI.Responder.Instance.CASModel.PropagateHairStyles)
                {
                    types.Add(BodyTypes.Hair);
                    types.Add(BodyTypes.Beard);
                }

                return outfits.Apply(builder, key, MasterController.Settings.mHairColorByOutfit, types, null);
            }

            return result;
        }

        public static void ApplyAllChanges()
        {
            CASLogic singleton = CASLogic.GetSingleton();
            if (singleton.CASMode == CASMode.Mermaid) return;

            using (SimBuilder builder = new SimBuilder())
            {
                if (sLastSim != null)
                {
                    SavedOutfit.Cache cache;
                    if (sOutfits.TryGetValue(singleton.mCurrentSimData, out cache))
                    {
                        ApplyAllChanges(builder, sLastSim, cache);
                    }
                }
                else
                {
                    foreach (KeyValuePair<SimDescriptionCore, SavedOutfit.Cache> sims in sOutfits)
                    {
                        ApplyAllChanges(builder, sims.Key, sims.Value);
                    }
                }
            }
        }
        protected static void ApplyAllChanges(SimBuilder builder, SimDescriptionCore sim, SavedOutfit.Cache outfits)
        {
            Common.StringBuilder msg = new Common.StringBuilder("ApplyAllChanges" + Common.NewLine + sim.FullName);

            try
            {
                foreach (SavedOutfit.Cache.Key outfit in outfits.Outfits)
                {
                    bool applyFull = false;

                    msg.Append(Common.NewLine + outfit);

                    SimOutfit origOutfit = CASParts.GetOutfit(sim, outfit.mKey, false);

                    switch (outfit.Category)
                    {
                        case OutfitCategories.Everyday:
                            if (outfit.Index != 0) continue;
                            break;
                        case OutfitCategories.Formalwear:
                        case OutfitCategories.Swimwear:
                        case OutfitCategories.Sleepwear:
                        case OutfitCategories.Athletic:
                        case OutfitCategories.Outerwear:
                            // User can change the number of outfits for these categories during CAS
                            if (origOutfit == null) continue;
                            break;
                        case OutfitCategories.Supernatural:
                            // Special category, is not a true outfit
                            continue;
                        default:
                            applyFull = true;
                            break;
                    }

                    if ((origOutfit == null) || (applyFull))
                    {
                        origOutfit = GetValidOutfit(sim, outfit.Category);
                        applyFull = true;

                        if (origOutfit == null)
                        {
                            msg.Append(Common.NewLine + "  No Valid Outfit");
                            continue;
                        }
                    }

                    builder.Clear(false);
                    OutfitUtils.SetOutfit(builder, origOutfit, sim);

                    if (applyFull)
                    {
                        msg.Append(Common.NewLine + "  ApplyFull");

                        builder.RemoveParts(sApplyAllNotTypes.ToArray());

                        List<BodyTypes> notTypes = null;
                        if (outfit.Category == OutfitCategories.Naked)
                        {
                            notTypes = new List<BodyTypes>(sApplyAllNotTypes);
                        }

                        outfit.Apply(builder, MasterController.Settings.mHairColorByOutfit, null, notTypes);

                        if (!ApplyGlobalChanges(builder, outfits, outfit.Category)) continue;
                    }
                    else
                    {
                        msg.Append(Common.NewLine + "  ApplyChanges");

                        //msg.Append(Common.NewLine + outfits.Load(outfit.mKey).ToString());

                        if (!ApplyFullChanges(builder, outfits, outfit.mKey, true)) continue;
                    }

                    msg.Append(Common.NewLine + "  Replace");

                    CASParts.ReplaceOutfit(sim, outfit.mKey, builder, false);

                    //msg.Append(Common.NewLine + new SavedOutfit(builder).ToString());
                }

                Common.DebugWriteLog(msg);
            }
            catch (Exception e)
            {
                Common.Exception(sim, null, msg.ToString(), e);
            }
        }

        protected static bool IsUnderwearMode()
        {
            CASLogic singleton = CASLogic.Instance;

            CASTattoo tattoo = CASTattoo.gSingleton;
            if ((tattoo != null) && (tattoo.Visible) && (singleton.CASMode != CASMode.Tattoo))
            {
                return true;
            }
            else
            {
                CASBodyHair bodyHair = CASBodyHair.gSingleton;
                if ((bodyHair != null) && (bodyHair.Visible))
                {
                    return true;
                }
            }

            return false;
        }

        protected static void ChangedOutfit()
        {
            CASLogic singleton = CASLogic.Instance;

            if (!singleton.mIsEditingTransformedWerewolf)
            {
                if (!IsUnderwearMode())
                {
                    ApplyFullChanges(singleton.mBuilder, GetOutfits(sPreviousSim), new CASParts.Key(singleton.OutfitCategory, singleton.OutfitIndex), false);
                }
                else
                {
                    ApplyGlobalChanges(singleton.mBuilder, GetOutfits(sPreviousSim), singleton.OutfitCategory);

                    ReinjectActiveColoring(BodyTypes.BodyHairUpperChest);
                }

                StoreChanges();
            }

            sPreviousKey = new CASParts.Key(singleton.OutfitCategory, singleton.OutfitIndex);
        }

        public static void StoreChanges()
        {
            StoreChanges(true);
        }
        protected static void StoreChanges(bool applyHairColor)
        {
            CASLogic singleton = CASLogic.Instance;
            if (singleton == null) return;

            if (!singleton.mIsEditingTransformedWerewolf)
            {
                GetOutfits(sPreviousSim).Replace(new CASParts.Key(singleton.OutfitCategory, singleton.OutfitIndex), singleton.mBuilder, applyHairColor);

                if (MasterController.Settings.mHairColorByOutfit)
                {
                    GetOutfits(sPreviousSim).ApplyColors(CASLogic.Instance.mCurrentSimData, new CASParts.Key(singleton.OutfitCategory, singleton.OutfitIndex));
                }
            }
        }

        public static void InitAvailableParts()
        {
            CASLogic singleton = CASLogic.Instance;
            if (singleton == null) return;

            sUnboxedParts = null;

            sFemaleBodyHairInstalled = false;

            OutfitCategories disableFilterCategories = OutfitCategories.None;

            foreach (OutfitCategories category in MasterController.Settings.mDisableClothingFilterV2)
            {
                disableFilterCategories |= category;
            }

            for (int i = 0; i < 3; i++)
            {
                if ((i > 0) || (IsSpecialEdit ()))
                {
                    disableFilterCategories = OutfitCategories.CategoryMask;
                }

                PartTester tester = new PartTester(i, disableFilterCategories, singleton.mBuilder);

                sUnboxedParts = CASParts.GetParts(tester.TestPart);

                if ((sUnboxedParts.Count > 0) && (tester.mValidTop))
                {
                    if (tester.mFemaleBodyHair)
                    {
                        sFemaleBodyHairInstalled = true;
                    }

                    break;
                }
            }

            if ((sUnboxedParts == null) || (sUnboxedParts.Count == 0))
            {
                sUnboxedParts = CASParts.GetParts(null);
            }

            switch (MasterController.Settings.mClothingSortOrder)
            {
                case SortOrder.EAStandard:
                    sUnboxedParts.Sort(new EASorter());
                    break;
                case SortOrder.CustomContentTop:
                    sUnboxedParts.Sort(new CustomSorter());
                    break;
                case SortOrder.CustomContentBottom:
                    sUnboxedParts.Sort(new CustomReverseSorter());
                    break;
            }

            singleton.mCasParts = new ArrayList();

            sHairParts = new List<CASParts.Wrapper>();

            switch (singleton.CASMode)
            {
                case CASMode.CreateABot:
                case CASMode.EditABot:
                    foreach (CASParts.Wrapper wrapper in sUnboxedParts)
                    {
                        singleton.mCasParts.Add(wrapper.mPart);
                    }
                    break;
                default:
                    foreach (CASParts.Wrapper wrapper in sUnboxedParts)
                    {
                        if (wrapper.mPart.BodyType == BodyTypes.Hair)
                        {
                            // Delayed to stop EA from listing parts without my say-so
                            sHairParts.Add(wrapper);
                        }
                        else
                        {
                            singleton.mCasParts.Add(wrapper.mPart);
                        }
                    }
                    break;
            }
        }

        protected static void OnShowUI(bool toShow)
        {
            try
            {
                if (!toShow) return;

                CASLogic singleton = CASLogic.GetSingleton();

                CASPuck casPuck = CASPuck.Instance;
                if (casPuck != null)
                {
                    if ((singleton.CASMode == CASMode.Full) && (casPuck.mSimBinButton != null))
                    {                
                        casPuck.mSimBinButton.Visible = true;
                    }

                    if (singleton.CurrentOccultType == OccultTypes.Mermaid)
                    {
                        casPuck.mSimOptionsButton.Enabled = true;
                        casPuck.mSimOptionsButton.TooltipText = null;
                        casPuck.mSaveSimButton.Enabled = true;
                        casPuck.mSaveSimButton.TooltipText = null;
                        casPuck.mShareSimButton.Enabled = true;
                        casPuck.mShareSimButton.TooltipText = null;
                    }

                    switch (singleton.CASMode)
                    {
                        case CASMode.Full:
                        case CASMode.Mirror:
                        case CASMode.Stylist:
                            if ((sLastSim != null) && (!IsSpecialEdit ()))
                            {
                                if (casPuck.mSimButtons != null)
                                {
                                    if (casPuck.mSimButtons[0] != null)
                                    {
                                        casPuck.mSimButtons[0].Visible = !IsUnderwearMode();
                                    }

                                    casPuck.UpdateWerewolfButton(sLastSim.IsWerewolf ? OccultTypes.Werewolf : singleton.CurrentOccultType);
                                    //CASPuck.UpdateWerewolfButton();
                                }
                            }
                            break;
                    }
                }

                CASCharacterSheet character = CASCharacterSheet.gSingleton;
                if (character != null)
                {
                    if ((character.mButtons != null) && (character.mButtons.Count >= 6))
                    {
                        character.mButtons[5].Enabled = true;
                        character.mButtons[5].TooltipText = character.mTextWindow.Visible ? string.Empty : (character.mButtons[5].Tag as string);
                    }
                }

                CASDresserSheet dresser = CASDresserSheet.gSingleton;
                if (dresser != null)
                {
                    Window newWindow, oldWindow;
                    CASDresserSheetEx.GetWindows(out newWindow, out oldWindow);
                    if (newWindow != null)
                    {
                        if ((oldWindow.Visible) && (!newWindow.Visible))
                        {
                            if (oldWindow != null)
                            {
                                oldWindow.Visible = false;
                                oldWindow.EffectFinished -= dresser.OnPanelGlideFinished;
                            }

                            newWindow.Visible = true;

                            CASDresserSheetEx.UpdateGlideEffects(newWindow);
                        }

                        CASDresserSheetEx.UpdatePanelGlide(newWindow);

                        if (Responder.Instance.CASModel.CASMode == CASMode.Stylist)
                        {
                            Button button = newWindow.GetChildByID(0x777023, true) as Button;
                            if (button != null)
                            {
                                button.Click -= dresser.OnBackButtonClick;
                                button.Click += dresser.OnBackButtonClick;
                            }

                            Window window2 = newWindow.GetChildByID(0x1, false) as Window;
                            window2.Visible = true;
                        }
                    }

                    if (dresser.mButtons != null)
                    {
                        if (dresser.mButtons[6] != null)
                        {
                            dresser.mButtons[6].Area = new Rect(2, 341, 58, 397);
                        }

                        if (dresser.mButtonText[6] != null)
                        {
                            dresser.mButtonText[6].Area = new Rect(0, 352, 109, 387);
                        }
                    }
                }

                if ((singleton.mIsEditingTransformedWerewolf) || (IsSpecialEdit()) || (singleton.IsEditingUniform))
                {
                    if ((dresser != null) && (!singleton.IsEditingUniform))
                    {
                        if ((dresser.mButtons != null) && (dresser.mButtonText != null))
                        {
                            for (int i = 1; i < dresser.mButtons.Length; i++)
                            {
                                if (dresser.mButtons[i] != null)
                                {
                                    dresser.mButtons[i].Visible = false;
                                }

                                if (dresser.mButtonText[i] != null)
                                {
                                    dresser.mButtonText[i].Visible = false;
                                }
                            }

                            if (IsSpecialEdit())
                            {
                                dresser.mButtonText[0].Caption = OutfitBase.Item.GetCategoryName(sActualCategory);
                            }
                        }

                        dresser.mNumButtons = 1;
                    }

                    CASDresserClothing clothing = CASDresserClothing.gSingleton;
                    if (clothing != null)
                    {
                        if (clothing.mAddOutfitButton != null)
                        {
                            clothing.mAddOutfitButton.Visible = false;
                        }

                        if (sActualCategory == OutfitCategories.Special)
                        {
                            if (clothing.mDeleteOutfitButtons != null)
                            {
                                foreach (Button button in clothing.mDeleteOutfitButtons)
                                {
                                    button.Visible = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if ((dresser != null) && (dresser.mButtons != null))
                    {
                        if (sLastSim != null)
                        {
                            if (sLastSim.Age == CASAgeGenderFlags.Toddler)
                            {
                                if (dresser.mButtons[4] != null)
                                {
                                    dresser.mButtons[4].Visible = true;
                                }

                                if (dresser.mButtonText[4] != null)
                                {
                                    dresser.mButtonText[4].Visible = true;
                                }

                                if (GameUtils.IsInstalled(ProductVersion.EP8))
                                {
                                    dresser.mNumButtons = 7;
                                }
                                else
                                {
                                    dresser.mNumButtons = 5;
                                }
                            }
                        }

                        if ((sLastSim != null) &&
                            ((sLastSim.Occupation != null) || 
                            ((sLastSim.CareerManager != null) && (sLastSim.CareerManager.School != null)) || 
                            (sLastSim.AssignedRole != null) || 
                            (SimTypes.InServicePool(sLastSim))))
                        {
                            if (dresser.mButtons[5] != null)
                            {
                                dresser.mButtons[5].Visible = true;
                            }

                            if (dresser.mButtonText[5] != null)
                            {
                                dresser.mButtonText[5].Visible = true;
                            }

                            if (GameUtils.IsInstalled(ProductVersion.EP8))
                            {
                                dresser.mNumButtons = 7;
                            }
                            else
                            {
                                dresser.mNumButtons = 6;
                            }
                        }
                    }
                }

                if (singleton.mIsEditingTransformedWerewolf)
                {
                    CASDresserClothing dresserClothing = CASDresserClothing.gSingleton;
                    if (dresserClothing != null)
                    {
                        if (dresserClothing.mOutfitButtons != null)
                        {
                            foreach (Button button in dresserClothing.mOutfitButtons)
                            {
                                button.Enabled = false;
                            }
                        }
                    }

                    CASClothing clothing = CASClothing.gSingleton;
                    if (clothing != null)
                    {
                        if (clothing.mEverydayButton != null)
                        {
                            clothing.mEverydayButton.Enabled = false;
                        }

                        if (clothing.mFormalButton != null)
                        {
                            clothing.mFormalButton.Enabled = false;
                        }

                        if (clothing.mExerciseButton != null)
                        {
                            clothing.mExerciseButton.Enabled = false;
                        }

                        if (clothing.mSleepwearButton != null)
                        {
                            clothing.mSleepwearButton.Enabled = false;
                        }

                        if (clothing.mSwimwearButton != null)
                        {
                            clothing.mSwimwearButton.Enabled = false;
                        }

                        if (clothing.mCareerButton != null)
                        {
                            clothing.mCareerButton.Enabled = false;
                        }

                        if (clothing.mOuterwearButton != null)
                        {
                            clothing.mOuterwearButton.Enabled = false;
                        }
                    }

                    CASClothingCategory category = CASClothingCategory.gSingleton;
                    if (category != null)
                    {
                        if (CASClothingCategory.CurrentTypeCategory != CASClothingCategory.Category.Accessories)
                        {
                            category.SetTypeCategory(CASClothingCategory.Category.Accessories);
                        }

                        if (category.mOutfitsButton != null)
                        {
                            category.mOutfitsButton.Visible = false;
                        }

                        if (category.mTopsButton != null)
                        {
                            category.mTopsButton.Visible = false;
                        }

                        if (category.mBottomsButton != null)
                        {
                            category.mBottomsButton.Visible = false;
                        }

                        if (category.mShoesButton != null)
                        {
                            category.mShoesButton.Visible = false;
                        }
                    }
                }
                else
                {
                    if (singleton.CurrentSimDescription.Age == CASAgeGenderFlags.Toddler)
                    {
                        CASClothing clothing = CASClothing.gSingleton;
                        if (clothing != null)
                        {
                            if (clothing.mSwimwearButton != null)
                            {
                                clothing.mSwimwearButton.Visible = true;
                            }
                        }
                    }
                }

                CASPhysical physical = CASPhysical.gSingleton;
                if (physical != null)
                {
                    if ((physical.mBeardButton != null) && (MasterController.Settings.mBeardsForAll))
                    {
                        physical.mBeardButton.Visible = true;
                    }

                    if ((sFemaleBodyHairInstalled) && (physical.mBodyHairButton != null))
                    {
                        physical.mBodyHairButton.Visible = true;
                    }
                }

                CASFacialDetails details = CASFacialDetails.gSingleton;
                if (details != null)
                {
                    if (details.mTattooButton != null)
                    {
                        details.mTattooButton.Visible = true;
                    }
                }

                bool wasInitialized = sInitialized;
                if (!sInitialized)
                {
                    CASCompositorController compositor = CASCompositorController.Instance;
                    if (compositor != null)
                    {
                        CASController controller = CASController.gSingleton;
                        if (controller != null)
                        {
                            compositor.ExitFullEditMode -= controller.OnExitFullEditMode;
                            compositor.ExitFullEditMode -= CASControllerEx.OnExitFullEditMode;
                            compositor.ExitFullEditMode += CASControllerEx.OnExitFullEditMode;
                        }
                    }

                    if (sLastSim != null)
                    {
                        // Stops NumInHousehold from returning anything other than "0"
                        singleton.mHouseholdScriptHandle = Simulator.kBadObjectGuid;
                        singleton.CreateHousehold();

                        sHousehold = Simulator.GetProxy(singleton.mHouseholdScriptHandle).Target as Household;
                        sHousehold.Add(singleton.mCurrentSimData as SimDescription);

                        singleton.PropagateHairStyles = sLastSim.PropagateHairStyle;
                        singleton.PropagateMakeUpStyles= sLastSim.PropagateMakeupStyle;
                    }

                    sInitialized = true;

                    InitAvailableParts();

                    sPreviousKey = new CASParts.Key(singleton.OutfitCategory, singleton.OutfitIndex);
                    sPreviousSim = singleton.mCurrentSimData;

                    if (sLastSim != null)
                    {
                        sWasCanceled = false;

                        SavedOutfit.Cache outfits = new SavedOutfit.Cache(sLastSim);
                        sOutfits[singleton.mCurrentSimData] = outfits;

                        if (MasterController.Settings.mHairColorByOutfit)
                        {
                            outfits.ApplyColors(singleton.mCurrentSimData, new CASParts.Key(singleton.OutfitCategory, singleton.OutfitIndex));
                        }
                    }

                    if (CASTattoo.gSingleton != null)
                    {
                        CASTattoo.gSingleton.mTattooParts = CASTattoo.gSingleton.GetVisibleParts(BodyTypes.Tattoo);
                        CASTattoo.gSingleton.SetActiveTattooType(CASTattoo.gSingleton.mActiveTattooID, true);
                    }
                }

                if (sFacialPanels == null)
                {
                    sFacialPanels = Common.DerivativeSearch.Find<ICASFacialBlendPanelAdjustment>();
                    foreach (ICASFacialBlendPanelAdjustment panel in sFacialPanels)
                    {
                        panel.Reset();
                    }
                }

                foreach (ICASFacialBlendPanelAdjustment panel in sFacialPanels)
                {
                    panel.Perform();
                }

                if (OnExternalShowUI != null)
                {
                    OnExternalShowUI(wasInitialized);
                }

                CASBasics basics = CASBasics.gSingleton;
                if (basics != null)
                {
                    AlterSlider(basics.mMuscleDefinitionSlider);
                    AlterSlider(basics.mThinHeavySlider);
                    AlterSlider(basics.mBreastSizeSlider);
                    AlterSlider(basics.mMuscleSlider);

                    Text text = basics.GetChildByID(0xdbb8cd0, true) as Text;
                    if (text != null)
                    {
                        text.Enabled = true;
                        text.Visible = true;
                    }

                    if (basics.mSupernaturalTypeButton != null)
                    {
                        basics.mSupernaturalTypeButton.Visible = true;
                        basics.mSupernaturalTypeButton.Enabled = true;

                        foreach (uint id in new uint[] { 0xd5f1e20, 0xd5f1e10, 0xd65d490, 0xd5f1df0, 0xd65e050, 0xd672540 })
                        {
                            Button occultButton = basics.GetChildByID(id, true) as Button;
                            if (occultButton == null) continue;

                            occultButton.Enabled = (sPregnant <= 0) && (OccultTypeHelper.IsInstalled((OccultTypes)occultButton.Tag));
                        }
                    }
                }
                else 
                {
                    CAPBasics capBasics = CAPBasics.gSingleton;
                    if (capBasics != null)
                    {
                    }
                }

                CAPCCMBasic ccmBasic = CAPCCMBasic.gSingleton;
                if (ccmBasic != null)
                {
                    if (ccmBasic.mUnicornButton != null)
                    {
                        ccmBasic.mUnicornButton.Visible = true;
                    }
                }

                CAPUnicorn unicorn = CAPUnicorn.gSingleton;
                if (unicorn != null)
                {
                    if ((unicorn.mHornColorsGrid != null) && (unicorn.mHornColorsGrid.Count == 0))
                    {
                        // From CAPUnicorn.Init()
                        unicorn.mCurrentHornColorPreset = new CASPartPreset();
                        ICASModel model = Responder.Instance.CASModel;
                        model.OnHairPresetApplied += unicorn.OnHairPresetApplied;
                        model.OnSetHairColor += unicorn.OnSetHairColor;
                        model.OnSimAddedToHousehold += unicorn.OnSimAddedToHousehold;
                        model.OnSimLoaded += unicorn.OnSimLoaded;
                        model.OnSimRandomized += unicorn.OnSimRandomized;
                        Responder.Instance.CASModel.OnCASContentAdded += unicorn.OnNewCASPartAdded;
                        unicorn.PopulateBeardColors();
                        
                        CAPUnicornEx.PopulateHornColors();
                        
                        CAPCCMBasic.gSingleton.SetLongPanel(false);
                        CAPCCMBasic.gSingleton.SetShortPanelHeight(unicorn.mMainWindowHeighWithScrollbars);
                    }
                }

                CASEyebrows eyebrows = CASEyebrows.gSingleton;
                if (eyebrows != null)
                {
                    if (eyebrows.mEyebrowPresetsGrid != null)
                    {
                        if (eyebrows.mEyebrowPresetsGrid.Count == 0)
                        {
                            CASEyebrowsEx.PopulateEyebrowGrid();
                        }
                    }
                }

                CASTattoo tattoo = CASTattoo.gSingleton;
                if (tattoo != null)
                {
                    if (!object.ReferenceEquals(sTattoo, tattoo))
                    {
                        if (tattoo.mbAppliedUnderwearUniform)
                        {
                            if (ApplyNakedOutfit(tattoo))
                            {
                                sTattoo = tattoo;
                            }
                        }
                    }
                }

                CASBodyHair bodyHair = CASBodyHair.gSingleton;
                if (bodyHair != null)
                {
                    if (!object.ReferenceEquals(sBodyHair, bodyHair))
                    {
                        if (bodyHair.mbAppliedUnderwearUniform)
                        {
                            if (ApplyNakedOutfit(bodyHair))
                            {
                                sBodyHair = bodyHair;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static bool ApplyNakedOutfit(CASUnderwear underwear)
        {
            if (MasterController.Settings.mNakedTattoo)
            {
                if (!CASLogic.CASOperationStack.Instance.UpToDate()) return false;

                CASLogic singleton = CASLogic.GetSingleton ();

                SimOutfit naked = singleton.mCurrentSimData.GetOutfit(OutfitCategories.Naked, 0);
                if (naked != null)
                {
                    underwear.DiscardUnderwear();

                    using (SimBuilder builder = new SimBuilder())
                    {
                        OutfitUtils.SetOutfit(builder, naked, singleton.mCurrentSimData);

                        ResourceKey newOutfit = builder.CacheOutfit(CASParts.GetOutfitName(singleton.mCurrentSimData, OutfitCategories.Naked, false));

                        int outfitCount = singleton.mCurrentSimData.GetOutfitCount(OutfitCategories.Everyday) - 1;

                        singleton.RequestSetUndoBlock();
                        singleton.RequestStateOutfit(OutfitCategories.Everyday, outfitCount, new SimOutfit(newOutfit));

                        if (underwear.mIsWerewolfOutfit)
                        {
                            singleton.RequestReapplyOfWerewolfTransformedOutfit();
                        }

                        sAddedOutfits.Add(outfitCount);

                        underwear.mbAppliedUnderwearUniform = true;
                    }
                }
            }

            return true;
        }

        protected static void AlterSlider(Slider slider)
        {
            if (slider == null) return;

            if (((slider.MinValue == 0) || (slider.MinValue == -256)) && (slider.MaxValue == 256))
            {
                slider.MinValue *= MasterController.Settings.mBodySliderMultiple;

                if (MasterController.Settings.mBodySliderOffset < 0)
                {
                    slider.MinValue += MasterController.Settings.mBodySliderOffset;
                }

                slider.MaxValue *= MasterController.Settings.mBodySliderMultiple;

                if (MasterController.Settings.mBodySliderOffset > 0)
                {
                    slider.MaxValue += MasterController.Settings.mBodySliderOffset;
                }
            }
        }

        protected static void RestoreOutfits()
        {
            if (sSwapOutfits)
            {
                object oldEveryDay = sLastSim.Outfits[sActualCategory];

                sLastSim.Outfits[sActualCategory] = sLastSim.Outfits[OutfitCategories.Everyday];
                sLastSim.Outfits[OutfitCategories.Everyday] = oldEveryDay;

                sSwapOutfits = false;
            }

            sLastSim.mCurrentShape.Pregnant = sPregnant;

            if (sNonPregnancyOutfits != null)
            {
                foreach (OutfitCategories category in Enum.GetValues(typeof(OutfitCategories)))
                {
                    if ((category != OutfitCategories.None) && (category < OutfitCategories.CategoryMask))
                    {
                        ArrayList list = sLastSim.Outfits[category] as ArrayList;
                        if (list == null) continue;

                        for (int i = 0; i < list.Count; i++)
                        {
                            SimOutfit outfit = list[i] as SimOutfit;
                            if (outfit == null) continue;

                            outfit.MorphWeightD = sPregnant;
                        }
                    }
                }

                sLastSim.mMaternityOutfits = sLastSim.Outfits;
                sLastSim.mOutfits = sNonPregnancyOutfits;

                sNonPregnancyOutfits = null;
            }
        }

        protected static SimOutfit GetValidOutfit(SimDescriptionCore sim, OutfitCategories exclusion)
        {
            foreach (OutfitCategories category in sValidChoices)
            {
                if (category == exclusion) continue;

                SimOutfit outfit = CASParts.GetOutfit(sLastSim, new CASParts.Key(category, 0), false);
                if (outfit != null) return outfit;
            }

            return null;
        }

        protected static void OnClosingDown()
        {
            try
            {
                if (sFacialPanels != null)
                {
                    foreach (ICASFacialBlendPanelAdjustment panel in sFacialPanels)
                    {
                        panel.ClosingDown();
                    }
                }

                if (OnExternalClosingDown != null)
                {
                    OnExternalClosingDown();
                }

                CASLogic singleton = CASLogic.GetSingleton();
                singleton.ClosingDown -= OnClosingDown;

                singleton.ShowUI -= OnShowUI;
                singleton.OnSimReplaced -= OnSimReplaced;

                singleton.OnSimOutfitCategoryChanged -= OnSimOutfitCategoryChanged;
                singleton.OnSimOutfitIndexChanged -= OnSimOutfitIndexChanged;
                singleton.OnCASPartAdded -= OnCASPartAdded;
                singleton.OnCASPartRemoved -= OnCASPartRemoved;
                singleton.OnSimAgeGenderChanged -= OnSimAgeGenderChanged;
                singleton.OnSimSpeciesChanged -= OnSimSpeciesChanged;
                singleton.OccultTypeSelected -= OnOccultTypeChanged;
                singleton.OnSimLoaded -= OnSimLoaded;
                //singleton.OnSimUpdated -= OnSimUpdated;
                singleton.OnPresetAppliedToPart -= OnPresetAppliedToPart;
                singleton.OnHairPresetApplied -= OnHairPresetApplied;
                singleton.OnSimAddedToHousehold -= OnSimAddedToHousehold;
                singleton.OnSimPreviewChange -= OnSimPreviewChange;
                singleton.OnSetHairColor -= OnSetHairColor;
                singleton.UndoSelected -= OnUndoSelected;
                singleton.RedoSelected -= OnRedoSelected;
                CASLogic.EditPart -= OnEditPart;

                singleton.IsEditingUniform = false;
                singleton.IsEditingMannequin = false;

                CASController casController = CASController.Singleton;

                if (CASCompositorController.Instance != null)
                {
                    CASCompositorController.Instance.ExitFullEditMode -= CASControllerEx.OnExitFullEditMode;
                }

                casController.OnSimFirstNameChanged -= OnSimNameChanged;
                casController.OnSimLastNameChanged -= OnSimNameChanged;

                bool saved = !sWasCanceled;
                if (sLastSim != null)
                {
                    sLastSim.PropagateHairStyle = singleton.PropagateHairStyles;
                    sLastSim.PropagateMakeupStyle = singleton.PropagateMakeUpStyles;

                    foreach (TraitNames trait in sRewardTraits)
                    {
                        sLastSim.TraitManager.AddElement(trait);
                    }

                    sRewardTraits = null;

                    Basic.PurchaseRewards.UpdateInterface(sLastSim);

                    if (singleton.CASMode != CASMode.Full)
                    {
                        sLastSim.mDeathStyle = sDeathType;
                    }

                    if (!sInstantiated)
                    {
                        try
                        {
                            Sim sim = sLastSim.CreatedSim;

                            sim.FadeOut();
                            sim.RemoveFromWorld();
                            sim.Dispose();
                            sim.Destroy();
                        }
                        catch
                        { }

                        sLastSim.CreatedSim = null;
                    }

                    if ((sNoHouse) && (sLastSim.Household != null))
                    {
                        sLastSim.Household.Remove(sLastSim);
                    }

                    if (sLastSim.Age != sAgeStage)
                    {
                        sLastSim.AgingYearsSinceLastAgeTransition = 0;
                    }
                    else
                    {
                        sLastSim.AgingYearsSinceLastAgeTransition = sYearsUntilTransition;
                    }

                    sLastSim.AgingEnabled = sAgingEnabled;

                    if (saved)
                    {
                        if (singleton.CASMode == CASMode.Tattoo)
                        {
                            for (int i = sLastSim.GetOutfitCount(OutfitCategories.Everyday) - 1; i >= sPreTattooCount; i--)
                            {
                                try
                                {
                                    sLastSim.RemoveOutfit(OutfitCategories.Everyday, i, true);
                                }
                                catch
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    saved = true;
                }

                if (saved)
                {
                    StoreChanges();

                    ApplyAllChanges();
                }

                if (sLastSim != null)
                {
                    RestoreOutfits();

                    if (saved)
                    {
                        for (int i = sLastSim.GetOutfitCount(OutfitCategories.Everyday) - 1; i >= 0; i--)
                        {
                            try
                            {
                                if (sAddedOutfits.Contains(i))
                                {
                                    sLastSim.RemoveOutfit(OutfitCategories.Everyday, i, true);
                                }
                            }
                            catch
                            {
                                break;
                            }
                        }

                        sAddedOutfits.Clear();

                        if ((singleton.CASMode == CASMode.Full) || (singleton.CASMode == CASMode.Tattoo))
                        {
                            if (sPregnant > 0)
                            {
                                // Intentionally taking the non-alternate outfit (require genetics from it)
                                SimOutfit origOutfit = CASParts.GetOutfit(sLastSim, CASParts.sPrimary, false);

                                SavedOutfit.Cache outfits = new SavedOutfit.Cache(sLastSim);

                                List<SavedOutfit.Cache.Key> altOutfits = outfits.AltOutfits;
                                
                                // Some odd interpreted error occurs when using foreach here, resulting in an infinite loop
                                for (int i = 0; i < altOutfits.Count; i++)
                                {
                                    SavedOutfit.Cache.Key outfit = altOutfits[i];

                                    using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sLastSim, outfit.mKey, origOutfit, true))
                                    {
                                        outfit.Apply(builder, MasterController.Settings.mHairColorByOutfit, null, null);

                                        if (!ApplyGlobalChanges(builder.Builder, outfits, outfit.Category))
                                        {
                                            builder.Invalidate();
                                        }
                                    }
                                }
                            }
                        }

                        if (singleton.CASMode == CASMode.Full)
                        {
                            sLastSim.FirstName = sFirstName;
                            sLastSim.LastName = sLastName;

                            if ((sLastSim.Household != null) && (sLastSim.Household.LotHome != null))
                            {
                                if (singleton.CurrentOccultType == OccultTypes.Ghost)
                                {
                                    Urnstones.SimToPlayableGhost(sLastSim, singleton.GhostDeathType);
                                }
                                else if (singleton.CurrentOccultType == OccultTypes.None)
                                {
                                    Urnstones.GhostToSim(sLastSim);
                                }
                            }

                            DelayedOccultTask.Perform(sLastSim, singleton.CurrentOccultType);

                            // Weight change not retained fix
                            sLastSim.mWeightShapeDelta = 0f;
                            sLastSim.mFitnessShapeDelta = 0f;
                            sLastSim.mShapeDeltaMultiplier = 1f;
                            sLastSim.CardioVectorDays = 0f;
                            sLastSim.HybridVectorDays = 0f;

                            sLastSim.ForceSetBodyShape(singleton.GetMorphFat(), singleton.GetMorphFit(), singleton.GetMorphThin(), sPregnant);
                        }
                        else
                        {
                            DelayedWerewolfTask.Perform(sLastSim);
                        }
                    }

                    if (sLastSim.CreatedSim != null)
                    {
                        sLastSim.CreatedSim.mVisibleOutfitInfo.Category = sActualCategory;
                        sLastSim.CreatedSim.mVisibleOutfitInfo.Index = sActualIndex;

                        if (sLastSim.CreatedSim.CurrentOutfitIndex > sLastSim.GetOutfitCount(sLastSim.CreatedSim.CurrentOutfitCategory))
                        {
                            sLastSim.CreatedSim.UpdateOutfitInfo();
                        }

                        sLastSim.CreatedSim.RefreshCurrentOutfit(false);
                        sLastSim.CreatedSim.FlagSet(Sim.SimFlags.CurrentOutfitCanHaveSuntan, SimTemperature.CanSimCurrentOutfitHaveSuntan(sLastSim.CreatedSim));
                    }

                    sLastSim.RemoveOutfits(OutfitCategories.Singed, false);

                    if (sLastSim.CareerOutfitIndex > sLastSim.GetOutfitCount(OutfitCategories.Career))
                    {
                        sLastSim.CareerOutfitIndex = sLastSim.GetOutfitCount(OutfitCategories.Career) - 1;
                    }

                    sLastSim.mDefaultOutfitKey = sLastSim.GetOutfit(OutfitCategories.Everyday, 0).Key;

                    if (sLastSim.CreatedSim != null)
                    {
                        SimOutfit currentOutfit = sLastSim.CreatedSim.CurrentOutfit;
                        if (currentOutfit != null)
                        {
                            ThumbnailManager.GenerateHouseholdSimThumbnail(currentOutfit.Key, currentOutfit.Key.InstanceId, 0x0, ThumbnailSizeMask.Large | ThumbnailSizeMask.ExtraLarge | ThumbnailSizeMask.Medium | ThumbnailSizeMask.Small, ThumbnailTechnique.Default, true, false, sLastSim.AgeGenderSpecies);
                        }

                        (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).NotifySimChanged(sLastSim.CreatedSim.ObjectId);

                        CASChangeReporter.Instance.SendChangedEvents(sLastSim.CreatedSim);

                        if (sLastSim.IsHuman)
                        {
                            EventTracker.SendEvent(EventTypeId.kPlannedOutfit, sLastSim.CreatedSim, sLastSim.CreatedSim);
                        }
                        else
                        {
                            EventTracker.SendEvent(EventTypeId.kPlannedOutfitPet, sLastSim.CreatedSim, sLastSim.CreatedSim);
                        }
                    }
                }

                sLastSim = null;
                sOutfits.Clear();

                if (sTask != null)
                {
                    sTask.Dispose();
                    sTask = null;
                }

                sTattoo = null;
                sBodyHair = null;

                if (sHousehold != null)
                {
                    Simulator.DestroyObject(sHousehold.ObjectId);
                    singleton.mHouseholdScriptHandle = Simulator.kBadObjectGuid;

                    sHousehold = null;
                }

                new RunQueueTask(sMode).AddToSimulator();
            }
            catch (Exception e)
            {
                Common.Exception(sLastSim, e);
            }
        }

        protected static string OutfitsToString(SimDescriptionCore sim)
        {
            string msg = null;

            foreach (OutfitCategories category in Enum.GetValues(typeof(OutfitCategories)))
            {
                ArrayList outfits = CASParts.GetOutfits(sim, category, false);
                if (outfits == null) continue;

                for (int i = 0; i < outfits.Count; i++)
                {
                    SimOutfit outfit = outfits[i] as SimOutfit;

                    msg += Common.NewLine + category + " " + i + " " + (outfit != null) + " " + ((outfit != null) ? outfit.IsValid.ToString() : "null");
                }
            }

            return msg;
        }

        public delegate void Updater();

        public static void Blacklist(CASParts.Wrapper part, bool addToList, Updater updater)
        {
            if (part != null)
            {
                string text = null;

                List<WorldType> worldTypes = new List<WorldType>();
                worldTypes.Add(GameUtils.GetCurrentWorldType());

                SimBuilder builder = CASLogic.Instance.mBuilder;

                InvalidPart invalid = new InvalidPart();
                invalid.Set(CASLogic.Instance.mCurrentOutfitCategory, worldTypes, (OutfitCategoriesExtended)0x0, builder.Age, builder.Gender, builder.Species);

                if (addToList)
                {
                    MasterController.Settings.AddBlacklistPart(part.mPart.Key, invalid);

                    text += Common.Localize("AddToBlacklist:MenuName");
                    text += Common.NewLine + " <Instance>0x" + part.mPart.Key.InstanceId.ToString("X16") + "</Instance>";
                    text += Common.NewLine + invalid.ToString();

                    if (updater != null)
                    {
                        updater();
                    }
                }
                else
                {
                    text += Common.Localize("LogCASParts:MenuName");
                    text += Common.NewLine + CASParts.PartToString(part.mPart);
                    text += Common.NewLine;
                    text += Common.NewLine + invalid.ToXML(part.mPart.Key, "  ");

                    Common.WriteLog(text, false);
                }

                Common.Notify(text);
            }
        }

        protected class EASorter : Comparer<CASParts.Wrapper>
        {
            public override int Compare(CASParts.Wrapper x, CASParts.Wrapper y)
            {
                return -x.mPart.DisplayIndex.CompareTo(y.mPart.DisplayIndex);
            }
        }

        protected class CustomSorter : Comparer<CASParts.Wrapper>
        {
            public override int Compare(CASParts.Wrapper x, CASParts.Wrapper y)
            {
                return x.CompareTo(y);
            }
        }

        protected class CustomReverseSorter : Comparer<CASParts.Wrapper>
        {
            public override int Compare(CASParts.Wrapper x, CASParts.Wrapper y)
            {
                return x.CompareToCustomBottom(y);
            }
        }

        protected class DelayedOccultTask : Common.FunctionTask
        {
            SimDescription mSim;

            OccultTypes mNewType;

            protected DelayedOccultTask(SimDescription sim, OccultTypes newType)
            {
                mSim = sim;
                mNewType = newType;
            }

            public static void Perform(SimDescription sim, OccultTypes newType)
            {
                new DelayedOccultTask(sim, newType).AddToSimulator();
            }

            protected override void OnPerform()
            {
                while (!GameStates.IsLiveState)
                {
                    SpeedTrap.Sleep();
                }

                if (mNewType != OccultTypes.None)
                {
                    if (!mSim.OccultManager.HasOccultType(mNewType))
                    {
                        OccultTypeHelper.Add(mSim, mNewType, false, false);
                    }

                    OccultFairy fairy = mSim.OccultManager.GetOccultType(OccultTypes.Fairy) as OccultFairy;
                    if (fairy != null)
                    {
                        fairy.GrantWings();
                    }

                    DelayedWerewolfTask.Perform(mSim);
                }
                else
                {
                    // Simbots and mummies do not have an Occult buttons in CAS, so appear as human
                    if ((!mSim.IsFrankenstein) && (!mSim.IsMummy))
                    {
                        if (mSim.OccultManager.HasAnyOccultType())
                        {
                            foreach (OccultTypes type in Enum.GetValues(typeof(OccultTypes)))
                            {
                                if (type == OccultTypes.None) continue;

                                OccultTypeHelper.Remove(mSim, type, true);
                            }
                        }
                    }
                }
            }
        }

        protected class DelayedWerewolfTask : Common.FunctionTask
        {
            SimDescription mSim;

            protected DelayedWerewolfTask(SimDescription sim)
            {
                mSim = sim;
            }

            public static void Perform(SimDescription sim)
            {
                new DelayedWerewolfTask(sim).AddToSimulator();
            }

            protected override void OnPerform()
            {
                while (!GameStates.IsLiveState)
                {
                    SpeedTrap.Sleep();
                }

                OccultTypeHelper.RebuildWerewolfOutfit(mSim);
            }
        }

        protected class RunQueueTask : Common.FunctionTask
        {
            OnGetMode mMode;

            public RunQueueTask(OnGetMode mode)
            {
                mMode = mode;
            }

            protected override void OnPerform()
            {
                if (sQueuedSims == null) return;

                if (sQueuedSims.Count == 0) return;

                while (!GameStates.IsLiveState)
                {
                    SpeedTrap.Sleep();
                }

                SimDescription sim = sQueuedSims[0];
                sQueuedSims.RemoveAt(0);

                CASBase.Perform(sim, mMode);
            }
        }

        public class CASTask : RepeatingTask
        {
            CASHair mCASHair;

            protected override bool OnPerform()
            {
                string msg = "A";

                try
                {
                    CASLoadSim loadSim = CASLoadSim.gSingleton;
                    if (loadSim != null)
                    {
                        if (loadSim.mSimGrid != null)
                        {
                            loadSim.mSimGrid.ItemClicked -= loadSim.OnSimGridClicked;
                            loadSim.mSimGrid.ItemClicked -= CASLoadSimEx.OnSimGridClicked;
                            loadSim.mSimGrid.ItemClicked += CASLoadSimEx.OnSimGridClicked;
                        }

                        if (loadSim.mLoadButton != null)
                        {
                            loadSim.mLoadButton.Enabled = true;
                        }
                    }

                    msg += "A1";

                    /*
                    CABBotSheet botSheet = CABBotSheet.gSingleton;
                    if (botSheet != null)
                    {
                        if (botSheet.mButtons != null)
                        {
                            foreach (Button button in botSheet.mButtons)
                            {
                                if (button == null) continue;

                                button.Click -= botSheet.OnNavigationButtonClick;
                                button.Click -= CABBotSheetEx.OnNavigationButtonClick;
                                button.Click += CABBotSheetEx.OnNavigationButtonClick;
                            }
                        }
                    }
                    */

                    /*
                    CASDresserSheet dresserSheet = CASDresserSheet.gSingleton;
                    if (dresserSheet != null)
                    {
                        if (dresserSheet.mButtons != null)
                        {
                            foreach (Button button in dresserSheet.mButtons)
                            {
                                if (button == null) continue;

                                button.Click -= dresserSheet.OnNavigationButtonClick;
                                button.Click -= CASDresserSheetEx.OnNavigationButtonClick;
                                button.Click += CASDresserSheetEx.OnNavigationButtonClick;
                            }
                        }

                        dresserSheet.EffectFinished -= dresserSheet.OnMainGlideFinished;
                        dresserSheet.EffectFinished += dresserSheet.OnMainGlideFinished;
                    }
                    */
                    CASDresserClothing dresser = CASDresserClothing.gSingleton;
                    if (dresser != null)
                    {
                        Responder.Instance.CASModel.OnSimOutfitIndexChanged -= dresser.OnSimOutfitIndexChanged;
                        Responder.Instance.CASModel.OnSimOutfitIndexChanged -= CASDresserClothingEx.OnSimOutfitIndexChanged;
                        Responder.Instance.CASModel.OnSimOutfitIndexChanged += CASDresserClothingEx.OnSimOutfitIndexChanged;

                        if (dresser.mAddOutfitButton != null)
                        {
                            dresser.mAddOutfitButton.Enabled = true;
                        }

                        if (dresser.mOutfitButtons != null)
                        {
                            for (uint i = 0x0; i < 0x3; i++)
                            {
                                dresser.mOutfitButtons[i].Click -= dresser.OnOutfitButtonClick;

                                dresser.mOutfitButtons[i].MouseDown -= CASDresserClothingEx.OnOutfitButtonMouseDown;
                                dresser.mOutfitButtons[i].MouseDown += CASDresserClothingEx.OnOutfitButtonMouseDown;
                            }
                        }
                    }

                    msg += "A2";

                    CASRequiredItemsDialog requiredDialog = CASRequiredItemsDialog.sDialog;
                    if (requiredDialog != null)
                    {
                        if (requiredDialog.mModalDialogWindow != null)
                        {
                            Button randomButton = requiredDialog.mModalDialogWindow.GetChildByID(0x104, true) as Button;
                            if (randomButton != null)
                            {
                                randomButton.Click -= requiredDialog.OnRandomizeNameClick;
                                randomButton.Click -= CASRequiredItemsDialogEx.OnRandomizeNameClick;
                                randomButton.Click += CASRequiredItemsDialogEx.OnRandomizeNameClick;
                            }
                        }

                        if (requiredDialog.mFirstNameTextEdit != null)
                        {
                            requiredDialog.mFirstNameTextEdit.TextChange -= requiredDialog.OnNameTextEditChange;
                            requiredDialog.mFirstNameTextEdit.TextChange -= CASRequiredItemsDialogEx.OnNameTextEditChange;
                            requiredDialog.mFirstNameTextEdit.TextChange += CASRequiredItemsDialogEx.OnNameTextEditChange;
                        }

                        if (requiredDialog.mLastNameTextEdit != null)
                        {
                            requiredDialog.mLastNameTextEdit.TextChange -= requiredDialog.OnNameTextEditChange;
                            requiredDialog.mLastNameTextEdit.TextChange -= CASRequiredItemsDialogEx.OnNameTextEditChange;
                            requiredDialog.mLastNameTextEdit.TextChange += CASRequiredItemsDialogEx.OnNameTextEditChange;
                        }
                    }

                    msg += "A3";

                    CASPuck puck = CASPuck.gSingleton;
                    if (puck != null)
                    {
                        if ((puck.mAcceptButton != null) && (CASLogic.Instance != null))
                        {
                            if ((CASLogic.Instance.CASMode == CASMode.Full) && !CASLogic.Instance.EditingExistingSim())
                            {
                                puck.mAcceptButton.Click -= puck.OnAcceptHousehold;
                                puck.mAcceptButton.Click -= CASPuckEx.OnAcceptHousehold;
                                puck.mAcceptButton.Click += CASPuckEx.OnAcceptHousehold;
                            }
                            else
                            {
                                puck.mAcceptButton.Click -= puck.OnAcceptSim;
                                puck.mAcceptButton.Click -= CASPuckEx.OnAcceptSim;
                                puck.mAcceptButton.Click += CASPuckEx.OnAcceptSim;
                            }
                        }

                        msg += "B";

                        if (puck.mCloseButton != null)
                        {
                            puck.mCloseButton.Click -= puck.OnCloseClick;
                            puck.mCloseButton.Click -= CASPuckEx.OnCloseClick;
                            puck.mCloseButton.Click += CASPuckEx.OnCloseClick;
                        }

                        if (puck.mOptionsButton != null)
                        {
                            puck.mOptionsButton.Click -= puck.OnOptionsClick;
                            puck.mOptionsButton.MouseUp -= CASPuckEx.OnOptionsMouseUp;
                            puck.mOptionsButton.MouseUp += CASPuckEx.OnOptionsMouseUp;
                        }
                    }

                    msg += "C";

                    if ((Responder.Instance != null) && (Responder.Instance.CASModel != null))
                    {
                        Responder.Instance.CASModel.OnSimOutfitCategoryChanged -= CASClothingEx.OnSimOutfitCategoryChanged;

                        CASClothing clothing = CASClothing.gSingleton;
                        if (clothing != null)
                        {
                            Responder.Instance.CASModel.OnSimOutfitCategoryChanged -= clothing.OnSimOutfitCategoryChanged;
                            Responder.Instance.CASModel.OnSimOutfitCategoryChanged += CASClothingEx.OnSimOutfitCategoryChanged;
                        }

                        Responder.Instance.CASModel.UndoSelected -= CASClothingCategoryEx.OnUndoRedo;
                        Responder.Instance.CASModel.RedoSelected -= CASClothingCategoryEx.OnUndoRedo;
                    }

                    msg += "D";

                    CASMakeup makeup = CASMakeup.gSingleton;
                    if (makeup != null)
                    {
                        if (makeup.mGridMakeupParts != null)
                        {
                            makeup.mGridMakeupParts.ItemClicked -= makeup.OnGridPartsClick;
                            makeup.mGridMakeupParts.ItemClicked -= CASMakeupEx.OnGridPartsClick;
                            makeup.mGridMakeupParts.ItemClicked += CASMakeupEx.OnGridPartsClick;
                        }

                        if (makeup.mGridMakeupPresets != null)
                        {
                            makeup.mGridMakeupPresets.ItemClicked -= makeup.OnGridPresetsClick;
                            makeup.mGridMakeupPresets.ItemClicked -= CASMakeupEx.OnGridPresetsClick;
                            makeup.mGridMakeupPresets.ItemClicked += CASMakeupEx.OnGridPresetsClick;
                        }

                        if (makeup.mButtonColor != null)
                        {
                            makeup.mButtonColor.Click -= makeup.OnButtonDesignClick;
                            makeup.mButtonColor.Click -= CASMakeupEx.OnButtonDesignClick;
                            makeup.mButtonColor.Click += CASMakeupEx.OnButtonDesignClick;
                        }

                        if (makeup.mButtonDesignCostume != null)
                        {
                            makeup.mButtonDesignCostume.Click -= makeup.OnButtonDesignClick;
                            makeup.mButtonDesignCostume.Click -= CASMakeupEx.OnButtonDesignClick;
                            makeup.mButtonDesignCostume.Click += CASMakeupEx.OnButtonDesignClick;
                        }

                        if (makeup.mOpacitySlider != null)
                        {
                            makeup.mOpacitySlider.MouseUp -= makeup.OnOpacitySliderMouseUp;
                            makeup.mOpacitySlider.MouseUp -= CASMakeupEx.OnOpacitySliderMouseUp;
                            makeup.mOpacitySlider.MouseUp += CASMakeupEx.OnOpacitySliderMouseUp;

                            makeup.mOpacitySlider.SliderValueChange -= makeup.OnOpacitySliderChange;
                            makeup.mOpacitySlider.SliderValueChange -= CASMakeupEx.OnOpacitySliderChange;
                            makeup.mOpacitySlider.SliderValueChange += CASMakeupEx.OnOpacitySliderChange;
                        }

                        Button costumeMakeup = makeup.GetChildByID(0x104, true) as Button;
                        if (costumeMakeup != null)
                        {
                            costumeMakeup.Click -= makeup.OnButtonTabClick;
                            costumeMakeup.Click -= CASMakeupEx.OnButtonTabClick;
                            costumeMakeup.Click += CASMakeupEx.OnButtonTabClick;
                        }
                    }

                    CASController casController = CASController.Singleton;
                    if (casController != null)
                    {
                        casController.OnSimFirstNameChanged -= CASPuck.Instance.OnSimNameChanged;
                        casController.OnSimFirstNameChanged -= OnSimNameChanged;
                        casController.OnSimFirstNameChanged += OnSimNameChanged;

                        casController.OnSimLastNameChanged -= CASPuck.Instance.OnSimNameChanged;
                        casController.OnSimLastNameChanged -= OnSimNameChanged;
                        casController.OnSimLastNameChanged += OnSimNameChanged;
                    }

                    CASClothingCategory.OnClothingGridFinishedPopulating -= CASClothingCategoryEx.OnClothingGridFinishedPopulating;
                    CASClothingCategory.OnClothingGridFinishedPopulating += CASClothingCategoryEx.OnClothingGridFinishedPopulating;

                    CABPartSelection cabPartSelection = CABPartSelection.gSingleton;
                    if (cabPartSelection != null)
                    {
                        if (!(cabPartSelection.mModel is CASModelProxy))
                        {
                            cabPartSelection.mModel = new CASModelProxy(cabPartSelection.mModel);
                        }
                    }

                    CASClothingCategory category = CASClothingCategory.gSingleton;
                    if (category != null)
                    {
                        if (!(category.mModel is CASModelProxy))
                        {
                            category.mModel = new CASModelProxy(category.mModel);
                        }

                        if ((Responder.Instance != null) && (Responder.Instance.CASModel != null))
                        {
                            Responder.Instance.CASModel.UndoSelected -= category.OnUndoRedo;
                            Responder.Instance.CASModel.RedoSelected -= category.OnUndoRedo;

                            Responder.Instance.CASModel.UndoSelected += CASClothingCategoryEx.OnUndoRedo;
                            Responder.Instance.CASModel.RedoSelected += CASClothingCategoryEx.OnUndoRedo;
                        }

                        if (category.mContentTypeFilter != null)
                        {
                            category.mContentTypeFilter.FiltersChanged -= category.PopulateTypesGrid;
                            category.mContentTypeFilter.FiltersChanged -= CASClothingCategoryEx.DelayedCategoryUpdate.Perform;
                            category.mContentTypeFilter.FiltersChanged += CASClothingCategoryEx.DelayedCategoryUpdate.Perform;
                        }

                        if (category.mDesignButton != null)
                        {
                            category.mDesignButton.Click -= category.OnDesignButtonClick;
                            category.mDesignButton.Click -= CASClothingCategoryEx.OnDesignButtonClick;
                            category.mDesignButton.Click += CASClothingCategoryEx.OnDesignButtonClick;
                        }
                    }

                    msg += "E";

                    if ((Responder.Instance != null) && (Responder.Instance.CASModel != null))
                    {
                        Responder.Instance.CASModel.UndoSelected -= CASHairEx.OnUndo;
                        Responder.Instance.CASModel.RedoSelected -= CASHairEx.OnRedo;
                    }

                    CAPUnicorn unicorn = CAPUnicorn.gSingleton;
                    if (unicorn != null)
                    {
                        if (unicorn.mHornColorsGrid != null)
                        {
                            unicorn.mHornColorsGrid.ItemClicked -= unicorn.OnHornColorGridClicked;
                            unicorn.mHornColorsGrid.ItemClicked -= CAPUnicornEx.OnHornColorGridClicked;
                            unicorn.mHornColorsGrid.ItemClicked += CAPUnicornEx.OnHornColorGridClicked;
                        }
                    }

                    CASHair hair = CASHair.gSingleton;
                    if (hair != null)
                    {
                        if ((Responder.Instance != null) && (Responder.Instance.CASModel != null))
                        {

                            Responder.Instance.CASModel.UndoSelected -= hair.OnUndo;
                            Responder.Instance.CASModel.RedoSelected -= hair.OnRedo;

                            Responder.Instance.CASModel.UndoSelected += CASHairEx.OnUndo;
                            Responder.Instance.CASModel.RedoSelected += CASHairEx.OnRedo;
                        }

                        if (!object.ReferenceEquals(mCASHair, hair))
                        {
                            mCASHair = hair;

                            CASHairEx.InitialPopulate(hair);
                        }

                        if (hair.mContentTypeFilter != null)
                        {
                            hair.mContentTypeFilter.FiltersChanged -= hair.RefreshHairGrid;
                            hair.mContentTypeFilter.FiltersChanged -= CASHairEx.RefreshHairGrid;
                            hair.mContentTypeFilter.FiltersChanged += CASHairEx.RefreshHairGrid;
                        }

                        Button childByID = hair.GetChildByID(0x5dbb905, true) as Button;
                        if (childByID != null)
                        {
                            childByID.Click -= hair.OnHairButtonClick;
                            childByID.MouseDown -= CASHairEx.OnHairButtonMouseDown;
                            childByID.MouseDown += CASHairEx.OnHairButtonMouseDown;
                        }

                        childByID = hair.GetChildByID(0x5dbb906, true) as Button;
                        if (childByID != null)
                        {
                            childByID.Click -= hair.OnHatsButtonClick;
                            childByID.MouseDown -= CASHairEx.OnHatsButtonMouseDown;
                            childByID.MouseDown += CASHairEx.OnHatsButtonMouseDown;
                        }

                        if (hair.mHairTypesGrid != null)
                        {
                            hair.mHairTypesGrid.ItemClicked -= hair.OnTypesGridItemClicked;
                            hair.mHairTypesGrid.ItemClicked -= CASHairEx.OnTypesGridItemClicked;
                            hair.mHairTypesGrid.ItemClicked += CASHairEx.OnTypesGridItemClicked;
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(msg, e);
                }

                return true;
            }
        }

        public class PartTester
        {
            int mIteration;

            SimBuilder mBuilder;

            public bool mValidTop;

            public bool mFemaleBodyHair;

            bool mRobotCAS;            

            OutfitCategories mDisableFilterCategories;

            Dictionary<ProductVersion, bool> mHideByProduct;

            public PartTester(int iteration, OutfitCategories disableFilterCategories, SimBuilder builder)
            {
                mIteration = iteration;
                mBuilder = builder;
                mDisableFilterCategories = disableFilterCategories;

                mHideByProduct = new Dictionary<ProductVersion, bool>();

                foreach (ProductVersion version in MasterController.Settings.mHideByProduct)
                {
                    mHideByProduct[version] = true;
                }

                CASLogic logic = CASLogic.Instance;

                switch (logic.CASMode)
                {
                    case CASMode.CreateABot:
                    case CASMode.EditABot:
                        mRobotCAS = true;
                        break;
                }
            }

            public bool TestPart(CASParts.Wrapper newPart)
            {
                // Iteration 0 - Find Age/Gender related maternity outfits
                // Iteration 1 - Find Age/Gender non-category specific outfits
                // Iteration 2 - Find any outfit                

                if (mHideByProduct.ContainsKey(newPart.GetVersion())) return false;                

                if (((newPart.mPart.Age & CASAgeGenderFlags.YoungAdult) == CASAgeGenderFlags.YoungAdult) ||
                    ((newPart.mPart.Age & CASAgeGenderFlags.Adult) == CASAgeGenderFlags.Adult))
                {
                    newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.YoungAdult;
                    newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Adult;

                    if (CASParts.BodyHairTypes.Contains(newPart.mPart.BodyType))
                    {
                        if (MasterController.Settings.mBodyHairForTeens)
                        {
                            newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Teen;
                        }
                    }
                    else if (CASParts.sAccessories.Contains(newPart.mPart.BodyType))
                    {
                        if (MasterController.Settings.mAdultAccessoriesForTeens)
                        {
                            newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Teen;
                        }
                    }
                    else if (mIteration != 0)
                    {
                        newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Teen;
                    }
                    else if ((MasterController.Settings.mMaleAdultClothesForTeen) && ((newPart.mPart.Gender & CASAgeGenderFlags.Male) != CASAgeGenderFlags.None))
                    {
                        newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Teen;
                    }
                    else if ((MasterController.Settings.mFemaleAdultClothesForTeen) && ((newPart.mPart.Gender & CASAgeGenderFlags.Female) != CASAgeGenderFlags.None))
                    {
                        newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Teen;
                    }

                    if (MasterController.Settings.mAdultClothesForElders)
                    {
                        newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Elder;
                    }
                }

                if ((sPregnant > 0) && (mIteration != 2))
                {
                    if ((newPart.mPart.BodyType == BodyTypes.FullBody) ||
                        (newPart.mPart.BodyType == BodyTypes.LowerBody) ||
                        (newPart.mPart.BodyType == BodyTypes.UpperBody))
                    {
                        if ((newPart.mPart.CategoryFlags & (uint)OutfitCategoriesExtended.ValidForMaternity) != (uint)OutfitCategoriesExtended.ValidForMaternity)
                        {
                            // Not a maternity mesh
                            return false;
                        }
                        else if (mBuilder.Age != (newPart.mPart.AgeGenderSpecies & mBuilder.Age))
                        {
                            // Wrong Age                            
                            return false;
                        }
                        else if (mBuilder.Gender != (newPart.mPart.AgeGenderSpecies & mBuilder.Gender))
                        {
                            // Wrong Gender                            
                            return false;
                        }
                    }
                }

                if (MasterController.Settings.mDisableMakeupFilter)
                {
                    switch (newPart.mPart.BodyType)
                    {
                        case BodyTypes.Mascara:
                        case BodyTypes.Lipstick:
                        case BodyTypes.Blush:
                        case BodyTypes.CostumeMakeup:
                        case BodyTypes.EyeLiner:
                        case BodyTypes.EyeShadow:
                            newPart.mPart.CategoryFlags |= (uint)OutfitCategories.CategoryMask;
                            break;
                    }
                }

                if (MasterController.Settings.mUniGenderHair)
                {
                    switch (newPart.mPart.BodyType)
                    {
                        case BodyTypes.Hair:
                            newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Male | CASAgeGenderFlags.Female;
                            break;
                    }
                }

                if (MasterController.Settings.mUniGenderAccessories)
                {
                    if (CASParts.sAccessories.Contains(newPart.mPart.BodyType))
                    {
                        newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Male | CASAgeGenderFlags.Female;
                    }
                }

                if (MasterController.Settings.mUniGenderChildMale)
                {
                    if (((newPart.mPart.Age & CASAgeGenderFlags.Toddler) == CASAgeGenderFlags.Toddler) ||
                        ((newPart.mPart.Age & CASAgeGenderFlags.Child) == CASAgeGenderFlags.Child))
                    {
                        newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Male;
                    }
                }

                if (MasterController.Settings.mUniGenderChildFemale)
                {
                    if (((newPart.mPart.Age & CASAgeGenderFlags.Toddler) == CASAgeGenderFlags.Toddler) ||
                        ((newPart.mPart.Age & CASAgeGenderFlags.Child) == CASAgeGenderFlags.Child))
                    {
                        newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Female;
                    }
                }

                if (MasterController.Settings.mUniGenderAdult)
                {
                    if (((newPart.mPart.Age & CASAgeGenderFlags.YoungAdult) == CASAgeGenderFlags.YoungAdult) ||
                        ((newPart.mPart.Age & CASAgeGenderFlags.Adult) == CASAgeGenderFlags.Adult) ||
                        ((newPart.mPart.Age & CASAgeGenderFlags.Teen) == CASAgeGenderFlags.Teen))
                    {
                        newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Male;
                        newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Female;
                    }
                }

                if (MasterController.Settings.mBeardsForAll)
                {
                    switch (newPart.mPart.BodyType)
                    {
                        case BodyTypes.Beard:
                            newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Female;
                            break;
                    }
                }

                if (CASParts.BodyHairTypes.Contains(newPart.mPart.BodyType))
                {
                    if (MasterController.Settings.mBodyHairForFemales)
                    {
                        newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Female;

                        mFemaleBodyHair = true;
                    }
                    else if ((newPart.mPart.Gender & CASAgeGenderFlags.Female) == CASAgeGenderFlags.Female)
                    {
                        mFemaleBodyHair = true;
                    }
                }

                switch (newPart.mPart.BodyType)
                {
                    case BodyTypes.TattooTemplate:
                    case BodyTypes.Tattoo:
                        newPart.mPart.AgeGenderSpecies |= CASAgeGenderFlags.Toddler | CASAgeGenderFlags.Child | CASAgeGenderFlags.Teen;
                        break;
                }

                if ((mIteration != 0) || (mDisableFilterCategories != OutfitCategories.None) || (IsSpecialEdit()))
                {                    
                    newPart.mPart.CategoryFlags |= (uint)mDisableFilterCategories;
                }

                if (!MasterController.Settings.mShowHiddenParts)
                {
                    CASLogic logic = CASLogic.Instance;

                    bool flag = false;
                    if (!logic.HiddenInCAS)
                    {                        
                        flag = true;
                    }
                    else if ((newPart.mPart.CategoryFlags & 0x1000000) == 0x0)
                    {                        
                        flag = true;
                    }
                    else if (logic.ActiveWardrobeContains(newPart.mPart))
                    {                        
                        flag = true;
                    }
                    else if ((logic.mCASMode == CASMode.Stylist) && ((newPart.mPart.CategoryFlags & 0x400) != 0x0))
                    {                        
                        flag = true;
                    }                    

                    if (!flag) return false;
                }

                if (newPart.BodyType == BodyTypes.UpperBody)
                {
                    mValidTop = true;
                }                

                switch (newPart.BodyType)
                {
                    case BodyTypes.Mascara:
                    case BodyTypes.Lipstick:
                    case BodyTypes.Blush:
                    case BodyTypes.CostumeMakeup:
                    case BodyTypes.EyeLiner:
                    case BodyTypes.EyeShadow:
                    case BodyTypes.Tattoo:
                    case BodyTypes.TattooTemplate:
                        return !mRobotCAS;
                    default:
                        return (mRobotCAS == OutfitUtils.IsBotPart(newPart.mPart));
                }
            }
        }
    }
}
