using System.Text;
using System;
using System.IO;


public class Debugger
{
    private static string filePath = @"C:\Users\mcuko\Desktop\LOG\log.txt";

    public static void Log(StringBuilder stringBuilder)
    {
        File.AppendAllText(filePath, stringBuilder.ToString());
    }

    public static void Log(string message)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(message + Environment.NewLine);
        File.AppendAllText(filePath, stringBuilder.ToString());
        stringBuilder.Clear();
    }
}
