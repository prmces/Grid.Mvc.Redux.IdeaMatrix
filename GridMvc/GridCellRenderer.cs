using System.Web;
using System.Web.Mvc;
using GridMvc.Columns;
using System;

namespace GridMvc
{
    public class GridCellRenderer : GridStyledRenderer, IGridCellRenderer
    {
        private const string TdClass = "grid-cell";

        public GridCellRenderer()
        {
            AddCssClass(TdClass);
        }

        public IHtmlString Render(IGridColumn column, IGridCell cell)
        {
            string cssStyles = GetCssStylesString();
            string cssClass = GetCssClassesString();

            var builder = new TagBuilder("td");
            if (!string.IsNullOrWhiteSpace(cssClass))
                builder.AddCssClass(cssClass);
            if (!string.IsNullOrWhiteSpace(cssStyles))
                builder.MergeAttribute("style", cssStyles);
            builder.MergeAttribute("data-name", column.Name);


            if (!column.HasCustomRender)
            {


                if (column.ColumnType.Name.ToLower() == "boolean" && !string.IsNullOrWhiteSpace(cell.ToString()))
                {

                    string booltrue = "Yes";
                    string boolfalse = "No";

                    if (!String.IsNullOrEmpty(column.BoolTrue))
                        booltrue = column.BoolTrue;
                    if (!String.IsNullOrEmpty(column.BoolFalse))
                        boolfalse = column.BoolFalse;

                    builder.InnerHtml = "<span class=\"label label-" + (cell.ToString().ToLower() == "true" ? "primary" : "default") + "\">" + (cell.ToString().ToLower() == "true" ? booltrue : boolfalse) + "</span>";
                }
                else
                {
                    builder.InnerHtml = cell.ToString();
                }
            }
            else
            {
                builder.InnerHtml = cell.ToString();
            }


            return MvcHtmlString.Create(builder.ToString());
        }
    }
}