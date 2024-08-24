using System;
using System.Xml;
using System.Xml.Schema;

class Program
{
    //remember to change these locations to point at the respective folders once you've cloned the repo to a location on your pc
    private readonly static string xsdDocsFolder = "C:\\Users\\XSD Validator\\XSD Docs\\";
    private readonly static string xsdTestFilesFolder = "C:\\Users\\XSD Validator\\Test Files\\";
    static void Main()
    {
        //get every one of our test XML files to compare to the xsds
        foreach (string xmlFilePath in Directory.GetFiles(xsdTestFilesFolder, "*.xml"))
        {
            bool isValid = ReadXSDFile(xmlFilePath);
            Console.WriteLine($"{Path.GetFileName(xmlFilePath)}: {(isValid ? "Valid" : "Invalid")}");
        }
    }

    /// <summary>
    /// Reads the target XSD file
    /// </summary>
    /// <returns>TRUE if file is valid, FALSE if file is invalid.</returns>
    private static bool ReadXSDFile(string xmlFilePath)
    {
        string xsdMarkup = File.ReadAllText(xsdDocsFolder + "xsdBasic.xsd"); //might need to do something else for different formats/types?

        XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
        xmlReaderSettings.Schemas.Add(null, XmlReader.Create(new StringReader(xsdMarkup)));
        xmlReaderSettings.ValidationType = ValidationType.Schema;

        using (XmlReader xmlReader = XmlReader.Create(xmlFilePath, xmlReaderSettings))
        {
            try
            {
                while (xmlReader.Read()) { } // Read the entire document to trigger validation
                return true; // Valid
            }
            catch (XmlSchemaValidationException ex)
            {
                Console.WriteLine($"Validation error in {Path.GetFileName(xmlFilePath)}: {ex.Message}");
                return false; // Invalid
            }
        }
    }
}
