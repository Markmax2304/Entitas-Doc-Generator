using System.Collections.Generic;

namespace EntitasDocGenerator
{
    internal class SystemData
    {
        internal string Name { get; set; }
        internal string Description { get; set; }
        internal List<string> Types { get; set; }
        internal ReactiveSystemData Reactive { get; set; }
        internal bool IsReactive { get { return Reactive != null; } }

        internal SystemData()
        {
            Name = string.Empty;
            Description = string.Empty;
            Types = new List<string>();
            Reactive = null;
        }
    }
}
