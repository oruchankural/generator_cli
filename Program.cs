using CommandLine;
namespace generator_cli;

class Program
{
    public class Options
    {
        [Option("service-name", Required = true, HelpText = "Name of the service interface")]
        public string ServiceName { get; set; }
    }
    static int Main(string[] args)
    {
        var result = Parser.Default.ParseArguments<Options>(args);

        return result.MapResult(
            (Options opts) => RunWithOptions(opts),
            errs => 1);
    }
    static int RunWithOptions(Options options)
    {
        string projectNamespace = GetProjectNamespace();
        GenerateServiceFile(projectNamespace, options.ServiceName);
        return 0;
    }
    static string GetProjectNamespace()
    {
        // Assuming the project file is in the root directory
        string projectFilePath = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj")[0];
        string projectContent = File.ReadAllText(projectFilePath);

        // Extracting the default namespace from the project file
        string namespaceTag = "<RootNamespace>";
        int start = projectContent.IndexOf(namespaceTag) + namespaceTag.Length;
        int end = projectContent.IndexOf("</RootNamespace>", start);
        string projectNamespace = projectContent.Substring(start, end - start).Trim();

        return projectNamespace;
    }
    static void GenerateServiceFile(string projectNamespace, string serviceName)
    {
        // Path to Contracts/Services directory
        string directoryPath = Path.Combine("Contracts", "Services");

        // Create the directory if it doesn't exist
        Directory.CreateDirectory(directoryPath);

        // Create a folder with the service name
        string serviceFolder = Path.Combine(directoryPath, serviceName);
        Directory.CreateDirectory(serviceFolder);

        // Path to the service interface file
        string readInterfaceFilePath = Path.Combine(serviceFolder, $"I{serviceName}ReadService.cs");
        string writeInterfaceFilePath = Path.Combine(serviceFolder, $"I{serviceName}WriteService.cs");

        // Check if the files already exist
        if (File.Exists(readInterfaceFilePath) || File.Exists(writeInterfaceFilePath))
        {
            Console.WriteLine($"Error: Interfaces for {serviceName} already exist.");
        }
        else
        {
            // Create the files and write interfaces
            File.WriteAllText(readInterfaceFilePath, GenerateServiceFileContent(projectNamespace, $"I{serviceName}ReadService"));
            File.WriteAllText(writeInterfaceFilePath, GenerateServiceFileContent(projectNamespace, $"I{serviceName}WriteService"));

            Console.WriteLine($"Interfaces for {serviceName} created successfully in {serviceName} folder.");
        }
    }
    static string GenerateServiceFileContent(string projectNamespace, string serviceName)
    {
        return $@"namespace {projectNamespace}.Contracts.Services;
        {{
            public interface I{serviceName}
            {{
                // Add read service methods here
            }}
        }}";
    }
}
