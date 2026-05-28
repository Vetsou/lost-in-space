namespace LostInSpace.Tests;

using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
public class CalculatorTest
{
	[TestCase]
	public void Add() => AssertThat(1 + 2).IsEqual(3);
}