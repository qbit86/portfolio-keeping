using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

[assembly: CLSCompliant(true)]

namespace Diversifolio
{
    internal static class Program
    {
        private static CultureInfo P => CultureInfo.InvariantCulture;

        private static async Task Main()
        {
            string dateString = DateTime.Now.ToString("yyyy-MM-dd", P);
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
            await using SqliteConnection connection = new(connectionString);
            connection.Open();

            var assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();
            Console.WriteLine($"{nameof(resourceNames)}: {string.Join(", ", resourceNames)}");

            await using Stream stream = assembly.GetManifestResourceStream(typeof(Program), "CreatePosition.sql")!;
            using StreamReader reader = new(stream);
            string commandText = await reader.ReadToEndAsync().ConfigureAwait(false);
            using SqliteCommand command = new(commandText, connection);
            command.ExecuteReader();
        }
    }
}
