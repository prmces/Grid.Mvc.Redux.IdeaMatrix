using System;
using GridMvc.Sorting;

namespace GridMvc.DataAnnotations
{
    /// <summary>
    ///     Marks property as Grid.Mvc column, with specified parameters
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GridColumnAttribute : GridHiddenColumnAttribute
    {
        private GridSortDirection? _initialDirection;
        private GridAggregateFunction? _aggregateValue;

        public GridColumnAttribute()
        {
            EncodeEnabled = true;
            SanitizeEnabled = true;
            SortEnabled = false;
            AggregateValue = GridAggregateFunction.None;
        }

        /// <summary>
        ///     Sets or get the column title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Sets or get the CSS class
        /// </summary>
        public string Css { get; set; }


        /// <summary>
        ///     Enable or disable column sorting
        /// </summary>
        public bool SortEnabled { get; set; }

        /// <summary>
        ///     Enable or disable column filtering
        /// </summary>
        public bool FilterEnabled { get; set; }

        /// <summary>
        ///     Sets or get column width,
        ///     Sample: "100px", "13%" ...
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        ///     Sets or get column custom filter widget type
        /// </summary>
        public string FilterWidgetType { get; set; }

        /// <summary>
        ///     Sets or get column Boolean True Label Name (Default: Yes)
        /// </summary>
        public string BoolTrue { get; set; }

        /// <summary>
        //     Sets or get column Boolean False Label Name (Default: No)
        /// </summary>
        public string BoolFalse { get; set; }

        /// <summary>
        ///     Sets or get sort initial direction
        /// </summary>
        public GridSortDirection SortInitialDirection
        {
            get { return _initialDirection.HasValue ? _initialDirection.Value : GridSortDirection.Ascending; }
            set { _initialDirection = value; }
        }

        public string ToolTip { get; set; }

        public GridSortDirection? GetInitialSortDirection()
        {
            return _initialDirection;
        }

        /// <summary>
        ///     Sets or get the AggregateValue
        /// </summary>
        public GridAggregateFunction AggregateValue
        {
            get { return _aggregateValue.HasValue ? _aggregateValue.Value : GridAggregateFunction.None; }
            set { _aggregateValue = value; }
        }

        public GridAggregateFunction? GetAggregateFunction()
        {
            return _aggregateValue;
        }

    }
}