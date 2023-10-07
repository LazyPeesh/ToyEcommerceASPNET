using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
	public interface ITransactionService
	{
		Task<Object> GetAllTransactionsAsync(int? queryPage);
		Task<Transaction> GetTransactionById(int id);
		Task<Transaction> CreateTransactionAsync(Transaction transaction);
		Task UpdateTransactionAsync(int id, Transaction transaction);
		Task DeleteTransactionAsync(int id);
	}
}