#!/usr/bin/env dotnet
#:package TUnit@0.25.21

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace MyTestProject;

public class MyTestClass
{
    [Test]
    public async Task MyTest()
    {
        var result = Add(1, 2);

        await Assert.That(result).IsEqualTo(3);
    }

	[Test]
	[Arguments(1, 1, 2)]
	[Arguments(2, 3, 5)]
	public async Task MyTestWithParameter(int a, int b, int expected) 
	{
        var result = Add(a, b);

        await Assert.That(result).IsEqualTo(expected);
	}


    private int Add(int x, int y)
    {
        return x + y;
    }
}
