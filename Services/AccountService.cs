using backend.Models;
using backend.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Services;

public class AccountService
{
    private readonly IMongoCollection<Account> _accountCollection;
    /* The `AccountService` constructor is initializing the `_accountCollection` field. It takes an
    `IOptions<DatabaseSetting>` parameter, which is used to retrieve the database connection settings. */
    public AccountService(
        IOptions<DatabaseSetting> databaseSetting)
    {
        var mongoClient = new MongoClient(
            databaseSetting.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSetting.Value.DatabaseName);

        _accountCollection = mongoDatabase.GetCollection<Account>(databaseSetting.Value.AccountCollectionName);
    }

    /// <summary>
    /// The function `GetAccountLogin` retrieves an account from a collection based on the provided NIC
    /// (National Identification Card) and returns it asynchronously.
    /// </summary>
    /// <param name="NIC">The parameter "NIC" stands for National Identification Card. It is used to
    /// uniquely identify an individual in some countries. In this code snippet, it is used as a parameter
    /// to search for an account in the account collection based on the NIC value.</param>
    public async Task<Account?> GetAccountLogin(string NIC) => await _accountCollection.Find(x => x.NIC == NIC).FirstOrDefaultAsync();
    /// <summary>
    /// The function `GetAccountAsync` retrieves all accounts from a collection asynchronously.
    /// </summary>
    public async Task<List<Account>> GetAccountAsync() => await _accountCollection.Find(_ => true).ToListAsync();
    /// <summary>
    /// The function retrieves a list of traveler accounts asynchronously.
    /// </summary>
    public async Task<List<Account>> GetTravelerAccountAsync() => await _accountCollection.Find(x => x.UserRole == "Traveler").ToListAsync();
    /// <summary>
    /// The function retrieves a list of user accounts with the roles "Agent" or "Back_Office"
    /// asynchronously.
    /// </summary>
    public async Task<List<Account>> GetUsersAccountAsync() => await _accountCollection.Find(x => x.UserRole == "Agent" || x.UserRole == "Back_Office").ToListAsync();
    /// <summary>
    /// The function `GetAccountAsync` retrieves an account from a collection based on the provided ID
    /// asynchronously.
    /// </summary>
    /// <param name="id">The id parameter is a string that represents the unique identifier of the account
    /// you want to retrieve.</param>
    public async Task<Account?> GetAccountAsync(string id) => await _accountCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    /// <summary>
    /// The function asynchronously inserts a new account into the account collection.
    /// </summary>
    /// <param name="Account">The "Account" parameter is an object of type "Account" that represents the
    /// account to be created.</param>
    public async Task CreateAccountAsync(Account account) => await _accountCollection.InsertOneAsync(account);
    /// <summary>
    /// The function `UpdateAccountAsync` updates an account in a collection based on its ID.
    /// </summary>
    /// <param name="id">The id parameter is a string that represents the unique identifier of the account
    /// that needs to be updated.</param>
    /// <param name="Account">The `Account` parameter is an object that represents the updated account
    /// information. It contains properties such as `Id`, `Name`, `Balance`, etc.</param>
    public async Task UpdateAccountAsync(string id, Account account) => await _accountCollection.ReplaceOneAsync(x => x.Id == id, account);
    /// <summary>
    /// The function removes an account from a collection based on its ID.
    /// </summary>
    /// <param name="id">The id parameter is a string that represents the unique identifier of the account
    /// that needs to be removed.</param>
    public async Task RemoveAccountAsync(string id) => await _accountCollection.DeleteOneAsync(x => x.Id == id);
}
