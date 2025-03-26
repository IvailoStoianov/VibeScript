# 🌟 VibeScript - A Learning Journey into Language Implementation

This project is my personal exploration into building an interpreter from scratch using C#. It's a fun learning exercise that helped me refresh my C# skills and understand how programming languages work under the hood. If you're interested in language implementation or getting back into C# after a break, feel free to explore and experiment!

## 🎓 Learning Goals

- **Understanding Interpreters**: Hands-on experience with lexing, parsing, and interpretation
- **Refreshing C# Skills**: Getting back into C# after a break from programming
- **Exploring OOP**: Practical application of object-oriented programming principles
- **Having Fun**: Because learning should be enjoyable! 🎉

## 🔍 What is VibeScript?

VibeScript is a simple JavaScript-inspired language I created for learning purposes. It supports basic operations like:
- Variable declarations (using `bet` instead of `let`, and `lockedIn` instead of `const`)
- Objects and nested properties
- Functions (using the `cook` keyword, because why not make it fun?)
- Basic arithmetic operations including remainder
- A simple print function called `vibe()` (similar to console.log)

## 🎮 Try It Out!

Want to experiment with VibeScript? Here's how:

1. Clone the repository
2. Open the solution in Visual Studio
3. Find the `ExampleSourceCode.txt` file in the project root
4. Write your VibeScript code in this file
5. Run the application to see the output!

### Example Code to Try:
```javascript
// Put this in ExampleSourceCode.txt
bet foo = 50 / 2;
 
lockedIn obj = {
   x: 100,
   y: 32,
   foo: foo,
   complex: {
    bar: true,
   },
};

vibe(50+1, foo+1, obj.y)  // Will output: 51 26 32

// Try a function!
cook add(x, y) {
    bet result = x + y;
    result + 5
}

lockedIn result = add(10, 4);
vibe(result)  // Will output: 19
```

## 🏗️ Project Structure

The project is split into two main parts (because that's how I learned interpreters should be structured):

### Frontend
- **Lexer**: Breaks code into tokens
- **Parser**: Builds an AST from tokens
- **AST**: Represents the code structure

### Runtime
- **Interpreter**: Executes the code
- **Runtime Values**: Handles different types of values
- **Environment**: Manages variables and scope

## 📝 Things I'd Like to Add (Eventually)

Just for fun, here are some features I might add when I have time:
- [ ] Strings (currently only have numbers and booleans)
- [ ] If statements
- [ ] Loops
- [ ] Boolean operations (currently only have true/false values)
- [ ] Maybe some error handling that's more helpful than just crashing 😅

## 🤔 Why VibeScript?

This project started as a way to:
1. Get back into C# after focusing on other languages in school
2. Learn how programming languages actually work
3. Have fun with creating something from scratch
4. Practice OOP principles in a real project

## 🚀 Want to Play Around With It?

Feel free to:
- Fork it and add your own features
- Use it to learn about interpreters
- Break it and fix it
- Have fun with it!

Remember: This is a learning project, not a production-ready language. Expect bugs, limitations, and opportunities to learn! 😊

## 📖 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

This project was started by following Tyler Laceby's excellent YouTube series on building an interpreter. While his tutorial uses TypeScript, I translated the concepts to C# and continued building upon them with my own additions. You can find his amazing tutorial series here: [Building an Interpreter](https://www.youtube.com/playlist?list=PL_2VhOvlMk4UHGqYCLWc6GO8FaPl8fQTh)

Special thanks to:
- Tyler Laceby for creating such an informative and easy-to-follow series on interpreter development
