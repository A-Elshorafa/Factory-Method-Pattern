# Factory Method Pattern – Order Notifications (.NET 10 + React)

## What is the Factory Method pattern?

A **Factory Method** is a method in a base class that creates an object. The base class calls it but doesn't implement it, and **each subclass overrides it to decide which concrete class to create**. The base class has the logic everyone shares and works only with the product's interface. The subclasses supply only the part that varies: which object to create, and how to build it.

Here, a shop called NileMart sends order notifications. Every channel follows the same steps: check the recipient, fit the message into the channel's limits, deliver each part, and add up the cost. Those steps are written once in `NotificationDispatcher`. What differs is the channel object, and each dispatcher subclass creates its own in `CreateChannel()`.

**How it differs from similar patterns:**

- A **Simple Factory** is a `switch` that returns one of several classes, like `StorageStackFactory` in the [Decorator](../../decorator-pattern/DocumentStorageDecorator) example. Adding a case means editing that switch. With a Factory Method you add a subclass and leave the existing code alone.
- An **Abstract Factory** creates a *family* of related objects (e.g. a button, a checkbox and a menu for one theme). A Factory Method creates *one* product, and it's usually part of a class that also does something with that product.
- In the [Strategy](../../strategy-pattern/ShippingStrategy) example, the client is handed a ready-made object. Here the creator builds the product itself, on every call.

## Structure

```
NotificationDispatcher              ◄── Creator: DispatchAsync / Preview / Describe   (Notifications/NotificationDispatcher.cs)
 │   # CreateChannel()              ◄── the factory method (abstract)
 ├── EmailDispatcher                ◄── Concrete Creator → new EmailChannel(from)
 ├── SmsDispatcher                  ◄── Concrete Creator → new SmsChannel(senderId)
 ├── PushDispatcher                 ◄── Concrete Creator → new PushChannel()
 └── WhatsAppDispatcher             ◄── Concrete Creator → new WhatsAppChannel(businessNumber)

INotificationChannel                ◄── Product: ValidateRecipient / Render / DeliverAsync / CostPerPart
 ├── EmailChannel                   ◄── one message, title as subject
 ├── SmsChannel                     ◄── GSM-7 or UCS-2 segments, billed per segment
 ├── PushChannel                    ◄── title and body cut to fit the banner, free
 └── WhatsAppChannel                ◄── bold title, international number only

NotificationService                 ◄── Client: picks a dispatcher, calls it through the base type
```

What `DispatchAsync` does for an SMS:

```
SmsDispatcher        Concrete Creator   CreateChannel() returned a new SmsChannel
SmsChannel           Product            ValidateRecipient("+201012345678") accepted it
SmsChannel           Product            Render() produced 2 parts. From NILEMART. GSM-7. 2 segments of up to 153 chars.
SmsChannel           Product            DeliverAsync() part 1/2 (153 chars) → sms-…
SmsChannel           Product            DeliverAsync() part 2/2 (91 chars) → sms-…
NotificationDispatcher Creator          2 parts × 0.30 EGP = 0.60 EGP
```

- `DispatchAsync`, `Preview` and `Describe` live in the Creator and are written once. Each one calls `CreateChannel()` and uses only `INotificationChannel`. None of them contains `new SmsChannel` or a `switch` on the channel.
- The factory method is `protected abstract`. Callers can't create a channel themselves; they ask a dispatcher to *do* something, and it creates the channel when it needs one.
- **Each creator decides how to build its product.** `SmsDispatcher` passes the sender ID, `EmailDispatcher` passes the From address, and `PushDispatcher` needs nothing. The options are read from the `Notifications` section of `appsettings.json`.
- A new channel object is created on every dispatch, so a channel can hold per-message state (a connection, a session) safely.
- `Program.cs` is the only place that lists the concrete creators. `NotificationService` gets them all as `IEnumerable<NotificationDispatcher>` and never names one.

### Adding a channel

To add Telegram, you write two new classes and add two one-line registrations, without editing any of the dispatch code:

1. `TelegramChannel : INotificationChannel` with its own recipient rule and limits.
2. `TelegramDispatcher : NotificationDispatcher`, which overrides `CreateChannel()` to return `new TelegramChannel(...)`.
3. A `Telegram` value in `NotificationChannel`, and `AddSingleton<NotificationDispatcher, TelegramDispatcher>()` in `Program.cs`.

## Project layout

```
NotificationFactoryMethod/
├── Program.cs                       endpoints, validation, DI (registers the concrete creators)
├── appsettings.json                 From address, SMS sender ID, WhatsApp number
├── Notifications/
│   ├── NotificationDispatcher.cs    Creator with the factory method
│   ├── *Dispatcher.cs               Concrete Creators
│   ├── INotificationChannel.cs      Product
│   ├── *Channel.cs                  Concrete Products
│   ├── TextParts.cs                 truncating and splitting text safely
│   ├── NotificationOptions.cs       settings the creators pass to their channels
│   └── NotificationOutbox.cs        every dispatch, newest first
├── Services/NotificationService.cs  Client
├── Models/                          one record/class/enum per file
└── ClientApp/                       React + Tailwind tester (Vite)
    └── src/
        ├── App.jsx                  page state and actions
        ├── api.js                   calls to the backend
        ├── styles.js                shared Tailwind class strings, channel and role colours
        └── components/              SendForm, ResultPanel, StepList, MessageView, Outbox, ChannelChip
```

