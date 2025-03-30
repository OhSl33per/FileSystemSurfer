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

            return SelectedDrive >= 0 ? Drives[SelectedDrive] : null;
        }

        private static void DisplayDrives()
        {
            Console.WriteLine("Drives Found:");

            for (int i = 0; i < Drives?.Count; i++)
            {
                DriveInfo drive = Drives[i];
                Console.WriteLine($"{i + 1}) Name: {drive.Name} Label: {drive.VolumeLabel}");
            }
        }

        private static void AskDrive()
        {

            Console.WriteLine("Please choose the number for the drive you'd like to explore!");

            while (true)
            {
                string? _response = new UserInput().GetUserInput();
                if (_response == null) return;
                var selectedDrive = _response;
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
    }
}