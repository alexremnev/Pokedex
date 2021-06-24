using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Pokedex.Api.Middleware;
using Pokedex.Services;
using Pokedex.Services.Translator;

namespace Pokedex.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var appConfiguration = Configuration.Get<AppConfiguration>();

            services.AddHttpClient();

            services.AddScoped<IPokemonProvider>(ctx => new PokemonProvider(ctx.GetRequiredService<IHttpClientFactory>(), ctx.GetRequiredService<ILogger<PokemonProvider>>(), appConfiguration.ApiEndpoint));

            services.AddScoped<ITranslatorFactory, TranslatorFactory>();
            services.AddScoped<ITranslator>(ctx => new ShakespeareTranslator(ctx.GetRequiredService<IHttpClientFactory>(), appConfiguration.ShakespeareTranslatorUrl));
            services.AddScoped<ITranslator>(ctx => new YodaTranslator(ctx.GetRequiredService<IHttpClientFactory>(), appConfiguration.YodaTranslatorUrl));

            services.AddScoped<IPokemonService, PokemonService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pokedex.Api", Version = "v1" });
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pokedex.Api v1"));
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
