using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos;
using Microsoft.OpenApi.Models;

using Backend.Services;
using Backend.Helpers;

namespace Backend
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		public IWebHostEnvironment WebHostEnvironment { get; }

		public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
		{
			Configuration = configuration;
			WebHostEnvironment = webHostEnvironment;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			IConfigurationSection cosmosDbConfigSection = Configuration.GetSection("CosmosDb");

			services.AddSingleton<ICosmosDBService>(serviceProvider => InitializeCosmosDBService(cosmosDbConfigSection));

			// Add Newtonsoft JSON for handling serialization
			services.AddControllers().AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.CheckAdditionalContent = BackendSettings.DefaultNewtonsoftJsonFormatter.CheckAdditionalContent;
				options.SerializerSettings.Culture = BackendSettings.DefaultNewtonsoftJsonFormatter.Culture;
				options.SerializerSettings.DateFormatHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.DateFormatHandling;
				options.SerializerSettings.DateTimeZoneHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.DateTimeZoneHandling;
				options.SerializerSettings.DefaultValueHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.DefaultValueHandling;
				options.SerializerSettings.FloatFormatHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.FloatFormatHandling;
				options.SerializerSettings.FloatParseHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.FloatParseHandling;
				options.SerializerSettings.Formatting = BackendSettings.DefaultNewtonsoftJsonFormatter.Formatting;
				options.SerializerSettings.MaxDepth = BackendSettings.DefaultNewtonsoftJsonFormatter.MaxDepth;
				options.SerializerSettings.MetadataPropertyHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.MetadataPropertyHandling;
				options.SerializerSettings.MissingMemberHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.MissingMemberHandling;
				options.SerializerSettings.NullValueHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.NullValueHandling;
				options.SerializerSettings.ObjectCreationHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.ObjectCreationHandling;
				options.SerializerSettings.PreserveReferencesHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.PreserveReferencesHandling;
				options.SerializerSettings.ReferenceLoopHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.ReferenceLoopHandling;
				options.SerializerSettings.StringEscapeHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.StringEscapeHandling;
				options.SerializerSettings.TypeNameAssemblyFormatHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.TypeNameAssemblyFormatHandling;
				options.SerializerSettings.TypeNameHandling = BackendSettings.DefaultNewtonsoftJsonFormatter.TypeNameHandling;
			});

			// Set the multipart-body-length limit to 32 MB
			services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options => options.MultipartBodyLengthLimit = 33554432); // 32MB

			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				// optional information such as the author, license, and description about the API
				/*c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Assignment API",
					Description = "A simple example ASP.NET Core Web API",
					TermsOfService = new Uri("https://example.com/terms"),
					Contact = new OpenApiContact
					{
						Name = "Shayne Boyer",
						Email = string.Empty,
						Url = new Uri("https://twitter.com/spboyer"),
					},
					License = new OpenApiLicense
					{
						Name = "Use under LICX",
						Url = new Uri("https://example.com/license"),
					}
				});*/

				// Set the comments path for the Swagger JSON and UI.
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger(c =>
		   {
			   //c.SerializeAsV2 = true; // Opt into 2.0 format in case if compatibility is needed
		   });

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Assignment API V1");
				c.RoutePrefix = string.Empty; // serve the Swagger UI at the app's root
			});

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		private static CosmosDBService InitializeCosmosDBService(IConfigurationSection configurationSection)
		{
			string databaseName = configurationSection.GetSection("DatabaseName").Value;
			string account = configurationSection.GetSection("Account").Value;
			string key = configurationSection.GetSection("Key").Value;
			CosmosClientBuilder clientBuilder = new CosmosClientBuilder(account, key);
			CosmosClient client = clientBuilder
								.WithConnectionModeDirect()
								.WithSerializerOptions(new CosmosSerializationOptions { IgnoreNullValues = true })
								.Build();
			CosmosDBService cosmosDbService = new CosmosDBService(client, databaseName, configurationSection);
			return cosmosDbService;
		}
	}
}
