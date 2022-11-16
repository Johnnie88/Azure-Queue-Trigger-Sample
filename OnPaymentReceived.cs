using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Pluralsightfuncs
{
    public static class OnPaymentReceived
    {
        [FunctionName("OnPaymentReceived")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            // TODO: put the connection value by default
            [Queue("orders"), StorageAccount("pluralsightqueue")] IAsyncCollector<Order> orderQueue,
            ILogger log)
        {
            log.LogInformation("Received a payment.");
            
            var requestBody = await new StreamReader(req.Body)
                .ReadToEndAsync();

            var order = JsonConvert.DeserializeObject<Order>(requestBody);

            await orderQueue.AddAsync(order);

            log.LogInformation($"Order {order.OrderId} received from" +
            $" {order.Email} for product {order.ProductId}");
           
            return new OkObjectResult($"Thank you for you purchase");
        }
    }

    public class Order 
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Email { get; set; }
        public decimal Price { get; set; }
    }
}
