using System;
using System.IO;

public class Logger
{
	private string logDirectory;

	public Logger(string logDirectory)
	{
		this.logDirectory = logDirectory;
	}

	public void Log(string message)
	{
		try
		{
            CheckAndDeleteOldLogs();
            // Get the current date
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

			// Construct the full path to the log file
			string logFilePath = Path.Combine(logDirectory, $"log_{currentDate}.txt");

			if (!File.Exists(logFilePath))
			{
				Directory.CreateDirectory(logDirectory);
				File.Create(logFilePath).Close();
			}

			// Get the current time
			string timestamp = DateTime.Now.ToString("HH:mm:ss");

			// Construct the log entry
			string logEntry = $"{timestamp} - {message}";

			// Write the log entry to the log file
			File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
		}
		catch (IOException e)
		{
			Console.WriteLine("An error occurred while writing to the log file:");
			Console.WriteLine(e.Message);
		}
	}

	// create function to check total files in directory logs and delete if more than 10 with based on date (oldest file will delete)
	public void CheckAndDeleteOldLogs()
	{
        try
		{
			// Get all files in the log directory
			string[] logFiles = Directory.GetFiles(logDirectory);

            // If the number of log files exceeds 10, delete the oldest file
            if (logFiles.Length > 5)
			{
                // Sort the log files by creation time
                Array.Sort(logFiles, (a, b) => File.GetCreationTime(a).CompareTo(File.GetCreationTime(b)));

				// Delete the oldest log file
				File.Delete(logFiles[0]);
            }
        }
        catch (IOException e)
		{
            Console.WriteLine("An error occurred while checking and deleting old log files:");
            Console.WriteLine(e.Message);
        }
    }
}