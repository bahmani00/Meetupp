using Application.Activities;
using Application.Common.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Application.IntegrationTests.Activities;

using static Testing;

public class CreateTests : BaseTestFixture {
  //[Fact(Timeout = 100)]
  [Fact]
  public async Task ShouldRequireAllFields() {
    var command = new Create.Command { Title = null!, Description = null!, Date = DateTime.MinValue, Category = null!, City = null!, Venue = null! };

    var result = await FluentActions.Invoking(() => SendAsync(command))
      .Should()
      .ThrowAsync<RestException>();
    //.WithMessage("?did*");
    //.Result;

    result.And.Code.Should().Be(StatusCodes.Status400BadRequest);

    var errors = result.And.Errors.As<Dictionary<string, string[]>>();
    errors.Count.Should().Be(6);
    errors.ContainsKey(nameof(Create.Command.Title)).Should().Be(true);
    errors.ContainsKey(nameof(Create.Command.Description)).Should().Be(true);
    errors.ContainsKey(nameof(Create.Command.Date)).Should().Be(true);
    errors.ContainsKey(nameof(Create.Command.Category)).Should().Be(true);
    errors.ContainsKey(nameof(Create.Command.City)).Should().Be(true);
    errors.ContainsKey(nameof(Create.Command.Venue)).Should().Be(true);
  }

  [Theory]
  [MemberData(nameof(InvalidCreateCommands.TestData), MemberType = typeof(InvalidCreateCommands))]
  public async Task ShouldRequireFields(Create.Command command, string errorKey) {
    var result = await FluentActions.Invoking(() => SendAsync(command))
      .Should()
      .ThrowAsync<RestException>();

    result.And.Code.Should().Be(StatusCodes.Status400BadRequest);

    var errors = result.And.Errors.As<Dictionary<string, string[]>>();
    errors.Count.Should().Be(1);
    errors.ContainsKey(errorKey).Should().Be(true);
  }

  [Fact]
  public async Task ShouldCreateActivity() {
    var userId = await RunAsDefaultUserAsync();
    var command = new Create.Command {
      Title = "Title",
      Description = "Description",
      Date = DateTime.Now.AddDays(1),
      Category = "Category",
      City = "City",
      Venue = "Venue"
    };

    var model = await SendAsync(command);

    //var model = await FindAsync<Activity>(model.Id);

    model.Should().NotBeNull();
    model.Id.Should().NotBe(Guid.Empty);
    model.Title.Should().Be(command.Title);
    model.Description.Should().Be(command.Description);
    model.Date.Should().Be(command.Date);
    model.Category.Should().Be(command.Category);
    model.City.Should().Be(command.City);
    model.Venue.Should().Be(command.Venue);

    model.UserActivities.Should().NotBeNull();
    model.UserActivities.Count().Should().Be(1);
    model.UserActivities.First().UserId.Should().Be(userId);
    //activity.UserActivities.First().CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    //activity.UserActivities.First().ModifiedBy.Should().BeNull();
    //activity.UserActivities.First().ModifiedOn.Should().BeCloseTo(DateTime.Minimum, TimeSpan.FromMilliseconds(10000));
    model.UserActivities.First().IsHost.Should().Be(true);
  }
}

public static class InvalidCreateCommands {
  private static readonly List<object[]> _data = new() {
    new object[] { new Create.Command {
      Title = null!, Description = "desc", Date = DateTime.Now.AddDays(1), Category = "cat", City = "city", Venue = "ven"
      }, "Title"
    },
    new object[] { new Create.Command {
      Title = "title", Description = "", Date = DateTime.Now.AddDays(1), Category = "cat", City = "city", Venue = "ven"
      }, "Description"
    },
    new object[] { new Create.Command {
      Title = "title", Description = "desc", Date = DateTime.Now, Category = "cat", City = "city", Venue = "ven"
      }, "Date"
    },
    new object[] { new Create.Command {
      Title = "title", Description = "desc", Date = DateTime.Now.AddDays(1), Category = "", City = "city", Venue = "ven"
      }, "Category"
    },
    new object[] { new Create.Command {
      Title = "title", Description = "desc", Date = DateTime.Now.AddDays(1), Category = "cat", City = "", Venue = "ven"
      }, "City"
    },
    new object[] { new Create.Command {
      Title = "title", Description = "desc", Date = DateTime.Now.AddDays(1), Category = "cat", City = "city", Venue = null!
      }, "Venue"
    },
  };

  public static IEnumerable<object[]> TestData {
    get { return _data; }
  }
}