using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Core.Entities;

namespace Core.Models
{
    public static class ToDoFile
    {
        public const string Extension = "tdlst";

        public static void Save(FileData data, string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            
            File.WriteAllText(path, JsonSerializer.Serialize(data));
        }

        public static FileData Load(string path)
        {
            if (path.Split(".").Last().Replace(" ", "") == Extension)
            {
                return JsonSerializer.Deserialize<FileData>(File.ReadAllText(path)) ?? new FileData();
            }
            
            throw new InvalidDataException("Invalid file format");
        }
    }
}