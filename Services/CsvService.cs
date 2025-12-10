using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace CSVReader.Services
{
    /// <summary>
    /// Service that handles reading and parsing CSV files
    /// </summary>
    public class CsvService
    {
        /// <summary>
        /// Reads CSV file and converts it into a DataTable with proper columns
        /// </summary>
        public DataTable LoadCsvToDataTable(string path)
        {
            if (string.IsNullOrEmpty(path)) 
                throw new ArgumentNullException(nameof(path));
            
            var dt = new DataTable();
            var allRows = new List<string[]>();

            using (var sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) 
                        continue;
                    
                    var parts = line.Split(',')
                        .Select(p => p.Trim() ?? string.Empty)
                        .ToArray();
                    allRows.Add(parts);
                }
            }

            if (allRows.Count == 0)
                return dt;

            int lastNonEmptyCol = -1;
            foreach (var parts in allRows)
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(parts[i]))
                    {
                        if (i > lastNonEmptyCol) 
                            lastNonEmptyCol = i;
                    }
                }
            }

            int columns = Math.Max(1, lastNonEmptyCol + 1);
       
            for (int i = 0; i < columns; i++)
            {
                dt.Columns.Add($"Column {i + 1}", typeof(string));
            }

            foreach (var parts in allRows)
            {
                var row = dt.NewRow();
                for (int i = 0; i < columns; i++)
                {
                    row[i] = i < parts.Length ? parts[i] : string.Empty;
                }
                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}
