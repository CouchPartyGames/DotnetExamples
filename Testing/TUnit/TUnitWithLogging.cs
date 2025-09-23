#!/usr/bin/env dotnet
#:package TUnit@0.6.*

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public class TUnitWithLogging
{
    // Method 1: Console output - most reliable approach (TUnit automatically captures this)
    [Test]
    public async Task MyTest_WithConsole()
    {
        int someValue = 0;
        int expectedValue = 0;
        
        Console.WriteLine("This is debug information");
        Console.Error.WriteLine("This is an error message");
        Console.WriteLine($"Testing values: someValue={someValue}, expectedValue={expectedValue}");
        
        // Your test logic here
        await Assert.That(someValue).IsEqualTo(expectedValue);
    }
    
    // Method 2: Debug and Trace output (captured by TUnit)
    [Test]
    public async Task MyTest_WithDebugTrace()
    {
        int someValue = 0;
        int expectedValue = 0;
        
        System.Diagnostics.Debug.WriteLine("This is debug information");
        System.Diagnostics.Trace.WriteLine("This is trace information");
        
        // Your test logic here
        await Assert.That(someValue).IsEqualTo(expectedValue);
    }
    
    // Method 3: Simple lifecycle hooks with console output
    [Before(Test)]
    public void BeforeTest()
    {
        Console.WriteLine("Starting a test...");
    }
    
    [After(Test)]
    public void AfterTest()
    {
        Console.WriteLine("Test completed.");
    }
    
    [Test]
    public async Task MyTest_WithLifecycleLogging()
    {
        int someValue = 0;
        int expectedValue = 0;
        
        Console.WriteLine("Inside test method - performing assertions");
        
        // Your test logic here
        await Assert.That(someValue).IsEqualTo(expectedValue);
    }
    
    // Method 4: Using TestContext only for what's confirmed to work
    [Before(Test)]
    public void BeforeTestWithContext(TestContext context)
    {
        Console.WriteLine("BeforeTest called with TestContext");
        // Only use basic TestContext functionality that's guaranteed to exist
    }
    
    [After(Test)] 
    public void AfterTestWithContext(TestContext context)
    {
        Console.WriteLine("AfterTest called with TestContext");
        // Basic context usage without trying to access specific properties
    }
    
    [Test]
    public async Task MyTest_WithBasicContext()
    {
        int someValue = 0;
        int expectedValue = 0;
        
        // Access TestContext statically (this should work based on search results)
        var currentContext = TestContext.Current;
        Console.WriteLine("TestContext.Current is available");
        
        // Your test logic here
        await Assert.That(someValue).IsEqualTo(expectedValue);
    }
}