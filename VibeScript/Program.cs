using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VibeScript.FrontEnd;
using VibeScript.FrontEnd.Interfaces;
using VibeScript.FrontEnd.Parser;
using VibeScript.RunTime;
using VibeScript.RunTime.Environment;
using VibeScript.RunTime.Values;

namespace VibeScript
{
    internal class Program
    {
        //TODO: Add strings
        //TODO: Add If's
        //TODO: Add loops
        static void Main(string[] args)
        {

            string content = File.ReadAllText("../../../ExampleSourceCode.txt");

            var parser = new Parser();   
            // Create the global environment first
            var globalEnv = new RuntimeEnvironment().CreateGlobalEnv();
            // Create program environment with global as parent
            var env = new RuntimeEnvironment(globalEnv);
            var interpreter = new Interpreter();

            var program = parser.ProduceAST(content);

            var result = interpreter.Evaluate(program, env);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };

            //Console.WriteLine(JsonConvert.SerializeObject(result, settings));

            //while (true)
            //{
            //    string input = Console.ReadLine();
            //    if (input == string.Empty || input == "exit")
            //    {
            //        return;
            //    }

            //    var program = parser.ProduceAST(input);

            //    var result = interpreter.Evaluate(program, env);

            //    Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            //}


        }
    }
}
