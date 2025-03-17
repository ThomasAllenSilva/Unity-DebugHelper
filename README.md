# Unity-DebugHelper

##### Table of Contents  
[Summary](#summary)

[How It Works](#how-it-works)

[How To Use](#how-to-use)

# Summary
When developing an application, you may create logic that should never fail. When it does, we call it an 'Exception.' You've probably faced countless times the most common one: "NullReferenceException".

When an exception occurs in Unity, the typical behavior is that it stops execution in the scope where the exception occurred. You should NEVER ignore an exception, as that means your logic failed and you will catch a bug soon or later.

After reading this, you may think that exceptions are evil, but they are not. In fact, they exist to assist you when a bug occurs. Of course, you don't want to encounter exceptions every time your program runs, but you shouldn't fear them or avoid them in a defensive way. In my experience, defensive programming is used incorrectly the majority of the time.

Take a look at this piece of code:


<img width="407" alt="early return" src="https://github.com/user-attachments/assets/76bf73dd-a369-4ea0-b182-9f6fe2e8b28f" />


This is the simplest example I could think of where Defensive Programming is used the wrong way. Why is it wrong? Well, think about the two possibilities here:

1 - Player Rigidbody Is Not Null

2 - Player Rigidbody Is Null

In the first case, the player will jump as expected, and the code will run without any problems.

In the second case, things become more problematic. If we try to jump, not only will the character fail to jump, but you'll also have no clue why it failed. Was the input not recognized? Was the Rigidbody not assigned? Was the force so low that we didn’t even notice the character jumping? We have no idea! You may find yourself in an even more complicated situation if you have many of these null checks spread across your codebase. Of course, this was a simple example, but you've probably encountered more complex situations where defensive programming was used incorrectly in a similar way.

What should we do instead? The answer is simple: let it throw an exception.

If your logic (in this case, the jump method) cannot work without a valid Rigidbody, why perform a null check and return early? Just to avoid an error message in the console? BAD!

Okay, so now we know how exceptions work and why defensive programming, when used the wrong way, can be a headache.

This plugin combines defensive programming and exceptions. We use asserts to ensure that objects/params are correctly configured before proceeding with execution. Inside the DebugHelper class, you'll find tons of asserts that you can use in your everyday work. You can use asserts not only for checking nullability but for pretty much anything.

Now you might be thinking: "Okay, but why should I use something like DebugHelper.Assert_IsNotNull instead of a regular if (null) throw exception?". Glad you asked! Now I'm gonna explain how this plugin works and why you should use it.

# How It Works
1 - Scripting Symbol

Every method is defined with the scripting symbol 'DEBUG_HELPER.' What does this mean? It means that you can enable or disable all your asserts throughout the project by simply adding or removing the symbol in your project settings. This is extremely helpful for release builds, where you don’t want the asserts running anymore and don’t want to dig deep into your code to remove them. If you need to re-enable them later, you can simply define the symbol again, and all asserts in your project will be recompiled.

More information about Scripting Symbol and Conditional Compilation here:

https://docs.unity3d.com/6000.0/Documentation/Manual/custom-scripting-symbols.html

https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.conditionalattribute?view=net-9.0

2 - Assert before even playing the game, avoiding waste of time

If your project has hundreds of prefabs or Scriptable Objects and you want to ensure they’re all correctly configured for gameplay, how would you do that? Of course, you could search through each one and check every component on these objects, but tbh, that’s a waste of time, especially in production. What this plugin allows you to do is write your asserts inside your scripts, and they will run even if it's a prefab or Scriptable Object. If an assertion fails, it will tell you which object failed, indicating the path. You can simply click the error message, and it will highlight the object in the assets folder. And of course, it will explain why it failed.

3 - Assert scene objects

If you want to assert scene objects, you can do that as well. This component runs the asserts during Awake/Start, and optionally, you can rerun the asserts every time the hierarchy changes (e.g., when a new object is added or removed). However, this option is only available in the editor.

Let’s take a look at how our jump method would look using asserts:

<img width="811" alt="jump with asserts" src="https://github.com/user-attachments/assets/d3ad0054-7d82-4fc6-951f-6a8e83fe23dd" />


Here, we are not only checking if it’s null, but we’re also providing documentation to the code. Anyone who joins the project will see this assertion, the error message, and will understand for sure that the Jump method can’t be executed without a valid Rigidbody reference. Again, this is a simple example, but the same idea can be applied to anything, your custom components, AI, systems inside your game, etc.


This plugin also offers a more visual way of reading these asserts. When an assertion fails, not only will an error message appear in the console, but a GUI window will also display the following information: the error message, the stack trace, and the currently active scenes.

This is great not only for programmers, but also for designers, QA, or anyone working on the project who might encounter a failed assertion. They can simply hit the 'Copy Error Message' button and, for example, create a new task to fix the bug that occurred. When the programmer works on this fix, they will already have the error message, the scene where it happened, and the stack trace. This speed up bug fixing a lot.

<img width="918" alt="error message window" src="https://github.com/user-attachments/assets/c14dad8d-16ae-4904-be94-d15900c4f17d" />


# How To Use

You can use the asserts anywhere you'd like. But if you want to take advantage of scene asserts and also prefab and Scriptable Object asserts, you'll need to take a few steps:

1 - Implement *IAssertableObject* interface

2 - Assert your object inside the *AssertObject* method.

That's it. It should look something like this:


<img width="827" alt="assertable" src="https://github.com/user-attachments/assets/15d9d4dd-4d01-4680-9db6-a53cbc891e48" />


Almost every assertion method has a contextObject parameter. What is that? It’s used to highlight the object where the exception occurred. Imagine you have 50 enemies in your scene, and one assert fails inside the enemy script. How would you know which enemy failed? That’s the purpose of the contextObject. Once you click the error message, it will highlight the object in the hierarchy where the exception occurred.

<img width="956" alt="console" src="https://github.com/user-attachments/assets/898770a3-0bac-4585-ad6c-890cce72e1b6" />



# Scripting Symbol

Now that you have your object configured to be asserted, you must define the scripting symbol 'DEBUG_HELPER' in the project settings so that the methods get compiled:

<img width="558" alt="symbol" src="https://github.com/user-attachments/assets/9ac1c583-4e47-4881-a2cc-61fe473486f1" />


# Object Assertion Settings:

These settings are used to define which assets should be asserted and where they're located

<img width="560" alt="settings" src="https://github.com/user-attachments/assets/856f82c3-b243-4ea4-bc7c-a8e89d4894f3" />


# Scene Assertion Runner:

This component is required in order to run your scene objects assertions.

To create = Right Click in the hierarchy -> Debug -> Scene Assertion Runner

With this component you can define Runtime Settings and also run common scene asserts when entering play mode.


<img width="558" alt="assertion runner" src="https://github.com/user-attachments/assets/946ef57f-570f-4121-a1c3-a33bfe273f3c" />


That's pretty much it. It's very easy to use and implement in your project. You can also implement this interface and run assertions in POCOs.


