using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
#if NET8_0_OR_GREATER
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif



namespace Common
{
    public class Database
    {
        // 데이터베이스 연결 인스턴스
        // 셋업
        public static SqlConnection setupConn;
        public static SqlCommand setupQry;
        // 데이터
        public static SqlConnection dataConn;
        public static SqlCommand dataQry;

        // 로컬
        public static SqlConnection localConn;
        public static SqlCommand localQry;
        // 서버
        public static SqlConnection serverConn;
        public static SqlCommand serverQry;

        // 원본데이터 (Raw)
        public static SqlConnection rawDataConn;
        public static SqlCommand rawDataQry;

        // ISMS
        public static SqlConnection ismsConn;
        public static SqlCommand ismsQry;

        // Property Setup
        public static SqlConnection getSetupConn() { return setupConn; }
        public static SqlCommand getSetupQuery() { return setupQry; }
        // Property Data
        public static SqlConnection getDataConn() { return dataConn; }
        public static SqlCommand getDataQuery() { return dataQry; }
        public static SqlConnection getLocalConn() { return localConn; }
        public static SqlCommand getLocalQuery() { return localQry; }
        public static SqlConnection getServerConn() { return serverConn; }
        public static SqlCommand getServerQuery() { return serverQry; }
        // Property Raw Data
        public static SqlConnection getRawDataConn() { return rawDataConn; }
        public static SqlCommand getRawDataQuery() { return rawDataQry; }
        // ISMS
        public static SqlConnection getISMSConn() { return ismsConn; }
        public static SqlCommand getISMSQuery() { return ismsQry; }


        public static bool InitDatabase(out string msgErr)
        {
            bool result = true;
            // 연결 문자열
            string ip, setupCatalog, dataCatalog, id, password;//, rawDataCatalog
            //Util.getDatabaseConnectionItems(out ip, out setupCatalog, out dataCatalog, out rawDataCatalog, out id, out password);
            Util.getDatabaseConnectionItems(out ip, out setupCatalog, out dataCatalog, out id, out password);

            try
            {
                // 셋업 연결
                setupConn = new SqlConnection();
                setupConn.ConnectionString = Util.getDatabaseConnectionString(ip, setupCatalog, id, password);
                setupConn.Open();
                setupConn.Close();
                setupQry = new SqlCommand();
                setupQry.Connection = setupConn;
                setupQry.CommandTimeout = 60;
                // 데이터 연결
                dataConn = new SqlConnection();
                dataConn.ConnectionString = Util.getDatabaseConnectionString(ip, dataCatalog, id, password);
                dataConn.Open();
                dataConn.Close();
                dataQry = new SqlCommand();
                dataQry.Connection = dataConn;
                dataQry.CommandTimeout = 60;
                /*
                // Raw 데이터 연결
                rawDataConn = new SqlConnection();
                rawDataConn.ConnectionString = Util.getDatabaseConnectionString(ip, rawDataCatalog, id, password);
                rawDataConn.Open();
                rawDataConn.Close();
                rawDataQry = new SqlCommand();
                rawDataQry.Connection = rawDataConn;
                rawDataQry.CommandTimeout = 3600;
                */
                msgErr = Define.MSG_CONNECTION_SUCCEEDED;
            }
            catch (Exception e)
            {
                msgErr = e.ToString();
                result = false;
            }
            return result;
        }

        public static bool InitDatabaseByINI(string ini, string setup, string data, out string msgErr)
        {
            bool result = true;

            try
            {
                // 셋업 연결
                setupConn = new SqlConnection();
                setupConn.ConnectionString = Util.GetIniFileString(ini, "ConnectionString", setup, "");
                setupConn.Open();
                setupConn.Close();
                setupQry = new SqlCommand();
                setupQry.Connection = setupConn;
                setupQry.CommandTimeout = 60;
                // 데이터 연결
                dataConn = new SqlConnection();
                dataConn.ConnectionString = Util.GetIniFileString(ini, "ConnectionString", data, "");
                dataConn.Open();
                dataConn.Close();
                dataQry = new SqlCommand();
                dataQry.Connection = dataConn;
                dataQry.CommandTimeout = 60;

                msgErr = Define.MSG_CONNECTION_SUCCEEDED;
            }
            catch (Exception e)
            {
                msgErr = e.ToString();
                result = false;
            }
            return result;
        }

