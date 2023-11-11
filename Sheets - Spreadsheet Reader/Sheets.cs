using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace swzwij.Sheets
{
    /// <summary>
    /// A utility class for reading and processing CSV data.
    /// </summary>
    public static class Sheets
    {
        #region Private Variables

        private const string COLLUM_REGEX = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        private const string ROW_REGEX = @"\r\n|\n\r|\n|\r";
        private static readonly char[] TRIM_CHARS = { '\"' };

        #endregion

        #region Public Function

        /// <summary>
        /// Reads a CSV file and returns its data as a list of dictionaries.
        /// </summary>
        /// <param name="fileName">The name of the CSV file to read.</param>
        /// <returns>A list of dictionaries representing the CSV data.</returns>
        public static List<Dictionary<string, object>> Read(string fileName)
        {
            List<Dictionary<string, object>> csvData = new();
            TextAsset csvFile = Resources.Load(fileName) as TextAsset;

            if (csvFile == null)
                return csvData;

            string[] rows = SplitRows(csvFile.text);

            if (rows.Length <= 1)
                return csvData;

            string[] headers = SplitColumns(rows[0]);

            for (int i = 1; i < rows.Length; i++)
            {
                string[] values = SplitColumns(rows[i]);

                if (values.Length == 0 || string.IsNullOrEmpty(values[0]))
                    continue;

                Dictionary<string, object> entry = CreateEntry(headers, values);

                csvData.Add(entry);
            }

            return csvData;
        }

        /// <summary>
        /// Retrieves the value from a specified CSV file at the given row index and under the specified header.
        /// </summary>
        /// <param name="fileName">The name of the CSV file to read.</param>
        /// <param name="headerName">The header under which the value is located.</param>
        /// <param name="index">The row index for which the value is requested.</param>
        /// <returns>The value in the specified CSV file at the given row index and under the specified header. 
        /// Returns null if the file, row, or header is not found.</returns>
        public static object GetValue(string fileName, string headerName, int index)
        {
            List<Dictionary<string, object>> csvData = Read(fileName);
            int newIndex = index - 2;

            if (newIndex >= csvData.Count || newIndex < 0)
                return ReturnError($"{fileName} does not contain the requested row at index {index}");

            Dictionary<string, object> csvColumn = csvData[newIndex];

            if (!csvColumn.ContainsKey(headerName))
                return ReturnError($"{fileName} does not contain the specified header: {headerName}");

            return csvColumn[headerName];
        }

        /// <summary>
        /// Retrieves the values of a specific row from a CSV file.
        /// </summary>
        /// <param name="fileName">The name of the CSV file.</param>
        /// <param name="index">The index of the row to retrieve (1-based).</param>
        /// <returns>An array containing the values of the specified row, or an empty array if the index is out of bounds.</returns>
        public static object[] GetRowValues(string fileName, int index) 
            => Read(fileName).ElementAtOrDefault(index - 2)?.Values.ToArray() ?? Array.Empty<object>();

        /// <summary>
        /// Retrieves the values of a specific column (header) from a CSV file parsed into a collection of dictionaries.
        /// </summary>
        /// <param name="fileName">The name of the CSV file.</param>
        /// <param name="headerName">The name of the header (column) to retrieve.</param>
        /// <returns>An array containing the values of the specified column, or an array of null values if the header is not found.</returns>
        public static object[] GetCollumValues(string fileName, string headerName) 
            => Read(fileName).Select(collum => collum.ContainsKey(headerName) ? collum[headerName] : null).ToArray();

        #endregion

        #region Private Functions

        /// <summary>
        /// Splits the input text into an array of rows based on newline characters.
        /// </summary>
        /// <param name="text">The input text to be split into rows.</param>
        /// <returns>An array of rows.</returns>
        private static string[] SplitRows(string text) => Regex.Split(text, ROW_REGEX);

        /// <summary>
        /// Splits a row into an array of columns based on the specified column separator regex.
        /// </summary>
        /// <param name="row">The row to be split into columns.</param>
        /// <returns>An array of columns.</returns>
        private static string[] SplitColumns(string row) => Regex.Split(row, COLLUM_REGEX);

        /// <summary>
        /// Creates a dictionary entry from the provided headers and values.
        /// </summary>
        /// <param name="headers">The headers representing column names.</param>
        /// <param name="values">The values representing a row of data.</param>
        /// <returns>A dictionary entry representing a row of CSV data.</returns>
        private static Dictionary<string, object> CreateEntry(string[] headers, string[] values)
        {
            Dictionary<string, object> entry = new();

            for (int j = 0; j < Mathf.Min(headers.Length, values.Length); j++)
                entry[headers[j]] = ParseValue(values[j]);

            return entry;
        }

        /// <summary>
        /// Parses a string value, trimming whitespaces and converting to appropriate data types.
        /// </summary>
        /// <param name="value">The string value to be parsed.</param>
        /// <returns>The parsed object value.</returns>
        private static object ParseValue(string value)
        {
            string trimmedValue = value?.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

            object finalValue = int.TryParse(trimmedValue, out int integerValue)
                ? integerValue
                : float.TryParse(trimmedValue, out float floatValue)
                ? floatValue
                : trimmedValue;

            return finalValue;
        }

        /// <summary>
        /// Logs an error message and returns null.
        /// </summary>
        /// <param name="message">The error message to be logged.</param>
        /// <returns>Returns null after logging the specified error message using Debug.LogError.</returns>
        private static object ReturnError(string message)
        {
            Debug.LogError(message);
            return null;
        }

        #endregion
    }
}