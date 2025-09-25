
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
            if (int.TryParse(inp, out int option))
            {
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
                            if (int.TryParse(subInp, out int subOption))
                            {
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
                                
                            } else
                            {
                                Console.WriteLine("Введено некорректное значение!");
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
            else
            {
                Console.WriteLine("Введено некорректное значение!");
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
        List<string> defaultEnglishWords = new List<string>{  "pronunciation", "recognition", "professionality", "specification", "organization", "dependency",
                                                              "exhibition", "demonstration", "embarrassment", "knowledge", "happiness", "obfuscation", 
                                                              "information", "atmosphere", "characteristics", "misunderstanding", "communication", "accident"};
        List<string> defaultRussianWords = new List<string> { "агрегация", "расширение", "абракадабра", "аббревиатура", "параллелепипед", "раздражение",
                                                              "прерогатива", "перекрёсток", "пересечение", "уведомление", "разработка", "произношение", 
                                                              "адекватность", "центробежность", "фрустрация", "деобфускация", "заготовка", "интуиция", 
                                                            };

        Regex englishLetters = new Regex(@"^[a-zA-Z]+$");
        Regex russianLetters = new Regex(@"^[а-яА-Я]+$");

        Console.WriteLine("Введите начальное слово (от 8 до 30 символов):");
        string? word = Console.ReadLine();
           
        if (word == null)
        {
            Random random = new Random();
            return (language == Language.Russian ? defaultRussianWords[random.Next(defaultRussianWords.Count)] : defaultEnglishWords[random.Next(defaultEnglishWords.Count)]);
        }

        if (language == Language.Russian)
        {
            if (russianLetters.IsMatch(word) && (word.Length >= 8 && word.Length <= 30))
            {
                return word;
            }
            Random random = new Random();
            Console.WriteLine("Введённое слово не соответствует условиям, начальное слово будет выбрано случайно!");
            return defaultRussianWords[random.Next(defaultRussianWords.Count)];
        }
        else
        {
            if (englishLetters.IsMatch(word) && (word.Length >= 8 && word.Length <= 30))
            {
                return word;
            }
            Random random = new Random();
            Console.WriteLine("Введённое слово не соответствует условиям, начальное слово будет выбрано случайно!");
            return defaultEnglishWords[random.Next(defaultEnglishWords.Count)];
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