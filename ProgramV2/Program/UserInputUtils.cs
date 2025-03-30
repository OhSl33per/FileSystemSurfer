namespace FileHandler
{
    public class UserInputUtils
    {
        public static string PromptUser(string prompt)
        {
            Console.WriteLine(prompt);
            string response = Console.ReadLine() ?? "";
            if (string.IsNullOrEmpty(response)) return PromptUser(prompt);
            return response;
        }
    }
}