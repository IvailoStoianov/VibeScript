﻿using Newtonsoft.Json;
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
            var parser = new Parser();   
            var env = new RuntimeEnvironment();
            env.DeclareVar("x", new NumberValue(100.0f));
            env.DeclareVar("true", new BooleanValue());
            env.DeclareVar("false", new BooleanValue(false));
            env.DeclareVar("nah", new NullValue());
            var interpreter = new Interpreter();
            while (true)
            {
                string input = Console.ReadLine();
                if (input == string.Empty || input == "exit")
                {
                    return;
                }

                var program = parser.ProduceAST(input);

                var result = interpreter.Evaluate(program, env);

                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            }

           
        }
    }
}
