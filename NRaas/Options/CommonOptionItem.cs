using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Options
{
    public interface ICommonOptionItem
    {
        string Name
        {
            get;
        }

        ThumbnailKey Thumbnail
        {
            get;
        }

        string DisplayValue
        {
            get;
        }

        bool UsingCount
        {
            get;
        }

        int Count
        {
            get;
            set;
        }

        string DisplayKey
        {
            get;
        }

        int ValueWidth
        {
            get;
        }

        ICommonOptionItem Clone();
    }

    public interface ICloseDialogOption
    { }

    [Persistable]
    public abstract class CommonOptionItem : ICommonOptionItem
    {
        protected string mName;

        [Persistable(false)]
        protected int mCount = -1;

        [Persistable(false)]
        protected ThumbnailKey mThumbnail = ThumbnailKey.kInvalidThumbnailKey;

        public CommonOptionItem()
        { }
        public CommonOptionItem(string name)
            : this(name, -1)
        { }
        public CommonOptionItem(string name, int count)
        {
            mName = name;
            mCount = count;
        }
        public CommonOptionItem(string name, int count, string icon, ProductVersion version)
            : this(name, count)
        {
            SetThumbnail(icon, version);
        }
        public CommonOptionItem(string name, int count, ResourceKey icon)
            : this(name, count)
        {
            SetThumbnail(icon);
        }
        public CommonOptionItem(string name, int count, ThumbnailKey thumbnail)
            : this(name, count)
        {
            mThumbnail = thumbnail;
        }
        public CommonOptionItem(CommonOptionItem source)
            : this(source.mName, source.mCount, source.mThumbnail)
        { }

        public void SetThumbnail(string icon, ProductVersion version)
        {
            SetThumbnail (ResourceKey.CreatePNGKey(icon, ResourceUtils.ProductVersionToGroupId(version)));
        }
        public void SetThumbnail(ThumbnailKey key)
        {
            mThumbnail = key;
        }
        public void SetThumbnail(ResourceKey icon)
        {
            mThumbnail = new ThumbnailKey(icon, ThumbnailSize.Medium);
        }

        public virtual string Name
        {
            get
            {
                return mName;
            }
        }

        public ThumbnailKey Thumbnail
        {
            get
            {
                return mThumbnail;
            }
        }

        public abstract string DisplayValue
        {
            get;
        }

        public bool IsSet
        {
            get { return (Count > 0); }
        }

        public virtual bool UsingCount
        {
            get { return (Count != -1); }
        }

        public int Count
        {
            get { return mCount; }
            set { mCount = value; }
        }

        public virtual string DisplayKey
        {
            get
            {
                return null;
            }
        }

        public virtual int ValueWidth
        {
            get { return 0; }
        }

        public void IncCount()
        {
            mCount++;
        }
        public void IncCount(int count)
        {
            mCount += count;
        }

        public virtual void Reset()
        {
            if (UsingCount)
            {
                mCount = 0;
            }
        }

        public override string ToString()
        {
            string displayValue = DisplayValue;
            if (displayValue != null)
            {
                return Name + " = " + displayValue;
            }
            else
            {
                return Name;
            }
        }

        public static int SortByName(CommonOptionItem l, CommonOptionItem r)
        {
            return l.Name.CompareTo(r.Name);
        }

        public virtual ICommonOptionItem Clone()
        {
            return MemberwiseClone() as ICommonOptionItem;
        }
    }
}
