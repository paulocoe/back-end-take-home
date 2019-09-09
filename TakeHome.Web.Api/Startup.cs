using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TakeHome.Data;
using TakeHome.Mediator.Handlers;
using TakeHome.Services;

namespace TakeHome
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITakeHomeRepository, TakeHomeRepository>(x => new TakeHomeRepository(Configuration["ConnectionString"]));
            services.AddScoped<ITakeHomeService, TakeHomeService>();            
            services.AddMvc();
            services.AddMediatR(typeof(GetShortestRouteHandler));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();            
        }
    }
}
