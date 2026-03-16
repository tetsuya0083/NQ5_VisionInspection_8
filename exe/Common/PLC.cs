using Common;
using Sharp7;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GLOBAL_PLC
{
    public class PLC
    {
        private S7Client Client;

        public string IP = "192.168.0.1";
        public int Rack = 0;
        public int Slot = 2;
        public int DB = 201;
        public bool connected = false;

        public PLC()
        {
            Client = new S7Client();
        }

        internal void Destroy()
        {
            Disconnect();
        }

        public string Connect()
        {
            connected = false;
            int Result = Client.ConnectTo(IP, Rack, Slot);
            if (Result == 0)
            {
                connected = true;
                return string.Empty;
            }

            string ret = Client.ErrorText(Result) + " (" + Client.ExecTime().ToString() + " ms)";
            return ret;
        }

        public string Disconnect()
        {
            string ret = string.Empty;
            Client.Disconnect();
            connected = false;
            return ret;
        }

        internal bool Read(int dataType, int startIdx, int size, byte[] buffer)
        {
            bool res = false;
            try
            {
                int Result = Client.DBRead(this.DB, startIdx, size, buffer);
                string ret = Client.ErrorText(Result) + " (" + Client.ExecTime().ToString() + " ms)";
                if (Result == 0)
                {
                    HexDump(buffer, size);
                    res = true;
                }
                else
                    Client.Disconnect();
            }
            catch (Exception e)
            {
                Client.Disconnect();
                Util.WriteLog("PLC Read() Error. " + e.ToString(), "LogErr");
                res = false;
            }
            return res;
        }

        public int Write(int startIdx, int size, byte[] buffer)
        {
            return Client.DBWrite(this.DB, startIdx, size, buffer);
        }

        public string HexDump(byte[] bytes, int Size)
        {
            string res = string.Empty;
            if (bytes == null)
                return res;

            try
            {
                int bytesLength = Size;
                int bytesPerLine = 16;

                char[] HexChars = "0123456789ABCDEF".ToCharArray();

                int firstHexColumn =
                      8                   // 8 characters for the address
                    + 3;                  // 3 spaces

                int firstCharColumn = firstHexColumn
                    + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                    + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                    + 2;                  // 2 spaces 

                int lineLength = firstCharColumn
                    + bytesPerLine           // - characters to show the ascii value
                    + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

                char[] line = (new String(' ', lineLength - 2) + Environment.NewLine).ToCharArray();
                int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
                StringBuilder result = new StringBuilder(expectedLines * lineLength);

                for (int i = 0; i < bytesLength; i += bytesPerLine)
                {
                    line[0] = HexChars[(i >> 28) & 0xF];
                    line[1] = HexChars[(i >> 24) & 0xF];
                    line[2] = HexChars[(i >> 20) & 0xF];
                    line[3] = HexChars[(i >> 16) & 0xF];
                    line[4] = HexChars[(i >> 12) & 0xF];
                    line[5] = HexChars[(i >> 8) & 0xF];
                    line[6] = HexChars[(i >> 4) & 0xF];
                    line[7] = HexChars[(i >> 0) & 0xF];

                    int hexColumn = firstHexColumn;
                    int charColumn = firstCharColumn;

                    for (int j = 0; j < bytesPerLine; j++)
                    {
                        if (j > 0 && (j & 7) == 0) hexColumn++;
                        if (i + j >= bytesLength)
                        {
                            line[hexColumn] = ' ';
                            line[hexColumn + 1] = ' ';
                            line[charColumn] = ' ';
                        }
                        else
                        {
                            byte b = bytes[i + j];
                            line[hexColumn] = HexChars[(b >> 4) & 0xF];
                            line[hexColumn + 1] = HexChars[b & 0xF];
                            line[charColumn] = (b < 32 ? '·' : (char)b);
                        }
                        hexColumn += 3;
                        charColumn++;
                    }
                    result.Append(line);
                }
                res = result.ToString();
            }
            catch (Exception e)
            {
                Util.WriteLog("HexDump() Error. " + e.ToString(), "LogErr");
            }
            return res;
        }

    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct _LS_FENET_HRADER
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] CompanyID;
        public ushort Reserved;
        public ushort PLC_Info;
        public byte CPU_Info;
        public byte Source_of_Frame;
        public ushort InVoke_ID;
        public ushort Length;
        public byte FEnet_Position;
        public byte Reserved_BCC; // 바이트 배열로 리턴
        public char[] Copy(int length, params char[] cDataFrame)
        {
            CompanyID = new char[] { 'L', 'S', 'I', 'S', '-', 'X', 'G', 'T' };
            Reserved = 0x0; //0x00 : 예약영역
            PLC_Info = 0x00;//클라이언트(MMI) → 서버(PLC) : Don’ care (0x00CPU_Info = 0xA0; //XGK: 0xA0
            Source_of_Frame = 0x33; //클라이언트(MMI) → 서버(PLC) : 0x33
            InVoke_ID = 0x01; //레임간의 순서를 구별하기 위한 ID
            Length = (ushort)length; //Application Instruction의 바이트 크기 // CPU 이므로 0 //(Bit0~3 : FEnet I/F 모듈의 슬롯(Slot) 번호 // Bit4~7 : FEnet I/F 모듈의 베이스(Base) 번호
            FEnet_Position = 0x00; //0x00 : 예약영역 (Application Header의 Byte Sum)
            Reserved_BCC = 0x00;
            byte[] byHeader = new byte[Marshal.SizeOf(this)];
            unsafe
            {
                fixed (byte* fixed_buffer = byHeader)
                {
                    Marshal.StructureToPtr(this, (IntPtr)fixed_buffer, false);
                }
            } // 배열 변환
            char[] cHeader = Array.ConvertAll(byHeader, x => (char)x); // 체크썸
            cHeader[cHeader.Length - 1] = (char)BCC_Check(cHeader, 0, cHeader.Length - 2); // 배열 합치기
            char[] reArray = new char[cHeader.Length + length];
            Array.Copy(cHeader, reArray, cHeader.Length);
            Array.Copy(cDataFrame, 0, reArray, cHeader.Length, length);
            return reArray;
        } // BCC 체크

        int BCC_Check(char[] buff, int iStart, int iEnd)
        {
            int CheckSum = 0; for (int i = iStart; i < iEnd; i++)
            {
                CheckSum = CheckSum + buff[i];
                if (CheckSum > 255)
                {
                    CheckSum = CheckSum - 256;
                }
            }
            return CheckSum;
        }
    };


    public class LS_PLC
    {
        void RegisterRead(string szDevice)
        {
            if (string.IsNullOrEmpty(szDevice))
            {
                return;
            }

            int nCnt = 0;
            char[] bufBody = new char[255];

            Array.Clear(bufBody, 0, bufBody.Length); //명령어(2) //[ h5400 읽기][ h5800 쓰기 ]
            bufBody[nCnt++] = (char)0x54;
            bufBody[nCnt++] = (char)0x00; //데이터 타입(2) //[ h00 비트 ][ h01 바이트 ][ h02 워드 ][ h03 더블워드 ][ h04 롱워드][ h14 연속]
            bufBody[nCnt++] = (char)0x14;
            bufBody[nCnt++] = (char)0x00; //예약 영역(2) : 0x0000 : Don’t Care.
            bufBody[nCnt++] = (char)0x00;
            bufBody[nCnt++] = (char)0x00; //블록수(2) : 읽고자 하는 블록의 개수. 0x0001
            bufBody[nCnt++] = (char)0x01;
            bufBody[nCnt++] = (char)0x00; //변수명 길이(2) : 최대 16자.
            bufBody[nCnt++] = (char)szDevice.Length;
            bufBody[nCnt++] = (char)0x00; //데이터 주소 최대 16자

            char[] ch = szDevice.ToCharArray();
            for (int i = 0; i < ch.Length; i++)
            {
                bufBody[nCnt++] = ch[i];
            } //데이터 개수(2) 최대 1400byte

            int nLength = 256 * 2;
            bufBody[nCnt++] = (char)(nLength & 0xFF); // Data length(L)
            bufBody[nCnt++] = (char)(nLength >> 8); // Data length(H) // 앞에서 만든 명령 프레임과 헤더프레임 더하기

            _LS_FENET_HRADER ls_Header = new _LS_FENET_HRADER();
            char[] reData = ls_Header.Copy(nCnt, bufBody);

            //** reData를 이더넷 통신으로 전송 ** 

        }
    }
}
