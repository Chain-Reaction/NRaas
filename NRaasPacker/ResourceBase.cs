using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRaasPacker
{
    public abstract class ResourceBase
    {
        protected IResourceIndexEntry mEntry;

        public ResourceBase(IResourceIndexEntry entry)
        {
            mEntry = entry;
        }

        protected IResource CreateResource(IPackage package)
        {
            return ResourceHandlers.CreateResource(mEntry, package);
        }
    }
}
