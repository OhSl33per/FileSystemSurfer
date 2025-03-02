
using MediaHandler;

namespace FileHandler
{
    class FileActions
    {
        private string? Selection_String { get; set; }
        private int Selection_Index { get; set; }
        private string DirectoryPath { get; set; }
        private List<string> FileList { get; set; }

        public FileActions(string directory)
        {
            DirectoryPath = directory;
            FileList = DirectoryUtils.GetFiles(directory);

            List<FileActionMethodsModel> actionMethods = new List<FileActionMethodsModel>
            {
                new FileActionMethodsModel
                {
                    Name = "View File Types",
                    Method = () => ViewFileTypes()
                },
                new FileActionMethodsModel
                {
                    Name = "View File Names",
                    Method = () => ViewFileNames()
                },
                new FileActionMethodsModel
                {
                    Name = "Search Files",
                    Method = () => SearchFiles()
                },
                new FileActionMethodsModel {
                    Name = "Perform Actions on a Single File",
                    Method = async () => await SelectSingleFile()
                },
                new FileActionMethodsModel
                {
                    Name = "[BACK TO DIRECTORY SEARCH]",
                    Method = () => DirectoryUtils.ListDirectory(DirectoryPath)
                }
            };

            string prompt = $"What would you like to do in {DirectoryPath}?";
            UserInput userInput = new UserInput(prompt: prompt, options: actionMethods.Select(a => a.Name).ToList());
            if (int.TryParse(userInput.Response, out int result) && result > 0 && result <= actionMethods.Count)
            {
                Selection_Index = result - 1;
                actionMethods[Selection_Index].Method();
            }
            else
            {
                Selection_String = userInput.Response;
                Console.WriteLine($"Response from user END: {userInput.Response}");
            }
            Console.WriteLine($"Response from user END: {userInput.Response}");

        }

        private void ViewFileTypes()
        {
            List<string> files = Directory.GetFiles(DirectoryPath).ToList();
            List<string> extenstions = files
                .Select(file => Path.GetExtension(file))
                // filter out nulls & extensions that don't apply
                .Where(ext => !string.IsNullOrEmpty(ext) && ext.Length <= 4)
                .Distinct()
                .ToList();
            Console.WriteLine($"The following file types were found in {DirectoryPath}");
            foreach (string type in extenstions)
            {
                Console.WriteLine(type);
            }
            Console.WriteLine("\n");
            new FileActions(DirectoryPath);
        }

        private void ViewFileNames()
        {
            List<string> files = Directory.GetFiles(DirectoryPath)
                .Select(f => Path.GetFileName(f))
                .ToList();

            foreach (var file in files)
            {
                Console.WriteLine(file);
            }

            Console.WriteLine("\n");
            new FileActions(DirectoryPath);
        }

        private void SearchFiles()
        {
            string prompt = $"What kind of search would you like to perform in {DirectoryPath}?";
            List<string> options = [
                "Pre-defined Pattern Search",
                "Custom Search (literal)",
                "Custom Search (pattern - RegEx)"
            ];
            UserInput searchType = new UserInput(prompt, options, directory: DirectoryPath, backAllowed: false);
            int searchOption;
            if (int.TryParse(searchType.Response, out int result)) searchOption = result - 1;
            else searchOption = 2;

            // create more detailed descriptions for user later??

            Console.Clear();
            bool searchRes = DirectoryUtils.SearchDirectory(directory: DirectoryPath, searchOption);
            if (searchRes) new FileActions(DirectoryPath);
        }

        private async Task SelectSingleFile()
        {
            string prompt = "What file would you like to perform actions on?";
            string action = new UserInput(prompt, options: FileList).Response ?? "";
            string selectedFile;
            if (int.TryParse(action, out int result))
            {
                selectedFile = FileList[result - 1];
                // new User input here for actions options
                var movieApiSearch = new MovieAPISearch();
                Dictionary<string, string>? comparison = await movieApiSearch.SingleSearch(selectedFile).ConfigureAwait(false);
                Console.WriteLine($"Origin Name: {comparison?["origin"]} ==> Suggested Name: {comparison?["result"]}");
            }
            else
            {
                Console.WriteLine("It didn't work out");
            }
            Console.WriteLine("It didn't work out");
        }
    }

    class FileActionMethodsModel
    {
        required public string Name { get; set; }
        required public Action Method { get; set; }

    }
}