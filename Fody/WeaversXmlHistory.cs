using System;
using System.Collections.Generic;
using System.IO;

public partial class Processor
{
    public static Dictionary<string, DateTime> TimeStamps = new Dictionary<string, DateTime>();

    public virtual bool CheckForWeaversXmlChanged()
    {
        var changed = false;
        foreach (var configFile in ConfigFiles)
        {
            var timeStamp = File.GetLastWriteTimeUtc(configFile);
            DateTime dateTime;
            if (TimeStamps.TryGetValue(configFile, out dateTime))
            {
                if (dateTime != timeStamp)
                {
                    Logger.LogError($"A re-build is required to apply the changes from '{configFile}'.");
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

    public virtual void FlushWeaversXmlHistory()
    {
        foreach (var configFile in ConfigFiles)
        {
            TimeStamps[configFile] = File.GetLastWriteTimeUtc(configFile);
        }
    }
}