using MailKit.Net.Smtp;
using MimeKit;

namespace WebApplication1.Services
{
    public class EmailService
  {
 private readonly IConfiguration _configuration;

  public EmailService(IConfiguration configuration)
    {
 _configuration = configuration;
      }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
  {
     try
      {
   var message = new MimeMessage();
     message.From.Add(new MailboxAddress(
      _configuration["EmailSettings:SenderName"] ?? "Ace Job Agency",
      _configuration["EmailSettings:SenderEmail"] ?? "noreply@acejobagency.com"));
        message.To.Add(new MailboxAddress("", toEmail));
         message.Subject = subject;

    var bodyBuilder = new BodyBuilder
       {
         HtmlBody = body
 };
    message.Body = bodyBuilder.ToMessageBody();

            // Check if SMTP is configured
      var smtpServer = _configuration["EmailSettings:SmtpServer"];
         var smtpUsername = _configuration["EmailSettings:Username"];
    var smtpPassword = _configuration["EmailSettings:Password"];

     if (!string.IsNullOrEmpty(smtpServer) && 
     !string.IsNullOrEmpty(smtpUsername) && 
        !string.IsNullOrEmpty(smtpPassword))
   {
       // Send real email
  using (var client = new SmtpClient())
       {
         await client.ConnectAsync(smtpServer, int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587"), false);
           await client.AuthenticateAsync(smtpUsername, smtpPassword);
         await client.SendAsync(message);
  await client.DisconnectAsync(true);

       Console.WriteLine($"? Email sent successfully to {toEmail}");
       }
     }
          else
        {
    // For development/testing - just log the email
    Console.WriteLine($"=== EMAIL SENT (CONSOLE ONLY - SMTP NOT CONFIGURED) ===");
     Console.WriteLine($"To: {toEmail}");
      Console.WriteLine($"Subject: {subject}");
    Console.WriteLine($"Body: {body}");
    Console.WriteLine($"=======================================================");
       }

         await Task.CompletedTask;
      }
         catch (Exception ex)
  {
Console.WriteLine($"? Error sending email: {ex.Message}");
   throw;
  }
   }

  public async Task Send2FACodeAsync(string toEmail, string code)
        {
    var subject = "Your 2FA Code - Ace Job Agency";
var body = $@"
    <html>
       <body>
           <h2>Two-Factor Authentication Code</h2>
            <p>Your verification code is: <strong>{code}</strong></p>
     <p>This code will expire in 10 minutes.</p>
    <p>If you did not request this code, please ignore this email.</p>
  </body>
      </html>";

    await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPasswordResetAsync(string toEmail, string resetLink)
     {
     var subject = "Password Reset - Ace Job Agency";
            var body = $@"
         <html>
           <body>
      <h2>Password Reset Request</h2>
    <p>Click the link below to reset your password:</p>
             <p><a href='{resetLink}'>Reset Password</a></p>
      <p>This link will expire in 1 hour.</p>
<p>If you did not request a password reset, please ignore this email.</p>
          </body>
   </html>";

    await SendEmailAsync(toEmail, subject, body);
    }
    }
}
