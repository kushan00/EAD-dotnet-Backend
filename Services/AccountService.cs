using backend.Models;
using backend.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Services;

public class AccountService
{
    private readonly IMongoCollection<Account> _accountCollection;
    public AccountService(
        IOptions<DatabaseSetting> databaseSetting)
    {
        var mongoClient = new MongoClient(
            databaseSetting.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSetting.Value.DatabaseName);

        _accountCollection = mongoDatabase.GetCollection<Account>(databaseSetting.Value.AccountCollectionName);
    }

    public async Task<Account?> GetAccountLogin(string NIC) => await _accountCollection.Find(x => x.NIC == NIC).FirstOrDefaultAsync();
    public async Task<List<Account>> GetAccountAsync() => await _accountCollection.Find(_ => true).ToListAsync();
    public async Task<Account?> GetAccountAsync(string id) => await _accountCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    public async Task CreateAccountAsync(Account account) => await _accountCollection.InsertOneAsync(account);
    public async Task UpdateAccountAsync(string id, Account account) => await _accountCollection.ReplaceOneAsync(x => x.Id == id, account);
    public async Task RemoveAccountAsync(string id) => await _accountCollection.DeleteOneAsync(x => x.Id == id);
}