## Run

Backend (terminal 1):

```bash
dotnet run
```

Swagger UI: http://localhost:8101/swagger

Client (terminal 2):

```bash
cd ClientApp && npm install && npm run dev
```

Client: http://localhost:8102

Vite forwards `/api` calls to the backend on port 8101, so no CORS setup is needed. Start the backend first.

## Client app

A one-page tester for the dispatchers:

- **Send a notification**:
  - **Channel**: one card per concrete creator, with its price. Under the cards, a code line shows which dispatcher is used and which channel its `CreateChannel()` builds, plus that channel's rules.
  - **Recipient**: the format hint comes from the channel. Switching channels fills in that channel's example recipient, unless you typed your own.
  - **Message**: title and body, with *Short*, *Long* and *Arabic* samples.
  - **Send** dispatches through the selected channel. **Preview on all channels** renders the message with every channel without sending it.
- **Result**:
  - *Sent / Rejected*: the steps of the dispatch, each labelled with its role (Concrete Creator, Product, Creator), next to what the recipient gets. A rejected send shows the channel's reason and stops after validation.
  - *Preview*: one card per channel with its part count, note and cost (the cheapest is green), rendered the way that channel displays it: an email with headers, SMS bubbles per segment, a push banner, and a WhatsApp bubble with a bold title.
- **Outbox**: every dispatch with its channel, recipient, parts and cost (or *Rejected*), plus the totals. **View** opens it in the result panel.

## Endpoints

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/channels` | Each channel with its creator, product, recipient format, example recipient and price |
| POST | `/api/notifications` | Send through the channel's dispatcher. Returns the parts, cost and steps (422 if the recipient is rejected) |
| POST | `/api/notifications/preview` | Render the same message for several channels without sending it |
| GET | `/api/notifications` | The outbox: every dispatch, sent or rejected, newest first |
| GET | `/api/notifications/{id}` | One dispatch with its parts and steps |
| DELETE | `/api/notifications` | Clear the outbox |

Prices are made up for the demo and in EGP: Email 0.05 per message, SMS 0.30 per segment, Push free, WhatsApp 0.45 per message.

## Try it in the client

1. Click **Send via Sms** with the defaults. The first step is `SmsDispatcher` calling `CreateChannel()`; the rest is the shared `NotificationDispatcher` code using the `SmsChannel`. One 111-char SMS, 0.30 EGP.
2. Pick **Email** (the recipient switches to `mona.adel@example.com`) and send. Same steps, a different product: the message now has From and Subject headers.
3. Pick **WhatsApp**, change the recipient to `01012345678` and send. `WhatsAppChannel` rejects it and nothing is billed. Pick **Sms** and send the same number: it's accepted. Each channel has its own recipient rule.
4. Click **Long**, then **Preview on all channels**. SMS needs 2 segments, Push cuts the body to 150 chars, and Email and WhatsApp each send one message.
5. Click **Arabic**, then **Preview on all channels**. The SMS switches to UCS-2 and splits at 67 chars, so this shorter message still costs two segments.
6. The **Outbox** lists every send, including the rejected one. Click **View** to see its steps again.

## Try it in Swagger

1. `GET /api/channels`. Each channel has its own creator and product. Note the `exampleRecipient` for each one.
2. `POST /api/notifications` with the default body (SMS to `+201012345678`). `steps` starts with `SmsDispatcher` calling `CreateChannel()`, then the `SmsChannel` validates, renders and delivers one 111-char SMS, and the creator totals 0.30 EGP.
3. Send the same body with `"channel": "Email"` and `"recipient": "mona.adel@example.com"`. The steps are the same; only the product is different. The part now has `From:` and `Subject:` headers.
4. Send with `"channel": "WhatsApp"` and `"recipient": "01012345678"`. You get **422**: `WhatsAppChannel` only accepts `+201…`. The SMS channel accepts that same local number. The recipient rule belongs to the product, and the creator stops before anything is sent or billed.
5. `POST /api/notifications/preview` with the default body (a 224-char message). One request, four products:
   - **Email**: 1 part, 0.05 EGP.
   - **SMS**: 2 segments of 153 + 91 chars, 0.60 EGP.
   - **Push**: body cut to 150 chars, free.
   - **WhatsApp**: 1 message with a bold title, 0.45 EGP.
6. Preview an Arabic message, e.g. `{"title": "تم شحن طلبك رقم 1042", "body": "طلبك في الطريق ومن المتوقع أن يصل يوم الخميس بين الساعة 10 و 14. تابع الشحنة على nilemart.example/t/1042", "channels": ["Sms"]}`. The SMS switches to UCS-2 and splits at 67 chars, so a 125-char message costs two segments.
7. `GET /api/notifications` lists every send, including the rejected WhatsApp one with its error and steps.
