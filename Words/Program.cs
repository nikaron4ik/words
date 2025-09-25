
using System.Text.RegularExpressions;
using Timer = System.Timers.Timer;

class Program
{


    static void Main(string[] args)
    {
        Console.WriteLine("Добро пожаловать в игру \"Слова\"!");


        while (true)
        {
            Console.WriteLine("Игра \"Слова\"!");

            Console.WriteLine("1. Начать игру");
            Console.WriteLine("2. Выход");
            Console.Write("Выберите опцию: ");
            string? inp = Console.ReadLine();

            if (inp == null) {
                Console.WriteLine("Введено некорректное значение!");
                continue;
            }
            int option = Convert.ToInt32(inp);
            switch (option)
            {
                case 1:
                    {
                        Console.WriteLine("Язык игры");

                        Console.WriteLine("1. Русский");
                        Console.WriteLine("2. English");
                        Console.WriteLine("3. Вернуться в главное меню");
                        Console.Write("Выберите опцию: ");
                        string? subInp = Console.ReadLine();

                        if (subInp == null)
                        {
                            Console.WriteLine("Введено некорректное значение!");
                            continue;
                        }
                        int subOption = Convert.ToInt32(subInp);
                        switch (subOption)
                        {
                            case 1:
                                startGame(getInitialWord(Language.Russian));
                                break;
                            case 2:
                                startGame(getInitialWord(Language.English));
                                break;
                            case 3:
                                break;
                        }
                        break;
                    }
                case 2:
                    Console.WriteLine("Спасибо за игру!");
                    return;
                default:
                    Console.WriteLine("Введено некорректное значение!");
                    break;
            }
        }


    }

    enum Language
    {
        English,
        Russian
    }

    enum Player
    {
        First,
        Second
    }


    private static string getInitialWord(Language language)
    {   
        Regex englishLetters = new Regex(@"^[a-zA-Z]+$");
        Regex russianLetters = new Regex(@"^[а-яА-Я]+$");

        Console.WriteLine("Введите начальное слово:");
        string? word = Console.ReadLine();

        if (word == null) return language == Language.Russian ? "абракадабра" : "pronunciation";

        if (language == Language.Russian)
        {
            if (russianLetters.IsMatch(word))
            {
                return word;
            }
            return "абракадабра";                       // TODO: пока также затычка, позже придумать что-то адекватнее
        }
        else
        {
            if (englishLetters.IsMatch(word))
            {
                return word;
            }
            return "pronunciation";                     // TODO: также затычка, позже придумать, как грамотнее обработать
        }
    }



    private static void startGame(String startWord)
    {
        Dictionary<char, int> startWordLetters = divideWordIntoLetters(startWord);
        HashSet<string> usedWords = new HashSet<string>();
        var player = Player.First;

        while (true)
        {
            string playerNum = (player == Player.First ? "1" : "2");
            Console.WriteLine($"\nХод игрока {playerNum}");
            Console.WriteLine($"Начальное слово: {startWord}");

            bool isTimeExpired = false;
            string? input = null;

            Thread inputThread = new Thread(() =>
            {
            try
                {
                    Console.WriteLine("Введите слово:");
                    input = Console.ReadLine();
                }
                catch (ThreadInterruptedException) { }
            });

            Timer timer = new Timer(20000);

            timer.Elapsed += (source, e) =>
            {
                inputThread?.Interrupt();
                isTimeExpired = true;
                timer.Stop();
            };

            isTimeExpired = false;
            timer.Start();
            inputThread.Start();
            inputThread.Join();

            timer.Stop();

            /**
            * Сомнительная проверка, ибо чтобы увидеть сообщение о времени требуется нажать enter, 
            * т.к. Console.Readline() даже после thread.Interrupt() не заканчивается автоматически,
            * но пока более адекватного способа прерывания ввода по времени не придумал 
            */ 
            if (isTimeExpired)
            {
                Console.WriteLine($"К сожалению, время на ввод уже вышло! Игрок {playerNum} проиграл!");
                break;
            }

            if (input == null || input == "")
            {
                Console.WriteLine($"Строка не была введена! Игрок {playerNum} проиграл!");
                break;
            }

            input = input.ToLower();

            if (usedWords.Contains(input))
            {
                Console.WriteLine("Слово уже было использовано ранее, придумайте другое!");
                continue;
            }

            if (isWordCorrect(input, startWordLetters))
            {
                usedWords.Add(input);
                Console.WriteLine("Слово подходит! Ход переходит к следующему игроку!");
                player = player == Player.First ? Player.Second : Player.First;
            }
            else
            {
                Console.WriteLine($"Слово не подходит! Игрок {playerNum} проиграл!");
                break;
            }

        }




        return;
    }

    private static Dictionary<char, int> divideWordIntoLetters(string word)
    {
        Dictionary<char, int> wordLetters = new Dictionary<char, int>();

        foreach (char c in word.ToLower())
        {
            if (wordLetters.ContainsKey(c))
            {
                wordLetters[c]++;
            }
            else
            {
                wordLetters.Add(c, 1);
            }
        }

        return wordLetters;
    }


    private static bool isWordCorrect(string input, Dictionary<char, int> startWordLetters)
    {
        Dictionary<char, int> inputLetters = divideWordIntoLetters(input);

        foreach (char k in inputLetters.Keys)
        {
            if (!(startWordLetters.ContainsKey(k) && startWordLetters[k] >= inputLetters[k])) return false;
        }
        return true;
    }
    

}