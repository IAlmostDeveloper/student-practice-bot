using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VkBot.Data;
using VkBot.Services;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace VkBot
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
			var connectionString = Configuration.GetConnectionString("MySql");
			services.AddSingleton<DataBaseContext>(new MySqlDataBase(connectionString));
			services.AddSingleton<StudentRepository>();
			
			var api = new VkApi();
			var apiAuthParams = new ApiAuthParams{ AccessToken = Configuration["Config:AccessToken"] };
			api.Authorize(apiAuthParams);
			services.AddSingleton<IVkApi>(api);
			services.AddSingleton<VkEventHandler>();
			services.AddControllers().AddNewtonsoftJson();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseEndpoints(endpoints => endpoints.MapControllers());
		}
	}
}