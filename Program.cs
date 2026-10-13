using NotificationFactoryMethod.Models;
using NotificationFactoryMethod.Notifications;
using NotificationFactoryMethod.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(builder.Configuration.GetSection("Notifications").Get<NotificationOptions>() ?? new());

// The only place that lists the concrete creators. A new channel is one more line here.
builder.Services.AddSingleton<NotificationDispatcher, EmailDispatcher>();
builder.Services.AddSingleton<NotificationDispatcher, SmsDispatcher>();
builder.Services.AddSingleton<NotificationDispatcher, PushDispatcher>();
builder.Services.AddSingleton<NotificationDispatcher, WhatsAppDispatcher>();

builder.Services.AddSingleton<NotificationOutbox>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "NotificationFactoryMethod.xml")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(o => o.RoutePrefix = "swagger");
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

const int MaxTitleLength = 200, MaxBodyLength = 5_000;

static string? ValidateMessage(string? title, string? body) =>
    string.IsNullOrWhiteSpace(title) ? "Title is required."
    : title.Length > MaxTitleLength ? $"Title must be at most {MaxTitleLength} characters."
    : string.IsNullOrWhiteSpace(body) ? "Body is required."
    : body.Length > MaxBodyLength ? $"Body must be at most {MaxBodyLength} characters."
    : null;

static string? ValidateChannel(NotificationChannel channel, NotificationService svc) =>
    Enum.IsDefined(channel) && svc.Supports(channel) ? null : $"Unknown channel. Use: {string.Join(", ", Enum.GetNames<NotificationChannel>())}.";

app.MapGet("/api/channels", (NotificationService svc) => svc.Channels())
.WithSummary("Each channel with its creator, product, recipient format and price");

app.MapPost("/api/notifications", async (SendNotificationRequest req, NotificationService svc) =>
{
    var error = ValidateChannel(req.Channel, svc)
        ?? (string.IsNullOrWhiteSpace(req.Recipient) ? "Recipient is required." : null)
        ?? ValidateMessage(req.Title, req.Body);
    if (error is not null) return Results.BadRequest(error);

    var result = await svc.SendAsync(req);
    return result.Status == DispatchStatus.Sent ? Results.Ok(result) : Results.UnprocessableEntity(result);
})
.WithSummary("Send through the channel's dispatcher. The steps show the factory method creating the channel, then the shared steps using it. 422 if the channel rejects the recipient.");

app.MapPost("/api/notifications/preview", (PreviewRequest req, NotificationService svc) =>
    (ValidateMessage(req.Title, req.Body) ?? req.Channels.Select(c => ValidateChannel(c, svc)).FirstOrDefault(e => e is not null)) is { } error
        ? Results.BadRequest(error)
        : Results.Ok(svc.Preview(req)))
.WithSummary("Render the same message for several channels without sending it. Compare parts, notes and cost.");

app.MapGet("/api/notifications", (NotificationOutbox outbox) => outbox.Entries)
.WithSummary("The outbox: every dispatch, sent or rejected, newest first");

app.MapGet("/api/notifications/{id}", (string id, NotificationOutbox outbox) =>
    outbox.Find(id) is { } result ? Results.Ok(result) : Results.NotFound($"Notification {id} not found."))
.WithSummary("One dispatch with its parts and steps");

app.MapDelete("/api/notifications", (NotificationOutbox outbox) => { outbox.Clear(); return Results.NoContent(); })
.WithSummary("Clear the outbox");

app.Run();