        public static bool InitDatabaseByINIWithRawData(string ini, string local, string server, string raw, out string msgErr)
        {
            bool result = true;
            Util.WriteLog("ini: " + ini, "LogErr", "DB");
            try
            {
                // 로컬 연결
                localConn = new SqlConnection();
                localConn.ConnectionString = Util.GetIniFileString(ini, "ConnectionStrings", local, "");
                Util.WriteLog("local: " + localConn.ConnectionString, "LogErr", "DB");
                localConn.Open();
                localConn.Close();
                localQry = new SqlCommand();
                localQry.Connection = localConn;
                localQry.CommandTimeout = 60;
                // 서버 연결
                serverConn = new SqlConnection();
                serverConn.ConnectionString = Util.GetIniFileString(ini, "ConnectionStrings", server, "");
                Util.WriteLog("server: " + serverConn.ConnectionString, "LogErr", "DB");
                serverConn.Open();
                serverConn.Close();
                serverQry = new SqlCommand();
                serverQry.Connection = serverConn;
                serverQry.CommandTimeout = 60;
                // 서버 Raw 데이터 연결
                rawDataConn = new SqlConnection();
                rawDataConn.ConnectionString = Util.GetIniFileString(ini, "ConnectionStrings", raw, "");
                Util.WriteLog("raw: " + rawDataConn.ConnectionString, "LogErr", "DB");
                rawDataConn.Open();
                rawDataConn.Close();
                rawDataQry = new SqlCommand();
                rawDataQry.Connection = rawDataConn;
                rawDataQry.CommandTimeout = 60;

                msgErr = Define.MSG_CONNECTION_SUCCEEDED;
            }
            catch (Exception e)
            {
                msgErr = e.ToString();
                result = false;
            }
            return result;
        }

        public static bool InitDatabaseISMS(string ini, string db, out string msgErr)
        {
            bool result = true;
            try
            {
                // 로컬 연결
                ismsConn = new SqlConnection();
                ismsConn.ConnectionString = Util.GetIniFileString(ini, "ConnectionStrings", db, "");
                ismsConn.Open();
                ismsConn.Close();
                ismsQry = new SqlCommand();
                ismsQry.Connection = ismsConn;
                ismsQry.CommandTimeout = 60;

                msgErr = Define.MSG_CONNECTION_SUCCEEDED;
            }
            catch (Exception e)
            {
                msgErr = e.ToString();
                result = false;
            }
            return result;
        }
        
        // Prepare Setup Connection 
        public static void prepareSetupConn()
        {
            if (ConnectionState.Open != getSetupConn().State)
                getSetupConn().Open();
        }
        // Prepare Data Connection
        public static void prepareDataConn()
        {
            if (ConnectionState.Open != getDataConn().State)
                getDataConn().Open();
        }
        // Prepare RawData Connection
        public static void prepareRawDataConn()
        {
            if (ConnectionState.Open != getRawDataConn().State)
                getRawDataConn().Open();
        }

        public static void prepareISMSConn()
        {
            if (ConnectionState.Open != getISMSConn().State)
                getISMSConn().Open();
        }

        // Command Text로 명령 생성
        public static void MakeQueryString(SqlCommand sqlCommand, string query)
        {
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.Parameters.Clear();
            sqlCommand.CommandText = query;
        }

        // DateTime
        public static DateTime getDateTimeValue(SqlDataReader reader, string p)
        {
            DateTime result = DateTime.MinValue;
            try
            {
                if (reader[p] is System.DBNull)
                    result = DateTime.MinValue;
                else
                    result = (DateTime)reader[p];
            }
            catch (Exception e)
            {
                Util.WriteLog(e.Message);
            }
            return result;
        }

