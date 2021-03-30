using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Core.Entities;

namespace Core.Models
{
    public class ToDoFile
    {
        public const string Extension = "tdlst";

        public static void Save(List<Task> tasks, List<Group> groups, int size, string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            
            File.WriteAllText(path, JsonSerializer.Serialize((tasks, groups, size)));
        }

        public static (List<Task>, List<Group>, int) Load(string path)
        {
            var result = (new List<Task>(), new List<Group>(), 0);

            if (path.Split(".").Last().Replace(" ", "") == Extension)
            {
                result = JsonSerializer.Deserialize<(List<Task>, List<Group>, int)>(File.ReadAllText(path));
            }
            else
            {
                throw new InvalidDataException("Invalid file format");
            }
            
            return result;
        }
    }
}