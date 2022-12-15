//using NUnit.Framework;

using Xunit;

namespace Application.IntegrationTests;

//using static Testing;

//[TestFixture]
public abstract class BaseTestFixture : IClassFixture<Testing> {
  protected BaseTestFixture() {
    //ResetState();
  }

  ////[SetUp]
  //public async Task TestSetUp() {
  //  await ResetState();
  //}
}