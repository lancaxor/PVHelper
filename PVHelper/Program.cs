using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PVHelper
{
    class Program
    {
        private static List<CommandWithHelp> commands = new List<CommandWithHelp>();
        private static List<Point> points = new List<Point>();
        private static String input;
        private static String HelpString = "Для получения справки по команде введите <имя команды>. Список доступных команд:\n\n" +
            "ADD\tДобавление сектор в список.\n" +
            "CLEAR\tОчистка списка секторов.\n" +
            "EXIT\tВыход из программы.\n" +
            "HELP\tВывод краткой справки по командам.\n" +
            "LIST\tВывод координат секторов, добавленных в список.\n" +
            "LOAD\tЗагрузить координаты секторов из файла.\n" +
            "SORT\tВывод секторов, отсортированных по расстоянию к указанному сектору.\n" +
            "REMOVE\tУдаление сектор из списка.";

        private static void Init()
        {
            commands.Add(new CommandWithHelp { Command = "Add", CommandHepString = "Добавить сектор в список. Использование: add xx;yy\nПараметры:\n\txx\tХ-координата.\n\tyy\tY-координата" });
            commands.Add(new CommandWithHelp { Command = "Help", CommandHepString = "Вывод этой справки." });
            commands.Add(new CommandWithHelp { Command = "List", CommandHepString = "Вывести список секторов." });
            commands.Add(new CommandWithHelp { Command = "Remove", CommandHepString = "Удалить сектор из списка. Использование: remove n\nПараметры:\n\tn\tИндекс сектора в списке (см. list)." });
            commands.Add(new CommandWithHelp { Command = "Load", CommandHepString = "Загрузить координаты секторов из текстового файла. Использование: load path\nПараметры:\n\tpath\tПуть к файлу." });
            commands.Add(new CommandWithHelp { Command = "Sort", CommandHepString = "Вывести список секторов, отсортированный по возрастанию расстояния до указанного сектора. Использование: sort xx;yy\nПараметры:\n\txx\tХ-координата.\n\tyy\tY-координата" });
            commands.Add(new CommandWithHelp { Command = "Clear", CommandHepString = "Очистить список секторов. Отменить операцию невозможно." });
            commands.Sort(new CmdWithHlpComparer());
        }

        static void Main(string[] args)
        {
            Init();
            int tmp;
            Console.WriteLine(HelpString);
            String tmpStr;
            do
            {
                input = Scan();
                switch (input.Substring(0,input.IndexOf(" ")<0? input.Length:input.IndexOf(" ")))      // format: command parameter
                {
                    case "help":
                        Console.WriteLine(HelpString);
                        break;

                    case "add":
                        if (input.IndexOf(" ") < 0)
                        {
                            var addCommand = from addCmd in commands where addCmd.Command.ToLower().Equals("add") select addCmd;
                            Console.WriteLine(addCommand.ToArray<CommandWithHelp>()[0].CommandHepString);
                            break;
                        }
                        tmpStr = input.Substring(input.IndexOf(" ")).Trim();
                        Add(tmpStr, "add");
                        break;

                    case "list":
                        if (points.Count == 0)
                            Console.WriteLine("Список секторов пуст. Используйте команду add для добавления сектора в список.");
                        else
                            foreach (Point p in points)
                                Console.WriteLine(p.ID + ". (" + p.Coords + ")");
                        break;

                    case "remove":
                        tmpStr = input.Substring(input.IndexOf(" ")).Trim();
                        if (input.IndexOf(" ") < 0)
                        {
                            var addCommand = from addCmd in commands where addCmd.Command.ToLower().Equals("remove") select addCmd;
                            foreach (var p in addCommand)
                                Console.WriteLine(p.CommandHepString);
                            break;
                        }
                        if (Int32.TryParse(tmpStr, out tmp))
                        {
                            var q = from nums in points where nums.ID == tmp select nums;
                            if (q.Count() == 0) {
                                Console.WriteLine("Сектора с номером {0} не существует. Введите list для просмотра списка секторов.", tmp);
                                break; }
                            String rc = q.ToArray<Point>()[0].Coords;
                            points.Remove(q.ToArray<Point>()[0]);
                            Console.WriteLine("Сектор с координатами {0} успешно удален из списка.", rc);
                        }
                        break;

                    case "sort":
                        if (input.IndexOf(" ") < 0)     //no space after command
                        {
                            var addCommand = from addCmd in commands where addCmd.Command.ToLower().Equals("add") select addCmd;
                            Console.WriteLine(addCommand.ToArray<CommandWithHelp>()[0].CommandHepString);
                            break;
                        }
                        tmpStr = input.Substring(input.IndexOf(" ")).Trim();        //get space
                        if (tmpStr.IndexOf(";") < 0)        //no semicolon in command parameter
                        {
                            Console.WriteLine("Неверный параметр команды SORT.");
                            var addCommand = from addCmd in commands where addCmd.Command.ToLower().Equals("sort") select addCmd;
                            Console.WriteLine(addCommand.ToArray<CommandWithHelp>()[0].CommandHepString);
                            break;
                        }
                        int x, y;
                        if ((Int32.TryParse(tmpStr.Substring(0, tmpStr.IndexOf(";")), out x) && Int32.TryParse(tmpStr.Substring(tmpStr.IndexOf(";") + 1), out y)))
                        {
                            var sorted = from pts in points orderby pts.GetDistance(x, y) select pts;
                            if (sorted.ToArray().Count() == 0)
                            {
                                Console.WriteLine("Список секторов пуст. Используйте команду ADD для добавления сектора в список.");
                                break;
                            }
                            foreach (var p in sorted)
                            {
                                Console.WriteLine(p.ID + ". (" + p.Coords + ") => " + p.GetDistance(x, y) + "км");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Неверные координаты.");
                            var addCommand = from addCmd in commands where addCmd.Command.ToLower().Equals("sort") select addCmd;
                            Console.WriteLine(addCommand.ToArray<CommandWithHelp>()[0].CommandHepString);
                        }
                        break;
                    case "exit":
                        break;
                    case "clear":
                        points.Clear();
                        Point.ZeroID = true;
                        Console.WriteLine("Список секторов успешно очищен.");
                        break;
                    case "load":
                        if (input.IndexOf(" ") < 0)     //no space after command
                        {
                            var addCommand = from addCmd in commands where addCmd.Command.ToLower().Equals("load") select addCmd;
                            Console.WriteLine(addCommand.ToArray<CommandWithHelp>()[0].CommandHepString);
                            break;
                        }
                        tmpStr = input.Substring(input.IndexOf(" ")).Trim();        //string after space
                        if (File.Exists(tmpStr))
                            foreach (String str in File.ReadAllLines(tmpStr))
                                Add(str, "load");
                        else Console.WriteLine("Файл {0} не существует или к нему невозможно получить доступ.", tmpStr);
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда. Введите help для вывода справки.");
                        break;
                }
            } while (input != "exit");
        }

        private static void Add(String coords, String command)
        {
            int x, y;
            if (coords.IndexOf(";") < 0)        //no semicolon symbol in coords
            {
                if (command.Equals("add"))
                {
                    Console.WriteLine("Неверный параметр.");
                    var query = from cmd in commands where cmd.Command.ToLower().Equals(command) select cmd;
                    Console.WriteLine(query.ToArray<CommandWithHelp>()[0].CommandHepString);
                }
                else Console.WriteLine("Неверный параметр: {0}. Игнорирование.", coords);
                return;
            }
            if ((Int32.TryParse(coords.Substring(0, coords.IndexOf(";")), out x) && Int32.TryParse(coords.Substring(coords.IndexOf(";") + 1), out y)))
            {
                var getc = from pts in points where pts.Coords.Equals(x.ToString() + ";" + y.ToString()) select pts;
                if (getc.ToArray().Count() > 0)
                {
                    Console.WriteLine("Сектор с координатами {0} уже существует в списке.", coords);
                    return;
                }
                points.Add(new Point { Coords = coords });
                Console.WriteLine("Сектор с координатами {0} успешно добавлен в список.", coords);
            }
            else
            {
                if (command.Equals("add"))
                {
                    Console.WriteLine("Неверные координаты.");
                    var query = from qres in commands where qres.Command.ToLower().Equals(command) select qres;
                    Console.WriteLine(query.ToArray<CommandWithHelp>()[0].CommandHepString);
                }
                else Console.WriteLine("Неверный параметр: {0}. Игнорирование.", coords);
            }
        }

        private static string Scan()
        {
            Console.Write("\n> ");
            return Console.ReadLine().Trim().ToLower();
        }
    }
}
