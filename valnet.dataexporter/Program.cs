using System;
using Oracle.ManagedDataAccess.Client;
using System.Text;
using valnet.dataexporter;

// //var connectionString = "User Id=VALNET;Password=VALNET;Data Source=//oracle-host-11g:1521/XE";
var connectionString = "User Id=VALNET;Password=VALNET;Data Source=//localhost:1520/XE";
string tableName = "VN_PROPERTY";

DbHelper dbhelper = new DbHelper(connectionString);
Console.WriteLine(dbhelper.generateScriptTable(tableName));

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

