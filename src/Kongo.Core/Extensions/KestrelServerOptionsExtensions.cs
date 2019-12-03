using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Linq;
using Kongo.Core.Models;

namespace Kongo.Core.Extensions
{
	public static class KestrelServerOptionsExtensions
	{
		public static void ConfigureEndpoints(this KestrelServerOptions options)
		{
			var configuration = options.ApplicationServices.GetRequiredService<IConfiguration>();
			var environment = options.ApplicationServices.GetRequiredService<IHostingEnvironment>();

			var endpoints = configuration.GetSection("HttpServer:Endpoints")
				.GetChildren()
				.ToDictionary(section => section.Key, section =>
				{
					var endpoint = new EndpointConfiguration();
					section.Bind(endpoint);
					return endpoint;
				});

			foreach (var endpoint in endpoints)
			{
				var config = endpoint.Value;
				var port = config.Port ?? (config.Scheme == "https" ? 443 : 80);

				var ipAddresses = new List<IPAddress>();
				if (config.Host == "localhost")
				{
					ipAddresses.Add(IPAddress.IPv6Loopback);
					ipAddresses.Add(IPAddress.Loopback);
				}
				else if (IPAddress.TryParse(config.Host, out var address))
				{
					ipAddresses.Add(address);
				}
				else
				{
					ipAddresses.Add(IPAddress.IPv6Any);
				}

				foreach (var address in ipAddresses)
				{
					options.Listen(address, port,
						listenOptions =>
						{
							if (config.Scheme == "https")
							{
								var certificate = LoadCertificate(config, environment);
								listenOptions.UseHttps(certificate);
							}
						});
				}
			}
		}

		public static void ConfigureUrls(this KestrelServerOptions options, string[] urls)
		{
			var environment = options.ApplicationServices.GetRequiredService<IHostingEnvironment>();
			var kongoOptions = options.ApplicationServices.GetRequiredService<KongoOptions>();

			foreach (var url in urls)
			{
				var uirScheme = url.Split(':')[0];
				var host = url.Split(':')[1].Substring(2);
				var portPart = url.Split(':')[2];
				Int32.TryParse(portPart, out int port);

				EndpointConfiguration endpoint = null;
				if (kongoOptions.CertificatePath != null && kongoOptions.CertificatePassword != null)
				{
					endpoint = new EndpointConfiguration()
					{
						Port = !string.IsNullOrEmpty(portPart) ? port : (uirScheme == "https" ? 443 : 80),
						Scheme = uirScheme,
						FilePath = kongoOptions.CertificatePath,
						Password = kongoOptions.CertificatePassword
					};
				} else
				{
					endpoint = new EndpointConfiguration()
					{
						Host = kongoOptions.CertificateSubject,
						Port = !string.IsNullOrEmpty(portPart) ? port : (uirScheme == "https" ? 443 : 80),
						Scheme = uirScheme,
						StoreLocation = "LocalMachine",
						StoreName = "My"
					};
				}

				var ipAddresses = new List<IPAddress>();
				if (host == "localhost")
				{
					ipAddresses.Add(IPAddress.IPv6Loopback);
					ipAddresses.Add(IPAddress.Loopback);
				}
				else if (IPAddress.TryParse(endpoint.Host, out var address))
				{
					ipAddresses.Add(address);
				}
				else
				{
					ipAddresses.Add(IPAddress.IPv6Any);
				}

				foreach (var address in ipAddresses)
				{
					options.Listen(address, endpoint.Port.Value,
						listenOptions =>
						{
							if (endpoint.Scheme == "https")
							{
								var certificate = LoadCertificate(endpoint, environment);
								listenOptions.UseHttps(certificate);
							}
						});
				}
			}
		}

		private static X509Certificate2 LoadCertificate(EndpointConfiguration config, IHostingEnvironment environment)
		{
			if (config.StoreName != null && config.StoreLocation != null)
			{
				using (var store = new X509Store(config.StoreName, Enum.Parse<StoreLocation>(config.StoreLocation)))
				{
					store.Open(OpenFlags.ReadOnly);
					var certificate = store.Certificates.Find(
						X509FindType.FindBySubjectName,
						config.Host,
						validOnly: !environment.IsDevelopment());

					if (certificate.Count == 0)
					{
						throw new InvalidOperationException($"Certificate not found for {config.Host}.");
					}

					return certificate[0];
				}
			}

			if (config.FilePath != null && config.Password != null)
			{
				return new X509Certificate2(config.FilePath, config.Password);
			}

			throw new InvalidOperationException("No valid certificate configuration found for the current endpoint.");
		}
	}
}
