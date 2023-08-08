using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

class Task
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
}

class Program
{
    private static List<Task> tasks = new List<Task>();
    private const string filename = "tasks.json";

    static void Main()
    {
        LoadTasks();

        while (true)
        {
            Console.WriteLine("---- Task Manager ----");
            Console.WriteLine("1. Add a Task");
            Console.WriteLine("2. View Tasks");
            Console.WriteLine("3. Mark Completed");
            Console.WriteLine("4. Delete Task");
            Console.WriteLine("5. Exit");

            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    ViewTasks();
                    break;
                case "3":
                    MarkCompleted();
                    break;
                case "4":
                    DeleteTask();
                    break;
                case "5":
                    SaveTasks();
                    Console.WriteLine("Exiting Task Manager...");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    static void AddTask()
    {
        Console.WriteLine("---- Add a Task ----");
        Console.Write("Enter Task Name: ");
        string name = Console.ReadLine();
        Console.Write("Enter Description: ");
        string description = Console.ReadLine();
        Console.Write("Enter Due Date (yyyy-mm-dd): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime dueDate))
        {
            tasks.Add(new Task { Name = name, Description = description, DueDate = dueDate });
            Console.WriteLine("Task added successfully!");
        }
        else
        {
            Console.WriteLine("Invalid date format. Task not added.");
        }
    }

    static void ViewTasks()
    {
        Console.WriteLine("---- View Tasks ----");
        for (int i = 0; i < tasks.Count; i++)
        {
            Task task = tasks[i];
            Console.WriteLine((i + 1) + ". [" + (task.IsCompleted ? "Completed" : "Incomplete") + "] " + task.Name + " (Due: " + task.DueDate.ToString("yyyy-MM-dd") + ")");
            Console.WriteLine("Description: " + task.Description);
        }
    }

    static void MarkCompleted()
    {
        Console.WriteLine("---- Mark Completed ----");
        Console.Write("Enter the task number to mark as completed: ");
        if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
        {
            tasks[taskNumber - 1].IsCompleted = true;
            Console.WriteLine("Task marked as completed!");
        }
        else
        {
            Console.WriteLine("Invalid task number. Task not marked.");
        }
    }

    static void DeleteTask()
    {
        Console.WriteLine("---- Delete Task ----");
        Console.Write("Enter the task number to delete: ");
        if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
        {
            tasks.RemoveAt(taskNumber - 1);
            Console.WriteLine("Task deleted successfully!");
        }
        else
        {
            Console.WriteLine("Invalid task number. Task not deleted.");
        }
    }

    static void LoadTasks()
    {
        if (File.Exists(filename))
        {
            string json = File.ReadAllText(filename);
            tasks = DeserializeTasks(json);
        }
    }

    static void SaveTasks()
    {
        string json = SerializeTasks(tasks);
        File.WriteAllText(filename, json);
    }

    // Custom serialization
    static string SerializeTasks(List<Task> tasks)
    {
        using (var writer = new StringWriter())
        {
            writer.WriteLine("[");
            for (int i = 0; i < tasks.Count; i++)
            {
                Task task = tasks[i];
                writer.WriteLine("{");
                writer.WriteLine("\"Name\": \"" + task.Name + "\",");
                writer.WriteLine("\"Description\": \"" + task.Description + "\",");
                writer.WriteLine("\"DueDate\": \"" + task.DueDate.ToString("yyyy-MM-dd") + "\",");
                writer.WriteLine("\"IsCompleted\": " + task.IsCompleted.ToString().ToLower());
                writer.Write("}");
                if (i < tasks.Count - 1)
                    writer.WriteLine(",");
            }
            writer.WriteLine();
            writer.WriteLine("]");
            return writer.ToString();
        }
    }

    // Custom deserialization
    static List<Task> DeserializeTasks(string json)
    {
        List<Task> deserializedTasks = new List<Task>();
        using (var reader = new StringReader(json))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim() == "{")
                {
                    Task task = new Task();
                    while ((line = reader.ReadLine()) != "}")
                    {
                        string[] parts = line.Trim().TrimEnd(',').Split(':');
                        string key = parts[0].Trim().Trim('"');
                        string value = parts[1].Trim().Trim('"', ',');

                        switch (key)
                        {
                            case "Name":
                                task.Name = value;
                                break;
                            case "Description":
                                task.Description = value;
                                break;
                            case "DueDate":
                                DateTime.TryParse(value, out DateTime dueDate);
                                task.DueDate = dueDate;
                                break;
                            case "IsCompleted":
                                bool.TryParse(value, out bool isCompleted);
                                task.IsCompleted = isCompleted;
                                break;
                        }
                    }
                    deserializedTasks.Add(task);
                }
            }
        }
        return deserializedTasks;
    }
}
