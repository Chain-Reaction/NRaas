using NRaasPacker.Exporters;
using NRaasPacker.Importers;
using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NRaasPacker.MenuUpdaters
{
    public abstract class MenuUpdater
    {
        protected readonly ToolStripMenuItem mItem;

        public MenuUpdater(ToolStripMenuItem item)
        {
            mItem = item;
        }

        public abstract void Update(ListView.SelectedListViewItemCollection selection);
    }
}
