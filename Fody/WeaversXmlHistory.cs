using System;
using System.Collections.Generic;
using System.IO;

public class WeaversXmlHistory
{
    public ILogger Logger;
    public ProjectWeaversFinder ProjectWeaversFinder;
    public static Dictionary<string, DateTime> TimeStamps;

    static WeaversXmlHistory()
    {
        TimeStamps = new Dictionary<string, DateTime>();
    }

    public bool CheckForChanged()
    {
        var changed = false;
        foreach (var configFile in ProjectWeaversFinder.ConfigFiles)
        {
            var timeStamp = File.GetLastWriteTimeUtc(configFile);
            DateTime dateTime;
            if (TimeStamps.TryGetValue(configFile, out dateTime))
            {
                if (dateTime != timeStamp)
                {
                    Logger.LogWarning(string.Format("A re-build is required to apply the changes from '{0}'.", configFile));
                    changed = true;
                }
            }
            else
            {
                TimeStamps[configFile] = timeStamp;
            }   
        }
        return changed;
    }

    public void Flush()
    {
        foreach (var configFile in ProjectWeaversFinder.ConfigFiles)
        {
            TimeStamps[configFile] = File.GetLastWriteTimeUtc(configFile);
        }
    }
}