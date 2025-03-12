using VibeScript.FrontEnd;
using VibeScript.FrontEnd.Interfaces;

namespace VibeScript
{
    internal class Program
    {
        static void Main(string[] args)
        {


            string filePath = "../../../ExampleSourceCode.txt";

            try
            {
                string sourceCode = File.ReadAllText(filePath);
                Lexer lexer = new Lexer();
                List<IToken> tokens = lexer.Tokenize(sourceCode);


                foreach (var token in tokens)
                {
                    Console.WriteLine($"Value: {token.Value}, Type: {token.Type}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading file: " + ex.Message);
            }
        }
    }
}
