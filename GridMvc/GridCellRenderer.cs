using System.Web;
using System.Web.Mvc;
using GridMvc.Columns;

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
                    builder.InnerHtml = "<input type=\"checkbox\" id=\"" + column.Name + "\" " + (cell.ToString().ToLower() == "true" ? "checked" : "") + ">";
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