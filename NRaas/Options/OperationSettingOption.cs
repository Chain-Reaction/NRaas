﻿using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;

namespace NRaas.CommonSpace.Options
{
    public abstract class OperationSettingOption<TTarget> : InteractionOptionItem<IActor,TTarget,GameHitParameters<TTarget>>
        where TTarget : class, IGameObject
    {
        public OperationSettingOption()
        { }
        public OperationSettingOption(string name)
            : base(name, -1)
        { }
        public OperationSettingOption(string name, int count)
            : base(name, count)
        { }
        public OperationSettingOption(string name, int count, string icon, ProductVersion version)
            : base(name, count, icon, version)
        { }
        public OperationSettingOption(string name, int count, ResourceKey icon)
            : base(name, count, icon)
        { }
        public OperationSettingOption(string name, int count, ThumbnailKey thumbnail)
            : base(name, count, thumbnail)
        { }

        public override string DisplayValue
        {
            get { return null; }
        }
    }
}
