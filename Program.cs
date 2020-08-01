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

namespace task1
{
    class Program
    {
        static void Main(string[] args)
        {
            string strPath = ""; // Путь к директории или файлу.
            int intCode = 0; // Код операции - действия, которое нужно выполнить.
            int intFlag = 0; // Признак наличия файла: 1, признак наличия директории: 2.

            foreach (string strI in args)
            {
                switch (strI.Substring(0, 2))
                {
                    case "/d":
                    case "-d":
                        if (int.TryParse(strI.Substring(2), out intCode))
                            if (intCode < 1 || intCode > 4) intCode = 0;
                        break;
                    default:
                        strPath = strI;
                        if (File.Exists(strPath))
                            intFlag = 1;
                        else if (Directory.Exists(strPath))
                            intFlag = 2;
                        break;
                }
            }
            if (intCode == 0 || intFlag == 0)
            {
                Console.WriteLine("Код\tДействие");
                Console.WriteLine("1. Удаляет из указанного файла введённое слово (если оно есть), сохраняя оригинальный файл.");
                Console.WriteLine("2. Возвращает из указанного файла количество слов и каждое 10-е слово через запятую.");
                Console.WriteLine("3. Выводит из указанного файла 3-е предложение с обратным порядком букв в словах.");
                Console.WriteLine("4. Выводит имена папок по указанному пути. Позволяет выдать список файлов выбранной папки.");
                Console.WriteLine("5. Завершает работу программы\n");
                while (intCode == 0)
                {
                    Console.Write("Задайте код действия: ");
                    int.TryParse(Console.ReadLine(), out intCode);
                    if (intCode == 5) return;
                    if (intCode < 1 || intCode > 4)
                    {
                        Console.WriteLine($"Введён неверный код действия: {intCode}.");
                        intCode = 0;
                    }
                }
                while (intFlag == 0)
                {
                    Console.Write("Задайте путь к файлу/директории: ");
                    strPath = Console.ReadLine();
                    if (strPath.Length == 0) return;
                    if (strPath == "5") return;
                    if (File.Exists(strPath))
                        intFlag = 1;
                    else if (Directory.Exists(strPath))
                        intFlag = 2;
                    else
                        Console.WriteLine($"Введёный путь не найден: '{strPath}'.");
                }
            }
            if (intCode >= 1 && intCode <= 3 && intFlag == 2)
            {
                Console.WriteLine($"Для действий 1, 2, 3 необходим путь к файлу, а не к директории: '{strPath}'.");
                Console.ReadKey();
                return;
            }
            // Основной код.
            switch (intCode)
            {
                case 1:
                    Proc1(strPath);
                    break;
                case 2:
                    Proc2(strPath);
                    break;
                case 3:
                    Proc3(strPath);
                    break;
                case 4:
                    Proc4(strPath);
                    break;
                default:
                    break;
            }
            Console.WriteLine("Для выхода нажмите любую клавишу.");
            Console.ReadKey();
        }

        // Удаляет из указанного файла введённое слово (если оно есть), сохраняя оригинальный файл.
        private static int Proc1(string strPath)
        {
            string strText;
            string strWord;
            int intPos = 0;
            int intCnt = 0;

            if (!File.Exists(strPath)) return -1;
            Console.Write("\nВведите слово для удаления: ");
            strWord = Console.ReadLine();
            strText = File.ReadAllText(strPath); // Читаем содержимое текстового файла в строку.
            //astrText = System.Text.RegularExpressions.Regex.Split(strText, strWord);
            //intCnt = astrText.Length - 1;
            intPos = strText.IndexOf(strWord, intPos); // Ищем подстроку strWord в строке strText. Если она найдена, то intPos - позиция символа, начиная с 0.
            while (intPos > -1)
            {
                strText = strText.Remove(intPos, strWord.Length);
                intPos = strText.IndexOf(strWord, intPos);
                intCnt++;
            }
            Console.WriteLine($"\nУдалено слов, шт: {intCnt}");
            if (intCnt > 0)
            {
                File.Move(strPath, Path.ChangeExtension(strPath, "bak")); // Переименовываем оригинальный файл.
                File.WriteAllText(strPath, strText); // Записываем результат.
            }
            return 0;
        }

