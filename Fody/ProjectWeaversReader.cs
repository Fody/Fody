using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

public partial class Processor
{
    public List<WeaverEntry> Weavers;

    public virtual void ReadProjectWeavers(Configuration configuration)
    {
        if (Weavers != null)
        {
            return;
        }

        Weavers = new List<WeaverEntry>();

        foreach (var configFile in configuration.ConfigFiles)
        {
            var xDocument = GetDocument(configFile);
            foreach (var element in xDocument.Root.Elements())
            {
                var assemblyName = element.Name.LocalName;
                var existing = Weavers.FirstOrDefault(x => string.Equals(x.AssemblyName, assemblyName,StringComparison.OrdinalIgnoreCase));
                var index = Weavers.Count;
                if (existing != null)
                {
                    index = Weavers.IndexOf(existing);
                    Weavers.Remove(existing);
                }
                var weaverEntry = new WeaverEntry
                                      {
                                          Element = element.ToString(SaveOptions.OmitDuplicateNamespaces), 
                                          AssemblyName = assemblyName
                                      };
                Weavers.Insert(index, weaverEntry);
            }
        }
    }

    static XDocument GetDocument(string configFilePath)
    {
        try
        {
            return XDocument.Load(configFilePath);
        }
        catch (XmlException exception)
        {
            throw new WeavingException(string.Format("Could not read '{0}' because it has invalid xml. Message: '{1}'.", "FodyWeavers.xml", exception.Message));
        }
    }
}