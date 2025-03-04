
using System.Text.RegularExpressions;

namespace MultiPlayerLobbyGame.Share.Utills;

public static class StringUtils
{
    public static int[] ExtractPorts(string input)
    {
        // Regular expression to match the port numbers
        string pattern = @":(\d+)";
        Regex regex = new Regex(pattern);

        // Find all matches
        MatchCollection matches = regex.Matches(input);

        // Extract the port numbers and convert them to integers
        int[] ports = new int[matches.Count];
        for (int i = 0; i < matches.Count; i++)
        {
            ports[i] = int.Parse(matches[i].Groups[1].Value);
        }

        return ports;
    }
}
