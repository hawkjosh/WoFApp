using System.Security.Cryptography;
using System.Text;

namespace WoFApp
{
    public interface IInput
    {
        string Read();
    }

    public class ConsoleIn : IInput
    {
        public string Read()
        {
            return Console.ReadLine()!;
        }
    }

    public interface IOutput
    {
        void Write(string output);
        void WriteLine(string output);
        void EmptyLine();
        void Clear();
    }

    public class ConsoleOut : IOutput
    {
        public void Write(string output)
        {
            Console.Write(output);
        }
        public void WriteLine(string output)
        {
            Console.WriteLine(output);
        }
        public void EmptyLine()
        {
            Console.WriteLine();
        }
        public void Clear()
        {
            Console.Clear();
        }
    }

    public enum State
    {
        WaitingToStart,
        RoundStarted,
        RoundOver,
        WaitingForUserInput,
        GuessingLetter,
        Solving,
        GameOver
    }

    public class GameController
    {
        public readonly IInput input;
        public readonly IOutput output;
        public string ChallengePhrase;
        public string MaskedPhrase;
        public string PuzzleSolution;
        public List<string> GuessList = new List<string>();
        public State State { get; set; }

        public GameController() : this(new ConsoleIn(), new ConsoleOut())
        {

        }

        public GameController(IInput input, IOutput output)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            this.input = input;
            this.output = output;
            MaskedPhrase = string.Empty;
            ChallengePhrase = string.Empty;
            PuzzleSolution = string.Empty;
            State = State.WaitingToStart;
        }

        public void GameStart()
        {
            Init();

            while (true)
            {
                PerformTurn();

                if (State == State.RoundOver)
                {
                    StartRound();
                    continue;
                }

                if (State == State.GameOver)
                {
                    LineClear(LineType.Prompt, Console.WindowWidth);
                    Console.ForegroundColor = ConsoleColor.Green;
                    output.Write("Congratulations, you've solved the puzzle!! Press any key to exit the game . . .");
                    Console.ResetColor();
                    input.Read();
                    break;
                }
            }
        }

        public void StartRound()
        {
            Random rand = new Random();
            var subjects = new string[]
            {
                "I", "You", "Kim", "Shruthi", "Josh", "Andrea", "People", "We", "They", "Mary"
            };
            var verbs = new string[]
            {
                  "will search for", "will get", "will find", "attained", "found", "will start interacting",
                    "will accept", "accepted", "loved", "will paint"
            };
            var objects = new string[]
            {
                "an offer", "an apple", "a car", "an orange", "a treasure", "a surface", "snow",
                "alligators", "good code", "a dog", "cookies", "foxes", "aubergines", "zebras"
            };
            int r = rand.Next(subjects.Length);
            var randomSubject = subjects[r].ToUpper();
            r = rand.Next(verbs.Length);
            var randomVerb = verbs[r].ToUpper();
            r = rand.Next(objects.Length);
            var randomObject = objects[r].ToUpper();

            ChallengePhrase = $"{randomSubject} {randomVerb} {randomObject}";

            MaskedPhrase = new string(ChallengePhrase.Select(c => c == ' ' ? ' ' : '-').ToArray());

            State = State.RoundStarted;
        }

        public enum LineType
        {
            Puzzle = 16,
            Prompt = 21
        }

        public void LineClear(LineType Line, int Count)
        {
            switch (Line)
            {
                case LineType.Puzzle:
                    Console.SetCursorPosition(0, (int)LineType.Puzzle);
                    output.Write(new string(' ', Count));
                    Console.SetCursorPosition(0, (int)LineType.Puzzle);
                    break;
                case LineType.Prompt:
                    Console.SetCursorPosition(0, (int)LineType.Prompt);
                    output.Write(new string(' ', Count));
                    Console.SetCursorPosition(0, (int)LineType.Prompt);
                    break;
            }
        }

        public void Pause(int ms)
        {
            Thread.Sleep(ms);
        }

        public void ErrorMessage(string errorMsg)
        {
            LineClear(LineType.Prompt, PerformTurnMessage.Length + 1);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            output.Write(errorMsg);
            Pause(250);
            LineClear(LineType.Prompt, errorMsg.Length);
            output.Write($"{errorMsg} .");
            Pause(250);
            LineClear(LineType.Prompt, errorMsg.Length + 2);
            output.Write($"{errorMsg} . .");
            Pause(250);
            LineClear(LineType.Prompt, errorMsg.Length + 4);
            output.Write($"{errorMsg} . . .");
            Pause(250);
            LineClear(LineType.Prompt, errorMsg.Length + 6);
            output.Write($"{errorMsg} . . . .");
            Pause(250);
            Console.ResetColor();
        }

        public string PerformTurnMessage = "Press 1 to spin or 2 to solve: ";

        public void PerformTurn()
        {
            output.Clear();
            RenderPuzzle();
            output.Write(PerformTurnMessage);
            State = State.WaitingForUserInput;

            var Action = input.Read();

            switch (Action)
            {
                case "1":
                    WheelSpin();
                    break;
                case "2":
                    Solve();
                    break;
                default:
                    ErrorMessage("Invalid entry, resetting .");
                    break;
            }
        }

