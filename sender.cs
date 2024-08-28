using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

class Program
{
    private const string ServiceBusConnectionString = "<Your_ServiceBus_Connection_String>";
    private const string QueueName = "<Your_Queue_Name>";

    public static async Task Main(string[] args)
    {
        await SendMessageAsync();
    }

    static async Task SendMessageAsync()
    {
        // Create a Service Bus client
        await using var client = new ServiceBusClient(ServiceBusConnectionString);

        // Create a sender for the queue
        ServiceBusSender sender = client.CreateSender(QueueName);

        try
        {
            // Create a message to send
            ServiceBusMessage message = new ServiceBusMessage("Hello, Service Bus!");

            // Send the message
            await sender.SendMessageAsync(message);
            Console.WriteLine($"Sent message: {message.Body}");
        }
        finally
        {
            // Close the client
            await sender.CloseAsync();
        }
    }
}
