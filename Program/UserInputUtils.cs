
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FileHandler
{
    public class UserInput
    {
        public string? Response { get; set; }
        public delegate void ListDirCallback(string directory);
        public string? SelectedDirectory { get; set; }

        // Single prompt -- no options
        public UserInput(string prompt)
        {
            PromptUser(prompt);
        }

        // Single prompt -- multiple options 
        public UserInput(string prompt, List<string> options, string? directory = "")
        {
            PromptUserWithOptions(prompt, options, directory);
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


        private void PromptUser(string prompt)
        {
            //prompt the user
            Console.WriteLine(prompt);
            string _response = Console.ReadLine() ?? "";
            Console.Clear();
            if (CheckProgramInerrupt(input: _response))
            {
                Response = _response;
            }
        }

        private void PromptUserWithOptions(string prompt, List<string> options, string? directory = "")
        {
            //prompt the user
            Console.WriteLine(prompt);
            for (int i = 0; i < options.Count(); i++)
            {
                Console.WriteLine($"{i + 1}) {options[i]}");
            }
            string _response = Console.ReadLine() ?? "";
            Console.Clear();
            if (int.TryParse(_response, out int result))
            {
                Response = _response;
            }
            CheckProgramInerrupt(input: _response, directory: directory);
            Response = _response;
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
                Response = Console.ReadLine();
                Console.Clear();
                HandleDirectoryTraversal(originInput: Response, directory, options, callback);
            }
            else if (int.TryParse(originInput, out int result) && result > 0 && result <= fileOptions.Count)
            {
                List<string> selected = options[fileOptions[result - 1]];

                UserInput userLocationSelection = new UserInput(prompt: $"\nPlease choose from the following \"{fileOptions[result - 1]}\"\nCurrent Diretory: {directory}\n==> Type \"select\" to perform additional actions on {directory}\n", options: selected, directory: directory);

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
                else if (Regex.IsMatch(userLocationSelection.Response ?? "", "select", RegexOptions.IgnoreCase))
                {
                    new FileActions(directory);
                    return;
                }
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
                // Restart program
                Program.Main();
                return false;
            }
            if (select.IsMatch(input))
            {
                SelectedDirectory = directory;
                Console.WriteLine($"Directory set: {directory}");
                return false;
            }
            return true;

        }
    }
}