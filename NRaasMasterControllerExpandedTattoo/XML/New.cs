public class CASTattoo : CASUnderwear, ICASUINode
{
    // Fields
    internal static CASTattoo gSingleton = null;
    [Tunable, TunableComment("Default AnkleLeft Scale")]
    public static float kDefaultAnkleLeftScales = 0.5f;
    [Tunable, TunableComment("Default AnkleOuterL Scale")]
    public static float kDefaultAnkleOuterLScales = 0.5f;
    [Tunable, TunableComment("Default AnkleOuterR Scale")]
    public static float kDefaultAnkleOuterRScales = 0.5f;
    [TunableComment("Default AnkleRight Scale"), Tunable]
    public static float kDefaultAnkleRightScales = 0.5f;
    [Tunable, TunableComment("Default Bellybutton Scale")]
    public static float kDefaultBellybuttonScales = 0.5f;
    [Tunable, TunableComment("Default BicepLeft Scale")]
    public static float kDefaultBicepLeftScales = 0.5f;
    [TunableComment("Default BicepRight Scale"), Tunable]
    public static float kDefaultBicepRightScales = 0.5f;
    [Tunable, TunableComment("Default BreastUpperL Scale")]
    public static float kDefaultBreastUpperLScales = 0.7f;
    [Tunable, TunableComment("Default BreastUpperR Scale")]
    public static float kDefaultBreastUpperRScales = 0.7f;
    [TunableComment("Default ButtLeft Scale"), Tunable]
    public static float kDefaultButtLeftScales = 0.7f;
    [TunableComment("Default ButtRight Scale"), Tunable]
    public static float kDefaultButtRightScales = 0.7f;
    [Tunable, TunableComment("Default CalfBackL Scale")]
    public static float kDefaultCalfBackLScales = 0.7f;
    [Tunable, TunableComment("Default CalfBackR Scale")]
    public static float kDefaultCalfBackRScales = 0.7f;
    [TunableComment("Default CalfFrontL Scale"), Tunable]
    public static float kDefaultCalfFrontLScales = 0.7f;
    [Tunable, TunableComment("Default CalfFrontR Scale")]
    public static float kDefaultCalfFrontRScales = 0.7f;
    [TunableComment("Default CheekLeft Scale"), Tunable]
    public static float kDefaultCheekLeftScales = 0.5f;
    [TunableComment("Default CheekRight Scale"), Tunable]
    public static float kDefaultCheekRightScales = 0.5f;
    [TunableComment("Default Chest Scale"), Tunable]
    public static float kDefaultChestScales = 0.65f;
    [TunableComment("Default FootLeft Scale"), Tunable]
    public static float kDefaultFootLeftScales = 0.7f;
    [Tunable, TunableComment("Default FootRight Scale")]
    public static float kDefaultFootRightScales = 0.7f;
    [Tunable, TunableComment("Default ForearmLeft Scale")]
    public static float kDefaultForearmLeftScales = 0.5f;
    [TunableComment("Default ForearmRight Scale"), Tunable]
    public static float kDefaultForearmRightScales = 0.5f;
    [Tunable, TunableComment("Default Forehead Scale")]
    public static float kDefaultForeheadScales = 0.5f;
    [TunableComment("Default FullBack Scale"), Tunable]
    public static float kDefaultFullBackScales = 0.7f;
    [Tunable, TunableComment("Default FullBody Scale")]
    public static float kDefaultFullBodyScales = 1f;
    [Tunable, TunableComment("Default FullFace Scale")]
    public static float kDefaultFullFaceScales = 1f;
    [Tunable, TunableComment("Default HandLeft Scale")]
    public static float kDefaultHandLeftScales = 0.4f;
    [Tunable, TunableComment("Default HandRight Scale")]
    public static float kDefaultHandRightScales = 0.4f;
    [Tunable, TunableComment("Default HipLeft Scale")]
    public static float kDefaultHipLeftScales = 0.7f;
    [TunableComment("Default HipRight Scale"), Tunable]
    public static float kDefaultHipRightScales = 0.7f;
    [TunableComment("Default LowerBack Scale"), Tunable]
    public static float kDefaultLowerBackScales = 0.5f;
    [TunableComment("Default LowerBelly Scale"), Tunable]
    public static float kDefaultLowerBellyScales = 0.7f;
    [Tunable, TunableComment("Default LowerLowBack Scale")]
    public static float kDefaultLowerLowBackScales = 0.7f;
    [Tunable, TunableComment("Default Neck Scale")]
    public static float kDefaultNeckScales = 0.5f;
    [TunableComment("Default NippleLeft Scale"), Tunable]
    public static float kDefaultNippleLeftScales = 0.5f;
    [Tunable, TunableComment("Default NippleRight Scale")]
    public static float kDefaultNippleRightScales = 0.5f;
    [Tunable, TunableComment("Default PalmLeft Scale")]
    public static float kDefaultPalmLeftScales = 0.4f;
    [Tunable, TunableComment("Default PalmRight Scale")]
    public static float kDefaultPalmRightScales = 0.4f;
    [Tunable, TunableComment("Default Pubic Scale")]
    public static float kDefaultPubicScales = 0.5f;
    [TunableComment("Default RibsLeft Scale"), Tunable]
    public static float kDefaultRibsLeftScales = 0.7f;
    [Tunable, TunableComment("Default RibsRight Scale")]
    public static float kDefaultRibsRightScales = 0.7f;
    [Tunable, TunableComment("Default ShoulderBackL Scale")]
    public static float kDefaultShoulderBackLScales = 0.7f;
    [TunableComment("Default ShoulderBackR Scale"), Tunable]
    public static float kDefaultShoulderBackRScales = 0.7f;
    [Tunable, TunableComment("Default ShoulderLeft Scale")]
    public static float kDefaultShoulderLeftScales = 0.5f;
    [TunableComment("Default ShoulderRight Scale"), Tunable]
    public static float kDefaultShoulderRightScales = 0.5f;
    [Tunable, TunableComment("Default ThighBackL Scale")]
    public static float kDefaultThighBackLScales = 0.7f;
    [Tunable, TunableComment("Default ThighBackR Scale")]
    public static float kDefaultThighBackRScales = 0.7f;
    [Tunable, TunableComment("Default ThighFrontL Scale")]
    public static float kDefaultThighFrontLScales = 0.7f;
    [Tunable, TunableComment("Default ThighFrontR Scale")]
    public static float kDefaultThighFrontRScales = 0.7f;
    [Tunable, TunableComment("Default Throat Scale")]
    public static float kDefaultThroatScales = 0.7f;
    [TunableComment("Default UpperBack Scale"), Tunable]
    public static float kDefaultUpperBackScales = 0.6f;
    [TunableComment("Default WristLeft Scale"), Tunable]
    public static float kDefaultWristLeftScales = 0.5f;
    [Tunable, TunableComment("Default WristRight Scale")]
    public static float kDefaultWristRightScales = 0.5f;
    private const uint kDisabledShade = 0xff808080;
    private readonly CASPart kInvalidCASPart;
    private readonly Vector2 kInvalidMousePos;
    private const int kMaxUndoSteps = 0x186a0;
    [Tunable, TunableComment("Enable Adult Tattoo")]
    public static int kNaughtyBits = 0x0;
    [Tunable, TunableComment("Minimum opacity")]
    private static float kOpacityMinumum = 0.1f;
    [Tunable, TunableComment("Delay before pickup")]
    internal static float kPickupDelay = 0.75f;
    private float[] kStorageReferenceDimensions;
    private const uint kTattooTemplateCount = 0x5;
    private const string kTriggerMapName = "casTattoo";
    private Button mAcceptButton;
    private int mActiveLayer;
    private TattooID mActiveTattooID;
    private CASPart mActiveTattooPart;
    private bool mAdvancedMode;
    private Button mAdvancedModeButton;
    private WindowBase mAdvancedPanel;
    private WindowBase mArmPanel;
    private WindowBase mBackPanel;
    private WindowBase mBodyPanel;
    private bool mbTargetSimAngleActive;
    private WindowBase mChestPanel;
    private WindowBase mClickedWin;
    private Button mColorPicker2Button;
    private Button mColorPickerButton;
    private CASPartPreset mCurrentPreset;
    private ImageDrawable mCursorImage;
    private ImageDrawable mCursorNoDragImage;
    private Button mDeleteButton;
    private WindowBase mFacePanel;
    private FadeEffect mFadeEffect;
    private float mFadeTime;
    private bool mFilter;
    private Button mFilterButton;
    private GlideEffect mGlideEffect;
    private List<uint> mInvalidIds;
    private Button[] mLayerButtons;
    private Window[] mLayerThumbs;
    private WindowBase mLegPanel;
    private Vector2 mMouseClickPos;
    private Dictionary<ControlIDs, Button> mNavButtons;
    private Slider mOpacitySlider;
    private ObjectGuid mPickupScript;
    private Button mRedoButton;
    private Button mRemoveAllButton;
    private Button mRemoveLayerButton;
    private bool mRemoveTattooOnUndo;
    private Button mSaveButton;
    private Slider mScaleSlider;
    private Button mShareButton;
    private float mTargetSimAngle;
    private StopWatch mTargetSimAngleStopWatch;
    private CASState mTargetState;
    private Rect mTattooBaseRect;
    private ItemGrid mTattooGrid;
    private float[] mTattooLayerScaleData;
    private CASPart[] mTattooParts;
    private ItemGrid mTattooPresetsGrid;
    private TattooTemplateData[] mTemplateParts;
    protected uint mTriggerHandle;
    private Dictionary<TattooID, float> mTunedScales;
    private Button mUndoButton;
    private List<UndoRedoData> mUndoRedoQueue;
    private int mUndoRedoQueueIndex;
    private bool mWasCustom;
    private static Dictionary<ControlIDs, bool> sNavButtonStates = new Dictionary<ControlIDs, bool>();
    private static Layout sTattooLayout = null;

    // Events
    public event UINodeShutdownCallback UINodeShutdown;

    // Methods
    public CASTattoo(uint winHandle) : base(winHandle)
    {
        this.mTunedScales = new Dictionary<TattooID, float>();
        this.kInvalidCASPart = new CASPart();
        this.mCurrentPreset = new CASPartPreset();
        this.mNavButtons = new Dictionary<ControlIDs, Button>();
        this.mLayerButtons = new Button[0x5];
        this.mLayerThumbs = new Window[0x5];
        this.kInvalidMousePos = new Vector2(-1f, -1f);
        this.mTargetSimAngleStopWatch = StopWatch.Create(StopWatch.TickStyles.Seconds);
        float[] numArray = new float[0x4];
        numArray[0x2] = 1f;
        numArray[0x3] = 1f;
        this.kStorageReferenceDimensions = numArray;
        this.mUndoRedoQueue = new List<UndoRedoData>();
        this.mTemplateParts = new TattooTemplateData[0x5];
        Enum.GetValues(typeof(TattooID));
    }

    public void Activate(bool fastTransition)
    {
        if (this.mFadeEffect != null)
        {
            if (fastTransition)
            {
                this.mFadeEffect.Duration = 0f;
            }
            else
            {
                this.mFadeEffect.Duration = this.mFadeTime;
            }
        }
        base.Visible = true;
    }

    private bool ActiveTattooPartInUse()
    {
        if (!Responder.Instance.CASModel.GetWornParts(this.mActiveTattooPart.BodyType).Contains(this.mActiveTattooPart))
        {
            return false;
        }
        return true;
    }

    private bool AddCompositePartsGridItem(ResourceKey layoutKey, CASPartPreset preset, bool testValid)
    {
        WindowBase windowByExportID = UIManager.LoadLayout(layoutKey).GetWindowByExportID(0x1);
        if (windowByExportID == null)
        {
            return false;
        }
        CustomContentIcon childByID = windowByExportID.GetChildByID(0x23, true) as CustomContentIcon;
        childByID.ContentType = UIUtils.GetCustomContentType(preset.mPart.Key, preset.mPresetId);
        Window window = windowByExportID.GetChildByID(0x20, true) as Window;
        if (window != null)
        {
            ImageDrawable drawable = window.Drawable as ImageDrawable;
            if (drawable != null)
            {
                ThumbnailKey key = new ThumbnailKey(preset.mPart.Key, (preset.mPresetId != uint.MaxValue) ? ((int) preset.mPresetId) : 0xffffffff, (uint) preset.mPart.BodyType, (uint) preset.mPart.AgeGenderSpecies, ThumbnailSize.Large);
                drawable.Image = UIManager.GetCASThumbnailImage(key);
                window.Invalidate();
            }
        }
        windowByExportID.TooltipText = this.GetPartName(preset.mPart);
        bool flag = true;
        if (testValid)
        {
            flag = this.TestPresetValidity(preset);
            if (!flag)
            {
                this.mInvalidIds.Add(preset.mPresetId);
            }
        }
        else
        {
            flag = !this.mInvalidIds.Contains(preset.mPresetId);
        }
        if (!flag)
        {
            return false;
        }
        Window window2 = windowByExportID.GetChildByID(0x2a, true) as Window;
        window2.Visible = true;
        this.mTattooGrid.AddItem(new ItemGridCellItem(windowByExportID, preset));
        return true;
    }

    private bool AddPartsGridItem(ItemGrid catalog, ResourceKey layoutKey, CASPart part, ResourceKeyContentCategory contentType)
    {
        WindowBase windowByExportID = UIManager.LoadLayout(layoutKey).GetWindowByExportID(0x1);
        if (windowByExportID == null)
        {
            return false;
        }
        if (part.Key != this.kInvalidCASPart.Key)
        {
            Window childByID = windowByExportID.GetChildByID(0x20, true) as Window;
            if (childByID != null)
            {
                ImageDrawable drawable = childByID.Drawable as ImageDrawable;
                if (drawable != null)
                {
                    ThumbnailKey key = new ThumbnailKey(part.Key, (int) CASUtils.PartDataGetPresetId(part.Key, 0x0), (uint) part.BodyType, (uint) part.AgeGenderSpecies, ThumbnailSize.Large);
                    drawable.Image = UIManager.GetCASThumbnailImage(key);
                    childByID.Invalidate();
                }
            }
            CustomContentIcon icon = windowByExportID.GetChildByID(0x23, true) as CustomContentIcon;
            if (UIUtils.IsContentTypeDisabled(contentType))
            {
                return false;
            }
            if (UIUtils.IsCustomFiltered(contentType))
            {
                this.mFilterButton.Enabled = true;
            }
            else if (this.mFilter)
            {
                return false;
            }
            icon.ContentType = contentType;
            windowByExportID.TooltipText = this.GetPartName(part);
            if (Responder.Instance.CASModel.ActiveWardrobeContains(part))
            {
                WindowBase base3 = windowByExportID.GetChildByID(0x29, true);
                if (base3 != null)
                {
                    base3.Visible = true;
                }
            }
        }
        catalog.AddItem(new ItemGridCellItem(windowByExportID, part));
        return true;
    }

    private void AddPresetGridItem(CASPartPreset preset, Vector3[] colors, ResourceKeyContentCategory contentType)
    {
        WindowBase windowByExportID = UIManager.LoadLayout(ResourceKey.CreateUILayoutKey("CASTattooColorGridItem", 0x0)).GetWindowByExportID(0x1);
        if (windowByExportID != null)
        {
            WindowBase childByID = windowByExportID.GetChildByID(0x20, true);
            int length = colors.Length;
            for (int i = 0x0; i < 0x4; i++)
            {
                WindowBase base4 = childByID.GetChildByID((uint) (0x30 + i), false);
                if (i < length)
                {
                    base4.Visible = true;
                    Color color = CompositorUtil.Vector3ToColor(colors[i]);
                    color.Alpha = 0xff;
                    base4.ShadeColor = color;
                }
                else
                {
                    base4.Visible = false;
                }
            }
            CustomContentIcon icon = windowByExportID.GetChildByID(0x23, true) as CustomContentIcon;
            icon.ContentType = contentType;
            this.mTattooPresetsGrid.AddItem(new ItemGridCellItem(windowByExportID, preset));
        }
    }

    private void AddTemplatePart(CASPart part, CASPartPreset preset)
    {
        this.mTemplateParts[this.mActiveLayer].mTemplatePart = part;
        this.mTemplateParts[this.mActiveLayer].mPreset = preset;
    }

    private void AddUndoRedoStep()
    {
        if (this.mUndoRedoQueueIndex > 0x0)
        {
            this.mUndoButton.Enabled = true;
        }
        this.mRedoButton.Enabled = false;
        if (this.mUndoRedoQueue.Count != this.mUndoRedoQueueIndex)
        {
            this.mUndoRedoQueue.RemoveRange(this.mUndoRedoQueueIndex, this.mUndoRedoQueue.Count - this.mUndoRedoQueueIndex);
        }
        if (this.mUndoRedoQueue.Count > 0x186a0)
        {
            this.mUndoRedoQueue.RemoveAt(0x0);
        }
        else
        {
            this.mUndoRedoQueueIndex++;
        }
        float num = ((float) this.mOpacitySlider.Value) / ((float) this.mOpacitySlider.MaxValue);
        uint opacityColor = 0xffffff | (0xff000000 & (((uint) (num * 255f)) << 0x18));
        this.mUndoRedoQueue.Add(new UndoRedoData(this.mTemplateParts, opacityColor, this.mTattooLayerScaleData));
    }

    private void CancelSimRotation()
    {
        if (this.mbTargetSimAngleActive)
        {
            this.mbTargetSimAngleActive = false;
            base.Tick -= new UIEventHandler<UIEventArgs>(this.OnTick);
            this.mTargetSimAngleStopWatch.Stop();
        }
    }

    private void ClearAllParts()
    {
        for (int i = 0x0; i < 0x5L; i++)
        {
            this.SetPart(this.kInvalidCASPart, i);
        }
    }

    private void ClearClickEvent()
    {
        Simulator.DisableScript(this.mPickupScript);
        this.mPickupScript = new ObjectGuid();
        this.mMouseClickPos = this.kInvalidMousePos;
        this.mClickedWin = null;
    }

    private void ClearTemplates()
    {
        for (uint i = 0x0; i < 0x5; i++)
        {
            this.mTemplateParts[i].mPreset = null;
            this.mTemplateParts[i].mTemplatePart.Key = ResourceKey.kInvalidResourceKey;
        }
    }

    private bool CurrentTemplatePartValid()
    {
        return (this.mTemplateParts[this.mActiveLayer].mTemplatePart.Key != ResourceKey.kInvalidResourceKey);
    }

    public override void Dispose()
    {
        this.ClearClickEvent();
        base.CancelDragDrop();
        foreach (CursorId id in Enum.GetValues(typeof(CursorId)))
        {
            UIManager.DeRegisterWindowCursor((uint) id);
        }
        this.CancelSimRotation();
        UIManager.GetSceneWindow().MouseDown -= new UIEventHandler<UIMouseEventArgs>(this.OnSceneWindowCameraMouseDown);
        Responder.Instance.CASModel.HighlightMode = CASHighlightModes.Parts;
        base.Dispose();
    }

    private Rect GetActivePresetRect()
    {
        int index = this.mActiveLayer * 0x4;
        if (this.mTattooLayerScaleData != null)
        {
            return new Rect(this.mTattooLayerScaleData[index], this.mTattooLayerScaleData[index + 0x1], this.mTattooLayerScaleData[index + 0x2], this.mTattooLayerScaleData[index + 0x3]);
        }
        return new Rect();
    }

    private float GetActivePresetScale()
    {
        return (this.GetActivePresetRect().Width / this.mTattooBaseRect.Width);
    }

    private float GetActiveTattooOpacity()
    {
        return this.GetFloatOpacityFromOpacityColor(CASUtils.ExtractTattooOpacity(this.CurrentPreset.mPresetString));
    }

    private Color[] GetColors()
    {
        if (!this.CurrentTemplatePartValid())
        {
            return null;
        }
        uint[] numArray = CASUtils.ExtractTattooColors(this.GetCurrentTemplatePresetString(), 0x1);
        Color[] colorArray = new Color[numArray.Length];
        int num = 0x0;
        foreach (uint num2 in numArray)
        {
            colorArray[num++] = new Color(num2);
        }
        return colorArray;
    }

    private CASPart GetCurrentTemplatePart()
    {
        return this.mTemplateParts[this.mActiveLayer].mTemplatePart;
    }

    private string GetCurrentTemplatePresetString()
    {
        return this.mTemplateParts[this.mActiveLayer].mPreset.mPresetString;
    }

    private CASPart GetDefaultPart()
    {
        foreach (CASPart part in this.mTattooParts)
        {
            if (part.Key.InstanceId == 0x8bea918d30acec87L)
            {
                return part;
            }
        }
        return this.kInvalidCASPart;
    }

    private void GetDimensionFloatsFromRect(float scale, Rect rect, ref float[] outputArray, uint baseIndex)
    {
        Vector2 vector = rect.Center();
        Vector2 vector2 = new Vector2(rect.Width, rect.Height);
        vector2 = (Vector2) (vector2 * (0.5f * scale));
        outputArray[baseIndex] = vector.x - vector2.x;
        outputArray[(int) ((IntPtr) (baseIndex + 0x1))] = vector.y - vector2.y;
        outputArray[(int) ((IntPtr) (baseIndex + 0x2))] = vector.x + vector2.x;
        outputArray[(int) ((IntPtr) (baseIndex + 0x3))] = vector.y + vector2.y;
    }

    private float GetFloatOpacityFromOpacityColor(uint colorAlpha)
    {
        uint num = (uint) ((colorAlpha & 0xff000000) >> 0x18);
        return (((float) num) / 255f);
    }

    private string GetPartName(CASPart part)
    {
        if (!(part.Key != ResourceKey.kInvalidResourceKey))
        {
            return "";
        }
        return CASUtils.PartDataGetName(part.Key);
    }

    private int GetTattooTemplateUsageIndex(CASPart part)
    {
        if (part.Key == ResourceKey.kInvalidResourceKey)
        {
            return 0xffffffff;
        }
        CASPartPreset preset = new CASPartPreset(part, CASUtils.PartDataGetPresetId(part.Key, 0x0), CASUtils.PartDataGetPreset(part.Key, 0x0));
        return CASUtils.GetTattooTemplateIndex(this.CurrentPreset.mPresetString, preset.mPresetString, false);
    }

    private int GetTattooTemplateUsageIndexInPreset(CASPart part, string preset)
    {
        if (part.Key == ResourceKey.kInvalidResourceKey)
        {
            return 0xffffffff;
        }
        CASPartPreset preset2 = new CASPartPreset(part, CASUtils.PartDataGetPresetId(part.Key, 0x0), CASUtils.PartDataGetPreset(part.Key, 0x0));
        return CASUtils.GetTattooTemplateIndex(preset, preset2.mPresetString, false);
    }

    private CASPart[] GetVisibleParts(BodyTypes bodyType)
    {
        ArrayList visibleCASParts = Responder.Instance.CASModel.GetVisibleCASParts(bodyType);
        visibleCASParts.Insert(0x0, this.kInvalidCASPart);
        return (visibleCASParts.ToArray(typeof(CASPart)) as CASPart[]);
    }

    private void Glide(bool glideOut)
    {
        this.mGlideEffect.TriggerEffect(glideOut);
    }

    private bool HasAnyPartsInUse()
    {
        foreach (TattooTemplateData data in this.mTemplateParts)
        {
            if (data.mTemplatePart.Key != this.kInvalidCASPart.Key)
            {
                return true;
            }
        }
        return false;
    }

    public override void Init()
    {
        foreach (object obj2 in base.EffectList)
        {
            this.mFadeEffect = obj2 as FadeEffect;
            if (this.mFadeEffect != null)
            {
                this.mFadeTime = this.mFadeEffect.Duration;
                break;
            }
        }
        base.FadeTransitionFinished += new UIEventHandler<UIHandledEventArgs>(this.OnFadeFinished);
        base.Init();
        this.mbTargetSimAngleActive = false;
        this.mTattooGrid = base.GetChildByID(0x92fa000, true) as ItemGrid;
        this.mTattooGrid.ItemClicked += new ItemGrid.ItemGridItemClickedEventHandler(this.OnPartsGridSelect);
        this.mTattooGrid.ForceCellTooltips = CASController.Singleton.DebugTooltips;
        this.mTattooPresetsGrid = base.GetChildByID(0x92fa001, true) as ItemGrid;
        this.mTattooPresetsGrid.ItemClicked += new ItemGrid.ItemGridItemClickedEventHandler(this.OnPresetGridSelect);
        this.mTattooPresetsGrid.ForceCellTooltips = CASController.Singleton.DebugTooltips;
        for (ControlIDs ds = ControlIDs.LegGroupButton; ds <= ControlIDs.NippleButton; ds += (ControlIDs) 0x1)
        {
            Button button = base.GetChildByID((uint) ds, true) as Button;
            if (button != null)
            {
                this.mNavButtons.Add(ds, button);
                button.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnNavButtonClicked);
            }
        }
        this.mArmPanel = base.GetChildByID(0x92fa060, true);
        this.mChestPanel = base.GetChildByID(0x92fa061, true);
        this.mBackPanel = base.GetChildByID(0x92fa062, true);
        this.mLegPanel = base.GetChildByID(0x92fa063, true);
        this.mFacePanel = base.GetChildByID(0x92fa064, true);
        this.mBodyPanel = base.GetChildByID(0x92fa065, true);
        this.mAdvancedModeButton = base.GetChildByID(0x92fa075, true) as Button;
        this.mAdvancedModeButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnAdvancedClick);
        this.mAdvancedPanel = base.GetChildByID(0x92fa020, true);
        for (uint i = 0x0; i < 0x5; i++)
        {
            this.mLayerButtons[i] = base.GetChildByID(0x92fa021 + i, true) as Button;
            this.mLayerThumbs[i] = base.GetChildByID(0x92fa026 + i, true) as Window;
            this.mLayerButtons[i].MouseDown += new UIEventHandler<UIMouseEventArgs>(this.OnLayerMouseDown);
            this.mLayerButtons[i].MouseUp += new UIEventHandler<UIMouseEventArgs>(this.OnLayerMouseUp);
            this.mLayerButtons[i].MouseMove += new UIEventHandler<UIMouseEventArgs>(this.OnLayerMouseMove);
            this.mLayerButtons[i].DragOver += new UIEventHandler<UIDragEventArgs>(this.OnMarkDragOver);
            this.mLayerButtons[i].DragEnter += new UIEventHandler<UIDragEventArgs>(this.OnMarkDragOver);
            this.mLayerButtons[i].DragDrop += new UIEventHandler<UIDragEventArgs>(this.OnMarkDragDrop);
            this.mLayerButtons[i].CursorID = 0x5aed101;
        }
        this.mAcceptButton = base.GetChildByID(0x92fa02d, true) as Button;
        this.mAcceptButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnAcceptClick);
        Button childByID = base.GetChildByID(0x92fa02e, true) as Button;
        childByID.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnCancelClick);
        this.mRemoveLayerButton = base.GetChildByID(0x92fa083, true) as Button;
        this.mRemoveLayerButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnLayerRemove);
        this.mUndoButton = base.GetChildByID(0x92fa080, true) as Button;
        this.mUndoButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnUndoClick);
        this.mRedoButton = base.GetChildByID(0x92fa081, true) as Button;
        this.mRedoButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnRedoClick);
        this.mFilterButton = base.GetChildByID(0x92fa070, true) as Button;
        this.mFilterButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnFilterClick);
        this.mDeleteButton = base.GetChildByID(0x92fa071, true) as Button;
        this.mDeleteButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnDeleteClick);
        this.mShareButton = base.GetChildByID(0x92fa072, true) as Button;
        this.mShareButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnShareClick);
        this.mSaveButton = base.GetChildByID(0x92fa073, true) as Button;
        this.mSaveButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnSaveClick);
        this.mRemoveAllButton = base.GetChildByID(0x92fa076, true) as Button;
        this.mRemoveAllButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnRemoveAllClick);
        this.mColorPickerButton = base.GetChildByID(0x92fa074, true) as Button;
        this.mColorPickerButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnColorPickerClick);
        this.mColorPicker2Button = base.GetChildByID(0x92fa082, true) as Button;
        this.mColorPicker2Button.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnColorPickerClick);
        this.mScaleSlider = base.GetChildByID(0x92fa02b, true) as Slider;
        this.mScaleSlider.MouseUp += new UIEventHandler<UIMouseEventArgs>(this.OnScaleSliderMouseUp);
        this.mScaleSlider.SliderValueChange += new UIEventHandler<UIValueChangedEventArgs>(this.OnScaleSliderChanged);
        this.mOpacitySlider = base.GetChildByID(0x92fa02c, true) as Slider;
        this.mOpacitySlider.MouseUp += new UIEventHandler<UIMouseEventArgs>(this.OnOpacitySliderMouseUp);
        this.mOpacitySlider.SliderValueChange += new UIEventHandler<UIValueChangedEventArgs>(this.OnOpacitySliderChanged);
        this.mOpacitySlider.MinValue = (int) (this.mOpacitySlider.MaxValue * kOpacityMinumum);
        this.mTattooParts = this.GetVisibleParts(BodyTypes.Tattoo);
        foreach (object obj3 in base.EffectList)
        {
            this.mGlideEffect = obj3 as GlideEffect;
            if (this.mGlideEffect != null)
            {
                break;
            }
        }
        this.InitCursors();
        UIManager.GetSceneWindow().MouseDown += new UIEventHandler<UIMouseEventArgs>(this.OnSceneWindowCameraMouseDown);
        this.PopulateTunedScales();
        this.RestoreState();
        this.OnNavButtonClickedHelper();
        CASFacialDetails.gSingleton.SetTattooPanel();
        this.UpdateRemoveAllButton();
        this.SetTattooCam(this.mActiveTattooID);
        Responder.Instance.CASModel.HighlightMode = CASHighlightModes.None;
    }

    private void InitCursors()
    {
        Layout layout = UIManager.LoadLayout(ResourceKey.CreateUILayoutKey("CASTattooDragCursor", 0x0));
        WindowBase windowByExportID = layout.GetWindowByExportID(0x1);
        windowByExportID.Visible = true;
        Window childByID = windowByExportID.GetChildByID(0x20, true) as Window;
        this.mCursorImage = childByID.Drawable as ImageDrawable;
        UIManager.RegisterWindowCursor(0xa9f6d10, windowByExportID, new Vector2(20f, 22f));
        windowByExportID = layout.GetWindowByExportID(0x2);
        windowByExportID.Visible = true;
        childByID = windowByExportID.GetChildByID(0x21, true) as Window;
        this.mCursorNoDragImage = childByID.Drawable as ImageDrawable;
        UIManager.RegisterWindowCursor(0xa9f6d11, windowByExportID, new Vector2(20f, 22f));
    }

    private bool IsUsingTattooTemplate(CASPart part)
    {
        return (this.GetTattooTemplateUsageIndex(part) != 0xffffffff);
    }

    private bool IsUsingTattooTemplatePreset(CASPart part, CASPartPreset preset)
    {
        return (CASUtils.GetTattooTemplateIndex(this.CurrentPreset.mPresetString, preset.mPresetString, true) != 0xffffffff);
    }

    internal static void Load()
    {
        sTattooLayout = UIManager.LoadLayoutAndAddToWindow(ResourceKey.CreateUILayoutKey("CASTattoo", 0x0), UICategory.CAS);
        gSingleton = sTattooLayout.GetWindowByExportID(0x1) as CASTattoo;
    }

    private void OnAcceptClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
    {
        if (this.HasAnyPartsInUse())
        {
            Responder.Instance.CASModel.RequestFinalizeCommitPresetToPart();
            this.AdvancedMode = false;
            this.PopulateTattooGrid(false, true);
            this.mTattooGrid.ShowSelectedItem(false);
        }
        else
        {
            this.ClearAllParts();
            this.ClearTemplates();
            this.CurrentPreset.mPresetId = 0x0;
            this.CurrentPreset.mPresetString = CASUtils.PartDataGetPreset(this.mActiveTattooPart.Key, 0x0);
            this.AdvancedMode = false;
            this.mRemoveTattooOnUndo = true;
            Responder.Instance.CASModel.RequestUndo();
        }
        eventArgs.Handled = true;
    }

    private void OnAdvancedClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
    {
        this.AdvancedMode = true;
        eventArgs.Handled = true;
    }

    private void OnCancelClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
    {
        bool flag = this.mUndoRedoQueueIndex > 0x1;
        this.AdvancedMode = false;
        if (flag)
        {
            Responder.Instance.CASModel.RequestUndo();
        }
        else
        {
            this.PopulateTattooGrid(false, this.mWasCustom);
        }
        eventArgs.Handled = true;
    }

    private void OnColorPickerClick(WindowBase sender, UIButtonClickEventArgs args)
    {
        Simulator.AddObject(new OneShotFunctionTask(new Function(this.ShowColorPickerTask)));
        args.Handled = true;
    }

    private void OnColorsChanged(Color[] colors)
    {
        this.SetColors(colors);
    }

    private void OnColorsSaved(Color[] colors)
    {
        bool flag = false;
        CASPart currentTemplatePart = this.GetCurrentTemplatePart();
        if (currentTemplatePart.Key != this.kInvalidCASPart.Key)
        {
            ObjectDesigner.SetCASPart(currentTemplatePart.Key);
            flag = ObjectDesigner.AddDesignPreset(this.GetCurrentTemplatePresetString()) != uint.MaxValue;
        }
        if (flag)
        {
            this.PopulatePresetsGrid(this.GetCurrentTemplatePart(), this.mFilterButton.Selected);
            CASController.Singleton.ErrorMsg(CASErrorCode.SaveSuccess);
        }
        else
        {
            CASController.Singleton.ErrorMsg(CASErrorCode.SaveFailed);
        }
    }

    private void OnDeleteClick(WindowBase sender, UIButtonClickEventArgs args)
    {
        if (this.mTattooPresetsGrid.Count > 0x0)
        {
            CASPart currentTemplatePart = this.GetCurrentTemplatePart();
            if ((currentTemplatePart.Key != this.kInvalidCASPart.Key) && (this.mTattooPresetsGrid.SelectedItem >= 0x0))
            {
                ObjectDesigner.SetCASPart(currentTemplatePart.Key);
                CASPartPreset selectedTag = this.mTattooPresetsGrid.SelectedTag as CASPartPreset;
                if (selectedTag != null)
                {
                    ObjectDesigner.RemoveDesignPresetById(selectedTag.mPresetId);
                    this.mDeleteButton.Enabled = false;
                    this.PopulatePresetsGrid(currentTemplatePart, this.mFilterButton.Selected);
                }
            }
        }
        else
        {
            CASPart defaultPart = this.GetDefaultPart();
            if ((defaultPart.Key != this.kInvalidCASPart.Key) && (this.mTattooGrid.SelectedItem >= 0x0))
            {
                ObjectDesigner.SetCASPart(defaultPart.Key);
                CASPartPreset preset2 = this.mTattooGrid.SelectedTag as CASPartPreset;
                if (preset2 != null)
                {
                    ObjectDesigner.RemoveDesignPresetById(preset2.mPresetId);
                    this.PopulateTattooGrid(false);
                }
            }
        }
        args.Handled = true;
    }

    private void OnDialogClosed(bool accept, bool colorChanged, Color[] colors)
    {
        if (accept)
        {
            if (this.AdvancedMode)
            {
                if (colorChanged)
                {
                    this.mAcceptButton.Enabled = true;
                }
                this.AddUndoRedoStep();
            }
            else
            {
                Responder.Instance.CASModel.RequestFinalizeCommitPresetToPart();
            }
            this.PopulatePresetsGrid(this.GetCurrentTemplatePart(), this.mFilterButton.Selected);
        }
        else if (colorChanged)
        {
            if (this.AdvancedMode)
            {
                UndoRedoData data = this.mUndoRedoQueue[this.mUndoRedoQueueIndex - 0x1];
                this.mTemplateParts = data.TemplateParts;
                this.mTattooLayerScaleData = data.TattooLayerScaleData;
                this.mScaleSlider.Value = (int) (this.GetActivePresetScale() * this.mScaleSlider.MaxValue);
                uint num = (uint) ((data.OpacityColor & 0xff000000) >> 0x18);
                this.mOpacitySlider.Value = (int) ((((float) num) / 255f) * this.mOpacitySlider.MaxValue);
                if (this.mUndoRedoQueueIndex == 0x1)
                {
                    Responder.Instance.CASModel.RequestUndo();
                }
                else
                {
                    this.UpdateModel(false);
                }
                this.UpdateAllParts();
            }
            else
            {
                Responder.Instance.CASModel.RequestUndo();
                Responder.Instance.CASModel.RequestClearRedoOperations();
            }
        }
    }

    private void OnFadeFinished(WindowBase sender, UIHandledEventArgs args)
    {
        if (!base.Visible)
        {
            Unload();
        }
        else
        {
            this.mFadeEffect.Duration = this.mFadeTime;
            if (CASCharacterSheet.gSingleton != null)
            {
                Button randomizeFaceButton = CASCharacterSheet.gSingleton.RandomizeFaceButton;
                if (randomizeFaceButton != null)
                {
                    randomizeFaceButton.Visible = true;
                }
            }
        }
    }

    private void OnFilterClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
    {
        this.mFilter = eventArgs.ButtonSelected;
        this.PopulateTattooGrid(false);
    }

    private void OnGlideFinished(WindowBase sender, UIHandledEventArgs args)
    {
        if (args.SourceWindow.CustomControlID == base.CustomControlID)
        {
            base.EffectFinished -= new UIEventHandler<UIHandledEventArgs>(this.OnGlideFinished);
            CASPuck.Instance.ShowSkewer(true, false);
            if (CASCharacterSheet.gSingleton != null)
            {
                CASCharacterSheet.gSingleton.Visible = true;
            }
            if (CASTattooSheet.gSingleton != null)
            {
                CASTattooSheet.gSingleton.Visible = true;
            }
        }
    }

    private void OnLayerMouseDown(WindowBase sender, UIMouseEventArgs eventArgs)
    {
        if (eventArgs.MouseKey == MouseKeys.kMouseLeft)
        {
            this.mClickedWin = sender;
            this.mMouseClickPos = sender.WindowToScreen(eventArgs.MousePosition);
            this.ActiveLayer = ((int) sender.ID) - 0x92fa021;
            this.mPickupScript = Simulator.AddObject(new OneShotFunctionTask(new Function(this.StartDrag), StopWatch.TickStyles.Seconds, kPickupDelay));
            UIManager.SetCaptureTarget(InputContext.kICMouse, sender);
            eventArgs.Handled = true;
        }
    }

    private void OnLayerMouseMove(WindowBase sender, UIMouseEventArgs eventArgs)
    {
        if ((this.mMouseClickPos != this.kInvalidMousePos) && (this.mClickedWin != null))
        {
            Vector2 vector = sender.WindowToScreen(eventArgs.MousePosition) - this.mMouseClickPos;
            if (vector.LengthSqr() > 50f)
            {
                this.StartDrag();
            }
        }
    }

    private void OnLayerMouseUp(WindowBase sender, UIMouseEventArgs eventArgs)
    {
        if (eventArgs.MouseKey == MouseKeys.kMouseLeft)
        {
            UIManager.ReleaseCapture(InputContext.kICMouse, sender);
            this.ClearClickEvent();
        }
    }

    private void OnLayerRemove(WindowBase sender, UIButtonClickEventArgs eventArgs)
    {
        this.CurrentPreset.mPresetId = 0x0;
        this.SelectItem(this.mTemplateParts[this.ActiveLayer].mTemplatePart, null);
        this.SetPart(this.kInvalidCASPart, this.ActiveLayer);
        this.mTattooGrid.SelectedItem = 0xffffffff;
        this.UpdateLayerEditButtons();
        eventArgs.Handled = true;
    }

    private void OnMarkDragDrop(WindowBase sender, UIDragEventArgs args)
    {
        if (args.Data is uint)
        {
            uint index = (uint) args.Data;
            uint num2 = sender.ID - 0x92fa021;
            if (index != num2)
            {
                TattooTemplateData item = this.mTemplateParts[index];
                List<TattooTemplateData> list = new List<TattooTemplateData>(this.mTemplateParts);
                list.RemoveAt((int) index);
                list.Insert((int) num2, item);
                this.mTemplateParts = list.ToArray();
                this.UpdateAllParts();
                this.UpdateModel(false);
                this.ActiveLayer = 0x0;
                this.mAcceptButton.Enabled = true;
                this.AddUndoRedoStep();
            }
        }
    }

    private void OnMarkDragOver(WindowBase sender, UIDragEventArgs args)
    {
        if (args.Data is uint)
        {
            uint data = (uint) args.Data;
            uint num2 = sender.ID - 0x92fa021;
            args.Result = data != num2;
        }
    }

    private void OnNavButtonClicked(WindowBase sender, UIButtonClickEventArgs args)
    {
        Simulator.AddObject(new OneShotFunctionTask(new Function(this.OnNavButtonClickedHelper)));
        args.Handled = true;
    }

    private void OnNavButtonClickedHelper()
    {
        this.ActiveLayer = 0x0;
        if (this.mNavButtons[ControlIDs.LegGroupButton].Selected)
        {
            this.LeftRightEnabled = true;
            this.mArmPanel.Visible = false;
            this.mChestPanel.Visible = false;
            this.mBackPanel.Visible = false;
            this.mLegPanel.Visible = true;
            this.mFacePanel.Visible = false;
            this.mBodyPanel.Visible = false;
            if (this.mNavButtons[ControlIDs.AnkleButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooAnkleLeft, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooNone | TattooID.TattooAnkleRight, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.ThighFrontButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooThighFrontL, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooThighFrontR, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.ThighBackButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooThighBackL, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooThighBackR, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.CalfFrontButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooCalfFrontL, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooCalfFrontR, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.CalfBackButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooCalfBackL, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooCalfBackR, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.AnkleOuterButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooAnkleOuterL, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooAnkleOuterR, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.FootButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooFootLeft, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooFootRight, false);
                }
            }
        }
        else if (this.mNavButtons[ControlIDs.ArmGroupButton].Selected)
        {
            this.LeftRightEnabled = true;
            this.mArmPanel.Visible = true;
            this.mChestPanel.Visible = false;
            this.mBackPanel.Visible = false;
            this.mLegPanel.Visible = false;
            this.mFacePanel.Visible = false;
            this.mBodyPanel.Visible = false;
            if (this.mNavButtons[ControlIDs.WristButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooWristTopLeft, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooWristTopRight, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.ForearmButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooForearmLeft, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooForearmRight, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.BicepButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooBicepLeft, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooBicepRight, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.ShoulderButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooNone | TattooID.TattooShoulderLeft, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooNone | TattooID.TattooShoulderRight, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.HandButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooHandLeft, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooHandRight, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.PalmButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooPalmLeft, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooPalmRight, false);
                }
            }
        }
        else if (this.mNavButtons[ControlIDs.ChestGroupButton].Selected)
        {
            this.LeftRightEnabled = true;
            this.mArmPanel.Visible = false;
            this.mChestPanel.Visible = true;
            this.mBackPanel.Visible = false;
            this.mLegPanel.Visible = false;
            this.mFacePanel.Visible = false;
            this.mBodyPanel.Visible = false;
            if (this.mNavButtons[ControlIDs.ChestButton].Selected)
            {
                this.SetActiveTattooType(TattooID.TattooNone | TattooID.TattooChest, false);
            }
            else if (this.mNavButtons[ControlIDs.BellyButton].Selected)
            {
                this.SetActiveTattooType(TattooID.TattooNone | TattooID.TattooBellybutton, false);
            }
            else if (this.mNavButtons[ControlIDs.ThroatButton].Selected)
            {
                this.SetActiveTattooType(TattooID.TattooThroat, false);
            }
            else if (this.mNavButtons[ControlIDs.RibsButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooRibsLeft, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooRibsRight, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.UpperBreastButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooBreastUpperL, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooBreastUpperR, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.HipButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooHipLeft, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooHipRight, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.LowerBellyButton].Selected)
            {
                this.SetActiveTattooType(TattooID.TattooLowerBelly, false);
            }
        }
        else if (this.mNavButtons[ControlIDs.BackGroupButton].Selected)
        {
            this.LeftRightEnabled = true;
            this.mArmPanel.Visible = false;
            this.mChestPanel.Visible = false;
            this.mBackPanel.Visible = true;
            this.mLegPanel.Visible = false;
            this.mFacePanel.Visible = false;
            this.mBodyPanel.Visible = false;
            if (this.mNavButtons[ControlIDs.UpperBackButton].Selected)
            {
                this.SetActiveTattooType(TattooID.TattooUpperBack, false);
            }
            else if (this.mNavButtons[ControlIDs.LowerBackButton].Selected)
            {
                this.SetActiveTattooType(TattooID.TattooLowerBack, false);
            }
            else if (this.mNavButtons[ControlIDs.FullBackButton].Selected)
            {
                this.SetActiveTattooType(TattooID.TattooNone | TattooID.TattooFullBack, false);
            }
            else if (this.mNavButtons[ControlIDs.NeckButton].Selected)
            {
                this.SetActiveTattooType(TattooID.TattooNeck, false);
            }
            else if (this.mNavButtons[ControlIDs.ShoulderBackButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooNone | TattooID.TattooShoulderBackL, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooNone | TattooID.TattooShoulderBackR, false);
                }
            }
            else if (this.mNavButtons[ControlIDs.LowerLowerBack].Selected)
            {
                this.SetActiveTattooType(TattooID.TattooLowerLowerBack, false);
            }
            else if (this.mNavButtons[ControlIDs.ButtCheekButton].Selected)
            {
                if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooNone | TattooID.TattooButtLeft, false);
                }
                else
                {
                    this.SetActiveTattooType(TattooID.TattooNone | TattooID.TattooButtRight, false);
                }
            }
        }
        else
        {
            if (this.mNavButtons[ControlIDs.BodyGroupButton].Selected)
            {
                this.LeftRightEnabled = true;
                this.mArmPanel.Visible = false;
                this.mChestPanel.Visible = false;
                this.mBackPanel.Visible = false;
                this.mLegPanel.Visible = false;
                this.mFacePanel.Visible = false;
                if (kNaughtyBits == 0x0)
                {
                    this.mBodyPanel.Visible = false;
                    this.SetActiveTattooType(TattooID.TattooNone | TattooID.TattooFullBody, false);
                }
                else
                {
                    this.mBodyPanel.Visible = true;
                    if (this.mNavButtons[ControlIDs.FullBodyButton].Selected)
                    {
                        this.SetActiveTattooType(TattooID.TattooNone | TattooID.TattooFullBody, false);
                    }
                    else if (this.mNavButtons[ControlIDs.NippleButton].Selected)
                    {
                        if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                        {
                            this.SetActiveTattooType(TattooID.TattooNippleL, false);
                        }
                        else
                        {
                            this.SetActiveTattooType(TattooID.TattooNippleR, false);
                        }
                    }
                    else if (this.mNavButtons[ControlIDs.PubicButton].Selected)
                    {
                        this.SetActiveTattooType(TattooID.TattooPubic, false);
                    }
                    goto Label_0C0D;
                }
            }
            if (this.mNavButtons[ControlIDs.FaceGroupButton].Selected)
            {
                this.LeftRightEnabled = true;
                this.mArmPanel.Visible = false;
                this.mChestPanel.Visible = false;
                this.mBackPanel.Visible = false;
                this.mLegPanel.Visible = false;
                this.mFacePanel.Visible = true;
                this.mBodyPanel.Visible = false;
                if (this.mNavButtons[ControlIDs.FullFaceButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooFullFace, false);
                }
                else if (this.mNavButtons[ControlIDs.CheekButton].Selected)
                {
                    if (this.mNavButtons[ControlIDs.LeftButton].Selected)
                    {
                        this.SetActiveTattooType(TattooID.TattooCheekLeft, false);
                    }
                    else
                    {
                        this.SetActiveTattooType(TattooID.TattooCheekRight, false);
                    }
                }
                else if (this.mNavButtons[ControlIDs.ForeheadButton].Selected)
                {
                    this.SetActiveTattooType(TattooID.TattooForehead, false);
                }
            }
        }
    Label_0C0D:
        this.SaveState();
    }

    private void OnOpacitySliderChanged(WindowBase sender, UIValueChangedEventArgs eventArgs)
    {
        if (this.ActiveTattooPartInUse())
        {
            this.UpdateModel(false);
        }
    }

    private void OnOpacitySliderMouseUp(WindowBase sender, UIMouseEventArgs args)
    {
        if (this.ActiveTattooPartInUse())
        {
            if (this.AdvancedMode)
            {
                this.mAcceptButton.Enabled = true;
                this.UpdateModel(false);
                this.AddUndoRedoStep();
            }
            else
            {
                this.UpdateModel(true);
            }
        }
    }

    private void OnPartsGridSelect(ItemGrid sender, ItemGridCellClickEvent args)
    {
        this.mSaveButton.Enabled = false;
        if (args.mTag is CASPart)
        {
            sender.RemoveTempItem();
            CASPart mTag = (CASPart) args.mTag;
            if (mTag.Key == this.kInvalidCASPart.Key)
            {
                this.ClearAllParts();
                this.ClearTemplates();
                this.CurrentPreset.mPresetId = 0x0;
                this.mDeleteButton.Enabled = false;
                this.mShareButton.Enabled = false;
                this.mTattooPresetsGrid.SelectedItem = 0xffffffff;
                this.mTattooPresetsGrid.Clear();
                if (!this.AdvancedMode)
                {
                    List<CASPart> wornParts = Responder.Instance.CASModel.GetWornParts(BodyTypes.Tattoo);
                    if (((wornParts == null) || (wornParts.Count == 0x0)) || ((wornParts.Count == 0x1) && (wornParts[0x0].Key == this.mActiveTattooPart.Key)))
                    {
                        this.mRemoveAllButton.Enabled = false;
                    }
                    this.CurrentPreset.mPresetString = CASUtils.PartDataGetPreset(this.mActiveTattooPart.Key, 0x0);
                    Responder.Instance.CASModel.RequestRemoveCASPart(this.mActiveTattooPart);
                }
                else
                {
                    this.ActiveLayer = 0x0;
                    this.SelectItem(this.mTemplateParts[this.ActiveLayer].mTemplatePart, null);
                }
                Audio.StartSound("ui_tertiary_button");
            }
            else if (args.mWin.Enabled)
            {
                CASPartPreset preset = new CASPartPreset(mTag, CASUtils.PartDataGetPresetId(mTag.Key, 0x0), CASUtils.PartDataGetPreset(mTag.Key, 0x0));
                this.CurrentPreset.mPresetId = 0x0;
                this.mScaleSlider.Value = (this.mScaleSlider.MaxValue - this.mScaleSlider.MinValue) * this.mTunedScales[this.mActiveTattooID];
                this.UpdateScaleFromSlider();
                if (!this.AdvancedMode)
                {
                    this.ClearTemplates();
                    this.ClearAllParts();
                    this.mOpacitySlider.Value = (this.mOpacitySlider.MaxValue + this.mOpacitySlider.MinValue) / 0x2;
                }
                this.SetPart(mTag, this.ActiveLayer);
                this.SelectItem(mTag, preset);
                CustomContentIcon childByID = args.mWin.GetChildByID(0x23, true) as CustomContentIcon;
                if (childByID != null)
                {
                    this.mDeleteButton.Enabled = childByID.Localuser;
                    this.mShareButton.Enabled = this.mDeleteButton.Enabled;
                }
                this.PopulatePresetsGrid(mTag, this.mFilter);
            }
            else
            {
                for (int i = 0x0; i < 0x5L; i++)
                {
                    if (this.mLayerButtons[i].Tag is CASPart)
                    {
                        CASPart tag = (CASPart) this.mLayerButtons[i].Tag;
                        if (tag.Key == mTag.Key)
                        {
                            this.ActiveLayer = i;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            CASPartPreset preset2 = args.mTag as CASPartPreset;
            if (preset2 != null)
            {
                ICASModel cASModel = Responder.Instance.CASModel;
                this.ClearTemplates();
                this.ClearAllParts();
                this.UpdateCurrentDataFromCompositePreset(preset2);
                this.UpdateModel(!this.AdvancedMode);
                this.mTattooPresetsGrid.SelectedItem = 0xffffffff;
                this.mTattooPresetsGrid.Clear();
                this.mSaveButton.Enabled = false;
                CustomContentIcon icon2 = args.mWin.GetChildByID(0x23, true) as CustomContentIcon;
                if ((icon2 != null) && !this.AdvancedMode)
                {
                    this.mDeleteButton.Enabled = icon2.Localuser;
                    this.mShareButton.Enabled = this.mDeleteButton.Enabled;
                }
                if (this.AdvancedMode)
                {
                    this.ActiveLayer = 0x0;
                    this.AddUndoRedoStep();
                }
                if (!this.AdvancedMode)
                {
                    this.mRemoveAllButton.Enabled = true;
                }
                Audio.StartSound("ui_tertiary_button");
            }
        }
        this.UpdateLayerEditButtons();
    }

    private void OnPresetGridSelect(ItemGrid sender, ItemGridCellClickEvent args)
    {
        CASPartPreset mTag = args.mTag as CASPartPreset;
        if (mTag != null)
        {
            this.SelectItem(this.GetCurrentTemplatePart(), mTag);
            CustomContentIcon childByID = args.mWin.GetChildByID(0x23, true) as CustomContentIcon;
            if (childByID != null)
            {
                this.mDeleteButton.Enabled = childByID.Localuser;
            }
            Audio.StartSound("ui_tertiary_button");
        }
    }

    private void OnRedo()
    {
        this.SetActiveTattooType(this.mActiveTattooID, true);
        this.UpdateRemoveAllButton();
    }

    private void OnRedoClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
    {
        UndoRedoData data = this.mUndoRedoQueue[this.mUndoRedoQueueIndex++];
        this.mTemplateParts = data.TemplateParts;
        this.mTattooLayerScaleData = data.TattooLayerScaleData;
        this.mScaleSlider.Value = (int) (this.GetActivePresetScale() * this.mScaleSlider.MaxValue);
        uint num = (uint) ((data.OpacityColor & 0xff000000) >> 0x18);
        this.mOpacitySlider.Value = (int) ((((float) num) / 255f) * this.mOpacitySlider.MaxValue);
        if (this.mUndoRedoQueueIndex == this.mUndoRedoQueue.Count)
        {
            this.mRedoButton.Enabled = false;
        }
        this.UpdateModel(false);
        this.UpdateAllParts();
        this.ActiveLayer = this.ActiveLayer;
        this.mUndoButton.Enabled = true;
        if (eventArgs != null)
        {
            eventArgs.Handled = true;
        }
    }

    private void OnRemoveAllClick(WindowBase sender, UIButtonClickEventArgs args)
    {
        List<CASPart> wornParts = Responder.Instance.CASModel.GetWornParts(BodyTypes.Tattoo);
        if (wornParts.Count > 0x0)
        {
            CASPart[] parts = new CASPart[wornParts.Count];
            int index = 0x0;
            foreach (CASPart part in wornParts)
            {
                parts[index] = part;
                index++;
            }
            Responder.Instance.CASModel.RequestRemoveCASParts(parts);
        }
        this.ClearAllParts();
        this.ClearTemplates();
        this.CurrentPreset.mPresetId = 0x0;
        this.mTattooGrid.SelectedItem = 0x0;
        this.mSaveButton.Enabled = false;
        this.mDeleteButton.Enabled = false;
        this.mAdvancedModeButton.Enabled = false;
        this.mShareButton.Enabled = false;
        this.mColorPickerButton.Enabled = false;
        this.mRemoveAllButton.Enabled = false;
        this.mTattooPresetsGrid.SelectedItem = 0xffffffff;
        this.mTattooPresetsGrid.Clear();
    }

    private void OnSaveClick(WindowBase sender, UIButtonClickEventArgs args)
    {
        args.Handled = true;
        bool flag = false;
        CASPart defaultPart = this.GetDefaultPart();
        if (defaultPart.Key != this.kInvalidCASPart.Key)
        {
            ObjectDesigner.SetCASPart(defaultPart.Key);
            string[] strArray = new string[0x5];
            int index = 0x0;
            foreach (TattooTemplateData data in this.mTemplateParts)
            {
                if ((data.mPreset != null) && !(data.mTemplatePart.Key == ResourceKey.kInvalidResourceKey))
                {
                    strArray[index] = data.mPreset.mPresetString;
                    index++;
                }
            }
            float[] originalReferenceDims = CASUtils.ExtractTattooDimensions(this.CurrentPreset.mPresetString, true);
            float[] dimensions = this.TranslateTattooScale(this.mTattooLayerScaleData, originalReferenceDims, this.kStorageReferenceDimensions);
            flag = ObjectDesigner.AddDesignPreset(CASUtils.InjectTattooDimensions(this.CurrentPreset.mPresetString, dimensions)) != uint.MaxValue;
            this.mFilterButton.Enabled = true;
        }
        if (flag)
        {
            this.PopulateTattooGrid(false);
            CASController.Singleton.ErrorMsg(CASErrorCode.SaveSuccess);
        }
        else
        {
            CASController.Singleton.ErrorMsg(CASErrorCode.SaveFailed);
        }
    }

    private void OnScaleSliderChanged(WindowBase sender, UIValueChangedEventArgs eventArgs)
    {
        this.UpdateScaleFromSlider();
        this.UpdateModel(false);
    }

    private void OnScaleSliderMouseUp(WindowBase sender, UIMouseEventArgs args)
    {
        this.UpdateScaleFromSlider();
        if (this.AdvancedMode)
        {
            this.mAcceptButton.Enabled = true;
            this.UpdateModel(false);
            this.AddUndoRedoStep();
        }
        else
        {
            this.UpdateModel(true);
        }
    }

    public void OnSceneWindowCameraMouseDown(WindowBase sender, UIMouseEventArgs eventArgs)
    {
        this.CancelSimRotation();
    }

    private void OnShareClick(WindowBase sender, UIButtonClickEventArgs args)
    {
        Simulator.AddObject(new OneShotFunctionTask(new Function(this.SharePresetDialogTask)));
        args.Handled = true;
    }

    private void OnTick(WindowBase sender, UIEventArgs args)
    {
        if (this.mbTargetSimAngleActive)
        {
            float elapsedTimeFloat = this.mTargetSimAngleStopWatch.GetElapsedTimeFloat();
            this.mTargetSimAngleStopWatch.Restart();
            elapsedTimeFloat = Math.Min(elapsedTimeFloat, 0.2f);
            float simRotationAngle = CASController.Singleton.SimRotationAngle;
            float num3 = this.mTargetSimAngle - simRotationAngle;
            if (num3 > 3.141593f)
            {
                num3 -= 6.283185f;
            }
            if (num3 < -3.141593f)
            {
                num3 += 6.283185f;
            }
            float num4 = Math.Abs(num3);
            float num5 = elapsedTimeFloat * 4.712389f;
            if (num4 <= num5)
            {
                CASController.Singleton.SimRotationAngle = this.mTargetSimAngle;
                this.CancelSimRotation();
            }
            else if (num3 >= 0f)
            {
                CASController singleton = CASController.Singleton;
                singleton.SimRotationAngle += num5;
            }
            else
            {
                CASController controller2 = CASController.Singleton;
                controller2.SimRotationAngle -= num5;
            }
        }
    }

    protected void OnTriggerDown(WindowBase sender, UITriggerEventArgs eventArgs)
    {
        switch (eventArgs.TriggerCode)
        {
            case 0x22551000:
                if (this.mUndoButton.Enabled)
                {
                    this.OnUndoClick(null, null);
                    Audio.StartSound("ui_undo");
                }
                eventArgs.Handled = true;
                return;

            case 0x22551001:
                if (this.mRedoButton.Enabled)
                {
                    this.OnRedoClick(null, null);
                    Audio.StartSound("ui_redo");
                }
                eventArgs.Handled = true;
                return;
        }
    }

    private void OnUndo()
    {
        if (this.mRemoveTattooOnUndo)
        {
            this.mRemoveTattooOnUndo = false;
            List<CASPart> wornParts = Responder.Instance.CASModel.GetWornParts(BodyTypes.Tattoo);
            Responder.Instance.CASModel.RequestRemoveCASPart(this.mActiveTattooPart);
            this.mTattooGrid.SelectedItem = 0x0;
            if (((wornParts != null) && (wornParts.Count == 0x1)) && wornParts.Contains(this.mActiveTattooPart))
            {
                this.mRemoveAllButton.Enabled = false;
            }
            this.mAdvancedModeButton.Enabled = false;
            this.mWasCustom = false;
        }
        else
        {
            this.UpdateRemoveAllButton();
            this.SetActiveTattooType(this.mActiveTattooID, true);
        }
    }

    private void OnUndoClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
    {
        UndoRedoData data = this.mUndoRedoQueue[--this.mUndoRedoQueueIndex - 0x1];
        this.mTemplateParts = data.TemplateParts;
        this.mTattooLayerScaleData = data.TattooLayerScaleData;
        this.mScaleSlider.Value = (int) (this.GetActivePresetScale() * this.mScaleSlider.MaxValue);
        uint num = (uint) ((data.OpacityColor & 0xff000000) >> 0x18);
        this.mOpacitySlider.Value = (int) ((((float) num) / 255f) * this.mOpacitySlider.MaxValue);
        if (this.mUndoRedoQueueIndex == 0x1)
        {
            Responder.Instance.CASModel.RequestUndo();
            this.mUndoButton.Enabled = false;
        }
        else
        {
            this.UpdateModel(false);
        }
        this.mRedoButton.Enabled = true;
        this.UpdateAllParts();
        this.ActiveLayer = this.ActiveLayer;
        if (eventArgs != null)
        {
            eventArgs.Handled = true;
        }
    }

    private bool PartInUse(CASPart part)
    {
        foreach (TattooTemplateData data in this.mTemplateParts)
        {
            if (data.mTemplatePart.Key == part.Key)
            {
                return true;
            }
        }
        return false;
    }

    private void PopulatePresetsGrid(CASPart wornPart, bool filter)
    {
        this.mTattooPresetsGrid.Clear();
        if (wornPart.Key != this.kInvalidCASPart.Key)
        {
            ObjectDesigner.SetCASPart(wornPart.Key);
            int num = 0x0;
            uint num2 = CASUtils.PartDataNumPresets(wornPart.Key);
            for (int i = 0x0; i < num2; i++)
            {
                string str = CASUtils.PartDataGetPreset(wornPart.Key, (uint) i);
                uint[] numArray = CASUtils.ExtractTattooColors(str, 0x1);
                Vector3[] colors = new Vector3[numArray.Length];
                num = 0x0;
                foreach (uint num4 in numArray)
                {
                    colors[num++] = CompositorUtil.ColorToVector3(new Color(num4));
                }
                uint id = CASUtils.PartDataGetPresetId(wornPart.Key, (uint) i);
                ResourceKeyContentCategory customContentTypeFromKeyAndPresetIndex = UIUtils.GetCustomContentTypeFromKeyAndPresetIndex(wornPart.Key, (uint) i);
                if (!UIUtils.IsContentTypeDisabled(customContentTypeFromKeyAndPresetIndex))
                {
                    CASPartPreset preset = new CASPartPreset(this.GetCurrentTemplatePart(), id, str);
                    this.AddPresetGridItem(preset, colors, customContentTypeFromKeyAndPresetIndex);
                    num = 0x0;
                    if (this.IsUsingTattooTemplatePreset(wornPart, preset))
                    {
                        if (UIUtils.ExtractCustomContentType(customContentTypeFromKeyAndPresetIndex) == ResourceKeyContentCategory.kLocalUserCreated)
                        {
                            this.mDeleteButton.Enabled = true;
                        }
                        this.mTattooPresetsGrid.SelectedItem = i;
                    }
                }
            }
        }
    }

    private void PopulateTattooGrid(bool updateTemplates)
    {
        this.PopulateTattooGrid(updateTemplates, false);
    }

    private void PopulateTattooGrid(bool updateTemplates, bool forceTemp)
    {
        ItemGrid mTattooGrid = this.mTattooGrid;
        int length = 0x0;
        uint[] numArray = CASUtils.ExtractEnabledLayerIndices(this.CurrentPreset.mPresetString);
        if (numArray != null)
        {
            length = numArray.Length;
        }
        int num2 = 0x0;
        if (mTattooGrid.Count > 0x0)
        {
            num2 = mTattooGrid.GetFirstVisibleItem() + ((int) (mTattooGrid.VisibleColumns * (mTattooGrid.VisibleRows - 0x1)));
        }
        mTattooGrid.SelectedItem = 0xffffffff;
        mTattooGrid.Clear();
        BodyTypes tattooTemplate = BodyTypes.TattooTemplate;
        CASPart[] visibleParts = this.GetVisibleParts(tattooTemplate);
        this.mFilterButton.Enabled = false;
        this.mDeleteButton.Enabled = false;
        this.mShareButton.Enabled = false;
        this.mSaveButton.Enabled = false;
        this.mColorPickerButton.Enabled = false;
        this.mColorPicker2Button.Enabled = false;
        this.mRemoveLayerButton.Enabled = false;
        ResourceKey layoutKey = ResourceKey.CreateUILayoutKey("GenericCasItem", 0x0);
        int num3 = 0x0;
        this.ActiveTattooPartInUse();
        int num4 = 0x0;
        if (updateTemplates)
        {
            this.ClearAllParts();
        }
        foreach (CASPart part in visibleParts)
        {
            ResourceKeyContentCategory customContentType = UIUtils.GetCustomContentType(part.Key);
            ResourceKeyContentCategory category2 = UIUtils.ExtractCustomContentType(customContentType);
            if (this.AddPartsGridItem(this.mTattooGrid, layoutKey, part, customContentType))
            {
                if (this.IsUsingTattooTemplate(part))
                {
                    if (updateTemplates)
                    {
                        this.SetPart(part, this.GetTattooTemplateUsageIndex(part) - 0x1);
                        this.UpdateTemplatePart(part);
                    }
                    this.mTattooGrid.SelectedItem = num3;
                    if (category2 == ResourceKeyContentCategory.kLocalUserCreated)
                    {
                        this.mDeleteButton.Enabled = true;
                        this.mShareButton.Enabled = true;
                    }
                    if (num4 == 0x0)
                    {
                        this.PopulatePresetsGrid(part, this.mFilter);
                    }
                    num4++;
                }
                num3++;
            }
            else if (this.IsUsingTattooTemplate(part))
            {
                if (updateTemplates)
                {
                    this.SetPart(part, this.GetTattooTemplateUsageIndex(part) - 0x1);
                    this.UpdateTemplatePart(part);
                }
                num4++;
            }
        }
        if ((this.mTattooGrid.SelectedItem == 0xffffffff) || (num4 > 0x1))
        {
            if (num4 != 0x1)
            {
                this.mTattooGrid.SelectedItem = 0x0;
            }
            this.mTattooPresetsGrid.SelectedItem = 0xffffffff;
            this.mTattooPresetsGrid.Clear();
        }
        if (num4 != length)
        {
            num4 = 0x0;
            this.mTattooGrid.SelectedItem = 0xffffffff;
            this.mTattooPresetsGrid.SelectedItem = 0xffffffff;
            this.mTattooPresetsGrid.Clear();
        }
        string str = "";
        string[] strArray = new string[0x5];
        int index = 0x0;
        foreach (TattooTemplateData data in this.mTemplateParts)
        {
            if ((data.mPreset != null) && !(data.mTemplatePart.Key == ResourceKey.kInvalidResourceKey))
            {
                strArray[index] = data.mPreset.mPresetString;
                index++;
            }
        }
        float[] originalReferenceDims = CASUtils.ExtractTattooDimensions(this.mCurrentPreset.mPresetString, true);
        float[] dimensions = this.TranslateTattooScale(this.mTattooLayerScaleData, originalReferenceDims, this.kStorageReferenceDimensions);
        str = CASUtils.InjectTattooDimensions(this.CurrentPreset.mPresetString, dimensions);
        CASPart defaultPart = this.GetDefaultPart();
        ObjectDesigner.SetCASPart(defaultPart.Key);
        uint num6 = CASUtils.PartDataNumPresets(defaultPart.Key);
        bool testValid = false;
        if (this.mInvalidIds == null)
        {
            this.mInvalidIds = new List<uint>();
            testValid = true;
        }
        for (int i = 0x1; i < num6; i++)
        {
            uint presetId = CASUtils.PartDataGetPresetId(defaultPart.Key, (uint) i);
            ResourceKeyContentCategory contentType = UIUtils.GetCustomContentType(defaultPart.Key, presetId);
            if (UIUtils.IsCustomFiltered(contentType))
            {
                this.mFilterButton.Enabled = true;
            }
            CASPartPreset preset = new CASPartPreset(defaultPart, presetId, CASUtils.PartDataGetPreset(defaultPart.Key, (uint) i));
            if (preset.Valid)
            {
                if (!this.mFilterButton.Selected || UIUtils.IsCustomFiltered(contentType))
                {
                    if (this.AddCompositePartsGridItem(layoutKey, preset, testValid))
                    {
                        if (CASUtils.CompareCompositeTattooPresets(preset.mPresetString, str) && !this.AdvancedMode)
                        {
                            num4 = 0x1;
                            this.mTattooGrid.SelectedItem = num3;
                            if (ObjectDesigner.IsUserDesignPreset((uint) i))
                            {
                                this.mShareButton.Enabled = true;
                                this.mDeleteButton.Enabled = true;
                            }
                        }
                        num3++;
                    }
                }
                else if (CASUtils.CompareCompositeTattooPresets(preset.mPresetString, this.CurrentPreset.mPresetString))
                {
                    num4 = 0x1;
                }
            }
        }
        if ((((num4 > 0x1) || forceTemp) || this.mWasCustom) && !this.AdvancedMode)
        {
            this.mWasCustom = false;
            WindowBase windowByExportID = UIManager.LoadLayout(layoutKey).GetWindowByExportID(0x1);
            if (windowByExportID != null)
            {
                Window childByID = windowByExportID.GetChildByID(0x20, true) as Window;
                if (childByID != null)
                {
                    childByID.Visible = false;
                }
                childByID = windowByExportID.GetChildByID(0x24, true) as Window;
                if (childByID != null)
                {
                    childByID.Visible = true;
                }
                windowByExportID.IgnoreMouse = false;
                this.mTattooGrid.AddTempItem(new ItemGridCellItem(windowByExportID, null));
            }
            this.mSaveButton.Enabled = true;
        }
        int count = mTattooGrid.Count;
        if (mTattooGrid.HasTempItem)
        {
            count++;
        }
        if (num2 >= count)
        {
            num2 = count - 0x1;
        }
        if (num2 > (mTattooGrid.VisibleRows * mTattooGrid.VisibleColumns))
        {
            int selectedItem = mTattooGrid.SelectedItem;
            mTattooGrid.SelectedItem = num2;
            mTattooGrid.ShowSelectedItem(true);
            mTattooGrid.SelectedItem = selectedItem;
        }
        this.UpdateLayerEditButtons();
    }

    private void PopulateTunedScales()
    {
        this.mTunedScales.Add(TattooID.TattooAnkleLeft, kDefaultAnkleLeftScales);
        this.mTunedScales.Add(TattooID.TattooNone | TattooID.TattooAnkleRight, kDefaultAnkleRightScales);
        this.mTunedScales.Add(TattooID.TattooNone | TattooID.TattooBellybutton, kDefaultBellybuttonScales);
        this.mTunedScales.Add(TattooID.TattooBicepLeft, kDefaultBicepLeftScales);
        this.mTunedScales.Add(TattooID.TattooBicepRight, kDefaultBicepRightScales);
        this.mTunedScales.Add(TattooID.TattooNone | TattooID.TattooChest, kDefaultChestScales);
        this.mTunedScales.Add(TattooID.TattooForearmLeft, kDefaultForearmLeftScales);
        this.mTunedScales.Add(TattooID.TattooForearmRight, kDefaultForearmRightScales);
        this.mTunedScales.Add(TattooID.TattooNone | TattooID.TattooFullBack, kDefaultFullBackScales);
        this.mTunedScales.Add(TattooID.TattooLowerBack, kDefaultLowerBackScales);
        this.mTunedScales.Add(TattooID.TattooNeck, kDefaultNeckScales);
        this.mTunedScales.Add(TattooID.TattooNone | TattooID.TattooShoulderLeft, kDefaultShoulderLeftScales);
        this.mTunedScales.Add(TattooID.TattooNone | TattooID.TattooShoulderRight, kDefaultShoulderRightScales);
        this.mTunedScales.Add(TattooID.TattooUpperBack, kDefaultUpperBackScales);
        this.mTunedScales.Add(TattooID.TattooWristTopLeft, kDefaultWristLeftScales);
        this.mTunedScales.Add(TattooID.TattooWristTopRight, kDefaultWristRightScales);
        this.mTunedScales.Add(TattooID.TattooNone | TattooID.TattooFullBody, kDefaultFullBodyScales);
        this.mTunedScales.Add(TattooID.TattooFullFace, kDefaultFullFaceScales);
        this.mTunedScales.Add(TattooID.TattooNone | TattooID.TattooShoulderBackL, kDefaultShoulderBackLScales);
        this.mTunedScales.Add(TattooID.TattooNone | TattooID.TattooShoulderBackR, kDefaultShoulderBackRScales);
        this.mTunedScales.Add(TattooID.TattooLowerLowerBack, kDefaultLowerLowBackScales);
        this.mTunedScales.Add(TattooID.TattooNone | TattooID.TattooButtLeft, kDefaultButtLeftScales);
        this.mTunedScales.Add(TattooID.TattooNone | TattooID.TattooButtRight, kDefaultButtRightScales);
        this.mTunedScales.Add(TattooID.TattooHandLeft, kDefaultHandLeftScales);
        this.mTunedScales.Add(TattooID.TattooHandRight, kDefaultHandRightScales);
        this.mTunedScales.Add(TattooID.TattooPalmLeft, kDefaultPalmLeftScales);
        this.mTunedScales.Add(TattooID.TattooPalmRight, kDefaultPalmRightScales);
        this.mTunedScales.Add(TattooID.TattooThroat, kDefaultThroatScales);
        this.mTunedScales.Add(TattooID.TattooRibsLeft, kDefaultRibsLeftScales);
        this.mTunedScales.Add(TattooID.TattooRibsRight, kDefaultRibsRightScales);
        this.mTunedScales.Add(TattooID.TattooBreastUpperL, kDefaultBreastUpperLScales);
        this.mTunedScales.Add(TattooID.TattooBreastUpperR, kDefaultBreastUpperRScales);
        this.mTunedScales.Add(TattooID.TattooHipLeft, kDefaultHipLeftScales);
        this.mTunedScales.Add(TattooID.TattooHipRight, kDefaultHipRightScales);
        this.mTunedScales.Add(TattooID.TattooThighFrontL, kDefaultThighFrontLScales);
        this.mTunedScales.Add(TattooID.TattooThighFrontR, kDefaultThighFrontRScales);
        this.mTunedScales.Add(TattooID.TattooThighBackL, kDefaultThighBackLScales);
        this.mTunedScales.Add(TattooID.TattooThighBackR, kDefaultThighBackRScales);
        this.mTunedScales.Add(TattooID.TattooCalfFrontL, kDefaultCalfFrontLScales);
        this.mTunedScales.Add(TattooID.TattooCalfFrontR, kDefaultCalfFrontRScales);
        this.mTunedScales.Add(TattooID.TattooCalfBackL, kDefaultCalfBackLScales);
        this.mTunedScales.Add(TattooID.TattooCalfBackR, kDefaultCalfBackRScales);
        this.mTunedScales.Add(TattooID.TattooAnkleOuterL, kDefaultAnkleOuterLScales);
        this.mTunedScales.Add(TattooID.TattooAnkleOuterR, kDefaultAnkleOuterRScales);
        this.mTunedScales.Add(TattooID.TattooFootLeft, kDefaultFootLeftScales);
        this.mTunedScales.Add(TattooID.TattooFootRight, kDefaultFootRightScales);
        this.mTunedScales.Add(TattooID.TattooCheekLeft, kDefaultCheekLeftScales);
        this.mTunedScales.Add(TattooID.TattooCheekRight, kDefaultCheekRightScales);
        this.mTunedScales.Add(TattooID.TattooForehead, kDefaultForeheadScales);
        this.mTunedScales.Add(TattooID.TattooLowerBelly, kDefaultLowerBellyScales);
        this.mTunedScales.Add(TattooID.TattooNippleL, kDefaultNippleLeftScales);
        this.mTunedScales.Add(TattooID.TattooNippleR, kDefaultNippleRightScales);
        this.mTunedScales.Add(TattooID.TattooPubic, kDefaultPubicScales);
    }

    internal static void ResetDefaults()
    {
        sNavButtonStates = new Dictionary<ControlIDs, bool>();
    }

    private void RestoreState()
    {
        if (sNavButtonStates.Keys.Count > 0x0)
        {
            foreach (KeyValuePair<ControlIDs, Button> pair in this.mNavButtons)
            {
                pair.Value.Selected = false;
            }
            foreach (KeyValuePair<ControlIDs, bool> pair2 in sNavButtonStates)
            {
                this.mNavButtons[pair2.Key].Selected = pair2.Value;
            }
        }
    }

    private void SaveState()
    {
        foreach (KeyValuePair<ControlIDs, Button> pair in this.mNavButtons)
        {
            if (sNavButtonStates.ContainsKey(pair.Key))
            {
                sNavButtonStates[pair.Key] = pair.Value.Selected;
            }
            else
            {
                sNavButtonStates.Add(pair.Key, pair.Value.Selected);
            }
        }
    }

    private void SelectItem(CASPart part, CASPartPreset preset)
    {
        this.mAcceptButton.Enabled = true;
        this.SelectItem(part, preset, !this.AdvancedMode);
        if (this.AdvancedMode)
        {
            this.AddUndoRedoStep();
        }
    }

    private void SelectItem(CASPart part, CASPartPreset preset, bool commit)
    {
        ICASModel cASModel = Responder.Instance.CASModel;
        if (!this.PartInUse(part) && (preset != null))
        {
            if (!this.AdvancedMode)
            {
                this.mRemoveAllButton.Enabled = true;
            }
            this.AddTemplatePart(part, preset);
        }
        else
        {
            this.UpdateTemplatePreset(part, preset);
        }
        this.UpdateModel(commit);
        Audio.StartSound("ui_tertiary_button");
    }

    private void SetActiveTattooType(TattooID tattooID, bool forceRefresh)
    {
        if ((this.mActiveTattooID != tattooID) || forceRefresh)
        {
            this.mActiveTattooID = tattooID;
            bool flag = false;
            foreach (CASPart part in this.mTattooParts)
            {
                if (part.Key.InstanceId == this.mActiveTattooID)
                {
                    this.mActiveTattooPart = part;
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                this.SetTattooCam(this.mActiveTattooID);
                this.UpdateCurrentPreset();
                this.ClearTemplates();
                this.PopulateTattooGrid(true);
            }
        }
    }

    private void SetColors(Color[] colors)
    {
        uint[] numArray = new uint[colors.Length];
        int num = 0x0;
        foreach (Color color in colors)
        {
            numArray[num++] = color.ARGB;
        }
        string presetStr = CASUtils.InjectTattooColors(this.GetCurrentTemplatePresetString(), numArray, 0x1);
        CASPartPreset preset = new CASPartPreset(this.GetCurrentTemplatePart(), presetStr);
        this.SelectItem(this.GetCurrentTemplatePart(), preset, false);
    }

    private void SetPart(CASPart part, int index)
    {
        ImageDrawable drawable = this.mLayerThumbs[index].Drawable as ImageDrawable;
        if (drawable != null)
        {
            if (part.Key != this.kInvalidCASPart.Key)
            {
                this.mLayerButtons[index].Tag = part;
                ThumbnailKey key = new ThumbnailKey(part.Key, (int) CASUtils.PartDataGetPresetId(part.Key, 0x0), (uint) part.BodyType, (uint) part.AgeGenderSpecies, ThumbnailSize.Medium);
                drawable.Image = UIManager.GetCASThumbnailImage(key);
            }
            else
            {
                drawable.Image = null;
                this.mLayerButtons[index].Tag = null;
            }
            this.mLayerThumbs[index].Invalidate();
        }
    }

    public void SetState(CASState state)
    {
        if (state.mPhysicalState == CASPhysicalState.Tattoos)
        {
            CASController.Singleton.mCurrUINode = this;
            Responder.Instance.CASModel.UndoSelected += new UndoRedoSelected(this.OnUndo);
            Responder.Instance.CASModel.RedoSelected += new UndoRedoSelected(this.OnRedo);
        }
        else
        {
            Responder.Instance.CASModel.UndoSelected -= new UndoRedoSelected(this.OnUndo);
            Responder.Instance.CASModel.RedoSelected -= new UndoRedoSelected(this.OnRedo);
            bool flag = true;
            switch (state.mPhysicalState)
            {
                case CASPhysicalState.HeadAndEars:
                case CASPhysicalState.Eyes:
                case CASPhysicalState.Mouth:
                case CASPhysicalState.Nose:
                case CASPhysicalState.Jaw:
                case CASPhysicalState.Moles:
                case CASPhysicalState.Makeup:
                case CASPhysicalState.Tattoos:
                    flag = false;
                    break;
            }
            if (((this.mFadeEffect == null) || !flag) || !base.Visible)
            {
                if (this.UINodeShutdown != null)
                {
                    this.UINodeShutdown(state);
                }
                Unload();
            }
            else
            {
                this.mTargetState = state;
                base.StartOuttro();
            }
        }
    }

    private void SetTattooCam(TattooID tattooId)
    {
        TattooID oid = tattooId;
        if (oid != (TattooID.TattooNone | TattooID.TattooFullBack))
        {
            if (oid == (TattooID.TattooNone | TattooID.TattooChest))
            {
                goto Label_0426;
            }
            if (oid == (TattooID.TattooNone | TattooID.TattooShoulderLeft))
            {
                goto Label_03C6;
            }
            if (oid == (TattooID.TattooNone | TattooID.TattooShoulderRight))
            {
                goto Label_03DE;
            }
            if (oid == TattooID.TattooAnkleLeft)
            {
                CASController.Singleton.SetAnkleCam(true);
                this.TryRotateSimTowards(-1.256637f);
                return;
            }
            if (oid == (TattooID.TattooNone | TattooID.TattooAnkleRight))
            {
                CASController.Singleton.SetAnkleCam(true);
                this.TryRotateSimTowards(1.256637f);
                return;
            }
            if (oid == (TattooID.TattooNone | TattooID.TattooBellybutton))
            {
                goto Label_0426;
            }
            if (oid == TattooID.TattooWristTopLeft)
            {
                goto Label_0366;
            }
            if (oid == TattooID.TattooWristTopRight)
            {
                goto Label_037E;
            }
            if (oid != TattooID.TattooUpperBack)
            {
                if (oid == TattooID.TattooBicepLeft)
                {
                    goto Label_03C6;
                }
                if (oid == TattooID.TattooBicepRight)
                {
                    goto Label_03DE;
                }
                if (oid == TattooID.TattooForearmLeft)
                {
                    goto Label_03C6;
                }
                if (oid == TattooID.TattooForearmRight)
                {
                    goto Label_03DE;
                }
                if (oid != TattooID.TattooLowerBack)
                {
                    if (oid == TattooID.TattooNeck)
                    {
                        CASController.Singleton.SetFaceCam(true);
                        this.TryRotateSimTowards(3.141593f);
                        return;
                    }
                    if (oid == (TattooID.TattooNone | TattooID.TattooFullBody))
                    {
                        goto Label_0426;
                    }
                    if (oid == TattooID.TattooFullFace)
                    {
                        goto Label_0456;
                    }
                    if ((((oid != (TattooID.TattooNone | TattooID.TattooShoulderBackL)) && (oid != (TattooID.TattooNone | TattooID.TattooShoulderBackR))) && ((oid != TattooID.TattooLowerLowerBack) && (oid != (TattooID.TattooNone | TattooID.TattooButtLeft)))) && (oid != (TattooID.TattooNone | TattooID.TattooButtRight)))
                    {
                        if (oid == TattooID.TattooHandLeft)
                        {
                            goto Label_0366;
                        }
                        if (oid == TattooID.TattooHandRight)
                        {
                            goto Label_037E;
                        }
                        if (oid == TattooID.TattooPalmLeft)
                        {
                            CASController.Singleton.SetHandCam(true);
                            this.TryRotateSimTowards(2.55f);
                            return;
                        }
                        if (oid == TattooID.TattooPalmRight)
                        {
                            CASController.Singleton.SetHandCam(true);
                            this.TryRotateSimTowards(-2.55f);
                            return;
                        }
                        if (oid == TattooID.TattooThroat)
                        {
                            goto Label_0456;
                        }
                        if (oid == TattooID.TattooRibsLeft)
                        {
                            CASController.Singleton.SetTopCam(true);
                            this.TryRotateSimTowards(-0.8f);
                            return;
                        }
                        if (oid == TattooID.TattooRibsRight)
                        {
                            CASController.Singleton.SetTopCam(true);
                            this.TryRotateSimTowards(0.8f);
                            return;
                        }
                        if ((((oid == TattooID.TattooBreastUpperL) || (oid == TattooID.TattooBreastUpperR)) || ((oid == TattooID.TattooHipLeft) || (oid == TattooID.TattooHipRight))) || (((oid == TattooID.TattooLowerBelly) || (oid == TattooID.TattooThighFrontL)) || (oid == TattooID.TattooThighFrontR)))
                        {
                            goto Label_0426;
                        }
                        if ((oid != TattooID.TattooThighBackL) && (oid != TattooID.TattooThighBackR))
                        {
                            if ((oid == TattooID.TattooCalfFrontL) || (oid == TattooID.TattooCalfFrontR))
                            {
                                goto Label_0426;
                            }
                            if ((oid != TattooID.TattooCalfBackL) && (oid != TattooID.TattooCalfBackR))
                            {
                                if (oid == TattooID.TattooAnkleOuterL)
                                {
                                    CASController.Singleton.SetAnkleCam(true);
                                    this.TryRotateSimTowards(-2.55f);
                                    return;
                                }
                                if (oid == TattooID.TattooAnkleOuterR)
                                {
                                    CASController.Singleton.SetAnkleCam(true);
                                    this.TryRotateSimTowards(2.55f);
                                    return;
                                }
                                if ((oid == TattooID.TattooFootLeft) || (oid == TattooID.TattooFootRight))
                                {
                                    CASController.Singleton.SetAnkleCam(true);
                                    this.TryRotateSimTowards(0f);
                                    return;
                                }
                                if (((oid == TattooID.TattooCheekLeft) || (oid == TattooID.TattooCheekRight)) || (oid == TattooID.TattooForehead))
                                {
                                    goto Label_0456;
                                }
                                if (((oid == TattooID.TattooNippleL) || (oid == TattooID.TattooNippleR)) || (oid == TattooID.TattooPubic))
                                {
                                    goto Label_0426;
                                }
                                return;
                            }
                        }
                    }
                }
            }
        }
        CASController.Singleton.SetTopCam(true);
        this.TryRotateSimTowards(3.141593f);
        return;
    Label_0366:
        CASController.Singleton.SetHandCam(true);
        this.TryRotateSimTowards(-1.256637f);
        return;
    Label_037E:
        CASController.Singleton.SetHandCam(true);
        this.TryRotateSimTowards(1.256637f);
        return;
    Label_03C6:
        CASController.Singleton.SetTopCam(true);
        this.TryRotateSimTowards(-1.256637f);
        return;
    Label_03DE:
        CASController.Singleton.SetTopCam(true);
        this.TryRotateSimTowards(1.256637f);
        return;
    Label_0426:
        CASController.Singleton.SetTopCam(true);
        this.TryRotateSimTowards(0f);
        return;
    Label_0456:
        CASController.Singleton.SetFaceCam(true);
        this.TryRotateSimTowards(0f);
    }

    private void SharePresetDialogTask()
    {
        ObjectDesigner.SetCASPart(this.GetDefaultPart().Key);
        CASPartPreset selectedTag = this.mTattooGrid.SelectedTag as CASPartPreset;
        if (selectedTag != null)
        {
            List<string> list = CASController.ShareDialogResponse("Tattoo Design");
            if (list != null)
            {
                ProgressDialog.Show(Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/Global:Sharing", new object[0x0]));
                if (!ObjectDesigner.ShareDesignPreset(list[0x0], selectedTag.mPresetString, list[0x1], selectedTag.mPresetId))
                {
                    ProgressDialog.Close();
                    CASController.Singleton.ErrorMsg(CASErrorCode.ExportFailed);
                }
            }
        }
    }

    private void ShowColorPickerTask()
    {
        CASMultiColorPickerDialog.OnColorsChanged += new CASMultiColorPickerDialog.ColorsChanged(this.OnColorsChanged);
        CASMultiColorPickerDialog.OnColorsSaved += new CASMultiColorPickerDialog.ColorsSaved(this.OnColorsSaved);
        CASMultiColorPickerDialog.OnDialogClosed += new CASMultiColorPickerDialog.DialogClosed(this.OnDialogClosed);
        Color[] colors = this.GetColors();
        if (colors != null)
        {
            CASController singleton = CASController.Singleton;
            singleton.AllowSimHighlight = false;
            CASMultiColorPickerDialog.ShowTattoo(colors, colors.Length, true, null);
            if (!this.AdvancedMode)
            {
                singleton.AllowSimHighlight = true;
            }
        }
        CASMultiColorPickerDialog.OnColorsChanged -= new CASMultiColorPickerDialog.ColorsChanged(this.OnColorsChanged);
        CASMultiColorPickerDialog.OnColorsSaved -= new CASMultiColorPickerDialog.ColorsSaved(this.OnColorsSaved);
        CASMultiColorPickerDialog.OnDialogClosed -= new CASMultiColorPickerDialog.DialogClosed(this.OnDialogClosed);
    }

    private void StartDrag()
    {
        if (gSingleton != null)
        {
            uint index = this.mClickedWin.ID - 0x92fa021;
            Audio.StartSound("ui_style_pickup");
            Window window = this.mLayerThumbs[index];
            ImageDrawable drawable = window.Drawable as ImageDrawable;
            this.mCursorImage.Image = drawable.Image;
            this.mCursorNoDragImage.Image = this.mCursorImage.Image;
            base.StartDragDrop(index, 0xa9f6d10, 0xa9f6d11, true);
            this.ClearClickEvent();
        }
    }

    private bool TestPresetValidity(CASPartPreset preset)
    {
        int num = 0x0;
        foreach (CASPart part in this.GetVisibleParts(BodyTypes.TattooTemplate))
        {
            int tattooTemplateUsageIndexInPreset = this.GetTattooTemplateUsageIndexInPreset(part, preset.mPresetString);
            if ((tattooTemplateUsageIndexInPreset > 0x0) && (tattooTemplateUsageIndexInPreset <= 0x5L))
            {
                num++;
            }
        }
        uint[] numArray = CASUtils.ExtractEnabledLayerIndices(preset.mPresetString);
        if (num != numArray.Length)
        {
            return false;
        }
        return true;
    }

    private float[] TranslateTattooScale(float[] originalDimensionData, float[] originalReferenceDims, float[] newReferenceDims)
    {
        float[] outputArray = originalDimensionData.Clone() as float[];
        Rect rect = new Rect(originalReferenceDims[0x0], originalReferenceDims[0x1], originalReferenceDims[0x2], originalReferenceDims[0x3]);
        Rect rect2 = new Rect(newReferenceDims[0x0], newReferenceDims[0x1], newReferenceDims[0x2], newReferenceDims[0x3]);
        for (int i = 0x0; i < 0x5L; i++)
        {
            float num2 = originalDimensionData[(i * 0x4) + 0x2] - originalDimensionData[i * 0x4];
            float scale = num2 / rect.Width;
            this.GetDimensionFloatsFromRect(scale, rect2, ref outputArray, (uint) (i * 0x4));
        }
        return outputArray;
    }

    private void TryRotateSimTowards(float angle)
    {
        if (!this.mbTargetSimAngleActive)
        {
            this.mbTargetSimAngleActive = true;
            base.Tick += new UIEventHandler<UIEventArgs>(this.OnTick);
            this.mTargetSimAngleStopWatch.Start();
        }
        this.mTargetSimAngle = angle;
    }

    private static void Unload()
    {
        if (gSingleton != null)
        {
            gSingleton.Dispose();
            gSingleton = null;
        }
        if (sTattooLayout != null)
        {
            sTattooLayout.Shutdown();
            sTattooLayout.Dispose();
            sTattooLayout = null;
        }
    }

    protected override void UnloadThis()
    {
        if (this.UINodeShutdown != null)
        {
            this.UINodeShutdown(this.mTargetState);
        }
    }

    private void UpdateAllParts()
    {
        for (int i = 0x0; i < 0x5L; i++)
        {
            this.SetPart(this.mTemplateParts[i].mTemplatePart, i);
        }
    }

    private void UpdateCurrentDataFromCompositePreset(CASPartPreset preset)
    {
        foreach (CASPart part in this.GetVisibleParts(BodyTypes.TattooTemplate))
        {
            int tattooTemplateUsageIndexInPreset = this.GetTattooTemplateUsageIndexInPreset(part, preset.mPresetString);
            if ((tattooTemplateUsageIndexInPreset > 0x0) && (tattooTemplateUsageIndexInPreset <= 0x5L))
            {
                int index = tattooTemplateUsageIndexInPreset - 0x1;
                this.mTemplateParts[index].mTemplatePart = part;
                this.SetPart(part, index);
                CASPartPreset preset2 = new CASPartPreset(part, CASUtils.PartDataGetPresetId(part.Key, 0x0), CASUtils.PartDataGetPreset(part.Key, 0x0));
                this.mTemplateParts[index].mPreset = new CASPartPreset(part, CASUtils.ExtractTattooTemplate(preset2.mPresetString, preset.mPresetString, (uint) tattooTemplateUsageIndexInPreset));
            }
        }
        float[] newReferenceDims = CASUtils.ExtractTattooDimensions(CASUtils.PartDataGetPreset(this.mActiveTattooPart.Key, 0x0), true);
        float[] originalDimensionData = CASUtils.ExtractTattooDimensions(preset.mPresetString, false);
        this.mTattooLayerScaleData = this.TranslateTattooScale(originalDimensionData, this.kStorageReferenceDimensions, newReferenceDims);
        this.mOpacitySlider.Value = (int) (this.GetFloatOpacityFromOpacityColor(CASUtils.ExtractTattooOpacity(preset.mPresetString)) * this.mOpacitySlider.MaxValue);
    }

    private void UpdateCurrentPreset()
    {
        this.CurrentPreset.mPresetString = string.Empty;
        if (this.ActiveTattooPartInUse())
        {
            this.CurrentPreset.mPresetString = Responder.Instance.CASModel.GetDesignPreset(this.mActiveTattooPart);
        }
        this.CurrentPreset.mPart = this.mActiveTattooPart;
        string preset = CASUtils.PartDataGetPreset(this.mActiveTattooPart.Key, 0x0);
        float[] numArray = CASUtils.ExtractTattooDimensions(preset, true);
        this.mTattooBaseRect.Set(numArray[0x0], numArray[0x1], numArray[0x2], numArray[0x3]);
        if (this.CurrentPreset.mPresetString.Length != 0x0)
        {
            this.mTattooLayerScaleData = CASUtils.ExtractTattooDimensions(this.CurrentPreset.mPresetString, false);
            this.mScaleSlider.Value = (int) (this.GetActivePresetScale() * this.mScaleSlider.MaxValue);
            this.mOpacitySlider.Value = (int) (this.GetActiveTattooOpacity() * this.mOpacitySlider.MaxValue);
        }
        else
        {
            this.mTattooLayerScaleData = CASUtils.ExtractTattooDimensions(preset, false);
            this.mScaleSlider.Value = (this.mScaleSlider.MaxValue - this.mScaleSlider.MinValue) * this.mTunedScales[this.mActiveTattooID];
            this.UpdateScaleFromSlider();
            this.mOpacitySlider.Value = (this.mOpacitySlider.MaxValue + this.mOpacitySlider.MinValue) / 0x2;
        }
    }

    private void UpdateLayerEditButtons()
    {
        if (this.AdvancedMode)
        {
            this.mColorPicker2Button.Enabled = (this.mLayerButtons[this.mActiveLayer].Tag is CASPart) && (((CASPart) this.mLayerButtons[this.mActiveLayer].Tag).Key != this.kInvalidCASPart.Key);
        }
        else
        {
            this.mColorPicker2Button.Enabled = (this.mTattooGrid.SelectedTag is CASPart) && (((CASPart) this.mTattooGrid.SelectedTag).Key != this.kInvalidCASPart.Key);
        }
        this.mColorPickerButton.Enabled = this.mColorPicker2Button.Enabled;
        this.mRemoveLayerButton.Enabled = this.mColorPicker2Button.Enabled;
        this.mScaleSlider.Enabled = this.mColorPicker2Button.Enabled;
        this.mOpacitySlider.Enabled = this.HasAnyPartsInUse();
        this.mAdvancedModeButton.Enabled = (!this.AdvancedMode && (this.mTattooGrid.SelectedItem != 0x0)) && (this.mLayerButtons[this.mActiveLayer].Tag is CASPart);
    }

    private void UpdateModel(bool commit)
    {
        ICASModel cASModel = Responder.Instance.CASModel;
        List<CASPart> wornParts = cASModel.GetWornParts(BodyTypes.Tattoo);
        bool flag = false;
        if (!wornParts.Contains(this.mActiveTattooPart))
        {
            flag = true;
        }
        this.CurrentPreset.mPresetString = CASUtils.PartDataGetPreset(this.mActiveTattooPart.Key, 0x0);
        if (this.HasAnyPartsInUse())
        {
            string[] templatePresets = new string[0x5];
            float[] dimensions = this.mTattooLayerScaleData.Clone() as float[];
            int index = 0x0;
            int num2 = 0x0;
            foreach (TattooTemplateData data in this.mTemplateParts)
            {
                if (((data.mPreset == null) || (data.mTemplatePart.Key == ResourceKey.kInvalidResourceKey)) || (data.mPreset.mPresetString == null))
                {
                    num2++;
                }
                else
                {
                    templatePresets[index] = data.mPreset.mPresetString;
                    for (int i = 0x0; i < 0x4; i++)
                    {
                        dimensions[(index * 0x4) + i] = this.mTattooLayerScaleData[(num2 * 0x4) + i];
                    }
                    num2++;
                    index++;
                }
            }
            this.CurrentPreset.mPresetString = CASUtils.MergeTattooTemplates(this.CurrentPreset.mPresetString, templatePresets);
            this.CurrentPreset.mPresetString = CASUtils.InjectTattooDimensions(this.CurrentPreset.mPresetString, dimensions);
        }
        float num4 = ((float) this.mOpacitySlider.Value) / ((float) this.mOpacitySlider.MaxValue);
        uint color = 0xffffff | (0xff000000 & (((uint) (num4 * 255f)) << 0x18));
        this.CurrentPreset.mPresetString = CASUtils.InjectTattooOpacity(this.CurrentPreset.mPresetString, color);
        if (flag)
        {
            cASModel.RequestAddCASPart(this.mActiveTattooPart, this.CurrentPreset.mPresetString);
        }
        else
        {
            cASModel.RequestCommitPresetToPart(this.mActiveTattooPart, this.CurrentPreset.mPresetString, commit);
        }
    }

    private void UpdateRemoveAllButton()
    {
        List<CASPart> wornParts = Responder.Instance.CASModel.GetWornParts(BodyTypes.Tattoo);
        this.mRemoveAllButton.Enabled = (wornParts != null) && (wornParts.Count != 0x0);
    }

    private void UpdateScaleFromSlider()
    {
        this.GetDimensionFloatsFromRect(((float) this.mScaleSlider.Value) / ((float) this.mScaleSlider.MaxValue), this.mTattooBaseRect, ref this.mTattooLayerScaleData, (uint) (this.mActiveLayer * 0x4));
    }

    private void UpdateTemplatePart(CASPart part)
    {
        int tattooTemplateUsageIndex = this.GetTattooTemplateUsageIndex(part);
        if ((tattooTemplateUsageIndex > 0x0) && (tattooTemplateUsageIndex <= 0x5L))
        {
            int index = tattooTemplateUsageIndex - 0x1;
            this.mTemplateParts[index].mTemplatePart = part;
            CASPartPreset preset = new CASPartPreset(part, CASUtils.PartDataGetPresetId(part.Key, 0x0), CASUtils.PartDataGetPreset(part.Key, 0x0));
            this.mTemplateParts[index].mPreset = new CASPartPreset(part, CASUtils.ExtractTattooTemplate(preset.mPresetString, this.CurrentPreset.mPresetString, (uint) tattooTemplateUsageIndex));
        }
    }

    private void UpdateTemplatePreset(CASPart part, CASPartPreset preset)
    {
        for (uint i = 0x0; i < 0x5; i++)
        {
            if (this.mTemplateParts[i].mTemplatePart.Key == part.Key)
            {
                this.mTemplateParts[i].mPreset = preset;
                if (preset == null)
                {
                    this.mTemplateParts[i].mTemplatePart.Key = ResourceKey.kInvalidResourceKey;
                }
                return;
            }
        }
    }

    // Properties
    private int ActiveLayer
    {
        get
        {
            return this.mActiveLayer;
        }
        set
        {
            this.mActiveLayer = value;
            foreach (Button button in this.mLayerButtons)
            {
                button.Selected = false;
            }
            this.mLayerButtons[this.mActiveLayer].Selected = true;
            List<ItemGridCellItem> items = this.mTattooGrid.Items;
            this.mTattooGrid.SelectedItem = 0xffffffff;
            int num = 0x0;
            foreach (ItemGridCellItem item in items)
            {
                if (item.mTag is CASPart)
                {
                    CASPart mTag = (CASPart) item.mTag;
                    item.mWin.Enabled = true;
                    item.mWin.ShadeColor = new Color(uint.MaxValue);
                    foreach (Button button2 in this.mLayerButtons)
                    {
                        if (button2.Tag is CASPart)
                        {
                            CASPart tag = (CASPart) button2.Tag;
                            if (mTag.Key == tag.Key)
                            {
                                if (button2.Selected)
                                {
                                    this.mTattooGrid.SelectedItem = num;
                                    this.mTattooGrid.ShowSelectedItem(false);
                                }
                                else
                                {
                                    item.mWin.Enabled = false;
                                    item.mWin.ShadeColor = new Color(0xff808080);
                                }
                                item.mWin.Invalidate();
                            }
                        }
                    }
                }
                num++;
            }
            this.PopulatePresetsGrid(this.GetCurrentTemplatePart(), this.mFilterButton.Selected);
            this.UpdateLayerEditButtons();
            this.mScaleSlider.Value = (int) (this.GetActivePresetScale() * this.mScaleSlider.MaxValue);
        }
    }

    private bool AdvancedMode
    {
        get
        {
            return this.mAdvancedMode;
        }
        set
        {
            if (value != this.mAdvancedMode)
            {
                this.mAdvancedMode = value;
                this.ActiveLayer = 0x0;
                this.mAdvancedPanel.Visible = value;
                this.mAdvancedModeButton.Enabled = !value;
                this.mRedoButton.Enabled = false;
                this.mUndoButton.Enabled = false;
                this.mUndoRedoQueue.Clear();
                this.mUndoRedoQueueIndex = 0x0;
                foreach (Button button in this.mNavButtons.Values)
                {
                    button.Enabled = !value;
                }
                CASFacialDetails.gSingleton.TabsEnabled = !value;
                if (value)
                {
                    this.mWasCustom = this.mTattooGrid.HasTempItem;
                    this.mSaveButton.Enabled = false;
                    this.mDeleteButton.Enabled = false;
                    this.mShareButton.Enabled = false;
                    this.mRemoveAllButton.Enabled = false;
                    Responder.Instance.CASModel.UndoSelected -= new UndoRedoSelected(this.OnUndo);
                    Responder.Instance.CASModel.RedoSelected -= new UndoRedoSelected(this.OnRedo);
                    this.mTriggerHandle = base.AddTriggerHook("casTattoo", TriggerActivationMode.kPermanent, 0xa);
                    base.TriggerDown += new UIEventHandler<UITriggerEventArgs>(this.OnTriggerDown);
                    CASController.Singleton.AllowSimHighlight = false;
                    this.AddUndoRedoStep();
                    this.mTattooGrid.RemoveTempItem();
                    this.mAcceptButton.Enabled = false;
                    CASPuck.Instance.ShowSkewer(false, false);
                    if (CASCharacterSheet.gSingleton != null)
                    {
                        CASCharacterSheet.gSingleton.Visible = false;
                    }
                    if (CASTattooSheet.gSingleton != null)
                    {
                        CASTattooSheet.gSingleton.Visible = false;
                    }
                }
                else
                {
                    this.OnNavButtonClickedHelper();
                    this.UpdateRemoveAllButton();
                    Responder.Instance.CASModel.UndoSelected += new UndoRedoSelected(this.OnUndo);
                    Responder.Instance.CASModel.RedoSelected += new UndoRedoSelected(this.OnRedo);
                    if (this.mTriggerHandle != 0x0)
                    {
                        base.RemoveTriggerHook(this.mTriggerHandle);
                        this.mTriggerHandle = 0x0;
                    }
                    base.TriggerDown -= new UIEventHandler<UITriggerEventArgs>(this.OnTriggerDown);
                    CASController.Singleton.AllowSimHighlight = true;
                    base.EffectFinished -= new UIEventHandler<UIHandledEventArgs>(this.OnGlideFinished);
                    base.EffectFinished += new UIEventHandler<UIHandledEventArgs>(this.OnGlideFinished);
                }
                CASFacialDetails.gSingleton.Glide(!value);
                this.Glide(!value);
            }
        }
    }

    private CASPartPreset CurrentPreset
    {
        get
        {
            return this.mCurrentPreset;
        }
        set
        {
            this.mCurrentPreset = value;
        }
    }

    private bool LeftRightEnabled
    {
        set
        {
            this.mNavButtons[ControlIDs.LeftButton].Enabled = value;
            this.mNavButtons[ControlIDs.RightButton].Enabled = value;
        }
    }

    // Nested Types
    private enum ControlIDs : uint
    {
        AcceptButton = 0x92fa02d,
        AdvancedButton = 0x92fa075,
        AdvancedPanel = 0x92fa020,
        AnkleButton = 0x92fa04b,
        AnkleOuterButton = 0x92fa050,
        ArmGroupButton = 0x92fa031,
        ArmPanel = 0x92fa060,
        BackGroupButton = 0x92fa033,
        BackPanel = 0x92fa062,
        BellyButton = 0x92fa039,
        BicepButton = 0x92fa036,
        BodyGroupButton = 0x92fa040,
        BodyPanel = 0x92fa065,
        ButtCheekButton = 0x92fa044,
        CalfBackButton = 0x92fa04f,
        CalfFrontButton = 0x92fa04e,
        CancelButton = 0x92fa02e,
        CheekButton = 0x92fa053,
        ChestButton = 0x92fa038,
        ChestGroupButton = 0x92fa032,
        ChestPanel = 0x92fa061,
        ColorPicker2 = 0x92fa082,
        ColorPickerButton = 0x92fa074,
        CursorDragThumbnail = 0x20,
        CursorNoDragThumbnail = 0x21,
        DeleteButton = 0x92fa071,
        FaceGroupButton = 0x92fa041,
        FacePanel = 0x92fa064,
        FilterButton = 0x92fa070,
        FootButton = 0x92fa051,
        ForearmButton = 0x92fa035,
        ForeheadButton = 0x92fa054,
        FullBackButton = 0x92fa03c,
        FullBodyButton = 0x92fa056,
        FullFaceButton = 0x92fa052,
        GridTattooParts = 0x92fa000,
        GridTattooPresets = 0x92fa001,
        HandButton = 0x92fa045,
        HipButton = 0x92fa04a,
        ItemCompositeTattoo = 0x2a,
        ItemCustom = 0x23,
        ItemThumbnailColor1 = 0x30,
        ItemThumbnailColor2 = 0x31,
        ItemThumbnailColor3 = 0x32,
        ItemThumbnailColor4 = 0x33,
        ItemThumbnailWindow = 0x20,
        ItemUnknownWindow = 0x24,
        ItemWardrobe = 0x29,
        Layer1Button = 0x92fa021,
        Layer1Texture = 0x92fa026,
        Layer2Button = 0x92fa022,
        Layer2Texture = 0x92fa027,
        Layer3Button = 0x92fa023,
        Layer3Texture = 0x92fa028,
        Layer4Button = 0x92fa024,
        Layer4Texture = 0x92fa029,
        Layer5Button = 0x92fa025,
        Layer5Texture = 0x92fa02a,
        LeftButton = 0x92fa03e,
        LegGroupButton = 0x92fa030,
        LegPanel = 0x92fa063,
        LowerBackButton = 0x92fa03d,
        LowerBellyButton = 0x92fa055,
        LowerLowerBack = 0x92fa043,
        NeckButton = 0x92fa03a,
        NippleButton = 0x92fa058,
        OpacitySlider = 0x92fa02c,
        PalmButton = 0x92fa046,
        PubicButton = 0x92fa057,
        RedoButton = 0x92fa081,
        RemoveAllButton = 0x92fa076,
        RemoveLayer = 0x92fa083,
        RibsButton = 0x92fa048,
        RightButton = 0x92fa03f,
        SaveButton = 0x92fa073,
        ShareButton = 0x92fa072,
        ShoulderBackButton = 0x92fa042,
        ShoulderButton = 0x92fa037,
        SizeSlider = 0x92fa02b,
        ThighBackButton = 0x92fa04d,
        ThighFrontButton = 0x92fa04c,
        ThroatButton = 0x92fa047,
        UndoButton = 0x92fa080,
        UpperBackButton = 0x92fa03b,
        UpperBreastButton = 0x92fa049,
        WristButton = 0x92fa034
    }

    private enum CursorId : uint
    {
        CursorDrag = 0xa9f6d10,
        CursorGrabbable = 0x5aed101,
        CursorNoDrag = 0xa9f6d11
    }

    private enum TattooID : ulong
    {
        TattooAnkleLeft = 0x9ead576b7be8318bL,
        TattooAnkleOuterL = 0x88bd1fa38b8c3e10L,
        TattooAnkleOuterR = 0x88bd1fa38b8c3e0eL,
        TattooAnkleRight = 0x9ead576b7be83195L,
        TattooBellybutton = 0xa1ef34b848c95f3cL,
        TattooBicepLeft = 0x16a14f63af3f070bL,
        TattooBicepRight = 0x16a14f63af3f0715L,
        TattooBreastUpperL = 0xcdb7e3fca8d4b018L,
        TattooBreastUpperR = 0xcdb7e3fca8d4b006L,
        TattooButtLeft = 0xce22df867d220705L,
        TattooButtRight = 0xce22df867d22071bL,
        TattooCalfBackL = 0xba4c75252921ff38L,
        TattooCalfBackR = 0xba4c75252921ff26L,
        TattooCalfFrontL = 0xba4c71252921f8e4L,
        TattooCalfFrontR = 0xba4c71252921f8faL,
        TattooCheekLeft = 0x6cccf2e9b615ff36L,
        TattooCheekRight = 0x6cccf2e9b615ff28L,
        TattooChest = 0x8bea918d30acec87L,
        TattooFootLeft = 0x8f846faa3e7fb8e8L,
        TattooFootRight = 0x8f846faa3e7fb8f6L,
        TattooForearmLeft = 0x1f866629d39760acL,
        TattooForearmRight = 0x1f866629d39760b2L,
        TattooForehead = 0xd317f129a8bb8ce8L,
        TattooFullBack = 0x853f2ad9434ee9b6L,
        TattooFullBody = 0x855347d9435fc457L,
        TattooFullFace = 0x343813e5e87d9b1bL,
        TattooHandLeft = 0xf9a6b7b70bc7a91dL,
        TattooHandRight = 0xf9a6b7b70bc7a903L,
        TattooHipLeft = 0x42e368e5f00c3a25L,
        TattooHipRight = 0x42e368e5f00c3a3bL,
        TattooLowerBack = 0x7428a490ecc0e8b6L,
        TattooLowerBelly = 0x5f002887ab7cec46L,
        TattooLowerLowerBack = 0x1750e08416599dL,
        TattooNeck = 0x78b883e60f112cd1L,
        TattooNippleL = 0x5412f73123f4519aL,
        TattooNippleR = 0x5412f73123f45184L,
        TattooNone = 0x0L,
        TattooPalmLeft = 0x7b623afd6dd3433cL,
        TattooPalmRight = 0x7b623afd6dd34322L,
        TattooPubic = 0x134036fd32c9c2a9L,
        TattooRibsLeft = 0x808b6d0a92a997e6L,
        TattooRibsRight = 0x808b6d0a92a997f8L,
        TattooShoulderBackL = 0xf5f6ec4ce872d5d2L,
        TattooShoulderBackR = 0xf5f6ec4ce872d5ccL,
        TattooShoulderLeft = 0x8d202e46cc284844L,
        TattooShoulderRight = 0x8d202e46cc28485aL,
        TattooThighBackL = 0x49e230e4b7463efaL,
        TattooThighBackR = 0x49e230e4b7463ee4L,
        TattooThighFrontL = 0x49e234e4b746450eL,
        TattooThighFrontR = 0x49e234e4b7464510L,
        TattooThroat = 0xda10d9d27c4671ceL,
        TattooUpperBack = 0xfdf67e5d36c7979L,
        TattooWristTopLeft = 0xca54892c9f014f46L,
        TattooWristTopRight = 0xca54892c9f014f58L
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TattooTemplateData
    {
        public CASPart mTemplatePart;
        public CASPartPreset mPreset;
    }

    protected enum Triggers : uint
    {
        kRedo = 0x22551001,
        kUndo = 0x22551000
    }

    private class UndoRedoData
    {
        // Fields
        private uint mOpacityColor;
        private float[] mTattooLayerScaleData;
        private CASTattoo.TattooTemplateData[] mTemplateParts;

        // Methods
        public UndoRedoData(CASTattoo.TattooTemplateData[] templateParts, uint opacityColor, float[] tattooLayerScaleData)
        {
            this.mTemplateParts = templateParts.Clone() as CASTattoo.TattooTemplateData[];
            this.mOpacityColor = opacityColor;
            this.mTattooLayerScaleData = tattooLayerScaleData.Clone() as float[];
        }

        // Properties
        public uint OpacityColor
        {
            get
            {
                return this.mOpacityColor;
            }
        }

        public float[] TattooLayerScaleData
        {
            get
            {
                return (this.mTattooLayerScaleData.Clone() as float[]);
            }
        }

        public CASTattoo.TattooTemplateData[] TemplateParts
        {
            get
            {
                return (this.mTemplateParts.Clone() as CASTattoo.TattooTemplateData[]);
            }
        }
    }
}

 
Collapse Methods
 
