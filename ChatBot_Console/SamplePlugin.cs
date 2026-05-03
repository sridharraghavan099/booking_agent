using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ChatBot_Console
{
    public class SamplePlugin
    {
        [KernelFunction, Description("Get order status from system using order ID")]
        public string GetOrderStatus([Description("Order Id of the Order is REQUIRED")]string orderId)
        {
            if (!orderId.Any())
                return "Order ID is Required";
            return $"Order {orderId} is shipped";
        }
    }
}
