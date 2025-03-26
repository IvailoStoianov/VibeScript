using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VibeScript.FrontEnd;
using VibeScript.FrontEnd.Parser;
using VibeScript.RunTime;
using VibeScript.RunTime.Environment;
using VibeScript.RunTime.Values;

namespace VibeScript
{
    internal class Program
    {
        // Future enhancements:
        // TODO: Add strings
        // TODO: Add if statements
        // TODO: Add loops
        static void Main(string[] args)
        {
            string content = File.ReadAllText("../../../ExampleSourceCode.txt");

            // Initialize the interpreter components
            var parser = new Parser();   
            var globalEnv = new RuntimeEnvironment().CreateGlobalEnv();
            var env = new RuntimeEnvironment(globalEnv);
            var interpreter = new Interpreter();

            // Parse and interpret the program
            var program = parser.ProduceAST(content);
            interpreter.Evaluate(program, env);

            // Uncomment for interactive REPL mode
            /*
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input) || input == "exit")
                {
                    break;
                }

                try
                {
                    var repl = parser.ProduceAST(input);
                    var result = interpreter.Evaluate(repl, env);
                    Console.WriteLine(JsonConvert.SerializeObject(result, new JsonSerializerSettings { 
                        Formatting = Formatting.Indented,
                        Converters = new List<JsonConverter> { new RuntimeValueJsonConverter() }
                    }));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            */
        }
    }
}