        public void RenderPuzzle()
        {
            int TotalWidth = ChallengePhrase.Length % 2 == 0 ? 48 : 49;
            int Padding = (TotalWidth - ChallengePhrase.Length) / 2;
            string HorizontalBorder = new string('-', TotalWidth + 2);
            GameLogo();
            output.EmptyLine();
            output.EmptyLine();
            output.WriteLine(HorizontalBorder);
            output.WriteLine($"|{new string(' ', TotalWidth)}|");
            output.WriteLine($"|{new string(' ', Padding)}{MaskedPhrase}{new string(' ', Padding)}|");
            output.WriteLine($"|{new string(' ', TotalWidth)}|");
            output.WriteLine(HorizontalBorder);
            output.EmptyLine();
            output.EmptyLine();
        }

        public string WheelSpinMsg = "Spinning the wheel .";

        public void WheelSpin()
        {
            LineClear(LineType.Prompt, PerformTurnMessage.Length + 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            output.Write(WheelSpinMsg);
            Pause(250);
            LineClear(LineType.Prompt, WheelSpinMsg.Length);
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            output.Write($"{WheelSpinMsg} .");
            Pause(250);
            LineClear(LineType.Prompt, WheelSpinMsg.Length + 2);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            output.Write($"{WheelSpinMsg} . .");
            Pause(250);
            LineClear(LineType.Prompt, WheelSpinMsg.Length + 4);
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            output.Write($"{WheelSpinMsg} . . .");
            Pause(250);
            LineClear(LineType.Prompt, WheelSpinMsg.Length + 6);
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            output.Write($"{WheelSpinMsg} . . . .");
            Pause(250);
            Console.ResetColor();
            LetterGuess();
        }

        public void Solve()
        {
            if (string.IsNullOrEmpty(ChallengePhrase))
            {
                LineClear(LineType.Prompt, Console.WindowWidth);
                Console.ForegroundColor = ConsoleColor.Red;
                output.Write("Sorry, but there is no puzzle to solve. Press any key to continue...");
                Console.ResetColor();
                input.Read();
                return;
            }

            LineClear(LineType.Prompt, Console.WindowWidth);
            output.Write("Please enter your solution: ");
            var UserInput = input.Read();
            PuzzleSolution = ChallengePhrase;
            if (string.Equals(UserInput, PuzzleSolution, StringComparison.OrdinalIgnoreCase))
            {
                MaskedPhrase = ChallengePhrase;
                int Width = MaskedPhrase.Length % 2 == 0 ? 48 : 49;
                int Pad = (Width - MaskedPhrase.Length) / 2;
                LineClear(LineType.Puzzle, Console.WindowWidth);
                output.Write($"|{new string(' ', Pad)}{MaskedPhrase}{new string(' ', Pad)}|");
                State = State.GameOver;
            }
            else
            {
                LineClear(LineType.Prompt, Console.WindowWidth);
                Console.ForegroundColor = ConsoleColor.Red;
                output.Write("Sorry, but that is incorrect. Press any key to continue...");
                Console.ResetColor();
                input.Read();
            }
        }

        public void LetterGuess()
        {
            LineClear(LineType.Prompt, WheelSpinMsg.Length + 8);
            output.Write("Please guess a letter: ");
            var UserInput = input.Read();

            if (string.IsNullOrEmpty(UserInput))
            {
                LineClear(LineType.Prompt, Console.WindowWidth);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                output.Write("Invalid entry, must be a valid alphabetical character. Press any key to continue . . .");
                Console.ResetColor();
                input.Read();
                return;
            }

            char Guess = UserInput[0];

            if (!char.IsLetter(Guess))
            {
                LineClear(LineType.Prompt, Console.WindowWidth);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                output.Write("Invalid entry, must be a valid alphabetical character. Press any key to continue . . .");
                Console.ResetColor();
                input.Read();
                return;
            }

            Guess = char.ToUpper(Guess);
            string Letter = Guess.ToString();

            if (GuessList.Contains(Letter))
            {
                LineClear(LineType.Prompt, Console.WindowWidth);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                output.Write($"The letter '{Letter}' has already been guessed. Press any key to continue . . .");
                Console.ResetColor();
                input.Read();
                return;
            }

            GuessList.Add(Letter);

            int LetterCount = 0;
            StringBuilder MaskPhraseUpdate = new StringBuilder(MaskedPhrase);
            if (ChallengePhrase != null)
            {
                for (int i = 0; i < ChallengePhrase.Length; i++)
                {
                    if (ChallengePhrase[i] == Guess)
                    {
                        MaskPhraseUpdate[i] = Guess;
                        LetterCount++;
                    }
                }
            }
            MaskedPhrase = MaskPhraseUpdate.ToString();
            int Width = MaskedPhrase.Length % 2 == 0 ? 48 : 49;
            int Pad = (Width - MaskedPhrase.Length) / 2;
            LineClear(LineType.Puzzle, Console.WindowWidth);
            output.Write($"|{new string(' ', Pad)}{MaskedPhrase}{new string(' ', Pad)}|");

            if (MaskedPhrase.Equals(ChallengePhrase))
            {
                State = State.GameOver;
            }
            else
            {
                if (LetterCount == 0)
                {
                    LineClear(LineType.Prompt, Console.WindowWidth);
                    Console.ForegroundColor = ConsoleColor.Red;
                    output.Write($"Hard luck, '{Guess}' is not in the phrase. Press any key to try again . . .");
                    Console.ResetColor();
                    input.Read();
                    return;
                }

                LineClear(LineType.Prompt, Console.WindowWidth);
                Console.ForegroundColor = ConsoleColor.Green;
                output.Write($"Great job, '{Guess}' is in the phrase {LetterCount} time(s). Press any key to continue . . .");
                Console.ResetColor();
                input.Read();
            }
        }

        public List<int> GetGuessedLetterIndex(string Guess)
        {
            int Start = 0;
            int End = MaskedPhrase?.Length ?? 0;
            int Pos = 0;
            int Count;
            var posList = new List<int>();
            if (MaskedPhrase != null)
            {
                while ((Start <= End) && (Pos > -1))
                {
                    Count = End - Start;
                    Pos = MaskedPhrase.IndexOf(Guess, Start, Count);
                    if (Pos == -1)
                        break;
                    posList.Add(Pos);
                    Start = Pos + 1;
                }
            }
            return posList;
        }

        public void Init()
        {
            output.Clear();
            output.EmptyLine();
            output.WriteLine("Welcome to . . .");
            output.EmptyLine();
            Pause(1500);
            TitleLogo();
            Pause(1500);
            output.EmptyLine();
            output.EmptyLine();
            output.Write("Press any key to start . . .");
            input.Read();
            StartRound();
        }

        public void GameLogo()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            output.WriteLine(" _       _ _                 __       _____   ___");
            Console.ForegroundColor = ConsoleColor.Red;
            output.WriteLine("( )  _  ( ) )               (  )     (  _  )/ ___)");
            output.WriteLine("| | ( ) | | |__    __    __  | |     | ( ) | (__");
            Console.ForegroundColor = ConsoleColor.Yellow;
            output.WriteLine("| | | | | |  _  \\/ __ \\/ __ \\| |     | | | |  __)");
            output.WriteLine("| (_/ \\_) | | | |  ___/  ___/| |     | (_) | |");
            Console.ForegroundColor = ConsoleColor.Green;
            output.WriteLine(" \\__/\\___/(_) (_)\\____)\\____)___)    (_____)_)");
            output.WriteLine("      ___              _");
            output.WriteLine("     (  _ \\           ( )_");
            Console.ForegroundColor = ConsoleColor.Magenta;
            output.WriteLine("     | (_(_)  _   _ __|  _)_   _  ___    __");
            output.WriteLine("     |  _)  / _ \\(  __) | ( ) ( )  _  \\/ __ \\");
            Console.ForegroundColor = ConsoleColor.Blue;
            output.WriteLine("     | |   ( (_) ) |  | |_| (_) | ( ) |  ___/");
            output.WriteLine("     (_)    \\___/(_)   \\__)\\___/(_) (_)\\____)");
            Console.ResetColor();
        }

