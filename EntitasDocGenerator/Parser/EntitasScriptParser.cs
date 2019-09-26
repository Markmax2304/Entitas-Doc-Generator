using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EntitasDocGenerator
{
    internal static class EntitasScriptParser
    {
        private const string componentExt = "Component";
        private const string systemExt = "System";
        private const string allMatcher = "AllOf";
        private const string anyMatcher = "AnyOf";
        private const string andUpper = "AND";
        private const string orUpper = "OR";

        #region Regexes
        private static readonly Regex summaryRegex = new Regex(@"<summary>(.+?)</summary>");
        private static readonly Regex attributeRegex = new Regex(@"\[(.+?)\]");
        private static readonly Regex firstWordRegex = new Regex(@"^\w+");
        private static readonly Regex nameRegex = new Regex(@"class\s+([\w_]+)");
        private static readonly Regex bodyComponentRegex = new Regex(@"\{(.*)\}");
        private static readonly Regex fieldRegex = new Regex(@"public\s+(.*)\s+[\w_]+");

        private static readonly Regex classRegex = new Regex(@"class.+?:(.*?){");
        private static readonly Regex typeSystemRegex = new Regex(@"I([\w_]+?)System");
        private static readonly Regex reactiveSystemRegex = new Regex(@"ReactiveSystem\<(.+?)Entity\>");
        private static readonly Regex triggerSystemRegex = new Regex(@"GetTrigger\(.+?\)\s*\{(.+?)\}");
        private static readonly Regex filterSystemRegex = new Regex(@"Filter\(.+?\)\s*\{\s*return\s+(.+?);\s*\}");
        #endregion

        internal static string ParseSummaryDescription(string text)
        {
            var descrMatches = summaryRegex.Matches(text);
            StringBuilder sb = new StringBuilder();

            foreach (Match match in descrMatches)
            {
                foreach(var line in match.Groups[1].Value.Split('/'))
                {
                    if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
                        continue;

                    sb.AppendLine(line.Trim());
                }
            }

            return sb.ToString();
        }

        #region Component Parsing
        internal static void ParseComponentAttributes(string text, ComponentData record)
        {
            var attrMatches = attributeRegex.Matches(text);

            foreach(Match match in attrMatches)
            {
                string[] attributes = match.Groups[1].Value.Split(',', ' ');
                foreach(var attribute in attributes)
                {
                    if (string.IsNullOrEmpty(attribute))
                        continue;

                    string neatAttr = firstWordRegex.Match(attribute).Value;
                    switch (neatAttr)
                    {
                        case "Unique":
                            record.Unique = true;
                            break;
                        case "FlagPrefix":
                            break;
                        case "PrimaryEntityIndex":
                            break;
                        case "EntityIndex":
                            break;
                        case "CustomComponentName":
                            break;
                        case "DontGenerate":
                            break;
                        case "Event":
                            record.Event = true;
                            break;
                        case "Cleanup":
                            break;
                        default:
                            record.Contexts.Add(neatAttr);
                            break;
                    }
                }
            }
        }

        internal static string ParseComponentName(string text)
        {
            var match = nameRegex.Match(text);
            return match.Groups[1].Value.Replace(componentExt, string.Empty);
        }

        internal static void ParseComponentFields(string text, ComponentData record)
        {
            string body = bodyComponentRegex.Match(text).Groups[1].Value;
            string[] rawFields = body.Split(';');
            foreach(var field in rawFields)
            {
                var match = fieldRegex.Match(field);
                string res = match.Groups[1].Value;

                if (string.IsNullOrEmpty(res))
                    continue;

                record.Fields.Add(res);
            }
        }
        #endregion

        #region System Parsing
        internal static string ParseSystemName(string text)
        {
            var match = nameRegex.Match(text);
            return match.Groups[1].Value.Replace(systemExt, string.Empty);
        }

        internal static void ParseSystemTypes(string text, SystemData record)
        {
            var types = classRegex.Match(text);
            var matches = typeSystemRegex.Matches(types.Groups[1].Value);
            foreach(Match match in matches)
            {
                record.Types.Add(match.Groups[1].Value);
            }
        }

        internal static bool TryParseReactiveSystem(string text, out string entity)
        {
            var match = reactiveSystemRegex.Match(text);
            entity = match.Groups[1].Value;
            return string.IsNullOrEmpty(entity) ? false : true;
        }

        internal static void ParseSystemTriggers(string text, ReactiveSystemData record)
        {
            var trigger = triggerSystemRegex.Match(text);
            string returnLine = trigger.Groups[1].Value;

            Regex regex = new Regex(record.Entity + @"Matcher\.([\w_]+)");
            var matches = regex.Matches(returnLine);

            foreach(Match match in matches)
            {
                if (string.Compare(match.Groups[1].Value, allMatcher) == 0)
                {
                    record.TriggerType = allMatcher;
                }
                else if (string.Compare(match.Groups[1].Value, anyMatcher) == 0)
                {
                    record.TriggerType = anyMatcher;
                }
                else
                {
                    record.Triggers.Add(match.Groups[1].Value);
                }
            }
        }
        #endregion
    }
}
