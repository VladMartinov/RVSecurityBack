using Bogus;
using Core.Entities;

namespace Tests.Integration.MockData;

public static class MockUser
{
    public static IEnumerable<User> GenNewUsers(int count)
    {
        var f = new Faker<User>(Mock.Locale)
            .RuleFor(x => x.UserName, f => f.Person.UserName)
            .RuleFor(x => x.TwoFactorEnabled, f => f.Random.Bool())
            .RuleFor(x => x.LockoutEnd, f => f.Random.Int(0, 100) > 50 ? null : DateTime.UtcNow.AddDays(2));
        return f.Generate(count);
    }
}