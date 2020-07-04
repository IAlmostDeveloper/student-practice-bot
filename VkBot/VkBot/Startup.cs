using DataBaseAccess.Data;
using DataBaseAccess.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VkBot.Services;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace VkBot
{
	public class Startup
	{
		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			Configuration = configuration;
			Env = env;
		}

		public IConfiguration Configuration { get; }
		public IWebHostEnvironment Env { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			var connectionString = Configuration.GetConnectionString("MySql" + Env.EnvironmentName);
			services.AddSingleton<DataBaseContext>(new MySqlDataBase(connectionString));
			services.AddSingleton<QuestionAndAnswerRepository>();
			services.AddSingleton<WordAndAnswerRepository>();
			
			var api = new VkApi();
			var apiAuthParams = new ApiAuthParams{ AccessToken = Configuration["Config:AccessToken"] };
			api.Authorize(apiAuthParams);
			services.AddSingleton<IVkApi>(api);
			services.AddSingleton<VkEventHandler>();

			services.AddSingleton<ErrorsStore>();
			
			services.AddControllers().AddNewtonsoftJson();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();
			else
				app.UseExceptionHandler("/handleError");
			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
		}
	}
}