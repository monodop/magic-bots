using Microsoft.Extensions.Logging;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.Common;

namespace MagicBots.Overseer.Framework.Services
{
    public class FirestoreService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        private FirestoreDb _db = null!;
        
        public FirestoreService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            Init();
        }
        
        public CollectionReference GetGuildScopedCollection(ITriggerContext context, string collectionName)
        {
            return _db.Collection("Guilds").Document(context.Guild!.Id.ToString()).Collection(collectionName);
        }

        public CollectionReference GetChannelScopedCollection(ITriggerContext context, string collectionName)
        {
            return _db.Collection("Guilds").Document(context.Guild!.Id.ToString()).Collection("Channels").Document(context.Channel!.Id.ToString()).Collection(collectionName);
        }

        public CollectionReference GetUserScopedCollection(ITriggerContext context, string collectionName)
        {
            return _db.Collection("Users").Document(context.User!.Id.ToString()).Collection(collectionName);
        }

        private void Init()
        {
            var connectionString = _configuration.GetFromConfig<string>("ConnectionStrings:GCP", _logger);
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };
            
            var keyPath = builder["Keypath"].ToString();
            var projectId = builder["ProjectId"].ToString();

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            _db = FirestoreDb.Create(projectId);
            _logger.LogInformation($"Created Cloud Firestore client with project ID '{projectId}'");
        }
    }
}