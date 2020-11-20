/*

Here's how to get the "Don't Panic in less than 200 chars" achievement:

First complete the practice version of this puzzle, in a golf-friendly language, like JS, Python, Ruby, etc.

On submitting your solution you can view other solutions.

Learn from some short solutions posted by other codingamers.

I started with the JS solution from pewpewpew. All credits go to him.

The solution clocked at 236 chars, so I added some optimisations on top of it.

Here are some pointers:

First you must minimize the readline().split(' ') redundancy. Instead of using this definition r=function(){return readline().split(' ')}; you can use r=x=>readline().split(" ");

In a for-loop repeating n times and that you don't need to make use of the index, use this shorthand for(i=n;i--;)

Use for(;;) instead of while(1)

Instead of checking, x[2]!="RIGHT", you can shorten it to x[2][0]!="R"

Remove all line breaks and the last semicolon. This will bring you to 199 chars.

Final note: Another way to achieve crazy golf code is to hard-code around the test cases.

*/
