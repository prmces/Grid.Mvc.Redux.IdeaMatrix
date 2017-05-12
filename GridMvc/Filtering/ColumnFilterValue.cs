using System.Runtime.Serialization;
using System.Web;

namespace GridMvc.Filtering
{
    /// <summary>
    ///     Structure that specifies filter settings for each column
    /// </summary>
    [DataContract]
    public struct ColumnFilterValue
    {
        //[DataMember(Name = "columnName")]
        public string ColumnName;

        [DataMember(Name = "filterType")]
        public GridFilterType FilterType;

        public string FilterValue;

        [DataMember(Name = "filterValue")]
        internal string FilterValueEncoded
        {
            get { return HttpUtility.UrlEncode(FilterValue); }
            set { FilterValue = value; }
        }

        public static ColumnFilterValue Null
        {
            get { return default(ColumnFilterValue); }
        }

        public static bool operator ==(ColumnFilterValue a, ColumnFilterValue b)
        {
            return a.ColumnName == b.ColumnName && a.FilterType == b.FilterType && a.FilterValue == b.FilterValue;
        }

        public static bool operator !=(ColumnFilterValue a, ColumnFilterValue b)
        {
            return a.ColumnName != b.ColumnName || a.FilterType != b.FilterType || a.FilterValue != b.FilterValue;
        }


        public override bool Equals(object obj)
        {

            if (!(obj is ColumnFilterValue))
                return false;

            ColumnFilterValue compareObject = (ColumnFilterValue)obj;

            return compareObject.ColumnName == ColumnName
                   && compareObject.FilterType == FilterType
                   && compareObject.FilterValue == FilterValue;

        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + ColumnName.GetHashCode();
            hash = (hash * 7) + FilterType.GetHashCode();
            hash = (hash * 7) + FilterValue.GetHashCode();

            return hash;
        }

    }
}