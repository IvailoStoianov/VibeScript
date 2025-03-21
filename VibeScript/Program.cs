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
        static void Main(string[] args)
        {

            string content = File.ReadAllText("../../../ExampleSourceCode.txt");

            var parser = new Parser();   
            var env = new RuntimeEnvironment();
            var interpreter = new Interpreter();

            //Create Default Global Environment
            env = env.CreateGlobalEnv();

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
