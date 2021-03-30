using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Core.Models;

namespace Core.Entities
{
    public static class CommandLineParser
    {
        public static ConsoleResponse Parse(string line)
        {
            List<string> args = line.Split(" ").ToList();

            switch (args[0])
            {
                case "/add":
                    int infoIndex = args.FindIndex((s => s == "-i"));
                    int deadlineIndex = args.FindIndex((s => s == "-d"));
                    int formatIndex = args.FindIndex(s => s == "-f");

                    if (args.Count < Math.Max(infoIndex, deadlineIndex) + 1)
                        throw new InvalidDataException();

                    return new ConsoleResponse(ConsoleResponse.Actions.Add)
                    {
                        Name = args[1],
                        Info = infoIndex == -1 ? "" : args[infoIndex + 1],
                        Deadline = deadlineIndex == -1
                            ? null
                            : DateTime.ParseExact(args[deadlineIndex + 1],
                                (formatIndex == -1 ? "" : args[formatIndex + 1]), CultureInfo.InvariantCulture)
                    };

                case "/add-subtask":
                    infoIndex = args.FindIndex(s => s == "-i");

                    if (args.Count < infoIndex + 1)
                        throw new InvalidDataException();

                    return new ConsoleResponse(ConsoleResponse.Actions.AddSubtask)
                    {
                        Info = infoIndex == -1 ? "" : args[infoIndex + 1]
                    };

                case "/delete":
                    if (args.Count < 2)
                        throw new InvalidDataException();

                    return new ConsoleResponse(ConsoleResponse.Actions.Delete)
                    {
                        Id = Int32.Parse(args[1])
                    };


                case "/create-group":
                    if (args.Count < 2)
                        throw new InvalidDataException();

                    return new ConsoleResponse(ConsoleResponse.Actions.CreateGroup)
                    {
                        Name = args[1]
                    };

                case "/delete-group":
                    if (args.Count < 2)
                        throw new InvalidDataException();

                    return new ConsoleResponse(ConsoleResponse.Actions.DeleteGroup)
                    {
                        Name = args[0]
                    };

                case "/add-to-group":
                    if (args.Count < 3)
                        throw new InvalidDataException();

                    return new ConsoleResponse(ConsoleResponse.Actions.AddToGroup)
                    {
                        Id = Int32.Parse(args[1]),
                        Name = args[2]
                    };

                case "/delete-from-group":
                    if (args.Count < 3)
                        throw new InvalidDataException();

                    return new ConsoleResponse(ConsoleResponse.Actions.DeleteFromGroup)
                    {
                        Id = Int32.Parse(args[1]),
                        Name = args[2]
                    };


                case "/all":
                    return new ConsoleResponse(ConsoleResponse.Actions.All);

                case "/today":
                    return new ConsoleResponse(ConsoleResponse.Actions.Today);


                case "/save":
                    return new ConsoleResponse(ConsoleResponse.Actions.Save)
                    {
                        Path = args.Count > 1 ? args[1] : ""
                    };

                case "/load":
                    if (args.Count < 2)
                        throw new InvalidDataException();

                    return new ConsoleResponse(ConsoleResponse.Actions.Load)
                    {
                        Path = args[1]
                    };

                case "/path":
                    if (args.Count > 1)
                        Path.GetFullPath(args[1]);
                    if (args[1].Split(".").Last() != ToDoFile.Extension)
                    {
                        throw new InvalidDataException();
                    }

                    return new ConsoleResponse(ConsoleResponse.Actions.Path)
                    {
                        Path = args.Count > 1 ? args[1] : ""
                    };


                case "/complete":
                    if (args.Count < 2)
                        throw new InvalidDataException();

                    return new ConsoleResponse(ConsoleResponse.Actions.Complete)
                    {
                        Id = Int32.Parse(args[1])
                    };

                case "/completed":
                    if (args.Count > 1)
                        return new ConsoleResponse(ConsoleResponse.Actions.CompletedGroup)
                        {
                            Name = args[1]
                        };

                    return new ConsoleResponse(ConsoleResponse.Actions.Completed);


                case "/exit":
                    return new ConsoleResponse(ConsoleResponse.Actions.Exit);

                case "/help":
                    return new ConsoleResponse(ConsoleResponse.Actions.Help)
                    {
                        HelpWith = args.Count > 1 ? args[1] : ""
                    };

                default:
                    return new ConsoleResponse(ConsoleResponse.Actions.None);
            }
        }
    }
}