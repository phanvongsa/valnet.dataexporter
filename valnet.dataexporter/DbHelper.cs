using System.Data;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace valnet.dataexporter;

public class DbHelper
{
    private string connectionString;
    //private OracleConnection connection;
    private const string sql_delim = "/";
    public DbHelper(string connectionString) {
        this.connectionString = connectionString;
    }
    
    public DataTable getDatabaseTables(){
        string sql = "SELECT table_name, tablespace_name  FROM user_tables";
        return executeQuery(CommandType.Text, sql);
    }
    public string generateScriptTable(string tableName) {
        StringBuilder sbsql = new StringBuilder();
        sbsql.AppendLine("DECLARE");
        sbsql.AppendLine("  table_count NUMBER;");
        sbsql.AppendLine("BEGIN");
        sbsql.AppendLine(generateScriptDropTable(tableName));
        sbsql.AppendLine(generateScriptCreateTable(tableName));
        sbsql.AppendLine("END;");
        sbsql.AppendLine(sql_delim);
        return sbsql.ToString();
    }

    private string generateScriptCreateTable(string tableName) {
        string sql = string.Format("SELECT DBMS_METADATA.GET_DDL('TABLE', '{0}', 'VALNET') FROM dual",tableName);
        //string create_sql =  
        return executeScaler(sql).Replace(" ENABLE", "");
    }

    private string executeScaler(string sql) {
        using (var connection = new OracleConnection(this.connectionString)) {
            try {
                connection.Open();
                using (OracleCommand cmd = new OracleCommand(sql, connection)) {
                    return cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }
    }
    private DataTable executeQuery(CommandType commandType,string sql) {
        var dataTable = new DataTable();
        try {
            using (var connection = new OracleConnection(this.connectionString))
            {
                connection.Open();
                using (var command = new OracleCommand(sql, connection))
                {
                    using (var adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(dataTable); // Fill the DataTable with query results
                    }
                }
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        return dataTable;
    }
    
    

    public bool testConnection() {
        try {
            using (var connection = new OracleConnection(this.connectionString)) {
                // Open connection to the database
                connection.Open();
                Console.WriteLine("Connected to Oracle Database");
                return true;
            }
        } catch(Exception ex) {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private string generateScriptDropTable(string tableName)
    {
        StringBuilder sbsql = new StringBuilder();
        //sbsql.AppendLine("BEGIN");

        sbsql.AppendLine("-- Get the count of the table");
        sbsql.AppendFormat("SELECT COUNT(*) INTO table_count FROM user_tables WHERE table_name = '{0}';\n",tableName);
        sbsql.AppendLine("-- Check if the table exists");
        sbsql.AppendLine("IF table_count > 0 THEN");
        sbsql.AppendFormat("    EXECUTE IMMEDIATE 'DROP TABLE {0}';\n", tableName);
        sbsql.AppendLine(" END IF;");
        sbsql.AppendLine("EXCEPTION");
        sbsql.AppendLine("  WHEN NO_DATA_FOUND THEN");
        sbsql.AppendLine("-- If the table doesn't exist, do nothing");
        sbsql.AppendLine("  NULL;");
        
        // sbsql.AppendFormat("  -- Drop the {0} table if it exists\n", tableName);
        // sbsql.AppendFormat("  IF EXISTS (SELECT 1 FROM user_tables WHERE table_name = '{0}') THEN\n",tableName);
        // sbsql.AppendFormat("      EXECUTE IMMEDIATE 'DROP TABLE {0}';\n", tableName);
        // sbsql.AppendLine("  END IF;");
        //sbsql.AppendLine("END;");
        //sbsql.AppendLine(sql_delim);
        return sbsql.ToString();
    }
}