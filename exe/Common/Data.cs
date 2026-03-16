using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using KEYENCE;
using XGCommLibDemo;

namespace SPC_Data
{
    public class JigInfo
    {
        public int jig_id = -1;
        public string jig_VinNo = string.Empty;
        public int jig_Number = -1;
        public DateTime jig_DateTime = DateTime.MinValue;

        internal static bool InsertJig(string vin, int jigNo, SqlConnection conn, SqlCommand qry, out string err)
        {
            err = string.Empty;
            bool result = true;

            try
            {
                try
                {
                    Database.MakeQueryString(qry, "INSERT INTO JigInfo " +
                        "( " +
                        " jig_VinNo, jig_Number, jig_DateTime) " +
                        " values " +
                        "( " +
                        "@jig_VinNo, @jig_Number, @jig_DateTime" +
                        " )");
                    string param = string.Empty;

                    // jig_VinNo
                    param = "@jig_VinNo";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = vin;

                    // jig_Number
                    param = "@jig_Number";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = jigNo;

                    // jig_DateTime
                    param = "@jig_DateTime";
                    qry.Parameters.Add(param, SqlDbType.DateTime);
                    qry.Parameters[param].Value = DateTime.Now;

                    // 연결 열기
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    // 실행
                    qry.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Util.WriteLog("Insert Jig Error. " + ex.Message, "LogErr");
                    err = ex.Message;
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("Insert Jig Error. " + e2.Message, "LogErr");
                err = e2.Message;
                return false;
            }

            return result;
        }

        internal static string getJigValue(string vin, SqlConnection conn, SqlCommand qry, out string err)
        {
            err = string.Empty;
            string result = "JIG ";

            try
            {
                try
                {
                    Database.MakeQueryString(qry, "SELECT * FROM JigInfo WHERE Jig_VinNo = @Jig_VinNo");

                    string param = string.Empty;
                    // jig_VinNo
                    param = "@jig_VinNo";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = vin;

                    // 연결 열기
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    // 실행
                    using (SqlDataReader reader = qry.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                int no = Database.getIntValue(reader, "Jig_Number");
                                string temp = string.Empty;
                                switch (no)
                                {
                                    case 1:
                                        temp = "A - " + temp;
                                        break;
                                    case 2:
                                        temp = temp + "A";
                                        break;
                                    case 3:
                                        temp = "B - " + temp;
                                        break;
                                    case 4:
                                        temp = temp + "B";
                                        break;
                                    default:
                                        break;
                                }
                                result += temp;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.WriteLog("Read Jig Error. " + ex.Message, "LogErr");
                    err = ex.Message;
                    return "";
                }
                finally
                {
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("Read Jig Error. " + e2.Message, "LogErr");
                err = e2.Message;
                return "";
            }

            return result;
        }
    }

    public class PLC_DATA
    {
        public ushort CAR_BT = 0;
        public int MIL = 0;         // 3 = SC
                                    // 5 = HC
                                    // 7 = BD

        public int CIEN = 0;        // 4DR 1=USA, 2=MEX, 3=BRZ, 4=GENERAL
                                    // 5DR 5=USA, 6=MEX, 7=BRZ, 8=GENERAL

        public int DIEZ = 0;        // NO KEY HOLE 1=NO SPOILER, 3=SPOILER
                                    //    KEY HOLE 5=NO SPOILER, 7=SPOILER

        public int UNO = 0;         // 1=PRODUCT, 2=AS

        public ushort SEQ = 0;
        public ushort RB_CAR_BT = 0;
        public ushort ASKD = 0;
        public ushort SPARE = 0;
        public string VIN = string.Empty;

        public PLC_DATA Parse(byte[] buffer)
        {
            /*
            int idx = 0;

            CAR_BT = S7.GetWordAt(buffer, idx);
            byte[] c = BitConverter.GetBytes(CAR_BT);
            MIL = c[1] / 16;
            CIEN  = c[1] % 16;
            DIEZ  = c[0] / 16;
            UNO  = c[0] % 16;
            idx += 2;

            SEQ = S7.GetWordAt(buffer, idx);
            idx += 2;

            RB_CAR_BT = S7.GetWordAt(buffer, idx);
            idx += 2;

            ASKD = S7.GetWordAt(buffer, idx);
            idx += 2;

            SPARE = S7.GetWordAt(buffer, idx);
            idx += 2;

            int size = 10;
            byte[] bufTar = new byte[size];
            Array.Copy(buffer, idx, bufTar, 0, size);
            VIN = Encoding.ASCII.GetString(bufTar).Trim('\0');
            */
            return this;
        }

        internal string getModelName()
        {
            if ((MIL == 7) || (MIL == 8))
                return "BD";

            if ((MIL == 5) || (MIL == 6))
            {
                if (CIEN < 5)
                    return "HC4";

                else return "HC5";
            }
            else if ((MIL == 3) || (MIL == 4))
            {
                if (CIEN < 5)
                    return "SC4";

                else return "SC5";
            }

            return "UNKNOWN";
        }

        public static string getVinNo(byte[] buf)
        {
            int size = 10;
            int idx = 10;
            byte[] bufTar = new byte[size];
            Array.Copy(buf, idx, bufTar, 0, size);
            return Encoding.ASCII.GetString(bufTar).Trim('\0');
        }
    }

    public class TrendData
    {
        public DateTime dateTime = DateTime.MinValue;
        public double data = double.NaN;

        public TrendData(DateTime dt, double val)
        {
            dateTime = dt;
            data = val;
        }
    }

    public class Model
    {
        public int md_id = 0;
        public string md_Name = string.Empty;
        public string md_Alias = string.Empty;

        // Hinge Hole Top
        public string md_HingeHole_Top_CameraIP = string.Empty;
        public int md_HingeHole_Top_Light = 0;
        // Upper
        public double md_JudgeTop_Upper_Rel_DiffL_min = 0;  // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 하한값
        public double md_JudgeTop_Upper_Rel_DiffL_max = 0;  // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 상한값
        public double md_JudgeTop_Upper_Rel_DiffH_min = 0;  // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 하한값
        public double md_JudgeTop_Upper_Rel_DiffH_max = 0;  // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 상한값
        public double md_JudgeTop_Upper_Calib_DiffL = 0; // outer 홀 기준 inner 홀의 x 편차에 대한 보정값 (결과에서 더해주어야 함)
        public double md_JudgeTop_Upper_Calib_DiffH = 0; // outer 홀 기준 inner 홀의 y 편차에 대한 보정값 (결과에서 더해주어야 함)
        // Lower
        public double md_JudgeTop_Lower_Rel_DiffL_min = 0;  // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 하한값
        public double md_JudgeTop_Lower_Rel_DiffL_max = 0;  // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 상한값
        public double md_JudgeTop_Lower_Rel_DiffH_min = 0;  // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 하한값
        public double md_JudgeTop_Lower_Rel_DiffH_max = 0;  // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 상한값
        public double md_JudgeTop_Lower_Calib_DiffL = 0; // outer 홀 기준 inner 홀의 x 편차에 대한 보정값 (결과에서 더해주어야 함)
        public double md_JudgeTop_Lower_Calib_DiffH = 0; // outer 홀 기준 inner 홀의 y 편차에 대한 보정값 (결과에서 더해주어야 함)

        // Hinge Hole Bottom
        public string md_HingeHole_Bottom_CameraIP = string.Empty;
        public int md_HingeHole_Bottom_Light = 0;
        // Upper
        public double md_JudgeBottom_Upper_Rel_DiffL_min = 0;   // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 하한값
        public double md_JudgeBottom_Upper_Rel_DiffL_max = 0;   // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 상한값
        public double md_JudgeBottom_Upper_Rel_DiffH_min = 0;   // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 하한값
        public double md_JudgeBottom_Upper_Rel_DiffH_max = 0;   // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 상한값
        public double md_JudgeBottom_Upper_Calib_DiffL = 0; // outer 홀 기준 inner 홀의 x 편차에 대한 보정값 (결과에서 더해주어야 함)
        public double md_JudgeBottom_Upper_Calib_DiffH = 0; // outer 홀 기준 inner 홀의 y 편차에 대한 보정값 (결과에서 더해주어야 함)
        // Lower
        public double md_JudgeBottom_Lower_Rel_DiffL_min = 0;   // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 하한값
        public double md_JudgeBottom_Lower_Rel_DiffL_max = 0;   // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 상한값
        public double md_JudgeBottom_Lower_Rel_DiffH_min = 0;   // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 하한값
        public double md_JudgeBottom_Lower_Rel_DiffH_max = 0;   // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 상한값
        public double md_JudgeBottom_Lower_Calib_DiffL = 0; // outer 홀 기준 inner 홀의 x 편차에 대한 보정값 (결과에서 더해주어야 함)
        public double md_JudgeBottom_Lower_Calib_DiffH = 0; // outer 홀 기준 inner 홀의 y 편차에 대한 보정값 (결과에서 더해주어야 함)

        // Frame Top
        public string md_FrameTop_CameraIP = string.Empty;
        public int md_FrameTop_Light = 0;
        public double md_FrameTop_BaseH = 0;        // 프레임 상부 H 픽셀 오프셋 값(측정값에서 빼주어야 함)
        public double md_FrameTop_BaseT = 0;        // 프레임 상부 T 픽셀 오프셋 값(측정값에서 빼주어야 함)
        public double md_FrameTop_DiffH_min = 0;    // 프레임 상부 측정 결과 H 값에 대한 판정 기준치 - 하한값
        public double md_FrameTop_DiffH_max = 0;    // 프레임 상부 측정 결과 H 값에 대한 판정 기준치 - 상한값
        public double md_FrameTop_DiffT_min = 0;    // 프레임 상부 측정 결과 T 값에 대한 판정 기준치 - 하한값
        public double md_FrameTop_DiffT_max = 0;    // 프레임 상부 측정 결과 T 값에 대한 판정 기준치 - 상한값
        // Frame Bottom
        public string md_FrameBottom_CameraIP = string.Empty;
        public int md_FrameBottom_Light = 0;
        public double md_FrameBottom_BaseT = 0; // 프레임 하부 T 픽셀 오프셋 값(측정값에서 빼주어야 함)
        public double md_FrameBottom_DiffT_min = 0; // 프레임 하부 측정 결과 T 값에 대한 판정 기준치 - 하한값
        public double md_FrameBottom_DiffT_max = 0; // 프레임 하부 측정 결과 T 값에 대한 판정 기준치 - 하한값


        public Model() { }
        public Model(string name) { md_Name = name; }

        // 속성 복사
        public Model(string newName, Model old)
        {
            md_Name = newName;
            md_HingeHole_Top_CameraIP = old.md_HingeHole_Top_CameraIP;
            md_HingeHole_Top_Light = old.md_HingeHole_Top_Light;

            md_JudgeTop_Upper_Rel_DiffL_min = old.md_JudgeTop_Upper_Rel_DiffL_min;
            md_JudgeTop_Upper_Rel_DiffL_max = old.md_JudgeTop_Upper_Rel_DiffL_max;
            md_JudgeTop_Upper_Rel_DiffH_min = old.md_JudgeTop_Upper_Rel_DiffH_min;
            md_JudgeTop_Upper_Rel_DiffH_max = old.md_JudgeTop_Upper_Rel_DiffH_max;
            md_JudgeTop_Upper_Calib_DiffL = old.md_JudgeTop_Upper_Calib_DiffL;
            md_JudgeTop_Upper_Calib_DiffH = old.md_JudgeTop_Upper_Calib_DiffH;

            md_JudgeTop_Lower_Rel_DiffL_min = old.md_JudgeTop_Lower_Rel_DiffL_min;
            md_JudgeTop_Lower_Rel_DiffL_max = old.md_JudgeTop_Lower_Rel_DiffL_max;
            md_JudgeTop_Lower_Rel_DiffH_min = old.md_JudgeTop_Lower_Rel_DiffH_min;
            md_JudgeTop_Lower_Rel_DiffH_max = old.md_JudgeTop_Lower_Rel_DiffH_max;
            md_JudgeTop_Lower_Calib_DiffL = old.md_JudgeTop_Lower_Calib_DiffL;
            md_JudgeTop_Lower_Calib_DiffH = old.md_JudgeTop_Lower_Calib_DiffH;

            md_HingeHole_Bottom_CameraIP = old.md_HingeHole_Bottom_CameraIP;
            md_HingeHole_Bottom_Light = old.md_HingeHole_Bottom_Light;

            md_JudgeBottom_Upper_Rel_DiffL_min = old.md_JudgeBottom_Upper_Rel_DiffL_min;
            md_JudgeBottom_Upper_Rel_DiffL_max = old.md_JudgeBottom_Upper_Rel_DiffL_max;
            md_JudgeBottom_Upper_Rel_DiffH_min = old.md_JudgeBottom_Upper_Rel_DiffH_min;
            md_JudgeBottom_Upper_Rel_DiffH_max = old.md_JudgeBottom_Upper_Rel_DiffH_max;
            md_JudgeBottom_Upper_Calib_DiffL = old.md_JudgeBottom_Upper_Calib_DiffL;
            md_JudgeBottom_Upper_Calib_DiffH = old.md_JudgeBottom_Upper_Calib_DiffH;

            md_JudgeBottom_Lower_Rel_DiffL_min = old.md_JudgeBottom_Lower_Rel_DiffL_min;
            md_JudgeBottom_Lower_Rel_DiffL_max = old.md_JudgeBottom_Lower_Rel_DiffL_max;
            md_JudgeBottom_Lower_Rel_DiffH_min = old.md_JudgeBottom_Lower_Rel_DiffH_min;
            md_JudgeBottom_Lower_Rel_DiffH_max = old.md_JudgeBottom_Lower_Rel_DiffH_max;
            md_JudgeBottom_Lower_Calib_DiffL = old.md_JudgeBottom_Lower_Calib_DiffL;
            md_JudgeBottom_Lower_Calib_DiffH = old.md_JudgeBottom_Lower_Calib_DiffH;

            md_FrameTop_CameraIP = old.md_FrameTop_CameraIP;
            md_FrameTop_Light = old.md_FrameTop_Light;
            md_FrameTop_BaseH = old.md_FrameTop_BaseH;
            md_FrameTop_BaseT = old.md_FrameTop_BaseT;
            md_FrameTop_DiffH_min = old.md_FrameTop_DiffH_min;
            md_FrameTop_DiffH_max = old.md_FrameTop_DiffH_max;
            md_FrameTop_DiffT_min = old.md_FrameTop_DiffT_min;
            md_FrameTop_DiffT_max = old.md_FrameTop_DiffT_max;

            md_FrameBottom_CameraIP = old.md_FrameBottom_CameraIP;
            md_FrameBottom_Light = old.md_FrameBottom_Light;
            md_FrameBottom_BaseT = old.md_FrameBottom_BaseT;
            md_FrameBottom_DiffT_min = old.md_FrameBottom_DiffT_min;
            md_FrameBottom_DiffT_max = old.md_FrameBottom_DiffT_max;
        }

        public static int getModelIDFromName(List<Model> list, string name)
        {
            foreach (Model m in list)
            {
                if (m.md_Name == name)
                    return m.md_id;
            }
            return -1;
        }
        public static string getModelNameFromID(List<Model> list, int id)
        {
            foreach (Model m in list)
            {
                if (m.md_id == id)
                    return m.md_Name;
            }
            return string.Empty;
        }

        public static string getModelNameFromID(SqlConnection conn, SqlCommand qry, int id)
        {
            string res = string.Empty;
            try
            {
                try
                {
                    Database.MakeQueryString(qry, "SELECT md_Name FROM Model WHERE md_id = " + id.ToString());
                    qry.CommandTimeout = 3600;
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    using (SqlDataReader reader = qry.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader != null)
                        {
                            if (reader.Read())
                            {
                                res = Database.getStringValue(reader, "md_Name");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.WriteLog("Loading Error. " + ex.Message, "LogErr");
                }
                finally
                {
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("Loading Error. " + e2.Message, "LogErr");
            }
            return res;
        }

        internal static List<Model> getModelList(SqlConnection conn, SqlCommand qry, out string err)
        {
            err = string.Empty;
            List<Model> list = new List<Model>();

            try
            {
                try
                {
                    Database.MakeQueryString(qry, "SELECT * FROM Model ORDER BY md_Name ASC");
                    qry.CommandTimeout = 3600;
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    using (SqlDataReader reader = qry.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Model model = new Model();
                                model.md_id = Database.getIntValue(reader, "md_id");
                                model.md_Name = Database.getStringValue(reader, "md_Name");
                                model.md_Alias = Database.getStringValue(reader, "md_Alias");

                                model.md_HingeHole_Top_CameraIP = Database.getStringValue(reader, "md_HingeHole_Top_CameraIP");
                                model.md_HingeHole_Top_Light = Database.getIntValue(reader, "md_HingeHole_Top_Light");

                                model.md_JudgeTop_Upper_Rel_DiffL_min = Database.getDoubleValue(reader, "md_JudgeTop_Upper_Rel_DiffL_min");
                                model.md_JudgeTop_Upper_Rel_DiffL_max = Database.getDoubleValue(reader, "md_JudgeTop_Upper_Rel_DiffL_max");
                                model.md_JudgeTop_Upper_Rel_DiffH_min = Database.getDoubleValue(reader, "md_JudgeTop_Upper_Rel_DiffH_min");
                                model.md_JudgeTop_Upper_Rel_DiffH_max = Database.getDoubleValue(reader, "md_JudgeTop_Upper_Rel_DiffH_max");
                                model.md_JudgeTop_Upper_Calib_DiffL = Database.getDoubleValue(reader, "md_JudgeTop_Upper_Calib_DiffL");
                                model.md_JudgeTop_Upper_Calib_DiffH = Database.getDoubleValue(reader, "md_JudgeTop_Upper_Calib_DiffH");

                                model.md_JudgeTop_Lower_Rel_DiffL_min = Database.getDoubleValue(reader, "md_JudgeTop_Lower_Rel_DiffL_min");
                                model.md_JudgeTop_Lower_Rel_DiffL_max = Database.getDoubleValue(reader, "md_JudgeTop_Lower_Rel_DiffL_max");
                                model.md_JudgeTop_Lower_Rel_DiffH_min = Database.getDoubleValue(reader, "md_JudgeTop_Lower_Rel_DiffH_min");
                                model.md_JudgeTop_Lower_Rel_DiffH_max = Database.getDoubleValue(reader, "md_JudgeTop_Lower_Rel_DiffH_max");
                                model.md_JudgeTop_Lower_Calib_DiffL = Database.getDoubleValue(reader, "md_JudgeTop_Lower_Calib_DiffL");
                                model.md_JudgeTop_Lower_Calib_DiffH = Database.getDoubleValue(reader, "md_JudgeTop_Lower_Calib_DiffH");

                                model.md_HingeHole_Bottom_CameraIP = Database.getStringValue(reader, "md_HingeHole_Bottom_CameraIP");
                                model.md_HingeHole_Bottom_Light = Database.getIntValue(reader, "md_HingeHole_Bottom_Light");

                                model.md_JudgeBottom_Upper_Rel_DiffL_min = Database.getDoubleValue(reader, "md_JudgeBottom_Upper_Rel_DiffL_min");
                                model.md_JudgeBottom_Upper_Rel_DiffL_max = Database.getDoubleValue(reader, "md_JudgeBottom_Upper_Rel_DiffL_max");
                                model.md_JudgeBottom_Upper_Rel_DiffH_min = Database.getDoubleValue(reader, "md_JudgeBottom_Upper_Rel_DiffH_min");
                                model.md_JudgeBottom_Upper_Rel_DiffH_max = Database.getDoubleValue(reader, "md_JudgeBottom_Upper_Rel_DiffH_max");
                                model.md_JudgeBottom_Upper_Calib_DiffL = Database.getDoubleValue(reader, "md_JudgeBottom_Upper_Calib_DiffL");
                                model.md_JudgeBottom_Upper_Calib_DiffH = Database.getDoubleValue(reader, "md_JudgeBottom_Upper_Calib_DiffH");

                                model.md_JudgeBottom_Lower_Rel_DiffL_min = Database.getDoubleValue(reader, "md_JudgeBottom_Lower_Rel_DiffL_min");
                                model.md_JudgeBottom_Lower_Rel_DiffL_max = Database.getDoubleValue(reader, "md_JudgeBottom_Lower_Rel_DiffL_max");
                                model.md_JudgeBottom_Lower_Rel_DiffH_min = Database.getDoubleValue(reader, "md_JudgeBottom_Lower_Rel_DiffH_min");
                                model.md_JudgeBottom_Lower_Rel_DiffH_max = Database.getDoubleValue(reader, "md_JudgeBottom_Lower_Rel_DiffH_max");
                                model.md_JudgeBottom_Lower_Calib_DiffL = Database.getDoubleValue(reader, "md_JudgeBottom_Lower_Calib_DiffL");
                                model.md_JudgeBottom_Lower_Calib_DiffH = Database.getDoubleValue(reader, "md_JudgeBottom_Lower_Calib_DiffH");

                                model.md_FrameTop_CameraIP = Database.getStringValue(reader, "md_FrameTop_CameraIP");
                                model.md_FrameTop_Light = Database.getIntValue(reader, "md_FrameTop_Light");

                                model.md_FrameTop_BaseH = Database.getDoubleValue(reader, "md_FrameTop_BaseH");
                                model.md_FrameTop_BaseT = Database.getDoubleValue(reader, "md_FrameTop_BaseT");
                                model.md_FrameTop_DiffH_min = Database.getDoubleValue(reader, "md_FrameTop_DiffH_min");
                                model.md_FrameTop_DiffH_max = Database.getDoubleValue(reader, "md_FrameTop_DiffH_max");
                                model.md_FrameTop_DiffT_min = Database.getDoubleValue(reader, "md_FrameTop_DiffT_min");
                                model.md_FrameTop_DiffT_max = Database.getDoubleValue(reader, "md_FrameTop_DiffT_max");

                                model.md_FrameBottom_CameraIP = Database.getStringValue(reader, "md_FrameBottom_CameraIP");
                                model.md_FrameBottom_Light = Database.getIntValue(reader, "md_FrameBottom_Light");

                                model.md_FrameBottom_BaseT = Database.getDoubleValue(reader, "md_FrameBottom_BaseT");
                                model.md_FrameBottom_DiffT_min = Database.getDoubleValue(reader, "md_FrameBottom_DiffT_min");
                                model.md_FrameBottom_DiffT_max = Database.getDoubleValue(reader, "md_FrameBottom_DiffT_max");

                                list.Add(model);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.WriteLog("Loading Error. " + ex.Message, "LogErr");
                    err = ex.Message;
                    return null;
                }
                finally
                {
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("Loading Error. " + e2.Message, "LogErr");
                err = e2.Message;
                return null;
            }

            err = "There is no Model";
            return list;
        }

        internal static bool CreateModel(string name, SqlConnection conn, SqlCommand qry, out string err, out Model m)
        {
            err = string.Empty;
            m = null;
            bool result = true;

            try
            {
                try
                {
                    Database.MakeQueryString(qry, "INSERT INTO Model " +
                        "( " +
                        " md_Name, md_MadeTime) " +
                        " values " +
                        "( " +
                        "@md_Name, @md_MadeTime" +
                        " )");
                    string param = string.Empty;

                    // md_Name
                    param = "@md_Name";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = name;

                    // md_MadeTime
                    param = "@md_MadeTime";
                    qry.Parameters.Add(param, SqlDbType.DateTime);
                    qry.Parameters[param].Value = DateTime.Now;

                    // 연결 열기
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    // 실행
                    qry.ExecuteNonQuery();

                    m = new Model(name);
                }
                catch (Exception ex)
                {
                    Util.WriteLog("Create Model Error. " + ex.Message, "LogErr");
                    err = ex.Message;
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("Create Error. " + e2.Message, "LogErr");
                err = e2.Message;
                return false;
            }

            return result;
        }

        internal static bool ModelDelete(string name, SqlConnection conn, SqlCommand qry, out string err)
        {
            err = string.Empty;
            bool result = true;

            try
            {
                try
                {
                    Database.MakeQueryString(qry, "DELETE FROM Model WHERE md_Name = @md_Name");
                    string param = string.Empty;

                    // md_Name
                    param = "@md_Name";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = name;

                    // 연결 열기
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    // 실행
                    qry.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Util.WriteLog("Delete Model Error. " + ex.Message, "LogErr");
                    err = ex.Message;
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("Delete Error. " + e2.Message, "LogErr");
                err = e2.Message;
                return false;
            }

            return result;
        }

        internal static bool InsertModel(Model m, SqlConnection conn, SqlCommand qry, out string err)
        {
            err = string.Empty;
            bool result = true;

            try
            {
                try
                {
                    Database.MakeQueryString(qry, "INSERT INTO Model " +
                        "( " +
                        " md_Name, md_Alias, " +
                        " md_HingeHole_Top_CameraIP, md_HingeHole_Top_Light, " +
                        " md_JudgeTop_Upper_Rel_DiffL_min, md_JudgeTop_Upper_Rel_DiffL_max, " +
                        " md_JudgeTop_Upper_Rel_DiffH_min, md_JudgeTop_Upper_Rel_DiffH_max, " +
                        " md_JudgeTop_Upper_Calib_DiffL, md_JudgeTop_Upper_Calib_DiffH, " +

                        " md_JudgeTop_Lower_Rel_DiffL_min, md_JudgeTop_Lower_Rel_DiffL_max, " +
                        " md_JudgeTop_Lower_Rel_DiffH_min, md_JudgeTop_Lower_Rel_DiffH_max, " +
                        " md_JudgeTop_Lower_Calib_DiffL, md_JudgeTop_Lower_Calib_DiffH, " +

                        " md_HingeHole_Bottom_CameraIP, md_HingeHole_Bottom_Light, " +
                        " md_JudgeBottom_Upper_Rel_DiffL_min, md_JudgeBottom_Upper_Rel_DiffL_max, " +
                        " md_JudgeBottom_Upper_Rel_DiffH_min, md_JudgeBottom_Upper_Rel_DiffH_max, " +
                        " md_JudgeBottom_Upper_Calib_DiffL, md_JudgeBottom_Upper_Calib_DiffH, " +

                        " md_JudgeBottom_Lower_Rel_DiffL_min, md_JudgeBottom_Lower_Rel_DiffL_max, " +
                        " md_JudgeBottom_Lower_Rel_DiffH_min, md_JudgeBottom_Lower_Rel_DiffH_max, " +
                        " md_JudgeBottom_Lower_Calib_DiffL, md_JudgeBottom_Lower_Calib_DiffH, " +

                        " md_FrameTop_CameraIP, md_FrameTop_Light, md_FrameTop_BaseH, md_FrameTop_BaseT, " +
                        " md_FrameTop_DiffH_min, md_FrameTop_DiffH_max, md_FrameTop_DiffT_min, md_FrameTop_DiffT_max, " +

                        " md_FrameBottom_CameraIP, md_FrameBottom_Light, md_FrameBottom_BaseT, " +
                        " md_FrameBottom_DiffT_min, md_FrameBottom_DiffT_max, " +
                        " md_MadeTime) " +
                        " values " +
                        "( " +
                        " @md_Name, @md_Alias, " +
                        " @md_HingeHole_Top_CameraIP, @md_HingeHole_Top_Light, " +
                        " @md_JudgeTop_Upper_Rel_DiffL_min, @md_JudgeTop_Upper_Rel_DiffL_max, " +
                        " @md_JudgeTop_Upper_Rel_DiffH_min, @md_JudgeTop_Upper_Rel_DiffH_max, " +
                        " @md_JudgeTop_Upper_Calib_DiffL, @md_JudgeTop_Upper_Calib_DiffH, " +

                        " @md_JudgeTop_Lower_Rel_DiffL_min, @md_JudgeTop_Lower_Rel_DiffL_max, " +
                        " @md_JudgeTop_Lower_Rel_DiffH_min, @md_JudgeTop_Lower_Rel_DiffH_max, " +
                        " @md_JudgeTop_Lower_Calib_DiffL, @md_JudgeTop_Lower_Calib_DiffH, " +

                        " @md_HingeHole_Bottom_CameraIP, @md_HingeHole_Bottom_Light, " +
                        " @md_JudgeBottom_Upper_Rel_DiffL_min, @md_JudgeBottom_Upper_Rel_DiffL_max, " +
                        " @md_JudgeBottom_Upper_Rel_DiffH_min, @md_JudgeBottom_Upper_Rel_DiffH_max, " +
                        " @md_JudgeBottom_Upper_Calib_DiffL, @md_JudgeBottom_Upper_Calib_DiffH, " +

                        " @md_JudgeBottom_Lower_Rel_DiffL_min, @md_JudgeBottom_Lower_Rel_DiffL_max, " +
                        " @md_JudgeBottom_Lower_Rel_DiffH_min, @md_JudgeBottom_Lower_Rel_DiffH_max, " +
                        " @md_JudgeBottom_Lower_Calib_DiffL, @md_JudgeBottom_Lower_Calib_DiffH, " +

                        " @md_FrameTop_CameraIP, @md_FrameTop_Light, @md_FrameTop_BaseH, @md_FrameTop_BaseT, " +
                        " @md_FrameTop_DiffH_min, @md_FrameTop_DiffH_max, " +
                        " @md_FrameTop_DiffT_min, @md_FrameTop_DiffT_max, " +
                        " @md_FrameBottom_CameraIP, @md_FrameBottom_Light, @md_FrameBottom_BaseT, " +
                        " @md_FrameBottom_DiffT_min, @md_FrameBottom_DiffT_max, " +
                        " @md_MadeTime" +
                        " )");
                    string param = string.Empty;

                    // md_Name
                    param = "@md_Name";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = m.md_Name;

                    param = "@md_Alias";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = m.md_Alias;

                    #region Hinge Hole Top
                    param = "@md_HingeHole_Top_CameraIP";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = m.md_HingeHole_Top_CameraIP;

                    param = "@md_HingeHole_Top_Light";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = m.md_HingeHole_Top_Light;

                    #region Hinge Hold Top Upper
                    param = "@md_JudgeTop_Upper_Rel_DiffL_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Upper_Rel_DiffL_min;

                    param = "@md_JudgeTop_Upper_Rel_DiffL_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Upper_Rel_DiffL_max;

                    param = "@md_JudgeTop_Upper_Rel_DiffH_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Upper_Rel_DiffH_min;

                    param = "@md_JudgeTop_Upper_Rel_DiffH_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Upper_Rel_DiffH_max;

                    param = "@md_JudgeTop_Upper_Calib_DiffL";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Upper_Calib_DiffL;

                    param = "@md_JudgeTop_Upper_Calib_DiffH";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Upper_Calib_DiffH;

                    #endregion // Hinge Hole Top Upper

                    #region Hinge Hole Top Lower
                    param = "@md_JudgeTop_Lower_Rel_DiffL_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Lower_Rel_DiffL_min;

                    param = "@md_JudgeTop_Lower_Rel_DiffL_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Lower_Rel_DiffL_max;

                    param = "@md_JudgeTop_Lower_Rel_DiffH_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Lower_Rel_DiffH_min;

                    param = "@md_JudgeTop_Lower_Rel_DiffH_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Lower_Rel_DiffH_max;

                    param = "@md_JudgeTop_Lower_Calib_DiffL";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Lower_Calib_DiffL;

                    param = "@md_JudgeTop_Lower_Calib_DiffH";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Lower_Calib_DiffH;

                    #endregion // Hinge Hole Top Lower
                    #endregion // Hinge Hole Top 

                    #region Hinge Hole Bottom
                    param = "@md_HingeHole_Bottom_CameraIP";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = m.md_HingeHole_Bottom_CameraIP;

                    param = "@md_HingeHole_Bottom_Light";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = m.md_HingeHole_Bottom_Light;

                    #region Hinge Hole Bottop Upper
                    param = "@md_JudgeBottom_Upper_Rel_DiffL_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Upper_Rel_DiffL_min;

                    param = "@md_JudgeBottom_Upper_Rel_DiffL_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Upper_Rel_DiffL_max;

                    param = "@md_JudgeBottom_Upper_Rel_DiffH_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Upper_Rel_DiffH_min;

                    param = "@md_JudgeBottom_Upper_Rel_DiffH_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Upper_Rel_DiffH_max;

                    param = "@md_JudgeBottom_Upper_Calib_DiffL";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Upper_Calib_DiffL;

                    param = "@md_JudgeBottom_Upper_Calib_DiffH";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Upper_Calib_DiffH;

                    #endregion // Hinge Hole Bottop Upper

                    #region Hinge Hole Bottom Lower
                    param = "@md_JudgeBottom_Lower_Rel_DiffL_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Lower_Rel_DiffL_min;

                    param = "@md_JudgeBottom_Lower_Rel_DiffL_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Lower_Rel_DiffL_max;

                    param = "@md_JudgeBottom_Lower_Rel_DiffH_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Lower_Rel_DiffH_min;

                    param = "@md_JudgeBottom_Lower_Rel_DiffH_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Lower_Rel_DiffH_max;

                    param = "@md_JudgeBottom_Lower_Calib_DiffL";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Lower_Calib_DiffL;

                    param = "@md_JudgeBottom_Lower_Calib_DiffH";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Lower_Calib_DiffH;

                    #endregion // Hinge Hole Bottom Lower
                    #endregion // Hinge Hole Bottom

                    #region Frame Top
                    param = "@md_FrameTop_CameraIP";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = m.md_FrameTop_CameraIP;

                    param = "@md_FrameTop_Light";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = m.md_FrameTop_Light;

                    param = "@md_FrameTop_BaseH";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameTop_BaseH;

                    param = "@md_FrameTop_BaseT";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameTop_BaseT;
                    #endregion //Frame Top

                    param = "@md_FrameTop_DiffH_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameTop_DiffH_min;

                    param = "@md_FrameTop_DiffH_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameTop_DiffH_max;

                    param = "@md_FrameTop_DiffT_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameTop_DiffT_min;

                    param = "@md_FrameTop_DiffT_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameTop_DiffT_max;

                    #region Frame Bottom
                    param = "@md_FrameBottom_CameraIP";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = m.md_FrameBottom_CameraIP;

                    param = "@md_FrameBottom_Light";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = m.md_FrameBottom_Light;

                    param = "@md_FrameBottom_BaseT";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameBottom_BaseT;

                    param = "@md_FrameBottom_DiffT_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameBottom_DiffT_min;

                    param = "@md_FrameBottom_DiffT_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameBottom_DiffT_max;
                    #endregion //Frame Bottom

                    // md_MadeTime
                    param = "@md_MadeTime";
                    qry.Parameters.Add(param, SqlDbType.DateTime);
                    qry.Parameters[param].Value = DateTime.Now;

                    // 연결 열기
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    // 실행
                    qry.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Util.WriteLog("Insert Model Error. " + ex.Message, "LogErr");
                    err = ex.Message;
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("Insert Error. " + e2.Message, "LogErr");
                err = e2.Message;
                return false;
            }

            return result;
        }

        internal static bool UpdateModel(Model m, SqlConnection conn, SqlCommand qry, out string err)
        {
            err = string.Empty;
            bool result = true;

            try
            {
                try
                {
                    Database.MakeQueryString(qry, "UPDATE Model " +
                        " SET " +
                        " md_Name = @md_Name, md_Alias = @md_Alias, " +
                        " md_HingeHole_Top_CameraIP = @md_HingeHole_Top_CameraIP, md_HingeHole_Top_Light = @md_HingeHole_Top_Light, " +
                        " md_JudgeTop_Upper_Rel_DiffL_min = @md_JudgeTop_Upper_Rel_DiffL_min, " +
                        " md_JudgeTop_Upper_Rel_DiffL_max = @md_JudgeTop_Upper_Rel_DiffL_max, " +
                        " md_JudgeTop_Upper_Rel_DiffH_min = @md_JudgeTop_Upper_Rel_DiffH_min, " +
                        " md_JudgeTop_Upper_Rel_DiffH_max = @md_JudgeTop_Upper_Rel_DiffH_max, " +
                        " md_JudgeTop_Upper_Calib_DiffH = @md_JudgeTop_Upper_Calib_DiffH, md_JudgeTop_Upper_Calib_DiffL = @md_JudgeTop_Upper_Calib_DiffL, " +

                        " md_JudgeTop_Lower_Rel_DiffL_min = @md_JudgeTop_Lower_Rel_DiffL_min, " +
                        " md_JudgeTop_Lower_Rel_DiffL_max = @md_JudgeTop_Lower_Rel_DiffL_max, " +
                        " md_JudgeTop_Lower_Rel_DiffH_min = @md_JudgeTop_Lower_Rel_DiffH_min, " +
                        " md_JudgeTop_Lower_Rel_DiffH_max = @md_JudgeTop_Lower_Rel_DiffH_max, " +
                        " md_JudgeTop_Lower_Calib_DiffH = @md_JudgeTop_Lower_Calib_DiffH, md_JudgeTop_Lower_Calib_DiffL = @md_JudgeTop_Lower_Calib_DiffL, " +

                        " md_HingeHole_Bottom_CameraIP = @md_HingeHole_Bottom_CameraIP, md_HingeHole_Bottom_Light = @md_HingeHole_Bottom_Light, " +
                        " md_JudgeBottom_Upper_Rel_DiffL_min = @md_JudgeBottom_Upper_Rel_DiffL_min, " +
                        " md_JudgeBottom_Upper_Rel_DiffL_max = @md_JudgeBottom_Upper_Rel_DiffL_max, " +
                        " md_JudgeBottom_Upper_Rel_DiffH_min = @md_JudgeBottom_Upper_Rel_DiffH_min, " +
                        " md_JudgeBottom_Upper_Rel_DiffH_max = @md_JudgeBottom_Upper_Rel_DiffH_max, " +
                        " md_JudgeBottom_Upper_Calib_DiffH = @md_JudgeBottom_Upper_Calib_DiffH, md_JudgeBottom_Upper_Calib_DiffL = @md_JudgeBottom_Upper_Calib_DiffL, " +

                        " md_JudgeBottom_Lower_Rel_DiffL_min = @md_JudgeBottom_Lower_Rel_DiffL_min, " +
                        " md_JudgeBottom_Lower_Rel_DiffL_max = @md_JudgeBottom_Lower_Rel_DiffL_max, " +
                        " md_JudgeBottom_Lower_Rel_DiffH_min = @md_JudgeBottom_Lower_Rel_DiffH_min, " +
                        " md_JudgeBottom_Lower_Rel_DiffH_max = @md_JudgeBottom_Lower_Rel_DiffH_max, " +
                        " md_JudgeBottom_Lower_Calib_DiffH = @md_JudgeBottom_Lower_Calib_DiffH, md_JudgeBottom_Lower_Calib_DiffL = @md_JudgeBottom_Lower_Calib_DiffL, " +

                        " md_FrameTop_CameraIP = @md_FrameTop_CameraIP, md_FrameTop_Light = @md_FrameTop_Light, " +
                        " md_FrameTop_BaseH = @md_FrameTop_BaseH, md_FrameTop_BaseT = @md_FrameTop_BaseT, " +
                        " md_FrameTop_DiffH_min = @md_FrameTop_DiffH_min, md_FrameTop_DiffH_max = @md_FrameTop_DiffH_max, " +
                        " md_FrameTop_DiffT_min = @md_FrameTop_DiffT_min, md_FrameTop_DiffT_max = @md_FrameTop_DiffT_max, " +
                        " md_FrameBottom_CameraIP = @md_FrameBottom_CameraIP, md_FrameBottom_Light = @md_FrameBottom_Light, " +
                        " md_FrameBottom_BaseT = @md_FrameBottom_BaseT, " +
                        " md_FrameBottom_DiffT_min = @md_FrameBottom_DiffT_min, md_FrameBottom_DiffT_max = @md_FrameBottom_DiffT_max, " +
                        " md_UpdatedTime = @md_UpdatedTime" +
                        " WHERE md_Name = @md_Name" +
                        "");
                    string param = string.Empty;

                    // md_Name
                    param = "@md_Name";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = m.md_Name;

                    param = "@md_Alias";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = m.md_Alias;

                    #region Hinge Hole Top
                    param = "@md_HingeHole_Top_CameraIP";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = m.md_HingeHole_Top_CameraIP;

                    param = "@md_HingeHole_Top_Light";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = m.md_HingeHole_Top_Light;

                    #region Hinge Hold Top Upper
                    param = "@md_JudgeTop_Upper_Rel_DiffL_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Upper_Rel_DiffL_min;

                    param = "@md_JudgeTop_Upper_Rel_DiffL_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Upper_Rel_DiffL_max;

                    param = "@md_JudgeTop_Upper_Rel_DiffH_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Upper_Rel_DiffH_min;

                    param = "@md_JudgeTop_Upper_Rel_DiffH_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Upper_Rel_DiffH_max;

                    param = "@md_JudgeTop_Upper_Calib_DiffH";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Upper_Calib_DiffH;

                    param = "@md_JudgeTop_Upper_Calib_DiffL";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Upper_Calib_DiffL;

                    #endregion // Hinge Hole Top Upper

                    #region Hinge Hole Top Lower
                    param = "@md_JudgeTop_Lower_Rel_DiffL_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Lower_Rel_DiffL_min;

                    param = "@md_JudgeTop_Lower_Rel_DiffL_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Lower_Rel_DiffL_max;

                    param = "@md_JudgeTop_Lower_Rel_DiffH_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Lower_Rel_DiffH_min;

                    param = "@md_JudgeTop_Lower_Rel_DiffH_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Lower_Rel_DiffH_max;

                    param = "@md_JudgeTop_Lower_Calib_DiffH";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Lower_Calib_DiffH;

                    param = "@md_JudgeTop_Lower_Calib_DiffL";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeTop_Lower_Calib_DiffL;

                    #endregion // Hinge Hole Top Lower
                    #endregion // Hinge Hole Top 

                    #region Hinge Hole Bottom
                    param = "@md_HingeHole_Bottom_CameraIP";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = m.md_HingeHole_Bottom_CameraIP;

                    param = "@md_HingeHole_Bottom_Light";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = m.md_HingeHole_Bottom_Light;

                    #region Hinge Hole Bottop Upper
                    param = "@md_JudgeBottom_Upper_Rel_DiffL_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Upper_Rel_DiffL_min;

                    param = "@md_JudgeBottom_Upper_Rel_DiffL_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Upper_Rel_DiffL_max;

                    param = "@md_JudgeBottom_Upper_Rel_DiffH_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Upper_Rel_DiffH_min;

                    param = "@md_JudgeBottom_Upper_Rel_DiffH_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Upper_Rel_DiffH_max;

                    param = "@md_JudgeBottom_Upper_Calib_DiffH";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Upper_Calib_DiffH;

                    param = "@md_JudgeBottom_Upper_Calib_DiffL";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Upper_Calib_DiffL;

                    #endregion // Hinge Hole Bottop Upper

                    #region Hinge Hole Bottom Lower
                    param = "@md_JudgeBottom_Lower_Rel_DiffL_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Lower_Rel_DiffL_min;

                    param = "@md_JudgeBottom_Lower_Rel_DiffL_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Lower_Rel_DiffL_max;

                    param = "@md_JudgeBottom_Lower_Rel_DiffH_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Lower_Rel_DiffH_min;

                    param = "@md_JudgeBottom_Lower_Rel_DiffH_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Lower_Rel_DiffH_max;

                    param = "@md_JudgeBottom_Lower_Calib_DiffH";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Lower_Calib_DiffH;

                    param = "@md_JudgeBottom_Lower_Calib_DiffL";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_JudgeBottom_Lower_Calib_DiffL;

                    #endregion // Hinge Hole Bottom Lower
                    #endregion // Hinge Hole Bottom

                    #region Frame Top
                    param = "@md_FrameTop_CameraIP";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = m.md_FrameTop_CameraIP;

                    param = "@md_FrameTop_Light";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = m.md_FrameTop_Light;

                    param = "@md_FrameTop_BaseH";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameTop_BaseH;

                    param = "@md_FrameTop_BaseT";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameTop_BaseT;

                    param = "@md_FrameTop_DiffH_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameTop_DiffH_min;

                    param = "@md_FrameTop_DiffH_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameTop_DiffH_max;

                    param = "@md_FrameTop_DiffT_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameTop_DiffT_min;

                    param = "@md_FrameTop_DiffT_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameTop_DiffT_max;
                    #endregion //Frame Top

                    #region Frame Bottom
                    param = "@md_FrameBottom_CameraIP";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = m.md_FrameBottom_CameraIP;

                    param = "@md_FrameBottom_Light";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = m.md_FrameBottom_Light;

                    param = "@md_FrameBottom_BaseT";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameBottom_BaseT;

                    param = "@md_FrameBottom_DiffT_min";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameBottom_DiffT_min;

                    param = "@md_FrameBottom_DiffT_max";
                    qry.Parameters.Add(param, SqlDbType.Float);
                    qry.Parameters[param].Value = m.md_FrameBottom_DiffT_max;
                    #endregion //Frame Bottom

                    // md_UpdatedTime
                    param = "@md_UpdatedTime";
                    qry.Parameters.Add(param, SqlDbType.DateTime);
                    qry.Parameters[param].Value = DateTime.Now;

                    // 연결 열기
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    // 실행
                    qry.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Util.WriteLog("Update Model Error. " + ex.Message, "LogErr");
                    err = ex.Message;
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("Update Error. " + e2.Message, "LogErr");
                err = e2.Message;
                return false;
            }

            return result;
        }

    }

    /*
	int	Checked
	varchar(255)	Checked
	int	Checked
     */
    public class Product
    {
        public int pd_id = 0;
        public int md_id = 0;
        public string pd_VinNo = string.Empty;
        public int pd_TotalResult = Define.RESULT_NA;
        public string pd_Date = "19000101";
        public string pd_Time = "000000";

        // 측정 파일
        public string md_HingeHole_Top_File = string.Empty;
        // 측정 당시 조명 값
        public int md_HingeHole_Top_Light = 0;

        // 판정 기준값들 - Top Upper
        public double md_JudgeTop_Upper_Rel_DiffL_min = 0;  // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 하한값
        public double md_JudgeTop_Upper_Rel_DiffL_max = 0;  // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 상한값
        public double md_JudgeTop_Upper_Rel_DiffH_min = 0;  // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 하한값
        public double md_JudgeTop_Upper_Rel_DiffH_max = 0;  // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 상한값
        public double md_JudgeTop_Upper_Calib_DiffL = 0; // outer 홀 기준 inner 홀의 x 편차에 대한 보정값 (결과에서 더해주어야 함)
        public double md_JudgeTop_Upper_Calib_DiffH = 0; // outer 홀 기준 inner 홀의 y 편차에 대한 보정값 (결과에서 더해주어야 함)
        // 측정값들 - Top Upper
        public double md_ValueTop_Upper_Rel_DiffL = 0; // outer 홀 기준 inner 홀의 x 좌표 편차 ===> 실제 트렌드에 보여지는 값
        public double md_ValueTop_Upper_Rel_DiffH = 0; // outer 홀 기준 inner 홀의 y 좌표 편차 ===> 실제 트렌드에 보여지는 값

        // 판정 기준값들 - Top Lower
        public double md_JudgeTop_Lower_Rel_DiffL_min = 0;  // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 하한값
        public double md_JudgeTop_Lower_Rel_DiffL_max = 0;  // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 상한값
        public double md_JudgeTop_Lower_Rel_DiffH_min = 0;  // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 하한값
        public double md_JudgeTop_Lower_Rel_DiffH_max = 0;  // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 상한값
        public double md_JudgeTop_Lower_Calib_DiffL = 0; // outer 홀 기준 inner 홀의 x 편차에 대한 보정값 (결과에서 더해주어야 함)
        public double md_JudgeTop_Lower_Calib_DiffH = 0; // outer 홀 기준 inner 홀의 y 편차에 대한 보정값 (결과에서 더해주어야 함)
        // 측정값들 - Top Lower
        public double md_ValueTop_Lower_Rel_DiffL = 0;  // outer 홀 기준 inner 홀의 x 좌표 편차 ===> 실제 트렌드에 보여지는 값
        public double md_ValueTop_Lower_Rel_DiffH = 0;  // outer 홀 기준 inner 홀의 y 좌표 편차 ===> 실제 트렌드에 보여지는 값
        // 판정 결과 - Hinge Hole Top
        public int md_HingeHole_Top_Result = Define.RESULT_NA;


        // 측정 파일
        public string md_HingeHole_Bottom_File = string.Empty;
        // 측정 당시 조명 값
        public int md_HingeHole_Bottom_Light = 0;
        // 판정 기준값들 - Bottom Upper
        public double md_JudgeBottom_Upper_Rel_DiffL_min = 0;   // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 하한값
        public double md_JudgeBottom_Upper_Rel_DiffL_max = 0;   // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 상한값
        public double md_JudgeBottom_Upper_Rel_DiffH_min = 0;   // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 하한값
        public double md_JudgeBottom_Upper_Rel_DiffH_max = 0;   // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 상한값
        public double md_JudgeBottom_Upper_Calib_DiffL = 0; // outer 홀 기준 inner 홀의 x 편차에 대한 보정값 (결과에서 더해주어야 함)
        public double md_JudgeBottom_Upper_Calib_DiffH = 0; // outer 홀 기준 inner 홀의 y 편차에 대한 보정값 (결과에서 더해주어야 함)
        // 측정값들 - Bottom Upper
        public double md_ValueBottom_Upper_Rel_DiffL = 0;   // outer 홀 기준 inner 홀의 x 좌표 편차 ===> 실제 트렌드에 보여지는 값
        public double md_ValueBottom_Upper_Rel_DiffH = 0;   // outer 홀 기준 inner 홀의 y 좌표 편차 ===> 실제 트렌드에 보여지는 값

        // 판정 기준값들 - Bottom Lower 
        public double md_JudgeBottom_Lower_Rel_DiffL_min = 0;   // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 하한값
        public double md_JudgeBottom_Lower_Rel_DiffL_max = 0;   // 아우터 홀 기준 인너홀의 x 편차에 대한 판정기준치 상한값
        public double md_JudgeBottom_Lower_Rel_DiffH_min = 0;   // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 하한값
        public double md_JudgeBottom_Lower_Rel_DiffH_max = 0;   // 아우터 홀 기준 인너홀의 y 편차에 대한 판정기준치 상한값
        public double md_JudgeBottom_Lower_Calib_DiffL = 0; // outer 홀 기준 inner 홀의 x 편차에 대한 보정값 (결과에서 더해주어야 함)
        public double md_JudgeBottom_Lower_Calib_DiffH = 0; // outer 홀 기준 inner 홀의 y 편차에 대한 보정값 (결과에서 더해주어야 함)
        // 측정값들
        public double md_ValueBottom_Lower_Rel_DiffL = 0;   // outer 홀 기준 inner 홀의 x 좌표 편차 ===> 실제 트렌드에 보여지는 값
        public double md_ValueBottom_Lower_Rel_DiffH = 0;   // outer 홀 기준 inner 홀의 y 좌표 편차 ===> 실제 트렌드에 보여지는 값
        // 판정 결과 - Hinge Hole Bottom
        public int md_HingeHole_Bottom_Result = Define.RESULT_NA;


        // 측정 결과 파일
        public string md_FrameTop_File = string.Empty;
        // 측정 당시 조명 값
        public int md_FrameTop_Light = 0;
        // 판정 기준값들 - Frame Top
        public double md_FrameTop_BaseH = 0;        // 프레임 상부 H 픽셀 오프셋 값(측정값에서 빼주어야 함)
        public double md_FrameTop_BaseT = 0;        // 프레임 상부 T 픽셀 오프셋 값(측정값에서 빼주어야 함)
        public double md_FrameTop_DiffH_min = 0;    // 프레임 상부 측정 결과 H 값에 대한 판정 기준치 - 하한값
        public double md_FrameTop_DiffH_max = 0;    // 프레임 상부 측정 결과 H 값에 대한 판정 기준치 - 상한값
        public double md_FrameTop_DiffT_min = 0;    // 프레임 상부 측정 결과 T 값에 대한 판정 기준치 - 하한값
        public double md_FrameTop_DiffT_max = 0;    // 프레임 상부 측정 결과 T 값에 대한 판정 기준치 - 상한값
        // 측정값들 - Frame Top
        public double md_ValueFrameTop_BaseH = 0;   // 측정된 H 픽셀값
        public double md_ValueFrameTop_BaseT = 0;   // 측정된 T 픽셀값
        // 이 값들은 측정값-기준값 (px)
        public double md_ValueFrameTop_DiffH_px = 0;    // 오프셋이 보정된 실제 H 간격의 픽셀값 (md_ValueFrameTop_BaseH - md_FrameTop_BaseH)
        public double md_ValueFrameTop_DiffT_px = 0;    // 오프셋이 보정된 실제 T 간격의 픽셀값 (md_ValueFrameTop_BaseT - md_FrameTop_BaseT)
        // 아래 값들은 계산된 이후의 값
        public double md_ValueFrameTop_DiffH = 0;   // 최종 계산된 H 간격의 결과값 ===> 트렌드에 보여짐
        public double md_ValueFrameTop_DiffT = 0;   // 최종 계산된 T 간격의 결과값 ===> 트렌드에 보여짐
        // 판정 결과 - Frame Top
        public int md_FrameTop_Result = Define.RESULT_NA;

        // 측정 결과 파일 
        public string md_FrameBottom_File = string.Empty;
        // 측정 당시 조명 값
        public int md_FrameBottom_Light = 0;
        // 판정 기준값들 - Frame Bottom
        public double md_FrameBottom_BaseT = 0; // 프레임 하부 T 픽셀 오프셋 값(측정값에서 빼주어야 함)
        public double md_FrameBottom_DiffT_min = 0; // 프레임 하부 측정 결과 T 값에 대한 판정 기준치 - 하한값
        public double md_FrameBottom_DiffT_max = 0; // 프레임 하부 측정 결과 T 값에 대한 판정 기준치 - 하한값
        // 측정갑들 - Frame Bottom
        public double md_ValueFrameBottom_BaseT = 0;        // 측정된 T 픽셀값
        public double md_ValueFrameBottom_DiffT_px = 0;     // 오프셋이 보정된 실제 T 간격의 픽셀값 (md_ValueFrameBottom_BaseT - md_FrameBottom_BaseT)
        public double md_ValueFrameBottom_DiffT = 0;        // 최종 계산된 T 간격의 결과값 ===> 트렌드에 보여짐
        // 판정 결과 - Frame Bottom
        public int md_FrameBottom_Result = Define.RESULT_NA;

        internal static List<Product> getProductList(SqlConnection conn, SqlCommand qry, int count, out string err)
        {
            err = string.Empty;
            List<Product> list = new List<Product>();

            try
            {
                try
                {
                    Database.MakeQueryString(qry, "SELECT TOP " + count.ToString() + " * FROM Product ORDER BY pd_id DESC");
                    qry.CommandTimeout = 3600;
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    using (SqlDataReader reader = qry.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Product pd = new Product();
                                pd.pd_id = Database.getIntValue(reader, "pd_id");
                                pd.md_id = Database.getIntValue(reader, "md_id");
                                pd.pd_VinNo = Database.getStringValue(reader, "pd_VinNo");
                                pd.pd_TotalResult = Database.getIntValue(reader, "pd_TotalResult");
                                pd.pd_Date = Database.getStringValue(reader, "pd_Date");
                                pd.pd_Time = Database.getStringValue(reader, "pd_Time");

                                list.Add(pd);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.WriteLog("Product List Loading Error. " + ex.Message, "LogErr");
                    err = ex.Message;
                    return null;
                }
                finally
                {
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("Product List Loading Error. " + e2.Message, "LogErr");
                err = e2.Message;
                return null;
            }
            return list;
        }

        internal static List<Product> getProductSearchResult(SqlConnection conn, SqlCommand qry, string command, out string err)
        {
            err = string.Empty;
            List<Product> list = new List<Product>();

            try
            {
                try
                {
                    Database.MakeQueryString(qry, command);
                    qry.CommandTimeout = 3600;
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    using (SqlDataReader reader = qry.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Product pd = new Product();
                                pd.md_id = Database.getIntValue(reader, "md_id");
                                pd.pd_id = Database.getIntValue(reader, "pd_id");
                                pd.pd_VinNo = Database.getStringValue(reader, "pd_VinNo");
                                pd.pd_TotalResult = Database.getIntValue(reader, "pd_TotalResult");
                                pd.pd_Date = Database.getStringValue(reader, "pd_Date");
                                pd.pd_Time = Database.getStringValue(reader, "pd_Time");

                                // 측정 파일
                                pd.md_HingeHole_Top_File = Database.getStringValue(reader, "md_HingeHole_Top_File");

                                // 기준값들 - Top Upper
                                pd.md_JudgeTop_Upper_Rel_DiffL_min = Database.getDoubleValue(reader, "md_JudgeTop_Upper_Rel_DiffL_min");
                                pd.md_JudgeTop_Upper_Rel_DiffL_max = Database.getDoubleValue(reader, "md_JudgeTop_Upper_Rel_DiffL_max");
                                pd.md_JudgeTop_Upper_Rel_DiffH_min = Database.getDoubleValue(reader, "md_JudgeTop_Upper_Rel_DiffH_min");
                                pd.md_JudgeTop_Upper_Rel_DiffH_max = Database.getDoubleValue(reader, "md_JudgeTop_Upper_Rel_DiffH_max");
                                pd.md_JudgeTop_Upper_Calib_DiffL = Database.getDoubleValue(reader, "md_JudgeTop_Upper_Calib_DiffL");
                                pd.md_JudgeTop_Upper_Calib_DiffH = Database.getDoubleValue(reader, "md_JudgeTop_Upper_Calib_DiffH");
                                // 측정값들 - Top Upper
                                pd.md_ValueTop_Upper_Rel_DiffL = Database.getDoubleValue(reader, "md_ValueTop_Upper_Rel_DiffL"); // inner 홀의 x 좌표
                                pd.md_ValueTop_Upper_Rel_DiffH = Database.getDoubleValue(reader, "md_ValueTop_Upper_Rel_DiffH"); // inner 홀의 y 좌표

                                // 기준값들 - Top Lower
                                pd.md_JudgeTop_Lower_Rel_DiffL_min = Database.getDoubleValue(reader, "md_JudgeTop_Lower_Rel_DiffL_min");
                                pd.md_JudgeTop_Lower_Rel_DiffL_max = Database.getDoubleValue(reader, "md_JudgeTop_Lower_Rel_DiffL_max");
                                pd.md_JudgeTop_Lower_Rel_DiffH_min = Database.getDoubleValue(reader, "md_JudgeTop_Lower_Rel_DiffH_min");
                                pd.md_JudgeTop_Lower_Rel_DiffH_max = Database.getDoubleValue(reader, "md_JudgeTop_Lower_Rel_DiffH_max");
                                pd.md_JudgeTop_Lower_Calib_DiffL = Database.getDoubleValue(reader, "md_JudgeTop_Lower_Calib_DiffL");
                                pd.md_JudgeTop_Lower_Calib_DiffH = Database.getDoubleValue(reader, "md_JudgeTop_Lower_Calib_DiffH");
                                // 측정값들 - Top Lower
                                pd.md_ValueTop_Lower_Rel_DiffL = Database.getDoubleValue(reader, "md_ValueTop_Lower_Rel_DiffL"); // inner 홀의 x 좌표
                                pd.md_ValueTop_Lower_Rel_DiffH = Database.getDoubleValue(reader, "md_ValueTop_Lower_Rel_DiffH"); // inner 홀의 y 좌표

                                // 측정 결과
                                pd.md_HingeHole_Top_Result = Database.getIntValue(reader, "md_HingeHole_Top_Result");

                                // 측정 파일
                                pd.md_HingeHole_Bottom_File = Database.getStringValue(reader, "md_HingeHole_Bottom_File");

                                // 기준값들 - Bottom Upper
                                pd.md_JudgeBottom_Upper_Rel_DiffL_min = Database.getDoubleValue(reader, "md_JudgeBottom_Upper_Rel_DiffL_min");
                                pd.md_JudgeBottom_Upper_Rel_DiffL_max = Database.getDoubleValue(reader, "md_JudgeBottom_Upper_Rel_DiffL_max");
                                pd.md_JudgeBottom_Upper_Rel_DiffH_min = Database.getDoubleValue(reader, "md_JudgeBottom_Upper_Rel_DiffH_min");
                                pd.md_JudgeBottom_Upper_Rel_DiffH_max = Database.getDoubleValue(reader, "md_JudgeBottom_Upper_Rel_DiffH_max");
                                pd.md_JudgeBottom_Upper_Calib_DiffL = Database.getDoubleValue(reader, "md_JudgeBottom_Upper_Calib_DiffL");
                                pd.md_JudgeBottom_Upper_Calib_DiffH = Database.getDoubleValue(reader, "md_JudgeBottom_Upper_Calib_DiffH");
                                // 측정값들 - Bottom Upper
                                pd.md_ValueBottom_Upper_Rel_DiffL = Database.getDoubleValue(reader, "md_ValueBottom_Upper_Rel_DiffL"); // inner 홀의 x 좌표
                                pd.md_ValueBottom_Upper_Rel_DiffH = Database.getDoubleValue(reader, "md_ValueBottom_Upper_Rel_DiffH"); // inner 홀의 y 좌표

                                // 기준값들 - Bottom Lower
                                pd.md_JudgeBottom_Lower_Rel_DiffL_min = Database.getDoubleValue(reader, "md_JudgeBottom_Lower_Rel_DiffL_min");
                                pd.md_JudgeBottom_Lower_Rel_DiffL_max = Database.getDoubleValue(reader, "md_JudgeBottom_Lower_Rel_DiffL_max");
                                pd.md_JudgeBottom_Lower_Rel_DiffH_min = Database.getDoubleValue(reader, "md_JudgeBottom_Lower_Rel_DiffH_min");
                                pd.md_JudgeBottom_Lower_Rel_DiffH_max = Database.getDoubleValue(reader, "md_JudgeBottom_Lower_Rel_DiffH_max");
                                pd.md_JudgeBottom_Lower_Calib_DiffL = Database.getDoubleValue(reader, "md_JudgeBottom_Lower_Calib_DiffL");
                                pd.md_JudgeBottom_Lower_Calib_DiffH = Database.getDoubleValue(reader, "md_JudgeBottom_Lower_Calib_DiffH");
                                // 측정값들 - Bottom Lower
                                pd.md_ValueBottom_Lower_Rel_DiffL = Database.getDoubleValue(reader, "md_ValueBottom_Lower_Rel_DiffL"); // inner 홀의 x 좌표
                                pd.md_ValueBottom_Lower_Rel_DiffH = Database.getDoubleValue(reader, "md_ValueBottom_Lower_Rel_DiffH"); // inner 홀의 y 좌표


                                // 측정 결과
                                pd.md_HingeHole_Bottom_Result = Database.getIntValue(reader, "md_HingeHole_Bottom_Result");

                                // 측정 파일
                                pd.md_FrameTop_File = Database.getStringValue(reader, "md_FrameTop_File");

                                // 판정기준값들 - Frame Top
                                pd.md_FrameTop_BaseH = Database.getDoubleValue(reader, "md_FrameTop_BaseH");
                                pd.md_FrameTop_BaseT = Database.getDoubleValue(reader, "md_FrameTop_BaseT");
                                pd.md_FrameTop_DiffH_min = Database.getDoubleValue(reader, "md_FrameTop_DiffH_min");
                                pd.md_FrameTop_DiffH_max = Database.getDoubleValue(reader, "md_FrameTop_DiffH_max");
                                pd.md_FrameTop_DiffT_min = Database.getDoubleValue(reader, "md_FrameTop_DiffT_min");
                                pd.md_FrameTop_DiffT_max = Database.getDoubleValue(reader, "md_FrameTop_DiffT_max");

                                // 측정 결과
                                pd.md_FrameTop_Result = Database.getIntValue(reader, "md_FrameTop_Result");

                                // 측정값들 - Frame Top
                                pd.md_ValueFrameTop_BaseH = Database.getDoubleValue(reader, "md_ValueFrameTop_BaseH");
                                pd.md_ValueFrameTop_BaseT = Database.getDoubleValue(reader, "md_ValueFrameTop_BaseT");
                                pd.md_ValueFrameTop_DiffH_px = Database.getDoubleValue(reader, "md_ValueFrameTop_DiffH_px");
                                pd.md_ValueFrameTop_DiffT_px = Database.getDoubleValue(reader, "md_ValueFrameTop_DiffT_px");
                                pd.md_ValueFrameTop_DiffH = Database.getDoubleValue(reader, "md_ValueFrameTop_DiffH");
                                pd.md_ValueFrameTop_DiffT = Database.getDoubleValue(reader, "md_ValueFrameTop_DiffT");

                                // 측정 파일
                                pd.md_FrameBottom_File = Database.getStringValue(reader, "md_FrameBottom_File");

                                // 판정기준값들 - Frame Bottom
                                pd.md_FrameBottom_BaseT = Database.getDoubleValue(reader, "md_FrameBottom_BaseT");
                                pd.md_FrameBottom_DiffT_min = Database.getDoubleValue(reader, "md_FrameBottom_DiffT_min");
                                pd.md_FrameBottom_DiffT_max = Database.getDoubleValue(reader, "md_FrameBottom_DiffT_max");

                                // 측정갑들 - Frame Bottom
                                pd.md_ValueFrameBottom_BaseT = Database.getDoubleValue(reader, "md_ValueFrameBottom_BaseT");
                                pd.md_ValueFrameBottom_DiffT_px = Database.getDoubleValue(reader, "md_ValueFrameBottom_DiffT_px");
                                pd.md_ValueFrameBottom_DiffT = Database.getDoubleValue(reader, "md_ValueFrameBottom_DiffT");

                                // 측정 결과
                                pd.md_FrameBottom_Result = Database.getIntValue(reader, "md_FrameBottom_Result");

                                list.Add(pd);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.WriteLog("Product Search result Loading Error. " + ex.Message, "LogErr");
                    err = ex.Message;
                    return null;
                }
                finally
                {
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("Product Search result Loading Error. " + e2.Message, "LogErr");
                err = e2.Message;
                return null;
            }
            return list;
        }

        public static bool SaveDatabase(Product pd, SqlConnection conn, SqlCommand qry, out string err)
        {
            err = string.Empty;
            bool result = true;

            try
            {
                try
                {
                    Database.MakeQueryString(qry, "INSERT INTO Product " +
                        "( " +
                        " md_id, pd_VinNo, pd_TotalResult, pd_Date, pd_Time, " +

                            // Hinge 
                            // Hinge Top
                            " md_HingeHole_Top_File, md_HingeHole_Top_Light, " +

                                // Hinge Top Upper
                                " md_JudgeTop_Upper_Rel_DiffL_min, md_JudgeTop_Upper_Rel_DiffL_max, " +
                                " md_JudgeTop_Upper_Rel_DiffH_min, md_JudgeTop_Upper_Rel_DiffH_max, " +
                                " md_JudgeTop_Upper_Calib_DiffL, md_JudgeTop_Upper_Calib_DiffH, " +
                                " md_ValueTop_Upper_Rel_DiffL, md_ValueTop_Upper_Rel_DiffH, " +
                                // Hinge Top Lower
                                " md_JudgeTop_Lower_Rel_DiffL_min, md_JudgeTop_Lower_Rel_DiffL_max, " +
                                " md_JudgeTop_Lower_Rel_DiffH_min, md_JudgeTop_Lower_Rel_DiffH_max, " +
                                " md_JudgeTop_Lower_Calib_DiffL, md_JudgeTop_Lower_Calib_DiffH, " +
                                " md_ValueTop_Lower_Rel_DiffL, md_ValueTop_Lower_Rel_DiffH, " +
                                " md_HingeHole_Top_Result, " +

                            // Hinge Bottom
                            " md_HingeHole_Bottom_File, md_HingeHole_Bottom_Light, " +

                                // Hinge Bottom Upper
                                " md_JudgeBottom_Upper_Rel_DiffL_min, md_JudgeBottom_Upper_Rel_DiffL_max, " +
                                " md_JudgeBottom_Upper_Rel_DiffH_min, md_JudgeBottom_Upper_Rel_DiffH_max, " +
                                " md_JudgeBottom_Upper_Calib_DiffL, md_JudgeBottom_Upper_Calib_DiffH, " +
                                " md_ValueBottom_Upper_Rel_DiffL, md_ValueBottom_Upper_Rel_DiffH, " +
                                // Hinge Bottom Lower
                                " md_JudgeBottom_Lower_Rel_DiffL_min, md_JudgeBottom_Lower_Rel_DiffL_max, " +
                                " md_JudgeBottom_Lower_Rel_DiffH_min, md_JudgeBottom_Lower_Rel_DiffH_max, " +
                                " md_JudgeBottom_Lower_Calib_DiffL, md_JudgeBottom_Lower_Calib_DiffH, " +
                                " md_ValueBottom_Lower_Rel_DiffL, md_ValueBottom_Lower_Rel_DiffH, " +
                            " md_HingeHole_Bottom_Result, " +

                            // Frame 
                            // Frame Top
                            " md_FrameTop_File, md_FrameTop_Light, " +
                            " md_FrameTop_BaseH, md_FrameTop_BaseT, " +
                            " md_FrameTop_DiffH_min, md_FrameTop_DiffH_max, " +
                            " md_FrameTop_DiffT_min, md_FrameTop_DiffT_max, " +
                            " md_ValueFrameTop_BaseH, md_ValueFrameTop_BaseT, " +
                            " md_ValueFrameTop_DiffH_px, md_ValueFrameTop_DiffT_px, md_ValueFrameTop_DiffH, md_ValueFrameTop_DiffT, " +
                            " md_FrameTop_Result, " +

                            // Frame Bottom
                            " md_FrameBottom_File, md_FrameBottom_Light, " +
                            " md_FrameBottom_BaseT," +
                            " md_FrameBottom_DiffT_min, md_FrameBottom_DiffT_max, " +
                            " md_ValueFrameBottom_BaseT, " +
                            " md_ValueFrameBottom_DiffT_px, md_ValueFrameBottom_DiffT, " +
                            " md_FrameBottom_Result " +



                        " ) values ( " +

                        " @md_id, @pd_VinNo, @pd_TotalResult, @pd_Date, @pd_Time, " +

                            // Hinge 
                            // Hinge Top
                            " @md_HingeHole_Top_File, @md_HingeHole_Top_Light, " +

                                // Hinge Top Upper
                                " @md_JudgeTop_Upper_Rel_DiffL_min, @md_JudgeTop_Upper_Rel_DiffL_max, " +
                                " @md_JudgeTop_Upper_Rel_DiffH_min, @md_JudgeTop_Upper_Rel_DiffH_max, " +
                                " @md_JudgeTop_Upper_Calib_DiffL, @md_JudgeTop_Upper_Calib_DiffH, " +
                                " @md_ValueTop_Upper_Rel_DiffL, @md_ValueTop_Upper_Rel_DiffH, " +
                                // Hinge Top Lower
                                " @md_JudgeTop_Lower_Rel_DiffL_min, @md_JudgeTop_Lower_Rel_DiffL_max, " +
                                " @md_JudgeTop_Lower_Rel_DiffH_min, @md_JudgeTop_Lower_Rel_DiffH_max, " +
                                " @md_JudgeTop_Lower_Calib_DiffL, @md_JudgeTop_Lower_Calib_DiffH, " +
                                " @md_ValueTop_Lower_Rel_DiffL, @md_ValueTop_Lower_Rel_DiffH, " +
                                " @md_HingeHole_Top_Result, " +

                            // Hinge Bottom
                            " @md_HingeHole_Bottom_File, @md_HingeHole_Bottom_Light, " +

                                // Hinge Bottom Upper
                                " @md_JudgeBottom_Upper_Rel_DiffL_min, @md_JudgeBottom_Upper_Rel_DiffL_max, " +
                                " @md_JudgeBottom_Upper_Rel_DiffH_min, @md_JudgeBottom_Upper_Rel_DiffH_max, " +
                                " @md_JudgeBottom_Upper_Calib_DiffL, @md_JudgeBottom_Upper_Calib_DiffH, " +
                                " @md_ValueBottom_Upper_Rel_DiffL, @md_ValueBottom_Upper_Rel_DiffH, " +
                                // Hinge Bottom Lower
                                " @md_JudgeBottom_Lower_Rel_DiffL_min, @md_JudgeBottom_Lower_Rel_DiffL_max, " +
                                " @md_JudgeBottom_Lower_Rel_DiffH_min, @md_JudgeBottom_Lower_Rel_DiffH_max, " +
                                " @md_JudgeBottom_Lower_Calib_DiffL, @md_JudgeBottom_Lower_Calib_DiffH, " +
                                " @md_ValueBottom_Lower_Rel_DiffL, @md_ValueBottom_Lower_Rel_DiffH, " +
                            " @md_HingeHole_Bottom_Result, " +

                            // Frame 
                            // Frame Top
                            " @md_FrameTop_File, @md_FrameTop_Light, " +
                            " @md_FrameTop_BaseH, @md_FrameTop_BaseT, " +
                            " @md_FrameTop_DiffH_min, @md_FrameTop_DiffH_max, " +
                            " @md_FrameTop_DiffT_min, @md_FrameTop_DiffT_max, " +
                            " @md_ValueFrameTop_BaseH, @md_ValueFrameTop_BaseT, " +
                            " @md_ValueFrameTop_DiffH_px, @md_ValueFrameTop_DiffT_px, @md_ValueFrameTop_DiffH, @md_ValueFrameTop_DiffT, " +
                            " @md_FrameTop_Result, " +

                            // Frame Bottom
                            " @md_FrameBottom_File, @md_FrameBottom_Light, " +
                            " @md_FrameBottom_BaseT, " +
                            " @md_FrameBottom_DiffT_min, @md_FrameBottom_DiffT_max, " +
                            " @md_ValueFrameBottom_BaseT, " +
                            " @md_ValueFrameBottom_DiffT_px, @md_ValueFrameBottom_DiffT, " +
                            " @md_FrameBottom_Result " +



                    " )");

                    string param = string.Empty;

                    // md_id
                    param = "@md_id";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.md_id;
                    // pd_VinNo
                    param = "@pd_VinNo";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_VinNo;
                    // pd_TotalResult
                    param = "@pd_TotalResult";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_TotalResult;
                    // pd_Date
                    param = "@pd_Date";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_Date;
                    // pd_Time
                    param = "@pd_Time";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_Time;

                    // Hinge 
                    {
                        #region Hinge Top 
                        {
                            // md_HingeHole_Top_File
                            param = "@md_HingeHole_Top_File";
                            qry.Parameters.Add(param, SqlDbType.VarChar);
                            qry.Parameters[param].Value = pd.md_HingeHole_Top_File;
                            // md_HingeHole_Top_Light
                            param = "@md_HingeHole_Top_Light";
                            qry.Parameters.Add(param, SqlDbType.Int);
                            qry.Parameters[param].Value = pd.md_HingeHole_Top_Light;

                            // Hinge Top Upper 
                            #region Hinge Top Upper
                            {
                                // md_JudgeTop_Upper_Rel_DiffL
                                param = "@md_JudgeTop_Upper_Rel_DiffL_min";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeTop_Upper_Rel_DiffL_min;
                                param = "@md_JudgeTop_Upper_Rel_DiffL_max";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeTop_Upper_Rel_DiffL_max;
                                // md_JudgeTop_Upper_Rel_DiffH
                                param = "@md_JudgeTop_Upper_Rel_DiffH_min";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeTop_Upper_Rel_DiffH_min;
                                param = "@md_JudgeTop_Upper_Rel_DiffH_max";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeTop_Upper_Rel_DiffH_max;
                                // md_JudgeTop_Upper_Calib_DiffL
                                param = "@md_JudgeTop_Upper_Calib_DiffL";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeTop_Upper_Calib_DiffL;
                                // md_JudgeTop_Upper_Calib_DiffH
                                param = "@md_JudgeTop_Upper_Calib_DiffH";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeTop_Upper_Calib_DiffH;

                                // md_ValueTop_Upper_Rel_DiffL
                                param = "@md_ValueTop_Upper_Rel_DiffL";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_ValueTop_Upper_Rel_DiffL;
                                // md_ValueTop_Upper_Rel_DiffH
                                param = "@md_ValueTop_Upper_Rel_DiffH";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_ValueTop_Upper_Rel_DiffH;
                            }
                            #endregion // Hinge Top Upper

                            // Hinge Top Lower 
                            #region Hinge Top Lower
                            {
                                // md_JudgeTop_Lower_Rel_DiffL
                                param = "@md_JudgeTop_Lower_Rel_DiffL_min";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeTop_Lower_Rel_DiffL_min;
                                param = "@md_JudgeTop_Lower_Rel_DiffL_max";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeTop_Lower_Rel_DiffL_max;
                                // md_JudgeTop_Lower_Rel_DiffH
                                param = "@md_JudgeTop_Lower_Rel_DiffH_min";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeTop_Lower_Rel_DiffH_min;
                                param = "@md_JudgeTop_Lower_Rel_DiffH_max";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeTop_Lower_Rel_DiffH_max;
                                // md_JudgeTop_Lower_Calib_DiffL
                                param = "@md_JudgeTop_Lower_Calib_DiffL";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeTop_Lower_Calib_DiffL;
                                // md_JudgeTop_Lower_Calib_DiffH
                                param = "@md_JudgeTop_Lower_Calib_DiffH";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeTop_Lower_Calib_DiffH;

                                // md_ValueTop_Lower_Rel_DiffL
                                param = "@md_ValueTop_Lower_Rel_DiffL";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_ValueTop_Lower_Rel_DiffL;
                                // md_ValueTop_Lower_Rel_DiffH
                                param = "@md_ValueTop_Lower_Rel_DiffH";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_ValueTop_Lower_Rel_DiffH;
                            }
                            #endregion // Hinge Top Lower 

                            // md_HingeHole_Top_Result
                            param = "@md_HingeHole_Top_Result";
                            qry.Parameters.Add(param, SqlDbType.Int);
                            qry.Parameters[param].Value = pd.md_HingeHole_Top_Result;
                        }
                        #endregion // Hinge Top

                        // Hinge Bottom
                        #region Hinge Bottom
                        {
                            // md_HingeHole_Bottom_File
                            param = "@md_HingeHole_Bottom_File";
                            qry.Parameters.Add(param, SqlDbType.VarChar);
                            qry.Parameters[param].Value = pd.md_HingeHole_Bottom_File;
                            // md_HingeHole_Bottom_Light
                            param = "@md_HingeHole_Bottom_Light";
                            qry.Parameters.Add(param, SqlDbType.Int);
                            qry.Parameters[param].Value = pd.md_HingeHole_Bottom_Light;

                            // Hinge Bottom Upper 
                            #region Hinge Bottom Upper
                            {
                                // md_JudgeBottom_Upper_Rel_DiffL
                                param = "@md_JudgeBottom_Upper_Rel_DiffL_min";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeBottom_Upper_Rel_DiffL_min;
                                param = "@md_JudgeBottom_Upper_Rel_DiffL_max";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeBottom_Upper_Rel_DiffL_max;
                                // md_JudgeBottom_Upper_Rel_DiffH
                                param = "@md_JudgeBottom_Upper_Rel_DiffH_min";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeBottom_Upper_Rel_DiffH_min;
                                param = "@md_JudgeBottom_Upper_Rel_DiffH_max";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeBottom_Upper_Rel_DiffH_max;
                                // md_JudgeBottom_Upper_Calib_DiffL
                                param = "@md_JudgeBottom_Upper_Calib_DiffL";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeBottom_Upper_Calib_DiffL;
                                // md_JudgeBottom_Upper_Calib_DiffH
                                param = "@md_JudgeBottom_Upper_Calib_DiffH";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeBottom_Upper_Calib_DiffH;

                                // md_ValueBottom_Upper_Rel_DiffL
                                param = "@md_ValueBottom_Upper_Rel_DiffL";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_ValueBottom_Upper_Rel_DiffL;
                                // md_ValueBottom_Upper_Rel_DiffH
                                param = "@md_ValueBottom_Upper_Rel_DiffH";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_ValueBottom_Upper_Rel_DiffH;
                            }
                            #endregion

                            // Hinge Bottom Lower
                            #region Hinge Bottom Lower
                            {
                                // md_JudgeBottom_Lower_Rel_DiffL
                                param = "@md_JudgeBottom_Lower_Rel_DiffL_min";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeBottom_Lower_Rel_DiffL_min;
                                param = "@md_JudgeBottom_Lower_Rel_DiffL_max";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeBottom_Lower_Rel_DiffL_max;
                                // md_JudgeBottom_Lower_Rel_DiffH
                                param = "@md_JudgeBottom_Lower_Rel_DiffH_min";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeBottom_Lower_Rel_DiffH_min;
                                param = "@md_JudgeBottom_Lower_Rel_DiffH_max";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeBottom_Lower_Rel_DiffH_max;
                                // md_JudgeBottom_Lower_Calib_DiffL
                                param = "@md_JudgeBottom_Lower_Calib_DiffL";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeBottom_Lower_Calib_DiffL;
                                // md_JudgeBottom_Lower_Calib_DiffH
                                param = "@md_JudgeBottom_Lower_Calib_DiffH";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_JudgeBottom_Lower_Calib_DiffH;

                                // md_ValueBottom_Lower_Rel_DiffL
                                param = "@md_ValueBottom_Lower_Rel_DiffL";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_ValueBottom_Lower_Rel_DiffL;
                                // md_ValueBottom_Lower_Rel_DiffH
                                param = "@md_ValueBottom_Lower_Rel_DiffH";
                                qry.Parameters.Add(param, SqlDbType.Float);
                                qry.Parameters[param].Value = pd.md_ValueBottom_Lower_Rel_DiffH;
                            }
                            #endregion

                            // md_HingeHole_Bottom_Result
                            param = "@md_HingeHole_Bottom_Result";
                            qry.Parameters.Add(param, SqlDbType.Int);
                            qry.Parameters[param].Value = pd.md_HingeHole_Bottom_Result;
                        }
                        #endregion // Hinge Bottom

                    }

                    // Frame
                    {
                        // Frame Top
                        {
                            // md_FrameTop_File
                            param = "@md_FrameTop_File";
                            qry.Parameters.Add(param, SqlDbType.VarChar);
                            qry.Parameters[param].Value = pd.md_FrameTop_File;
                            // md_FrameTop_Light
                            param = "@md_FrameTop_Light";
                            qry.Parameters.Add(param, SqlDbType.Int);
                            qry.Parameters[param].Value = pd.md_FrameTop_Light;

                            // md_FrameTop_BaseH
                            param = "@md_FrameTop_BaseH";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_FrameTop_BaseH;
                            // md_FrameTop_BaseT
                            param = "@md_FrameTop_BaseT";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_FrameTop_BaseT;
                            // md_FrameTop_DiffH
                            param = "@md_FrameTop_DiffH_min";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_FrameTop_DiffH_min;
                            param = "@md_FrameTop_DiffH_max";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_FrameTop_DiffH_max;
                            // md_FrameTop_DiffT
                            param = "@md_FrameTop_DiffT_min";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_FrameTop_DiffT_min;
                            param = "@md_FrameTop_DiffT_max";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_FrameTop_DiffT_max;

                            // md_ValueFrameTop_BaseH
                            param = "@md_ValueFrameTop_BaseH";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameTop_BaseH;
                            // md_ValueFrameTop_BaseT
                            param = "@md_ValueFrameTop_BaseT";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameTop_BaseT;
                            // md_ValueFrameTop_DiffH_px
                            param = "@md_ValueFrameTop_DiffH_px";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameTop_DiffH_px;
                            // md_ValueFrameTop_DiffT_px
                            param = "@md_ValueFrameTop_DiffT_px";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameTop_DiffT_px;
                            // md_ValueFrameTop_DiffH
                            param = "@md_ValueFrameTop_DiffH";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameTop_DiffH;
                            // md_ValueFrameTop_DiffT
                            param = "@md_ValueFrameTop_DiffT";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameTop_DiffT;

                            // md_FrameTop_Result
                            param = "@md_FrameTop_Result";
                            qry.Parameters.Add(param, SqlDbType.Int);
                            qry.Parameters[param].Value = pd.md_FrameTop_Result;
                        }
                    }

                    // Frame Bottom
                    {
                        // md_FrameBottom_File
                        param = "@md_FrameBottom_File";
                        qry.Parameters.Add(param, SqlDbType.VarChar);
                        qry.Parameters[param].Value = pd.md_FrameBottom_File;
                        // md_FrameBottom_Light
                        param = "@md_FrameBottom_Light";
                        qry.Parameters.Add(param, SqlDbType.Int);
                        qry.Parameters[param].Value = pd.md_FrameBottom_Light;

                        // md_FrameBottom_BaseT
                        param = "@md_FrameBottom_BaseT";
                        qry.Parameters.Add(param, SqlDbType.Float);
                        qry.Parameters[param].Value = pd.md_FrameBottom_BaseT;
                        // md_FrameBottom_DiffT
                        param = "@md_FrameBottom_DiffT_min";
                        qry.Parameters.Add(param, SqlDbType.Float);
                        qry.Parameters[param].Value = pd.md_FrameBottom_DiffT_min;
                        param = "@md_FrameBottom_DiffT_max";
                        qry.Parameters.Add(param, SqlDbType.Float);
                        qry.Parameters[param].Value = pd.md_FrameBottom_DiffT_max;

                        // md_ValueFrameBottom_BaseT
                        param = "@md_ValueFrameBottom_BaseT";
                        qry.Parameters.Add(param, SqlDbType.Float);
                        qry.Parameters[param].Value = pd.md_ValueFrameBottom_BaseT;
                        // md_ValueFrameBottom_DiffT_px
                        param = "@md_ValueFrameBottom_DiffT_px";
                        qry.Parameters.Add(param, SqlDbType.Float);
                        qry.Parameters[param].Value = pd.md_ValueFrameBottom_DiffT_px;
                        // md_ValueFrameBottom_DiffT
                        param = "@md_ValueFrameBottom_DiffT";
                        qry.Parameters.Add(param, SqlDbType.Float);
                        qry.Parameters[param].Value = pd.md_ValueFrameBottom_DiffT;

                        // md_FrameBottom_Result
                        param = "@md_FrameBottom_Result";
                        qry.Parameters.Add(param, SqlDbType.Int);
                        qry.Parameters[param].Value = pd.md_FrameBottom_Result;
                    }

                    // 연결 열기
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    // 실행
                    qry.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Util.WriteLog("Insert Product Error. " + ex.Message, "LogErr");
                    err = ex.Message;
                    result = false;
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("Insert Product Error. " + e2.Message, "LogErr");
                err = e2.Message;
                result = false;
            }

            return result;
        }


        public static bool UpdateDatabase(Product pd, SqlConnection conn, SqlCommand qry, out string err)
        {
            err = string.Empty;
            bool result = true;

            try
            {
                try
                {
                    Database.MakeQueryString(qry, "UPDATE Product " +
                         " SET " +
                        " pd_TotalResult=@pd_TotalResult " +

                                // Hinge 
                                // Hinge Top
                                // Hinge Top Upper
                                ((pd.md_HingeHole_Top_File != string.Empty) ? (
                                ", md_ValueTop_Upper_Rel_DiffL=@md_ValueTop_Upper_Rel_DiffL, " +
                                " md_ValueTop_Upper_Rel_DiffH=@md_ValueTop_Upper_Rel_DiffH, " +
                                // Hinge Top Lower
                                " md_ValueTop_Lower_Rel_DiffL=@md_ValueTop_Lower_Rel_DiffL, " +
                                " md_ValueTop_Lower_Rel_DiffH=@md_ValueTop_Lower_Rel_DiffH, " +
                                " md_HingeHole_Top_Result=@md_HingeHole_Top_Result ") : " ") +

                                 // Hinge Bottom
                                 // Hinge Bottom Upper
                                 ((pd.md_HingeHole_Bottom_File != string.Empty) ? (
                                ", md_ValueBottom_Upper_Rel_DiffL=@md_ValueBottom_Upper_Rel_DiffL, " +
                                " md_ValueBottom_Upper_Rel_DiffH=@md_ValueBottom_Upper_Rel_DiffH, " +
                                // Hinge Bottom Lower
                                " md_ValueBottom_Lower_Rel_DiffL=@md_ValueBottom_Lower_Rel_DiffL, " +
                                " md_ValueBottom_Lower_Rel_DiffH=@md_ValueBottom_Lower_Rel_DiffH, " +
                                " md_HingeHole_Bottom_Result=@md_HingeHole_Bottom_Result ") : " ") +

                                  // Frame 
                                  // Frame Top
                                  ((pd.md_FrameTop_File != string.Empty) ? (
                                    ", md_ValueFrameTop_BaseH=@md_ValueFrameTop_BaseH, " +
                                    " md_ValueFrameTop_BaseT=@md_ValueFrameTop_BaseT, " +
                                    " md_ValueFrameTop_DiffH_px=@md_ValueFrameTop_DiffH_px, " +
                                    " md_ValueFrameTop_DiffT_px=@md_ValueFrameTop_DiffT_px, " +
                                    " md_ValueFrameTop_DiffH=@md_ValueFrameTop_DiffH, " +
                                    " md_ValueFrameTop_DiffT=@md_ValueFrameTop_DiffT, " +
                                    " md_FrameTop_Result=@md_FrameTop_Result ") : " ") +
                                  // Frame Bottom
                                  ((pd.md_FrameBottom_File != string.Empty) ? (
                                    ", md_ValueFrameBottom_BaseT=@md_ValueFrameBottom_BaseT, " +
                                    " md_ValueFrameBottom_DiffT_px=@md_ValueFrameBottom_DiffT_px, " +
                                    " md_ValueFrameBottom_DiffT=@md_ValueFrameBottom_DiffT, " +
                                    " md_FrameBottom_Result=@md_FrameBottom_Result ") : " ") +


                            " WHERE pd_VinNo=@pd_VinNo");

                    string param = string.Empty;

                    // pd_TotalResult
                    param = "@pd_TotalResult";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_TotalResult;

                    // Hinge 
                    {
                        // Hinge Top
                        {
                            if (pd.md_HingeHole_Top_File != string.Empty)
                            {
                                // Hinge Top Upper 
                                #region Hinge Top Upper
                                {
                                    // md_ValueTop_Upper_Rel_DiffL
                                    param = "@md_ValueTop_Upper_Rel_DiffL";
                                    qry.Parameters.Add(param, SqlDbType.Float);
                                    qry.Parameters[param].Value = pd.md_ValueTop_Upper_Rel_DiffL;
                                    // md_ValueTop_Upper_Rel_DiffH
                                    param = "@md_ValueTop_Upper_Rel_DiffH";
                                    qry.Parameters.Add(param, SqlDbType.Float);
                                    qry.Parameters[param].Value = pd.md_ValueTop_Upper_Rel_DiffH;
                                }
                                #endregion

                                // Hinge Top Lower
                                #region Hinge Top Lower
                                {
                                    // md_ValueTop_Lower_Rel_DiffL
                                    param = "md_ValueTop_Lower_Rel_DiffL";
                                    qry.Parameters.Add(param, SqlDbType.Float);
                                    qry.Parameters[param].Value = pd.md_ValueTop_Lower_Rel_DiffL;
                                    // md_ValueTop_Lower_Rel_DiffH
                                    param = "@md_ValueTop_Lower_Rel_DiffH";
                                    qry.Parameters.Add(param, SqlDbType.Float);
                                    qry.Parameters[param].Value = pd.md_ValueTop_Lower_Rel_DiffH;
                                }
                                #endregion

                                // md_HingeHole_Top_Result
                                param = "@md_HingeHole_Top_Result";
                                qry.Parameters.Add(param, SqlDbType.Int);
                                qry.Parameters[param].Value = pd.md_HingeHole_Top_Result;
                            }
                        }

                        // Hinge Bottom
                        {
                            if (pd.md_HingeHole_Bottom_File != string.Empty)
                            {
                                // Hinge Bottom Upper 
                                #region Hinge Bottom Upper
                                {
                                    // md_ValueBottom_Upper_Rel_DiffL
                                    param = "@md_ValueBottom_Upper_Rel_DiffL";
                                    qry.Parameters.Add(param, SqlDbType.Float);
                                    qry.Parameters[param].Value = pd.md_ValueBottom_Upper_Rel_DiffL;
                                    // md_ValueBottom_Upper_Rel_DiffH
                                    param = "@md_ValueBottom_Upper_Rel_DiffH";
                                    qry.Parameters.Add(param, SqlDbType.Float);
                                    qry.Parameters[param].Value = pd.md_ValueBottom_Upper_Rel_DiffH;
                                }
                                #endregion

                                // Hinge Bottom Lower
                                #region Hinge Bottom Lower
                                {
                                    // md_ValueBottom_Lower_Rel_DiffL
                                    param = "md_ValueBottom_Lower_Rel_DiffL";
                                    qry.Parameters.Add(param, SqlDbType.Float);
                                    qry.Parameters[param].Value = pd.md_ValueBottom_Lower_Rel_DiffL;
                                    // md_ValueBottom_Lower_Rel_DiffH
                                    param = "@md_ValueBottom_Lower_Rel_DiffH";
                                    qry.Parameters.Add(param, SqlDbType.Float);
                                    qry.Parameters[param].Value = pd.md_ValueBottom_Lower_Rel_DiffH;
                                }
                                #endregion

                                // md_HingeHole_Bottom_Result
                                param = "@md_HingeHole_Bottom_Result";
                                qry.Parameters.Add(param, SqlDbType.Int);
                                qry.Parameters[param].Value = pd.md_HingeHole_Bottom_Result;
                            }
                        }

                    }

                    // Frame
                    {
                        // Frame Top
                        if (pd.md_FrameTop_File != string.Empty)
                        {
                            // md_ValueFrameTop_BaseH
                            param = "@md_ValueFrameTop_BaseH";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameTop_BaseH;
                            // md_ValueFrameTop_BaseT
                            param = "@md_ValueFrameTop_BaseT";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameTop_BaseT;
                            // md_ValueFrameTop_DiffH_px
                            param = "@md_ValueFrameTop_DiffH_px";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameTop_DiffH_px;
                            // md_ValueFrameTop_DiffT_px
                            param = "@md_ValueFrameTop_DiffT_px";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameTop_DiffT_px;
                            // md_ValueFrameTop_DiffH
                            param = "@md_ValueFrameTop_DiffH";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameTop_DiffH;
                            // md_ValueFrameTop_DiffT
                            param = "@md_ValueFrameTop_DiffT";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameTop_DiffT;

                            // md_FrameTop_Result
                            param = "@md_FrameTop_Result";
                            qry.Parameters.Add(param, SqlDbType.Int);
                            qry.Parameters[param].Value = pd.md_FrameTop_Result;
                        }

                        // Frame Bottom
                        if (pd.md_FrameBottom_File != string.Empty)
                        {
                            // md_ValueFrameBottom_BaseT
                            param = "@md_ValueFrameBottom_BaseT";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameBottom_BaseT;
                            // md_ValueFrameBottom_DiffT_px
                            param = "@md_ValueFrameBottom_DiffT_px";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameBottom_DiffT_px;
                            // md_ValueFrameBottom_DiffT
                            param = "@md_ValueFrameBottom_DiffT";
                            qry.Parameters.Add(param, SqlDbType.Float);
                            qry.Parameters[param].Value = pd.md_ValueFrameBottom_DiffT;

                            // md_FrameBottom_Result
                            param = "@md_FrameBottom_Result";
                            qry.Parameters.Add(param, SqlDbType.Int);
                            qry.Parameters[param].Value = pd.md_FrameBottom_Result;
                        }


                    }

                    // pd_VinNo
                    param = "@pd_VinNo";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_VinNo;

                    // 연결 열기
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    // 실행
                    qry.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Util.WriteLog("Update Product Error. " + ex.Message, "LogErr");
                    err = ex.Message;
                    result = false;
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("Update Product Error. " + e2.Message, "LogErr");
                err = e2.Message;
                result = false;
            }

            return result;
        }

        internal void setModelValues(Model model)
        {
            md_id = model.md_id;
            md_HingeHole_Top_Light = model.md_HingeHole_Top_Light;

            md_JudgeTop_Upper_Rel_DiffL_min = model.md_JudgeTop_Upper_Rel_DiffL_min;
            md_JudgeTop_Upper_Rel_DiffL_max = model.md_JudgeTop_Upper_Rel_DiffL_max;
            md_JudgeTop_Upper_Rel_DiffH_min = model.md_JudgeTop_Upper_Rel_DiffH_min;
            md_JudgeTop_Upper_Rel_DiffH_max = model.md_JudgeTop_Upper_Rel_DiffH_max;
            md_JudgeTop_Upper_Calib_DiffH = model.md_JudgeTop_Upper_Calib_DiffH;
            md_JudgeTop_Upper_Calib_DiffL = model.md_JudgeTop_Upper_Calib_DiffL;

            md_JudgeTop_Lower_Rel_DiffL_min = model.md_JudgeTop_Lower_Rel_DiffL_min;
            md_JudgeTop_Lower_Rel_DiffL_max = model.md_JudgeTop_Lower_Rel_DiffL_max;
            md_JudgeTop_Lower_Rel_DiffH_min = model.md_JudgeTop_Lower_Rel_DiffH_min;
            md_JudgeTop_Lower_Rel_DiffH_max = model.md_JudgeTop_Lower_Rel_DiffH_max;
            md_JudgeTop_Lower_Calib_DiffH = model.md_JudgeTop_Lower_Calib_DiffH;
            md_JudgeTop_Lower_Calib_DiffL = model.md_JudgeTop_Lower_Calib_DiffL;

            md_HingeHole_Bottom_Light = model.md_HingeHole_Bottom_Light;

            md_JudgeBottom_Upper_Rel_DiffL_min = model.md_JudgeBottom_Upper_Rel_DiffL_min;
            md_JudgeBottom_Upper_Rel_DiffL_max = model.md_JudgeBottom_Upper_Rel_DiffL_max;
            md_JudgeBottom_Upper_Rel_DiffH_min = model.md_JudgeBottom_Upper_Rel_DiffH_min;
            md_JudgeBottom_Upper_Rel_DiffH_max = model.md_JudgeBottom_Upper_Rel_DiffH_max;
            md_JudgeBottom_Upper_Calib_DiffH = model.md_JudgeBottom_Upper_Calib_DiffH;
            md_JudgeBottom_Upper_Calib_DiffL = model.md_JudgeBottom_Upper_Calib_DiffL;

            md_JudgeBottom_Lower_Rel_DiffL_min = model.md_JudgeBottom_Lower_Rel_DiffL_min;
            md_JudgeBottom_Lower_Rel_DiffL_max = model.md_JudgeBottom_Lower_Rel_DiffL_max;
            md_JudgeBottom_Lower_Rel_DiffH_min = model.md_JudgeBottom_Lower_Rel_DiffH_min;
            md_JudgeBottom_Lower_Rel_DiffH_max = model.md_JudgeBottom_Lower_Rel_DiffH_max;
            md_JudgeBottom_Lower_Calib_DiffH = model.md_JudgeBottom_Lower_Calib_DiffH;
            md_JudgeBottom_Lower_Calib_DiffL = model.md_JudgeBottom_Lower_Calib_DiffL;

            md_FrameTop_Light = model.md_FrameTop_Light;
            md_FrameTop_BaseH = model.md_FrameTop_BaseH;
            md_FrameTop_BaseT = model.md_FrameTop_BaseT;
            md_FrameTop_DiffH_min = model.md_FrameTop_DiffH_min;
            md_FrameTop_DiffH_max = model.md_FrameTop_DiffH_max;
            md_FrameTop_DiffT_min = model.md_FrameTop_DiffT_min;
            md_FrameTop_DiffT_max = model.md_FrameTop_DiffT_max;

            md_FrameBottom_Light = model.md_FrameBottom_Light;
            md_FrameBottom_BaseT = model.md_FrameBottom_BaseT;
            md_FrameBottom_DiffT_min = model.md_FrameBottom_DiffT_min;
            md_FrameBottom_DiffT_max = model.md_FrameBottom_DiffT_max;
        }

        internal static void Rejudge(Product pd)
        {
        }

        internal static double getRecentAverageValues(SqlConnection conn, SqlCommand qry, int md_id, string columnName, int count, int upperCount, int lowerCount, out string err)
        {
            double value = 0.0;
            err = string.Empty;
            List<Double> valueList = new List<double>();

            try
            {
                try
                {
                    Database.MakeQueryString(qry, "SELECT TOP " + count.ToString() + " " + columnName + " FROM Product WHERE md_id=" +
                        md_id.ToString() + " ORDER BY pd_id DESC ");
                    qry.CommandTimeout = 10;
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    using (SqlDataReader reader = qry.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                double tempVal = Database.getDoubleValue(reader, columnName);
                                valueList.Add(tempVal);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.WriteLog("getRecentAverageValues 1 Error. " + ex.Message, "LogErr");
                    err = ex.Message;
                    return 0.0;
                }
                finally
                {
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("getRecentAverageValues 2 Error. " + e2.Message, "LogErr");
                err = e2.Message;
            }

            // Lower + Upper
            if (valueList.Count <= (lowerCount + upperCount))
            {
                err = "Count is not enough: " + valueList.Count.ToString();
                return value;
            }

            // sorting
            valueList.Sort();

            // remove upper
            for (int i = 0; i < upperCount; i++)
            {
                valueList.RemoveAt(valueList.Count - 1);
            }
            for (int i = 0; i < lowerCount; i++)
            {
                valueList.RemoveAt(0);
            }
            value = valueList.Average();


            return value;
        }
    }

}

namespace VISION_Data
{
    public enum VISION_LINE { LINE_NONE, LINE_PRIMER_FRONT, LINE_PRIMER_REAR, LINE_SEALER_FRONT, LINE_SEALER_REAR, LINE_BODY_FRONT, LINE_BODY_REAR }
    public enum VISION_DATA_TYPE { DATA_NONE, DATA_BODY, DATA_SEALER, DATA_PRIMER }
    public enum VISION_RESULT { VISION_RESULT_NONE, VISION_RESULT_NG, VISION_RESULT_OK }
    public enum VISION_BODY_RESULT { VISION_BODY_RESULT_NONE, VISION_BODY_RESULT_NG, VISION_BODY_RESULT_OVER, VISION_BODY_RESULT_OK, VISION_BODY_RESULT_OK1, VISION_BODY_RESULT_PASS }
    public enum VISION_SEALER_RESULT { VISION_SEALER_RESULT_NONE, VISION_SEALER_RESULT_NG, VISION_SEALER_RESULT_OK }
    public enum VISION_PRIMER_RESULT { VISION_PRIMER_RESULT_NONE, VISION_PRIMER_RESULT_NG, VISION_PRIMER_RESULT_OK }
    public class VISION_PRODUCT
    {
        public int pd_id;
        public VISION_LINE pd_LineNo = VISION_LINE.LINE_NONE;
        public VISION_DATA_TYPE pd_DataType = VISION_DATA_TYPE.DATA_NONE;
        public DateTime pd_DateTime;
        public string pd_Model = string.Empty;
        public string pd_VinNo = string.Empty;
        public int pd_SeqNo;
        public string pd_FileImage = string.Empty;
        public string pd_FileMovie = string.Empty;
        public int pd_PartCount = 0;
        public string pd_PartResult = string.Empty;
        public VISION_BODY_RESULT pd_BodyResult = VISION_BODY_RESULT.VISION_BODY_RESULT_OK;
        public VISION_SEALER_RESULT pd_SealerResult = VISION_SEALER_RESULT.VISION_SEALER_RESULT_NONE;
        public VISION_PRIMER_RESULT pd_PrimerResult = VISION_PRIMER_RESULT.VISION_PRIMER_RESULT_NONE;
        public VISION_RESULT pd_TotalResult = VISION_RESULT.VISION_RESULT_OK;

        public static string[] Vision_Body_Result = { "NA", "NG", "OVER", "OK", "OK1", "PASS" };
        public static string[] Vision_Sealer_Result = { "NA", "NG", "OK" };
        public static string[] Vision_Primer_Result = { "NA", "NG", "OK" };

        public static string TableName = "VisionProduct";
        public static string TableName_Server = "VisionProduct_Server";
        public static string TableName_Raw = "VisionProduct_Raw";

        public static string[] Vision_Lines = { "NONE", "Front Primer", "Rear Primer", "Front Sealer", "Rear Sealer", "Front Body", "Rear Body" };

        public VISION_PRODUCT()
        {

        }

        internal static List<VISION_PRODUCT> getSearchList(SqlConnection conn, SqlCommand qry, string cmd, out string err)
        {
            err = string.Empty;
            List<VISION_PRODUCT> list = new List<VISION_PRODUCT>();

            try
            {
                try
                {
                    Database.MakeQueryString(qry, cmd);
                    qry.CommandTimeout = 3600;
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    using (SqlDataReader reader = qry.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                VISION_PRODUCT pd = new VISION_PRODUCT();
                                pd.pd_id = Database.getIntValue(reader, "pd_id");
                                pd.pd_DataType = (VISION_DATA_TYPE)Database.getIntValue(reader, "pd_DataType");
                                pd.pd_DateTime = Database.getDateTimeValue(reader, "pd_DateTime");
                                pd.pd_Model = Database.getStringValue(reader, "pd_Model");
                                pd.pd_VinNo = Database.getStringValue(reader, "pd_VinNo");
                                pd.pd_SeqNo = Database.getIntValue(reader, "pd_SeqNo");
                                pd.pd_FileImage = Database.getStringValue(reader, "pd_FileImage");
                                pd.pd_FileMovie = Database.getStringValue(reader, "pd_FileMovie");
                                pd.pd_PartCount = Database.getIntValue(reader, "pd_PartCount");
                                pd.pd_PartResult = Database.getStringValue(reader, "pd_PartResult");
                                pd.pd_BodyResult = (VISION_BODY_RESULT)Database.getIntValue(reader, "pd_BodyResult");
                                pd.pd_SealerResult = (VISION_SEALER_RESULT)Database.getIntValue(reader, "pd_SealerResult");
                                pd.pd_PrimerResult = (VISION_PRIMER_RESULT)Database.getIntValue(reader, "pd_PrimerResult");
                                pd.pd_TotalResult = (VISION_RESULT)Database.getIntValue(reader, "pd_TotalResult");

                                list.Add(pd);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.WriteLog("getSearchList List Loading Error 1. " + ex.Message, "LogErr");
                    err = ex.Message;
                    return null;
                }
                finally
                {
                }
            }
            catch (Exception e2)
            {
                Util.WriteLog("getSearchList List Loading Error 2. " + e2.Message, "LogErr");
                err = e2.Message;
                return null;
            }
            return list;
        }

        internal static void SaveDatabase(VISION_PRODUCT pd, Object lockIt, out string err)
        {
            err = string.Empty;
            // Database Insert
            SqlConnection conn = Database.getDataConn();
            SqlCommand qry = Database.getDataQuery();
            try
            {
                try
                {
                    Database.MakeQueryString(qry, "INSERT INTO " + TableName + " " +
                        "( " +
                        " pd_DataType, pd_DateTime, pd_Model, pd_VinNo, pd_SeqNo, pd_FileImage, pd_FileMovie, " +
                        " pd_PartCount, pd_PartResult, " +
                        " pd_BodyResult, pd_SealerResult, pd_PrimerResult, " +
                        " pd_TotalResult ) " +
                        " values " +
                        "( " +
                        " @pd_DataType, @pd_DateTime, @pd_Model,  @pd_VinNo, @pd_SeqNo, @pd_FileImage, @pd_FileMovie, " +
                        " @pd_PartCount, @pd_PartResult, " +
                        " @pd_BodyResult, @pd_SealerResult, @pd_PrimerResult, " +
                        " @pd_TotalResult " +
                        " )");
                    string param = string.Empty;

                    // pd_DataType
                    param = "@pd_DataType";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_DataType;

                    // pd_DateTime
                    param = "@pd_DateTime";
                    qry.Parameters.Add(param, SqlDbType.DateTime);
                    qry.Parameters[param].Value = pd.pd_DateTime;

                    // pd_Model
                    param = "@pd_Model";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_Model;

                    // pd_VinNo
                    param = "@pd_VinNo";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_VinNo;

                    // pd_SeqNo
                    param = "@pd_SeqNo";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_SeqNo;

                    // pd_FileImage
                    param = "@pd_FileImage";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_FileImage;

                    // pd_FileMovie
                    param = "@pd_FileMovie";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_FileMovie;

                    // pd_PartCount
                    param = "@pd_PartCount";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_PartCount;

                    // pd_PartResult
                    param = "@pd_PartResult";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_PartResult;

                    // pd_BodyResult
                    param = "@pd_BodyResult";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_BodyResult;

                    // pd_SealerResult
                    param = "@pd_SealerResult";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_SealerResult;

                    // pd_PrimerResult
                    param = "@pd_PrimerResult";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_PrimerResult;

                    // pd_TotalResult
                    param = "@pd_TotalResult";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = (int)pd.pd_TotalResult;

                    // 연결 열기
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    // 실행
                    qry.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    err = "Database Insert Error 1: " + ex.Message;
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception e2)
            {
                err = "Database Insert Error 2: " + e2.Message;
            }

            // 성공하든 실패하든 Data 파일에 기록 - 처리된 파일명을 저장하여 중복 저장되지 않도록
            lock (lockIt)
            {
                FileStream myFileStream = null;
                try
                {
                    string sFile = Util.GetWorkingDirectory() + "\\DATA\\" + pd.pd_DateTime.ToString("yyyy-MM-dd") + ".txt";
                    myFileStream = new FileStream(sFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    myFileStream.Seek(0, SeekOrigin.End);
                    string sContents = pd.pd_FileImage + "\r\n";
                    myFileStream.Write(Encoding.Default.GetBytes(sContents), 0, Encoding.Default.GetByteCount(sContents));
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                }
                finally
                {
                    if (null != myFileStream)
                        myFileStream.Close();
                }
            }
        }

        internal static VISION_RESULT SaveDatabase_Server(VISION_PRODUCT pd, out string err)
        {
            VISION_RESULT vr = VISION_RESULT.VISION_RESULT_NONE;
            err = string.Empty;
            // Database Insert
            SqlConnection conn = Database.getServerConn();
            SqlCommand qry = Database.getServerQuery();
            try
            {
                try
                {
                    Database.MakeQueryString(qry, "INSERT INTO " + TableName_Server + " " +
                        "( " +
                        " pd_id, pd_LineNo, pd_DataType, pd_DateTime, pd_Model, pd_VinNo, pd_SeqNo, pd_FileImage, pd_FileMovie, " +
                        " pd_PartCount, pd_PartResult, " +
                        " pd_BodyResult, pd_SealerResult, pd_PrimerResult, " +
                        " pd_TotalResult ) " +
                        " values " +
                        "( " +
                        " @pd_id, @pd_LineNo, @pd_DataType, @pd_DateTime, @pd_Model,  @pd_VinNo, @pd_SeqNo, @pd_FileImage, @pd_FileMovie, " +
                        " @pd_PartCount, @pd_PartResult, " +
                        " @pd_BodyResult, @pd_SealerResult, @pd_PrimerResult, " +
                        " @pd_TotalResult " +
                        " )");
                    string param = string.Empty;

                    // pd_id
                    param = "@pd_id";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_id;

                    // pd_LineNo
                    param = "@pd_LineNo";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_LineNo;

                    // pd_DataType
                    param = "@pd_DataType";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_DataType;

                    // pd_DateTime
                    param = "@pd_DateTime";
                    qry.Parameters.Add(param, SqlDbType.DateTime);
                    qry.Parameters[param].Value = pd.pd_DateTime;

                    // pd_Model
                    param = "@pd_Model";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_Model;

                    // pd_VinNo
                    param = "@pd_VinNo";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_VinNo;

                    // pd_SeqNo
                    param = "@pd_SeqNo";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_SeqNo;

                    // pd_FileImage
                    param = "@pd_FileImage";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_FileImage;

                    // pd_FileMovie
                    param = "@pd_FileMovie";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_FileMovie;

                    // pd_PartCount
                    param = "@pd_PartCount";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_PartCount;

                    // pd_PartResult
                    param = "@pd_PartResult";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_PartResult;

                    // pd_BodyResult
                    param = "@pd_BodyResult";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_BodyResult;

                    // pd_SealerResult
                    param = "@pd_SealerResult";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_SealerResult;

                    // pd_PrimerResult
                    param = "@pd_PrimerResult";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_PrimerResult;

                    // pd_TotalResult
                    param = "@pd_TotalResult";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = (int)pd.pd_TotalResult;

                    // 연결 열기
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    // 실행
                    qry.ExecuteNonQuery();

                    // 최종 결과 
                    vr = pd.pd_TotalResult;
                }
                catch (Exception ex)
                {
                    err = "Database Insert Error 1: " + ex.Message;
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception e2)
            {
                err = "Database Insert Error 2: " + e2.Message;
            }

            return vr;
        }

        internal static void SaveDatabase_Raw(VISION_PRODUCT pd, out string err)
        {
            string ini = Util.GetWorkingDirectory() + "\\server.ini";
            int movie = Int32.Parse(Util.GetIniFileString(ini, "DATA", "Movie", "0"));
            err = string.Empty;
            // Database Insert
            SqlConnection conn = Database.getRawDataConn();
            SqlCommand qry = Database.getRawDataQuery();
            try
            {
                try
                {
                    byte[] imageBytes = GetDataBytes(pd.pd_FileImage, out err);
                    byte[] movieBytes = null;
                    if (movie == 1)
                        movieBytes = GetDataBytes(pd.pd_FileMovie, out err);

                    if (movie == 1)
                    {
                        Database.MakeQueryString(qry, "INSERT INTO " + TableName_Raw + " " +
                            " ( " +
                            " pd_id, pd_LineNo, pd_DateTime, pd_DataType, pd_FileImage " +
                            ((imageBytes != null) ? ", pd_Image" : " ") +
                            ", pd_FileMovie " +
                            ((movieBytes != null) ? ", pd_Movie" : " ") +
                            " ) " +
                            " values " +
                            "( " +
                            " @pd_id, @pd_LineNo, @pd_DateTime, @pd_DataType, @pd_FileImage " +
                            ((imageBytes != null) ? ", @pd_Image" : " ") +
                            ", @pd_FileMovie " +
                            ((movieBytes != null) ? ", @pd_Movie" : " ") +
                            " )");
                    }
                    else
                    {
                        Database.MakeQueryString(qry, "INSERT INTO " + TableName_Raw + " " +
                            " ( " +
                            " pd_id, pd_LineNo, pd_DateTime, pd_DataType, pd_FileImage " +
                            ((imageBytes != null) ? ", pd_Image" : " ") +
                            ", pd_FileMovie " +
                            " ) " +
                            " values " +
                            "( " +
                            " @pd_id, @pd_LineNo, @pd_DateTime, @pd_DataType, @pd_FileImage " +
                            ((imageBytes != null) ? ", @pd_Image" : " ") +
                            ", @pd_FileMovie " +
                            " )");
                    }
                    string param = string.Empty;

                    // pd_id
                    param = "@pd_id";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_id;

                    // pd_LineNo
                    param = "@pd_LineNo";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_LineNo;

                    // pd_DateTime
                    param = "@pd_DateTime";
                    qry.Parameters.Add(param, SqlDbType.DateTime);
                    qry.Parameters[param].Value = pd.pd_DateTime;

                    // pd_DataType
                    param = "@pd_DataType";
                    qry.Parameters.Add(param, SqlDbType.Int);
                    qry.Parameters[param].Value = pd.pd_DataType;

                    // pd_FileImage
                    param = "@pd_FileImage";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_FileImage;

                    // pd_Image
                    if (imageBytes != null)
                        qry.Parameters.Add("@pd_Image", SqlDbType.VarBinary, imageBytes.Length).Value = imageBytes;

                    // pd_FileMovie
                    param = "@pd_FileMovie";
                    qry.Parameters.Add(param, SqlDbType.VarChar);
                    qry.Parameters[param].Value = pd.pd_FileMovie;

                    // pd_Movie
                    if (movieBytes != null && (movie == 1))
                        qry.Parameters.Add("@pd_Movie", SqlDbType.VarBinary, movieBytes.Length).Value = movieBytes;

                    // 연결 열기
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    // 실행
                    qry.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    err += "Server Database Insert Error 1: " + ex.Message;
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception e2)
            {
                err += "Server Database Insert Error 2: " + e2.Message;
            }
        }

        internal static void SendNGImageMovie(VISION_PRODUCT pd, out string err)
        {
            err = string.Empty;

            string iniServer = Util.GetWorkingDirectory() + "\\Server.ini";
            string ip = Util.GetIniFileString(iniServer, "TCP", "SERVER", "10.114.43.184");
            int port = Int32.Parse(Util.GetIniFileString(iniServer, "TCP", "PORT", "50001"));

            try
            {
                string date = pd.pd_DateTime.ToString("yyyyMMdd");
                // 업로드 할 파일
                string filename = string.Empty;
                filename = pd.pd_FileImage;
                if (File.Exists(filename))
                {
                    string fileTitle = filename.Substring(filename.LastIndexOf("\\") + 1);
                    int len = fileTitle.Length;

                    FileInfo fi = new FileInfo(filename);
                    long lenFile = fi.Length;

                    string pre = ((int)pd.pd_LineNo).ToString() + ":" + date + ":" + len.ToString() + ":" + lenFile.ToString() + ":" + fileTitle + ":| ";
                    byte[] preBuffer = Encoding.Unicode.GetBytes(pre);

                    Util.WriteLog("***** Send Image Started: " + filename);
                    // Connect the socket to the remote endpoint.
                    // Create a TCP socket.
                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress ipAddr = IPAddress.Parse(ip);
                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

                    client.Connect(ipEndPoint);

                    client.SendFile(filename, preBuffer, null, TransmitFileOptions.UseDefaultWorkerThread);
                    // Release the socket.
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    Util.WriteLog("##### Send Image Fnished: " + filename);
                }
                filename = pd.pd_FileMovie;
                if (File.Exists(filename))
                {
                    string fileTitle = filename.Substring(filename.LastIndexOf("\\") + 1);
                    int len = fileTitle.Length;

                    FileInfo fi = new FileInfo(filename);
                    long lenFile = fi.Length;

                    string pre = ((int)pd.pd_LineNo).ToString() + ":" + date + ":" + len.ToString() + ":" + lenFile.ToString() + ":" + fileTitle + ":| ";
                    byte[] preBuffer = Encoding.Unicode.GetBytes(pre);

                    Util.WriteLog("***** Send Movie Started: " + filename);
                    // Connect the socket to the remote endpoint.
                    // Create a TCP socket.
                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress ipAddr = IPAddress.Parse(ip);
                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

                    client.Connect(ipEndPoint);

                    client.SendFile(filename, preBuffer, null, TransmitFileOptions.UseDefaultWorkerThread);
                    // Release the socket.
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    Util.WriteLog("##### Send Movie Fnished: " + filename);
                }
            }
            catch (Exception e)
            {
                err = e.Message;
            }
            finally
            {
            }
        }

        private static byte[] GetDataBytes(string file, out string err)
        {
            err = string.Empty;
            byte[] buf = null;
            if (File.Exists(file))
            {
                try
                {
                    using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            buf = reader.ReadBytes((int)stream.Length);
                        }
                    }
                }
                catch (Exception exVar)
                {
                    err += "File: " + file + "\r\n";
                    err += exVar.Message;
                }
            }
            return buf;
        }

        internal static VISION_BODY_RESULT GetBodyResult(string ini, string f, Bitmap bmp)
        {
            int x = Int32.Parse(Util.GetIniFileString(ini, "Body_Result", "Left", "0"));
            int y = Int32.Parse(Util.GetIniFileString(ini, "Body_Result", "Top", "0"));
            Color cl = bmp.GetPixel(x, y);

            int R = Int32.Parse(Util.GetIniFileString(ini, "Body_Result", "R", "0"));
            int B = Int32.Parse(Util.GetIniFileString(ini, "Body_Result", "B", "0"));

            if ((cl.R > 200) && (cl.G < 100) && (cl.B < 100))
                return VISION_BODY_RESULT.VISION_BODY_RESULT_NG;

            if ((cl.R < 100) && (cl.G < 50) && (cl.B > 100))
                return VISION_BODY_RESULT.VISION_BODY_RESULT_OVER;

            //Debug.Assert(cl.B > B);
            return VISION_BODY_RESULT.VISION_BODY_RESULT_NONE;
        }

        internal static void UpdateVIN(string pd_id, string vinOld, string vinNew, out string err)
        {
            err = string.Empty;
            SqlConnection conn = Database.getDataConn();
            SqlCommand qry = Database.getDataQuery();
            try
            {
                Database.MakeQueryString(qry, "UPDATE Product SET pd_VinNo = @pd_VinNo WHERE pd_id = " + pd_id);

                string param = string.Empty;

                param = "@pd_VinNo";
                qry.Parameters.Add(param, SqlDbType.VarChar);
                qry.Parameters[param].Value = vinNew;

                // 연결 열기
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                qry.ExecuteNonQuery();

                err = "VIN Changed.\tBefore: " + vinOld + "\tAfter: " + vinNew;
            }
            catch (Exception ex)
            {
                err = "Change VIN Error. " + ex.Message;
            }
            finally
            {
                conn.Close();
            }

        }

        internal static bool ParseData(string ini, string f, VISION_DATA_TYPE dataType, out DateTime dt, out string model, out int cmt, out string err)
        {
            bool result = true;
            dt = DateTime.MinValue;
            model = string.Empty;
            cmt = 0;
            err = string.Empty;

            try
            {
                int idx = f.LastIndexOf("\\");
                string file = f.Substring(idx + 1);
                string date = f.Substring(idx - 10, 10);
                dt = new DateTime(Int32.Parse(date.Substring(0, 4)), Int32.Parse(date.Substring(5, 2)), Int32.Parse(date.Substring(8, 2)),
                                    Int32.Parse(file.Substring(11, 2)), Int32.Parse(file.Substring(13, 2)), Int32.Parse(file.Substring(15, 2)));

                // 프런트는 2자리, 리어는 6자리
                int lengthModel = Int32.Parse(Util.GetIniFileString(ini, "DATA", "ModelLength", "2"));
                switch (dataType)
                {
                    case VISION_DATA_TYPE.DATA_NONE:
                        break;
                    case VISION_DATA_TYPE.DATA_BODY:
                        model = file.Substring(18, lengthModel);
                        cmt = Int32.Parse(file.Substring(18 + lengthModel + 1, 4));
                        break;
                    case VISION_DATA_TYPE.DATA_SEALER:
                        break;
                    case VISION_DATA_TYPE.DATA_PRIMER:
                        model = file.Substring(18, lengthModel);
                        cmt = Int32.Parse(file.Substring(18 + lengthModel + 1, 4));
                        break;
                    default:
                        break;
                }

            }
            catch (Exception e)
            {
                err = "ParseData Exception: " + e.Message + "\t" + f;
                return false;
            }

            return result;
        }

        internal static bool IsExist(DateTime dt, string f, int day = 10)
        {
            bool exist = false;
            try
            {
                string sFile = Util.GetWorkingDirectory() + "\\DATA\\" + dt.ToString("yyyy-MM-dd") + ".txt";
                foreach (string line in File.ReadLines(sFile))
                {
                    if (line.Contains(f))
                    {
                        exist = true;
                        break;
                    }
                }

                // 10일 이상 지난 파일은 삭제
                string fileDelete = Util.GetWorkingDirectory() + "\\DATA\\" + (DateTime.Now.AddDays(-1 * day)).ToString("yyyy-MM-dd") + ".txt";
                if (File.Exists(fileDelete))
                    File.Delete(fileDelete);
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }
            return exist;
        }

        internal static string GetPrimerVinNO(string f, out string err)
        {
            err = string.Empty;
            string result = string.Empty;
            try
            {
                result = f.Substring(f.Length - 14, 10);
            }
            catch (Exception e)
            {
                err = "GetPrimerVinNO Exception: " + e.Message + "\t" + f;
                return string.Empty;
            }
            return result;
        }

        internal static void GetPrimerResult(string ini, string f, Bitmap bmp, out int partCount, out string partResult)
        {
            partCount = 0;
            partResult = string.Empty;
            int maxCount = 10;
            int x, y;
            Color cl;
            for (int i = 0; i < maxCount; i++)
            {
                x = Int32.Parse(Util.GetIniFileString(ini, "Primer_Part", "x" + (i + 1).ToString(), "0"));
                y = Int32.Parse(Util.GetIniFileString(ini, "Primer_Part", "y" + (i + 1).ToString(), "0"));
                cl = bmp.GetPixel(x, y);
                if ((cl.R > 200) && (cl.G < 100) && (cl.B < 100))
                    partResult += ((int)VISION_PRIMER_RESULT.VISION_PRIMER_RESULT_NG).ToString();
                else if ((cl.G > 200) && (cl.R > 100) && (cl.B < 100))
                    partResult += ((int)VISION_PRIMER_RESULT.VISION_PRIMER_RESULT_OK).ToString();
                else
                    partResult += ((int)VISION_PRIMER_RESULT.VISION_PRIMER_RESULT_NONE).ToString();

                partCount++;
            }
        }

        internal static bool ParseDataSealer(string f, VISION_DATA_TYPE dataType, out int seq, out string vin, out DateTime dt, out VISION_RESULT res, out string err)
        {
            err = string.Empty;
            bool result = true;
            dt = DateTime.MinValue;
            seq = 0;
            vin = string.Empty;
            res = VISION_RESULT.VISION_RESULT_NONE;

            try
            {
                int idx = f.LastIndexOf("\\");
                string file = f.Substring(idx + 1);
                seq = Int32.Parse(file.Substring(0, 4));
                vin = file.Substring(5, 10);
                dt = File.GetLastWriteTime(f);
                string s = file.Substring(file.LastIndexOf(".") - 2, 2).ToUpper();
                if (s == "OK")
                    res = VISION_RESULT.VISION_RESULT_OK;
                else
                    res = VISION_RESULT.VISION_RESULT_NG;
            }
            catch (Exception e)
            {
                err = "Sealer ParseData Exception: " + e.Message + "\t" + f;
                result = false;
            }

            return result;

        }

        internal static int GetNGCount(VISION_LINE line, DateTime from, DateTime to, string model, bool matchModel)
        {
            int count = 0;
            try
            {
            }
            catch (Exception e2)
            {
                Util.WriteLog("GetNGCount. " + e2.Message, "LogErr", "GVMS_DB");
            }
            return count;
        }

        internal static double GetOKRatio(VISION_DATA_TYPE dt, DateTime from, DateTime to)
        {
            double ratio = 0;
            try
            {
            }
            catch (Exception e2)
            {
                Util.WriteLog("GetOKRatio. " + e2.Message, "LogErr", "GVMS_DB");
            }
            return ratio;
        }

        internal static DateTime GetDailyLastTime(VISION_LINE lineNo)
        {
            DateTime last = DateTime.Now;
            try
            {
            }
            catch (Exception e2)
            {
                Util.WriteLog("GetDailyLastTime. " + e2.Message, "LogErr", "GVMS_DB");
            }
            return last;
        }

        internal static int[] GetWeeklyData(VISION_LINE line, DateTime[] from, DateTime[] to)
        {
            int[] result = new int[5];

            return result;
        }

        internal static double[] GetMonthlyData(VISION_DATA_TYPE dataType)
        {
            double[] result = new double[12];
            int year = DateTime.Now.Year;
            return result;
        }
    }
}

namespace VIBRATION_DATA
{
    public class NODE
    {
        public int IDNode;
        public int IDParent;
        public int IDRefNode;
        public short TreeType;
        public short NodeType;
        public string NodeName;
        public string NodeDescription;
        public int NodeStatus;
        public int SortOrderId;
        public bool NodeActive;
        public DateTime LastUpdated;
        public DateTime StatusChanged;
        public int NodeTag;

        internal static List<NODE> GetNodeList(string connStr, string cmd, out string err)
        {
            err = string.Empty;
            List<NODE> list = new List<NODE>();
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connStr;
                SqlCommand qry = new SqlCommand();
                qry.Connection = conn;

                Database.MakeQueryString(qry, cmd);
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlDataReader reader = qry.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            /*
                            public int IDNode;
                            public int IDParent;
                            public int IDRefNode;
                            public short TreeType;
                            public short NodeType;
                            public string NodeName;
                            public string NodeDescription;
                            public int NodeStatus;
                            public int SortOrderId;
                            public bool NodeActive;
                            public DateTime LastUpdated;
                            public DateTime StatusChanged;
                            public int NodeTag;
                            */
                            NODE n = new NODE();
                            n.IDNode = Database.getIntValue(reader, "IDNode");
                            n.IDParent = Database.getIntValue(reader, "IDParent");
                            n.IDRefNode = Database.getIntValue(reader, "IDRefNode");
                            n.TreeType = Database.getShortValue(reader, "TreeType");
                            n.NodeType = Database.getShortValue(reader, "NodeType");
                            n.NodeName = Database.getStringValue(reader, "NodeName");
                            n.NodeDescription = Database.getStringValue(reader, "NodeDescription");
                            n.NodeStatus = Database.getIntValue(reader, "NodeStatus");
                            n.SortOrderId = Database.getIntValue(reader, "SortOrderId");
                            n.NodeActive = Database.getBoolValue(reader, "NodeActive");
                            n.LastUpdated = Database.getDateTimeValue(reader, "LastUpdated");
                            n.StatusChanged = Database.getDateTimeValue(reader, "StatusChanged");
                            n.NodeTag = Database.getIntValue(reader, "NodeTag");
                            list.Add(n);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                err = e.Message;
            }
            return list;
        }
    }
}

namespace CURRENT_DATA
{
    public class CurrentNode
    {
        public static Hashtable Values = new Hashtable();
        public static int iconWidth = 40;
        public static int iconHeight = 59;

        public void Init()
        {
            Values.Clear();

            // Assembly
            {
                Values.Add("ASSEMBLY1X", 1350);
                Values.Add("ASSEMBLY1Y", 397);

                Values.Add("ASSEMBLY2X", 105);
                Values.Add("ASSEMBLY2Y", 397);

                Values.Add("ASSEMBLY3X", 105);
                Values.Add("ASSEMBLY3Y", 489);

                Values.Add("ASSEMBLY4X", 1350);
                Values.Add("ASSEMBLY4Y", 489);

                Values.Add("ASSEMBLY5X", 1350);
                Values.Add("ASSEMBLY5Y", 576);

                Values.Add("ASSEMBLY6X", 585);
                Values.Add("ASSEMBLY6Y", 576);

                Values.Add("ASSEMBLY7X", 363);
                Values.Add("ASSEMBLY7Y", 655);

                Values.Add("ASSEMBLY8X", 1350);
                Values.Add("ASSEMBLY8Y", 655);

                Values.Add("ASSEMBLY9X", 1350);
                Values.Add("ASSEMBLY9Y", 305);

                Values.Add("ASSEMBLY10X", 105);
                Values.Add("ASSEMBLY10Y", 305);

                Values.Add("ASSEMBLY11X", 105);
                Values.Add("ASSEMBLY11Y", 214);

                Values.Add("ASSEMBLY12X", 1350);
                Values.Add("ASSEMBLY12Y", 214);

                Values.Add("ASSEMBLY13X", 1350);
                Values.Add("ASSEMBLY13Y", 111);

                Values.Add("ASSEMBLY14X", 1310);
                Values.Add("ASSEMBLY14Y", 397);

                Values.Add("ASSEMBLY15X", 1110);
                Values.Add("ASSEMBLY15Y", 385);

                Values.Add("ASSEMBLY16X", 1110);
                Values.Add("ASSEMBLY16Y", 450);

                Values.Add("ASSEMBLY17X", 1150);
                Values.Add("ASSEMBLY17Y", 385);

                Values.Add("ASSEMBLY18X", 1150);
                Values.Add("ASSEMBLY18Y", 450);

                Values.Add("ASSEMBLY19X", 1190);
                Values.Add("ASSEMBLY19Y", 385);

                Values.Add("ASSEMBLY20X", 1190);
                Values.Add("ASSEMBLY20Y", 450);

                Values.Add("ASSEMBLY21X", 1230);
                Values.Add("ASSEMBLY21Y", 385);

                Values.Add("ASSEMBLY22X", 1230);
                Values.Add("ASSEMBLY22Y", 450);

                Values.Add("ASSEMBLY23X", 1270);
                Values.Add("ASSEMBLY23Y", 385);

                Values.Add("ASSEMBLY24X", 1270);
                Values.Add("ASSEMBLY24Y", 450);

                Values.Add("ASSEMBLY25X", 305);
                Values.Add("ASSEMBLY25Y", 450);

                Values.Add("ASSEMBLY26X", 305);
                Values.Add("ASSEMBLY26Y", 525);

                Values.Add("ASSEMBLY27X", 265);
                Values.Add("ASSEMBLY27Y", 450);

                Values.Add("ASSEMBLY28X", 265);
                Values.Add("ASSEMBLY28Y", 525);

                Values.Add("ASSEMBLY29X", 225);
                Values.Add("ASSEMBLY29Y", 450);

                Values.Add("ASSEMBLY30X", 225);
                Values.Add("ASSEMBLY30Y", 525);

                Values.Add("ASSEMBLY31X", 185);
                Values.Add("ASSEMBLY31Y", 450);

                Values.Add("ASSEMBLY32X", 185);
                Values.Add("ASSEMBLY32Y", 525);

                Values.Add("ASSEMBLY33X", 145);
                Values.Add("ASSEMBLY33Y", 450);

                Values.Add("ASSEMBLY34X", 145);
                Values.Add("ASSEMBLY34Y", 525);

                Values.Add("ASSEMBLY35X", 1310);
                Values.Add("ASSEMBLY35Y", 489);

                Values.Add("ASSEMBLY36X", 1310);
                Values.Add("ASSEMBLY36Y", 305);

                Values.Add("ASSEMBLY37X", 1110);
                Values.Add("ASSEMBLY37Y", 248);

                Values.Add("ASSEMBLY38X", 1110);
                Values.Add("ASSEMBLY38Y", 318);

                Values.Add("ASSEMBLY39X", 1150);
                Values.Add("ASSEMBLY39Y", 248);

                Values.Add("ASSEMBLY40X", 1150);
                Values.Add("ASSEMBLY40Y", 318);

                Values.Add("ASSEMBLY41X", 1190);
                Values.Add("ASSEMBLY41Y", 248);

                Values.Add("ASSEMBLY42X", 1190);
                Values.Add("ASSEMBLY42Y", 318);

                Values.Add("ASSEMBLY43X", 1230);
                Values.Add("ASSEMBLY43Y", 248);

                Values.Add("ASSEMBLY44X", 1230);
                Values.Add("ASSEMBLY44Y", 318);

                Values.Add("ASSEMBLY45X", 1230);
                Values.Add("ASSEMBLY45Y", 248);

                Values.Add("ASSEMBLY46X", 1270);
                Values.Add("ASSEMBLY46Y", 318);

                Values.Add("ASSEMBLY47X", 305);
                Values.Add("ASSEMBLY47Y", 178);

                Values.Add("ASSEMBLY48X", 305);
                Values.Add("ASSEMBLY48Y", 248);

                Values.Add("ASSEMBLY49X", 265);
                Values.Add("ASSEMBLY49Y", 178);

                Values.Add("ASSEMBLY50X", 265);
                Values.Add("ASSEMBLY50Y", 248);

                Values.Add("ASSEMBLY51X", 225);
                Values.Add("ASSEMBLY51Y", 178);

                Values.Add("ASSEMBLY52X", 225);
                Values.Add("ASSEMBLY52Y", 248);

                Values.Add("ASSEMBLY53X", 185);
                Values.Add("ASSEMBLY53Y", 178);

                Values.Add("ASSEMBLY54X", 185);
                Values.Add("ASSEMBLY54Y", 248);

                Values.Add("ASSEMBLY55X", 145);
                Values.Add("ASSEMBLY55Y", 178);

                Values.Add("ASSEMBLY56X", 145);
                Values.Add("ASSEMBLY56Y", 248);

                Values.Add("ASSEMBLY57X", 1310);
                Values.Add("ASSEMBLY57Y", 214);

                Values.Add("ASSEMBLY58X", 277);
                Values.Add("ASSEMBLY58Y", 110);
            }

            // Moving
            {
                // T/L & T/G
                {
                    Values.Add("MOVING1X", 0054);
                    Values.Add("MOVING1Y", 0159);

                    Values.Add("MOVING2X", 0094);
                    Values.Add("MOVING2Y", 0159);

                    Values.Add("MOVING3X", 0156);
                    Values.Add("MOVING3Y", 0519);

                    Values.Add("MOVING4X", 0196);
                    Values.Add("MOVING4Y", 0519);

                    Values.Add("MOVING5X", 0076);
                    Values.Add("MOVING5Y", 0519);

                    Values.Add("MOVING6X", 0116);
                    Values.Add("MOVING6Y", 0519);

                    Values.Add("MOVING7X", 0179);
                    Values.Add("MOVING7Y", 0578);

                    Values.Add("MOVING8X", 0179);
                    Values.Add("MOVING8Y", 0637);

                    Values.Add("MOVING9X", 0059);
                    Values.Add("MOVING9Y", 0578);

                    Values.Add("MOVING10X", 0099);
                    Values.Add("MOVING10Y", 0578);

                    Values.Add("MOVING11X", 0059);
                    Values.Add("MOVING11Y", 0637);

                    Values.Add("MOVING12X", 0099);
                    Values.Add("MOVING12Y", 0637);
                }

                // HOOD
                {
                    Values.Add("MOVING13X", 0437);
                    Values.Add("MOVING13Y", 0159);

                    Values.Add("MOVING14X", 0316);
                    Values.Add("MOVING14Y", 0637);

                    Values.Add("MOVING15X", 0396);
                    Values.Add("MOVING15Y", 0637 - iconHeight);

                    Values.Add("MOVING16X", 0436);
                    Values.Add("MOVING16Y", 0637 - iconHeight);

                    Values.Add("MOVING17X", 0396);
                    Values.Add("MOVING17Y", 0637);

                    Values.Add("MOVING18X", 0436);
                    Values.Add("MOVING18Y", 0637);

                    Values.Add("MOVING19X", 0316);
                    Values.Add("MOVING19Y", 0637 - iconHeight * 2);

                    Values.Add("MOVING20X", 0356);
                    Values.Add("MOVING20Y", 0637 - iconHeight * 2);
                }

                // RD RH
                {
                    Values.Add("MOVING21X", 0675);
                    Values.Add("MOVING21Y", 0129);

                    Values.Add("MOVING22X", 0675);
                    Values.Add("MOVING22Y", 0129 + iconHeight);

                    Values.Add("MOVING23X", 0558);
                    Values.Add("MOVING23Y", 0637 - iconHeight);

                    Values.Add("MOVING24X", 0598);
                    Values.Add("MOVING24Y", 0637 - iconHeight);

                    Values.Add("MOVING25X", 0558);
                    Values.Add("MOVING25Y", 0637);

                    Values.Add("MOVING26X", 0598);
                    Values.Add("MOVING26Y", 0637);

                    Values.Add("MOVING27X", 0635);
                    Values.Add("MOVING27Y", 0519);

                    Values.Add("MOVING28X", 0675);
                    Values.Add("MOVING28Y", 0519);

                    Values.Add("MOVING56X", 0558);
                    Values.Add("MOVING56Y", 0720);

                }

                // FD RH
                {
                    Values.Add("MOVING29X", 0797);
                    Values.Add("MOVING29Y", 0129);

                    Values.Add("MOVING30X", 0797);
                    Values.Add("MOVING30Y", 0129 + iconHeight);

                    Values.Add("MOVING31X", 0876);
                    Values.Add("MOVING31Y", 0637 - iconHeight);

                    Values.Add("MOVING32X", 0916);
                    Values.Add("MOVING32Y", 0637 - iconHeight);

                    Values.Add("MOVING33X", 0876);
                    Values.Add("MOVING33Y", 0637);

                    Values.Add("MOVING34X", 0916);
                    Values.Add("MOVING34Y", 0637);

                    Values.Add("MOVING35X", 0797);
                    Values.Add("MOVING35Y", 0637 - iconHeight * 2);

                    Values.Add("MOVING36X", 0837);
                    Values.Add("MOVING36Y", 0637 - iconHeight * 2);

                    Values.Add("MOVING55X", 0797);
                    Values.Add("MOVING55Y", 0720);

                }

                // FD LH
                {
                    Values.Add("MOVING37X", 1162);
                    Values.Add("MOVING37Y", 0129);

                    Values.Add("MOVING38X", 1162);
                    Values.Add("MOVING38Y", 0129 + iconHeight);

                    Values.Add("MOVING39X", 1043);
                    Values.Add("MOVING39Y", 0637 - iconHeight);

                    Values.Add("MOVING40X", 1083);
                    Values.Add("MOVING40Y", 0637 - iconHeight);

                    Values.Add("MOVING41X", 1043);
                    Values.Add("MOVING41Y", 0637);

                    Values.Add("MOVING42X", 1083);
                    Values.Add("MOVING42Y", 0637);

                    Values.Add("MOVING43X", 1112);
                    Values.Add("MOVING43Y", 0637 - iconHeight * 2);

                    Values.Add("MOVING44X", 1152);
                    Values.Add("MOVING44Y", 0637 - iconHeight * 2);

                    Values.Add("MOVING53X", 1152);
                    Values.Add("MOVING53Y", 0720);
                }

                // RD LH
                {
                    Values.Add("MOVING45X", 1276);
                    Values.Add("MOVING45Y", 0129);

                    Values.Add("MOVING46X", 1276);
                    Values.Add("MOVING46Y", 0129 + iconHeight);

                    Values.Add("MOVING47X", 1353);
                    Values.Add("MOVING47Y", 0637 - iconHeight);

                    Values.Add("MOVING48X", 1393);
                    Values.Add("MOVING48Y", 0637 - iconHeight);

                    Values.Add("MOVING49X", 1353);
                    Values.Add("MOVING49Y", 0637);

                    Values.Add("MOVING50X", 1393);
                    Values.Add("MOVING50Y", 0637);

                    Values.Add("MOVING51X", 1276);
                    Values.Add("MOVING51Y", 0637 - iconHeight * 2);

                    Values.Add("MOVING52X", 1316);
                    Values.Add("MOVING52Y", 0637 - iconHeight * 2);

                    Values.Add("MOVING54X", 1276);
                    Values.Add("MOVING54Y", 0720);
                }
            }

            // FLR
            {
                int width = 57;
                int height = 60;
                // FLR COMPL
                {
                    Values.Add("FLR1X", 0319);
                    Values.Add("FLR1Y", 0408);

                    Values.Add("FLR2X", 0271);
                    Values.Add("FLR2Y", 0258);

                    Values.Add("FLR3X", 0271);
                    Values.Add("FLR3Y", 0612);

                    Values.Add("FLR4X", 0349);
                    Values.Add("FLR4Y", 0258);

                    Values.Add("FLR24X", 0349 + width);
                    Values.Add("FLR24Y", 0258);

                    Values.Add("FLR25X", 0349 + width * 2);
                    Values.Add("FLR25Y", 0258);

                    Values.Add("FLR26X", 0349 + width * 3);
                    Values.Add("FLR26Y", 0258);

                    Values.Add("FLR21X", 0349 + width);
                    Values.Add("FLR21Y", 0258 + height);

                    Values.Add("FLR22X", 0349 + width * 2);
                    Values.Add("FLR22Y", 0258 + height);

                    Values.Add("FLR23X", 0349 + width * 3);
                    Values.Add("FLR23Y", 0258 + height);

                    Values.Add("FLR18X", 0349 + width);
                    Values.Add("FLR18Y", 0258 + height * 2);

                    Values.Add("FLR19X", 0349 + width * 2);
                    Values.Add("FLR19Y", 0258 + height * 2);

                    Values.Add("FLR20X", 0349 + width * 3);
                    Values.Add("FLR20Y", 0258 + height * 2);

                    Values.Add("FLR15X", 0349 + width);
                    Values.Add("FLR15Y", 0258 + height * 3);

                    Values.Add("FLR16X", 0349 + width * 2);
                    Values.Add("FLR16Y", 0258 + height * 3);

                    Values.Add("FLR17X", 0349 + width * 3);
                    Values.Add("FLR17Y", 0258 + height * 3);

                    Values.Add("FLR12X", 0349 + width);
                    Values.Add("FLR12Y", 0258 + height * 4);

                    Values.Add("FLR13X", 0349 + width * 2);
                    Values.Add("FLR13Y", 0258 + height * 4);

                    Values.Add("FLR14X", 0349 + width * 3);
                    Values.Add("FLR14Y", 0258 + height * 4);

                    Values.Add("FLR9X", 0349 + width);
                    Values.Add("FLR9Y", 0258 + height * 5);

                    Values.Add("FLR10X", 0349 + width * 2);
                    Values.Add("FLR10Y", 0258 + height * 5);

                    Values.Add("FLR11X", 0349 + width * 3);
                    Values.Add("FLR11Y", 0258 + height * 5);

                    Values.Add("FLR5X", 0349);
                    Values.Add("FLR5Y", 0615);

                    Values.Add("FLR6X", 0349 + width);
                    Values.Add("FLR6Y", 0615);

                    Values.Add("FLR7X", 0349 + width * 2);
                    Values.Add("FLR7Y", 0615);

                    Values.Add("FLR8X", 0349 + width * 3);
                    Values.Add("FLR8Y", 0615);

                    Values.Add("FLR27X", 0585);
                    Values.Add("FLR27Y", 0615);

                    Values.Add("FLR47X", 0276 - width / 2);
                    Values.Add("FLR47Y", 0615 + height);

                    Values.Add("FLR48X", 0276 + width / 2);
                    Values.Add("FLR48Y", 0615 + height);

                    Values.Add("FLR49X", 0077);
                    Values.Add("FLR49Y", 0615);

                }

                // FRT FLR
                {
                    Values.Add("FLR28X", 1287);
                    Values.Add("FLR28Y", 0258);

                    Values.Add("FLR29X", 1287 - width);
                    Values.Add("FLR29Y", 0258);

                    Values.Add("FLR30X", 1287 - width * 2);
                    Values.Add("FLR30Y", 0258);

                    Values.Add("FLR31X", 1287);
                    Values.Add("FLR31Y", 0258 + height);

                    Values.Add("FLR32X", 1287);
                    Values.Add("FLR32Y", 0258 + height * 2);

                    Values.Add("FLR33X", 1287);
                    Values.Add("FLR33Y", 0258 + height * 3);

                    Values.Add("FLR34X", 1287 - width);
                    Values.Add("FLR34Y", 0258 + height);

                    Values.Add("FLR35X", 1287 - width * 2);
                    Values.Add("FLR35Y", 0258 + height);

                    Values.Add("FLR36X", 1287 - width);
                    Values.Add("FLR36Y", 0258 + height * 2);

                    Values.Add("FLR37X", 1287 - width * 2);
                    Values.Add("FLR37Y", 0258 + height * 2);

                    Values.Add("FLR38X", 1287 - width);
                    Values.Add("FLR38Y", 0258 + height * 3);

                    Values.Add("FLR39X", 1287 - width * 2);
                    Values.Add("FLR39Y", 0258 + height * 3);

                    Values.Add("FLR40X", 1287 - width * 4);
                    Values.Add("FLR40Y", 0258 + height * 2);

                    Values.Add("FLR41X", 1287 - width * 3);
                    Values.Add("FLR41Y", 0258 + height);

                    Values.Add("FLR42X", 1287 - width * 3);
                    Values.Add("FLR42Y", 0258 + height * 3);

                    Values.Add("FLR43X", 0956);
                    Values.Add("FLR43Y", 0241 + height * 2);

                    Values.Add("FLR44X", 956);
                    Values.Add("FLR44Y", 0241 + height * 1);

                    Values.Add("FLR45X", 1148);
                    Values.Add("FLR45Y", 0525);

                    Values.Add("FLR46X", 1070);
                    Values.Add("FLR46Y", 0258 + height * 4);

                    Values.Add("FLR50X", 1295);
                    Values.Add("FLR50Y", 0615);
                }
            }

            // SIDE
            {
                int width = 47;
                int height = 52;
                // LH
                {
                    Values.Add("SIDE1X", 0406);
                    Values.Add("SIDE1Y", 0305);

                    Values.Add("SIDE20X", 0406 - width);
                    Values.Add("SIDE20Y", 0305 - 10);

                    Values.Add("SIDE19X", 0406 - width);
                    Values.Add("SIDE19Y", 0305 - 10 - height);

                    Values.Add("SIDE17X", 0406 - width - 100);
                    Values.Add("SIDE17Y", 0305 - 10 - height);

                    Values.Add("SIDE18X", 0406 - width - 100);
                    Values.Add("SIDE18Y", 0305 - 10 - height * 0);

                    Values.Add("SIDE2X", 0452);
                    Values.Add("SIDE2Y", 0271);

                    Values.Add("SIDE3X", 0452 + width);
                    Values.Add("SIDE3Y", 0271);

                    Values.Add("SIDE5X", 0452);
                    Values.Add("SIDE5Y", 0271 - height);

                    Values.Add("SIDE6X", 0452 + width);
                    Values.Add("SIDE6Y", 0271 - height);

                    Values.Add("SIDE8X", 0452);
                    Values.Add("SIDE8Y", 0271 - height * 2);

                    Values.Add("SIDE9X", 0452 + width);
                    Values.Add("SIDE9Y", 0271 - height * 2);

                    Values.Add("SIDE16X", 0452 + width * 3);
                    Values.Add("SIDE16Y", 0271 - height * 2);

                    Values.Add("SIDE15X", 0452 + width * 3);
                    Values.Add("SIDE15Y", 0271 - height * 1);

                    Values.Add("SIDE14X", 0452 + width * 3);
                    Values.Add("SIDE14Y", 0271);

                    Values.Add("SIDE11X", 0452 + width * 3);
                    Values.Add("SIDE11Y", 0271 + height);

                    Values.Add("SIDE4X", 0452 + width * 2);
                    Values.Add("SIDE4Y", 0271 + 30);

                    Values.Add("SIDE7X", 0452 + width * 2);
                    Values.Add("SIDE7Y", 0271 + 30 - height);

                    Values.Add("SIDE10X", 0452 + width * 2);
                    Values.Add("SIDE10Y", 0271 + 30 - height * 2);

                    Values.Add("SIDE12X", 0452);
                    Values.Add("SIDE12Y", 0271 + height);

                    Values.Add("SIDE13X", 0452 + width);
                    Values.Add("SIDE13Y", 0271 + height);

                    Values.Add("SIDE43X", 0452);
                    Values.Add("SIDE43Y", 0271 + height * 2);

                    Values.Add("SIDE44X", 0452 + width);
                    Values.Add("SIDE44Y", 0271 + height * 2);

                    Values.Add("SIDE45X", 0452 + width * 2);
                    Values.Add("SIDE45Y", 0271 + height * 2);

                    Values.Add("SIDE24X", 0452 - width);
                    Values.Add("SIDE24Y", 0271 + height * 2);

                    Values.Add("SIDE22X", 0452 - width * 2);
                    Values.Add("SIDE22Y", 0271 + height * 2);

                    Values.Add("SIDE21X", 0452 - width * 2);
                    Values.Add("SIDE21Y", 0271 + height * 2 + 120);

                    Values.Add("SIDE40X", 0452);
                    Values.Add("SIDE40Y", 0271 + height * 3);

                    Values.Add("SIDE41X", 0452 + width);
                    Values.Add("SIDE41Y", 0271 + height * 3);

                    Values.Add("SIDE42X", 0452 + width * 2);
                    Values.Add("SIDE42Y", 0271 + height * 3);

                    Values.Add("SIDE37X", 0452);
                    Values.Add("SIDE37Y", 0271 + height * 4);

                    Values.Add("SIDE38X", 0452 + width);
                    Values.Add("SIDE38Y", 0271 + height * 4);

                    Values.Add("SIDE39X", 0452 + width * 2);
                    Values.Add("SIDE39Y", 0271 + height * 4);

                    Values.Add("SIDE34X", 0452);
                    Values.Add("SIDE34Y", 0271 + height * 5);

                    Values.Add("SIDE35X", 0452 + width);
                    Values.Add("SIDE35Y", 0271 + height * 5);

                    Values.Add("SIDE36X", 0452 + width * 2);
                    Values.Add("SIDE36Y", 0271 + height * 5);

                    Values.Add("SIDE31X", 0452);
                    Values.Add("SIDE31Y", 0271 + height * 6);

                    Values.Add("SIDE32X", 0452 + width);
                    Values.Add("SIDE32Y", 0271 + height * 6);

                    Values.Add("SIDE33X", 0452 + width * 2);
                    Values.Add("SIDE33Y", 0271 + height * 6);

                    Values.Add("SIDE28X", 0452);
                    Values.Add("SIDE28Y", 0271 + height * 7);

                    Values.Add("SIDE29X", 0452 + width);
                    Values.Add("SIDE29Y", 0271 + height * 7);

                    Values.Add("SIDE30X", 0452 + width * 2);
                    Values.Add("SIDE30Y", 0271 + height * 7);

                    Values.Add("SIDE26X", 0452);
                    Values.Add("SIDE26Y", 0271 + height * 8);

                    Values.Add("SIDE27X", 0452 + width);
                    Values.Add("SIDE27Y", 0271 + height * 8);

                    Values.Add("SIDE46X", 0452 + width * 2);
                    Values.Add("SIDE46Y", 0271 + height * 8);

                    Values.Add("SIDE47X", 0452 + width * 3);
                    Values.Add("SIDE47Y", 0271 + height * 8);

                    Values.Add("SIDE25X", 0452 - width);
                    Values.Add("SIDE25Y", 0271 + height * 8);

                    Values.Add("SIDE23X", 0452 - width * 2);
                    Values.Add("SIDE23Y", 0271 + height * 8);

                    Values.Add("SIDE95X", 0069);
                    Values.Add("SIDE95Y", 0271 + height * 8);

                }

                // RH
                {
                    Values.Add("SIDE63X", 0865);
                    Values.Add("SIDE63Y", 0271 - height * 2);

                    Values.Add("SIDE62X", 0865);
                    Values.Add("SIDE62Y", 0271 - height * 1);

                    Values.Add("SIDE61X", 0865);
                    Values.Add("SIDE61Y", 0271 - height * 0);

                    Values.Add("SIDE58X", 0865);
                    Values.Add("SIDE58Y", 0271 + height * 1);

                    Values.Add("SIDE51X", 0865 + width * 1);
                    Values.Add("SIDE51Y", 0271 - 30 + height * 1);

                    Values.Add("SIDE54X", 0865 + width * 1);
                    Values.Add("SIDE54Y", 0271 - 30 + height * 0);

                    Values.Add("SIDE57X", 0865 + width * 1);
                    Values.Add("SIDE57Y", 0271 - 30 - height * 1);

                    Values.Add("SIDE92X", 0865);
                    Values.Add("SIDE92Y", 0271 + height * 2);

                    Values.Add("SIDE91X", 0865 + width * 1);
                    Values.Add("SIDE91Y", 0271 + height * 2);

                    Values.Add("SIDE90X", 0865 + width * 2);
                    Values.Add("SIDE90Y", 0271 + height * 2);

                    Values.Add("SIDE71X", 0865 + width * 3);
                    Values.Add("SIDE71Y", 0271 + height * 2);

                    Values.Add("SIDE60X", 0865 + width * 2);
                    Values.Add("SIDE60Y", 0271 + height * 1);

                    Values.Add("SIDE59X", 0865 + width * 3);
                    Values.Add("SIDE59Y", 0271 + height * 1);

                    Values.Add("SIDE50X", 0865 + width * 2);
                    Values.Add("SIDE50Y", 0271 + height * 0);

                    Values.Add("SIDE49X", 0865 + width * 3);
                    Values.Add("SIDE49Y", 0271 + height * 0);

                    Values.Add("SIDE48X", 0865 + width * 4);
                    Values.Add("SIDE48Y", 0305);

                    Values.Add("SIDE67X", 0865 + width * 5);
                    Values.Add("SIDE67Y", 0305 - 5);

                    Values.Add("SIDE66X", 0865 + width * 5);
                    Values.Add("SIDE66Y", 0305 - 5 - height);

                    Values.Add("SIDE64X", 0865 + width * 6 + 20);
                    Values.Add("SIDE64Y", 0305 - 5 - height);

                    Values.Add("SIDE65X", 0865 + width * 6 + 20);
                    Values.Add("SIDE65Y", 0305 - 5);

                    Values.Add("SIDE53X", 0865 + width * 2);
                    Values.Add("SIDE53Y", 0271 - height * 1);

                    Values.Add("SIDE52X", 0865 + width * 3);
                    Values.Add("SIDE52Y", 0271 - height * 1);

                    Values.Add("SIDE56X", 0865 + width * 2);
                    Values.Add("SIDE56Y", 0271 - height * 2);

                    Values.Add("SIDE55X", 0865 + width * 3);
                    Values.Add("SIDE55Y", 0271 - height * 2);

                    Values.Add("SIDE69X", 0865 + width * 4);
                    Values.Add("SIDE69Y", 0271 + height * 2);

                    Values.Add("SIDE68X", 0865 + width * 4);
                    Values.Add("SIDE68Y", 497);

                    Values.Add("SIDE89X", 0865);
                    Values.Add("SIDE89Y", 0271 + height * 3);

                    Values.Add("SIDE88X", 0865 + width * 1);
                    Values.Add("SIDE88Y", 0271 + height * 3);

                    Values.Add("SIDE87X", 0865 + width * 2);
                    Values.Add("SIDE87Y", 0271 + height * 3);

                    Values.Add("SIDE86X", 0865);
                    Values.Add("SIDE86Y", 0271 + height * 4);

                    Values.Add("SIDE83X", 0865);
                    Values.Add("SIDE83Y", 0271 + height * 5);

                    Values.Add("SIDE82X", 0865 + width * 1);
                    Values.Add("SIDE82Y", 0271 + height * 5);

                    Values.Add("SIDE81X", 0865 + width * 2);
                    Values.Add("SIDE81Y", 0271 + height * 5);

                    Values.Add("SIDE85X", 0865 + width * 1);
                    Values.Add("SIDE85Y", 0271 + height * 4);

                    Values.Add("SIDE84X", 0865 + width * 2);
                    Values.Add("SIDE84Y", 0271 + height * 4);

                    Values.Add("SIDE80X", 0865);
                    Values.Add("SIDE80Y", 0271 + height * 6);

                    Values.Add("SIDE79X", 0865 + width * 1);
                    Values.Add("SIDE79Y", 0271 + height * 6);

                    Values.Add("SIDE78X", 0865 + width * 2);
                    Values.Add("SIDE78Y", 0271 + height * 6);

                    Values.Add("SIDE77X", 0865);
                    Values.Add("SIDE77Y", 0271 + height * 7);

                    Values.Add("SIDE76X", 0865 + width * 1);
                    Values.Add("SIDE76Y", 0271 + height * 7);

                    Values.Add("SIDE75X", 0865 + width * 2);
                    Values.Add("SIDE75Y", 0271 + height * 7);

                    Values.Add("SIDE93X", 0865);
                    Values.Add("SIDE93Y", 0271 + height * 8);

                    Values.Add("SIDE94X", 0865 - width);
                    Values.Add("SIDE94Y", 0271 + height * 8);

                    Values.Add("SIDE74X", 0865 + width);
                    Values.Add("SIDE74Y", 0271 + height * 8);

                    Values.Add("SIDE73X", 0865 + width * 2);
                    Values.Add("SIDE73Y", 0271 + height * 8);

                    Values.Add("SIDE72X", 0865 + width * 3);
                    Values.Add("SIDE72Y", 0271 + height * 8);

                    Values.Add("SIDE70X", 0865 + width * 4);
                    Values.Add("SIDE70Y", 0271 + height * 8);

                    Values.Add("SIDE96X", 1367);
                    Values.Add("SIDE96Y", 0271 + height * 8);


                }
            }

            // CR&ROOF
            {
                int width = 55;
                //int height = 55;

                {
                    Values.Add("CR&ROOF" + "1" + "X", 0853);
                    Values.Add("CR&ROOF" + "1" + "Y", 0461);

                    Values.Add("CR&ROOF" + "2" + "X", 0831);
                    Values.Add("CR&ROOF" + "2" + "Y", 0310);

                    Values.Add("CR&ROOF" + "3" + "X", 0769);
                    Values.Add("CR&ROOF" + "3" + "Y", 0571);

                    Values.Add("CR&ROOF" + "4" + "X", 0910);
                    Values.Add("CR&ROOF" + "4" + "Y", 0571);

                    Values.Add("CR&ROOF" + "5" + "X", 0910 + width);
                    Values.Add("CR&ROOF" + "5" + "Y", 0571);

                }
            }

            // BC&MF
            {
                int width = 40;
                int height = 50;
                int largewidth = 78;
                // BC
                {
                    Values.Add("BC&MF" + "94" + "X", 1299);
                    Values.Add("BC&MF" + "94" + "Y", 0233 - height);

                    Values.Add("BC&MF" + "95" + "X", 1299 + width);
                    Values.Add("BC&MF" + "95" + "Y", 0233 - height);

                    Values.Add("BC&MF" + "1" + "X", 1299);
                    Values.Add("BC&MF" + "1" + "Y", 0233);

                    Values.Add("BC&MF" + "2" + "X", 1299 + width);
                    Values.Add("BC&MF" + "2" + "Y", 0233);

                    Values.Add("BC&MF" + "3" + "X", 1299);
                    Values.Add("BC&MF" + "3" + "Y", 0233 + height);

                    Values.Add("BC&MF" + "4" + "X", 1240);
                    Values.Add("BC&MF" + "4" + "Y", 0212);

                    Values.Add("BC&MF" + "5" + "X", 1240);
                    Values.Add("BC&MF" + "5" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "6" + "X", 1240 - largewidth);
                    Values.Add("BC&MF" + "6" + "Y", 0212);

                    Values.Add("BC&MF" + "7" + "X", 1240 - largewidth);
                    Values.Add("BC&MF" + "7" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "15" + "X", 1240 - largewidth * 2);
                    Values.Add("BC&MF" + "15" + "Y", 0212 - height);

                    Values.Add("BC&MF" + "8" + "X", 1240 - largewidth * 2);
                    Values.Add("BC&MF" + "8" + "Y", 0212);

                    Values.Add("BC&MF" + "9" + "X", 1240 - largewidth * 2);
                    Values.Add("BC&MF" + "9" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "14" + "X", 1240 - largewidth * 2);
                    Values.Add("BC&MF" + "14" + "Y", 0212 + height * 2);

                    Values.Add("BC&MF" + "17" + "X", 1240 - largewidth * 3);
                    Values.Add("BC&MF" + "17" + "Y", 0212 - height);

                    Values.Add("BC&MF" + "10" + "X", 1240 - largewidth * 3);
                    Values.Add("BC&MF" + "10" + "Y", 0212);

                    Values.Add("BC&MF" + "11" + "X", 1240 - largewidth * 3);
                    Values.Add("BC&MF" + "11" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "16" + "X", 1240 - largewidth * 3);
                    Values.Add("BC&MF" + "16" + "Y", 0212 + height * 2);

                    Values.Add("BC&MF" + "12" + "X", 1240 - largewidth * 4);
                    Values.Add("BC&MF" + "12" + "Y", 0212);

                    Values.Add("BC&MF" + "13" + "X", 1240 - largewidth * 4);
                    Values.Add("BC&MF" + "13" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "38" + "X", 854);
                    Values.Add("BC&MF" + "38" + "Y", 0212 - height);

                    Values.Add("BC&MF" + "39" + "X", 854 + width);
                    Values.Add("BC&MF" + "39" + "Y", 0212 - height);

                    Values.Add("BC&MF" + "36" + "X", 854);
                    Values.Add("BC&MF" + "36" + "Y", 0212 - height * 2);

                    Values.Add("BC&MF" + "37" + "X", 854 + width);
                    Values.Add("BC&MF" + "37" + "Y", 0212 - height * 2);

                    Values.Add("BC&MF" + "89" + "X", 854 + width * 2);
                    Values.Add("BC&MF" + "89" + "Y", 0212 - height * 2);

                    Values.Add("BC&MF" + "18" + "X", 1240 - largewidth * 5);
                    Values.Add("BC&MF" + "18" + "Y", 0212);

                    Values.Add("BC&MF" + "19" + "X", 1240 - largewidth * 5);
                    Values.Add("BC&MF" + "19" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "28" + "X", 854);
                    Values.Add("BC&MF" + "28" + "Y", 0212 + height * 2);

                    Values.Add("BC&MF" + "29" + "X", 854 + width);
                    Values.Add("BC&MF" + "29" + "Y", 0212 + height * 2);

                    Values.Add("BC&MF" + "30" + "X", 854);
                    Values.Add("BC&MF" + "30" + "Y", 0212 + height * 3);

                    Values.Add("BC&MF" + "31" + "X", 854 + width);
                    Values.Add("BC&MF" + "31" + "Y", 0212 + height * 3);

                    Values.Add("BC&MF" + "87" + "X", 854 + width * 2);
                    Values.Add("BC&MF" + "87" + "Y", 0212 + height * 3);

                    Values.Add("BC&MF" + "42" + "X", 0735);
                    Values.Add("BC&MF" + "42" + "Y", 0162);

                    Values.Add("BC&MF" + "43" + "X", 0735 + width);
                    Values.Add("BC&MF" + "43" + "Y", 0162);

                    Values.Add("BC&MF" + "40" + "X", 0735);
                    Values.Add("BC&MF" + "40" + "Y", 0162 - height);

                    Values.Add("BC&MF" + "41" + "X", 0735 + width);
                    Values.Add("BC&MF" + "41" + "Y", 0162 - height);

                    Values.Add("BC&MF" + "88" + "X", 0735 - width);
                    Values.Add("BC&MF" + "88" + "Y", 0162 - height);

                    Values.Add("BC&MF" + "20" + "X", 1240 - largewidth * 6);
                    Values.Add("BC&MF" + "20" + "Y", 0212);

                    Values.Add("BC&MF" + "21" + "X", 1240 - largewidth * 6);
                    Values.Add("BC&MF" + "21" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "32" + "X", 0735);
                    Values.Add("BC&MF" + "32" + "Y", 0212 + height * 2);

                    Values.Add("BC&MF" + "33" + "X", 0735 + width);
                    Values.Add("BC&MF" + "33" + "Y", 0212 + height * 2);

                    Values.Add("BC&MF" + "44" + "X", 0735 - width * 3);
                    Values.Add("BC&MF" + "44" + "Y", 0212 + height * 2);

                    Values.Add("BC&MF" + "45" + "X", 0735 - width * 2);
                    Values.Add("BC&MF" + "45" + "Y", 0212 + height * 2);

                    Values.Add("BC&MF" + "46" + "X", 0735 - width * 3);
                    Values.Add("BC&MF" + "46" + "Y", 0212 + height * 3);

                    Values.Add("BC&MF" + "47" + "X", 0735 - width * 2);
                    Values.Add("BC&MF" + "47" + "Y", 0212 + height * 3);

                    Values.Add("BC&MF" + "86" + "X", 0735 - width);
                    Values.Add("BC&MF" + "86" + "Y", 0212 + height * 3);

                    Values.Add("BC&MF" + "34" + "X", 0735);
                    Values.Add("BC&MF" + "34" + "Y", 0212 + height * 3);

                    Values.Add("BC&MF" + "35" + "X", 0735 + width);
                    Values.Add("BC&MF" + "35" + "Y", 0212 + height * 3);

                    Values.Add("BC&MF" + "22" + "X", 1240 - largewidth * 7);
                    Values.Add("BC&MF" + "22" + "Y", 0212);

                    Values.Add("BC&MF" + "23" + "X", 1240 - largewidth * 7);
                    Values.Add("BC&MF" + "23" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "24" + "X", 1240 - largewidth * 8);
                    Values.Add("BC&MF" + "24" + "Y", 0212);

                    Values.Add("BC&MF" + "25" + "X", 1240 - largewidth * 8);
                    Values.Add("BC&MF" + "25" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "26" + "X", 1240 - largewidth * 9);
                    Values.Add("BC&MF" + "26" + "Y", 0212);

                    Values.Add("BC&MF" + "27" + "X", 1240 - largewidth * 9);
                    Values.Add("BC&MF" + "27" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "48" + "X", 0445);
                    Values.Add("BC&MF" + "48" + "Y", 0162 - height);

                    Values.Add("BC&MF" + "49" + "X", 0445 + width);
                    Values.Add("BC&MF" + "49" + "Y", 0162 - height);

                    Values.Add("BC&MF" + "50" + "X", 0445);
                    Values.Add("BC&MF" + "50" + "Y", 0162);

                    Values.Add("BC&MF" + "51" + "X", 0445 + width);
                    Values.Add("BC&MF" + "51" + "Y", 0162);

                    Values.Add("BC&MF" + "55" + "X", 1240 - largewidth * 10);
                    Values.Add("BC&MF" + "55" + "Y", 0212);

                    Values.Add("BC&MF" + "56" + "X", 1240 - largewidth * 10);
                    Values.Add("BC&MF" + "56" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "57" + "X", 1240 - largewidth * 11);
                    Values.Add("BC&MF" + "57" + "Y", 0212);

                    Values.Add("BC&MF" + "58" + "X", 1240 - largewidth * 11);
                    Values.Add("BC&MF" + "58" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "64" + "X", 0323);
                    Values.Add("BC&MF" + "64" + "Y", 0212 - height);

                    Values.Add("BC&MF" + "66" + "X", 0323 + width);
                    Values.Add("BC&MF" + "66" + "Y", 0212 - height);

                    Values.Add("BC&MF" + "59" + "X", 1240 - largewidth * 12);
                    Values.Add("BC&MF" + "59" + "Y", 0212 + height * 0);

                    Values.Add("BC&MF" + "60" + "X", 1240 - largewidth * 12);
                    Values.Add("BC&MF" + "60" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "63" + "X", 0323);
                    Values.Add("BC&MF" + "63" + "Y", 0212 + height * 2);

                    Values.Add("BC&MF" + "65" + "X", 0323 + width);
                    Values.Add("BC&MF" + "65" + "Y", 0212 + height * 2);

                    Values.Add("BC&MF" + "61" + "X", 1240 - largewidth * 13);
                    Values.Add("BC&MF" + "61" + "Y", 0212);

                    Values.Add("BC&MF" + "62" + "X", 1240 - largewidth * 13);
                    Values.Add("BC&MF" + "62" + "Y", 0212 + height);

                    Values.Add("BC&MF" + "90" + "X", 0134);
                    Values.Add("BC&MF" + "90" + "Y", 0233 - height);

                    Values.Add("BC&MF" + "91" + "X", 0134 + width);
                    Values.Add("BC&MF" + "91" + "Y", 0233 - height);

                    Values.Add("BC&MF" + "52" + "X", 0134);
                    Values.Add("BC&MF" + "52" + "Y", 0233);

                    Values.Add("BC&MF" + "53" + "X", 0134 + width);
                    Values.Add("BC&MF" + "53" + "Y", 0233);

                    Values.Add("BC&MF" + "54" + "X", 0134 + width);
                    Values.Add("BC&MF" + "54" + "Y", 0233 + height);

                }

                // MF
                {
                    Values.Add("BC&MF" + "92" + "X", 0177);
                    Values.Add("BC&MF" + "92" + "Y", 0547);

                    Values.Add("BC&MF" + "93" + "X", 0177 + width);
                    Values.Add("BC&MF" + "93" + "Y", 0547);

                    Values.Add("BC&MF" + "75" + "X", 0146);
                    Values.Add("BC&MF" + "75" + "Y", 0547 + height);

                    Values.Add("BC&MF" + "76" + "X", 0146 + width * 1);
                    Values.Add("BC&MF" + "76" + "Y", 0547 + height);

                    Values.Add("BC&MF" + "77" + "X", 0146 + width * 2);
                    Values.Add("BC&MF" + "77" + "Y", 0547 + height);

                    Values.Add("BC&MF" + "71" + "X", 1169);
                    Values.Add("BC&MF" + "71" + "Y", 0547);

                    Values.Add("BC&MF" + "72" + "X", 1169 + width * 1);
                    Values.Add("BC&MF" + "72" + "Y", 0547);

                    Values.Add("BC&MF" + "67" + "X", 1169 + width * 2);
                    Values.Add("BC&MF" + "67" + "Y", 0547);

                    Values.Add("BC&MF" + "68" + "X", 1169 + width * 3);
                    Values.Add("BC&MF" + "68" + "Y", 0547);

                    Values.Add("BC&MF" + "73" + "X", 1169);
                    Values.Add("BC&MF" + "73" + "Y", 0547 + height);

                    Values.Add("BC&MF" + "74" + "X", 1169 + width * 1);
                    Values.Add("BC&MF" + "74" + "Y", 0547 + height);

                    Values.Add("BC&MF" + "69" + "X", 1169 + width * 2);
                    Values.Add("BC&MF" + "69" + "Y", 0547 + height);

                    Values.Add("BC&MF" + "70" + "X", 1169 + width * 3);
                    Values.Add("BC&MF" + "70" + "Y", 0547 + height);

                    Values.Add("BC&MF" + "78" + "X", 1362);
                    Values.Add("BC&MF" + "78" + "Y", 0566);

                    Values.Add("BC&MF" + "79" + "X", 1362 + width);
                    Values.Add("BC&MF" + "79" + "Y", 0566);

                    Values.Add("BC&MF" + "80" + "X", 1286);
                    Values.Add("BC&MF" + "80" + "Y", 0483);

                    Values.Add("BC&MF" + "81" + "X", 1286 + width * 1);
                    Values.Add("BC&MF" + "81" + "Y", 0483);

                    Values.Add("BC&MF" + "84" + "X", 1286 + width * 2);
                    Values.Add("BC&MF" + "84" + "Y", 0483);

                    Values.Add("BC&MF" + "82" + "X", 1286);
                    Values.Add("BC&MF" + "82" + "Y", 0611 + height * 1);

                    Values.Add("BC&MF" + "83" + "X", 1286 + width * 1);
                    Values.Add("BC&MF" + "83" + "Y", 0611 + height * 1);

                    Values.Add("BC&MF" + "85" + "X", 1286 + width * 2);
                    Values.Add("BC&MF" + "85" + "Y", 0611 + height * 1);

                }
            }

            // BB&BR
            {
                int width = 50;
                int height = 80;
                //int largewidth = 78;
                // BB
                {
                    string line = "BB&BR";
                    Values.Add(line + "1" + "X", 0190);
                    Values.Add(line + "1" + "Y", 0218);

                    Values.Add(line + "2" + "X", 0190 + width * 1);
                    Values.Add(line + "2" + "Y", 0218);

                    Values.Add(line + "3" + "X", 1057);
                    Values.Add(line + "3" + "Y", 0248);

                    Values.Add(line + "4" + "X", 0417);
                    Values.Add(line + "4" + "Y", 0248);

                    Values.Add(line + "5" + "X", 0537);
                    Values.Add(line + "5" + "Y", 0207);

                    Values.Add(line + "48" + "X", 0537 + 23);
                    Values.Add(line + "48" + "Y", 0207 - height);

                    Values.Add(line + "6" + "X", 0537);
                    Values.Add(line + "6" + "Y", 0207 + height * 1);

                    Values.Add(line + "7" + "X", 0537 + width * 1);
                    Values.Add(line + "7" + "Y", 0207);

                    Values.Add(line + "8" + "X", 0537 + width * 1);
                    Values.Add(line + "8" + "Y", 0207 + height * 1);

                    Values.Add(line + "9" + "X", 0686);
                    Values.Add(line + "9" + "Y", 0207);

                    Values.Add(line + "49" + "X", 0686 + 23);
                    Values.Add(line + "49" + "Y", 0207 - height);

                    Values.Add(line + "10" + "X", 0686);
                    Values.Add(line + "10" + "Y", 0207 + height * 1);

                    Values.Add(line + "11" + "X", 0686 + width * 1);
                    Values.Add(line + "11" + "Y", 0207);

                    Values.Add(line + "12" + "X", 0686 + width * 1);
                    Values.Add(line + "12" + "Y", 0207 + height * 1);

                    Values.Add(line + "13" + "X", 1267);
                    Values.Add(line + "13" + "Y", 0173);

                    Values.Add(line + "14" + "X", 1267 + width * 1);
                    Values.Add(line + "14" + "Y", 0173);

                    Values.Add(line + "15" + "X", 0983);
                    Values.Add(line + "15" + "Y", 0207);

                    Values.Add(line + "16" + "X", 0983);
                    Values.Add(line + "16" + "Y", 0207 + height * 1);

                    Values.Add(line + "24" + "X", 1227);
                    Values.Add(line + "24" + "Y", 0248);

                    Values.Add(line + "25" + "X", 1227 + width * 1);
                    Values.Add(line + "25" + "Y", 0248);

                    Values.Add(line + "26" + "X", 1227 + width * 2);
                    Values.Add(line + "26" + "Y", 0248);

                    Values.Add(line + "17" + "X", 1227 + width * 1);
                    Values.Add(line + "17" + "Y", 0248 + height * 1);

                    Values.Add(line + "18" + "X", 1227 + width * 2);
                    Values.Add(line + "18" + "Y", 0248 + height * 1);

                    Values.Add(line + "19" + "X", 1227 + width * 1);
                    Values.Add(line + "19" + "Y", 0248 + height * 2);

                    Values.Add(line + "20" + "X", 1227 + width * 2);
                    Values.Add(line + "20" + "Y", 0248 + height * 2);

                    Values.Add(line + "21" + "X", 1227 + width * 0);
                    Values.Add(line + "21" + "Y", 0248 + height * 3);

                    Values.Add(line + "22" + "X", 1227 + width * 1);
                    Values.Add(line + "22" + "Y", 0248 + height * 3);

                    Values.Add(line + "23" + "X", 1227 + width * 2);
                    Values.Add(line + "23" + "Y", 0248 + height * 3);

                    Values.Add(line + "27" + "X", 1227 + 20);
                    Values.Add(line + "27" + "Y", 0248 + height * 4);

                    Values.Add(line + "28" + "X", 1227 + 20 + width * 1);
                    Values.Add(line + "28" + "Y", 0248 + height * 4);

                    Values.Add(line + "30" + "X", 1057);
                    Values.Add(line + "30" + "Y", 0248 + height * 3);

                    Values.Add(line + "29" + "X", 0417);
                    Values.Add(line + "29" + "Y", 0248 + height * 3);

                    Values.Add(line + "39" + "X", 0161);
                    Values.Add(line + "39" + "Y", 0288);

                    Values.Add(line + "40" + "X", 0161 + width * 1);
                    Values.Add(line + "40" + "Y", 0288);

                    Values.Add(line + "41" + "X", 0161 + width * 2);
                    Values.Add(line + "41" + "Y", 0288);

                    Values.Add(line + "35" + "X", 0161);
                    Values.Add(line + "35" + "Y", 0288 + height * 1);

                    Values.Add(line + "36" + "X", 0161 + width * 1);
                    Values.Add(line + "36" + "Y", 0288 + height * 1);

                    Values.Add(line + "37" + "X", 0161);
                    Values.Add(line + "37" + "Y", 0288 + height * 2);

                    Values.Add(line + "38" + "X", 0161 + width * 1);
                    Values.Add(line + "38" + "Y", 0288 + height * 2);

                    Values.Add(line + "31" + "X", 0161 - width * 2);
                    Values.Add(line + "31" + "Y", 0288 + height * 3);

                    Values.Add(line + "32" + "X", 0161 - width * 1);
                    Values.Add(line + "32" + "Y", 0288 + height * 3);

                    Values.Add(line + "42" + "X", 0161);
                    Values.Add(line + "42" + "Y", 0288 + height * 3);

                    Values.Add(line + "43" + "X", 0161 + width * 1);
                    Values.Add(line + "43" + "Y", 0288 + height * 3);

                    Values.Add(line + "44" + "X", 0161 + width * 2);
                    Values.Add(line + "44" + "Y", 0288 + height * 3);

                    Values.Add(line + "45" + "X", 0161 + width * 3);
                    Values.Add(line + "45" + "Y", 0288 + height * 3);

                    Values.Add(line + "46" + "X", 0161 + width * 4);
                    Values.Add(line + "46" + "Y", 0288 + height * 3);

                    Values.Add(line + "47" + "X", 0161 + 20 - width * 2);
                    Values.Add(line + "47" + "Y", 0288 + height * 4);

                    Values.Add(line + "33" + "X", 0161 + 20);
                    Values.Add(line + "33" + "Y", 0288 + height * 4);

                    Values.Add(line + "34" + "X", 0161 + 20 + width);
                    Values.Add(line + "34" + "Y", 0288 + height * 4);

                }
            }

            // 1F
            {
                int width = 40;
                int height = 63;
                {
                    string line = "1F";
                    Values.Add(line + "1" + "X", 0109);
                    Values.Add(line + "1" + "Y", 0255);

                    Values.Add(line + "2" + "X", 0109);
                    Values.Add(line + "2" + "Y", 0255 + height * 1 + 3);

                    Values.Add(line + "3" + "X", 0982);
                    Values.Add(line + "3" + "Y", 0561);

                    Values.Add(line + "4" + "X", 0982);
                    Values.Add(line + "4" + "Y", 0561 + height * 1 + 3);

                    Values.Add(line + "5" + "X", 0567);
                    Values.Add(line + "5" + "Y", 0563);

                    Values.Add(line + "6" + "X", 0567);
                    Values.Add(line + "6" + "Y", 0563 - height * 1);

                    Values.Add(line + "7" + "X", 0090);
                    Values.Add(line + "7" + "Y", 0563);

                    Values.Add(line + "8" + "X", 0090);
                    Values.Add(line + "8" + "Y", 0563 - height * 1);

                    Values.Add(line + "9" + "X", 0567);
                    Values.Add(line + "9" + "Y", 0563 + height * 2);

                    Values.Add(line + "10" + "X", 0567);
                    Values.Add(line + "10" + "Y", 0563 + height * 1);

                    Values.Add(line + "11" + "X", 0090);
                    Values.Add(line + "11" + "Y", 0563 + height * 2);

                    Values.Add(line + "12" + "X", 0090);
                    Values.Add(line + "12" + "Y", 0563 + height * 1);

                    Values.Add(line + "13" + "X", 0618);
                    Values.Add(line + "13" + "Y", 0250);

                    Values.Add(line + "14" + "X", 1400 - width * 1);
                    Values.Add(line + "14" + "Y", 0160);

                    Values.Add(line + "15" + "X", 1400);
                    Values.Add(line + "15" + "Y", 0160);

                }
            }

            // 2F
            {
                int width = 40;
                int height = 60;
                {
                    string line = "2F";
                    Values.Add(line + "1" + "X", 0718);
                    Values.Add(line + "1" + "Y", 0152);

                    Values.Add(line + "2" + "X", 1238);
                    Values.Add(line + "2" + "Y", 0152);

                    Values.Add(line + "3" + "X", 0778);
                    Values.Add(line + "3" + "Y", 0152);

                    Values.Add(line + "4" + "X", 0120);
                    Values.Add(line + "4" + "Y", 0180);

                    Values.Add(line + "5" + "X", 0463);
                    Values.Add(line + "5" + "Y", 0227);

                    Values.Add(line + "6" + "X", 0591);
                    Values.Add(line + "6" + "Y", 0264);

                    Values.Add(line + "7" + "X", 0591);
                    Values.Add(line + "7" + "Y", 0264 + height * 1);

                    Values.Add(line + "8" + "X", 0977);
                    Values.Add(line + "8" + "Y", 0264);

                    Values.Add(line + "9" + "X", 0977);
                    Values.Add(line + "9" + "Y", 0264 + height * 1);

                    Values.Add(line + "10" + "X", 0147);
                    Values.Add(line + "10" + "Y", 0267);

                    Values.Add(line + "11" + "X", 0147);
                    Values.Add(line + "11" + "Y", 0267 + height * 1);

                    Values.Add(line + "12" + "X", 0093);
                    Values.Add(line + "12" + "Y", 0348);

                    Values.Add(line + "13" + "X", 0093);
                    Values.Add(line + "13" + "Y", 0348 + height * 1);

                    Values.Add(line + "14" + "X", 0933);
                    Values.Add(line + "14" + "Y", 0488);

                    Values.Add(line + "15" + "X", 0933);
                    Values.Add(line + "15" + "Y", 0488 + height * 1);

                    Values.Add(line + "16" + "X", 0148);
                    Values.Add(line + "16" + "Y", 0511);

                    Values.Add(line + "17" + "X", 0686);
                    Values.Add(line + "17" + "Y", 0399);

                    Values.Add(line + "18" + "X", 0993);
                    Values.Add(line + "18" + "Y", 0399);

                    Values.Add(line + "19" + "X", 0686);
                    Values.Add(line + "19" + "Y", 0399 + 80);

                    Values.Add(line + "20" + "X", 0993);
                    Values.Add(line + "20" + "Y", 0399 + 80);

                    Values.Add(line + "21" + "X", 1148);
                    Values.Add(line + "21" + "Y", 0278);

                    Values.Add(line + "22" + "X", 1241);
                    Values.Add(line + "22" + "Y", 0336);

                    Values.Add(line + "23" + "X", 1241);
                    Values.Add(line + "23" + "Y", 0336 + 70 * 1);

                    Values.Add(line + "24" + "X", 1241);
                    Values.Add(line + "24" + "Y", 0336 + 70 * 2);

                    Values.Add(line + "25" + "X", 0348);
                    Values.Add(line + "25" + "Y", 0288);

                    Values.Add(line + "26" + "X", 0348);
                    Values.Add(line + "26" + "Y", 0288 + height * 1);

                    Values.Add(line + "27" + "X", 0611);
                    Values.Add(line + "27" + "Y", 0577);

                    Values.Add(line + "28" + "X", 0611);
                    Values.Add(line + "28" + "Y", 0577 + height * 1);

                    Values.Add(line + "29" + "X", 0698);
                    Values.Add(line + "29" + "Y", 0268);

                    Values.Add(line + "31" + "X", 0698);
                    Values.Add(line + "31" + "Y", 0268 + height * 1);

                    Values.Add(line + "30" + "X", 0872);
                    Values.Add(line + "30" + "Y", 0268);

                    Values.Add(line + "32" + "X", 0872);
                    Values.Add(line + "32" + "Y", 0268 + height * 1);

                    Values.Add(line + "33" + "X", 0190);
                    Values.Add(line + "33" + "Y", 0152);

                    Values.Add(line + "36" + "X", 0190 + width * 1);
                    Values.Add(line + "36" + "Y", 0152);

                    Values.Add(line + "34" + "X", 0828);
                    Values.Add(line + "34" + "Y", 148);

                    Values.Add(line + "35" + "X", 1318);
                    Values.Add(line + "35" + "Y", 0182);

                }
            }

            // 3F
            {
                int width = 40;
                //int height = 60;
                {
                    string line = "3F";

                    Values.Add(line + "1" + "X", 0242);
                    Values.Add(line + "1" + "Y", 0308);

                    Values.Add(line + "2" + "X", 0342);
                    Values.Add(line + "2" + "Y", 0250);

                    Values.Add(line + "3" + "X", 0342 - width * 1);
                    Values.Add(line + "3" + "Y", 0250);

                    Values.Add(line + "4" + "X", 0721);
                    Values.Add(line + "4" + "Y", 0295);

                    Values.Add(line + "5" + "X", 0885);
                    Values.Add(line + "5" + "Y", 0300);

                    Values.Add(line + "6" + "X", 0905);
                    Values.Add(line + "6" + "Y", 0245);

                    Values.Add(line + "7" + "X", 0525);
                    Values.Add(line + "7" + "Y", 0252);

                    Values.Add(line + "8" + "X", 0525 - width * 1);
                    Values.Add(line + "8" + "Y", 0252);

                    Values.Add(line + "9" + "X", 1285);
                    Values.Add(line + "9" + "Y", 0301);

                    Values.Add(line + "10" + "X", 1070);
                    Values.Add(line + "10" + "Y", 0255);

                    Values.Add(line + "11" + "X", 1251);
                    Values.Add(line + "11" + "Y", 0245);

                    Values.Add(line + "12" + "X", 1136);
                    Values.Add(line + "12" + "Y", 0310);

                    Values.Add(line + "13" + "X", 0300);
                    Values.Add(line + "13" + "Y", 0308);

                    Values.Add(line + "14" + "X", 0408);
                    Values.Add(line + "14" + "Y", 0308);

                    Values.Add(line + "15" + "X", 0466);
                    Values.Add(line + "15" + "Y", 0308);

                    Values.Add(line + "16" + "X", 0947);
                    Values.Add(line + "16" + "Y", 0300);

                    Values.Add(line + "17" + "X", 1003);
                    Values.Add(line + "17" + "Y", 0315);

                    Values.Add(line + "18" + "X", 1026);
                    Values.Add(line + "18" + "Y", 0257);

                    Values.Add(line + "19" + "X", 0242);
                    Values.Add(line + "19" + "Y", 0380);

                    Values.Add(line + "20" + "X", 0336);
                    Values.Add(line + "20" + "Y", 0418);

                    Values.Add(line + "21" + "X", 0332 - width * 1);
                    Values.Add(line + "21" + "Y", 0418);

                    Values.Add(line + "22" + "X", 0721);
                    Values.Add(line + "22" + "Y", 0378);

                    Values.Add(line + "23" + "X", 0885);
                    Values.Add(line + "23" + "Y", 0378);

                    Values.Add(line + "24" + "X", 0905);
                    Values.Add(line + "24" + "Y", 0436);

                    Values.Add(line + "25" + "X", 0525);
                    Values.Add(line + "25" + "Y", 0431);

                    Values.Add(line + "26" + "X", 0525 - width * 1);
                    Values.Add(line + "26" + "Y", 0431);

                    Values.Add(line + "27" + "X", 1008);
                    Values.Add(line + "27" + "Y", 0447);

                    Values.Add(line + "28" + "X", 1285);
                    Values.Add(line + "28" + "Y", 0374);

                    Values.Add(line + "29" + "X", 1243);
                    Values.Add(line + "29" + "Y", 0430);

                    Values.Add(line + "30" + "X", 1136);
                    Values.Add(line + "30" + "Y", 0378);

                    Values.Add(line + "31" + "X", 0296);
                    Values.Add(line + "31" + "Y", 0363);

                    Values.Add(line + "32" + "X", 0408);
                    Values.Add(line + "32" + "Y", 0375);

                    Values.Add(line + "33" + "X", 0466);
                    Values.Add(line + "33" + "Y", 0365);

                    Values.Add(line + "34" + "X", 0954);
                    Values.Add(line + "34" + "Y", 0378);

                    Values.Add(line + "35" + "X", 1003);
                    Values.Add(line + "35" + "Y", 0378);

                    Values.Add(line + "36" + "X", 1080);
                    Values.Add(line + "36" + "Y", 0446);

                    Values.Add(line + "37" + "X", 0218);
                    Values.Add(line + "37" + "Y", 0214);

                    Values.Add(line + "38" + "X", 0839);
                    Values.Add(line + "38" + "Y", 0210);

                    Values.Add(line + "39" + "X", 0988);
                    Values.Add(line + "39" + "Y", 0590);

                    Values.Add(line + "40" + "X", 1058);
                    Values.Add(line + "40" + "Y", 0516);

                    Values.Add(line + "41" + "X", 1115);
                    Values.Add(line + "41" + "Y", 0609);

                    Values.Add(line + "42" + "X", 1097);
                    Values.Add(line + "42" + "Y", 0206);

                    Values.Add(line + "43" + "X", 1016);
                    Values.Add(line + "43" + "Y", 0138);


                }
            }

        }

        public static void GetCoordinate(string parent, int index, out int x, out int y)
        {
            x = (int)Values[parent + index.ToString() + "X"];
            y = (int)Values[parent + index.ToString() + "Y"];
        }
    }

    public class ScaleCurrent
    {
        public string Factory = string.Empty;
        public string DB = string.Empty;
        public string Code = string.Empty;
        public double ScaleMin = 0.0;
        public double ScaleMax = 0.0;
        public double PeakAlarm = 0.0;
        public double PeakFault = 0.0;
        public double MeanAlarm = 0.0;
        public double MeanFault = 0.0;

        internal static List<ScaleCurrent> LoadScales(string dataConn)
        {
            List<ScaleCurrent> list = new List<ScaleCurrent>();
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = dataConn;
                SqlCommand qry = new SqlCommand();
                qry.Connection = conn;
                string cmd = " SELECT * FROM ScaleLog AS S " +
                    " WHERE  Exists ( SELECT 1 FROM ScaleLog WHERE Code = S.Code HAVING S.Changed = Max(Changed) ) " +
                    " ORDER BY Code ASC ";

                Database.MakeQueryString(qry, cmd);
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlDataReader reader = qry.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            ScaleCurrent sc = new ScaleCurrent();
                            sc.Factory = Database.getStringValue(reader, "Factory");
                            sc.DB = Database.getStringValue(reader, "DB");
                            sc.Code = Database.getStringValue(reader, "Code");
                            sc.ScaleMin = Database.getDoubleValue(reader, "ScaleMin");
                            sc.ScaleMax = Database.getDoubleValue(reader, "ScaleMax");
                            sc.PeakAlarm = Database.getDoubleValue(reader, "PeakAlarm");
                            sc.PeakFault = Database.getDoubleValue(reader, "PeakFault");
                            sc.MeanAlarm = Database.getDoubleValue(reader, "MeanAlarm");
                            sc.MeanFault = Database.getDoubleValue(reader, "MeanFault");
                            list.Add(sc);
                        }
                    }
                }
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            catch (Exception e)
            {
                //Util.WriteLog("Load ScaleList Failed. " + e.Message, "LogErr");
                string msg = e.Message;
            }

            return list;
        }
    }
}

namespace KMX_SMART
{
    public class MaintAlarm
    {
        public int Shop = 0; // 1:ST, 2:WE, 3:PA, 4:GA
        public DateTime alarmDate;
        public string parentName;
        public string pointName;
        public string processName;
    }

    public class VibAarm
    {
        public string ParentName = string.Empty;
        public DateTime AlarmDate = DateTime.MinValue;
    }

    public class ErrorRobot
    {
        public int RobotKey = -1;
        public string RobotName = string.Empty;
        public DateTime dtEvent = DateTime.MinValue;
        public string ErrorCode = string.Empty;
        public string Description = string.Empty;
        public int Checked = 0;
    }

    public class ErrorVibration
    {

    }

    public class ErrorCurrent
    {

    }
}

namespace YONGSAN_CPAD_VISION
{
    public class CONFIG
    {
        public static string GetConnectionString(string ini, out string dataSource)
        {
            dataSource = Util.GetIniFileString(ini, "Server", "Data Source", "");
            string user = Util.GetIniFileString(ini, "Server", "User ID", "");
            string pass = Util.GetIniFileString(ini, "Server", "Password", "");
            string catalog = Util.GetIniFileString(ini, "Server", "Initial Catalog", "");

            return "Password=" + pass + ";Persist Security Info=True;" +
                "User ID=" + user + ";Initial Catalog=" + catalog + ";Data Source=" + dataSource + ";Connection Timeout=5";
        }
    }

    public class Model
    {
        public int ModelID;
        public string ModelServerName;
        public int ModelPLCNumber = 0;
        public string ModelImage;
        public List<CameraObject> CameraList = new List<CameraObject>();
        public List<Step> StepList = new List<Step>();

        public Model()
        {

        }

        public static List<Model> GetModelList(string iniModel, List<CameraObject> camList)
        {
            List<Model> list = new List<Model>();

            int MaxID = Util.GetIniFileInt(iniModel, "Model", "MaxID", 0);
            for (int i = 1; i <= MaxID; i++)
            {
                string key = "MODEL" + i.ToString();
                string name = Util.GetIniFileString(iniModel, key, "ServerName", string.Empty);
                if (name == string.Empty)
                    continue;

                Model mo = new Model();
                mo.ModelID = Util.GetIniFileInt(iniModel, key, "ID", 0);
                mo.ModelServerName = Util.GetIniFileString(iniModel, key, "ServerName", string.Empty);
                mo.ModelPLCNumber = Util.GetIniFileInt(iniModel, key, "PLCNumber");
                mo.ModelImage = Util.GetIniFileString(iniModel, key, "Image", string.Empty);
                if (!File.Exists(mo.ModelImage) && mo.ModelImage.LastIndexOf("\\") > 0)
                    mo.ModelImage = Util.GetWorkingDirectory() + "\\Images\\" + Path.GetFileName(mo.ModelImage);

                mo.CameraList = camList;
                mo.StepList = Step.GetStepList(iniModel, mo);
                list.Add(mo);
            }
            return list;
        }

        internal static void Remove(string ini, Model mo)
        {
            string key = "MODEL" + mo.ModelID.ToString();
            Util.SetIniFileNull(ini, key);
        }
    }

    public enum HIT_TEST
    {
        _NONE
        , _MOVE, _LEFTTOP, _TOP, _RIGHTTOP
        , _LEFT, _RIGHT
        , _LEFTBOTTOM, _BOTTOM, _RIGHTBOTTOM
    };

    public enum JUDGE_RESULT { _NONE = -1, _NG = 0, _OK = 1 }
    public class Step
    {
        public Model owner = null;
        public int CameraID = 0;
        public int StepNo;
        public int Rotate = 0;
        public bool ShowArea = true;
        public int ShowTools = 1; // 0:ALL, 1:ALL ON, 2:NG ON
        public List<VisionTool> VisionToolList = new List<VisionTool>();
        public Rectangle rcTarget = new Rectangle(320, 180, 140, 100);
        public GraphicsPath objPath = new GraphicsPath();
        public GraphicsPath[] objPathHandler = new GraphicsPath[8];
        public HIT_TEST HitTest = HIT_TEST._NONE;
        public bool IsSelected = false;
        public JUDGE_RESULT StepResult = JUDGE_RESULT._NONE;
        public string StepImageFile = string.Empty;
        public bool ResultVisible = true;

        internal static List<Step> GetStepList(string ini, Model mo)
        {
            List<Step> list = new List<Step>();
            string category = "MODEL" + mo.ModelID.ToString();
            for (int j = 0; j < mo.CameraList.Count; j++)
            {
                int cameraID = mo.CameraList[j].ID;
                int MaxID = 100;// Util.GetIniFileInt(ini, category, "Camera" + cameraID.ToString() + "StepCount", 0);
                for (int i = 0; i <= MaxID; i++)
                {
                    string key = Step.GetStepKeyName(mo.ModelID, cameraID, i);
                    string no = Util.GetIniFileString(ini, key, "No", string.Empty);
                    if (no == string.Empty)
                        continue;

                    Step st = new Step();
                    if (!Int32.TryParse(no, out st.StepNo))
                        continue;

                    st.owner = mo;
                    st.CameraID = Int32.Parse(Util.GetIniFileString(ini, key, "CameraID", "1"));
                    st.Rotate = Int32.Parse(Util.GetIniFileString(ini, key, "Rotate", "0"));
                    st.ShowArea = Util.GetIniFileString(ini, key, "ShowArea", "1") == "1";
                    st.ShowTools = Int32.Parse(Util.GetIniFileString(ini, key, "ShowTools", "1"));
                    st.rcTarget = new Rectangle(Util.GetIniFileInt(ini, key, "X", 0), Util.GetIniFileInt(ini, key, "Y", 0),
                                            Util.GetIniFileInt(ini, key, "W", 0), Util.GetIniFileInt(ini, key, "H", 0));
                    st.rcTarget.Width = Math.Max(st.rcTarget.Width, 100);
                    st.rcTarget.Height = Math.Max(st.rcTarget.Height, 100);

                    st.VisionToolList = VisionTool.GetVisionToolList(ini, mo, st);

                    list.Add(st);
                }
            }
            return list;
        }

        internal static string GetStepKeyName(int modelID, int cameraID, int stepNo)
        {
            return "MODEL" + modelID.ToString() + "-" + "CAMERA" + cameraID.ToString() + "-" + "STEP" + stepNo.ToString();
        }

        internal HIT_TEST GetHitTest(int x, int y)
        {
            if (objPathHandler[0] != null && objPathHandler[0].IsVisible(new Point(x, y)))
                return HIT_TEST._LEFTTOP;

            if (objPathHandler[1] != null && objPathHandler[1].IsVisible(new Point(x, y)))
                return HIT_TEST._TOP;

            if (objPathHandler[2] != null && objPathHandler[2].IsVisible(new Point(x, y)))
                return HIT_TEST._RIGHTTOP;

            if (objPathHandler[3] != null && objPathHandler[3].IsVisible(new Point(x, y)))
                return HIT_TEST._LEFT;

            if (objPathHandler[4] != null && objPathHandler[4].IsVisible(new Point(x, y)))
                return HIT_TEST._RIGHT;

            if (objPathHandler[5] != null && objPathHandler[5].IsVisible(new Point(x, y)))
                return HIT_TEST._LEFTBOTTOM;

            if (objPathHandler[6] != null && objPathHandler[6].IsVisible(new Point(x, y)))
                return HIT_TEST._BOTTOM;

            if (objPathHandler[7] != null && objPathHandler[7].IsVisible(new Point(x, y)))
                return HIT_TEST._RIGHTBOTTOM;

            if (objPath.IsVisible(new Point(x, y)))
                return HIT_TEST._MOVE;

            return HIT_TEST._NONE;
        }

        internal static void Remove(string ini, Model mo, Step s)
        {
            string key = Step.GetStepKeyName(mo.ModelID, s.CameraID, s.StepNo);
            Util.SetIniFileNull(ini, key);
        }

        internal static Step Add(string ini, Model mo, int cameraID, out int no)
        {
            no = 0;
            for (int i = 0; i < mo.StepList.Count; i++)
            {
                int camID = mo.StepList[i].CameraID;
                if (camID == cameraID)
                {
                    int newID = no;
                    Step s = mo.StepList.Find(x => x.StepNo == newID);
                    if (s == null)
                        break;
                    no++;
                }
            }

            Step st = new Step();
            st.owner = mo;
            st.StepNo = no;
            st.CameraID = cameraID;
            st.Rotate = 0;
            st.ShowArea = true;
            st.ShowTools = 1;
            mo.StepList.Add(st);

            string category = Step.GetStepKeyName(mo.ModelID, st.CameraID, no);
            Util.SetIniFileString(ini, category, "No", st.StepNo.ToString());
            Util.SetIniFileString(ini, category, "CameraID", st.CameraID.ToString());
            Util.SetIniFileString(ini, category, "Rotate", st.Rotate.ToString());
            Util.SetIniFileString(ini, category, "ShowArea", st.ShowArea ? "1" : "0");
            Util.SetIniFileString(ini, category, "ShowTools", st.ShowTools.ToString());
            Util.SetIniFileString(ini, category, "X", st.rcTarget.X.ToString());
            Util.SetIniFileString(ini, category, "Y", st.rcTarget.Y.ToString());
            Util.SetIniFileString(ini, category, "W", st.rcTarget.Width.ToString());
            Util.SetIniFileString(ini, category, "H", st.rcTarget.Height.ToString());

            return st;
        }
    }

    public class VisionTool
    {
        public Step owner = null;
        public string Number = "00";
        public string Result = "NA";
        public string Value = "0000000";
        public int PositionX = 0;
        public int PositionY = 0;
        public PictureBox picToolResult = null;

        public VisionTool(Step s)
        {
            owner = s;
        }

        internal static List<VisionTool> GetVisionToolList(string ini, Model mo, Step s)
        {
            List<VisionTool> list = new List<VisionTool>();
            string keyStep = Step.GetStepKeyName(mo.ModelID, s.CameraID, s.StepNo);
            if (!Int32.TryParse(Util.GetIniFileString(ini, keyStep, "ToolCount", "0"), out int toolCount))
                return null;

            for (int j = 0; j < toolCount; j++)
            {
                int index = j + 1;
                VisionTool vt = new VisionTool(s);
                string keyToolX = "TOOL" + index.ToString() + "X";
                Int32.TryParse(Util.GetIniFileString(ini, keyStep, keyToolX, "0"), out vt.PositionX);
                string keyToolY = "TOOL" + index.ToString() + "Y";
                Int32.TryParse(Util.GetIniFileString(ini, keyStep, keyToolY, "0"), out vt.PositionY);

                list.Add(vt);
            }
            return list;
        }
    }

    public enum USER_MESSAGE { WM_USER = 0x0400 }
    public class YONGSAN_VISION_CORE
    {
        // PInvoke declaration for SendMessage
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam = 0, string lParam = "");
        // Windows Messages Constants
        public static uint WM_USER_PAINT = ((int)USER_MESSAGE.WM_USER) + 100;

        public static uint WM_USER_STATUS_PLC = ((int)USER_MESSAGE.WM_USER) + 201;
        public static uint WM_USER_STATUS_SERVER = ((int)USER_MESSAGE.WM_USER) + 202;

        public static uint WM_USER_LOG_CAMERA = ((int)USER_MESSAGE.WM_USER) + 1001;
        public static uint WM_USER_LOG_PLC = ((int)USER_MESSAGE.WM_USER) + 1002;

        public static uint WM_USER_TEST_INIT = ((int)USER_MESSAGE.WM_USER) + 2000;
        public static uint WM_USER_TEST_MODELNAME = ((int)USER_MESSAGE.WM_USER) + 2100;
        public static uint WM_USER_FINAL_IMAGE = ((int)USER_MESSAGE.WM_USER) + 3000;

        public static uint WM_USER_RESULT = ((int)USER_MESSAGE.WM_USER) + 4000;

        public static uint WM_USER_MESSAGE_CAMERA = ((int)USER_MESSAGE.WM_USER) + 5001;
        public static uint WM_USER_MESSAGE_GENERAL = ((int)USER_MESSAGE.WM_USER) + 8001;

        public static uint WM_USER_DATETIME = ((int)USER_MESSAGE.WM_USER) + 8500;
        public static uint WM_USER_FLICKER = ((int)USER_MESSAGE.WM_USER) + 9999;

        public string DataFolder { get; set; } = string.Empty;
        public string SaveFolder { get; set; } = string.Empty;
        public bool Initialized { get; set; } = false;
        public List<Model> ModelList = null;

        public List<CameraObject> CameraList = null;
        public Thread[] WorkingThread = null;
        public int IsNewData = 0;
        private XGCommSocket XGComm = new XGCommSocket();
        public Form Owner = null;
        public System.Threading.Timer timerWorker = null;

        public string EQUIPMENT = string.Empty;
        public string iniSetup = string.Empty;
        public string iniPLC = string.Empty;
        public string iniCamera = string.Empty;
        public string iniModel = string.Empty;
        public string iniServer = string.Empty;

        delegate void FormSendMessageCallback(uint message, int wparam, string lparam);
        private void FormSendMessage(uint message, int wparam = 0, string lparam = "")
        {
            if (Owner == null)
                return;

            if (Owner.InvokeRequired)
            {
                FormSendMessageCallback cb = new FormSendMessageCallback(FormSendMessage);
                Owner.Invoke(cb, new object[] { message, wparam, lparam });
            }
            else
            {
                SendMessage(Owner.Handle, message, wparam, lparam);
            }
        }

        public YONGSAN_VISION_CORE(Form form, string equip)
        {
            Owner = form;
            EQUIPMENT = equip;

            if (Load())
                this.Initialized = true;
        }

        internal void CloseObject()
        {
            for (int i = 0; i < CameraList.Count; i++)
            {
                CameraObject cam = CameraList[i];
                try
                {
                    if (cam != null && cam.Client.Connected)
                    {
                        WorkingThread[i].Abort();
                        cam.timer.Stop();
                        cam.Client.Close();
                    }
                }
                catch (Exception)
                {
                }
            }

            FormSendMessage(WM_USER_FLICKER, 0, "");

            // 워커스레드 종료
            timerWorker.Change(Timeout.Infinite, Timeout.Infinite);
            timerWorker.Dispose();
        }

        public void timerWorker_Callback(object state)
        {
            timerWorker.Change(Timeout.Infinite, Timeout.Infinite);
            DoProcess();

            int interval = 1000;// Int32.Parse(Util.GetIniFileString(iniSetup, "Setup", "TimerInterval", "1000"));
            timerWorker.Change(0, interval);
        }

        public Model CurrentModel = null;
        internal bool Load()
        {
            try
            {
                iniSetup = Util.GetWorkingDirectory() + "\\Setup" + EQUIPMENT + ".ini";
                using (FileStream f = File.Open(iniSetup, FileMode.OpenOrCreate)) { }
                iniPLC = Util.GetWorkingDirectory() + "\\PLC" + EQUIPMENT + ".ini";
                using (FileStream f = File.Open(iniPLC, FileMode.OpenOrCreate)) { }
                iniCamera = Util.GetWorkingDirectory() + "\\Camera" + EQUIPMENT + ".ini";
                using (FileStream f = File.Open(iniCamera, FileMode.OpenOrCreate)) { }
                iniModel = Util.GetWorkingDirectory() + "\\Model" + EQUIPMENT + ".ini";
                using (FileStream f = File.Open(iniModel, FileMode.OpenOrCreate)) { }
                iniServer = Util.GetWorkingDirectory() + "\\Server" + EQUIPMENT + ".ini";
                using (FileStream f = File.Open(iniServer, FileMode.OpenOrCreate)) { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Create INI File failed. " + ex.Message);
                return false;
            }

            DataFolder = Util.GetIniFileString(iniSetup, "Setup", "DataFolder", "C:\\TP\\DATA");
            SaveFolder = Util.GetIniFileString(iniSetup, "Setup", "SaveFolder", "C:\\VISION_DATA");

            InitPLC();

            InitServerDatabase();

            CameraList = CameraObject.GetCameraList(iniCamera);
            CurrentStep = new int[CameraList.Count];
            CurrentFinal = new int[CameraList.Count];
            CurrentTrigger = new int[CameraList.Count];
            PreviousTrigger = new int[CameraList.Count];

            ModelList = Model.GetModelList(iniModel, CameraList);

            InitCamera();

            return true;
        }

        private void InitPLC()
        {
            string ip = Util.GetIniFileString(iniPLC, "PLC", "IP", "192.168.0.1");
            if (Util.PingTest(ip))
                FormSendMessage(WM_USER_STATUS_PLC, 1, "");
            else
                FormSendMessage(WM_USER_STATUS_PLC, 0, "");
        }

        private void InitServerDatabase()
        {
            SqlConnection conn = new SqlConnection(CONFIG.GetConnectionString(iniServer, out string ip));
            if (ip == string.Empty)
                return;
            string[] split = ip.Split(',');
            if (!Util.PingTest(split[0]))
            {
                FormSendMessage(WM_USER_STATUS_SERVER, 0, "");
                return;
            }

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                FormSendMessage(WM_USER_STATUS_SERVER, 1, "");
            }
            catch (Exception e)
            {
                FormSendMessage(WM_USER_STATUS_SERVER, 0, "");
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private void InitCamera()
        {
            // 카매라 수만큼 스레드 생성
            if (CameraList.Count > 0)
                WorkingThread = new Thread[CameraList.Count];

            while (true)
            {
                bool allConnected = true;
                for (int i = 0; i < CameraList.Count; i++)
                {
                    CameraObject cam = CameraList[i];
                    try
                    {
                        if (cam.Client == null || !cam.Client.Connected)
                        {
                            cam.Client = new TcpClient(cam.IPAddress, cam.Port);
                            Thread.Sleep(1000);
                            cam.Stream = cam.Client.GetStream();
                            cam.Send("OE,0\r", out string errMsg);
                            FormSendMessage(WM_USER_LOG_CAMERA, cam.ID, "SENT: OE,0");
                        }

                        if (cam.Client.Connected)
                        {
                            WorkingThread[i] = new Thread(new ParameterizedThreadStart(ThreadTask));
                            WorkingThread[i].Start(cam);
                            allConnected &= cam.Client.Connected;

                            // 각 카메라마다 동작하는 타이머 설정
                            cam.timer = new System.Windows.Forms.Timer();

                            cam.timer.Interval = Util.GetIniFileInt(iniPLC, "PLC", "Interval", 1000);
                            cam.timer.Tick += (sender, e) => TimerEventProcessor(sender, e, cam.ID);
                            cam.timer.Start();
                        }
                    }
                    catch (Exception)
                    {
                        allConnected &= false;
                        break;
                    }
                }
                if (allConnected)
                    break;
                else
                    Application.DoEvents();
            }

            TimerCallback callback = new TimerCallback(timerWorker_Callback);
            timerWorker = new System.Threading.Timer(callback, null, 1000, 1000);
        }

        private void TimerEventProcessor(object sender, EventArgs e, int index)
        {
            for (int i = 0; i < CameraList.Count; i++)
            {
                CameraObject cam = CameraList[i];
                if (cam.ID == index)
                    ReadPLCData(cam);
            }
        }

        private void DoProcess()
        {
            if (IsNewData == 0)
            {
                if (true == ReadServerDatabase(out bool isReset))
                    ModelChanged(CurrentModel.ModelServerName);
                else if (isReset == true)
                {
                    // 모든 카메라에 대해 트리거 리셋
                    for (int i = 0; i < CameraList.Count; i++)
                    {
                        CameraObject cam = CameraList[i];
                        string category = "CAMERA" + cam.ID.ToString() + "TRIGGER";
                        char cDeviceType = Util.GetIniFileString(iniPLC, category, "DeviceType", "D")[0];
                        int addrTrigger = Int32.Parse(Util.GetIniFileString(iniPLC, category, "Address", "100"));
                        WritePLCData(cDeviceType, addrTrigger, 0);
                    }
                    IsNewData = 0;
                }
            }

            if (IsNewData == 1)
            {
                if (CurrentModel == null)
                    return;

                // 모델 셋팅
                int addrModel = Int32.Parse(Util.GetIniFileString(iniPLC, "Model", "Address", "150"));
                char cDeviceType = Util.GetIniFileString(iniPLC, "Model", "DeviceType", "D")[0];
                int modelValue = CurrentModel.ModelPLCNumber;
                WritePLCData(cDeviceType, addrModel, modelValue);

                // 모든 카메라에 대해 트리거 설정
                for (int i = 0; i < CameraList.Count; i++)
                {
                    CameraObject cam = CameraList[i];
                    string category = "CAMERA" + cam.ID.ToString() + "TRIGGER";
                    // New Model ===> 1
                    int addrTrigger = Int32.Parse(Util.GetIniFileString(iniPLC, category, "Address", "100"));
                    WritePLCData(cDeviceType, addrTrigger, 1);
                }

                bool writeSucceed = true;
                // Model
                {
                    UInt16[] bufRead = new UInt16[1];
                    uint uReturn = XGComm.ReadDataWord(cDeviceType, addrModel, 1, false, bufRead);
                    if (uReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                    {
                        int model = bufRead[0];
                        writeSucceed &= (model == modelValue);
                    }
                }
                // Trigger
                {
                    for (int i = 0; i < CameraList.Count; i++)
                    {
                        CameraObject cam = CameraList[i];
                        string category = "CAMERA" + cam.ID.ToString() + "TRIGGER";
                        int addrTrigger = Int32.Parse(Util.GetIniFileString(iniPLC, category, "Address", "100"));
                        UInt16[] bufRead = new UInt16[1];
                        uint uReturn = XGComm.ReadDataWord(cDeviceType, addrTrigger, 1, false, bufRead);
                        if (uReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                        {
                            int trigger = bufRead[0];
                            writeSucceed &= (trigger == 1);
                        }
                    }
                }

                if (writeSucceed == true)
                    IsNewData = 2;
            }

            if (IsNewData == 2)
            {
                bool totalFinished = true;
                for (int i = 0; i < CameraList.Count; i++)
                {
                    CameraObject cam = CameraList[i];
                    totalFinished = (totalFinished && cam.IsFinished && (CurrentFinal[cam.ID - 1] == 1));
                }

                if (totalFinished == true)
                {
                    int totalResult = 1;
                    for (int i = 0; i < CurrentModel.StepList.Count; i++)
                    {
                        if (CurrentModel.StepList[i].StepResult == JUDGE_RESULT._NG)
                        {
                            totalResult = 0;
                            break;
                        }
                    }

                    ProcessFinalResult(totalResult);
                    if (totalResult == 0)
                        FormSendMessage(WM_USER_FLICKER, 1, "");

                    for (int i = 0; i < CameraList.Count; i++)
                    {
                        CameraObject cam = CameraList[i];
                        cam.IsFinished = false;
                    }
                    DateTime dt = DateTime.Now;
                    string finalImageFile = MakeFinalResultImage(dt);
                    string fileName = Path.GetFileName(finalImageFile);
                    UpdateServerData(totalResult, fileName);

                    IsNewData = 0;

                    // 최종 결과 텍스트 파일로 저장
                    string contents = dt.ToString("yyyy-MM-dd HH:mm:ss") + "\t" +
                        CurrentModel.ModelServerName + "\t" +
                        (totalResult == 1 ? "OK" : "NG") + "\t" + fileName;

                    WriteResult(dt, contents);
                }
            }
        }

        private void WriteResult(DateTime dt, string contents)
        {
            string dir = SaveFolder + "\\" + dt.ToString("yyyy") + "\\" +
                dt.ToString("MM-dd");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // 로그 파일 이름(날짜)
            string sLogFile = dir + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            // 내용

            FileStream myFileStream = null;
            try
            {
                myFileStream = new FileStream(sLogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                myFileStream.Seek(0, SeekOrigin.End);
                myFileStream.Write(Encoding.Default.GetBytes(contents), 0, Encoding.Default.GetByteCount(contents));
            }
            finally
            {
                if (null != myFileStream)
                    myFileStream.Close();
            }
        }

        private bool ReadServerDatabase(out bool isReset)
        {
            isReset = false;
            bool inspectionReady = false;

            SqlConnection conn = new SqlConnection(CONFIG.GetConnectionString(iniServer, out string ip));
            if (ip == string.Empty)
                return false;
            string[] split = ip.Split(',');
            if (!Util.PingTest(split[0]))
            {
                FormSendMessage(WM_USER_STATUS_SERVER, 0, "");
                return false;
            }

            string cmd = "SELECT * FROM EQIP_IF WHERE EQIP='" + EQUIPMENT + "'";
            SqlCommand qry = new SqlCommand(cmd, conn);

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlDataReader reader = qry.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader != null && reader.Read())
                    {
                        string status = Database.getStringValue(reader, "STATUS");
                        // 0이면 설비 가동 불가능
                        if (status == "0")
                        {
                            IsNewData = 0;
                        }

                        // 0에서 1로 변경 시 설비 가동 가능
                        if (IsNewData != 1 && status == "1")
                        {
                            string model = Database.getStringValue(reader, "SPEC");
                            CurrentModel = ModelList.Find(x => x.ModelPLCNumber.ToString() == model);
                            if (CurrentModel != null)
                            {
                                inspectionReady = true;
                            }
                        }

                        // 1에서 0으로 변경 시 리셋
                        if (IsNewData != 0 && status == "0")
                        {
                            isReset = true;
                        }
                    }
                }
                FormSendMessage(WM_USER_STATUS_SERVER, 1, "");
            }
            catch (Exception e)
            {
                FormSendMessage(WM_USER_STATUS_SERVER, 0, "");
                Util.WriteLog("Database Error. " + e.Message, "LogErr", "DB");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return inspectionReady;
        }

        private bool ConnectPLC()
        {
            string ip = Util.GetIniFileString(iniPLC, "PLC", "IP", "192.168.0.1");
            if (!Util.PingTest(ip))
            {
                FormSendMessage(WM_USER_STATUS_PLC, 0, "");
                return false;
            }

            string port = Util.GetIniFileString(iniPLC, "PLC", "Port", "2004");
            int retry = 0;
            bool success = false;
            while (retry < 5)
            {
                if (XGComm.Connect(ip, Convert.ToInt32(port)) != (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                {
                    FormSendMessage(WM_USER_STATUS_PLC, 0, "");
                    success = false;
                    Thread.Sleep(100);
                    retry++;
                    continue;
                }
                else
                {
                    FormSendMessage(WM_USER_STATUS_PLC, 1, "");
                    success = true;
                    break;
                }
            }
            return success;
        }

        public int[] CurrentStep = null;
        public int[] CurrentFinal = null;
        public int[] CurrentTrigger = null;
        public int[] PreviousTrigger = null;
        private bool ReadPLCData(CameraObject cam)
        {
            bool success = ConnectPLC();
            if (!success)
            {
                FormSendMessage(WM_USER_LOG_PLC, 0, "PLC Connection FAILED");
                return false;
            }

            string category = "CAMERA" + cam.ID.ToString();

            int cameraIndex = cam.ID - 1;
            // Step Read
            CurrentStep[cameraIndex] = 0;
            {
                char cDeviceType = Util.GetIniFileString(iniPLC, category + "Step", "DeviceType", "D")[0];
                UInt16[] bufRead = new UInt16[1];
                if (Int64.TryParse(Util.GetIniFileString(iniPLC, category + "Step", "Address", "0"), out long address))
                {
                    uint uReturn = XGComm.ReadDataWord(cDeviceType, address, 1, false, bufRead);
                    if (uReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                    {
                        CurrentStep[cameraIndex] = bufRead[0];
                        string s = "PLC Address:" + cDeviceType + address.ToString() + " STEP: " + CurrentStep.ToString();
                        //SendMessage(Owner.Handle, WM_USER_LOG_PLC, 1, s);
                    }
                }
            }

            // Finish Read
            CurrentFinal[cameraIndex] = 0;
            {
                char cDeviceType = Util.GetIniFileString(iniPLC, category + "Finish", "DeviceType", "D")[0];
                UInt16[] bufRead = new UInt16[1];
                if (Int64.TryParse(Util.GetIniFileString(iniPLC, category + "Finish", "Address", "0"), out long address))
                {
                    uint uReturn = XGComm.ReadDataWord(cDeviceType, address, 1, false, bufRead);
                    if (uReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                    {
                        CurrentFinal[cameraIndex] = bufRead[0];
                        string s = "PLC Address:" + cDeviceType + address.ToString() + " FINISH: " + CurrentFinal.ToString();
                        //SendMessage(Owner.Handle, WM_USER_LOG_PLC, 1, s);
                    }
                }
            }

            // Trigger Read
            {
                char cDeviceType = Util.GetIniFileString(iniPLC, category + "Trigger", "DeviceType", "D")[0];
                UInt16[] bufRead = new UInt16[1];
                if (Int64.TryParse(Util.GetIniFileString(iniPLC, category + "Trigger", "Address", "0"), out long address))
                {
                    uint uReturn = XGComm.ReadDataWord(cDeviceType, address, 1, false, bufRead);
                    if (uReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                    {
                        // int readvalue = bufRead[0];
                        //if (cam.ShouldBeWriteTrigger == 3 && readvalue != 3)
                        {
                            //    WritePLCData(cDeviceType, (int)address, 3);
                        }
                        //else
                        {
                            CurrentTrigger[cameraIndex] = bufRead[0];
                            if (CurrentTrigger[cameraIndex] == 2 && CurrentTrigger[cameraIndex] != PreviousTrigger[cameraIndex])
                            {
                                DoTrigger(cam.ID);
                            }
                            string s = "PLC Address:" + cDeviceType + address.ToString() + " TRIGGER: " + CurrentTrigger.ToString();
                            //SendMessage(Owner.Handle, WM_USER_LOG_PLC, 1, s);

                            // 이상 동작으로 트리거가 0으로 변경된 경우 서버 STATUS=4로 업데이트
                            if (CurrentTrigger[cameraIndex] == 0 && CurrentTrigger[cameraIndex] != PreviousTrigger[cameraIndex])
                            {
                                UpdateServerData(4, string.Empty);
                            }
                            PreviousTrigger[cameraIndex] = CurrentTrigger[cameraIndex];
                        }
                    }
                }
            }

            return true;
        }

        private void WritePLCData(char devType, int address, int value)
        {
            bool success = ConnectPLC();
            if (!success)
            {
                string s = "PLC Connection FAILED";
                FormSendMessage(WM_USER_LOG_PLC, 0, s);
                return;
            }
            UInt16[] uWrite = new UInt16[1];
            uWrite[0] = (UInt16)value;
            uint uReturn = XGComm.WriteDataWord(devType, address, 1, false, uWrite);
            if (uReturn != (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
            {
                string s = "WritePLCData Failed. " + devType + address.ToString() + " : " + value.ToString();
                FormSendMessage(WM_USER_LOG_PLC, 0, s);
            }
        }

        private void ThreadTask(object obj)
        {
            CameraObject cam = (CameraObject)obj;
            while (true)
            {
                try
                {
                    if (cam.Client == null || !cam.Client.Connected)
                    {
                        cam.Client = new TcpClient(cam.IPAddress, cam.Port);
                        cam.Stream = cam.Client.GetStream();
                        cam.Send("OE,0\r", out string errMsg);
                    }

                    if (cam.Client.Connected)
                    {
                        string data = cam.Receive(out string errMsg);
                        MessageReceived(cam.ID.ToString() + ":" + data);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void MessageReceived(string data)
        {
            data = data.Replace("\0", "");
            data = data.Replace("\r", "");
            string cameraData = "CameraData Received: " + data;
            FormSendMessage(WM_USER_MESSAGE_CAMERA, 0, cameraData);

            int cameraNum = 0;
            string cmd = string.Empty;
            // 프로그램 번호 전환 응답이 오면 모든 카메라 측정 준비 상태로 전환 (IsFinished = false)
            if (data.Length >= 4)
            {
                cameraNum = Int32.Parse(data.Substring(0, data.IndexOf(":")));
                cmd = data.Substring(data.IndexOf(":") + 1, 2).ToUpper();
                if (cmd == "PW")
                {
                    for (int i = 0; i < CameraList.Count; i++)
                    {
                        CameraObject cam = CameraList[i];
                        if (cam.ID == cameraNum)
                        {
                            cam.Send("FNW,1,1," + cam.ResultFileName + "\r", out string msg);
                            cam.IsFinished = false;
                        }
                    }
                    return;
                }

                // Time Change 응답
                if (cmd == "TC")
                    return;
            }

            if (data.Length >= 7)
            {
                // FTP 저장 파일명
                cmd = data.Substring(data.IndexOf(":") + 1, 3).ToUpper();
                if (cmd == "FNR")
                {
                    for (int i = 0; i < CameraList.Count; i++)
                    {
                        CameraObject cam = CameraList[i];
                        if (cam.ID == cameraNum)
                        {
                            cam.IsFinished = false;
                            //cam.Send("T2\r");
                        }
                    }
                    return;
                }
                cmd = data.Substring(data.IndexOf(":") + 1, 3).ToUpper();
                if (cmd == "FNW")
                {
                    for (int i = 0; i < CameraList.Count; i++)
                    {
                        CameraObject cam = CameraList[i];
                        if (cam.ID == cameraNum)
                        {
                            cam.Send("T2\r", out string msg);
                        }
                    }
                    return;
                }
                cmd = data.Replace(" ", "").Substring(data.IndexOf(":") + 1, 5).ToUpper();
                if (cmd == "ER,T2")
                {
                    for (int i = 0; i < CameraList.Count; i++)
                    {
                        CameraObject cam = CameraList[i];
                        if (cam.ID == cameraNum)
                        {
                            cam.Send("OE,0\r", out string errMsg);
                            cam.Send("T2\r", out errMsg);
                        }
                    }
                    return;
                }
            }
            int idxComma = data.IndexOf(",");
            if (idxComma < 0)
                return;

            // 카메라 번호 추출
            // 1:RT,00025,      NG,     01,         OK,         0000100,02,NG,0000067,03,NG,0000003,04,OK,0000100
            cameraNum = Int32.Parse(data.Substring(0, data.IndexOf(":")));

            // id를 제외한 문자열 추출
            // RT,00025,      NG,     01,         OK,         0000100,02,NG,0000067,03,NG,0000003,04,OK,0000100
            data = data.Substring(2);
            idxComma = data.IndexOf(",");

            cmd = data.Substring(0, idxComma).ToUpper();
            if (cmd == "RT")
            {
                if (CurrentModel == null)
                    return;

                for (int i = 0; i < CameraList.Count; i++)
                {
                    CameraObject cam = CameraList[i];
                    if (cam.ID == cameraNum)
                    {
                        string file = FindFinalImageFile(cam);
                        for (int j = 0; j < CurrentModel.StepList.Count; j++)
                        {
                            Step s = CurrentModel.StepList[j];
                            if (s.StepNo == cam.CurrentStep && s.CameraID == cam.ID)
                            {
                                ParseResultData(cam, s, data.Substring(data.IndexOf(",") + 1));

                                s.StepImageFile = file;
                                // 최종 이미지 생성을 위해 리스트에 추가
                                // 그냥 추가하면 순서가 매번 바뀌기 때문에 인덱스에 맞춰서 저장 
                                //FinalServerImageFileList.Add(file);
                                int pictureIndex = GetPictureIndex(s);
                                if (pictureIndex >= 0)
                                {
                                    FinalServerImage serverImage = new FinalServerImage
                                    {
                                        ImageIndex = pictureIndex,
                                        FileName = file
                                    };
                                    FinalServerImageFileList.Add(serverImage);
                                }
                                string contents = s.Rotate.ToString() + "," + file;
                                FormSendMessage(WM_USER_FINAL_IMAGE, cam.ID - 1, contents);
                            }
                        }

                        if (this.EQUIPMENT == "E204")
                        {
                            // change trigger flag
                            string category = "CAMERA" + cam.ID.ToString() + "TRIGGER";
                            int addrTrigger = Int32.Parse(Util.GetIniFileString(iniPLC, category, "Address", "100"));
                            char cDeviceType = Util.GetIniFileString(iniPLC, category, "DeviceType", "D")[0];
                            //cam.ShouldBeWriteTrigger = 3;
                            //Util.WriteLog("PLC Write: " + cam.ShouldBeWriteTrigger.ToString(), "Log", "Daq_" + cam.ID.ToString());
                            WritePLCData(cDeviceType, addrTrigger, 3);
                            PreviousTrigger[cam.ID - 1] = 3;
                            Thread.Sleep(100);
                        }

                        // 스텝별 판정 결과 저장
                        for (int j = 0; j < CurrentModel.StepList.Count; j++)
                        {
                            Step s = CurrentModel.StepList[j];
                            if (s.StepNo == cam.CurrentStep && s.CameraID == cam.ID)
                            {
                                s.StepResult = (JUDGE_RESULT)cam.TotalResult;
                                FormSendMessage(WM_USER_PAINT);
                            }
                        }
                        cam.IsFinished = true;
                    }
                }

            }
        }

        private int GetPictureIndex(Step s)
        {
            int idx = 0;
            for (int i = 0; i < CameraList.Count; i++)
            {
                CameraObject cam = CameraList[i];
                for (int j = 0; j < CurrentModel.StepList.Count; j++)
                {
                    Step st = CurrentModel.StepList[j];
                    if (st == s)
                        return idx;
                    idx++;
                }
            }
            return -1;
        }

        public class FinalServerImage : IComparable<FinalServerImage>
        {
            public int ImageIndex = -1;
            public string FileName = string.Empty;
            public int CompareTo(FinalServerImage other)
            {
                return this.ImageIndex.CompareTo(other.ImageIndex);
            }
        }

        public List<FinalServerImage> FinalServerImageFileList = new List<FinalServerImage>();
        private string MakeFinalResultImage(DateTime dt)
        {
            string fullpath = string.Empty;
            try
            {
                string dir = SaveFolder + "\\" + dt.ToString("yyyy") + "\\" +
                    dt.ToString("MM-dd");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                fullpath = dir + "\\" + dt.ToString("yyyyMMdd_HHmmss") + "_" + CurrentModel.ModelServerName + ".png";

                int countFileList = FinalServerImageFileList.Count;
                if (countFileList < 1)
                    return string.Empty;

                FinalServerImageFileList.Sort();
                // 전체 화면 캡쳐를 위해서 이미지를 하나 더 추가
                countFileList++;

                // 가로, 세로 사각형 개수를 구하기 위해 근사값을 사용합니다.
                int squaresPerColumn = (int)Math.Ceiling(Math.Sqrt(countFileList));
                int squaresPerRow = (int)Math.Ceiling((double)countFileList / squaresPerColumn);

                Bitmap bmpEach = Util.GetBitmapFromFile(FinalServerImageFileList[0].FileName);
                if (bmpEach == null)
                    return string.Empty;

                int size = Util.GetIniFileInt(iniSetup, "Setup", "ImageCompressionRatio", 100);
                int widthOne = (int)(bmpEach.Width * size * 1.0 / 100.0);
                int heightOne = (int)(bmpEach.Height * size * 1.0 / 100.0);

                // 이미지들 사이의 빈 공간
                int gapWidth = 10;
                int gapHeight = 10;

                // 최종 이미지 크기
                int TotalWidth = squaresPerColumn * (widthOne + gapWidth);
                int TotalHeight = squaresPerRow * (heightOne + gapHeight);

                // 새로 생성된 이미지
                Bitmap bmpFinal = new Bitmap(TotalWidth, TotalHeight);
                using (Graphics graphics = Graphics.FromImage(bmpFinal))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    int imageIndex = 0;
                    for (int row = 0; row < squaresPerRow; row++)
                    {
                        for (int col = 0; col < squaresPerColumn; col++)
                        {
                            if (imageIndex < countFileList)
                            {
                                Bitmap bmp = null;
                                // 인덱스가 0인건 전체 화면 캡쳐 이미지
                                if (imageIndex == 0)
                                {
                                    bmp = Util.CaptureScreen(Owner, string.Empty, out string err);
                                }
                                else
                                {
                                    string file = FinalServerImageFileList[imageIndex - 1].FileName;
                                    Util.WriteLog("Image Index: " + imageIndex.ToString() + " " + file, "Log", "FILE");
                                    bmp = Util.GetBitmapFromFile(file);
                                }

                                if (bmp != null)
                                {
                                    int x = col * (widthOne + gapWidth);
                                    int y = row * (heightOne + gapHeight);
                                    graphics.DrawImage(bmp, x, y, widthOne, heightOne);
                                    //graphics.DrawString(imageIndex.ToString(), new Font("Verdana", 200), new SolidBrush(Color.Red), new Point(x, y));
                                }
                            }
                            imageIndex++;
                        }
                    }
                    bmpFinal.Save(fullpath);
                    string userMessage = "Final Image Saved. " + fullpath;
                    FormSendMessage(WM_USER_MESSAGE_GENERAL, 1, userMessage);
                }
            }
            catch (Exception e)
            {
                string userMessage = "MakeFinalResultImage failed. " + e.Message;
                FormSendMessage(WM_USER_MESSAGE_GENERAL, 0, userMessage);
                return string.Empty;
            }
            return fullpath;
        }

        private void UpdateServerData(int res, string file)
        {
            SqlConnection conn = new SqlConnection(CONFIG.GetConnectionString(iniServer, out string ip));
            if (ip == string.Empty)
                return;
            string[] split = ip.Split(',');
            if (!Util.PingTest(split[0]))
            {
                FormSendMessage(WM_USER_STATUS_SERVER, 0, "");
                return;
            }

            int status = 2;
            if (res == 0)
            {
                status = 3;
                file = string.Empty;
            }

            string cmd = "UPDATE EQIP_IF SET STATUS='" + status.ToString() + "', IMG_FILE_NAME1='" + file + "' WHERE EQIP='" + EQUIPMENT + "'";
            SqlCommand qry = new SqlCommand(cmd, conn);

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                qry.ExecuteNonQuery();
                FormSendMessage(WM_USER_STATUS_SERVER, 1, "");
            }
            catch (Exception e)
            {
                FormSendMessage(WM_USER_STATUS_SERVER, 0, "UpdateServerData failed. " + e.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private void ProcessFinalResult(int totalResult)
        {
            int res = (totalResult == 1) ? 1 : 2;
            int addrResult = Int32.Parse(Util.GetIniFileString(iniPLC, "Result", "Address", "300"));
            char cDeviceType = Util.GetIniFileString(iniPLC, "Result", "DeviceType", "D")[0];
            WritePLCData(cDeviceType, addrResult, res);
            int addrTrigger = Int32.Parse(Util.GetIniFileString(iniPLC, "Trigger", "Address", "100"));
            WritePLCData(cDeviceType, addrTrigger, 3);

            // 화면에 이미지 출력하고 
            for (int i = 0; i < CameraList.Count; i++)
            {
                CameraObject cam = CameraList[i];
                for (int j = 0; j < CurrentModel.StepList.Count; j++)
                {
                    Step s = CurrentModel.StepList[j];
                    if (cam.ID == s.CameraID)
                    {
                        string file = FindFinalImageFile(cam);
                        string contents = s.Rotate.ToString() + "," + file;
                        FormSendMessage(WM_USER_FINAL_IMAGE, cam.ID - 1, contents);
                    }
                }
            }

            // 전체 툴에 대한 결과 이미지 PictureBox 컨트롤 생성
            for (int j = 0; j < CurrentModel.StepList.Count; j++)
            {
                Step s = CurrentModel.StepList[j];
                for (int i = 0; i < s.VisionToolList.Count; i++)
                {
                    VisionTool vt = s.VisionToolList[i];
                    string file = Util.GetWorkingDirectory() + "\\Images\\OK.png";
                    if (vt.Result == "NG")
                        file = Util.GetWorkingDirectory() + "\\Images\\NG.png";

                    int index = i + 1;
                    vt.picToolResult = new PictureBox();
                    vt.picToolResult.BackColor = Color.White;
                    vt.picToolResult.Name = "pic" + index.ToString();
                    vt.picToolResult.Size = new Size(50, 50);
                    vt.picToolResult.SizeMode = PictureBoxSizeMode.StretchImage;
                    vt.picToolResult.Tag = index.ToString();
                    vt.picToolResult.Visible = false;
                    vt.picToolResult.Image = Util.GetBitmapFromFile(file);
                    vt.picToolResult.Cursor = Cursors.Hand;
                    Util.SetControlEllipse(vt.picToolResult);
                }
            }

            FormSendMessage(WM_USER_DATETIME, 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            FormSendMessage(WM_USER_RESULT, totalResult, "");
        }

        private string FindFinalImageFile(CameraObject cam)
        {
            int retry = 0;
            while (retry < 10)
            {
                string folder = DataFolder + "\\CAM" + cam.ID.ToString();
                if (Directory.Exists(folder))
                {
                    string[] files = Directory.GetFiles(folder, cam.ResultFileName + "*", SearchOption.AllDirectories);
                    if (files.Length > 0)
                        return files[0];
                    else
                    {
                        Thread.Sleep(500);
                        retry++;
                        continue;
                    }
                }
                else
                    retry++;
            }
            return string.Empty;
        }

        private void ParseResultData(CameraObject cam, Step s, string data)
        {
            // 00026,NG,01,OK,0000100,02,NG,0000067,03,NG,0000003,04,OK,0000100
            string resultid = data.Substring(0, data.IndexOf(","));
            //lblResultID.Text = resultid;

            // NG,01,OK,0000100,02,NG,0000067,03,NG,0000003,04,OK,0000100
            data = data.Substring(data.IndexOf(",") + 1);
            string result = data.Substring(0, data.IndexOf(","));
            if (result == "OK")
                cam.TotalResult = 1;

            //lblTotalResult.Text = result;

            //listResult.Items.Clear();
            data = data.Substring(data.IndexOf(",") + 1);
            // 01,OK,0000100,       02,NG,0000067,      03,NG,0000003,      04,OK,0000100
            string[] resultDetail = data.Split(',');
            int count = resultDetail.Length / 3;
            for (int i = 0; i < count; i++)
            {
                if (i < s.VisionToolList.Count)
                {
                    VisionTool vt = s.VisionToolList[i];
                    vt.Number = resultDetail[i * 3 + 0];
                    vt.Result = resultDetail[i * 3 + 1];
                    vt.Value = resultDetail[i * 3 + 2];
                }
            }
        }

        internal void SendCommand(string cmd)
        {
            for (int i = 0; i < CameraList.Count; i++)
            {
                CameraObject cam = CameraList[i];
                cam.Send(cmd + "\r", out string msg);
            }
        }

        public Bitmap ImageBitmapOriginal = null;
        public Bitmap ImageDrawTemp = null;
        public bool showNG = false;
        internal void SetCameraProgram(int cameraNum, int progNo)
        {
            string msg = "CAM" + cameraNum.ToString() + " Set Program Number: " + progNo.ToString();
            FormSendMessage(WM_USER_MESSAGE_CAMERA, cameraNum, msg);
            //if (progNo > 0)
            {
                for (int i = 0; i < CameraList.Count; i++)
                {
                    CameraObject cam = CameraList[i];
                    if (cam.ID == cameraNum)
                        cam.Send("PW," + progNo.ToString() + "\r", out string err);
                }
            }
        }

        internal void DoTrigger(int camNum = 0)
        {
            // 파일 이름 지정
            string datetime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            for (int i = 0; i < CameraList.Count; i++)
            {
                CameraObject cam = CameraList[i];
                if (camNum == 0 || cam.ID == camNum)
                {
                    cam.ResultFileName = datetime;
                    int sameStepCount = 0;
                    int curStep = -1;
                    for (int j = 0; j < CurrentModel.StepList.Count; j++)
                    {
                        Step s = CurrentModel.StepList[j];
                        if (s.CameraID == cam.ID)
                        {
                            sameStepCount++;
                            curStep = s.StepNo;
                        }
                    }

                    if (sameStepCount == 1)
                    {
                        cam.CurrentStep = (ushort)curStep;
                        SetCameraProgram(cam.ID, curStep);
                    }
                    else
                    {
                        cam.CurrentStep = (ushort)CurrentStep[cam.ID - 1];
                        SetCameraProgram(cam.ID, CurrentStep[cam.ID - 1]);
                    }
                }
            }
        }

        public Pen penOK = new Pen(Color.Lime, 3);
        public Pen penNG = new Pen(Color.Red, 3);
        private void DrawStepProperty(Graphics g, Step st)
        {
            if (!st.ShowArea)
                return;

            g.DrawRectangle((st.StepResult == JUDGE_RESULT._OK) ? penOK : penNG, st.rcTarget);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            //g.DrawString(st.CameraID.ToString() + "-" + st.StepNo.ToString(), new Font("Verdana", 20),
            g.DrawString("CAM" + st.CameraID.ToString(), new Font("Verdana", 20),
                new SolidBrush((st.StepResult == JUDGE_RESULT._OK) ? Color.Lime : Color.Red), st.rcTarget, stringFormat);

        }

        private void DrawStepTools(Graphics g, Step s, PictureBox picTarget)
        {
            if (s.ShowTools < 1)
                return;

            for (int i = 0; i < s.VisionToolList.Count; i++)
            {
                VisionTool vt = s.VisionToolList[i];
                if (vt.picToolResult == null)
                    continue;

                Util.GetOriginalCoordinateFromPictureBoxStretched(picTarget, ImageBitmapOriginal, vt.PositionX, vt.PositionY, out int xout, out int yout);
                vt.picToolResult.Location = new Point(xout, yout);
                vt.picToolResult.Parent = picTarget;
                // 모두 숨기기
                if (s.ShowTools == 0)
                    vt.picToolResult.Visible = false;
                // 모두 보이기
                else if (s.ShowTools == 1)
                    vt.picToolResult.Visible = true;
                // NG만 보이기
                else if (s.ShowTools == 2 && vt.Result == "NG")
                    vt.picToolResult.Visible = true;
            }
        }

        internal void ModelChanged(string model)
        {
            // 이전 판정 결과가 있으면 툴 결과들 삭제
            if (CurrentModel != null)
            {
                for (int j = 0; j < CurrentModel.StepList.Count; j++)
                {
                    Step s = CurrentModel.StepList[j];
                    for (int i = 0; i < s.VisionToolList.Count; i++)
                    {
                        VisionTool vt = s.VisionToolList[i];
                        if (vt.picToolResult != null)
                        {
                            vt.picToolResult.Visible = false;
                            vt.picToolResult.Dispose();
                        }
                    }
                }
            }

            FormSendMessage(WM_USER_TEST_INIT);
            FormSendMessage(WM_USER_FLICKER, 0);

            ModelList = Model.GetModelList(iniModel, CameraList);

            CurrentModel = ModelList.Find(x => x.ModelServerName.ToUpper() == model.ToUpper());
            if (CurrentModel != null)
            {
                if (!File.Exists(CurrentModel.ModelImage))
                    CurrentModel.ModelImage = Util.GetWorkingDirectory() + "\\Images\\" + Path.GetFileName(CurrentModel.ModelImage);
                FormSendMessage(WM_USER_TEST_MODELNAME, 0, CurrentModel.ModelServerName);

                //picModel.Image = Util.GetBitmapFromFile(CurrentModel.ModelImage);
                ImageBitmapOriginal = Util.GetBitmapFromFile(CurrentModel.ModelImage);
                if (ImageBitmapOriginal != null)
                    ImageDrawTemp = new Bitmap(ImageBitmapOriginal.Width, ImageBitmapOriginal.Height);
                // 스텝 판정 결과 초기화
                foreach (Step s in CurrentModel.StepList)
                    s.StepResult = JUDGE_RESULT._NONE;

                FinalServerImageFileList.Clear();
                for (int k = 0; k < CameraList.Count; k++)
                {
                    CameraObject cam = CameraList[k];
                    cam.Init();
                    // PC의 날짜/시간과 카메라의 날짜/시간을 동일하게 설정
                    DateTime now = DateTime.Now;
                    string contents = "TC," +
                        now.ToString("yy") + "," +
                        now.ToString("MM") + "," +
                        now.ToString("dd") + "," +
                        now.ToString("HH") + "," +
                        now.ToString("mm") + "," +
                        now.ToString("ss") + "," +
                        "\r";
                    cam.Send(contents, out string errMsg);

                    for (int j = 0; j < CurrentModel.StepList.Count; j++)
                    {
                        Step s = CurrentModel.StepList[j];
                        if (s.CameraID == cam.ID)
                        {
                            for (int i = 0; i < s.VisionToolList.Count; i++)
                            {
                                VisionTool vt = s.VisionToolList[i];
                                if (vt.picToolResult != null)
                                    vt.picToolResult.Dispose();
                            }
                        }
                    }
                }

                IsNewData = 1;
                FormSendMessage(WM_USER_PAINT);
            }
        }

        internal void TimerFlicker()
        {
            if (CurrentModel == null)
                return;

            showNG = !showNG;
            for (int i = 0; i < CurrentModel.StepList.Count; i++)
            {
                Step s = CurrentModel.StepList[i];
                if (s.StepResult == JUDGE_RESULT._NG)
                    s.ResultVisible = showNG;
                else
                    s.ResultVisible = true;
            }
        }

        internal void Paint(PaintEventArgs e, PictureBox picOwner)
        {
            if (CurrentModel == null)
                return;

            try
            {
                if (ImageDrawTemp != null)
                {
                    Graphics g = Graphics.FromImage(ImageDrawTemp);
                    g.DrawImage(ImageBitmapOriginal, 0, 0);
                    // 스텝 항목들 표시
                    for (int i = 0; i < CurrentModel.StepList.Count; i++)
                    {
                        Step s = CurrentModel.StepList[i];
                        if (s.StepResult != JUDGE_RESULT._NONE && s.ResultVisible == true)
                        {
                            DrawStepProperty(g, s);
                            DrawStepTools(g, s, picOwner);
                        }
                    }
                    e.Graphics.DrawImage(ImageDrawTemp,
                        new Rectangle(0, 0, picOwner.Width, picOwner.Height),
                        new Rectangle(0, 0, ImageDrawTemp.Width, ImageDrawTemp.Height),
                        GraphicsUnit.Pixel);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}