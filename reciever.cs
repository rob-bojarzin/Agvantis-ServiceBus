using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

class Program
{
    private const string ServiceBusConnectionString = "<Your_ServiceBus_Connection_String>";
    private const string QueueName = "<Your_Queue_Name>";

    public static async Task Main(string[] args)
    {
        await ReceiveMessagesAsync();
    }

    static async Task ReceiveMessagesAsync()
    {
        // Create a Service Bus client
        await using var client = new ServiceBusClient(ServiceBusConnectionString);

        // Create a processor for the queue
        ServiceBusProcessor processor = client.CreateProcessor(QueueName, new ServiceBusProcessorOptions());

        // Register handlers for processing messages
        processor.ProcessMessageAsync += MessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;

        // Start processing
        await processor.StartProcessingAsync();

        Console.WriteLine("Press any key to stop processing...");
        Console.ReadKey();

        // Stop processing
        await processor.StopProcessingAsync();

        // Close the client
        await processor.DisposeAsync();
    }

    static async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        Console.WriteLine($"Received message: {body}");

        // Complete the message
        await args.CompleteMessageAsync(args.Message);
    }

    static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"Error occurred: {args.Exception.Message}");
        return Task.CompletedTask;
    }
}
