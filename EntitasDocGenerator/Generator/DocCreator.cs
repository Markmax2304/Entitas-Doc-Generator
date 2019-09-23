using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        private const string componentNameTable = "Component";
        private const string contextTable = "Contexts";
        private const string uniqueTable = "Unique";
        private const string eventTable = "Event";
        private const string fieldsTable = "Fields";
        private const string descriptionTable = "Description";

        private const string systemNameTable = "System";
        private const string typeTable = "Types";

        private const string reactiveSystemNameTable = "Reactive System";
        private const string EntityTable = "Entity";
        private const string filterTable = "Filter";
        private const string triggerTable = "Triggers";

        private const string imgArrowUp = ":arrow_up:";
        private const string imgCheckMark = ":white_check_mark:";
        private const string imgNegCheckMark = ":x:";
        private const string imgFlagMark = ":triangular_flag_on_post:";

        internal static void CreateDocumetation(Node root, string docDirectory)
        {
            string projectName = "NeonZuma documentation";

            using (var streamWriter = new StreamWriter(Path.Combine(docDirectory, docName), false, Encoding.UTF8))
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
                WriteTops(root, streamWriter);
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

        private static void WriteTops(Node root, StreamWriter writer, int level = 2)
        {
            if (!root.HasNodes)
                return;

            foreach(var node in root.Children)
            {
                string name = Path.GetFileName(node.Path);
                writer.WriteLine(MarkdownConverter.GetHead(name, level));

                if (node.HasSystems)
                {
                    WriteTableSystem(node.Systems, writer);
                }

                if (node.HasComponents)
                {
                    WriteTableComponent(node.Components, writer);
                }

                writer.WriteLine(MarkdownConverter.GetLocalLink(imgArrowUp, context));

                if (node.HasNodes)
                {
                    WriteTops(node, writer, level + 1);
                }
            }
        }

        #region Write Systems
        private static void WriteTableSystem(List<string> elements, StreamWriter writer)
        {
            List<SystemData> systems = new List<SystemData>();

            foreach(var element in elements)
            {
                systems.Add(ParseSystemFile(element));
            }

            // system
            writer.WriteLine(MarkdownConverter.GetTableHead(
                new HeadCell(systemNameTable, AlignmentCell.None),
                new HeadCell(typeTable, AlignmentCell.Left),
                new HeadCell(descriptionTable, AlignmentCell.Left)));

            foreach (var system in systems.Where(x => !x.IsReactive))
            {
                string description = system.Description.Replace("\r\n", "<br/>");
                string types = system.Types.Count > 1 ?
                    MarkdownConverter.ConvertToUnnumaricalList(system.Types) :
                    system.Types.First();

                writer.WriteLine(MarkdownConverter.GetTableCustomRow(system.Name, types, description));
            }

            writer.WriteLine(MarkdownConverter.GetHorizontalLine());

            // reactive
            writer.WriteLine(MarkdownConverter.GetTableHead(
                new HeadCell(reactiveSystemNameTable, AlignmentCell.None),
                new HeadCell(typeTable, AlignmentCell.Left),
                new HeadCell(EntityTable, AlignmentCell.Center),
                new HeadCell(filterTable, AlignmentCell.Left),
                new HeadCell(triggerTable, AlignmentCell.Left),
                new HeadCell(descriptionTable, AlignmentCell.Left)));

            foreach (var system in systems.Where(x => x.IsReactive))
            {
                string description = system.Description.Replace("\r\n", "<br/>");
                string types = system.Types.Count > 1 ?
                    MarkdownConverter.ConvertToUnnumaricalList(system.Types) :
                    (system.Types.FirstOrDefault() ?? imgNegCheckMark);
                string triggerType = (string.IsNullOrEmpty(system.Reactive.TriggerType) ?
                    string.Format("{0}:<br/>", system.Reactive.TriggerType) : string.Empty);
                string triggers = system.Reactive.Triggers.Count > 1 ?
                    MarkdownConverter.ConvertToUnnumaricalList(system.Reactive.Triggers) :
                    (system.Reactive.Triggers.FirstOrDefault() ?? imgNegCheckMark);

                writer.WriteLine(MarkdownConverter.GetTableCustomRow(system.Name, types, system.Reactive.Entity,
                    system.Reactive.Filter, triggerType + triggers, description));
            }

            writer.WriteLine(MarkdownConverter.GetHorizontalLine());
        }

        private static SystemData ParseSystemFile(string path)
        {
            SystemData record = new SystemData();

            using (var reader = new StreamReader(path))
            {
                string fileText = reader.ReadToEnd()
                    .Replace("\r\n", string.Empty);

                record.Name = EntitasScriptParser.ParseSystemName(fileText);
                record.Description = EntitasScriptParser.ParseSummaryDescription(fileText);
                EntitasScriptParser.ParseSystemTypes(fileText, record);

                if(EntitasScriptParser.TryParseReactiveSystem(fileText, out string entity))
                {
                    record.Reactive = new ReactiveSystemData() { Entity = entity };
                    EntitasScriptParser.ParseSystemTriggers(fileText, record.Reactive);
                    EntitasScriptParser.ParseSystemFilters(fileText, record.Reactive);
                }
            }

            return record;
        }
        #endregion

        #region Write Components
        private static void WriteTableComponent(List<string> elements, StreamWriter writer)
        {
            writer.WriteLine(MarkdownConverter.GetTableHead(
                new HeadCell(componentNameTable, AlignmentCell.None),
                new HeadCell(contextTable, AlignmentCell.Left),
                new HeadCell(uniqueTable, AlignmentCell.Center),
                new HeadCell(eventTable, AlignmentCell.Center),
                new HeadCell(fieldsTable, AlignmentCell.Left),
                new HeadCell(descriptionTable, AlignmentCell.Left)));

            foreach (var element in elements)
            {
                var component = ParseComponentFile(element);

                string description = component.Description.Replace("\r\n", "<br/>");
                string contexts = component.Contexts.Count > 1 ?
                    MarkdownConverter.ConvertToUnnumaricalList(component.Contexts) :
                    (component.Contexts.FirstOrDefault() ?? imgNegCheckMark);
                string unique = component.Unique ? imgCheckMark : string.Empty;
                string ecsEvent = component.Event ? imgCheckMark : string.Empty;
                string fields = component.Fields.Count > 1 ? MarkdownConverter.ConvertToUnnumaricalList(component.Fields) :
                    (component.Fields.FirstOrDefault() ?? imgFlagMark);
                writer.WriteLine(MarkdownConverter.GetTableCustomRow(component.Name, contexts,
                    unique, ecsEvent, fields, description));
            }

            writer.WriteLine(MarkdownConverter.GetHorizontalLine());
        }

        private static ComponentData ParseComponentFile(string path)
        {
            ComponentData record = new ComponentData();

            using(var reader = new StreamReader(path))
            {
                string fileText = reader.ReadToEnd()
                    .Replace("\r\n", string.Empty);

                record.Name = EntitasScriptParser.ParseComponentName(fileText);
                record.Description = EntitasScriptParser.ParseSummaryDescription(fileText);
                EntitasScriptParser.ParseComponentAttributes(fileText, record);
                EntitasScriptParser.ParseComponentFields(fileText, record);
            }

            return record;
        }
        #endregion
    }
}
