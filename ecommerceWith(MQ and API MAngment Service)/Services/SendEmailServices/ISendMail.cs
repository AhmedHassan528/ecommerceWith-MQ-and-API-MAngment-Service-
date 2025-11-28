namespace Authentication_With_JWT.Services
{
    public interface ISendMail
    {
        Task<string> SendEmailAsync(string emailTo, string subject, string? token, string controllerName, string? ReqUrl);
        Task<bool> sendInvoice(int OrderId);
    }
}
