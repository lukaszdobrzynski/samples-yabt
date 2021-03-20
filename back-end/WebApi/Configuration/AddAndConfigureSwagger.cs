﻿using System;
using System.IO;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using Raven.Yabt.Domain.Common;
using Raven.Yabt.WebApi.Authorization.ApiKeyAuth;
using Raven.Yabt.WebApi.Configuration.Swagger;

namespace Raven.Yabt.WebApi.Configuration
{
	internal static partial class ServiceCollectionExtensions
	{
		private const string AppName = "Yet Another Bug Tracker (YABT) API";

		public static void AddAndConfigureSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(options =>
				{
					options.SwaggerDoc("v1", new OpenApiInfo { Title = AppName, Version = "v1" });

					// Locate the XML file being generated by ASP.NET...
					var mainXmlFile   = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
					var domainXmlFile = $"{typeof(BaseService<>).Assembly.GetName().Name}.xml";
					var mainXmlPath	  = Path.Combine(AppContext.BaseDirectory, mainXmlFile);
					var domainXmlPath = Path.Combine(AppContext.BaseDirectory, domainXmlFile);

					//... and tell Swagger to use those XML comments.
					options.IncludeXmlComments(mainXmlPath);
					options.IncludeXmlComments(domainXmlPath);

					// Configure authentication
					options.AddSecurityDefinition ( PredefinedUserApiKeyAuthOptions.DefaultScheme, 
													new OpenApiSecurityScheme
													{
														Description = "The API key corresponds to the user",
														Name = PredefinedUserApiKeyAuthHandler.ApiKeyHeaderName,
														In = ParameterLocation.Header,
														Type = SecuritySchemeType.ApiKey,
														Scheme = PredefinedUserApiKeyAuthOptions.DefaultScheme
													});
					options.OperationFilter<SwaggerSecurityRequirementsOperationFilter>();
				});
		}

		public static void AddAppSwaggerUi(this IApplicationBuilder app)
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", AppName);
				options.OAuthAppName(AppName);
			});
		}
	}
}
