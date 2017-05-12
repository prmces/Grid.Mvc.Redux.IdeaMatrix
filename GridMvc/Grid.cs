﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GridMvc.Columns;
using GridMvc.DataAnnotations;
using GridMvc.Filtering;
using GridMvc.Html;
using GridMvc.Pagination;
using GridMvc.Resources;
using GridMvc.Sorting;
using System;


namespace GridMvc
{
    /// <summary>
    ///     Grid.Mvc base class
    /// </summary>
    public class Grid<T> : GridBase<T>, IGrid where T : class
    {

        private readonly IGridAnnotaionsProvider _annotaions;
        private readonly IColumnBuilder<T> _columnBuilder;
        private readonly GridColumnCollection<T> _columnsCollection;
        private readonly FilterGridItemsProcessor<T> _currentFilterItemsProcessor;
        private readonly SortGridItemsProcessor<T> _currentSortItemsProcessor;

        private int _displayingItemsCount = -1; // count of displaying items (if using pagination)
        private bool _enablePaging;
        private IGridPager _pager;

        private IGridItemsProcessor<T> _pagerProcessor;
        private IGridSettingsProvider _settings;

        public Grid(IEnumerable<T> items)
            : this(items.AsQueryable())
        {
        }

        public Grid(IQueryable<T> items)
            : base(items)
        {
            #region init default properties

            //set up sort settings:
            _settings = new QueryStringGridSettingsProvider();
            Sanitizer = new Sanitizer();
            EmptyGridText = Strings.DefaultGridEmptyText;
            Language = Strings.Lang;

            _currentSortItemsProcessor = new SortGridItemsProcessor<T>(this, _settings.SortSettings);
            _currentFilterItemsProcessor = new FilterGridItemsProcessor<T>(this, _settings.FilterSettings);
            AddItemsPreProcessor(_currentFilterItemsProcessor);
            InsertItemsProcessor(0, _currentSortItemsProcessor);

            _annotaions = new GridAnnotaionsProvider();

            #endregion

            //Set up column collection:
            _columnBuilder = new DefaultColumnBuilder<T>(this, _annotaions);
            _columnsCollection = new GridColumnCollection<T>(_columnBuilder, _settings.SortSettings);
            RenderOptions = new GridRenderOptions();

            ApplyGridSettings();
        }

        /// <summary>
        ///     Grid columns collection
        /// </summary>
        public IGridColumnCollection<T> Columns
        {
            get { return _columnsCollection; }
        }

        /// <summary>
        ///     Sets or get default value of sorting for all adding columns
        /// </summary>
        public bool DefaultSortEnabled
        {
            get { return _columnBuilder.DefaultSortEnabled; }
            set { _columnBuilder.DefaultSortEnabled = value; }
        }

        /// <summary>
        ///     Set or get default value of filtering for all adding columns
        /// </summary>
        public bool DefaultFilteringEnabled
        {
            get { return _columnBuilder.DefaultFilteringEnabled; }
            set { _columnBuilder.DefaultFilteringEnabled = value; }
        }

        public GridRenderOptions RenderOptions { get; set; }

        /// <summary>
        ///     Provides settings, using by the grid
        /// </summary>
        public override IGridSettingsProvider Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                _currentSortItemsProcessor.UpdateSettings(_settings.SortSettings);
                _currentFilterItemsProcessor.UpdateSettings(_settings.FilterSettings);
            }
        }

        /// <summary>
        ///     Items, displaying in the grid view
        /// </summary>
        IEnumerable<object> IGrid.ItemsToDisplay
        {
            get { return GetItemsToDisplay(); }
        }

        #region IGrid Members

        /// <summary>
        ///     Count of current displaying items
        /// </summary>
        public virtual int DisplayingItemsCount
        {
            get
            {
                if (_displayingItemsCount >= 0)
                    return _displayingItemsCount;
                _displayingItemsCount = GetItemsToDisplay().Count();
                return _displayingItemsCount;
            }
        }

        /// <summary>
        ///     Enable or disable paging for the grid
        /// </summary>
        public bool EnablePaging
        {
            get { return _enablePaging; }
            set
            {
                if (_enablePaging == value) return;
                _enablePaging = value;
                if (_enablePaging)
                {
                    if (_pagerProcessor == null)
                        _pagerProcessor = new PagerGridItemsProcessor<T>(Pager);
                    AddItemsProcessor(_pagerProcessor);
                }
                else
                {
                    RemoveItemsProcessor(_pagerProcessor);
                }
            }
        }


        public string Language { get; set; }

        /// <summary>
        ///     Gets or set Grid column values sanitizer
        /// </summary>
        public ISanitizer Sanitizer { get; set; }

        /// <summary>
        ///     Manage pager properties
        /// </summary>
        public IGridPager Pager
        {
            get { return _pager ?? (_pager = new GridPager()); }
            set { _pager = value; }
        }

        IGridColumnCollection IGrid.Columns
        {
            get { return Columns; }
        }

        public bool EnableAggregateOnFooter { get; set; }
        public bool EnableAggregateOnFooterByPage { get; set; }

        #endregion

        /// <summary>
        ///     Applies data annotations settings
        /// </summary>
        private void ApplyGridSettings()
        {
            GridTableAttribute opt = _annotaions.GetAnnotationForTable<T>();
            if (opt == null) return;
            EnablePaging = opt.PagingEnabled;
            if (opt.PageSize > 0)
                Pager.PageSize = opt.PageSize;

            if (opt.PagingMaxDisplayedPages > 0 && Pager is GridPager)
            {
                (Pager as GridPager).MaxDisplayedPages = opt.PagingMaxDisplayedPages;
            }
        }

        /// <summary>
        ///     Methods returns items that will need to be displayed
        /// </summary>
        protected internal virtual IEnumerable<T> GetItemsToDisplay()
        {
            PrepareItemsToDisplay();
            if (EnableAggregateOnFooter || EnableAggregateOnFooterByPage)
                AggregateFields();
            return AfterItems;
        }


        /// <summary>
        ///     Generates columns for all properties of the model
        /// </summary>
        public virtual void AutoGenerateColumns()
        {
            //TODO add support order property
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanRead)
                    Columns.Add(pi);
            }
        }

        /// <summary>
        ///     Fill AggregateResult of the columns
        /// </summary>
        protected internal virtual void AggregateFields()
        {
            if (this.GridItems.Any())
            {
                foreach (IGridColumn column in this.Columns)
                {
                    double value = 0.0;
                    double valuedisplaying = 0.0;

                    if (column.ColumnType == typeof(int) || column.ColumnType == typeof(decimal) || column.ColumnType == typeof(double))
                    {
                        if (column.AggregateValue != GridMvc.Sorting.GridAggregateFunction.None)
                        {
                            foreach (object item in this.GridItems)
                            {

                                var cellvalue = column.GetCell(item);
                                if (!String.IsNullOrEmpty(cellvalue.Value))
                                {
                                    value += Convert.ToDouble(cellvalue.Value);
                                    if (AfterItems.Contains(item))
                                        valuedisplaying += Convert.ToDouble(cellvalue.Value);
                                }

                            }

                            column.AggregateResult = new GridMvc.GridCell(String.Format("{0:0.00}", value / (column.AggregateValue == GridAggregateFunction.Sum ? 1 : ItemsCount)));
                            column.PageAggregateResult = new GridMvc.GridCell(String.Format("{0:0.00}", valuedisplaying / (column.AggregateValue == GridAggregateFunction.Sum ? 1 : AfterItems.Count())));

                        }

                    }

                }
            }

        }



    }


}
