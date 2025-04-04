

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.DTOs;
using Microsoft.Extensions.Options;

namespace SneakerAPI.Infrastructure.Repositories;

public class EmailSender : IEmailSender
{

    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPass ;

    public EmailSender(IOptions<EmailSettings> smtpSettings)
    {   
                _smtpServer=smtpSettings.Value.SmtpServer;
                _smtpPort=smtpSettings.Value.SmtpPort;
                _smtpUser=smtpSettings.Value.SmtpUser;
                _smtpPass=smtpSettings.Value.SmtpPass;
    }

    

   
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Sneaker Luxury Store", _smtpUser));
        message.To.Add(new MailboxAddress(email, email));
        message.Subject = subject;

        message.Body = new TextPart("html")
        {
            Text = htmlMessage
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_smtpUser, _smtpPass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
