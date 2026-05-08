using System.IO;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");
var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");

Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

// Generate and write the detailed sales summary report
var salesReport = GenerateSalesReport(salesFiles);
File.WriteAllText(Path.Combine(salesTotalDir, "SalesSummary.txt"), salesReport);

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();
    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);
    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }
    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;
    
    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);
        
        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        
        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }
    
    return salesTotal;
}

string GenerateSalesReport(IEnumerable<string> salesFiles)
{
    StringBuilder report = new StringBuilder();
    double reportTotal = 0;
    
    // Build the header
    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");
    
    // Build the details section
    List<(string fileName, double total)> fileTotals = new List<(string, double)>();
    
    foreach (var file in salesFiles)
    {
        // Read and parse each file
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        double fileTotal = data?.Total ?? 0;
        
        // Get just the filename for display
        string fileName = Path.GetFileName(file);
        fileTotals.Add((fileName, fileTotal));
        reportTotal += fileTotal;
    }
    
    // Write the total sales
    report.AppendLine($" Total Sales: {reportTotal:C}");
    report.AppendLine();
    report.AppendLine(" Details:");
    
    // Write details for each file
    foreach (var (fileName, total) in fileTotals)
    {
        report.AppendLine($"  {fileName}: {total:C}");
    }
    
    return report.ToString();
}

record SalesData (double Total);