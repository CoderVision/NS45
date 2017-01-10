using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace NtccSteward.Repository.Framework
{
    public static class DataExtensions
    {
        /// <summary>
        /// Returns the value from the specified field; if null, returns the specified default value.
        /// </summary>
        /// <typeparam name="T"> The type of the data to return</typeparam>
        /// <param name="reader"> IDataReader instance</param>
        /// <param name="columnName"> The name of the column to retreive data for</param>
        /// <param name="defaultValue"> The value to be returned if the column value is null</param>
        /// <returns> The value of a column, or the default value if null </returns>
        public static T ValueOrDefault<T>(this SqlDataReader reader, string columnName, T defaultValue)
        {
            // DBNull.Value created an error, because it's in mscorlib, which isn't referenced.
            return (reader[columnName].ToString() != string.Empty ? (T)reader[columnName] : defaultValue);
        }


        /// <summary>
        /// Returns the value from the specified field; if null, returns the specified default value.
        /// </summary>
        /// <typeparam name="T"> The type of the data to return</typeparam>
        /// <param name="reader"> IDataReader instance</param>
        /// <param name="columnIndex"> The index of the column to retreive data for</param>
        /// <param name="defaultValue"> The value to be returned if the column value is null</param>
        /// <returns> The value of a column, or the default value if null </returns>
        public static T ValueOrDefault<T>(this SqlDataReader reader, int columnIndex, T defaultValue)
        {
            var columnName = reader.GetName(columnIndex);

            return ValueOrDefault<T>(reader, columnName, default(T));
        }


        /// <summary>
        /// Returns the value from the specified field; if null, returns the specified default value.
        /// </summary>
        /// <typeparam name="T"> The type of the data to return</typeparam>
        /// <param name="reader"> IDataReader instance</param>
        /// <param name="columnName"> The name of the column to retreive data for</param>
        /// <returns> The value of a column, or the default value if null </returns>
        public static T ValueOrDefault<T>(this SqlDataReader reader, string columnName)
        {
            return ValueOrDefault<T>(reader, columnName, default(T));
        }


        /// <summary>
        /// Returns the value from the specified field; if null, returns the specified default value.
        /// </summary>
        /// <typeparam name="T"> The type of the data to return</typeparam>
        /// <param name="reader"> IDataReader instance</param>
        /// <param name="columnIndex"> The index of the column to retreive data for</param>
        /// <returns> The value of a column, or the default value if null </returns>
        public static T ValueOrDefault<T>(this SqlDataReader reader, int columnIndex)
        {
            return ValueOrDefault<T>(reader, columnIndex, default(T));
        }

        public static SqlDateTime ToSqlDateTime(this DateTime date)
        {
            SqlDateTime returnDate = SqlDateTime.Null;

            var minDate = new DateTime(SqlDateTime.MinValue.TimeTicks);
            var maxDate = new DateTime(SqlDateTime.MaxValue.TimeTicks);

            if (date > minDate && date < maxDate)
                returnDate = new SqlDateTime(date);


            return returnDate;
        }

        public static SqlString ToSqlString(this string value)
        {
            SqlString returnValue = SqlString.Null;

            if (!string.IsNullOrWhiteSpace(value))
                returnValue = new SqlString(value);

            return returnValue;
        }
    }
}
