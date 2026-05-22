using NUnit.Framework;
using SecureApp.Utilities;

namespace SecureApp.Tests.Tests;

[TestFixture]
public class TestInputValidation
{
    [Test]
    public void TestForSQLInjection()
    {
        var result = UserInputValidator.ValidateNewUser("admin' --", "user@example.com");

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.EqualTo("Input contains blocked patterns."));
    }

    [Test]
    public void TestForXSS()
    {
        var result = UserInputValidator.ValidateNewUser("<script>alert(1)</script>", "user@example.com");

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.EqualTo("Username contains invalid characters."));
        Assert.That(result.Errors, Has.Some.EqualTo("Input contains blocked patterns."));
    }

    [Test]
    public void TestValidInput()
    {
        var result = UserInputValidator.ValidateNewUser("john.doe", "john@example.com");

        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }
}