MVC and Razor Page content is great for viewing in a browser, but what about for sending as email? Rendering MVC views and pages to strings variables for sending as email has never been very straightforward, as far as I can tell. I've done a couple of solutions over the years that were really app-specific. There are libraries for this like [RazorLight](https://github.com/toddams/RazorLight), and what looks like a great solution from [Rick Strahl](https://weblog.west-wind.com/posts/2022/Jun/21/Back-to-Basics-Rendering-Razor-Views-to-String-in-ASPNET-Core#how-to-capture-razor-output). There are lot of articles on this online if you go searching -- lots of one-off solutions and StackOverflow answers. You too may have been tempted to implement your own Razor substitute or templating mechanism at some point. IMO this is still too complicated.

I wanted to take a swing at this that let me use Razor pages in email that are easy to preview and debug in the browser, yet would work seamlessly as email content. Since my solution uses an `HttpClient` that is not authenticated, this Razor page email content usually needs to prevent direct viewing in a browser by end users, but can't use the `[Authorize]` attribute. (For example, welcome emails for new user registrations that might contain passwords.)

Since there are many email clients out there, I didn't want to couple this solution to a certain mail client. That's why this repo name is "RazorToString", and does not reference email explicitly, even though that's the use case this is intended for. All that said, here's my approach in a nutshell:

- install the NuGet package **AO.RazorToString** from my custom [package source](https://aosoftware.blob.core.windows.net/packages/index.json)
- in your application startup, add the [AuthorizeEmailFilter](https://github.com/adamfoneil/RazorToString/blob/master/RazorToString/Filters/AuthorizeEmailFilter.cs) and [AuthorizeEmailOptions](https://github.com/adamfoneil/RazorToString/blob/master/RazorToString/Models/AuthorizeEmailOptions.cs)
- create Razor pages that you want to send as email, and add the `[AuthorizeEmail]` attribute instead of `[Authorize]`.
- use the `IServiceProvider` [RenderViewAsync](https://github.com/adamfoneil/RazorToString/blob/master/RazorToString/Extensions/ServiceProviderExtensions.cs#L33) extension method wherever you need to render a page or view to a string.
- there are a few bonus methods part of this library: [BuildUrl](https://github.com/adamfoneil/RazorToString/blob/master/RazorToString/Extensions/ServiceProviderExtensions.cs#L31) and [GetHttpsUrl](https://github.com/adamfoneil/RazorToString/blob/master/RazorToString/Extensions/ServiceProviderExtensions.cs#L25) that let you craft absolute URLs without knowing the host name.

## Setup
After you've installed the NuGet package, you need a couple of `Startup` and configuration changes:

Add the `AuthorizeEmailFilter` like this:
```csharp
services.AddRazorPages().AddMvcOptions(setup =>
{
    setup.Filters.Add<AuthorizeEmailFilter>();
});
```

Add an `AuthorizeEmailOptions` value to your `appsettings.json` file:

```json
"AuthorizeEmailOptions": {
    "HashSalt": <a secret value you make up>
}
```

There are different ways to get this into your configuration, but this makes sense to me:
```csharp
services.Configure<AuthorizeEmailOptions>(config => Configuration.Bind("AuthorizeEmailOptions", config));            
```

Now, to create Razor pages for email, create pages as usual, but add the `[AuthorizeEmail]` attribute to them. Your pages can do whatever they need to, and you can preview them in a browser to debug.

## Usage

You'll need a `using` statement somewhere to make the extension method available:
```csharp
@using RazorToStringServices.Extensions
```
You'll also need to `@inject` an `IServiceProvider` wherever you're using this because that's what the `RenderPageAsync` extension method requires. In Blazor, it would be like this:
```csharp
@inject IServiceProvider Services
```
Now you can use the `RenderPageAsync` extension method. Here's an example that assumes a fictional page `/Email/NewUser/{id}` using [Mailgun](https://www.mailgun.com/). Note that a full Mailgun tutorial is beyond the scope of this repo. I'm just showing below how the html is built and inserted into an email.

```csharp
var html = await Services.RenderPageAsync($"Email/NewUser/{user.Id}");

await Mailgun.SendAsync(new EmailMessage()
{
    ToEmail = email,
    Subject = "Your New User Account",
    HtmlBody = html
});
```
