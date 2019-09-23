using System;
using System.Collections.Generic;
using System.Text;

namespace EntitasDocGenerator
{
    internal enum AlignmentCell { Left, Center, Right, None }

    internal static class MarkdownConverter
    {
        private const string headSign = "#";
        private const string dashSign = "-";
        private const string twoDotsSign = ":";
        private const string contextFormat = "[{0}](#{1})";
        private const string lineSign = "_";
        private const string stickSign = "|";
        private const string linkFormat = "[{0}{1}](#{2})";
        private const string starSign = "*";

        private const string ulBeginTag = "<ul>";
        private const string ulEndTag = "</ul>";
        private const string liBeginTag = "<li>";
        private const string liEndTag = "</li>";

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
            sb.Append(dashSign).Append(" ");
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

        internal static string GetTableHead(params HeadCell[] cells)
        {
            StringBuilder head = new StringBuilder(stickSign);
            StringBuilder line = new StringBuilder(stickSign);

            foreach(var cell in cells)
            {
                head.Append(cell.Text).Append(stickSign);

                if(cell.Align == AlignmentCell.Center || cell.Align == AlignmentCell.Left)
                {
                    line.Append(twoDotsSign);
                }

                for (int i = 0; i < cell.Text.Length; i++)
                {
                    line.Append(dashSign);
                }

                if (cell.Align == AlignmentCell.Center || cell.Align == AlignmentCell.Right)
                {
                    line.Append(twoDotsSign);
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

        internal static string ConvertToUnnumaricalList(List<string> tops)
        {
            StringBuilder sb = new StringBuilder(ulBeginTag);

            foreach(var top in tops)
            {
                sb.Append(liBeginTag).Append(top).Append(liEndTag);
            }

            return sb.Append(ulEndTag).ToString();
        }
    }
}
