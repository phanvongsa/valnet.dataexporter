using System.Data;
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
Console.WriteLine($"env: {config["env"]}");
Console.WriteLine($"connectionString: {config["connectionString"]}");
DbHelper dbhelper = new DbHelper(config["connectionString"]);
Console.WriteLine(dbhelper.generateScriptTable("LOGS"));
// DataTable qTables = dbhelper.getDatabaseTables();//executeQuery(CommandType.Text,"SELECT table_name, tablespace_name  FROM user_tables");
// foreach (DataRow row in qTables.Rows) {
//     Console.WriteLine(dbhelper.generateScriptTable(row["table_name"].ToString()));
//     break;
// //    Console.WriteLine($"Table: {row["table_name"]}, Tablespace: {row["tablespace_name"]}");
// }

// string tableName = "VN_PROPERTY";
// Console.WriteLine(dbhelper.generateScriptTable(tableName));

// //var connectionString = "User Id=VALNET;Password=VALNET;Data Source=//oracle-host-11g:1521/XE";
// var connectionString = "User Id=VALNET;Password=VALNET;Data Source=//localhost:1521/XE";
// string tableName = "VN_PROPERTY";
//

//
// dbhelper.testConnection();
// Console.WriteLine(dbhelper.generateScriptTable(tableName));

// Generate Create or Update Table Script
//string script = GenerateCreateOrUpdateTableScript(connectionString, tableName);
//Console.WriteLine(script);


// Console.WriteLine("Initialising Connection");
// //var connectionString = "User Id=VALNET;Password=VALNET;Data Source=//oracle-host-11g:1521/XE";
// var connectionString = "User Id=VALNET;Password=VALNET;Data Source=//localhost:1520/XE";
// using (var conn = new OracleConnection(connectionString)) {
//     try
//     {
//         // Open connection to the database
//         conn.Open();
//         Console.WriteLine("Connected to Oracle Database");
//
//         // Define the SQL query to fetch tables
//         string sqlQuery = "SELECT table_name FROM user_tables";
//
//         // Create an Oracle command
//         using (OracleCommand cmd = new OracleCommand(sqlQuery, conn))
//         {
//             // Execute the query and get the result
//             using (OracleDataReader reader = cmd.ExecuteReader())
//             {
//                 Console.WriteLine("Tables in the database:");
//
//                 // Loop through all tables and display their names
//                 while (reader.Read())
//                 {
//                     Console.WriteLine(reader.GetString(0)); // Print table name
//                     break;
//                 }
//             }
//         }
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine("Error: " + ex.Message);
//     }
// }

