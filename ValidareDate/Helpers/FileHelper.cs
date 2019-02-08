using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace ValidareDate.Helpers
{
    public class FileHelper
    {
        public static DataTable ConvertExcelToDataTable(string connString, string sheetName)
        {
            OleDbConnection dbConn = new OleDbConnection(connString);

            DataTable dt = new DataTable();

            try
            {
                dbConn.Open();
                using(var cmd = new OleDbCommand($"SELECT * FROM [{sheetName}$]", dbConn))
                {
                    var adapter = new OleDbDataAdapter();
                    adapter.SelectCommand = cmd;
                    var ds = new DataSet();
                    adapter.Fill(ds);
                    dt = ds.Tables[0];
                }
            }catch(Exception ex)
            {
                
            }
            finally
            {
                dbConn.Close();
            }
            return dt;
        }
    }
}