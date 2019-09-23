using System;
using System.IO;

namespace EntitasDocGenerator
{
    class Program
    {
        internal const string componentPattern = "*Component.cs";
        internal const string systemPattern = "*System.cs";
        internal const string systemDir = "Systems";
        internal const string componentDir = "Components";
        internal const string descriptionSign = "/d";

        static void Main(string[] args)
        {
            try
            {
                string sourceDirectory = args[0];//*/ @"D:\My_Documents\Projects\test_doc";
                string docDirectory = args[1];//*/ @"D:\My_Documents\Projects\test_doc";

                if (!Directory.Exists(sourceDirectory) || !Directory.Exists(docDirectory))
                    return;

                Node root = new Node(sourceDirectory);
                ParseDirectory(sourceDirectory, root);

                DocCreator.CreateDocumetation(root, docDirectory);

                //Console.ReadKey();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }

        static void ParseDirectory(string path, Node parent)
        {
            string[] folders = Directory.GetDirectories(path);

            foreach(string folder in folders)
            {
                switch (Path.GetFileName(folder))
                {
                    case systemDir:
                        foreach (string system in Directory.GetFiles(folder, systemPattern))
                        {
                            parent.AddSystem(system);
                        }
                        break;
                    case componentDir:
                        foreach (string component in Directory.GetFiles(folder, componentPattern))
                        {
                            parent.AddComponent(component);
                        }
                        break;
                    default:
                        Node node = new Node(folder);
                        parent.AddChildNode(node);
                        ParseDirectory(folder, node);
                        break;
                }
            }
        }
    }
}
