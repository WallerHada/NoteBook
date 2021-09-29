
using System;
using System.IO;
using System.Text;

namespace NoteBook.LogNote;
public class LogHelper
{
    private static readonly string lockKey = "lock";
    public static void SaveLog(Exception ex, string remark)
    {
        StringBuilder sb = new();
        if (!string.IsNullOrEmpty(remark)) sb.AppendLine(remark);
        sb.AppendLine(ex.Message);
        sb.AppendLine(ex.StackTrace);
        sb.AppendLine(ex.Source);
        SaveLog("错误日志", sb.ToString());
    }

    public static void SaveLog(String logName, string msg, params string[] para)
    {
        try
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\");

            }

            DateTime now = DateTime.Now;
            string LogFile = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + logName + "_" + now.ToString("yyyy-MM-dd") + ".log";
            lock (lockKey)
            {
                using (FileStream fs = new(LogFile, FileMode.Append, FileAccess.Write))
                {
                    byte[] datetimefile = Encoding.Default.GetBytes(logName + "_" + now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ":\r\n");
                    fs.Write(datetimefile, 0, datetimefile.Length);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        byte[] data = Encoding.Default.GetBytes((para.Length > 0 ? string.Format(msg, para) : msg) + "\r\n==========================================\r\n");
                        fs.Write(data, 0, data.Length);
                    }
                    fs.Flush();
                }
            }
        }
        catch
        { }
    }
}
