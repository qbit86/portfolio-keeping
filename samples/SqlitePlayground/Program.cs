using System;
using System.Globalization;
using System.IO;
using Microsoft.Data.Sqlite;

[assembly: CLSCompliant(true)]

namespace Diversifolio
{
    internal static class Program
    {
        private static CultureInfo F => CultureInfo.InvariantCulture;

        private static void Main()
        {
            string dateString = DateTime.Now.ToString("yyyy-MM-dd", F);
            string dirName = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SqlitePlayground");
            Directory.CreateDirectory(dirName);
            Console.WriteLine($"{nameof(dirName)}: {dirName}");
            string baseName = $"SqlitePlayground-{dateString}.db";
            string path = Path.Join(dirName, baseName);

            SqliteConnectionStringBuilder connectionStringBuilder = new()
            {
                DataSource = path,
                Mode = SqliteOpenMode.ReadWriteCreate
            };
            string connectionString = connectionStringBuilder.ToString();
            using SqliteConnection connection = new(connectionString);
            connection.Open();
        }
    }
}
