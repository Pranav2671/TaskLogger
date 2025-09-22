using System;
using System.Data.SQLite;

namespace CrudWithSQLite
{
    class Program
    {
        const string DatabaseName = "TaskLog.db";
        static readonly string connectionString = $"Data Source={DatabaseName};";

        static void Main(string[] args)
        {
            CreateDatabaseAndTable();

            while (true)
            {
                Console.WriteLine("\n--- Task Logger (SQLite) ---");
                Console.WriteLine("1. Add Task");
                Console.WriteLine("2. View Tasks");
                Console.WriteLine("3. Update Task");
                Console.WriteLine("4. Delete Task");
                Console.WriteLine("5. Search Task by Date");
                Console.WriteLine("6. Exit");
                Console.Write("Enter choice: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddTask(); break;
                    case "2": ViewTasks(); break;
                    case "3": UpdateTask(); break;
                    case "4": DeleteTask(); break;
                    case "5": SearchTaskByDate(); break;
                    case "6": return; // Exit
                    default: Console.WriteLine("Invalid choice!"); break;
                }
            }
        }

        static void CreateDatabaseAndTable()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string tableCmd = @"CREATE TABLE IF NOT EXISTS TaskLog (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Description TEXT NOT NULL,
                                        DateDone TEXT NOT NULL,
                                        HoursTaken REAL NOT NULL
                                    );";
                using (var cmd = new SQLiteCommand(tableCmd, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        static void AddTask()
        {
            Console.Write("Enter task description: ");
            string description = Console.ReadLine();

            Console.Write("Enter date done (dd/MM/yyyy): ");
            string dateDone = Console.ReadLine();

            Console.Write("Enter hours taken: ");
            double hoursTaken = Convert.ToDouble(Console.ReadLine());

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string insertCmd = "INSERT INTO TaskLog (Description, DateDone, HoursTaken) VALUES (@desc, @dateDone, @hoursTaken)";
                using (var cmd = new SQLiteCommand(insertCmd, conn))
                {
                    cmd.Parameters.AddWithValue("@desc", description);
                    cmd.Parameters.AddWithValue("@dateDone", dateDone);
                    cmd.Parameters.AddWithValue("@hoursTaken", hoursTaken);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Task added successfully!");
        }

        static void ViewTasks()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string selectCmd = "SELECT * FROM TaskLog";
                using (var cmd = new SQLiteCommand(selectCmd, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n--- Task List ---");
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No tasks found.");
                        return;
                    }

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Id"]}. {reader["Description"]} (Date Done: {reader["DateDone"]}, Hours Taken: {reader["HoursTaken"]})");
                    }
                }
            }
        }

        static void UpdateTask()
        {
            ViewTasks();
            Console.Write("Enter task ID to update: ");
            int id = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter new task description: ");
            string newDesc = Console.ReadLine();

            Console.Write("Enter new date done (dd/MM/yyyy): ");
            string newDateDone = Console.ReadLine();

            Console.Write("Enter new hours taken: ");
            double newHoursTaken = Convert.ToDouble(Console.ReadLine());

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string updateCmd = "UPDATE TaskLog SET Description = @desc, DateDone = @dateDone, HoursTaken = @hoursTaken WHERE Id = @id";
                using (var cmd = new SQLiteCommand(updateCmd, conn))
                {
                    cmd.Parameters.AddWithValue("@desc", newDesc);
                    cmd.Parameters.AddWithValue("@dateDone", newDateDone);
                    cmd.Parameters.AddWithValue("@hoursTaken", newHoursTaken);
                    cmd.Parameters.AddWithValue("@id", id);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                        Console.WriteLine("Task updated successfully!");
                    else
                        Console.WriteLine("Invalid ID. No task updated.");
                }
            }
        }

        static void DeleteTask()
        {
            ViewTasks();
            Console.Write("Enter task ID to delete: ");
            int id = Convert.ToInt32(Console.ReadLine());

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string deleteCmd = "DELETE FROM TaskLog WHERE Id = @id";
                using (var cmd = new SQLiteCommand(deleteCmd, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                        Console.WriteLine("Task deleted successfully!");
                    else
                        Console.WriteLine("Invalid ID. No task deleted.");
                }
            }
        }

        static void SearchTaskByDate()
        {
            Console.Write("Enter date to search (dd/MM/yyyy): ");
            string searchDate = Console.ReadLine();

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string searchCmd = "SELECT * FROM TaskLog WHERE DateDone = @dateDone";
                using (var cmd = new SQLiteCommand(searchCmd, conn))
                {
                    cmd.Parameters.AddWithValue("@dateDone", searchDate);
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine($"\n--- Tasks done on {searchDate} ---");
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No tasks found for this date.");
                            return;
                        }

                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["Id"]}. {reader["Description"]} (Hours Taken: {reader["HoursTaken"]})");
                        }
                    }
                }
            }
        }
    }
}
