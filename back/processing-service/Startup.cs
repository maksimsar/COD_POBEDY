using System;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProcessingService.Services;
using ProcessingService.Utils;

namespace ProcessingService
{
    public class Startup
    {
        readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            // Конфигурация RabbitMQ
            var rmqSection = _configuration.GetRequiredSection("RabbitMQ");
            var rmq = rmqSection.Get<RabbitMqSettings>()!;
            services.Configure<RabbitMqSettings>(rmqSection);

            // Конфигурация ML-сервиса
            var mlSection = _configuration.GetRequiredSection("MlService");
            var ml = mlSection.Get<MlServiceSettings>()!;
            services.Configure<MlServiceSettings>(mlSection);

            // Конфигурация хранилищ
            var storageSection = _configuration.GetRequiredSection("Storage");
            var storage = storageSection.Get<StorageSettings>()!;
            services.Configure<StorageSettings>(storageSection);

            // MassTransit + RabbitMQ
            services.AddMassTransit(x =>
            {
                x.AddConsumer<DenoiseConsumer>();
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(rmq.Host, h =>
                    {
                        h.Username(rmq.Username);
                        h.Password(rmq.Password);
                    });
                    cfg.ReceiveEndpoint("denoise-queue", e =>
                        e.ConfigureConsumer<DenoiseConsumer>(ctx));
                });
            });

            // HTTP-клиент для ML-сервиса
            services.AddHttpClient<IMlServiceClient, MlServiceClient>(client =>
            {
                client.BaseAddress = new Uri(ml.BaseUrl);
            });

            // Сервис работы с файлами
            services.AddSingleton<IStorageService, FileStorageService>();

            services.AddControllers();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
