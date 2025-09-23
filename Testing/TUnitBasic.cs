#!/usr/bin/env dotnet
#:package TUnit@0.6.*

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


    [Test]
    public async Task TestException()
    {
        await Assert.That(() => DivideByZero(10, 0))
            .ThrowsExceptionOfType<DivideByZeroException>();
    }

    [Test]
    public async Task TestExceptionWithMessage()
    {
        await Assert.That(() => ThrowArgumentException())
            .ThrowsExceptionOfType<ArgumentException>()
            .WithMessage("Invalid argument");
    }

    private int Add(int x, int y) => x + y;

    private int DivideByZero(int x, int y)
    {
        if (y == 0)
            throw new DivideByZeroException();
        return x / y;
    }

    private void ThrowArgumentException()
    {
        throw new ArgumentException("Invalid argument");
    }
}
