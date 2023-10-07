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
            try
            {
                var transaction = await _transactionService.GetAllTransactionsAsync(page);

                return Ok(transaction);

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

        // GET api/v1/transactions/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionById(id);

                if (transaction == null)
                    return new BadRequestObjectResult(new
                    {
                        status = "error",
                        message = $"Transaction with Id = {id} not found"
                    });
                return new OkObjectResult(new
                {
                    status = "success",
                    transaction
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

        // POST api/v1/products
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromForm] Transaction transaction)
        {
            try
            {
                await _transactionService.CreateTransactionAsync(transaction);

                return new OkObjectResult(new
                {
                    Status = "success",
                    message = $"Product created successfully",
                    transaction
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

        // PUT api/v1/transactions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction([FromRoute] int id, [FromForm] Transaction transaction)
        {
            try
            {

                var existedTransaction = await _transactionService.GetTransactionById(id);

                if (existedTransaction == null)
                    return new BadRequestObjectResult(new
                    {
                        status = "error",
                        message = $"Transaction with Id = {id} not found"
                    });

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

                return new OkObjectResult(new
                {
                    status = "success",
                    message = $"Product with Id = {id} updated successfully",
                    updateTransaction
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

        // DELETE api/v1/transactions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction([FromRoute] int id)
        {
            try
            {
                await _transactionService.DeleteTransactionAsync(id);

                return new OkObjectResult(new
                {
                    status = "success",
                    message = $"Product with Id = {id} deleted successfully"
                });;
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

    }
}