        // Возвращает из указанного файла количество слов и каждое 10-е слово через запятую.
        private static int Proc2(string strPath)
        {
            string strText;
            string[] astrText;
            int intI;

            if (!File.Exists(strPath)) return -1;
            strText = File.ReadAllText(strPath); // Читаем содержимое текстового файла в строку.
            astrText = strText.Split(new char[] { ' ', '"', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine($"\nКоличество слов в файле, шт: {astrText.Length}");
            for (intI = 9; intI < astrText.Length; intI += 10)
                Console.Write((intI == 9 ? "" : ", ") + astrText[intI]);
            return 0;
        }

        // Выводит из указанного файла 3-е предложение с обратным порядком букв в словах.
        private static int Proc3(string strPath)
        {
            string strText;
            string[] astrText;
            char[] achrWord;
            char chrTmp;
            int intI, intJ, intK;

            if (!File.Exists(strPath)) return -1;
            strText = File.ReadAllText(strPath); // Читаем содержимое текстового файла в строку.
            astrText = strText.Split(new char[] { '.', '?', '!' }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine($"\nКоличество предложений в файле, шт: {astrText.Length}");
            strText = astrText[2].Trim(); // Сохраняем 3-е предложение.
            Console.WriteLine($"\nТретье предложение:\n{strText}.");
            astrText = strText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Разбиваем 3-е предложение на слова.
            for (intI = 0; intI < astrText.Length; intI++) // Цикл по словам для перестановки в них букв в обратном порядке.
            {
                achrWord = astrText[intI].ToCharArray(); // Разбиваем слово на символы.
                for (intJ = 0; intJ < achrWord.Length; intJ++) // Пропускаем слева символы ',', '"', '(', ')', не нужные для перестановки.
                    if (",\"()".IndexOf(achrWord[intJ], 0) == -1) break;
                for (intK = achrWord.Length - 1; intK >= 0; intK--) // Пропускаем справа символы ',', '"', '(', ')', не нужные для перестановки.
                    if (",\"()".IndexOf(achrWord[intK], 0) == -1) break;
                for (intJ += 0; intJ < (intJ + intK + 1) / 2; intJ++) // Меняем порядок букв: Array.Reverse(achrWord);
                {
                    chrTmp = achrWord[intJ];
                    achrWord[intJ] = achrWord[intK];
                    achrWord[intK] = chrTmp;
                    intK--;
                }
                astrText[intI] = new String(achrWord); // Собираем символы в слово.
            }
            strText = String.Join(" ", astrText); // Собираем слова в предложение.
            Console.WriteLine($"\nТретье предложение с обратным порядком букв в словах:\n{strText}.");
            return 0;
        }

        // Выводит имена папок по указанному пути. Позволяет выдать список файлов выбранной папки.
        private static int Proc4(string strPath)
        {
            string[] astrDir;
            string strTmp;
            int intI, intJ;

            if (File.Exists(strPath))
                strPath = Path.GetDirectoryName(strPath);
            if (!Directory.Exists(strPath)) return -1;
            Console.WriteLine("Список папок по пути:");
            Console.WriteLine(strPath);
            astrDir = Directory.GetDirectories(strPath); // Массив полных путей к папкам в директории strPath.
            Array.Sort(astrDir); // Сортировка массива в алфавитном порядке.
            Console.WriteLine("   ID\tПапка");
            for (intI = 0; intI < astrDir.Length; intI++) // Выделяем имена папок из полных путей и добавляем слева числовой идентификатор.
            {
                strTmp = astrDir[intI];
                intJ = strTmp.LastIndexOf(Path.DirectorySeparatorChar);
                if (intJ > -1) strTmp = strTmp.Substring(intJ + 1);
                Console.Write("{0,5:0}", intI + 1);
                Console.WriteLine("\t" + strTmp);
            }
            intI = 0;
            while (intI == 0)
            {
                Console.Write("Введите ID папки для вывода списка файлов или 0 для выхода: ");
                int.TryParse(Console.ReadLine(), out intI);
                if (intI == 0) return 0;
                if (intI < 1 || intI > astrDir.Length)
                {
                    Console.WriteLine("Введён неверный ID папки: {intI}.");
                    intI = 0;
                }
            }
            strPath = astrDir[intI - 1];
            astrDir = Directory.GetFiles(strPath); // Массив полных путей к файлам в директории strPath.
            Array.Sort(astrDir); // Сортировка массива в алфавитном порядке.
            Console.WriteLine("    №\tФайл");
            for (intI = 0; intI < astrDir.Length; intI++) // Выделяем имена файлов из полных путей и добавляем слева номер по порядку.
            {
                strTmp = astrDir[intI];
                //intJ = strTmp.LastIndexOf(Path.DirectorySeparatorChar);
                //if (intJ > -1) strTmp = strTmp.Substring(intJ + 1);
                strTmp = Path.GetFileName(strTmp);
                Console.Write("{0,5:0}", intI + 1);
                Console.WriteLine("\t" + strTmp);
            }
            return 0;
        }
    }
}