using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhengwei.Contact.Api.Models;

namespace Zhengwei.Contact.Api.Data
{
    public class ContactContext
    {
        private IMongoDatabase _database;
        private IMongoCollection<ContactBook> _collection;
        private AppSettings _appSettings;

        public ContactContext(IOptionsSnapshot<AppSettings> settings)
        {
            _appSettings = settings.Value;
            var client = new MongoClient(_appSettings.MongoContactConnectionString);
            if(client != null)
            {
                _database = client.GetDatabase(_appSettings.MongoContactConnectionString);
            }
        }

        public IMongoCollection<ContactBook> ContactBooks
        {
            get
            {
                CheckAndCreateCollection("ContactBooks");
                return _database.GetCollection<ContactBook>("ContactBooks");
            }
        }
        public IMongoCollection<ContactApplyRequest> ContactApplyRequests
        {
            get
            {
                CheckAndCreateCollection("ContactApplyRequest");
                return _database.GetCollection<ContactApplyRequest>("ContactApplyRequest");
            }
        }

        private void CheckAndCreateCollection(string collectName)
        {
            var collectionList = _database.ListCollections().ToList();
            var collectNames = new List<string>();
            collectionList.ForEach(b => collectNames.Add(b["name"].AsString));
            if (!collectNames.Contains(collectName))
            {
                _database.CreateCollection(collectName);
            }
        }
    }
}
