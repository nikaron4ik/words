using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Timers;

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
            int option = Convert.ToInt32(Console.ReadLine());
            switch (option)
            {
                case 1:
                    {
                        Console.WriteLine("Выберите язык игры");
                        Console.WriteLine("1. Русский");
                        Console.WriteLine("2. English");
                        Console.WriteLine("3. Вернуться в главное меню");
                        int subOption = Convert.ToInt32(Console.ReadLine());
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
        Dictionary<char, int> startWordLetters = new Dictionary<char, int>();

        foreach (char c in startWord) {
            if (startWordLetters.ContainsKey(c))
            {
                startWordLetters[c]++;
            }
            else
            {
                startWordLetters.Add(c, 1);
            }
        }

        

        return;
    }

}