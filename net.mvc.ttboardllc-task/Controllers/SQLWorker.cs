using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data.Sql;
using System.EnterpriseServices;

public class SQLWorker
//сделать проверку на наличие базы на сервере либо указать имя базы по умолчанию
{
    public static SQLWorker instanse;

    public static SQLWorker getInstance(string connectionString)
    {
        ConnectionString = connectionString;
        return Instance;
    }

    public static SQLWorker Instance
    {
        get
        {
            if (instanse == null)
            {
                instanse = new SQLWorker(ConnectionString);
            }
            return instanse;
        }
    }
    //Предоставляет открытое подключение к базе данных SQL Server
    private SqlConnection connectionSQL;

    //статус соединения с базой
    private bool isConnected = false;

    //Параметры подключения к базе
    //private DBConnect ConnectionParam;
    private static string ConnectionString;

    private bool tLog;

    //private string ConStr;

    //Путь до Лог-файла
    //private string LogFilePath;

    //Объект для записи лога ошибок
    private LogWriter Logger;

    private const string errorConnect = "Ошибка при подключении к базе";
    private const string errorExecQuery = "Ошибка при выполнения запроса к базе: ";
    private const string errorBulkCopy = "Ошибка при копировании";


    public SQLWorker(string ConnectStr, string LogPath, LogWriterType type)
    {
        ConnectionString = ConnectStr;
        Logger = new LogWriter(LogPath, type);
        isConnected = Connect();
        tLog = true;
    }

    public SQLWorker(string ConnectStr)
    {
        ConnectionString = ConnectStr;
        isConnected = Connect();
        tLog = false;
    }

    //возвращает статус соединения с базой
    public bool Connected
    {
        get { return isConnected; }
    }

    //соединение с базой
    private bool Connect()
    {
        bool isConnected = false;
        try
        {
            //string connectionString     = GetConnectionString();
            connectionSQL = new SqlConnection(ConnectionString);
            connectionSQL.Open();
            isConnected = true;
        }
        catch (Exception e)
        {
            if (tLog)
            {
                isConnected = false;
                Logger.Write(e.ToString());
            }
            else
                throw new MessageException(errorConnect, e);
        }
        return isConnected;
    }

    //Функция преобразования типов C# в тип SQL Server 
    private string SQLType(string type)
    {
        string str = "";
        switch (type)
        {
            case "Boolean": str = "Bit"; break;
            case "Byte": str = "TinyInt"; break;
            case "Binary": str = "VarBinary(max)"; break;
            case "DateTime": str = "DateTime"; break;
            case "Decimal": str = "Decimal"; break;
            case "Double": str = "Float"; break;
            case "Single": str = "Real"; break;
            case "Guid": str = "UniqueIdentifier"; break;
            case "Int16": str = "SmallInt"; break;
            case "Int32": str = "int"; break;
            case "Int64": str = "BigInt"; break;
            case "Object": str = "Variant"; break;
            case "String": str = "NVarChar(max)"; break;
            case "Time": str = "Timestamp"; break;
            case "AnsiString": str = "VarChar(MAX)"; break;
            case "AnsiStringFixedLength": str = "Char(MAX)"; break;
            case "Currency": str = "Money"; break;
            case "StringFixedLength": str = "NChar(max)"; break;
            default: str = "VarChar(MAX)"; break;
        }
        return str;
    }

    //Создание строки фиктивного запроса к базе
    private static string CreateFixedQuery(string TableName)
    {
        return "select * from " + TableName + " where 1=0";
    }

    //Возвращает словарь соответствий имен столбцов типам столбцов
    private Dictionary<string, string> GetColumnTypes(string TableName)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        DataTable tbl = ExecQuery(CreateFixedQuery(TableName));
        if (tbl != null)
        {
            foreach (DataColumn col in tbl.Columns)
                result.Add(col.ColumnName, col.DataType.Name);
            return result;
        }
        return null;
    }

    //Возвращает словарь соответствий имен столбцов типам столбцов
    private Dictionary<string, int> GetColumnsIndexes(string TableName)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        DataTable tbl = ExecQuery(CreateFixedQuery(TableName));
        if (tbl != null)
        {
            int i = 0;
            foreach (DataColumn col in tbl.Columns)
            {
                result.Add(col.ColumnName, i);
                i++;
            }
            return result;
        }
        return null;
    }

    private int GetIndexOfTableColumn(string colName, string TableName)
    {
        Dictionary<string, int> dict = GetColumnsIndexes(TableName);
        return dict[colName];
    }

    //получение строки подключения
    //public string GetConnectionString() //переделать в private
    //{
    //    string ConString;
    //    ConString = "Data Source = "    + ConnectionParam.Server   + "; " +
    //                "Initial Catalog = " + ConnectionParam.DataBase + "; " ;
    //    if (ConnectionParam.IntegratedSecurity)
    //        ConString += "Integrated Security=SSPI;";
    //    else
    //        if (ConnectionParam.Trusted_Connection)
    //            ConString += "Trusted_Connection=True;";        
    //        else            
    //            ConString += "UserId="   + ConnectionParam.UserId   + "; " +
    //                         "Password=" + ConnectionParam.Password + ";";           
    //    return ConString;
    //}

    //выполнение произвольного запроса
    private DataTable ExecQuery(string QueryString)
    {
        DataTable SQLTable = new DataTable();
        SqlCommand SQLCommand = new SqlCommand(QueryString, connectionSQL);
        SqlDataAdapter SQLAdapter = new SqlDataAdapter(SQLCommand);
        try
        {
            SQLAdapter.Fill(SQLTable);
        }
        catch (Exception e)
        {
            if (tLog)
                Logger.Write(e.ToString());
            else
                throw new MessageException(errorExecQuery + QueryString, e);
        }
        return SQLTable;
    }

    //public void ExecuteStorageProcedure(string Name, string[] value)
    //{
    //    ExecWithoutResult(Name);
    //}

    //выполнение произвольного запроса
    public void ExecuteStorageProcedure(string Name, string[] paramNames, string[] parameters)
    {
        DataTable SQLTable = new DataTable();
        SqlCommand SQLCommand = new SqlCommand(Name, connectionSQL);
        SQLCommand.CommandType = CommandType.StoredProcedure;
        if (paramNames != null || parameters != null)
        {
            if (paramNames.Length != parameters.Length)
                throw new MessageException("Длинна массивов имен параметров и массива значений параметров должна быть одинаковой!");
            for (int i = 0; i < parameters.Length; i++)
                SQLCommand.Parameters.Add(new SqlParameter(paramNames[i], parameters[i]));
        }
        try
        {
            if (connectionSQL.State == ConnectionState.Closed)
                connectionSQL.Open();
            SQLCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            if (tLog)
                Logger.Write(e.ToString());
            else
                throw new MessageException(errorExecQuery + Name, e);
        }
    }

    //выполнение произвольного запроса
    private DataTable ExecQuery(string QueryString, SqlParameter[] parametrs)
    {
        DataTable SQLTable = new DataTable();
        SqlCommand SQLCommand = new SqlCommand(QueryString, connectionSQL);
        foreach (SqlParameter par in parametrs)
            SQLCommand.Parameters.Add(par);
        SqlDataAdapter SQLAdapter = new SqlDataAdapter(SQLCommand);
        try
        {
            SQLAdapter.Fill(SQLTable);
        }
        catch (Exception e)
        {
            if (tLog)
                Logger.Write(e.ToString());
            else
                throw new MessageException(errorExecQuery + QueryString, e);
        }
        return SQLTable;
    }

    //выполнение массового копирования в базу данных
    public void BulkCopy(DataTable Table)
    {
        SqlBulkCopy SBK = new SqlBulkCopy(ConnectionString);
        try
        {
            if (!ExistTable(Table.TableName))                        //Если существует таблица с таким именем в БД,
                CreateTable(Table.TableName, GetStringColumns(Table));
            SBK.DestinationTableName = Table.TableName;             //имя целевой таблицы на сервере
            var colIndexes = GetColumnsIndexes(Table.TableName);
            foreach (DataColumn column in Table.Columns)
            {
                if (colIndexes.ContainsKey(column.ColumnName))
                    SBK.ColumnMappings.Add(new SqlBulkCopyColumnMapping(column.ColumnName, colIndexes[column.ColumnName]));
            }
            //SqlBulkCopyColumnMapping cm = new SqlBulkCopyColumnMapping(
            //SBK.ColumnMappings.Add
            SBK.WriteToServer(Table);                               //производим массовое копирование в базу    
            SBK.Close();
        }
        catch (Exception e)
        {
            if (tLog)
                Logger.Write(e.ToString());
            else
                throw new MessageException(errorBulkCopy, e);
        }
    }

    //получение строки имен столбцов с их типом через запятую
    public string GetStringColumns(DataTable Table)
    {
        DataColumnCollection column = Table.Columns;
        int n = column.Count;
        //получение первой пары "Имя столбца" "тип данных"
        string str = column[0].ColumnName + " " +
                                    SQLType(column[0].DataType.ToString());
        //получение остальных пар "Имя столбца" "тип данных". сделано отдельно для нужного количество запятых
        for (int i = 1; i < n; i++)
        {
            str += ", " + column[i].ColumnName + " "
                        + SQLType(column[i].DataType.ToString());
        }
        return str;
    }

    //создание таблицы в SQL Server 
    public DataTable CreateTable(string TableName, string StringColumns)
    {
        return ExecQuery("create table " + TableName + "(" + StringColumns + ")");
    }

    ////вставка строки в БД
    //public void Insert(string TableName, string StringColumns, string[] Values)
    //{
    //    ExecQuery("insert into " + TableName + "(" + StringColumns + ") " + "values (" + Values + ")");
    //}

    private string[] parseColumns(string columns)
    {
        return columns.Replace(" ", "").Split(',');
    }

    private string parseStringColumns(string columns)
    {
        string result = string.Empty;
        string[] cols = parseColumns(columns);
        foreach (string col in cols)
            result += "@" + col + ", ";
        return result.Substring(0, result.Length - 2);
    }

    private string parseStringColumns(string[] columns)
    {
        string result = string.Empty;
        foreach (string col in columns)
            result += "@" + col + ", ";
        return result.Substring(0, result.Length - 2);
    }

    private SqlParameter[] CreateSQLParametersToInsert(string[] columns, string[] values)
    {
        if (columns.Length != values.Length)
            return null;
        List<SqlParameter> parameters = new List<SqlParameter>();
        for (int i = 0; i < columns.Length; i++)
        {
            parameters.Add(new SqlParameter("@" + columns[i], values[i]));
        }
        return parameters.ToArray();
    }

    //вставка строки в БД
    public void Insert(string TableName, string StringColumns, string[] Values)
    {
        ExecQuery("insert into " + TableName + "(" + StringColumns + ") " + "values (" + parseStringColumns(StringColumns) + ")", CreateSQLParametersToInsert(parseColumns(StringColumns), Values));
    }

    //вставка строки в БД
    public void Insert(string TableName, string StringColumns, string Values)
    {
        ExecQuery("insert into " + TableName + "(" + StringColumns + ") " + "values (" + Values + ")");
    }

    //------------------ Вставка ----------------------------------------------------------------------
    public void Insert(string TableName, string[] StringColumns, string[] Values)
    {
        ExecQuery("insert into " + TableName + "(" + ForColumn(StringColumns) + ") " + "values (" + parseStringColumns(StringColumns) + ")", CreateSQLParametersToInsert(StringColumns, Values));
    }
    //------------------ Вставка ----------------------------------------------------------------------

    //Обновление 
    public void Update(string TableName, string StringName, string values, string where)        // принимает 3 параметра: имя таблицы, имя столбца, новое значение столбца, условие
    {
        if (where != String.Empty)                                                               //если строка условия не пустая,то...
            ExecQuery("UPDATE " + TableName + " SET " + StringName + "='" + values + "' WHERE " + where);//...проверяем его
        else
            ExecQuery("UPDATE " + TableName + " SET " + StringName + "='" + values + "'");                //иначе делаем запрос без условия

    }

    //Обновление 
    public void Update(string TableName, string[] StringName, string[] values, string[] whereCol, string[] whereValues)        // принимает 3 параметра: имя таблицы, имя столбца, новое значение столбца, условие
    {
        string where = CreateWhere(whereCol, whereValues);
        string set = CreateSet(StringName, values);
        if (where != String.Empty)                                              //если строка условия не пустая,то...
            ExecQuery("UPDATE " + TableName + " SET " + set + " WHERE " + where);    //...проверяем его
        else if (set != String.Empty)
            ExecQuery("UPDATE " + TableName + " SET " + set);                      //иначе делаем запрос без условия

    }
    //----------------метод для формирования  values с запятыми для selecta--------------------------
    private string ForColumn(string[] columns)
    {
        string column = string.Empty;
        string value = string.Empty;
        for (int i = 0; i < columns.Length - 1; i++)
            column += columns[i] + ", ";
        column += columns[columns.Length - 1];
        return column;
    }
    //----------------метод для формирования  values с запятыми для selecta--------------------------

    //----------------метод для формирования  values с запятыми для selecta--------------------------
    private string ForValues(string[] columns)
    {
        string column = string.Empty;
        //string value = string.Empty;
        for (int i = 0; i < columns.Length - 1; i++)
            column += "'" + columns[i] + "', ";
        column += "'" + columns[columns.Length - 1] + "'";
        return column;
    }
    //----------------метод для формирования  values с запятыми для selecta--------------------------

    //----------------метод для формирования  where с запятыми и AND для selecta--------------------------

    private string ForWhere(string[] columns, string[] wheres)
    {
        string where = string.Empty;

        if (wheres.Length == 0)
            return string.Empty;

        for (int i = 0; i < wheres.Length - 1; i++)
            where += columns[i] + "=" + wheres[i] + " AND ";
        where += columns[wheres.Length - 1] + "=" + wheres[wheres.Length - 1];

        return where;
    }
    //----------------метод для формирования  where с запятыми и AND для selecta--------------------------

    private string CreateWhereToSelect(string[] colWhere, string[] typeWhere)
    {
        string result = string.Empty;
        for (int i = 0; i < colWhere.Length; i++)
        {
            if (typeWhere[i] == "LIKE")
                result += colWhere[i] + " " + typeWhere[i] + " '%' + @" + colWhere[i] + " + '%' AND ";
            else
                result += colWhere[i] + "=@" + colWhere[i] + " AND ";
        }
        if (result != string.Empty)
            result = result.Substring(0, result.Length - 5);
        return result;
    }

    private SqlParameter[] CreateParametersToSelect(string[] colWhere, string[] valWhere)
    {
        List<SqlParameter> parameters = new List<SqlParameter>();
        for (int i = 0; i < colWhere.Length; i++)
        {
            SqlParameter param = new SqlParameter("@" + colWhere[i], valWhere[i]);
            parameters.Add(param);
        }
        return parameters.ToArray();
    }

    public DataTable Select(string tableName, string value, string[] columnsWhere, string[] typeWhere, string[] wheres)
    {
        string where = CreateWhereToSelect(columnsWhere, typeWhere);
        SqlParameter[] par = CreateParametersToSelect(columnsWhere, wheres);
        if (where != string.Empty)
            return ExecQuery("select " + value + " from " + tableName + " where " + where, par);
        else
            return ExecQuery("select " + value + " from " + tableName);

        //return Select(tableName, ForColumn(columns), ForWhere(columns, wheres));
    }


    /// <summary>
    /// Select-запрос, использует предыдушие методы,работает с массивами
    /// </summary>
    /// <param name="tableName">имя таблицы</param>
    /// <param name="columns">массив имен колонок</param>
    /// <param name="wheres">массив условий</param>
    /// <returns>результат выполнения запроса</returns>
    public DataTable Select(string tableName, string[] columns, string[] wheres)
    {
        return Select(tableName, ForColumn(columns), ForWhere(columns, wheres));
    }
    //------------------вывод Select, использует готовые строки для запроса-----------------------------------------------
    public DataTable Select(string Table, string Values, string Where)                        //принимает три параметра:имя таблицы, имя столбца и строчку для условия Where
    {
        if (Table == String.Empty)                                                            //имя таблицы должно быть не пустым
            throw new Exception("Функция Select. Table должны быть не пустыми");
        else if (Values == String.Empty)                                                     //столбцы должны быть не пустыми
            throw new Exception("Функция Select. Values должны быть не пустыми");
        else if (Where != String.Empty)                                                       //если строка условия не пустая,то...
            return ExecQuery("SELECT " + Values + " FROM " + Table + " Where " + Where);      //...проверяем его
        else
            return ExecQuery("SELECT " + Values + " FROM " + Table);                        //иначе делаем запрос без условия
    }
    //------------------вывод Select, использует готовые строки для запроса-----------------------------------------------
    //------------------ Выполнение любого запроса ----------------------------------------------------------------------
    public void Exec(string procedureName, string[] values)
    {
        if(values != null)
            ExecQuery("exec " + procedureName + " " + ForValues(values));
        else
            ExecQuery("exec " + procedureName);
    }
    //------------------ Выполнение любого запроса ----------------------------------------------------------------------

    //идентификатор наличия таблицы в БД
    public bool ExistTable(string TableName)
    {
        bool ExistTbl;
        DataTable Tbl = ExecQuery(QueryStrExistTable(TableName));
        if (Tbl.Rows.Count == 0)
            ExistTbl = false;
        else
            ExistTbl = true;
        return ExistTbl;
    }

    //построение строки запроса для установления наличия таблицы в БД
    private string QueryStrExistTable(string TableName)
    {
        return "SELECT name FROM dbo.sysobjects where name='" + TableName + "'";
    }

    //Удаляет все строки в таблице, не записывая в журнал удаление отдельных строк. 
    public DataTable Truncate(string TableName)
    {
        return ExecQuery("TRUNCATE TABLE " + TableName);
    }
    //-----------------------Метод для формирования условия where------------------------------
    private string CreateWhere(string[] whereCol, string[] whereValues)
    {
        string strQuery = string.Empty;
        if (whereCol.Length != 0 && whereCol.Length == whereValues.Length)
        {
            for (int i = 0; i < whereCol.Length; i++)
                strQuery += whereCol[i] + " = '" + whereValues[i] + "' AND ";
            strQuery = strQuery.Substring(0, strQuery.Length - 5);
        }
        return strQuery;
    }
    //-----------------------Метод для формирования условия where------------------------------
    //------------------------Метод для формирования перечисления столбцов------------------------
    private string CreateSet(string[] setCol, string[] setValues)
    {
        string strQuery = string.Empty;
        if (setCol.Length != 0 && setCol.Length == setValues.Length)
        {
            for (int i = 0; i < setCol.Length; i++)
                strQuery += setCol[i] + " = N'" + setValues[i] + "', ";
            strQuery = strQuery.Substring(0, strQuery.Length - 2);
        }
        return strQuery;
    }
    //------------------------Метод для формирования перечисления столбцов------------------------

    //------------------------Удаление----------------------------------------------------
    public void Delete(string TableName, string where)
    {
        if (where != string.Empty)
            ExecQuery("delete from " + TableName + " where " + where);
        else
            ExecQuery("delete from " + TableName);
    }
    //------------------------Удаление----------------------------------------------------

    public void Delete(string TableName, string[] columns, string[] weres)
    {
        Delete(TableName, CreateWhere(columns, weres));
    }

    public DataTable DropTable(string TableName)
    {
        return ExecQuery("drop table " + TableName);
    }

    public void Close()
    {
        connectionSQL.Close();
    }
}
