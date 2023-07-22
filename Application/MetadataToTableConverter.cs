namespace Napper.Application;

public static class MetadataToTableConverter
{

  private static readonly string[] Keys = new[]
  {
    "Score",
    "Nap Count",
    "Nap Hours",
    "Awake Hours",
    "Night Hours",
    "Sleep Hours",
    "06:00",
    "06:30",
    "07:00",
    "07:30",
    "08:00",
    "08:30",
    "09:00",
    "09:30",
    "10:00",
    "10:30",
    "11:00",
    "11:30",
    "12:00",
    "12:30",
    "13:00",
    "13:30",
    "14:00",
    "14:30",
    "15:00",
    "15:30",
    "16:00",
    "16:30",
    "17:00",
    "17:30",
    "18:00",
    "18:30",
    "19:00",
    "19:30",
    "20:00"
  };


  // The first row is "", "Schedule 1", "Schedule 2", ...
  // The second row is "Number of naps", "3", "2","1"
  public static string[][] ConvertToTable(this List<ScheduleDayMetadata> metadatas)
  {

    var result = new string[Keys.Length + 1][];

    // Write header
    result[0] = new string[metadatas.Count + 1];
    result[0][0] = ""; // First cell is empty

    for (int i = 0; i < metadatas.Count; i++)
    {
        result[0][i + 1] = $"Schedule {i + 1}"; // Schedule labels
    }

    // Write keys and values
    for (int i = 0; i < Keys.Length; i++)
    {
        result[i + 1] = new string[metadatas.Count + 1];
        result[i + 1][0] = Keys[i];

        for (int j = 0; j < metadatas.Count; j++)
        {
            string[] values = GetValues(metadatas[j]);
            result[i + 1][j + 1] = values[i];
        }
    }

    return result;
  }

  private static string[] GetValues(ScheduleDayMetadata metadata)
  {
    var result = new string[36];

    result[0] = metadata.Score.ToString();
    result[1] = metadata.NumberOfNaps.ToString();
    result[2] = metadata.NapHours.ToString();
    result[3] = metadata.AwakeHours.ToString();
    result[4] = metadata.NightHours.ToString();
    result[5] = metadata.TotalSleepHours.ToString();

    Array.Copy(metadata.ActivitiesIn30MinuteSpans, 0, result, 6, metadata.ActivitiesIn30MinuteSpans.Length);

    return result;
  }
}