using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Swagger;

public class UnauthorizedResponsesOperationFilter : IOperationFilter {
  private readonly bool includeUnauthorizedAndForbiddenResponses;
  private readonly string schemeName;

  public UnauthorizedResponsesOperationFilter(bool includeUnauthorizedAndForbiddenResponses, string schemeName = "Bearer") {
    this.includeUnauthorizedAndForbiddenResponses = includeUnauthorizedAndForbiddenResponses;
    this.schemeName = schemeName;
  }

  public void Apply(OpenApiOperation operation, OperationFilterContext context) {
    var filters = context.ApiDescription.ActionDescriptor.FilterDescriptors;
    var metadta = context.ApiDescription.ActionDescriptor.EndpointMetadata;

    //if (operation.Parameters.Any(p => p.Name.EndsWith("id", StringComparison.InvariantCultureIgnoreCase)))
    //  operation.Responses.TryAdd("400", new OpenApiResponse { Description = "BadRequest" });

    //if (context.ApiDescription.IsHttpMethod(HttpMethod.Delete)) {
    //  operation.Responses.Remove("200");
    //  operation.Responses.TryAdd("204", new OpenApiResponse { Description = "NoContent" });
    //} else if (context.ApiDescription.IsHttpMethod(HttpMethod.Post)) {
    //  operation.Responses.Remove("200");
    //  operation.Responses.TryAdd("201", new OpenApiResponse { Description = "Created" });
    //} else {
    //  operation.Responses.TryAdd("200", new OpenApiResponse { Description = "Success" });
    //}

    var hasAnonymous = filters.Any(p => p.Filter is AllowAnonymousFilter) || metadta.Any(p => p is AllowAnonymousAttribute);
    if (hasAnonymous) return;

    var hasAuthorize = filters.Any(p => p.Filter is AuthorizeFilter) || metadta.Any(p => p is AuthorizeAttribute);
    if (!hasAuthorize) return;

    //if (includeUnauthorizedAndForbiddenResponses) {
    //  operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });

    //  if (filters.Count(p => (p.Filter as AuthorizeFilter)?.Policy != null) > 1
    //    || metadta.Count(p => (p as AuthorizeAttribute)?.Policy != null) > 1)
    //    operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
    //}

    //Add Lockout icon on top of swagger ui page to authenticate
    operation.Security.Add(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Scheme = schemeName,
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>() //new[] { "readAccess", "writeAccess" }
        }
    });
  }
}