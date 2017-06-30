namespace ionix.Data.MongoDB.Migration
{
	using System;
	using System.Collections.Generic;
	using global::MongoDB.Bson;
	using global::MongoDB.Driver;
	using global::MongoDB.Driver.Linq;

    public abstract class CollectionMigration : Migration
    {
        protected string CollectionName;

        public CollectionMigration(MigrationVersion version, string collectionName) : base(version)
        {
            CollectionName = collectionName;
        }

        public virtual IMongoQueryable<BsonDocument> Filter()
        {
            return null;
        }

        public override void Update()
        {
            var collection = GetCollection();
            var documents = GetDocuments(collection);
            UpdateDocuments(collection, documents);
        }

        public virtual void UpdateDocuments(IMongoCollection<BsonDocument> collection, IEnumerable<BsonDocument> documents)
        {
            foreach (var document in documents)
            {
                try
                {
                    UpdateDocument(collection, document);
                }
                catch (Exception exception)
                {
                    OnErrorUpdatingDocument(document, exception);
                }
            }
        }

        protected virtual void OnErrorUpdatingDocument(BsonDocument document, Exception exception)
        {
            var message =
                new
                {
                    Message = "Failed to update document",
                    CollectionName,
                    Id = document.TryGetDocumentId(),
                    MigrationVersion = Version,
                    MigrationDescription = Description
                };
            throw new MigrationException(message.ToString(), exception);
        }

        public abstract void UpdateDocument(IMongoCollection<BsonDocument> collection, BsonDocument document);

        protected virtual IMongoCollection<BsonDocument> GetCollection()
        {
            return Database.GetCollection<BsonDocument>(CollectionName);
        }

        protected virtual IEnumerable<BsonDocument> GetDocuments(IMongoCollection<BsonDocument> collection)
        {
            var query = Filter();
            return query != null
                ? query.ToList()
                : collection.AsQueryable().ToList();
        }
    }
}