using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services;
using ToyEcommerceASPNET.Services.interfaces;

namespace ToyEcommerceASPNET.Controllers
{
	[Route("api/v1/transactions")]
	[ApiController]
	public class TransactionController : ControllerBase
	{
		private readonly ITransactionService _transactionService;

		public TransactionController(ITransactionService transactionService)
		{
			_transactionService = transactionService;
		}

		// GET: api/v1/transactions
		[HttpGet]
		public async Task<IActionResult> GetTransactions([FromQuery(Name = "page")] int page)
		{
			var transaction = await _transactionService.GetAllTransactionsAsync(page);

			return Ok(transaction);
		}

		// GET api/v1/transactions/{id}
		[HttpGet("{id}")]
		public async Task<IActionResult> Get([FromRoute] int id)
		{
			var transaction = await _transactionService.GetTransactionById(id);

			if (transaction == null)
				return NotFound($"Product with Id = {id} not found");

			return Ok(transaction);
		}

		// POST api/v1/products
		[HttpPost]
		public async Task<IActionResult> CreateTransaction([FromForm] Transaction transaction)
		{
			await _transactionService.CreateTransactionAsync(transaction);

			return Ok(transaction);
		}

		// PUT api/v1/transactions/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTransaction([FromRoute] int id, [FromForm] Transaction transaction)
		{
			var existedTransaction = await _transactionService.GetTransactionById(id);

			if (existedTransaction == null)
				return NotFound($"Product with Id = {id} not found");

			Transaction updateTransaction = new Transaction()
			{
				Id = id,
				Type = transaction.Type,
				Amount = transaction.Amount,
				PaymentMethod = transaction.PaymentMethod,
				Timestamp = DateTime.Now,
				Status = transaction.Status,
			};

			await _transactionService.UpdateTransactionAsync(id, updateTransaction);

			return Ok(updateTransaction);
		}

		// DELETE api/v1/transactions/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTransaction([FromRoute] int id)
		{
			await _transactionService.DeleteTransactionAsync(id);

			return Ok($"Product with Id = {id} deleted");
		}

	}
}
