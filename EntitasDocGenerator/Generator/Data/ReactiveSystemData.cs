using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitasDocGenerator
{
    internal class ReactiveSystemData
    {
        internal string Entity { get; set; }
        internal List<string> Triggers { get; set; }
        internal string TriggerType { get; set; }

        internal ReactiveSystemData()
        {
            Entity = string.Empty;
            Triggers = new List<string>();
            TriggerType = string.Empty;
        }
    }
}
