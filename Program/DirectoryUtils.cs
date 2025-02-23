
namespace FileHandler
{
    public class DirectoryUtils
    {
        private static int SelectedDrive { get; set; } = 0;

        private static List<DriveInfo>? Drives { get; set; }

        public static DriveInfo? GetDrives()
        {
            Drives = DriveInfo
                .GetDrives()
                .Where(d => d.IsReady && (d.DriveType == DriveType.Fixed || d.DriveType == DriveType.Network))
                .ToList();

            DisplayDrives();
            AskDrive();

            return SelectedDrive > 0 ? Drives[SelectedDrive] : null;
        }

        private static void DisplayDrives()
        {
            Console.WriteLine("Here are the drives we've found:");

            for (int i = 0; i < Drives?.Count; i++)
            {
                DriveInfo drive = Drives[i];
                Console.WriteLine($"{i + 1}) Name: {drive.Name} Label: {drive.VolumeLabel}");
            }
        }

        private static void AskDrive(bool subsequent = false)
        {

            Console.WriteLine("Please choose the number for the drive you'd like to explore!");

            while (true)
            {
                var selectedDrive = Console.ReadLine();

                if (int.TryParse(selectedDrive, out int result) && result > 0 && result <= Drives?.Count())
                {
                    SelectedDrive = result - 1;
                    break;
                }
                else
                {
                    Console.WriteLine("That is not a valid selection, please try again");
                }

            }
        }

        public static void ListDirectory(string directory)
        {
            UserInput type = new UserInput(prompt: $"What would you like to see in {directory}?\n1) Files\n2) Directories");

            string? userTypeSelection = type.Response;

            var options = new Dictionary<string, List<string>>();

            var dirs = Directory.EnumerateDirectories(directory).ToList();
            var files = Directory.EnumerateFiles(directory).ToList();
            options.Add("Files", files);
            options.Add("Directories", dirs);

            UserInput traverseSystem = new UserInput(
                originInput: userTypeSelection,
                directory: directory,
                options: options,
                ListDirectory
            );
        }
    }
}