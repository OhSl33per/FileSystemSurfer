using System.Text.RegularExpressions;

namespace FileHandler
{
    class FileActions
    {
        private string? Selection_String { get; set; }
        private int? Selection_Index { get; set; }
        public FileActions(string directory)
        {
            List<string> initActions = [
                "View File Types",
                "View File Names",
                "Search Files",
                "Search Directories",
            ];

            string prompt = $"What would you like to do in {directory}?";
            UserInput userInput = new UserInput(prompt: prompt, options: initActions);
            if (int.TryParse(userInput.Response, out int result) && result > 0 && result <= initActions.Count)
            {
                Selection_Index = result - 1;
                switch (Selection_Index)
                {
                    case 0:
                        ViewFileTypes(directory);
                        break;
                    default:
                        Console.WriteLine("Selection out of Range");
                        new FileActions(directory);
                        return;
                }
            }
            else
            {
                Selection_String = userInput.Response;
                Console.WriteLine($"Response from user END: {userInput.Response}");
            }
            Console.WriteLine($"Response from user END: {userInput.Response}");

        }

        private void ViewFileTypes(string directory)
        {
            List<string> files = Directory.GetFiles(directory).ToList();
            List<string> extenstions = files
                .Select(file => Path.GetExtension(file))
                // filter out nulls & extensions that don't apply
                .Where(ext => !string.IsNullOrEmpty(ext) && ext.Length <= 4)
                .Distinct()
                .ToList();
            Console.WriteLine($"The following file types were found in {directory}");
            foreach (string type in extenstions)
            {
                Console.WriteLine(type);
            }
        }
    }
}