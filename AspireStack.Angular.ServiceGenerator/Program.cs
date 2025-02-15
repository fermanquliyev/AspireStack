using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag;
using NSwag.CodeGeneration.TypeScript;
using System.Text.RegularExpressions;

internal class Program
{
    private static async Task Main(string[] args)
    {
        string swaggerUrl = Environment.GetEnvironmentVariable("services__AspireStackApi__https__0") + "/swagger/v1/swagger.json"; // Adjust the URL
        string outputPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "AspireStack.Angular", "src", "app", "services", "api-services", "api-service-proxies.ts");

        using var httpClient = new HttpClient();
        string swaggerJson = await httpClient.GetStringAsync(swaggerUrl);

        var document = await OpenApiDocument.FromJsonAsync(swaggerJson);
        var settings = new TypeScriptClientGeneratorSettings
        {
            Template = TypeScriptTemplate.Angular,
            HttpClass = HttpClass.HttpClient,
            UseSingletonProvider = true,
            GenerateClientInterfaces = true,
            GenerateOptionalParameters = true,
            RxJsVersion = 7.8M,
            InjectionTokenType = InjectionTokenType.InjectionToken
        };

        settings.TypeScriptGeneratorSettings.TypeScriptVersion = 5.5M;
        settings.ClassName = "ApiService";

        var generator = new TypeScriptClientGenerator(document, settings);
        string code = generator.GenerateFile();

        // Fix the generated code
        code = code.Replace("import { Injectable, Inject, Optional, InjectionToken } from '@angular/core';", "import { Injectable, Inject, Optional, InjectionToken, Injector } from '@angular/core';");
        code = code.Replace("@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string", "injector: Injector");
        code = code.Replace("this.http = http;", "this.http = injector.get(HttpClient);");
        code = code.Replace("this.baseUrl = baseUrl ?? \"\";", "this.baseUrl = injector.get(API_BASE_URL, '');");
        code = Regex.Replace(code,
            @"encodeURIComponent\("""" *\+ *(.*?)\)",
            "encodeURIComponent(\"\" + JSON.stringify($1))");

        await File.WriteAllTextAsync(outputPath, code);
        Console.WriteLine("✅ Angular API service proxies generated successfully!");
        Console.WriteLine($"📄 Output: {outputPath}");
        Console.WriteLine(code);
    }
}
