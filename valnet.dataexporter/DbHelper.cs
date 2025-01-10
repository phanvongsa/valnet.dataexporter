using System.Data;
using System.Text;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace valnet.dataexporter;

public class DbHelper
{
    private string connectionString;
    //private OracleConnection connection;
    private const string sql_delim = "/";
    private IConfigurationRoot config;
    public DbHelper(IConfigurationRoot __config) {
        this.config = __config;
        this.connectionString = this.config["connectionString"];
    }
    
    
    public DataTable getDatabaseTables(){
        string sql = "SELECT table_name, tablespace_name  FROM user_tables";
        return executeQuery(CommandType.Text, sql);
    }

    public string generateScriptTableData(string tableName) {
        StringBuilder sbsql = new StringBuilder();
        // SQL query to fetch data
        string sql = String.Format("SELECT * FROM {0} WHERE ROWNUM <= {1}",tableName, config["max-data-row"]);
        DataTable q = executeQuery(CommandType.Text,sql);
        DataColumnCollection columns = q.Columns;
        
        sbsql.AppendLine("--- "+tableName+ "("+q.Rows.Count+")");
        string ins_def_into = "";
        string ins_def_vals = "";
        foreach (DataColumn column in columns) {
            ins_def_into += ", "+column.ColumnName;
            ins_def_vals += ", {"+column.ColumnName+"}";
        }
        
        ins_def_into = ins_def_into.Substring(1).Trim();
        ins_def_vals = ins_def_vals.Substring(1).Trim();
        string ins_def_statement = $"INSERT INTO {tableName} ({ins_def_into}) VALUES ({ins_def_vals});";
        foreach (DataRow row in q.Rows) {
            String ins_statement = ins_def_statement;
            foreach (DataColumn column in columns) {
                ins_statement = ins_statement.Replace("{" + column.ColumnName + "}", generateSQLStringValue(row[column], column.DataType));
            }
            sbsql.AppendLine(ins_statement);
        }
        sbsql.AppendLine(sql_delim);
        return sbsql.ToString();
    }

    private string generateSQLStringValue(object value, Type columnType) {
        if(value==DBNull.Value)
            return "NULL";
        else if (columnType == typeof(string) || columnType == typeof(char))
            return $"'{value.ToString().Replace("'", "''")}'";
        else if (columnType == typeof(DateTime))
            return $"TO_DATE('{((DateTime)value):yyyy-MM-dd HH:mm:ss}', 'YYYY-MM-DD HH24:MI:SS')";
        else 
            return value.ToString();
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
        string create_sql = executeScaler(sql).Replace(" ENABLE", "");
        int segmentIndex = create_sql.IndexOf("SEGMENT CREATION IMMEDIATE");
        if (segmentIndex >= 0)
            create_sql = create_sql.Substring(0, segmentIndex);
        create_sql = "  EXECUTE IMMEDIATE '" + create_sql.Replace("'","''") + "';";
        return create_sql;
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
        sbsql.AppendLine("  -- Get the count of the table");
        sbsql.AppendFormat("    SELECT COUNT(*) INTO table_count FROM user_tables WHERE table_name = '{0}';\n",tableName);
        sbsql.AppendLine("  -- Check if the table exists");
        sbsql.AppendLine("  IF table_count > 0 THEN");
        sbsql.AppendFormat("    EXECUTE IMMEDIATE 'DROP TABLE {0}';\n", tableName);
        sbsql.AppendLine("  END IF;");
        return sbsql.ToString();
    }
}