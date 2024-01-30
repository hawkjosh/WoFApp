using System.Text;

namespace WoFApp
{
    public interface IInputProvider
    {
        string Read();
    }

    public class ConsoleInputProvider : IInputProvider
    {
        public string Read()
        {
            return Console.ReadLine()!;
        }
    }

    public interface IOutputProvider
    {
        void Write(string output);
        void WriteLine(string output);
        void WriteLine();
        void Clear();
    }

    public class ConsoleOutputProvider : IOutputProvider
    {
        public void Write(string output)
        {
            Console.Write(output);
        }
        public void WriteLine(string output)
        {
            Console.WriteLine(output);
        }
        public void WriteLine()
        {
            Console.WriteLine();
        }
        public void Clear()
        {
            Console.Clear();
        }
    }

    public enum GameState
    {
        WaitingToStart,
        RoundStarted,
        RoundOver,
        WaitingForUserInput,
        GuessingLetter,
        Solving,
        GameOver
    }

    public class GameManager
    {
        private readonly IInputProvider inputProvider;
        private readonly IOutputProvider outputProvider;
        public string ChallengePhrase;
        private string MaskedPhrase;
        private string PuzzleSolution;
        public List<string> CharGuessList = new List<string>();
        public GameState GameState { get; private set; }

        public GameManager() : this(new ConsoleInputProvider(), new ConsoleOutputProvider())
        {

        }

        public GameManager(IInputProvider inputProvider, IOutputProvider outputProvider)
        {
            if (inputProvider == null)
                throw new ArgumentNullException(nameof(inputProvider));
            if (outputProvider == null)
                throw new ArgumentNullException(nameof(outputProvider));
            this.inputProvider = inputProvider;
            this.outputProvider = outputProvider;
            MaskedPhrase = string.Empty;
            ChallengePhrase = string.Empty;
            PuzzleSolution = string.Empty;
            GameState = GameState.WaitingToStart;
        }

        public void StartGame()
        {
            InitGame();

            while (true)
            {
                PerformSingleTurn();

                if (GameState == GameState.RoundOver)
                {
                    StartNewRound();
                    continue;
                }

                if (GameState == GameState.GameOver)
                {
                    ClearLine("promptLine", Console.WindowWidth);
                    Console.ForegroundColor = ConsoleColor.Green;
                    outputProvider.Write("Congratulations, you've solved the puzzle!! Press any key to exit the game . . .");
                    Console.ResetColor();
                    inputProvider.Read();
                    break;
                }
            }
        }

        public void StartNewRound()
        {
            Random rnd = new Random();
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
            int r = rnd.Next(subjects.Length);
            var randomSubject = subjects[r].ToUpper();
            r = rnd.Next(verbs.Length);
            var randomVerb = verbs[r].ToUpper();
            r = rnd.Next(objects.Length);
            var randomObject = objects[r].ToUpper();

            ChallengePhrase = $"{randomSubject} {randomVerb} {randomObject}";

            MaskedPhrase = new string(ChallengePhrase.Select(c => c == ' ' ? ' ' : '-').ToArray());

            GameState = GameState.RoundStarted;
        }



        public void ClearLine(string line, int prevText)
        {
            int puzzleLineNumber = 15;
            int promptLineNumber = 17;
            switch (line)
            {
                case "puzzleLine":
                    Console.SetCursorPosition(0, puzzleLineNumber);
                    outputProvider.Write(new string(' ', prevText));
                    Console.SetCursorPosition(0, puzzleLineNumber);
                    break;
                case "promptLine":
                    Console.SetCursorPosition(0, promptLineNumber);
                    outputProvider.Write(new string(' ', prevText));
                    Console.SetCursorPosition(0, promptLineNumber);
                    break;
            }
        }

        public void ShowErrorMessage(string errorMessage)
        {
            ClearLine("promptLine", PerformSingleTurnMessage.Length + 1);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            outputProvider.Write(errorMessage);
            Thread.Sleep(250);
            ClearLine("promptLine", errorMessage.Length);
            outputProvider.Write($"{errorMessage} .");
            Thread.Sleep(250);
            ClearLine("promptLine", errorMessage.Length + 2);
            outputProvider.Write($"{errorMessage} . .");
            Thread.Sleep(250);
            ClearLine("promptLine", errorMessage.Length + 4);
            outputProvider.Write($"{errorMessage} . . .");
            Thread.Sleep(250);
            ClearLine("promptLine", errorMessage.Length + 6);
            outputProvider.Write($"{errorMessage} . . . .");
            Thread.Sleep(250);
            Console.ResetColor();
        }

