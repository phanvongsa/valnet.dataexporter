using System.Data;
using System.Text;
using Microsoft.Extensions.Configuration;
using valnet.dataexporter;

// This is a simple minimal console app without explicit class declaration
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "debug";

// Build the configuration based on environment
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true);

var config = builder.Build();
string output_file_path = string.Format(@"{0}tables.sql",config["output_folder"]);
// Console.WriteLine($"env: {config["env"]}");
// Console.WriteLine($"connectionString: {config["connectionString"]}");
DbHelper dbhelper = new DbHelper(config["connectionString"]);
DataTable qTables = dbhelper.getDatabaseTables();
StringBuilder sboutput = new StringBuilder();
sboutput.AppendFormat("-- Generated @ {0}\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
foreach (DataRow row in qTables.Rows) {
    sboutput.AppendLine(dbhelper.generateScriptTable(row["table_name"].ToString()));
}
File.WriteAllText(output_file_path, sboutput.ToString());
Console.WriteLine("Generated file: {0}",output_file_path);