        internal static string getDateTimeStringValue(DataRow row, string v)
        {
            string result = string.Empty;
            try
            {
                if (row[v] is System.DBNull)
                    result = string.Empty;
                else
                    result = ((DateTime)row[v]).ToLocalTime().ToString("yy-MM-dd HH:mm:ss");
            }
            catch (Exception e)
            {
                result = (e.Message);
            }
            return result;
        }

        // string
        public static string getStringValue(SqlDataReader reader, string p)
        {
            string result = String.Empty;
            try
            {
                if (reader[p] is System.DBNull)
                    result = string.Empty;
                else
                    result = (string)reader[p];
            }
            catch (Exception e)
            {
                Util.WriteLog(e.Message);
            }
            return result;
        }

        internal static object getStringValue(DataRow row, string v)
        {
            string result = String.Empty;
            try
            {
                if (row[v] is System.DBNull)
                    result = string.Empty;
                else
                    result = (string)row[v];
            }
            catch (Exception e)
            {
                Util.WriteLog(e.Message);
            }
            return result;
        }

        // double
        public static double getDoubleValue(SqlDataReader reader, string p)
        {
            double result = 0;
            try
            {
                if (reader[p] is System.DBNull)
                    result = 0;
                else
                    result = (double)reader[p];
            }
            catch (Exception e)
            {
                Util.WriteLog(e.Message);
            }
            return result;
        }

        public static byte[] getImageValue(SqlDataReader reader, string p)
        {
            byte[] result = null;
            try
            {
                if (reader[p] is System.DBNull)
                    result = null;
                else
                    result = (byte[])reader[p];
            }
            catch (Exception e)
            {
                Util.WriteLog(e.Message);
            }
            return result;
        }

        public static float getRealValue(SqlDataReader reader, string p)
        {
            float result = 0;
            try
            {
                if (reader[p] is System.DBNull)
                    result = 0;
                else
                    result = (float)reader[p];
            }
            catch (Exception e)
            {
                Util.WriteLog(e.Message);
            }
            return result;
        }

        // int
        public static int getIntValue(SqlDataReader reader, string p)
        {
            int result = 0;
            try
            {
                if (reader[p] is System.DBNull)
                    result = 0;
                else
                    result = (int)reader[p];
            }
            catch (Exception e)
            {
                Util.WriteLog(e.Message);
            }
            return result;
        }

        // long
        public static long getLongValue(SqlDataReader reader, string p)
        {
            long result = 0;
            try
            {
                if (reader[p] is System.DBNull)
                    result = 0;
                else
                    result = (long)reader[p];
            }
            catch (Exception e)
            {
                Util.WriteLog(e.Message);
            }
            return result;
        }

        // smallint
        public static short getShortValue(SqlDataReader reader, string p)
        {
            short result = 0;
            try
            {
                if (reader[p] is System.DBNull)
                    result = 0;
                else
                    result = (short)reader[p];
            }
            catch (Exception e)
            {
                Util.WriteLog(e.Message);
            }
            return result;
        }

        // tinyint
        public static byte getTinyIntValue(SqlDataReader reader, string p)
        {
            byte result = 0;
            try
            {
                if (reader[p] is System.DBNull)
                    result = 0;
                else
                    result = (byte)reader[p];
            }
            catch (Exception e)
            {
                Util.WriteLog(e.Message);
            }
            return result;
        }

        // bool
        public static bool getBoolValue(SqlDataReader reader, string p)
        {
            bool result = false;
            try
            {
                if (reader[p] is System.DBNull)
                    result = false;
                else
                    result = (bool)reader[p];
            }
            catch (Exception e)
            {
                Util.WriteLog(e.Message);
            }
            return result;
        }

        public static double getDecimalValue(SqlDataReader reader, string p)
        {
            double result = 0;
            try
            {
                if (reader[p] is System.DBNull)
                    result = 0;
                else
                    result = Decimal.ToDouble((Decimal)reader[p]);
            }
            catch (Exception e)
            {
                Util.WriteLog(e.Message);
            }
            return result;
        }
    }
}
