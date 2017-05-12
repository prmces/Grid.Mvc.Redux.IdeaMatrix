using System;
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GridMvc.Filtering.Types
{
    /// <summary>
    ///     Object contains some logic for filtering DateTime columns
    /// </summary>
    internal sealed class DateTimeFilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof(DateTime); }
        }

        public override Expression GetFilterExpression(Expression leftExpr, string value, GridFilterType filterType)
        {

            var ci = new CultureInfo("en-US");
            var formats = new[] { "M-d-yyyy", "dd-MM-yyyy", "MM-dd-yyyy", "M.d.yyyy", "dd.MM.yyyy", "MM.dd.yyyy", "M/d/yyyy", "dd/MM/yyyy", "MM/dd/yyyy" }
                    .Union(ci.DateTimeFormat.GetAllDateTimePatterns()).ToArray();

            if (filterType == GridFilterType.Equals)
            {
                GetValidType(filterType);
                object typedValue = GetTypedValue(value);
                if (typedValue == null)
                    return null; //incorrent filter value;

                Expression valueExpr = Expression.Constant(typedValue);
                var newLeftExp = Expression.GreaterThanOrEqual(leftExpr, valueExpr);
                DateTime nextDay = DateTime.ParseExact(value, formats, ci, DateTimeStyles.AssumeLocal).AddDays(1);
                var dayAfterExp = Expression.Constant(nextDay);
                var newRightExp = Expression.LessThan(leftExpr, dayAfterExp);
                return Expression.And(newLeftExp, newRightExp);
            }

            return base.GetFilterExpression(leftExpr, value, filterType);
        }

        /// <summary>
        ///     There are filter types that allowed for DateTime column
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override GridFilterType GetValidType(GridFilterType type)
        {
            switch (type)
            {
                case GridFilterType.Equals:
                case GridFilterType.GreaterThan:
                case GridFilterType.GreaterThanOrEquals:
                case GridFilterType.LessThan:
                case GridFilterType.LessThanOrEquals:
                    return type;
                default:
                    return GridFilterType.Equals;
            }
        }

        public override object GetTypedValue(string value)
        {
            DateTime date;
            if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return null;
            return date;
        }
    }
}