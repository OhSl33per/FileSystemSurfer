using FileHandler;

class Program
{
    public static void Main()
    {
        Console.WriteLine("Sup Sup! Let's get started!\n");
        Thread.Sleep(2000);
        Console.WriteLine("You may restart this application at any time by typing \"restart\", \nand exit this application at any time by typing \"exit\" at a prompt\n");
        Thread.Sleep(2000);

        DriveInfo? selectedDrive = DirectoryUtils.GetDrives();

        if (selectedDrive != null)
        {
            Console.WriteLine($"\nSelected Drive info:\nDirectory: {selectedDrive.Name}\nLabel: {selectedDrive.VolumeLabel}\nFree Space: {selectedDrive.TotalFreeSpace}\nFormat: {selectedDrive.DriveFormat}\nType: {selectedDrive.DriveType}\n\n");

            Console.WriteLine($"Great! Now let's find the directory you'd like to manage in {selectedDrive.VolumeLabel}");

            DirectoryUtils.ListDirectory(selectedDrive.Name);
        }
    }

}