namespace API;

public static class ConfigureServices {
  public static WebApplication ApplySecurityHeaders(this WebApplication app) {

    //Configuring Content Type Options with the �nosniff� option disables MIME-type sniffing
    // to prevent attacks where files are missing metadata: X-Content-Type-Options: nosniff
    //https://stackoverflow.com/questions/18337630/what-is-x-content-type-options-nosniff
    app.UseXContentTypeOptions();
    //exclude the �Referrer� header, which can improve security in cases where 
    //the URL of the previous web page contains sensitive data.Referrer-Policy: no-referrer
    app.UseReferrerPolicy(opt => opt.NoReferrer());
    //enables the detection of XSS attacks: X-XSS-Protection: 1; mode=block
    app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
    //prevent click-jacking attacks: X-Frame-Options: Deny
    app.UseXfo(opt => opt.Deny());
    //Content Security Policy header,allows you to configure at a very granular level what content 
    //you want to allow your web app to load and precisely which sources you want to load content from.
    //use app.UseCspReportOnly to get the reports
    app.UseCsp(opt => opt
        .BlockAllMixedContent()
        .StyleSources(s => s.Self()
            .CustomSources("https://fonts.googleapis.com", "sha256-F4GpCPyRepgP5znjMD8sc7PEjzet5Eef4r09dEGPpTs="))
        .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:"))
        .FormActions(s => s.Self())
        .FrameAncestors(s => s.Self())
        //to fix react-cropper issue: "blob:", "data:"
        .ImageSources(s => s.Self().CustomSources("https://res.cloudinary.com", "blob:", "data:"))
    //tried this but couldn't fix
    //.ScriptSources(s => s.Self().CustomSources("sha256-ma5XxS1EBgt17N22Qq31rOxxRWRfzUTQS1KOtfYwuNo="))
    );

    return app;
  }
}