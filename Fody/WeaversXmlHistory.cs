using System;
using System.Collections.Generic;
using System.IO;

public partial class Processor
{
    public static Dictionary<string, DateTime> TimeStamps = new Dictionary<string, DateTime>();

    public virtual bool CheckForWeaversXmlChanged(Configuration configuration)
    {
        var changed = false;
        foreach (var configFile in configuration.ConfigFiles)
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

    public virtual void FlushWeaversXmlHistory(Configuration configuration)
    {
        foreach (var configFile in configuration.ConfigFiles)
        {
            TimeStamps[configFile] = File.GetLastWriteTimeUtc(configFile);
        }
    }
}