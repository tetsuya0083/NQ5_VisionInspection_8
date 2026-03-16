using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class Constants
    {
        public static int RETRY_COUNT = 10;

        public static int ChannelCount = 8;

        /*
        // 주파수 범위
        _FreqSpanList[0]  := 25600;
        _FreqSpanList[1]  := 12800;
        _FreqSpanList[2]  := 6400;
        _FreqSpanList[3]  := 3200;
        _FreqSpanList[4]  := 1600;
        _FreqSpanList[5]  := 800;
        _FreqSpanList[6]  := 400;
        _FreqSpanList[7]  := 200;
        _FreqSpanList[8]  := 100;
        _FreqSpanList[9]  := 50;
        _FreqSpanList[10] := 25;
        _FreqSpanList[11] := 12.5;
         */
        public const float MIN_FREQUENCY = 12.5f;
        public const float BASE_FREQUENCY = 3200f;
        public const float MAX_FREQUENCY = 25600f;
        public static float getFreqSpanFromIndex(int index)
        {
            return (float)(MAX_FREQUENCY / Math.Pow(2, index));
        }
        public static int getFreqSpanIndex(float freqSpan)
        {
            return (int)Math.Log((MAX_FREQUENCY / freqSpan), 2);
        }

        /*
        // 라인수
        _LineList[0] := 400;
        _LineList[1] := 800;
        _LineList[2] := 1600;
        _LineList[3] := 3200;
        _LineList[4] := 6400;
        _LineList[5] := 12800;
        _LineList[6] := 25600;
        _LineList[7] := 51200;
        _LineList[8] := 102400;
         */
        public const int MIN_LINE = 400;
        public static int getLineFromIndex(int index)
        {
            return (int)(MIN_LINE * Math.Pow(2, index));
        }
        public static int getLineIndex(int line)
        {
            return (int)Math.Log(line / MIN_LINE, 2);
        }

        // Max Frequency 또는 Bandpass High를 주면 그에 맞는 최대주파수 범위를 리턴
        public static float calcFreqSpan(float freq)
        {
            // 실제 주파수
            return (float)(Math.Ceiling(freq / BASE_FREQUENCY) * BASE_FREQUENCY);
        }
        public static int calcFreqSpanIndex(float freq)
        {
            // 주파수의 인덱스
            return getFreqSpanIndex(calcFreqSpan(freq));
        }
        // 주파수 범위와 측정 시간을 주면 그에 맞는 라인수를 리턴
        public static int calLine(float freq, double time/*ms*/)
        {
            // 실제 라인수
            return (int)(freq * Math.Pow(2, Math.Ceiling(Math.Log(Math.Ceiling(time / 1000), 2))));
        }
        public static int calLineIndex(float freq, double time/*ms*/)
        {
            // 라인수의 인덱스
            return getLineIndex(calLine(freq, time));
        }

        /*
        /////////////////////////////////////////////////////////
        // sensor status check
        public static ModuleErrorType getSensorStatus(float val)
        {
            int temp = getGapValue(val);
            // 6볼트보다 작으면 쇼트
            if (temp < 6)
                return ModuleErrorType.tShort;
            // 14볼트보다 크면 오픈
            if (temp > 14)
                return ModuleErrorType.tOpen;

            return ModuleErrorType.tNormal;
        }
        public static int getGapValue(float val)
        {
            return (int)(val * 24.72692 / 512);
        }
        //////////////////////////////////////////////////////////
         */

        public enum SpectrumType { tdRmsSpectrum, tdPowerSpectrum, tdMagSpectrum }
        public enum TimeWindowType { twHanning, twRect, twFlatTop, twKaiservessel }

    }

    public class Define
    {
        // Database.ini 
        public const string INI_DATABASE = "Database.ini"; // 데이터베이스 연결 정보를 가지고 있는 ini 파일    
        public const string INI_MES = "MES.ini"; // MES 연결 정보를 가지고 있는 ini 파일    
        public const string STR_CATALOG	=       "Catalog";      //	Database.ini 파일에서 연결할 셋업 데이터베이스 이름
        public const string STR_CONNECTION =	"Connection";   //	Database.ini 파일의 App값
        public const string STR_DATACATALOG =   "DataCatalog";  //	Database.ini 파일에서 연결할 데이터 데이터베이스 이름
        public const string STR_RAWDATACATALOG = "RawDataCatalog";  //	Database.ini 파일에서 연결할 데이터 Raw 데이터베이스 이름
        public const string STR_ID = "ID";           //	Database.ini 파일에서 서버에 연결할 ID = sa
        public const string STR_IP =	        "IP";           //	Database.ini 파일에서 연결할 서버 아이피 주소
        public const string STR_PASSWORD =      "Password";     //	Database.ini 파일에서 서버에 연결할 패스워드 = nada

        // Config.ini
        public const string INI_CONFIG = "Setup.ini";
        public const string STR_SEARCH = "SEARCH";
        public const string STR_FROM = "FROM";
        public const string STR_TO = "TO";
        public const string STR_ENABLED = "ENABLED";
        public const string STR_BASIC = "Basic";
        public const string STR_WORKLINENO = "WorkLineNo";
        public const string STR_SAVETERM = "SaveTerm";

        // DAQ
        public const string INI_DAQCONFIG = "DaqConfig.ini"; // 채널 정보 저장(감도, ICP, DC, OFFSET)
        public const string STR_SENSITIVITY = "Sensitivity";
        public const string STR_ICP = "ICP";
        public const string STR_DC = "DC";
        public const string STR_OFFSET = "OFFSET";
        public const string STR_CH = "CH";

        // IP Address
        public const string STR_LOOPBACK_IP =   "127.0.0.1";    // 루프백 주소  

        // Event Log
        public const string STR_LOG = "Log";
        public const string STR_LOG_ERROR = "LogErr";
        public const string STR_DAQ = "DAQ";
        public const string STR_IQIS = "IQIS";
        public const string STR_PROCESS = "PROCESS";
        public const string STR_DB = "DB";
        public const string STR_LOG_ANAL = "LogAnalysis";
        public const string STR_ANAL = "Analysis";
        public const string STR_MES = "MES";
        public const string STR_PLC = "PLC";

        // 사용자 메시지
        public static string MSG_CONNECTION_SUCCEEDED = "데이터베이스 연결 성공";
        public static string MSG_MODULE = "모듈 설정";
        public static string MSG_MODULE_DELETE = "선택한 모듈을 삭제하시겠습니까?";

        // 에러 메시지
        public static string ERROR_MODULE_LIST = "모듈 정보 불러오기 실패. ";
        public static string ERROR_MODULE_INSERT = "모듈 정보 삽입 실패. ";

        // 저주파/고주파
        public const int LOW_FREQ = 0;
        public const int HIGH_FREQ = 1;

        // 메시지
        public const string STR_QUERY_CLOSE = "프로그램을 종료하시겠습니까?";
        public const string STR_MSG_ANALYSIS = "분석 프로그램";

        public const int RESULT_NA = 0;
        public const int RESULT_OK = 1;
        public const int RESULT_NG = 2;

        public enum HingeHolePosition { TopUpper, TopLower, BottomUpper, BottomLower }


        internal static string GetResultString(int res)
        {
            string result = "NONE";
            switch (res)
            {
                case RESULT_NG: result = "NG"; break;
                case RESULT_OK: result = "OK"; break;
                case RESULT_NA: result = "NA"; break;
                default: break;
            }
            return result;
        }
    }
}
