using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Pluralize.NET;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Swagger;

public class ApplySummariesOperationFilter : IOperationFilter {

  public void Apply(OpenApiOperation operation, OperationFilterContext context) {
    if (context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor) return;

    var pluralizer = new Pluralizer();

    var actionName = controllerActionDescriptor.ActionName;
    var singularizeName = pluralizer.Singularize(controllerActionDescriptor.ControllerName);
    var pluralizeName = pluralizer.Pluralize(singularizeName);

    var parameterCount = operation.Parameters.Where(p => p.Name != "version" && p.Name != "api-version").Count();
    var summary = "";

    if (IsGetAllAction()) {
      summary = $"Returns the list of all  {pluralizeName}";
    } else if (IsActionName("Post", "Create")) {
      summary = $"Create {singularizeName}";

      if (operation.Parameters.Count > 0 && !operation.Parameters[0].Description.HasValue())
        operation.Parameters[0].Description = $"A {singularizeName} representation";
    } else if (IsActionName("Read", "Get")) {
      summary = $"Retrieve {singularizeName} by unique id";

      if (operation.Parameters.Count > 0 && !operation.Parameters[0].Description.HasValue())
        operation.Parameters[0].Description = $"a unique id for {singularizeName}";
    } else if (IsActionName("Put", "Edit", "Update")) {
      summary = $"Update {singularizeName} by unique id";

      //if (!operation.Parameters[0].Description.HasValue())
      //    operation.Parameters[0].Description = $"A unique id for {singularizeName}";

      if (operation.Parameters.Count > 0 && !operation.Parameters[0].Description.HasValue())
        operation.Parameters[0].Description = $"A {singularizeName} representation";
    } else if (IsActionName("Delete", "Remove")) {
      summary = $"Delete {singularizeName} by unique id";

      if (operation.Parameters.Count > 0 && !operation.Parameters[0].Description.HasValue())
        operation.Parameters[0].Description = $"A unique id for {singularizeName}";
    } else {
      summary = $"{actionName} {singularizeName}";
    }

    if (!operation.Summary.HasValue())
      operation.Summary = summary;

    #region Local Functions
    bool IsGetAllAction() {
      foreach (var name in new[] { "Get", "Read", "Select" }) {
        if (actionName.Equals(name, StringComparison.OrdinalIgnoreCase) && parameterCount == 0 ||
            actionName.Equals($"{name}All", StringComparison.OrdinalIgnoreCase) ||
            actionName.Equals($"{name}{pluralizeName}", StringComparison.OrdinalIgnoreCase) ||
            actionName.Equals($"{name}All{singularizeName}", StringComparison.OrdinalIgnoreCase) ||
            actionName.Equals($"{name}All{pluralizeName}", StringComparison.OrdinalIgnoreCase)) {
          return true;
        }
      }
      return false;
    }

    bool IsActionName(params string[] names) {
      foreach (var name in names) {
        if (actionName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
            actionName.Equals($"{name}ById", StringComparison.OrdinalIgnoreCase) ||
            actionName.Equals($"{name}{singularizeName}", StringComparison.OrdinalIgnoreCase) ||
            actionName.Equals($"{name}{singularizeName}ById", StringComparison.OrdinalIgnoreCase)) {
          return true;
        }
      }
      return false;
    }
    #endregion
  }
}

public static class StringExtensions {
  public static bool HasValue(this string value, bool ignoreWhiteSpace = true) {
    return ignoreWhiteSpace ? !string.IsNullOrWhiteSpace(value) : !string.IsNullOrEmpty(value);
  }
  public static bool IsHttpMethod(this ApiDescription apiDescription, params HttpMethod[] methods) {
    return methods.Any(x => x.Method == apiDescription.HttpMethod);
  }
}