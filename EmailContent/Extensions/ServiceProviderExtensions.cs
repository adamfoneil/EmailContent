﻿using EmailContentServices.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace EmailContentServices.Extensions
{
    public static class ServiceProviderExtensions
    {
        public const string EmailToken = "email-token";

        /// <summary>
        /// thanks to https://stackoverflow.com/a/68760353/2023653
        /// </summary>
        public static ICollection<string> GetBaseUrls(this IServiceProvider services)
        {
            var server = services.GetService<IServer>();
            var addresses = server?.Features.Get<IServerAddressesFeature>();
            return addresses?.Addresses ?? Array.Empty<string>();
        }

        public static string GetHttpsUrl(this IServiceProvider services) => GetBaseUrls(services).First(url => url.StartsWith("https://"));

        /// <summary>
        /// builds a URL to a resource in this application.
        /// Path should NOT start with a slash
        /// </summary>
        public static string BuildUrl(this IServiceProvider services, string path) => GetHttpsUrl(services) + path;

        public static async Task<string> RenderViewAsync(this IServiceProvider services, string path)
        {
            var url = BuildUrl(services, path);

            var http = services.GetRequiredService<HttpClient>();
            http.DefaultRequestHeaders.Clear();
            http.DefaultRequestHeaders.Add(EmailToken, BuildEmailToken(services, path));

            return await http.GetStringAsync(url);
        }

        public static string BuildEmailToken(this IServiceProvider services, string path)
        {
            var options = services.GetService<IOptions<AuthorizeEmailOptions>>();
            return HashString((options?.Value.HashSalt ?? string.Empty) + path);
        }

        private static string HashString(string input)
        {
            using var md5 = MD5.Create();
            return Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }

    }
}