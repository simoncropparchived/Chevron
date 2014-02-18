using System;
using NUnit.Framework;

[TestFixture]
public class VariableNameValidatorTest
{
    [Test]
    public void Simple()
    {
        VariableNameValidator.ValidateSuffix("a2fa");
        VariableNameValidator.ValidateSuffix("2");
        VariableNameValidator.ValidateSuffix("_");
        VariableNameValidator.ValidateSuffix("a");
        VariableNameValidator.ValidateSuffix("$");
        Assert.Throws<Exception>(() => VariableNameValidator.ValidateSuffix("@"));
        Assert.Throws<Exception>(() => VariableNameValidator.ValidateSuffix("/"));
        Assert.Throws<Exception>(() => VariableNameValidator.ValidateSuffix(@"\"));
        Assert.Throws<Exception>(() => VariableNameValidator.ValidateSuffix("%"));
    }
}