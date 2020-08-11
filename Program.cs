/*
 * Считать по указанному в пути текстовый файл и удалить (предварительно сохранив оригинальный файл) в нём указанный
 * в консоли символ/слово, в случае, если указанного слова в тексте нет вывести соответствующее сообщение.
 * 
 * Считывает текстовый файл и вывести на экран количество слов в тексте, а также вывести каждое 10-е слово через запятую.
 *
 * Вывести 3-е предложение в тексте. При чем буквы слов должны быть в обратном порядке.
 * 
 * Вывести имена папок по указанному пути в консоли. У каждой папки должен быть идентификатор, по которому пользователь сможет
 * находить нужную папку и видеть все файлы, которые у неё внутри. Имена папок и файлов должны быть отсортированы в алфавитном порядке.
 */

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace task1
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = "";
            string[] parts;
            TxtFile tf;
            TxtPart tp;
            DirDirectories dd;
            DirFiles df;
            DirOutput dp;

            TxtInput ti = new TxtInput(args);
            if (ti.Flag > 0)
            {
                if (ti.Code != 4)
                {
                    tf = new TxtFile(ti.Path);
                    if (tf.Load(ref text))
                    {
                        switch (ti.Code)
                        {
                            case 1:
                                tp = new TxtPart();
                                Console.Write("Введите слово для удаления: ");
                                tp.Pattern = Console.ReadLine();
                                parts = tp.Split(ref text);
                                if (parts.Length - 1 > 0)
                                {
                                    tp.Pattern = "";
                                    Console.WriteLine($"Удалено фраз '{tp.Pattern}', шт: '{parts.Length - 1}'.");
                                    text = tp.Join(parts);
                                    if (!tf.Save(ref text, true)) Console.WriteLine($"Не удалось записать файл  '{tf.Path}'.");
                                }
                                else
                                    Console.WriteLine($"Не найдена фраза '{tp.Pattern}'.");
                                break;
                            case 2:
                                tp = new TxtPart();
                                tp.Pattern = "[ ,.!?\"\n\r]";
                                parts = tp.Split(ref text);
                                Console.WriteLine($"Количество слов в файле, шт: {parts.Length}.");
                                Console.WriteLine("Каждое 10-е слово:");
                                for (int i = 9; i < parts.Length; i += 10)
                                    Console.Write((i == 9 ? "" : ", ") + parts[i]);
                                Console.WriteLine(".");
                                break;
                            case 3:
                                tp = new TxtPart();
                                tp.Pattern = "[.!?]";
                                parts = tp.Split(ref text);
                                Console.WriteLine($"Количество предложений в файле, шт: {parts.Length - 1}.");
                                if (parts.Length - 1 > 2)
                                {
                                    tp.Pattern = @"[A-zА-яЁё]+";
                                    parts[2] = parts[2].Trim();
                                    parts[2] = tp.Reverse(ref parts[2]);
                                    Console.WriteLine("3-е предложение с буквами слов в обратном порядке:");
                                    Console.WriteLine(parts[2] + ".");
                                }
                                else
                                    Console.WriteLine("Отсутствует 3-е предложение.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Не удалось считать файл '{tf.Path}'.");
                    }
                }
                else
                {
                    dd = new DirDirectories();
                    dd.Path = ti.Path;
                    parts = dd.Items();
                    Console.WriteLine("   ID\tПапка");
                    dp = new DirOutput();
                    dp.Print(parts);
                    int i = 0;
                    while (i == 0)
                    {
                        Console.Write("Введите ID папки для вывода списка файлов или 0 для выхода: ");
                        int.TryParse(Console.ReadLine(), out i);
                        if (i == 0) return;
                        if (i < 1 || i > parts.Length)
                        {
                            Console.WriteLine("Введён неверный ID папки: {i}.");
                            i = 0;
                        }
                    }
                    df = new DirFiles();
                    df.Path = parts[i - 1];
                    parts = df.Items();
                    Console.WriteLine("   ID\tФайл");
                    dp.Print(parts);
                }
            }
            Console.WriteLine("Для выхода нажмите любую клавишу.");
            Console.ReadKey();
        }
    }

    // Согласно принципу S (The Single Responsibility Principle) - принцип единой ответственности (SRP) создаём для 1, 2 и 3-го задания
    // используем три класса: TxtInput - ввод исходных данных, TxtFile - загрузка/сохранение данных из/в файл; TxtPart - работа с частями
    // текста (разбиение, объединение и реверс).
    public class TxtInput
    {
        public string Path;
        public int Code;
        public int Flag;
        public TxtInput(string[] args) // Конструктор по умолчанию.
        {
            foreach (string s in args)
            {
                switch (s.Substring(0, 2))
                {
                    case "/d":
                    case "-d":
                        if (int.TryParse(s.Substring(2), out this.Code))
                            if (this.Code < 1 || this.Code > 4) this.Code = 0;
                        break;
                    default:
                        this.Path = s;
                        if (File.Exists(this.Path))
                            this.Flag = 1;
                        else if (Directory.Exists(this.Path))
                            this.Flag = 2;
                        break;
                }
            }
            if (this.Code == 0 || this.Flag == 0)
            {
                Console.WriteLine("Код\tДействие");
                Console.WriteLine("1. Удаляет из указанного файла введённое слово (если оно есть), сохраняя оригинальный файл.");
                Console.WriteLine("2. Возвращает из указанного файла количество слов и каждое 10-е слово через запятую.");
                Console.WriteLine("3. Выводит из указанного файла 3-е предложение с обратным порядком букв в словах.");
                Console.WriteLine("4. Выводит имена папок по указанному пути. Позволяет выдать список файлов выбранной папки.");
                Console.WriteLine("5. Завершает работу программы\n");
                while (this.Code == 0)
                {
                    Console.Write("Задайте код действия: ");
                    int.TryParse(Console.ReadLine(), out this.Code);
                    if (this.Code == 5) return;
                    if (this.Code < 1 || this.Code > 4)
                    {
                        Console.WriteLine($"Введён неверный код действия: {this.Code}.");
                        this.Code = 0;
                    }
                }
                while (this.Flag == 0)
                {
                    Console.Write("Задайте путь к файлу/директории: ");
                    this.Path = Console.ReadLine();
                    if (this.Path.Length == 0) return;
                    if (this.Path == "5") return;
                    if (File.Exists(this.Path))
                        this.Flag = 1;
                    else if (Directory.Exists(this.Path))
                        this.Flag = 2;
                    else
                        Console.WriteLine($"Введёный путь не найден: '{this.Path}'.");
                }
            }
            if (this.Code >= 1 && this.Code <= 3 && this.Flag == 2)
            {
                this.Flag = 0;
                Console.WriteLine($"Для действий 1, 2, 3 необходим путь к файлу, а не к директории: '{this.Path}'.");
            }
            return;
        }
    }

    public class TxtFile
    {
        public string Path;
        public TxtFile(string p) { Path = p; } // Конструктор по умолчанию.
        public bool Load(ref string t)
        {
            if (!File.Exists(this.Path)) return false;
            t = File.ReadAllText(this.Path); // Читаем содержимое текстового файла.
            return true;
        }
        public bool Save(ref string t, bool b = true)
        {
            if (!File.Exists(this.Path)) return false;
            if (b) File.Move(this.Path, System.IO.Path.ChangeExtension(this.Path, "bak")); // Переименовываем оригинальный файл.
            File.WriteAllText(this.Path, t); // Записываем результат в текстовый файл.
            return true;
        }
    }

    public class TxtPart
    {
        public string Pattern;
        public string[] Split(ref string t)
        {
            if (this.Pattern.Substring(0, 1) == "[" && this.Pattern.Substring(this.Pattern.Length - 1, 1) == "]") // Разбиваем текст на части.
                return t.Split(this.Pattern.Substring(1, this.Pattern.Length - 2).ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            else
                return t.Split(this.Pattern, int.MaxValue, StringSplitOptions.RemoveEmptyEntries);
        }
        public string Join(string[] a)
        {
            return String.Join(this.Pattern, a); // Собираем текст из частей.
        }

        private string ReverseWord(Match m)
        {
            string w = m.Value;
            char[] chars = w.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }
        public string Reverse(ref string t)
        {
            Regex regex = new Regex(this.Pattern);
            return regex.Replace(t, new MatchEvaluator(this.ReverseWord));
        }
    }

    // Согласно принципу O (The Open Closed Principle) - обозначает принцип открытости/закрытости (OCP) создаём общий интерфейс для
    // классов, возвращающих список папок и список файлов из указанной директории.
    public class IDir
    {
        public string Path;
        public virtual string[] Items()
        {
            return new String[] { };
        }
    }

    public class DirDirectories : IDir
    {
        public override string[] Items()
        {
            if (!Directory.Exists(this.Path)) return new String[] { };
            string[] a = Directory.GetDirectories(this.Path); // Массив полных путей к папкам в директории 'Path'.
            Array.Sort(a);
            return a;
        }
    }

    public class DirFiles : IDir
    {
        public override string[] Items()
        {
            if (!Directory.Exists(this.Path)) return new String[] { };
            string[] a = Directory.GetFiles(this.Path); // Массив полных путей к файлам в директории 'Path'.
            Array.Sort(a);
            return a;
        }
    }

    public class DirOutput
    {
        public void Print(string[] a) 
        {
            for (int i = 0; i < a.Length; i++) // Выделяем имена папок/файлов из полных путей и добавляем слева идентификатор (номер по порядку).
            {
                string s = a[i];
                int p = s.LastIndexOf(Path.DirectorySeparatorChar);
                if (p > -1) s = s.Substring(p + 1);
                Console.Write("{0,5:0}", i + 1);
                Console.WriteLine("\t" + s);
            }
        }
    }
}
