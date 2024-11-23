# TinyIpc

[![NuGet](https://img.shields.io/nuget/v/TinyIpc.svg?maxAge=259200)](https://www.nuget.org/packages/TinyIpc/)
![Build](https://github.com/steamcore/TinyIpc/workflows/Build/badge.svg)

.NET inter process broadcast message bus.

Intended for quick broadcast messaging in Windows desktop applications, it just works.

## Quick introduction

* Designed to be serverless
* Clients may drop in and out at any time
* Messages expire after a specified timeout, default 1 second
* The log is kept small for performance, default max log size is 1 MB
* Reads are queued and should be received in the same order as they were published

## Benefits and drawbacks

It's easy to use and there is no complicated setup. It is suited for small messages,
so big messages probably need some other transport mechanism. With high enough
throughput messages may be lost if receivers are not able to get a read lock before
the message timeout is reached.

## Performance
Every publish operation reads and writes the entire contents of a shared memory
mapped file and every read operation which is triggered by writes also reads the
entire file so if performance is important then batch publish several messages
at once to reduce the amount of reads and writes.

## OS Support

Unfortunately TinyIpc only works on Windows because the named primitives that
are core to this entire solution only works on Windows and throws
PlatformNotSupportedException on other operating systems by design.

See https://github.com/dotnet/runtime/issues/4370 for more information.

## Compared to other solutions

|                                             | TinyIPC  | IpcChannel | Named Pipes |
|---------------------------------------------|----------|------------|-------------|
| Broadcasting to all listeners (except self) | &#x2713; | &#x2717;   | &#x2717;    |
| No master process                           | &#x2713; | &#x2717;   | &#x2717;    |
| Insensitive to process privilege level      | &#x2713; | &#x2713;   | &#x2713;    |
| Entirely in memory                          | &#x2713; | &#x2713;   | &#x2713;    |

## Simple example

One message bus listening to the other.
Check [ConsoleApp](samples/ConsoleApp/) for a sample application.

```csharp
using var messagebus1 = new TinyMessageBus("ExampleChannel");
using var messagebus2 = new TinyMessageBus("ExampleChannel");

messagebus2.MessageReceived +=
	(sender, e) => Console.WriteLine(e.Message.ToString());

while (true)
{
	var message = Console.ReadLine();
	await messagebus1.PublishAsync(BinaryData.FromString(message));
}
```
## Example using generic hosting

Equivalent example to the above using generic hosting.
Check [GenericHost](samples/GenericHost/) for a sample application.

```csharp
// Add service to IServiceCollection
services.AddTinyIpc(options =>
{
	options.Name = "ExampleChannel";
});

// Later use ITinyIpcFactory to create instances
using var tinyIpcInstance1 = tinyIpcFactory.CreateInstance();
using var tinyIpcInstance2 = tinyIpcFactory.CreateInstance();

tinyIpcInstance2.MessageBus.MessageReceived +=
	(sender, e) => Console.WriteLine(e.Message.ToString());

while (true)
{
	var message = Console.ReadLine();
	await tinyIpcInstance1.MessageBus.PublishAsync(BinaryData.FromString(message));
}
```
