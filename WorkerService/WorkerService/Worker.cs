namespace WorkerService
{

    using System.Text;

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string _filePath;
        private int _batchNumber = 0;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            string folder = Path.Combine(
                AppContext.BaseDirectory, "Output");

            Directory.CreateDirectory(folder);

            _filePath = Path.Combine(
                folder, "transactions.txt");
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Dummy Worker started at: {time}",
                DateTime.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await GenerateAndSaveDataAsync(stoppingToken);

                    // Wait 10 seconds before next batch
                    await Task.Delay(
                        TimeSpan.FromSeconds(10),
                        stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex, "Error generating dummy data");

                    await Task.Delay(
                        TimeSpan.FromSeconds(5),
                        stoppingToken);
                }
            }

            _logger.LogInformation("Dummy Worker stopped.");
        }

        private async Task GenerateAndSaveDataAsync(
            CancellationToken cancellationToken)
        {
            _batchNumber++;

            var sb = new StringBuilder();

            sb.AppendLine(
                $"--- Batch {_batchNumber} | " +
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} ---");

            for (int i = 1; i <= 100; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var transaction = new
                {
                    TransactionId = Guid.NewGuid(),
                    AccountNo = $"100000{Random.Shared.Next(1000, 9999)}",
                    Amount = Math.Round(
                        Random.Shared.NextDouble() * 100000, 2),
                    BranchCode = $"BR{Random.Shared.Next(1, 51):D3}",
                    TransactionDate = DateTime.Now
                };

                sb.AppendLine(
                    $"{transaction.TransactionId}|" +
                    $"{transaction.AccountNo}|" +
                    $"{transaction.Amount:F2}|" +
                    $"{transaction.BranchCode}|" +
                    $"{transaction.TransactionDate:yyyy-MM-dd HH:mm:ss}");
            }

            await File.AppendAllTextAsync(
                _filePath,
                sb.ToString(),
                Encoding.UTF8,
                cancellationToken);

            _logger.LogInformation(
                "Batch {batch} saved: 100 records to {path}",
                _batchNumber, _filePath);
        }
    }

}
