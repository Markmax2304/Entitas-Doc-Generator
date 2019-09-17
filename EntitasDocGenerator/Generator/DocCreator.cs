using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EntitasDocGenerator
{
    internal static class DocCreator
    {
        private const string docName = "Documentation.md";
        private const string projectDescription = "This {0} of project based on Entitas framework. " +
                "It looks like list of components and systems with its short description";
        private const string context = "Context";
        private const string componentExt = "Component";
        private const string systemExt = "System";
        private const string sharpExt = ".cs";
        private const string descriptionTable = "Description";
        private const string imgArrowUp = "arrow_up";

        internal static void CreateDocumetation(Node root, string docDirectory, bool description)
        {
            string projectName = "NeonZuma documentation";

            using (var streamWriter = new StreamWriter(Path.Combine(docDirectory, docName), false, Encoding.Default))
            {
                // Before context
                streamWriter.WriteLine(MarkdownConverter.GetHead(projectName, 1));
                streamWriter.WriteLine(String.Format(projectDescription, projectName));
                streamWriter.WriteLine();

                // Context
                streamWriter.WriteLine(MarkdownConverter.GetHead(context, 2));
                WriteContext(root, streamWriter);
                streamWriter.WriteLine(MarkdownConverter.GetHorizontalLine());

                // Parse each top
                WriteTops(root, streamWriter, description);
            }
        }

        private static void WriteContext(Node root, StreamWriter writer, int indent = 0)
        {
            if (!root.HasNodes)
                return;

            foreach(var node in root.Children)
            {
                string text = Path.GetFileName(node.Path);
                writer.WriteLine(MarkdownConverter.GetMarkContext(text, indent));

                if (node.HasNodes)
                {
                    WriteContext(node, writer, indent + 1);
                }
            }
        }

        private static void WriteTops(Node root, StreamWriter writer, bool description, int level = 2)
        {
            if (!root.HasNodes)
                return;

            foreach(var node in root.Children)
            {
                string name = Path.GetFileName(node.Path);
                writer.WriteLine(MarkdownConverter.GetHead(name, level));

                if (node.HasSystems)
                {
                    WriteTableElements(node.Systems, writer, systemExt);
                }

                if (node.HasComponents)
                {
                    WriteTableElements(node.Components, writer, componentExt);
                }

                writer.WriteLine(MarkdownConverter.GetLocalLink(imgArrowUp, context));

                if (node.HasNodes)
                {
                    WriteTops(node, writer, description, level + 1);
                }
            }
        }

        // TODO: separate to two methods for systems and components
        private static void WriteTableElements(List<string> elements, StreamWriter writer, string extension)
        {
            writer.WriteLine(MarkdownConverter.GetTableHead(extension, descriptionTable));

            foreach(var element in elements)
            {
                // TODO: Replace getting name. Better get name from file as class name
                // also, in future, maybe need to get component attributes and description from comments
                string name = Path.GetFileName(element);
                name = name.Replace(extension, string.Empty);
                name = name.Replace(sharpExt, string.Empty);
                writer.WriteLine(MarkdownConverter.GetTableCustomRow(name, "Hello world"));
            }

            writer.WriteLine(MarkdownConverter.GetHorizontalLine());
        }
    }
}
