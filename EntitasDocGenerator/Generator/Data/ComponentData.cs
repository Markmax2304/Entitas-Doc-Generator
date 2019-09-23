using System.Collections.Generic;

namespace EntitasDocGenerator
{
    internal class ComponentData
    {
        internal string Name { get; set; }
        internal List<string> Contexts { get; set; }
        internal string Description { get; set; }
        internal bool Unique { get; set; }
        internal bool Event { get; set; }
        internal List<string> Fields { get; set; }

        public ComponentData()
        {
            Name = string.Empty;
            Contexts = new List<string>();
            Description = string.Empty;
            Unique = false;
            Event = false;
            Fields = new List<string>();
        }
    }
}