        public void TitleLogo()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            output.WriteLine(" _       _ _                 __       _____   ___");
            Pause(125);
            Console.ForegroundColor = ConsoleColor.Red;
            output.WriteLine("( )  _  ( ) )               (  )     (  _  )/ ___)");
            output.WriteLine("| | ( ) | | |__    __    __  | |     | ( ) | (__");
            Pause(125);
            Console.ForegroundColor = ConsoleColor.Yellow;
            output.WriteLine("| | | | | |  _  \\/ __ \\/ __ \\| |     | | | |  __)");
            output.WriteLine("| (_/ \\_) | | | |  ___/  ___/| |     | (_) | |");
            Pause(125);
            Console.ForegroundColor = ConsoleColor.Green;
            output.WriteLine(" \\__/\\___/(_) (_)\\____)\\____)___)    (_____)_)");
            output.WriteLine("      ___              _");
            output.WriteLine("     (  _ \\           ( )_");
            Pause(125);
            Console.ForegroundColor = ConsoleColor.Magenta;
            output.WriteLine("     | (_(_)  _   _ __|  _)_   _  ___    __");
            output.WriteLine("     |  _)  / _ \\(  __) | ( ) ( )  _  \\/ __ \\");
            Pause(125);
            Console.ForegroundColor = ConsoleColor.Blue;
            output.WriteLine("     | |   ( (_) ) |  | |_| (_) | ( ) |  ___/");
            output.WriteLine("     (_)    \\___/(_)   \\__)\\___/(_) (_)\\____)");
            Console.ResetColor();
        }
    }
}