        public string PerformSingleTurnMessage = "Press 1 to spin or 2 to solve: ";

        public void PerformSingleTurn()
        {
            outputProvider.Clear();
            DrawPuzzle();
            outputProvider.Write(PerformSingleTurnMessage);
            GameState = GameState.WaitingForUserInput;

            var UserAction = inputProvider.Read();

            switch (UserAction)
            {
                case "1":
                    Spin();
                    break;
                case "2":
                    Solve();
                    break;
                default:
                    ShowErrorMessage("Invalid entry, resetting .");
                    break;
            }
        }

        private void DrawPuzzle()
        {
            ShowLogo();
            outputProvider.WriteLine();
            outputProvider.WriteLine("The puzzle is:");
            outputProvider.WriteLine();
            outputProvider.WriteLine(MaskedPhrase);
            outputProvider.WriteLine();

            //// Below is for dev purposes only - shows the solved puzzle
            //Console.ForegroundColor = ConsoleColor.Cyan;
            //outputProvider.WriteLine("| ~ ~ FOR DEV USE ~ ~");
            //outputProvider.WriteLine("|");
            //outputProvider.Write("| Solution: ");
            //outputProvider.WriteLine($" {ChallengePhrase} ");
            //outputProvider.WriteLine("|");
            //outputProvider.WriteLine("| ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");
            //Console.ResetColor();
            //outputProvider.WriteLine();
        }

        public string SpinningWheelMessage = "Spinning the wheel .";

