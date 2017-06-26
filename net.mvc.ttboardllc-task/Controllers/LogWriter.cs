using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

public enum LogWriterType { FileLog, EventLog };

class LogWriter
{
    /// <summary>
    /// Путь до лог-файла
    /// </summary>
    private string LogFilePath;
    /// <summary>
    /// Имя журнала, в который будет записываться лог
    /// </summary>
    private string logSource;

    /// <summary>
    /// Тип лога: запись в журнал Windows Application, запись в файл
    /// </summary>
    private LogWriterType logType;

    private const string dualLogSource = "Application";
    private const EventLogEntryType dualLogType = EventLogEntryType.Error;

    /// <summary>
    /// Конструктор класса - по умолчанию записывает данные в файл на ЖД
    /// </summary>
    /// <param name="path">Путь до лога</param>
    public LogWriter(string path)
    {
        LogFilePath = path;
        logType = LogWriterType.FileLog;
    }

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="path">Путь до лога</param>
    /// <param name="type">Тип пути до лога(FileLog - путь до файла на ЖД, EventLog - Имя папки в каталоге Приложения(Application) в системном журнале Windows)</param>
    public LogWriter(string path, LogWriterType type)
    {
        if (type == LogWriterType.EventLog)
            logSource = path;
        else if (type == LogWriterType.FileLog)
            LogFilePath = path;
        logType = type;
    }

    /// <summary>
    /// Записывает событие в лог
    /// </summary>
    /// <param name="Msg">Сообщение о событии</param>
    /// <returns>При корректном выполнении возвращает true, иначе false</returns>
    public void Write(string Msg)
    {
        if (logType == LogWriterType.FileLog)
            WriteMsgToTxt(Msg);
        else if (logType == LogWriterType.EventLog)
            WriteMsgToEvent(Msg, EventLogEntryType.Error);
    }

    /// <summary>
    /// Записывает событий в журнал Windows
    /// </summary>
    /// <param name="Msg">Сообщение для записи в лог</param>
    /// <param name="eventType">Тип сообщения(Ошибка, предупреждение, информация)</param>
    /// <returns>Возвращает true - если операция завершилась корректно, иначе false</returns>
    private void WriteMsgToEvent(string Msg, EventLogEntryType eventType)
    {
        EventLog.WriteEntry(logSource, GetFormatedLog(Msg), eventType);
    }

    /// <summary>
    /// Запись лога в текстовый файл
    /// </summary>
    /// <param name="Msg">Сообщение для записи в лог</param>
    /// <returns>Возвращает true - если операция завершилась корректно, иначе false</returns>
    private void WriteMsgToTxt(string Msg)
    {
        File.AppendAllText(LogFilePath, GetFormatedLog(Msg), Encoding.UTF8);
    }

    /// <summary>
    /// Создание строки лога с датой и временем
    /// </summary>
    /// <param name="Msg">Сообщение лога</param>
    /// <returns>Возвращает строку с сформированной ошибкой</returns>
    private static string GetFormatedLog(string Msg)
    {
        string FormatedMsg = "Дата: " + DateTime.Now.ToString() + Environment.NewLine +
                                "Ошибка: " + Msg + Environment.NewLine;
        return FormatedMsg;
    }

    public static void WriteDualLog(string Msg)
    {
        //EventLog.WriteEntry(dualLogSource, GetFormatedLog(Msg), dualLogType);
    }
}
