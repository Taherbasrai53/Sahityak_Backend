using Microsoft.Data.SqlClient;
using System.Data;

namespace Sahityak.Helper
{
    public class DataHelper
    {
        private DataSet _dataSet;
        public DataSet dataSet { get { return _dataSet; } }

        private string _connString;
        public string ConnString { get { return _connString; } }

        private SqlCommand _sqlCommand;
        public SqlCommand sqlCommand { get { return _sqlCommand; } }

        public DataHelper() : this(CommonHelper.CurrentConnString)
        {
        }

        public DataHelper(string connString)
        {
            _connString = connString;
        }


        public ExecuteNonQueryResult ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, null);
        }

        public ExecuteNonQueryResult ExecuteNonQuery(string commandText, List<SqlPara> Paras)
        {
            ExecuteNonQueryResult result = new ExecuteNonQueryResult();
            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(_connString);

                sqlConn.Open();

                SqlCommand sqlComm = sqlConn.CreateCommand();
                if (Paras != null)
                {
                    foreach (SqlPara para in Paras)
                    {
                        SqlParameter sqlPara = sqlComm.Parameters.AddWithValue(para.Name, para.Value);
                        sqlPara.Direction = para.Direction;
                    }
                }
                sqlComm.CommandText = commandText;
                _sqlCommand = sqlComm;

                result.NoOfRowsAffected = sqlComm.ExecuteNonQuery();
                result.flag = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (sqlConn != null)
                {
                    try
                    {
                        sqlConn.Close();
                    }
                    catch { }
                }
            }
            return result;
        }

        public class ExecuteNonQueryResult
        {
            public bool flag { get; set; }
            public int NoOfRowsAffected { get; set; }
        }

        public void Select(string commandText)
        {
            Select(commandText, null);
        }
        public void Select(string commandText, List<SqlPara> Paras)
        {
            //Console.WriteLine("In User Sync");
            bool flag = false;
            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(_connString);
                sqlConn.Open();

                SqlCommand sqlComm = sqlConn.CreateCommand();
                if (Paras != null)
                {
                    foreach (SqlPara par in Paras)
                    {
                        sqlComm.Parameters.AddWithValue(par.Name, par.Value);
                    }
                }
                sqlComm.CommandText = commandText;

                //Console.WriteLine("Before Execute Nonquery");
                var adapter = new SqlDataAdapter(sqlComm);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                _dataSet = ds;
                flag = true;

                //Console.WriteLine("After Execute Nonquery");
            }
            catch (Exception ex)
            {
                throw ex;
                //Console.WriteLine(ex.Message);
                //return new Response(false, ex.ToString());
            }
            finally
            {
                if (sqlConn != null)
                {
                    try
                    {
                        sqlConn.Close();
                    }
                    catch { }
                }
            }
            //Console.WriteLine("Out Sync user");
        }
    }

    public class SqlPara
    {
        public SqlPara()
        {
        }
        public SqlPara(string Name, object Value, ParameterDirection Direction = ParameterDirection.Input)
        {
            this.Name = Name;
            this.Value = Value;
            this.Direction = Direction;
        }
        public string Name { get; set; }
        public object Value { get; set; }
        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;
    }
}
