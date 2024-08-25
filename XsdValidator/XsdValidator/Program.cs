using System;
using System.Xml;
using System.Xml.Schema;

class Program
{
    //remember to change these locations to point at the respective folders once you've cloned the repo to a location on your pc
    private readonly static string xsdDocsFolder = "C:\\Users\\XSD Validation\\XSD Docs\\";
    private readonly static string xsdTestFilesFolder = "C:\\Users\\XSD Validation\\Test Files\\";
    /// <summary>
    /// This lets us map which XML files need to be checked against which XSD specifications, every xml doc can only be checked against 1 xsd
    /// <br></br> 
    /// It's impossible for an xml file (which has a specific format) to match 2 or more different formats at the same time
    /// </summary>
    private readonly static Dictionary<string, string> mapofXsdsAndXmltypes = new Dictionary<string, string>() { {"XML1", "XSD1.xsd" }, { "XML2", "XSD2.xsd" } };

    static void Main()
    {
        try
        {
            HandleDifferentXsdFormats();
        }
        catch
        {
        }
    }

    /// <summary>
    /// Compares multiple XMLs to their respective XSDs
    /// <br></br>
    /// This allows us to handle different types, rather than having a different method/app for a different xsd/xml type
    /// </summary>
    private static void HandleDifferentXsdFormats()
    {
        try
        {
            //get every one of our test XML files to compare to the xsds
            foreach (string xmlFilePath in Directory.GetFiles(xsdTestFilesFolder, "*.xml"))
            {
                string respectiveXsdFileName = GetXSDFileNameOfFileToCheckAgainst(Path.GetFileName(xmlFilePath));

                if (respectiveXsdFileName == string.Empty)
                {
                    Console.WriteLine($"Could not process: {Path.GetFileName(xmlFilePath)}");
                    continue;
                }

                bool isValid = ReadXSDFile(xmlFilePath, respectiveXsdFileName);
                Console.WriteLine($"{Path.GetFileName(xmlFilePath)}: {(isValid ? "Valid" : "Invalid")}");
            }
        }
        catch
        {
        }
    }
    /// <summary>
    /// Gets the name of the XSD for which this current XML needs to be checked against
    /// </summary>
    /// <param name="inputXmlName"></param>
    /// <returns>The name of the XSD to check the current XML against</returns>
    private static string GetXSDFileNameOfFileToCheckAgainst(string inputXmlName)
    {
        try
        {
            inputXmlName = inputXmlName.ToUpper().Substring(0, 4); //the first 4 chars have to look like "XML1", etc.
            string respectiveXsdFileName = mapofXsdsAndXmltypes[inputXmlName];
            return respectiveXsdFileName;
        }
        catch
        {
            return string.Empty;
        }
    }


    /// <summary>
    /// Reads the target XSD file
    /// </summary>
    /// <returns>TRUE if file is valid, FALSE if file is invalid.</returns>
    private static bool ReadXSDFile(string xmlFilePath, string xsdFileName)
    {
        try
        {
            string xsdMarkup = File.ReadAllText(xsdDocsFolder + xsdFileName); //might need to do something else for different formats/types?

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.Schemas.Add(null, XmlReader.Create(new StringReader(xsdMarkup)));
            xmlReaderSettings.ValidationType = ValidationType.Schema;

            using (XmlReader xmlReader = XmlReader.Create(xmlFilePath, xmlReaderSettings))
            {
                try
                {
                    while (xmlReader.Read()) { } // Read the entire document to trigger validation ; do nothing while reading
                    return true; // Valid
                }
                catch (XmlSchemaValidationException ex)
                {
                    Console.WriteLine($"Validation error in {Path.GetFileName(xmlFilePath)}: {ex.Message}");
                    return false; // Invalid
                }
            }
        }
        catch
        {
            return false;
        }
    }
}
