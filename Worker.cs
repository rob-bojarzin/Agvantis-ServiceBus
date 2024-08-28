using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly IServiceBusService _serviceBusService;
        private readonly ILogger<Worker> _logger;
        private ServiceBusProcessor _processor;

        public Worker(IServiceBusService serviceBusService, ILogger<Worker> logger)
        {
            _serviceBusService = serviceBusService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor = _serviceBusService.CreateProcessor();

            _processor.ProcessMessageAsync += async args =>
            {
                var body = args.Message.Body.ToString();
                _logger.LogInformation($"Received message: {body}");

                await args.CompleteMessageAsync(args.Message);
            };

            _processor.ProcessErrorAsync += async args =>
            {
                _logger.LogError($"Error occurred: {args.Exception.Message}");
            };

            await _processor.StartProcessingAsync(stoppingToken);

            _logger.LogInformation("ServiceBusWorker running.");

            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            await _processor.StopProcessingAsync();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ServiceBusWorker is stopping.");
            await base.StopAsync(cancellationToken);
        }
    }
}
