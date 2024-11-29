using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace valnet.dataexporter;

public class DbHelper
{
    private string connectionString;
    private OracleConnection connection;
    private const string sql_delim = "/";
    public DbHelper(string connectionString)
    {
        this.connectionString = connectionString;
        this.connection = new OracleConnection(connectionString);
    }

    public string generateScriptTable(string tableName) {
        StringBuilder sbsql = new StringBuilder(generateScriptDropTable(tableName));
        // TODO build table
        return sbsql.ToString();
    }

    private string generateScriptDropTable(string tableName)
    {
        StringBuilder sbsql = new StringBuilder();
        sbsql.AppendLine("BEGIN");
        sbsql.AppendFormat("  -- Drop the {0} table if it exists\n", tableName);
        sbsql.AppendFormat("  IF EXISTS (SELECT 1 FROM user_tables WHERE table_name = '{0}') THEN\n",tableName);
        sbsql.AppendFormat("      EXECUTE IMMEDIATE 'DROP TABLE {0}';\n", tableName);
        sbsql.AppendLine("  END IF;");
        sbsql.AppendLine("END;");
        sbsql.AppendLine(sql_delim);
        return sbsql.ToString();
    }
}