using System.Text.RegularExpressions;

namespace FileHandler
{
    public class UserInput
    {
        public string? Response { get; set; }
        public delegate void ListDirCallback(string directory);

        public UserInput()
        {
            // strictly for the GetUserInput method
        }

        // Single prompt -- no options
        public UserInput(string prompt, string directory)
        {
            PromptUser(prompt, directory);
        }

        // Single prompt -- multiple options 
        public UserInput(string prompt, List<string> options, string? directory = "", bool backAllowed = true)
        {
            PromptUserWithOptions(prompt, options, directory, backAllowed);
        }

        public UserInput(
            string? originInput,
            string directory,
            Dictionary<string, List<string>> options,
            ListDirCallback? callback = null
        )
        {
            HandleDirectoryTraversal(originInput, directory, options, callback);
        }


        private void PromptUser(string prompt, string directory)
        {
            //prompt the user
            Console.WriteLine(prompt);
            string? _response = GetUserInput(directory);
            if (_response == null) return;
            Console.Clear();
            Response = _response;
        }

        private void PromptUserWithOptions(string prompt, List<string> options, string? directory = "", bool backAllowed = true)
        {
            //prompt the user
            Console.WriteLine(prompt);
            for (int i = 0; i < options.Count(); i++)
            {
                Console.WriteLine($"{i + 1}) {options[i]}");
            }
            string? _response = GetUserInput(directory: directory);
            if (_response == null) return;
            Console.Clear();
            if (Regex.IsMatch(_response, "back", RegexOptions.IgnoreCase) && backAllowed)
            {
                string? newDir = Path.GetDirectoryName(directory);
                Console.WriteLine($"NEW DIRECTORY SELECTED: {newDir}");
                if (newDir?.Length > 0)
                {
                    DirectoryUtils.ListDirectory(directory: newDir);
                    return;
                }
            }
            if (Regex.IsMatch(_response, "select", RegexOptions.IgnoreCase) && !string.IsNullOrEmpty(directory)) return;
            // Successful: set Response
            if (int.TryParse(_response, out int result) && result <= options.Count && result > 0) Response = _response;
            else
            {
                Console.Clear();
                Console.WriteLine("Sorry, that is not a valid option, please try again.");
                PromptUserWithOptions(prompt, options, directory, backAllowed);
            }
        }

        private void HandleDirectoryTraversal(
            string? originInput,
            string directory,
            Dictionary<string, List<string>> options,
            ListDirCallback? callback = null
        )
        {
            List<string> fileOptions = options.Keys.ToList();
            if (originInput == null || originInput == "")
            {
                Console.WriteLine("What would you like to see?");
                for (int i = 0; i < fileOptions.Count; i++) Console.WriteLine($"{i + 1}) {fileOptions[i]}");
                string? _response = GetUserInput(directory: directory);

                // UserInput filesOrDirs = new UserInput(prompt: "What would you like to see?", options: fileOptions, directory);
                // string? _response = filesOrDirs.Response;
                if (_response == null) return;
                Response = _response;
                Console.Clear();
                HandleDirectoryTraversal(originInput: Response, directory, options, callback);
            }
            else if (int.TryParse(originInput, out int result) && result > 0 && result <= fileOptions.Count)
            {
                List<string> selected = options[fileOptions[result - 1]];

                string prompt = $"\nPlease choose from the following \"{fileOptions[result - 1]}\"\nCurrent Diretory: {directory}\n==> Type \"select\" to perform additional actions on {directory}\n==> Type \"back\" to go back\n";
                UserInput userLocationSelection = new UserInput(prompt: prompt, options: selected, directory: directory);

                if (int.TryParse(userLocationSelection.Response, out int locResult))
                {
                    Console.WriteLine($"You selected: {locResult}");
                    if (locResult > 0 || locResult <= selected.Count)
                    {
                        callback?.Invoke(directory: selected[locResult - 1]);
                    }
                    else
                    {
                        Console.WriteLine("Sorry, that is not a valid option, please try again.");
                        callback?.Invoke(directory: directory);
                    }
                }
                else if (string.IsNullOrEmpty(userLocationSelection.Response)) return;
                else
                {
                    Console.WriteLine("Sorry, that is not a valid option, please try again.");
                    callback?.Invoke(directory: directory);
                }
            }
            else
            {
                Console.WriteLine("Sorry, that is not a valid option, please try again.");
                Console.WriteLine("What would you like to see?\n1) Files\n2) Directories");
            }
        }

        public string? GetUserInput(string? directory = "")
        {
            string input = Console.ReadLine() ?? "";
            if (CheckProgramInerrupt(input: input, directory: directory))
            {
                return input;
            }
            else
            {
                return null;
            }
        }

        private bool CheckProgramInerrupt(string input, string? directory = "")
        {
            Regex exit = new Regex("exit", RegexOptions.IgnoreCase);
            Regex restart = new Regex("restart", RegexOptions.IgnoreCase);
            Regex select = new Regex("select", RegexOptions.IgnoreCase);

            if (exit.IsMatch(input))
            {
                Console.WriteLine("Thanks for letting us help you! See you next time!");
                // Exit program
                Environment.Exit(0);
                return false;
            }
            if (restart.IsMatch(input))
            {
                Console.WriteLine("Firing up the DeLorean! Let's go back... Back to the Future!");
                Console.Clear();
                // Restart program
                Program.Main();
                return false;
            }
            if (select.IsMatch(input))
            {
                Console.Clear();
                Console.WriteLine($"Directory set: {directory}");
                new FileActions(directory: directory ?? "");
                return false;
            }
            return true;

        }
    }
}