        public void Spin()
        {
            ClearLine("promptLine", PerformSingleTurnMessage.Length + 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            outputProvider.Write(SpinningWheelMessage);
            Thread.Sleep(250);
            ClearLine("promptLine", SpinningWheelMessage.Length);
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            outputProvider.Write($"{SpinningWheelMessage} .");
            Thread.Sleep(250);
            ClearLine("promptLine", SpinningWheelMessage.Length + 2);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            outputProvider.Write($"{SpinningWheelMessage} . .");
            Thread.Sleep(250);
            ClearLine("promptLine", SpinningWheelMessage.Length + 4);
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            outputProvider.Write($"{SpinningWheelMessage} . . .");
            Thread.Sleep(250);
            ClearLine("promptLine", SpinningWheelMessage.Length + 6);
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            outputProvider.Write($"{SpinningWheelMessage} . . . .");
            Thread.Sleep(250);
            Console.ResetColor();
            GuessLetter();
        }

        public void Solve()
        {
            if (string.IsNullOrEmpty(ChallengePhrase))
            {
                ClearLine("promptLine", Console.WindowWidth);
                Console.ForegroundColor = ConsoleColor.Red;
                outputProvider.Write("Sorry, but there is no puzzle to solve. Press any key to continue...");
                Console.ResetColor();
                inputProvider.Read();
                return;
            }

            ClearLine("promptLine", Console.WindowWidth);
            outputProvider.Write("Please enter your solution: ");
            var UserInput = inputProvider.Read();
            PuzzleSolution = ChallengePhrase;
            if (string.Equals(UserInput, PuzzleSolution, StringComparison.OrdinalIgnoreCase))
            {
                MaskedPhrase = ChallengePhrase;
                ClearLine("puzzleLine", MaskedPhrase.Length);
                outputProvider.Write(MaskedPhrase);
                GameState = GameState.GameOver;
            }
            else
            {
                ClearLine("promptLine", Console.WindowWidth);
                Console.ForegroundColor = ConsoleColor.Red;
                outputProvider.Write("Sorry, but that is incorrect. Press any key to continue...");
                Console.ResetColor();
                inputProvider.Read();
            }
        }

        public void GuessLetter()
        {
            ClearLine("promptLine", SpinningWheelMessage.Length + 8);
            outputProvider.Write("Please guess a letter: ");
            var UserInput = inputProvider.Read();

            if (string.IsNullOrEmpty(UserInput))
            {
                ClearLine("promptLine", Console.WindowWidth);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                outputProvider.Write("Invalid entry, must be a valid alphabetical character. Press any key to continue . . .");
                Console.ResetColor();
                inputProvider.Read();
                return;
            }

            char Guess = UserInput[0];

            if (!char.IsLetter(Guess))
            {
                ClearLine("promptLine", Console.WindowWidth);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                outputProvider.Write("Invalid entry, must be a valid alphabetical character. Press any key to continue . . .");
                Console.ResetColor();
                inputProvider.Read();
                return;
            }

            Guess = char.ToUpper(Guess);
            string GuessString = Guess.ToString();

            if (CharGuessList.Contains(GuessString))
            {
                ClearLine("promptLine", Console.WindowWidth);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                outputProvider.Write($"The letter '{Guess}' has already been guessed. Press any key to continue . . .");
                Console.ResetColor();
                inputProvider.Read();
                return;
            }

            CharGuessList.Add(GuessString);

            int count = 0;
            StringBuilder MaskPhraseUpdate = new StringBuilder(MaskedPhrase);
            if (ChallengePhrase != null)
            {
                for (int i = 0; i < ChallengePhrase.Length; i++)
                {
                    if (ChallengePhrase[i] == Guess)
                    {
                        MaskPhraseUpdate[i] = Guess;
                        count++;
                    }
                }
            }
            MaskedPhrase = MaskPhraseUpdate.ToString();
            ClearLine("puzzleLine", MaskedPhrase.Length);
            outputProvider.Write(MaskedPhrase);

            if (MaskedPhrase.Equals(ChallengePhrase))
            {
                GameState = GameState.GameOver;
            }
            else
            {
                if (count == 0)
                {
                    ClearLine("promptLine", Console.WindowWidth);
                    Console.ForegroundColor = ConsoleColor.Red;
                    outputProvider.Write($"Hard luck, '{Guess}' is not in the phrase. Press any key to try again . . .");
                    Console.ResetColor();
                    inputProvider.Read();
                    return;
                }

                ClearLine("promptLine", Console.WindowWidth);
                Console.ForegroundColor = ConsoleColor.Green;
                outputProvider.Write($"Great job, '{Guess}' is in the phrase {count} time(s). Press any key to continue . . .");
                Console.ResetColor();
                inputProvider.Read();
            }
        }

        private List<int> GetAllIndicesOfGuessedLetterInChallengePhrase(string guess)
        {
            int start = 0;
            int end = MaskedPhrase?.Length ?? 0;
            int at = 0;
            int count;
            var posList = new List<int>();
            if (MaskedPhrase != null)
            {
                while ((start <= end) && (at > -1))
                {
                    count = end - start;
                    at = MaskedPhrase.IndexOf(guess, start, count);
                    if (at == -1)
                        break;
                    posList.Add(at);
                    start = at + 1;
                }
            }
            return posList;
        }

        public void InitGame()
        {
            outputProvider.Clear();
            outputProvider.WriteLine();
            outputProvider.WriteLine("Welcome to . . .");
            outputProvider.WriteLine();
            Thread.Sleep(1500);
            ShowLogo();
            Thread.Sleep(1500);
            outputProvider.WriteLine();
            outputProvider.WriteLine();
            outputProvider.Write("Press any key to start . . .");
            inputProvider.Read();
            StartNewRound();
        }

        public void ShowLogo()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            outputProvider.WriteLine(" _       _ _                 __       _____   ___");
            Console.ForegroundColor = ConsoleColor.Red;
            outputProvider.WriteLine("( )  _  ( ) )               (  )     (  _  )/ ___)");
            outputProvider.WriteLine("| | ( ) | | |__    __    __  | |     | ( ) | (__");
            Console.ForegroundColor = ConsoleColor.Yellow;
            outputProvider.WriteLine("| | | | | |  _  \\/ __ \\/ __ \\| |     | | | |  __)");
            outputProvider.WriteLine("| (_/ \\_) | | | |  ___/  ___/| |     | (_) | |");
            Console.ForegroundColor = ConsoleColor.Green;
            outputProvider.WriteLine(" \\__/\\___/(_) (_)\\____)\\____)___)    (_____)_)");
            outputProvider.WriteLine("      ___              _");
            outputProvider.WriteLine("     (  _ \\           ( )_");
            Console.ForegroundColor = ConsoleColor.Magenta;
            outputProvider.WriteLine("     | (_(_)  _   _ __|  _)_   _  ___    __");
            outputProvider.WriteLine("     |  _)  / _ \\(  __) | ( ) ( )  _  \\/ __ \\");
            Console.ForegroundColor = ConsoleColor.Blue;
            outputProvider.WriteLine("     | |   ( (_) ) |  | |_| (_) | ( ) |  ___/");
            outputProvider.WriteLine("     (_)    \\___/(_)   \\__)\\___/(_) (_)\\____)");
            Console.ResetColor();
        }
    }
}
