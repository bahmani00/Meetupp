using Application.Activities;
using Application.Common.Exceptions;
using FluentAssertions;
using Xunit;

namespace Application.IntegrationTests.Activities;

using static Testing;

public class DeleteActivityTests : BaseTestFixture {
  [Fact]
  public async Task ShouldRequireValidTodoItemId() {
    var command = new Delete.Command(Guid.Empty);

    await FluentActions.Invoking(() =>
        SendAsync(command)).Should().ThrowAsync<RestException>();
  }

  //[Fact]
  //public async Task ShouldDeleteTodoItem() {
  //  var listId = await SendAsync(new CreateTodoListCommand {
  //    Title = "New List"
  //  });

  //  var itemId = await SendAsync(new CreateTodoItemCommand {
  //    ListId = listId,
  //    Title = "New Item"
  //  });

  //  await SendAsync(new DeleteTodoItemCommand(itemId));

  //  var item = await FindAsync<TodoItem>(itemId);

  //  item.Should().BeNull();
  //}
}