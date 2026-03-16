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
}
