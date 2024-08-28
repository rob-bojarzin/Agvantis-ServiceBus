using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ServiceBusWorkerService
{
    public interface IServiceBusService
    {
        ServiceBusProcessor CreateProcessor();
        Task SendMessageAsync(string message);
    }

    public class ServiceBusService : IServiceBusService
    {
        private readonly ServiceBusClient _client;
        private readonly ILogger<ServiceBusService> _logger;
        private readonly string _queueName;

        public ServiceBusService(ILogger<ServiceBusService> logger, string connectionString, string queueName)
        {
            _client = new ServiceBusClient(connectionString);
            _logger = logger;
            _queueName = queueName;
        }

        public ServiceBusProcessor CreateProcessor()
        {
            var processorOptions = new ServiceBusProcessorOptions
            {
                // Configure options here if needed
            };
            var processor = _client.CreateProcessor(_queueName, processorOptions);

            return processor;
        }

        public async Task SendMessageAsync(string message)
        {
            var sender = _client.CreateSender(_queueName);

            try
            {
                var serviceBusMessage = new ServiceBusMessage(message);
                await sender.SendMessageAsync(serviceBusMessage);
                _logger.LogInformation($"Sent message: {message}");
            }
            finally
            {
                await sender.CloseAsync();
            }
        }
    }
}
