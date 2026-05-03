using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;


namespace ChatBot_Console;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Create a kernel with OpenAI as the AI provider
        var kernelBuilder = Kernel.CreateBuilder();


        //Configure ollmai provider
        kernelBuilder.AddOpenAIChatCompletion(
                        modelId: "llama3.1:8b",
                        endpoint: new Uri("http://localhost:11434/v1"),
                        apiKey: "ollama"
                        );
            var kernel = kernelBuilder.Build();

        //Plugin registration
        kernel.ImportPluginFromType<SamplePlugin>();
       

        // Create CharService
        var ChatService = kernel.GetRequiredService<IChatCompletionService>();

        //Chat History
        var chatHistory = new ChatHistory();

        //Add user message to chat history
        chatHistory.AddSystemMessage(@"You are a system assistant connected to an internal order database.

STRICT RULES:
- You ONLY know data from available functions
- You MUST use the function GetOrderStatusById for any order status request
- DO NOT ask for shipping method, tracking number, or any extra details
- DO NOT use external knowledge
- If orderId is missing, ask ONLY for orderId
- If orderId is provided, immediately call the function
- Striclty use your Tools capabilities
- Dont use your trained knowledge, use ONLY the tool result
- Dont answer any questions which not able to access the Plugin methods
- If the question is not related to plugin dont explain plugin details

Correct behavior:
User: what is order status of 123
Answer: Your order 123 is shipped.
");

        var settings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            Temperature = 0
        };

        while (true)
        {
            Console.Write("User: ");
            var userInput = Console.ReadLine();

            chatHistory.AddUserMessage(userInput);

            var result = await ChatService.GetChatMessageContentAsync(
                chatHistory,
                settings,
                kernel
            );

            Console.WriteLine("AI: " + result.Content);

            chatHistory.AddAssistantMessage(result.Content);
        }

    }
}