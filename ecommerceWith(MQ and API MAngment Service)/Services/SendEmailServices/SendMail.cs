
using Authentication_With_JWT.Setting;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using MultiTenancy.Models.CheckOutModels;

namespace Authentication_With_JWT.Services
{
    public class SendMail : ISendMail
    {
        private readonly MailSettings _mailSetting;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _dbContext;


        public SendMail(IOptions<MailSettings> mailSetting, UserManager<AppUser> userManager, ApplicationDbContext dbContext)
        {
            _mailSetting = mailSetting.Value;
            _userManager = userManager;
            _dbContext = dbContext;

        }

        public async Task<string> SendEmailAsync(string emailTo, string subject, string? token, string controllerName, string? ReqUrl)
        {

            try
            {
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_mailSetting.Email),
                    Subject = subject
                };

                email.To.Add(MailboxAddress.Parse(emailTo));

                var builder = new BodyBuilder();

                // body are
                var user = await _userManager.FindByEmailAsync(emailTo);
                if (user is null)
                    return "Email is incorrect";

                var confirmationLink = "";
                if (!string.IsNullOrEmpty(ReqUrl))
                {
                    confirmationLink = $"{ReqUrl}/ConfirmEmail?UserId={user.Id}&Token={token}";
                }
                else
                {
                    confirmationLink = $"{ReqUrl}/ConfirmEmail?UserId={user.Id}&Token={token}";
                }
                builder.HtmlBody = 
                    $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2>{subject}</h2>
                        <p>Dear {user.FirstName} {user.LastName},</p>
                        <p>Thank you for signing up! Please confirm your email address by clicking the button below:</p>
                        <p>
                            <a href='{confirmationLink}' 
                               style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                                Confirm Email
                            </a>
                        </p>
                        <p>If the button doesn't work, copy and paste the following link into your browser:</p>
                        <p><a href='{confirmationLink}'>{confirmationLink}</a></p>
                        <br>
                        <p>If you didn’t request this, please ignore this email.</p>
                        <p>Best regards,<br><strong>Ahmed Hassan</strong></p>
                    </body>
                    </html>";
                email.Body = builder.ToMessageBody();

                email.From.Add(new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Email));


                using var smtp = new SmtpClient();
                smtp.Connect(_mailSetting.Host, _mailSetting.Port, SecureSocketOptions.SslOnConnect);
                smtp.Authenticate(_mailSetting.Email, _mailSetting.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }

        public async Task<bool> sendInvoice(int OrderId)
        {
            try
            {

                var order = await _dbContext.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == OrderId);

                var user = await _userManager.FindByIdAsync(order.CartOwner);

                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_mailSetting.Email),
                    Subject = "Thank you for using our store, here is is your invoice"
                };

                email.To.Add(MailboxAddress.Parse(user!.Email));

                var builder = new BodyBuilder();

                builder.HtmlBody =
                       @"<html>
                        <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <div style='text-align: center; margin-bottom: 20px;'>
                                <h2 style='color: #333;'>Invoice</h2>
                            </div>
                            <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
                                <thead>
                                    <tr style='background-color: #f5f5f5;'>
                                        <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Product</th>
                                        <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Description</th>
                                        <th style='padding: 10px; border: 1px solid #ddd; text-align: right;'>Quantity</th>
                                        <th style='padding: 10px; border: 1px solid #ddd; text-align: right;'>Price</th>
                                        <th style='padding: 10px; border: 1px solid #ddd; text-align: right;'>Total</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {0}
                                </tbody>
                            </table>
                            <div style='text-align: right; margin-top: 20px;'>
                                <p style='font-size: 16px; font-weight: bold;'>Grand Total: {1:C}</p>
                            </div>
                            <div style='margin-top: 30px; text-align: center; color: #777;'>
                                <p>Thank you for your purchase!</p>
                            </div>
                        </body>
                        </html>"
                ;

                string tableRows = string.Join("\n", order.Items.Select(item =>
                    $@"<tr>
            <td style='padding: 10px; border: 1px solid #ddd;'>{item.ProductName}</td>
            <td style='padding: 10px; border: 1px solid #ddd;'>{item.ProductDescription}</td>
            <td style='padding: 10px; border: 1px solid #ddd; text-align: right;'>{item.Count}</td>
            <td style='padding: 10px; border: 1px solid #ddd; text-align: right;'>{item.Price:C}</td>
            <td style='padding: 10px; border: 1px solid #ddd; text-align: right;'>
                {(item.Count * item.Price):C}</td>
        </tr>"));
                decimal grandTotal = order.TotalAmount;
                builder.HtmlBody = string.Format(builder.HtmlBody, tableRows, grandTotal);

                email.Body = builder.ToMessageBody();


                email.From.Add(new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Email));

                using var smtp = new SmtpClient();
                smtp.Connect(_mailSetting.Host, _mailSetting.Port, SecureSocketOptions.SslOnConnect);
                smtp.Authenticate(_mailSetting.Email, _mailSetting.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
