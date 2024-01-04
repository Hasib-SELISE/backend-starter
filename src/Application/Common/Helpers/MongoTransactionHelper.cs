using MongoDB.Driver;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Helpers;

public class MongoTransactionHelper
{
    private readonly IEcapMongoDbDataContextProvider _ecapMongoDbDataContextProvider;

    /// <summary>
    /// Default constructor 
    /// </summary>
    /// <param name="ecapMongoDbDataContextProvider"></param>
    public MongoTransactionHelper(IEcapMongoDbDataContextProvider ecapMongoDbDataContextProvider)
    {
        _ecapMongoDbDataContextProvider = ecapMongoDbDataContextProvider;
    }
    
    /// <summary>
    /// Run the transaction
    /// </summary>
    /// <param name="transactionAction"></param>
    public void RunTransaction(Action<IClientSessionHandle> transactionAction)
    {
        var client = _ecapMongoDbDataContextProvider.GetTenantDataContext().Client;
        using var session = client.StartSession();
        
        session.StartTransaction();

        try
        {
            transactionAction(session);
            session.CommitTransaction();
        }
        catch (Exception ex)
        {
            session.AbortTransaction();
            throw;
        }
    }
}

// Usages example
/*_mongoTransactionHelper.RunTransaction(session => {
    var collection1 = database.GetCollection<BsonDocument>("collection1");
    var collection2 = database.GetCollection<BsonDocument>("collection2");

    var document1 = new BsonDocument { { "key1", "value1" } };
    collection1.InsertOne(session, document1);

    var document2 = new BsonDocument { { "key2", "value2" } };
    collection2.InsertOne(session, document2);
});*/
