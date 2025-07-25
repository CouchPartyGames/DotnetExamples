#!/usr/bin/env dotnet
#:package Braintree@5.34.0

// https://github.com/braintree/braintree_dotnet
using Braintree;

var gateway = new BraintreeGateway
{
    Environment = Braintree.Environment.SANDBOX,
    MerchantId = "the_merchant_id",
    PublicKey = "a_public_key",
    PrivateKey = "a_private_key"
};

TransactionRequest request = new TransactionRequest
{
    Amount = 1000.00M,
    PaymentMethodNonce = nonceFromTheClient,
    Options = new TransactionOptionsRequest
    {
        SubmitForSettlement = true
    }
};

Result<Transaction> result = gateway.Transaction.Sale(request);

if (result.IsSuccess())
{
    Transaction transaction = result.Target;
    Console.WriteLine("Success!: " + transaction.Id);
}
else if (result.Transaction != null)
{
    Transaction transaction = result.Transaction;
    Console.WriteLine("Error processing transaction:");
    Console.WriteLine("  Status: " + transaction.Status);
    Console.WriteLine("  Code: " + transaction.ProcessorResponseCode);
    Console.WriteLine("  Text: " + transaction.ProcessorResponseText);
}
else
{
    foreach (ValidationError error in result.Errors.DeepAll())
    {
        Console.WriteLine("Attribute: " + error.Attribute);
        Console.WriteLine("  Code: " + error.Code);
        Console.WriteLine("  Message: " + error.Message);
    }
}