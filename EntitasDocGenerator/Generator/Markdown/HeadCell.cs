using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitasDocGenerator
{
    internal class HeadCell
    {
        internal string Text { get; set; }
        internal AlignmentCell Align { get; set; }

        internal HeadCell(string text, AlignmentCell align)
        {
            Text = text;
            Align = align;
        }
    }
}
