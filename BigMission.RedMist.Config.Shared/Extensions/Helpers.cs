namespace BigMission.RedMist.Config.Shared.Extensions;

internal static class Helpers
{
    /// <summary>
    /// Finds the next available ID that is not in the list of existing IDs starting at 1.
    /// </summary>
    /// <param name="existingIds"></param>
    /// <returns></returns>
    /// <exception cref="Exception">when out of IDs</exception>
    public static int NextId(this IEnumerable<int> existingIds)
    {
        for (int i = 1; i < int.MaxValue; i++)
        {
            if (!existingIds.Contains(i))
            {
                return i;
            }
        }
        throw new Exception("Out of available IDs");
    }
}
