
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.Runtime.Loader;
using ExamProctorPlatform.Models;

namespace ExamProctorPlatform.Controllers
{
    [Authorize(Roles = "Student")]
    public class CodingController : Controller
    {
        private readonly Data.AppDbContext _context;

        // Constructor injection agar aap future mein direct DB query scaling karna chahein
        public CodingController(Data.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Initializing C# Boilerplate starter template structure with advanced dynamic tracking metadata
            var question = new CodingQuestion
            {
                Id = 2,
                Title = "System Challenge: Add Two Numbers in C#",
                ProblemStatement = "Create a public method named 'Add' inside a class named 'Solution' that accepts two integers and returns their sum arithmetic validation score.",
                StarterCode = "using System;\n\npublic class Solution\n{\n    public int Add(int a, int b)\n    {\n        // Write C# execution logic here\n        return 0;\n    }\n}",
                TestCasesJson = "[{\"InputA\":7,\"InputB\":8,\"Expected\":\"15\"},{\"InputA\":10,\"InputB\":20,\"Expected\":\"30\"},{\"InputA\":100,\"InputB\":200,\"Expected\":\"300\"}]"
            };

            return View(question);
        }


        [HttpPost]
        public IActionResult CompileCode(string userCode)
        {
            try
            {
                // 1. DYNAMIC REVOLUTION: Hardcoded lists ke badle direct database se question aur test cases load karna
                // Hum check kar rahe hain agar DB mein koi real saved question meta aur JSON matrix data package maujood hai
                var targetQuestion = _context.CodingQuestions.FirstOrDefault(q => q.Id == 2);

                List<TestCaseBlueprint> activeTestCases;

                if (targetQuestion != null && !string.IsNullOrEmpty(targetQuestion.TestCasesJson))
                {
                    // Agar database mein dynamically configuration saved hai, toh wahan se deserialize karein
                    activeTestCases = System.Text.Json.JsonSerializer.Deserialize<List<TestCaseBlueprint>>(targetQuestion.TestCasesJson)
                                      ?? new List<TestCaseBlueprint>();
                }
                else
                {
                    // Failsafe Fallback Engine backup matrix if db row context is missing during live presentations
                    activeTestCases = new List<TestCaseBlueprint>
            {
                new TestCaseBlueprint { InputA = 7, InputB = 8, Expected = "15" },
                new TestCaseBlueprint { InputA = 10, InputB = 20, Expected = "30" },
                new TestCaseBlueprint { InputA = 100, InputB = 200, Expected = "300" }
            };
                }

                // 2. Standard Roslyn Parsing Layers Node Pipeline
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(userCode);
                var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location)!, "System.Runtime.dll"))
        };

                string assemblyName = Path.GetRandomFileName();
                CSharpCompilation compilation = CSharpCompilation.Create(
                    assemblyName,
                    syntaxTrees: new[] { syntaxTree },
                    references: references,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );

                using (var ms = new MemoryStream())
                {
                    EmitResult result = compilation.Emit(ms);
                    if (!result.Success)
                    {
                        var failures = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);
                        return Json(new { success = false, message = "Compilation Failure: " + failures.First().GetMessage() });
                    }

                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    var type = assembly.GetType("Solution");
                    if (type == null) return Json(new { success = false, message = "Class 'Solution' schema declaration layer not found." });

                    var instance = Activator.CreateInstance(type);
                    var method = type.GetMethod("Add");

                    // 3. Automated Test Bench Loop Runner Engine Mapping
                    int totalPassed = 0;
                    for (int i = 0; i < activeTestCases.Count; i++)
                    {
                        var currentCase = activeTestCases[i];
                        var output = method!.Invoke(instance, new object[] { currentCase.InputA, currentCase.InputB });

                        if (output != null && output.ToString() == currentCase.Expected)
                        {
                            totalPassed++;
                        }
                        else
                        {
                            return Json(new { success = false, message = $"Failed at Database Matrix Test Bench {i + 1}! Inputs: ({currentCase.InputA}, {currentCase.InputB}). Expected: {currentCase.Expected}, Got: {(output ?? "null")}" });
                        }
                    }

                    return Json(new { success = true, message = $"Elite Major Project Badge! All {totalPassed}/{activeTestCases.Count} Database Dynamic Test Cases Passed Successfully!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Runtime Execution Interruption Engine Exception: " + ex.Message });
            }
        }
    }
}