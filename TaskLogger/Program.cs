using Microsoft.Data.Sqlite;

namespace CrudWithSQLite
{
    class Program
    {
        static string connectionString = "Data Source=tasks.db;";

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
                Console.WriteLine("5. Exit");
                Console.Write("Enter choice: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddTask(); break;
                    case "2": ViewTasks(); break;
                    case "3": UpdateTask(); break;
                    case "4": DeleteTask(); break;
                    case "5": return; // Exit program
                    default: Console.WriteLine("Invalid choice!"); break;
                }
            }
        }

        // Create Database and Table if not exists
        static void CreateDatabaseAndTable()
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string tableCmd = @"CREATE TABLE IF NOT EXISTS TaskLog (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Description TEXT NOT NULL,
                                        CreatedAt TEXT NOT NULL
                                    );";
                using (var cmd = new SqliteCommand(tableCmd, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // CREATE
        static void AddTask()
        {
            Console.Write("Enter task description: ");
            string description = Console.ReadLine();
            string createdAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string insertCmd = "INSERT INTO TaskLog (Description, CreatedAt) VALUES (@desc, @createdAt)";
                using (var cmd = new SqliteCommand(insertCmd, conn))
                {
                    cmd.Parameters.AddWithValue("@desc", description);
                    cmd.Parameters.AddWithValue("@createdAt", createdAt);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Task added successfully!");
        }

        // READ
        static void ViewTasks()
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string selectCmd = "SELECT * FROM TaskLog";
                using (var cmd = new SqliteCommand(selectCmd, conn))
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
                        Console.WriteLine($"{reader["Id"]}. {reader["Description"]} (Created At: {reader["CreatedAt"]})");
                    }
                }
            }
        }

        // UPDATE
        static void UpdateTask()
        {
            ViewTasks();
            Console.Write("Enter task ID to update: ");
            int id = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter new task description: ");
            string newDesc = Console.ReadLine();

            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string updateCmd = "UPDATE TaskLog SET Description = @desc WHERE Id = @id";
                using (var cmd = new SqliteCommand(updateCmd, conn))
                {
                    cmd.Parameters.AddWithValue("@desc", newDesc);
                    cmd.Parameters.AddWithValue("@id", id);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                        Console.WriteLine("Task updated successfully!");
                    else
                        Console.WriteLine("Invalid ID. No task updated.");
                }
            }
        }

        // DELETE
        static void DeleteTask()
        {
            ViewTasks();
            Console.Write("Enter task ID to delete: ");
            int id = Convert.ToInt32(Console.ReadLine());

            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string deleteCmd = "DELETE FROM TaskLog WHERE Id = @id";
                using (var cmd = new SqliteCommand(deleteCmd, conn))
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
    }
}
