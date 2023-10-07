using Microsoft.EntityFrameworkCore;
using ToyEcommerceASPNET.Data;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services.interfaces;

namespace ToyEcommerceASPNET.Services
{
	public class TransactionService : ITransactionService
	{
		private readonly ApplicationDbContext _context;
		public TransactionService(ApplicationDbContext context)
		{
			this._context = context;
		}
		public async Task<Object> GetAllTransactionsAsync(int? queryPage)
		{
			var transactions = await _context.Transactions.ToListAsync();

			int page = queryPage.GetValueOrDefault(1) <= 0 ? 1 : queryPage.GetValueOrDefault(1);
			int perPage = 5;    // number of items per page
			var total = transactions.Count();

			var data = new
			{
				status = "success",
				transactions = transactions.Skip((page - 1) * perPage).Take(perPage),
				totalPage = total,
                totalLength = page
			};

			return data;
		}

		public async Task<Transaction> GetTransactionById(int id)
		{
			return await _context.Transactions.FindAsync(id);
		}

		public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
		{
			await _context.Transactions.AddAsync(transaction);
			await _context.SaveChangesAsync();

			return transaction;
		}

		public async Task UpdateTransactionAsync(int id, Transaction transaction)
		{
			Transaction tr = await _context.Transactions.FindAsync(id);

			_context.Entry(tr).State = EntityState.Detached;
			_context.Attach(transaction);
			try
			{
				_context.Transactions.Update(transaction);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (TransactionExists(transaction.Id))
					throw;
			}
		}

		public async Task DeleteTransactionAsync(int id)
		{
			var transaction = await _context.Transactions.FindAsync(id);
			if (transaction != null)
			{
				_context.Transactions.Remove(transaction);
				await _context.SaveChangesAsync();
			}
		}
		private bool TransactionExists(int id)
		{
			return _context.Transactions.Any(transaction => transaction.Id == id);
		}

	}
}
