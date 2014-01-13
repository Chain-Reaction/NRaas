using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.Metadata;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Proxies
{
    public abstract class CASModelProxy : ICASModel
    {
        protected readonly ICASModel mCASModel;

        public CASModelProxy(ICASModel casModel)
        {
            mCASModel = casModel;

            mCASModel.OnFacialBlendApplied += OnFacialBlendAppliedProxy;
            mCASModel.OnOutfitSavedDeleted += OnOutfitSavedDeletedProxy;
            mCASModel.OnSimOutfitRemoved += OnSimOutfitRemovedProxy;
            mCASModel.OnSimPreviewLoadFinished += OnSimPreviewLoadFinishedProxy;
            mCASModel.OnSimSpeciesChanged += OnSimSpeciesChangedProxy;

            mCASModel.ClosingDown += OnClosingDownProxy;
            mCASModel.FacialBlendComponentsUpdate += OnFacialBlendComponentsUpdateProxy;
            mCASModel.FavoritesUpdated += OnFavoritesUpdatedProxy;
            mCASModel.OnCASContentAdded += OnCASContentAddedProxy;
            mCASModel.OnCASError += OnCASErrorProxy;
            mCASModel.OnCASPartAdded += OnCASPartAddedProxy;
            mCASModel.OnCASPartRemoved += OnCASPartRemovedProxy;
            mCASModel.OnDynamicUpdateCurrentSimThumbnail += OnDynamicUpdateCurrentSimThumbnailProxy;
            mCASModel.OnHairPresetApplied += OnHairPresetAppliedProxy;
            mCASModel.OnHouseholdSaved += OnHouseholdSavedProxy;
            mCASModel.OnPresetAppliedToPart += OnPresetAppliedToPartProxy;
            mCASModel.OnSetHairColor += OnSetHairColorProxy;
            mCASModel.OnSimAddedToHousehold += OnSimAddedToHouseholdProxy;
            mCASModel.OnSimAgeGenderChanged += OnSimAgeGenderChangedProxy;
            mCASModel.OnSimDeleted += OnSimDeletedProxy;
            mCASModel.OnSimFaceRandomized += OnSimFaceRandomizedProxy;
            mCASModel.OnSimLoaded += OnSimLoadedProxy;
            mCASModel.OnSimOutfitCategoryChanged += OnSimOutfitCategoryChangedProxy;
            mCASModel.OnSimOutfitIndexChanged += OnSimOutfitIndexChangedProxy;
            mCASModel.OnSimPreviewChange += OnSimPreviewChangeProxy;
            mCASModel.OnSimRandomized += OnSimRandomizedProxy;
            mCASModel.OnSimReplaced += OnSimReplacedProxy;
            mCASModel.OnSimSaved += OnSimSavedProxy;
            mCASModel.OnSimUpdated += OnSimUpdatedProxy;
            mCASModel.OnUndoRedoStackChanged += OnUndoRedoStackChangedProxy;
            mCASModel.OnUpdateThumbnails += OnUpdateThumbnailsProxy;
            mCASModel.RedoSelected += OnRedoSelectedProxy;
            mCASModel.ShowUI += OnShowUIProxy;
            mCASModel.SignUpdated += OnSignUpdatedProxy;
            mCASModel.TraitsUpdated += OnTraitsUpdatedProxy;
            mCASModel.TraitsValidatedOnAgeChange += OnTraitsValidatedOnAgeChangeProxy;
            mCASModel.UndoSelected += OnUndoSelectedProxy;
            mCASModel.VoiceUpdated += OnVoiceUpdatedProxy;
            mCASModel.OccultTypeSelected += OnOccultTypeSelectedProxy;
        }

        // Events
        public event FacialBlendApplied OnFacialBlendApplied;
        public event OutfitSavedDelegate OnOutfitSavedDeleted;
        public event SimOutfitRemoved OnSimOutfitRemoved;
        public event SimPreviewLoadFinished OnSimPreviewLoadFinished;
        public event SimSpeciesChanged OnSimSpeciesChanged;

        public event ClosingDownDelegate ClosingDown;
        public event FacialBlendComponentsUpdateDelegate FacialBlendComponentsUpdate;
        public event FavoritesUpdatedDelegate FavoritesUpdated;
        public event NewCASContentInstalled OnCASContentAdded;
        public event CASErrorDelegate OnCASError;
        public event CASPartAdded OnCASPartAdded;
        public event CASPartRemoved OnCASPartRemoved;
        public event DynamicUpdateCurrentSimThumbnailDelegate OnDynamicUpdateCurrentSimThumbnail;
        public event HairPresetAppliedDelegate OnHairPresetApplied;
        public event HouseholdSavedDelegate OnHouseholdSaved;
        public event PresetAppliedToPart OnPresetAppliedToPart;
        public event SetHairColorDelegate OnSetHairColor;
        public event SimAddedToHouseholdDelegate OnSimAddedToHousehold;
        public event SimAgeGenderChanged OnSimAgeGenderChanged;
        public event SimDeletedDelegate OnSimDeleted;
        public event SimRandomizedDelegate OnSimFaceRandomized;
        public event SimLoadedDelegate OnSimLoaded;
        public event SimOutfitCategoryChanged OnSimOutfitCategoryChanged;
        public event SimOutfitIndexChanged OnSimOutfitIndexChanged;
        public event SimPreviewChangeDelegate OnSimPreviewChange;
        public event SimRandomizedDelegate OnSimRandomized;
        public event SimReplacedDelegate OnSimReplaced;
        public event Sims3.UI.CAS.SimSavedDelegate OnSimSaved;
        public event SimUpdatedDelegate OnSimUpdated;
        public event UndoRedoStackChangedDelegate OnUndoRedoStackChanged;
        public event UpdateThumbnailsDelegate OnUpdateThumbnails;
        public event UndoRedoSelected RedoSelected;
        public event ShowUIDelegate ShowUI;
        public event SignUpdatedDelegate SignUpdated;
        public event TraitsUpdatedDelegate TraitsUpdated;
        public event ValidateTraitsOnAgeChangeDelegate TraitsValidatedOnAgeChange;
        public event UndoRedoSelected UndoSelected;
        public event VoiceUpdatedDelegate VoiceUpdated;
        public event OccultTypeChosenDelegate OccultTypeSelected;

        public void OnFacialBlendAppliedProxy()
        {
            try
            {
                if (OnFacialBlendApplied != null)
                {
                    OnFacialBlendApplied();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnFacialBlendAppliedProxy", e);
            }
        }

        public void OnOutfitSavedDeletedProxy()
        {
            try
            {
                if (OnOutfitSavedDeleted != null)
                {
                    OnOutfitSavedDeleted();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnOutfitSavedDeletedProxy", e);
            }
        }

        public void OnSimOutfitRemovedProxy()
        {
            try
            {
                if (OnSimOutfitRemoved != null)
                {
                    OnSimOutfitRemoved();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimOutfitRemovedProxy", e);
            }
        }

        public void OnSimPreviewLoadFinishedProxy()
        {
            try
            {
                if (OnSimPreviewLoadFinished != null)
                {
                    OnSimPreviewLoadFinished();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimPreviewLoadFinishedProxy", e);
            }
        }

        public void OnSimSpeciesChangedProxy(CASAgeGenderFlags species)
        {
            try
            {
                if (OnSimSpeciesChanged != null)
                {
                    OnSimSpeciesChanged(species);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimSpeciesChangedProxy", e);
            }
        }

        public void OnClosingDownProxy()
        {
            try
            {
                if (ClosingDown != null)
                {
                    ClosingDown();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnClosingDownProxy", e);
            }
        }

        public void OnFacialBlendComponentsUpdateProxy()
        {
            try
            {
                if (FacialBlendComponentsUpdate != null)
                {
                    FacialBlendComponentsUpdate();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnFacialBlendComponentsUpdateProxy", e);
            }
        }

        public void OnFavoritesUpdatedProxy(FavoriteMusicType music, FavoriteFoodType food, Color color)
        {
            try
            {
                if (FavoritesUpdated != null)
                {
                    FavoritesUpdated(music, food, color);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnFavoritesUpdatedProxy", e);
            }
        }

        public void OnCASContentAddedProxy()
        {
            try
            {
                if (OnCASContentAdded != null)
                {
                    OnCASContentAdded();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnCASContentAddedProxy", e);
            }
        }

        public void OnCASErrorProxy(CASErrorCode errorCode)
        {
            try
            {
                if (OnCASError != null)
                {
                    OnCASError(errorCode);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnCASErrorProxy", e);
            }
        }

        public void OnCASPartAddedProxy(CASPart part)
        {
            try
            {
                if (OnCASPartAdded != null)
                {
                    OnCASPartAdded(part);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnCASPartAddedProxy", e);
            }
        }

        public void OnCASPartRemovedProxy(CASPart part)
        {
            try
            {
                if (OnCASPartRemoved != null)
                {
                    OnCASPartRemoved(part);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnCASPartRemovedProxy", e);
            }
        }

        public void OnDynamicUpdateCurrentSimThumbnailProxy()
        {
            try
            {
                if (OnDynamicUpdateCurrentSimThumbnail != null)
                {
                    OnDynamicUpdateCurrentSimThumbnail();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnDynamicUpdateCurrentSimThumbnailProxy", e);
            }
        }

        public void OnHairPresetAppliedProxy(BodyTypes bodyType, ColorInfo hairColors)
        {
            try
            {
                if (OnHairPresetApplied != null)
                {
                    OnHairPresetApplied(bodyType, hairColors);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnHairPresetAppliedProxy", e);
            }
        }

        public void OnHouseholdSavedProxy(string householdName)
        {
            try
            {
                if (OnHouseholdSaved != null)
                {
                    OnHouseholdSaved(householdName);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnHouseholdSavedProxy", e);
            }
        }

        public void OnPresetAppliedToPartProxy(CASPart part, string preset)
        {
            try
            {
                if (OnPresetAppliedToPart != null)
                {
                    OnPresetAppliedToPart(part, preset);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnPresetAppliedToPartProxy", e);
            }
        }

        public void OnSetHairColorProxy(BodyTypes bodyType)
        {
            try
            {
                if (OnSetHairColor != null)
                {
                    OnSetHairColor(bodyType);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSetHairColorProxy", e);
            }
        }

        public void OnSimAddedToHouseholdProxy(int simIndex)
        {
            try
            {
                if (OnSimAddedToHousehold != null)
                {
                    OnSimAddedToHousehold(simIndex);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimAddedToHouseholdProxy", e);
            }
        }

        public void OnSimAgeGenderChangedProxy(CASAgeGenderFlags age, CASAgeGenderFlags gender)
        {
            try
            {
                if (OnSimAgeGenderChanged != null)
                {
                    OnSimAgeGenderChanged(age, gender);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimAgeGenderChangedProxy", e);
            }
        }

        public void OnSimDeletedProxy(int newSimIndex)
        {
            try
            {
                if (OnSimDeleted != null)
                {
                    OnSimDeleted(newSimIndex);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimDeletedProxy", e);
            }
        }

        public void OnSimFaceRandomizedProxy()
        {
            try
            {
                if (OnSimFaceRandomized != null)
                {
                    OnSimFaceRandomized();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimFaceRandomizedProxy", e);
            }
        }

        public void OnSimLoadedProxy(ResourceKey key)
        {
            try
            {
                if (OnSimLoaded != null)
                {
                    OnSimLoaded(key);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimLoadedProxy", e);
            }
        }

        public void OnSimOutfitCategoryChangedProxy(OutfitCategories outfitCategory)
        {
            try
            {
                if (OnSimOutfitCategoryChanged != null)
                {
                    OnSimOutfitCategoryChanged(outfitCategory);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimOutfitCategoryChangedProxy", e);
            }
        }

        public void OnSimOutfitIndexChangedProxy(int index)
        {
            try
            {
                if (OnSimOutfitIndexChanged != null)
                {
                    OnSimOutfitIndexChanged(index);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimOutfitIndexChangedProxy", e);
            }
        }

        public void OnSimPreviewChangeProxy(int simIndex)
        {
            try
            {
                if (OnSimPreviewChange != null)
                {
                    OnSimPreviewChange(simIndex);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimPreviewChangeProxy", e);
            }
        }

        public void OnSimRandomizedProxy()
        {
            try
            {
                if (OnSimRandomized != null)
                {
                    OnSimRandomized();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimRandomizedProxy", e);
            }
        }

        public void OnSimReplacedProxy(int simIndex)
        {
            try
            {
                if (OnSimReplaced != null)
                {
                    OnSimReplaced(simIndex);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimReplacedProxy", e);
            }
        }

        public void OnSimSavedProxy(ResourceKey key)
        {
            try
            {
                if (OnSimSaved != null)
                {
                    OnSimSaved(key);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimSavedProxy", e);
            }
        }

        public void OnSimUpdatedProxy(int simIndex)
        {
            try
            {
                if (OnSimUpdated != null)
                {
                    OnSimUpdated(simIndex);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSimUpdatedProxy", e);
            }
        }

        public void OnUndoRedoStackChangedProxy(int index, int numOperations)
        {
            try
            {
                if (OnUndoRedoStackChanged != null)
                {
                    OnUndoRedoStackChanged(index, numOperations);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnUndoRedoStackChangedProxy", e);
            }
        }

        public void OnUpdateThumbnailsProxy()
        {
            try
            {
                if (OnUpdateThumbnails != null)
                {
                    OnUpdateThumbnails();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnUpdateThumbnailsProxy", e);
            }
        }

        public void OnRedoSelectedProxy()
        {
            try
            {
                if (RedoSelected != null)
                {
                    RedoSelected();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnRedoSelectedProxy", e);
            }
        }

        public void OnShowUIProxy(bool toShow)
        {
            try
            {
                if (ShowUI != null)
                {
                    ShowUI(toShow);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnShowUIProxy", e);
            }
        }

        public void OnSignUpdatedProxy(Zodiac zodiac)
        {
            try
            {
                if (SignUpdated != null)
                {
                    SignUpdated(zodiac);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnSignUpdatedProxy", e);
            }
        }

        public void OnTraitsUpdatedProxy()
        {
            try
            {
                if (TraitsUpdated != null)
                {
                    TraitsUpdated();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnTraitsUpdatedProxy", e);
            }
        }

        public void OnTraitsValidatedOnAgeChangeProxy(List<ITraitEntryInfo> traitsRemoved, int traitsOverLimit, int numInvalidTraits)
        {
            try
            {
                if (TraitsValidatedOnAgeChange != null)
                {
                    TraitsValidatedOnAgeChange(traitsRemoved, traitsOverLimit, numInvalidTraits);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnTraitsValidatedOnAgeChangeProxy", e);
            }
        }

        public void OnUndoSelectedProxy()
        {
            try
            {
                if (UndoSelected != null)
                {
                    UndoSelected();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnUndoSelectedProxy", e);
            }
        }

        public void OnVoiceUpdatedProxy()
        {
            try
            {
                if (VoiceUpdated != null)
                {
                    VoiceUpdated();
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnVoiceUpdatedProxy", e);
            }
        }

        public void OnOccultTypeSelectedProxy(OccultTypes currentType)
        {
            try
            {
                if (OccultTypeSelected != null)
                {
                    OccultTypeSelected(currentType);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnOccultTypeSelectedProxy", e);
            }
        }
       
        // Methods
        public bool CurrentSimHasJumpingOutfit()
        {
            return mCASModel.CurrentSimHasJumpingOutfit();
        }

        public bool CurrentSimHasRacingOutfit()
        {
            return mCASModel.CurrentSimHasRacingOutfit();
        }

        public void DumpCASData()
        {
            mCASModel.DumpCASData();
        }

        public float GetCurlSize()
        {
            return mCASModel.GetCurlSize();
        }

        public SimOutfit GetCurrentCachedOutfit(string instanceName)
        {
            return mCASModel.GetCurrentCachedOutfit(instanceName);
        }

        public List<FacialBlend> GetFurBlends()
        {
            return mCASModel.GetFurBlends();
        }

        public ResourceKey GetFurMap()
        {
            return mCASModel.GetFurMap();
        }

        public float GetNumCurls()
        {
            return mCASModel.GetNumCurls();
        }

        public List<CASController.BreedOutfit> GetPetBreeds(CASAgeGenderFlags species)
        {
            return mCASModel.GetPetBreeds(species);
        }

        public ArrayList GetUnicornCASParts(BodyTypes bodyType, bool isChild)
        {
            return mCASModel.GetUnicornCASParts(bodyType, isChild);
        }

        public bool IsUnicorn()
        {
            return mCASModel.IsUnicorn();
        }

        public void RandomizeName()
        {
            mCASModel.RandomizeName();
        }

        public void RequestAddCASPartAndApplyBlend(CASPart part, BaseBlend blend, float blendAmount)
        {
            mCASModel.RequestAddCASPartAndApplyBlend(part, blend, blendAmount);
        }

        public void RequestAddPeltLayer(CASPart part, string preset)
        {
            mCASModel.RequestAddPeltLayer(part, preset);
        }

        public void RequestAddSelectOutfit(OutfitCategories category)
        {
            mCASModel.RequestAddSelectOutfit(category);
        }

        public void RequestChangePeltLayerPart(CASPart newPart, CASPart oldPart, string preset)
        {
            mCASModel.RequestChangePeltLayerPart(newPart, oldPart, preset);
        }

        public void RequestClearUndoBlock()
        {
            mCASModel.RequestClearUndoBlock();
        }

        public void RequestCreateNewPet(CASAgeGenderFlags ageGenderSpecies)
        {
            mCASModel.RequestCreateNewPet(ageGenderSpecies);
        }

        public void RequestDeleteUserUniform(ResourceKey key)
        {
            mCASModel.RequestDeleteUserUniform(key);
        }

        public void RequestFinalizePeltHueShift()
        {
            mCASModel.RequestFinalizePeltHueShift();
        }

        public void RequestForceSetSkinTonePreset(SkinTonePreset preset)
        {
            mCASModel.RequestForceSetSkinTonePreset(preset);
        }

        public void RequestMovePeltLayer(CASPart part, CASPart newLocationPart)
        {
            mCASModel.RequestMovePeltLayer(part, newLocationPart);
        }

        public void RequestPeltHueShift(Vector3 targetHSV, CASPart basePart, List<CASPart> partsToShift, bool finalize)
        {
            mCASModel.RequestPeltHueShift(targetHSV, basePart, partsToShift, finalize);
        }

        public void RequestRandomizeName()
        {
            mCASModel.RequestRandomizeName();
        }

        public void RequestRandomizePet()
        {
            mCASModel.RequestRandomizePet();
        }

        public void RequestRandomizePetCoat()
        {
            mCASModel.RequestRandomizePetCoat();
        }

        public void RequestRandomizePetShape()
        {
            mCASModel.RequestRandomizePetShape();
        }

        public void RequestSaveUserUniform(ulong components)
        {
            mCASModel.RequestSaveUserUniform(components);
        }

        public void RequestSetBlendAmounts(Dictionary<FacialBlend, float> blends, bool finalize)
        {
            mCASModel.RequestSetBlendAmounts(blends, finalize);
        }

        public void RequestSetBreed(SimOutfit newOutfit)
        {
            mCASModel.RequestSetBreed(newOutfit);
        }

        public void RequestSetCoat(SimOutfit newOutfit, bool colorationOnly)
        {
            mCASModel.RequestSetCoat(newOutfit, colorationOnly);
        }

        public void RequestSetCurlSize(float size, bool finalize)
        {
            mCASModel.RequestSetCurlSize(size, finalize);
        }

        public void RequestSetFurMap(ResourceKey furMap, bool finalize)
        {
            mCASModel.RequestSetFurMap(furMap, finalize);
        }

        public void RequestSetNumCurls(float numCurls, bool finalize)
        {
            mCASModel.RequestSetNumCurls(numCurls, finalize);
        }

        public void RequestSetPeltLayersOpacity(CASPart part, List<CASPart> mLinkedParts, float newOpacity, bool finalize)
        {
            mCASModel.RequestSetPeltLayersOpacity(part, mLinkedParts, newOpacity, finalize);
        }

        public void RequestSetPeltLayersRotation(CASPart part, List<CASPart> mLinkedParts, Vector2 centerPoint, float newRotation, bool finalize)
        {
            mCASModel.RequestSetPeltLayersRotation(part, mLinkedParts, centerPoint, newRotation, finalize);
        }

        public void RequestSetPeltLayersTranslationScale(CASPart part, List<CASPart> mLinkedParts, float scaleScaleX, float scaleScaleY, Vector2 newCenterPoint, Vector2 oldCenterPoint, bool finalize)
        {
            mCASModel.RequestSetPeltLayersTranslationScale(part, mLinkedParts, scaleScaleX, scaleScaleY, newCenterPoint, oldCenterPoint, finalize);
        }

        public void RequestSetUndoBlock()
        {
            mCASModel.RequestSetUndoBlock();
        }

        public void RequestSpeciesAndBreed(CASAgeGenderFlags newSpecies, SimOutfit newOutfit)
        {
            mCASModel.RequestSpeciesAndBreed(newSpecies, newOutfit);
        }

        public void RequestStateOutfit(OutfitCategories category, int index, SimOutfit newOutfit)
        {
            mCASModel.RequestStateOutfit(category, index, newOutfit);
        }

        public bool ShareOutfit(string packageName, string description, ulong uniformComponents)
        {
            return mCASModel.ShareOutfit(packageName, description, uniformComponents);
        }

        public void TriggerAdvancedModeAnimation()
        {
            mCASModel.TriggerAdvancedModeAnimation();
        }

        public void UpdateSimOffset(bool bSitPosture)
        {
            mCASModel.UpdateSimOffset(bSitPosture);
        }

        public bool ActiveWardrobeContains(CASPart part)
        {
            return mCASModel.ActiveWardrobeContains(part);
        }

        public void AddOutfit(OutfitCategories category)
        {
            mCASModel.AddOutfit(category);
        }

        public void AddTrait(ulong traitGuid)
        {
            mCASModel.AddTrait(traitGuid);
        }

        public void AttachWindowToSimRelativePosition(WindowBase win, float horizontalOffset, float height)
        {
            mCASModel.AttachWindowToSimRelativePosition(win, horizontalOffset, height);
        }

        public ObjectGuid CreateGeneticsOffspringPreviewSim(ISimDescription simDesc)
        {
            return mCASModel.CreateGeneticsOffspringPreviewSim(simDesc);
        }

        public ObjectGuid CreateGeneticsPreviewSim(ISimDescription simDesc)
        {
            return mCASModel.CreateGeneticsPreviewSim(simDesc);
        }

        public void CreateHousehold()
        {
            mCASModel.CreateHousehold();
        }

        public bool DeleteSimFromContent(ResourceKey key)
        {
            return mCASModel.DeleteSimFromContent(key);
        }

        public void DetachWindowFromSim(WindowBase win)
        {
            mCASModel.DetachWindowFromSim(win);
        }

        public void Done()
        {
            mCASModel.Done();
        }

        public void DumpHousehold()
        {
            mCASModel.DumpHousehold();
        }

        public void EditCASPart()
        {
            mCASModel.EditCASPart();
        }

        public bool EditingExistingSim()
        {
            return mCASModel.EditingExistingSim();
        }

        public bool ExportSim(string name, string description)
        {
            return mCASModel.ExportSim(name, description);
        }

        public void ExtractOutfitHairColor()
        {
            mCASModel.ExtractOutfitHairColor();
        }

        public int FindCurrentHairColorPresetIndex(BodyTypes bodyType, bool filterUserOnly)
        {
            return mCASModel.FindCurrentHairColorPresetIndex(bodyType, filterUserOnly);
        }

        public void FixSims(OutfitSources outfitSources, SimFixes simFixes)
        {
            mCASModel.FixSims(outfitSources, simFixes);
        }

        public List<IInitialMajorWish> GetAllLifetimeWishes()
        {
            return mCASModel.GetAllLifetimeWishes();
        }

        public List<PersonaInfo> GetAvailablePersonas(CASAgeGenderFlags age)
        {
            return mCASModel.GetAvailablePersonas(age);
        }

        public float GetBlendAmount(BaseBlend blend)
        {
            return mCASModel.GetBlendAmount(blend);
        }

        public void GetBodyTypeAndRegionFromPickData(int casId, ref int bodyType, ref int partRegion)
        {
            mCASModel.GetBodyTypeAndRegionFromPickData(casId, ref bodyType, ref partRegion);
        }

        public SimOutfit GetCurrentOutfit()
        {
            return mCASModel.GetCurrentOutfit();
        }

        public List<ITraitEntryInfo> GetCurrentTraits()
        {
            return mCASModel.GetCurrentTraits();
        }

        public string GetDesignPreset(CASPart part)
        {
            return mCASModel.GetDesignPreset(part);
        }

        public KeyValuePair<string, Dictionary<string, Complate>> GetDesignPresetEntry(CASPart part)
        {
            return mCASModel.GetDesignPresetEntry(part);
        }

        public List<ITraitEntryInfo> GetDictionaryTraits()
        {
            return mCASModel.GetDictionaryTraits();
        }

        public Color GetHairColor(BodyTypes bodyType, int index)
        {
            return mCASModel.GetHairColor(bodyType, index);
        }

        public ThumbnailKey GetHouseholdThumbnailKey()
        {
            return mCASModel.GetHouseholdThumbnailKey();
        }

        public IInitialMajorWish GetLifetimeWish(uint nodeId)
        {
            return mCASModel.GetLifetimeWish(nodeId);
        }

        public List<IInitialMajorWish> GetLifetimeWishes()
        {
            return mCASModel.GetLifetimeWishes();
        }

        public string GetLocalizedFullName(string firstName, string lastName)
        {
            return mCASModel.GetLocalizedFullName(firstName, lastName);
        }

        public List<ResourceKey> GetMakeupColorPresets()
        {
            return mCASModel.GetMakeupColorPresets();
        }

        public int GetMaxTraitsFor(CASAgeGenderFlags age)
        {
            return mCASModel.GetMaxTraitsFor(age);
        }

        public float GetMorphFat()
        {
            return mCASModel.GetMorphFat();
        }

        public float GetMorphFit()
        {
            return mCASModel.GetMorphFit();
        }

        public float GetMorphThin()
        {
            return mCASModel.GetMorphThin();
        }

        public ArrayList GetOutfits(OutfitCategories category)
        {
            return mCASModel.GetOutfits(category);
        }

        public uint GetPickDataFromBodyTypeAndRegion(int bodyType, int partRegion)
        {
            return mCASModel.GetPickDataFromBodyTypeAndRegion(bodyType, partRegion);
        }

        public ArrayList GetPresetBlendsForRegion(FacialBlend.PresetFacialRegions region, CASAgeGenderFlags species)
        {
            return mCASModel.GetPresetBlendsForRegion(region, species);
        }

        public IRecipe GetRecipe(FavoriteFoodType food)
        {
            return mCASModel.GetRecipe(food);
        }

        public ISimDescription GetSimDescription(ResourceKey key, ref ResourceKeyContentCategory category)
        {
            return mCASModel.GetSimDescription(key, ref category);
        }

        public SimOutfit GetSimOutfitFromSimDescriptionKey(ResourceKey key, OutfitCategories outfitCategory)
        {
            return mCASModel.GetSimOutfitFromSimDescriptionKey(key, outfitCategory);
        }

        public SimOutfit GetSimOutfitFromTitle(string title)
        {
            return mCASModel.GetSimOutfitFromTitle(title);
        }

        public List<ISimDescription> GetSimsInHousehold()
        {
            return mCASModel.GetSimsInHousehold();
        }

        public float GetSkinTone()
        {
            return mCASModel.GetSkinTone();
        }

        public ResourceKey GetSkinToneKey()
        {
            return mCASModel.GetSkinToneKey();
        }

        public ArrayList GetSkinTonePresets()
        {
            return mCASModel.GetSkinTonePresets();
        }

        public ResourceKey GetSkinToneRamp()
        {
            return mCASModel.GetSkinToneRamp();
        }

        public List<ResourceKey> GetSkinToneRamps()
        {
            return mCASModel.GetSkinToneRamps();
        }

        public List<ITraitEntryInfo> GetTraitsOfSets(ITraitEntryInfo[] sets)
        {
            return mCASModel.GetTraitsOfSets(sets);
        }

        public virtual ArrayList GetVisibleCASParts(BodyTypes bodyType)
        {
            return mCASModel.GetVisibleCASParts(bodyType);
        }

        public virtual ArrayList GetVisibleCASParts(BodyTypes bodyType, uint categories)
        {
            return mCASModel.GetVisibleCASParts(bodyType, categories);
        }

        public List<CASPart> GetWornParts(BodyTypes bodyType)
        {
            return mCASModel.GetWornParts(bodyType);
        }

        public bool HasValidName(ISimDescription desc)
        {
            return mCASModel.HasValidName(desc);
        }

        public void InitializeEyeColor()
        {
            mCASModel.InitializeEyeColor();
        }

        public bool IsMusicTypeInstalled(FavoriteMusicType music)
        {
            return mCASModel.IsMusicTypeInstalled(music);
        }

        public bool IsProcessing()
        {
            return mCASModel.IsProcessing();
        }

        public bool IsValidHouseholdName()
        {
            return mCASModel.IsValidHouseholdName();
        }

        public bool IsVegetarian()
        {
            return mCASModel.IsVegetarian();
        }

        public void LinkMaterials(BodyTypes bodyType, int flag)
        {
            mCASModel.LinkMaterials(bodyType, flag);
        }

        public void LTRForceChangeState(ISimDescription simDesc1, ISimDescription simDesc2, Sims3.UI.Controller.LongTermRelationshipTypes relation)
        {
            mCASModel.LTRForceChangeState(simDesc1, simDesc2, relation);
        }

        public void LTRSetDefaultLiking(ISimDescription simDesc1, ISimDescription simDesc2)
        {
            mCASModel.LTRSetDefaultLiking(simDesc1, simDesc2);
        }

        public ResourceKey MakeCompoundBlendKey(string name)
        {
            return mCASModel.MakeCompoundBlendKey(name);
        }

        public ISimDescription MakeDescendant(ISimDescription dad, ISimDescription mom, CASAgeGenderFlags age, CASAgeGenderFlags gender, float averageMood, Random pregoRandom, bool interactive, bool updateGenealogy, bool setName)
        {
            return mCASModel.MakeDescendant(dad, mom, age, gender, averageMood, pregoRandom, interactive, updateGenealogy, setName);
        }

        public ResourceKey MakeFacialBlendKey(string name)
        {
            return mCASModel.MakeFacialBlendKey(name);
        }

        public bool OrientSimToward(float newAngle)
        {
            return mCASModel.OrientSimToward(newAngle);
        }

        public ArrayList PartDataGetPresets(CASPart part)
        {
            return mCASModel.PartDataGetPresets(part);
        }

        public void PlayTraitAnimation(ulong traitGuid)
        {
            mCASModel.PlayTraitAnimation(traitGuid);
        }

        public void Quit()
        {
            mCASModel.Quit();
        }

        public void RandomizeTraits()
        {
            mCASModel.RandomizeTraits();
        }

        public void RegisterPatternKeys(CASPart part, Complate topComplate)
        {
            mCASModel.RegisterPatternKeys(part, topComplate);
        }

        public void RemoveOutfit(OutfitCategories category, int index, int newOutfitIndex)
        {
            mCASModel.RemoveOutfit(category, index, newOutfitIndex);
        }

        public void RemoveTrait(ulong traitGuid)
        {
            mCASModel.RemoveTrait(traitGuid);
        }

        public void ReportBrokenOutfits(OutfitSources outfitSources)
        {
            mCASModel.ReportBrokenOutfits(outfitSources);
        }

        public virtual void RequestAddCASPart(CASPart part, bool randomizeDesign)
        {
            mCASModel.RequestAddCASPart(part, randomizeDesign);
        }

        public virtual void RequestAddCASPart(CASPart part, string preset)
        {
            mCASModel.RequestAddCASPart(part, preset);
        }

        public void RequestAddPartAndCommitPreset(CASPart part, KeyValuePair<string, Dictionary<string, Complate>> presetEntry)
        {
            mCASModel.RequestAddPartAndCommitPreset(part, presetEntry);
        }

        public void RequestAddSimToHousehold(bool clearNameAndTraits)
        {
            mCASModel.RequestAddSimToHousehold(clearNameAndTraits);
        }

        public void RequestBeardUsesHairColor(bool value)
        {
            mCASModel.RequestBeardUsesHairColor(value);
        }

        public void RequestBodyHairUseHairColor(bool value)
        {
            mCASModel.RequestBodyHairUseHairColor(value);
        }

        public void RequestClearChangeReport()
        {
            mCASModel.RequestClearChangeReport();
        }

        public void RequestClearRedoOperations()
        {
            mCASModel.RequestClearRedoOperations();
        }

        public void RequestClearStack()
        {
            mCASModel.RequestClearStack();
        }

        public void RequestCloneSim(int index)
        {
            mCASModel.RequestCloneSim(index);
        }

        public void RequestCommitPresetToPart(CASPart part, KeyValuePair<string, Dictionary<string, Complate>> presetEntry)
        {
            mCASModel.RequestCommitPresetToPart(part, presetEntry);
        }

        public void RequestCommitPresetToPart(CASPart part, string preset)
        {
            mCASModel.RequestCommitPresetToPart(part, preset);
        }

        public void RequestCommitPresetToPart(CASPart part, KeyValuePair<string, Dictionary<string, Complate>> presetEntry, bool finalize)
        {
            mCASModel.RequestCommitPresetToPart(part, presetEntry, finalize);
        }

        public void RequestCommitPresetToPart(CASPart part, string preset, bool finalize)
        {
            mCASModel.RequestCommitPresetToPart(part, preset, finalize);
        }

        public void RequestCreateNewSim()
        {
            mCASModel.RequestCreateNewSim();
        }

        public void RequestDeleteSimFromHousehold(int index)
        {
            mCASModel.RequestDeleteSimFromHousehold(index);
        }

        public void RequestEyebrowsUseHairColor(bool value)
        {
            mCASModel.RequestEyebrowsUseHairColor(value);
        }

        public void RequestFinalizeBlendChanges(BaseBlend blend1, BaseBlend blend2)
        {
            mCASModel.RequestFinalizeBlendChanges(blend1, blend2);
        }

        public void RequestFinalizeCommitPresetToPart()
        {
            mCASModel.RequestFinalizeCommitPresetToPart();
        }

        public void RequestLerpCamera(string basePose, string closeUpPose, float zoom, float zoomDelta, CASCameraLerp lerp, bool bUseSitPosture)
        {
            mCASModel.RequestLerpCamera(basePose, closeUpPose, zoom, zoomDelta, lerp, bUseSitPosture);
        }

        public void RequestLoadSim(ResourceKey key)
        {
            mCASModel.RequestLoadSim(key);
        }

        public void RequestMorphFat(float value, bool forceNewOp)
        {
            mCASModel.RequestMorphFat(value, forceNewOp);
        }

        public void RequestMorphFit(float value, bool forceNewOp)
        {
            mCASModel.RequestMorphFit(value, forceNewOp);
        }

        public void RequestMorphThin(float value, bool forceNewOp)
        {
            mCASModel.RequestMorphThin(value, forceNewOp);
        }

        public void RequestMorphThinFat(float thin, float fat, bool forceNewOp)
        {
            mCASModel.RequestMorphThinFat(thin, fat, forceNewOp);
        }

        public void RequestRandomDesign(CASPart part)
        {
            mCASModel.RequestRandomDesign(part);
        }

        public void RequestRandomFavorites(FavoriteFoodType food, FavoriteMusicType music, Color color)
        {
            mCASModel.RequestRandomFavorites(food, music, color);
        }

        public void RequestRandomizeFace()
        {
            mCASModel.RequestRandomizeFace();
        }

        public void RequestRandomizeSim()
        {
            mCASModel.RequestRandomizeSim();
        }

        public void RequestRedo()
        {
            mCASModel.RequestRedo();
        }

        public void RequestRemoveCASPart(CASPart part)
        {
            mCASModel.RequestRemoveCASPart(part);
        }

        public void RequestRemoveCASParts(CASPart[] parts)
        {
            mCASModel.RequestRemoveCASParts(parts);
        }

        public void RequestRemoveOutfit(OutfitCategories category, int index, bool undoable)
        {
            mCASModel.RequestRemoveOutfit(category, index, undoable);
        }

        public void RequestReplaceSim(int index)
        {
            mCASModel.RequestReplaceSim(index);
        }

        public void RequestSaveHousehold()
        {
            mCASModel.RequestSaveHousehold();
        }

        public void RequestSaveSim()
        {
            mCASModel.RequestSaveSim();
        }

        public void RequestSaveSimToWorld()
        {
            mCASModel.RequestSaveSimToWorld();
        }

        public void RequestSaveUniform(string title, ulong components)
        {
            mCASModel.RequestSaveUniform(title, components);
        }

        public void RequestSecondaryNormalMapWeight(uint index, float value, bool appendToFacialBlend)
        {
            mCASModel.RequestSecondaryNormalMapWeight(index, value, appendToFacialBlend);
        }

        public void RequestSetAllHairColor(BodyTypes bodyType, List<Color> colors, bool includeEyeBrow, Color eyeBrowColor, bool finalize)
        {
            mCASModel.RequestSetAllHairColor(bodyType, colors, includeEyeBrow, eyeBrowColor, finalize);
        }

        public void RequestSetBlendAmount(BaseBlend blend, float amount)
        {
            mCASModel.RequestSetBlendAmount(blend, amount);
        }

        public void RequestSetDualBlendAmount(BaseBlend blend1, BaseBlend blend2, float amount1, float amount2)
        {
            mCASModel.RequestSetDualBlendAmount(blend1, blend2, amount1, amount2);
        }

        public void RequestSetFavoriteColor(Color favoriteColor)
        {
            mCASModel.RequestSetFavoriteColor(favoriteColor);
        }

        public void RequestSetFavoriteFood(FavoriteFoodType favoriteFood)
        {
            mCASModel.RequestSetFavoriteFood(favoriteFood);
        }

        public void RequestSetFavoriteMusic(FavoriteMusicType favoriteMusic)
        {
            mCASModel.RequestSetFavoriteMusic(favoriteMusic);
        }

        public void RequestSetHairColor(BodyTypes bodyType, int index, Color color, bool finalize)
        {
            mCASModel.RequestSetHairColor(bodyType, index, color, finalize);
        }

        public void RequestSetHairColorPreset(BodyTypes bodyType, ResourceKey hairPresetKey)
        {
            mCASModel.RequestSetHairColorPreset(bodyType, hairPresetKey);
        }

        public void RequestSetOutfit(SimOutfit newOutfit)
        {
            mCASModel.RequestSetOutfit(newOutfit);
        }

        public void RequestSetPitch(float value)
        {
            mCASModel.RequestSetPitch(value);
        }

        public void RequestSetPreviewSim(int index)
        {
            mCASModel.RequestSetPreviewSim(index);
        }

        public void RequestSetPreviewSimNoSave(int index)
        {
            mCASModel.RequestSetPreviewSimNoSave(index);
        }

        public void RequestSetSign(Zodiac zodiac)
        {
            mCASModel.RequestSetSign(zodiac);
        }

        public void RequestSetSkinTonePreset(SkinTonePreset preset)
        {
            mCASModel.RequestSetSkinTonePreset(preset);
        }

        public void RequestSetTraits(List<ulong> traits)
        {
            mCASModel.RequestSetTraits(traits);
        }

        public void RequestSetTraits(List<ulong> traits, bool playTraitAnimation)
        {
            mCASModel.RequestSetTraits(traits, playTraitAnimation);
        }

        public void RequestSetVoiceVariation(VoiceVariationType voice)
        {
            mCASModel.RequestSetVoiceVariation(voice);
        }

        public void RequestSkinTone(float value, bool forceNewOp)
        {
            mCASModel.RequestSkinTone(value, forceNewOp);
        }

        public void RequestUndo()
        {
            mCASModel.RequestUndo();
        }

        public void RequestUpdateCurrentSim(bool bAllCategories)
        {
            mCASModel.RequestUpdateCurrentSim(bAllCategories);
        }

        public void ResetPreviewSim()
        {
            mCASModel.ResetPreviewSim();
        }

        public void SaveBlendPreset(string packageName)
        {
            mCASModel.SaveBlendPreset(packageName);    
        }

        public void SavePartPreset(CASPart part, string presetName)
        {
            mCASModel.SavePartPreset(part, presetName);
        }

        public void SaveSimFacesData(List<BlendUnit> blendUnits, int tryCount, string dataDirName)
        {
            mCASModel.SaveSimFacesData(blendUnits, tryCount, dataDirName);
        }

        public bool SetHouseholdStartingFunds()
        {
            return mCASModel.SetHouseholdStartingFunds();
        }

        public void SetUIInitialized(bool initialized)
        {
            mCASModel.SetUIInitialized(initialized);
        }

        public void SimsVisible(bool visible)
        {
            mCASModel.SimsVisible(visible);
        }
        
        // Properties
        public int NumInHousehold { get { return mCASModel.NumInHousehold; } }
        public int NumPetsInHousehold { get { return mCASModel.NumPetsInHousehold; } }
        public bool SimOffsetLerpInProgress { get { return mCASModel.SimOffsetLerpInProgress; } }

        public CASAgeGenderFlags Age { get { return mCASModel.Age; } set { mCASModel.Age = value; } }
        public bool BeardUsesHairColor { get { return mCASModel.BeardUsesHairColor; } set { mCASModel.BeardUsesHairColor = value; } }
        public string Bio { get { return mCASModel.Bio; } set { mCASModel.Bio = value; } }
        public bool BodyHairUseHairColor { get { return mCASModel.BodyHairUseHairColor; } }
        public CASMode CASMode { get { return mCASModel.CASMode; } }
        public ISimDescription CurrentSimDescription { get { return mCASModel.CurrentSimDescription; } }
        public bool EyebrowsUseHairColor { get { return mCASModel.EyebrowsUseHairColor; } }
        public Color FavoriteColor { get { return mCASModel.FavoriteColor; } set { mCASModel.FavoriteColor = value; } }
        public FavoriteFoodType FavoriteFoodType { get { return mCASModel.FavoriteFoodType; } set { mCASModel.FavoriteFoodType = value; } }
        public FavoriteMusicType FavoriteMusicType { get { return mCASModel.FavoriteMusicType; } set { mCASModel.FavoriteMusicType = value; } }
        public string FirstName { get { return mCASModel.FirstName; } set { mCASModel.FirstName = value; } }
        public CASAgeGenderFlags Gender { get { return mCASModel.Gender; } set { mCASModel.Gender = value; } }
        public bool HiddenInCAS { get { return mCASModel.HiddenInCAS; } set { mCASModel.HiddenInCAS = value; } }
        public uint HighlightIndex { get { return mCASModel.HighlightIndex; } set { mCASModel.HighlightIndex = value; } }
        public CASHighlightModes HighlightMode { get { return mCASModel.HighlightMode; } set { mCASModel.HighlightMode = value; } }
        public string HouseholdName { get { return mCASModel.HouseholdName; } set { mCASModel.HouseholdName = value; } }
        public string LastName { get { return mCASModel.LastName; } set { mCASModel.LastName = value; } }
        public uint LifetimeWish { get { return mCASModel.LifetimeWish; } set { mCASModel.LifetimeWish = value; } }
        public int MaxTraits { get { return mCASModel.MaxTraits; } }
        public int NumSimsInHousehold { get { return mCASModel.NumSimsInHousehold; } }
        public OutfitCategories OutfitCategory { get { return mCASModel.OutfitCategory; } set { mCASModel.OutfitCategory = value; } }
        public int OutfitIndex { get { return mCASModel.OutfitIndex; } set { mCASModel.OutfitIndex = value; } }
        public int PreviewSimIndex { get { return mCASModel.PreviewSimIndex; } }
        public bool PropagateHairStyles { get { return mCASModel.PropagateHairStyles; } set { mCASModel.PropagateHairStyles = value; } }
        public float[] SecondaryNormalMapWeights { get { return mCASModel.SecondaryNormalMapWeights; } }
        public uint SelectionIndex { get { return mCASModel.SelectionIndex; } set { mCASModel.SelectionIndex = value; } }
        public ResKeyTable SimDescriptions { get { return mCASModel.SimDescriptions; } }
        public CASAgeGenderFlags Species { get { return mCASModel.Species; } set { mCASModel.Species = value; } }
        public float VoicePitchModifier { get { return mCASModel.VoicePitchModifier; } set { mCASModel.VoicePitchModifier = value; } }
        public VoiceVariationType VoiceVariation { get { return mCASModel.VoiceVariation; } set { mCASModel.VoiceVariation = value; } }
        public Zodiac Zodiac { get { return mCASModel.Zodiac; } set { mCASModel.Zodiac = value; } }
        public float ZoomLevel { get { return mCASModel.ZoomLevel; } }
        public bool PropagateMakeUpStyles { get { return mCASModel.PropagateMakeUpStyles; } set { mCASModel.PropagateMakeUpStyles = value; } }

        public bool Active
        {
            get { return mCASModel.Active; }
        }

        public OccultTypes CurrentOccultType
        {
            get { return mCASModel.CurrentOccultType; }
        }

        public Vector3 CurrentWingColor
        {
            get
            {
                return mCASModel.CurrentWingColor;
            }
            set
            {
                mCASModel.CurrentWingColor = value;
            }
        }

        public WingTypes CurrentWingType
        {
            get
            {
                return mCASModel.CurrentWingType;
            }
            set
            {
                mCASModel.CurrentWingType = value;
            }
        }

        public bool CurrentlyEditingTransformedWerewolf()
        {
            return mCASModel.CurrentlyEditingTransformedWerewolf();
        }

        public void ExecuteSetSkinTone(ResourceKey key)
        {
            mCASModel.ExecuteSetSkinTone(key);
        }

        public void ExitEditWerewolfMode()
        {
            mCASModel.ExitEditWerewolfMode();
        }

        public List<string> GetDeathTypeNames()
        {
            return mCASModel.GetDeathTypeNames();
        }

        public List<string> GetFairyWingNames()
        {
            return mCASModel.GetFairyWingNames();
        }

        public CASAgeGenderFlags GetFlagsByIndex(int index)
        {
            return mCASModel.GetFlagsByIndex(index);
        }

        public SimOutfit GetLastBuiltOutfit()
        {
            return mCASModel.GetLastBuiltOutfit();
        }

        public SimOutfit GetOutfitByIndex(int index)
        {
            return mCASModel.GetOutfitByIndex(index);
        }

        public int GhostDeathTypeIndex
        {
            get { return mCASModel.GhostDeathTypeIndex; }
        }

        public string GhostDeathTypeString
        {
            get
            {
                return mCASModel.GhostDeathTypeString;
            }
            set
            {
                mCASModel.GhostDeathTypeString = value;
            }
        }

        public void RequestExportSupernaturalData(int simSpreadsheetID)
        {
            mCASModel.RequestExportSupernaturalData(simSpreadsheetID);
        }

        public void RequestReapplyOfWerewolfTransformedOutfit()
        {
            mCASModel.RequestReapplyOfWerewolfTransformedOutfit();
        }

        public void RequestSetFairyWings(WingTypes newType)
        {
            mCASModel.RequestSetFairyWings(newType);
        }

        public void RequestSetFairyWings(Vector3 newColor, Vector3 oldColor)
        {
            mCASModel.RequestSetFairyWings(newColor, oldColor);
        }

        public void RequestSetFairyWings(WingTypes newType, Vector3 newColor)
        {
            mCASModel.RequestSetFairyWings(newType, newColor);
        }

        public void RequestSetOccultType(OccultTypes newType)
        {
            mCASModel.RequestSetOccultType(newType);
        }

        public void SetFairyWingColor(Vector3 newColor, float perceivedLuminance)
        {
            mCASModel.SetFairyWingColor(newColor, perceivedLuminance);
        }

        public void SetFairyWingType(WingTypes newType)
        {
            mCASModel.SetFairyWingType(newType);
        }

        public void SetVisualOverride(OccultTypes type)
        {
            mCASModel.SetVisualOverride(type);
        }

        public void SetupEditWerewolfMode()
        {
            mCASModel.SetupEditWerewolfMode();
        }

        public void UpdateVisualsToCurrent()
        {
            mCASModel.UpdateVisualsToCurrent();
        }

        public void UpdateWerewolfOutfit()
        {
            mCASModel.UpdateWerewolfOutfit();
        }

        public List<object> GetHairColorPresets()
        {
            return mCASModel.GetHairColorPresets();
        }

        public bool IsEditingUniform
        {
            get { return mCASModel.IsEditingUniform; }
        }

        public void RequestLoadSim(ISimDescription simDesc, bool inHousehold)
        {
            mCASModel.RequestLoadSim(simDesc, inHousehold);
        }

        public OccultTypes GetOccultTypeFromKey(ResourceKey key)
        {
            return mCASModel.GetOccultTypeFromKey(key);
        }

        public string GetRobotHoverVFX(CASPart part)
        {
            return mCASModel.GetRobotHoverVFX(part);
        }

        public bool IsEditingMannequin
        {
            get { return mCASModel.IsEditingMannequin; }
        }

        public void RequestLoadSim(ISimDescription simDesc, bool inHousehold, bool isRobotRestore)
        {
            mCASModel.RequestLoadSim(simDesc, inHousehold, isRobotRestore);
        }

        public void RequestRandomizeRobot()
        {
            mCASModel.RequestRandomizeRobot();
        }

        public void RequestRestoreOriginalRobot()
        {
            mCASModel.RequestRestoreOriginalRobot();
        }

        public void RequestSetOriginalRobot()
        {
            mCASModel.RequestSetOriginalRobot();
        }

        public void StartTalkingRobotFaceVFX()
        {
            mCASModel.StartTalkingRobotFaceVFX();
        }

        public void StopTalkingRobotFaceVFX()
        {
            mCASModel.StopTalkingRobotFaceVFX();
        }
    }
}
