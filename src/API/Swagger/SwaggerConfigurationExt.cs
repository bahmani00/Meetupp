using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace API.Swagger;

public static class SwaggerConfigurationExt {
  public static void AddSwagger(this IServiceCollection services) {
    //More info : https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters


    //Add services and configuration to use swagger
    services.AddSwaggerGen(options => {
      options.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Meetupp API",
        Version = "v1",
        Description = "An API to perform Meetupp operations"
      });

      // Set the comments path for the Swagger JSON and UI.
      //Add endpoint's comment
      var xmlDocPath = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlDocPath);
      options.IncludeXmlComments(xmlPath);
      //Add model's comment
      xmlDocPath = $"{typeof(Application.Activities.ActivityDto).Assembly.GetName().Name}.xml";
      xmlPath = Path.Combine(AppContext.BaseDirectory, xmlDocPath);
      options.IncludeXmlComments(xmlPath);

      options.EnableAnnotations();
      options.UseInlineDefinitionsForEnums();

      //options.DescribeAllParametersInCamelCase();
      //options.IgnoreObsoleteActions();
      //options.IgnoreObsoleteProperties();

      options.CustomSchemaIds(x => x.ToString().Replace("+", ".", StringComparison.OrdinalIgnoreCase));

      #region Filters
      //Enable to use [SwaggerRequestExample] & [SwaggerResponseExample]
      //options.ExampleFilters();

      //It doesn't work anymore in recent versions because of replacing Swashbuckle.AspNetCore.Examples with Swashbuckle.AspNetCore.Filters
      //Adds an Upload button to endpoints which have [AddSwaggerFileUploadButton]
      //options.OperationFilter<AddFileParamTypesOperationFilter>();

      //Set summary of action if not already set
      //options.OperationFilter<ApplySummariesOperationFilter>();

      #region Add UnAuthorized to Response
      //Add 401 response and security requirements (Lock icon) to actions that need authorization
      options.OperationFilter<UnauthorizedResponsesOperationFilter>(true, "Bearer");// "OAuth2");
      #endregion
      options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

      #region Add Jwt Authentication
      //options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
      //  Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
      //  In = ParameterLocation.Header,
      //  Name = "Authorization",
      //  Type = SecuritySchemeType.ApiKey,
      //});
      //https://www.infoworld.com/article/3650668/implement-authorization-for-swagger-in-aspnet-core-6.html
      options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
      });

      options.OperationFilter<SecurityRequirementsOperationFilter>();

      //OAuth2Scheme
      //options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme());
      #endregion

      #region Versioning
      //// Remove version parameter from all Operations
      //options.OperationFilter<RemoveVersionParameters>();

      ////set version "api/v{version}/[controller]" from current swagger doc verion
      //options.DocumentFilter<SetVersionInPaths>();

      //Seperate and categorize end-points by doc version
      //options.DocInclusionPredicate((docName, apiDesc) =>
      //{
      //  if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

      //  var versions = methodInfo.DeclaringType
      //      .GetCustomAttributes<ApiVersionAttribute>(true)
      //      .SelectMany(attr => attr.Versions);

      //  return versions.Any(v => $"v{v.ToString()}" == docName);
      //});
      #endregion

      //If use FluentValidation then must be use this package to show validation in swagger (MicroElements.Swashbuckle.FluentValidation)
      //options.AddFluentValidationRules();
      #endregion
    });
  }

  public static void UseSwaggerAndUI(this WebApplication app) {
    //More info : https://github.com/domaindrivendev/Swashbuckle.AspNetCore

    //Swagger middleware for generate "Open API Documentation" in swagger.json
    app.UseSwagger();

    //Swagger middleware for generate UI from swagger.json
    app.UseSwaggerUI(options => {
      //options.RoutePrefix = "api-docs";
      options.SwaggerEndpoint("v1/swagger.json", "API v1");

      if (app.Environment.IsDevelopment())
        options.EnablePersistAuthorization();

      #region Customizing
      //// Display
      options.DefaultModelExpandDepth(2);
      options.DefaultModelRendering(ModelRendering.Model);
      //options.DefaultModelsExpandDepth(-1);
      //options.DisplayRequestDuration();
      options.DocExpansion(DocExpansion.None);
      options.EnableTryItOutByDefault();

      options.DisplayOperationId();
      options.EnableDeepLinking();

      options.EnableFilter();
      //options.MaxDisplayedTags(5);
      //options.ShowExtensions();

      //// Network
      //options.EnableValidator();
      //options.SupportedSubmitMethods(SubmitMethod.Get);

      //// Other
      //options.DocumentTitle = "CustomUIConfig";
      options.InjectStylesheet("/swagger-ui/style.css");
      options.InjectStylesheet("/swagger-ui/dark.css");
      //options.InjectJavascript("/ext/custom-javascript.js");
      #endregion
    });

    //ReDoc UI middleware. ReDoc UI is an alternative to swagger-ui
    app.UseReDoc(options => {
      options.SpecUrl("/swagger/v1/swagger.json");
      //options.SpecUrl("/swagger/v2/swagger.json");

      #region Customizing
      //By default, the ReDoc UI will be exposed at "/api-docs"
      //go to http://localhost:5000/docs to see ReDoc UI
      options.RoutePrefix = "docs";
      //options.DocumentTitle = "My API Docs";

      options.EnableUntrustedSpec();
      options.ScrollYOffset(10);
      options.HideHostname();
      options.HideDownloadButton();
      //options.ExpandResponses("200,201");
      options.RequiredPropsFirst();
      options.NoAutoAuth();
      options.PathInMiddlePanel();
      options.HideLoading();
      options.NativeScrollbars();
      options.DisableSearch();
      options.OnlyRequiredInSamples();
      options.SortPropsAlphabetically();
      #endregion
    });
  }
}