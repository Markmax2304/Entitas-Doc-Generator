using System;
using System.Text;

namespace EntitasDocGenerator
{
    internal static class MarkdownConverter
    {
        private const string headSign = "#";
        private const string markSign = "-";
        private const string contextFormat = "[{0}](#{1})";
        private const string lineSign = "_";
        private const string stickSign = "|";
        private const string dashSign = "-";
        private const string linkFormat = "[:{0}:{1}](#{2})";
        private const string starSign = "*";

        internal static string GetHead(string text, int level)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < level; i++)
            {
                sb.Append(headSign);
            }
            return sb.AppendFormat(" {0}", text).ToString();
        }

        internal static string GetMarkContext(string text, int level)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < level; i++)
            {
                sb.Append("    ");
            }
            sb.Append(markSign).Append(" ");
            return sb.AppendFormat(contextFormat, text, text).ToString();
        }

        internal static string GetHorizontalLine()
        {
            var sb = new StringBuilder();
            for(int i = 0; i < 3; i++)
            {
                sb.Append(lineSign);
            }
            return sb.ToString();
        }

        internal static string GetTableHead(params string[] columns)
        {
            StringBuilder head = new StringBuilder(stickSign);
            StringBuilder line = new StringBuilder(stickSign);

            foreach(var col in columns)
            {
                head.Append(col).Append(stickSign);

                for (int i = 0; i < col.Length; i++)
                {
                    line.Append(dashSign);
                }

                line.Append(stickSign);
            }

            return head.AppendLine().Append(line).ToString();
        }

        internal static string GetTableCustomRow(params string[] columns)
        {
            StringBuilder row = new StringBuilder(stickSign);
            row.Append(starSign).Append(starSign).Append(starSign)
                .Append(columns[0]).Append(starSign).Append(starSign).Append(starSign).Append(stickSign);

            for (int i = 1; i < columns.Length; i++)
            {
                row.Append(columns[i]).Append(stickSign);
            }

            return row.ToString();
        }

        internal static string GetLocalLink(string imageKey, string nameLink)
        {
            return String.Format(linkFormat, imageKey, nameLink, nameLink);
        }
    }
}
