using System.Collections.Generic;

namespace EntitasDocGenerator
{
    internal class Node
    {
        internal string Path { get; private set; }

        internal bool HasNodes { get { return Children.Count > 0; } }
        internal bool HasComponents { get { return Components.Count > 0; } }
        internal bool HasSystems { get { return Systems.Count > 0; } }

        internal List<Node> Children { get; private set; }
        internal List<string> Components { get; private set; }
        internal List<string> Systems { get; private set; }

        internal Node(string path)
        {
            Path = path;
            Children = new List<Node>();
            Components = new List<string>();
            Systems = new List<string>();
        }

        internal void AddChildNode(Node node)
        {
            if (!Children.Contains(node))
            {
                Children.Add(node);
            }
        }

        internal void AddComponent(string component)
        {
            if (!Components.Contains(component))
            {
                Components.Add(component);
            }
        }

        internal void AddSystem(string system)
        {
            if (!Systems.Contains(system))
            {
                Systems.Add(system);
            }
        }

        public override string ToString()
        {
            return Path;
        }
    }
}
