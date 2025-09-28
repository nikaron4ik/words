
using System.Text.RegularExpressions;
using Timer = System.Timers.Timer;

class Program
{

    static readonly List<string> DefaultEnglishWords = new List<string>{  "pronunciation", "recognition", "professionality", "specification", "organization", "dependency",
                                                              "exhibition", "demonstration", "embarrassment", "knowledge", "happiness", "obfuscation",
                                                              "information", "atmosphere", "characteristics", "misunderstanding", "communication", "accident"};
    static readonly List<string> DefaultRussianWords = new List<string> { "агрегация", "расширение", "абракадабра", "аббревиатура", "параллелепипед", "раздражение",
                                                              "прерогатива", "перекрёсток", "пересечение", "уведомление", "разработка", "произношение",
                                                              "адекватность", "центробежность", "фрустрация", "деобфускация", "заготовка", "интуиция"};


    enum Language { English, Russian }

    enum Player { First, Second }


    static void Main(string[] args)
    {
        Console.WriteLine("Добро пожаловать в игру \"Слова\"!");


        while (true)
        {
            ShowMainMenu();
            string? mainMenuInput = Console.ReadLine();

            if (mainMenuInput == null) {
                Console.WriteLine("Введено некорректное значение!");
                continue;
            }
            if (int.TryParse(mainMenuInput, out int option))
            {
                switch (option)
                {
                    case 1:
                        {
                            ShowLanguageSelectionMenu();
                            string? languageInput = Console.ReadLine();

                            if (languageInput == null)
                            {
                                Console.WriteLine("Введено некорректное значение!");
                                continue;
                            }
                            if (int.TryParse(languageInput, out int subOption))
                            {
                                switch (subOption)
                                {
                                    case 1:
                                        StartGame(GetInitialWord(Language.Russian));
                                        break;
                                    case 2:
                                        StartGame(GetInitialWord(Language.English));
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

    static void ShowMainMenu()
    {
        Console.WriteLine("Игра \"Слова\"!");

        Console.WriteLine("1. Начать игру");
        Console.WriteLine("2. Выход");
        Console.Write("Выберите опцию: ");
    }

    static void ShowLanguageSelectionMenu()
    {
        Console.WriteLine("Язык игры");

        Console.WriteLine("1. Русский");
        Console.WriteLine("2. English");
        Console.WriteLine("3. Вернуться в главное меню");
        Console.Write("Выберите опцию: ");
    }

    static string GetInitialWord(Language language)
    { 
        
        string? word = InitializeWord();

        if (IsValidInitialWord(word, language))
        {
            return word;
        }

        
        Console.WriteLine("Введённое слово не соответствует условиям, начальное слово будет выбрано случайно!");
        return GetRandomWord(language);
        
    }

    static string? InitializeWord()
    {
        Console.WriteLine("Введите начальное слово (от 8 до 30 символов):");
        return Console.ReadLine();
    }

    static bool IsValidInitialWord(string? word, Language language)
    {
        if (word == null) return false;

        Regex regex = language == Language.Russian
            ? new Regex(@"^[а-яА-Я]+$")
            : new Regex(@"^[a-zA-Z]+$"); 

        if (regex.IsMatch(word) && word.Length >= 8 && word.Length <= 30)
        {
            return true;
        }

        return false;
    }

    static string GetRandomWord(Language language)
    {
        Random random = new Random();
        return language == Language.Russian 
            ? DefaultRussianWords[random.Next(DefaultRussianWords.Count)] 
            : DefaultEnglishWords[random.Next(DefaultEnglishWords.Count)];
    }



    static void StartGame(String startWord)
    {
        Dictionary<char, int> startWordLetters = DivideWordIntoLetters(startWord);
        HashSet<string> usedWords = new HashSet<string>();
        var player = Player.First;

        while (true)
        {
            ShowPlayerTurn(player, startWord);

            (string? word, bool isTimeOut) playerInput = HandlePlayerInputWithTimer();


            if (HasPlayerLost(playerInput.word, playerInput.isTimeOut, player)) break;

            string word = playerInput.word.ToLower();

            if (IsWordAlreadyUsed(word, usedWords)) continue;

            if (IsWordCorrect(word, startWordLetters))
            {
                usedWords.Add(word);
                Console.WriteLine("Слово подходит! Ход переходит к следующему игроку!");
                player = SwitchPlayer(player);
            }
            else
            {
                Console.WriteLine($"Слово не подходит! Игрок {GetPlayerNumber(player)} проиграл!");
                break;
            }

        }

        return;
    }

    /* 
     *  Questionable checking, because in order to see time expired message user need to press "Enter" button,
     *  cuz Console.Readline() doesn't end automatically even after thread.Interrput(),
     *  but I don't know how to fix it properly right now, maybe will find any good way later
    */
    static (string? input, bool isTimeOut) HandlePlayerInputWithTimer()
    {
        bool isTimeOut = false;
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
            isTimeOut = true;
            timer.Stop();
        };

        isTimeOut = false;
        timer.Start();
        inputThread.Start();
        inputThread.Join();

        timer.Stop();

        return (input, isTimeOut);
    }

    static void ShowPlayerTurn(Player player, string startWord)
    {
        string playerNum = (player == Player.First ? "1" : "2");
        Console.WriteLine($"\nХод игрока {playerNum}");
        Console.WriteLine($"Начальное слово: {startWord}");
    }

    static string GetPlayerNumber(Player player)
    {
        return player == Player.First ? "1" : "2";
    }

    static Player SwitchPlayer(Player player)
    {
        return player == Player.First ? Player.Second : Player.First;
    }

    static bool HasPlayerLost(string? word, bool isTimeOut, Player player)
    {
        if (isTimeOut == true || (word == null || word == ""))
        {
            ShowLossMessage(player, isTimeOut);
            return true;
        }
        return false;
    }

    static bool IsWordAlreadyUsed(string word, HashSet<string> usedWords)
    {
        if (usedWords.Contains(word))
        {
            Console.WriteLine("Слово уже было использовано ранее, придумайте другое!");
            return true;
        }
        return false;
    }

    static void ShowLossMessage(Player player, bool isTimeOut)
    {
        if (isTimeOut)
        {
            Console.WriteLine($"К сожалению, время на ввод уже вышло! Игрок {GetPlayerNumber(player)} проиграл!");
        }
        else
        {
            Console.WriteLine($"Строка не была введена! Игрок {GetPlayerNumber(player)} проиграл!");
        }
    }

    static Dictionary<char, int> DivideWordIntoLetters(string word)
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


    static bool IsWordCorrect(string input, Dictionary<char, int> startWordLetters)
    {
        Dictionary<char, int> inputLetters = DivideWordIntoLetters(input);

        foreach (char k in inputLetters.Keys)
        {
            if (!(startWordLetters.ContainsKey(k) && startWordLetters[k] >= inputLetters[k])) return false;
        }
        return true;
    }
    

}