using Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<QueuedHostedService>();
    })
    .Build();

await host.RunAsync();
