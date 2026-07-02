using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamProctorPlatform.Models;

namespace ExamProctorPlatform.Controllers
{
    [Authorize(Roles = "Student")]
    public class CodingController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var question = new CodingQuestion
            {
                Title = "Reverse a String",

                ProblemStatement =
@"Write a program to reverse a string.

Input:
hello

Output:
olleh

Constraints:

• String length <= 1000

• Use C#",

                StarterCode =
@"using System;

class Program
{
    static void Main()
    {

    }
}"
            };

            return View(question);
        }

        [HttpPost]
        public IActionResult CompileCode(string userCode)
        {
            return Json(new
            {
                success = true,
                message = "Compilation Successful (Demo Mode)"
            });
        }

        [HttpPost]
        public IActionResult SubmitCode(string userCode)
        {
            return Json(new
            {
                success = true,
                message = "Solution Submitted Successfully."
            });
        }
    }
}