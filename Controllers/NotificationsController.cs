using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using ms_notificaciones.Models;
namespace ms_notificaciones.Controllers;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;



[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    [Route("email-welcome")]
    [HttpPost]
    public async Task<ActionResult> SendEmailWelcome(ModelEmail data)
    {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

        var client = new SendGridClient(apiKey);

        SendGridMessage msg = createMessage(data);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENDGRID_TEMPLATE"));
        msg.SetTemplateData(new
        {
            name = data.nombreDestino,
            message = "Busca tu felicidad, bienvenido"
        });
        var response = await client.SendEmailAsync(msg);
        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
        {
            return Ok("Correo enviado a la dirección: " + data.correoDestino);
        }
        else
        {
            return BadRequest("Error enviando el mensaje a : " + data.correoDestino);
        }
    }

    [Route("email-recover-key")]
    [HttpPost]
    public async Task<ActionResult> SendEmailRecoverKey(ModelEmail data)
    {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

        var client = new SendGridClient(apiKey);
        SendGridMessage msg = createMessage(data);


        msg.SetTemplateId(Environment.GetEnvironmentVariable("RESTORE_PASSWORD_SENDGRID_TEMPLATE"));
        msg.SetTemplateData(new
        {
            name = data.nombreDestino,
            message = "A new password was requested for your account"
        });
        var response = await client.SendEmailAsync(msg);
        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
        {
            Console.Write("Hola Mundo Sobre Linea");
            return Ok("Correo enviado a la dirección: " + data.correoDestino);
        }
        else
        {
            return BadRequest("Error enviando el mensaje a : " + data.correoDestino);
        }
    }

    [Route("enviar-sms-nueva-clave")]
    [HttpPost]
    public async Task<ActionResult> EnviarSMSNuevaClave(ModelSMS data)
    {
    var AwsRegion = "us-east-2"; // Región de AWS para Colombia;
    var accessKeyId = Environment.GetEnvironmentVariable("ACCESS_KEY_AWS");
    var secretAccessKey = Environment.GetEnvironmentVariable("SECRET_KEY_AWS");
    var client = new AmazonSimpleNotificationServiceClient(accessKeyId, secretAccessKey, RegionEndpoint.GetBySystemName(AwsRegion));
    var messageAttributes = new Dictionary<string, MessageAttributeValue>();
    var smsType = new MessageAttributeValue{
        DataType = "String",
        StringValue = "Transactional"
    };

     messageAttributes.Add("AWS.SNS.SMS.SMSType", smsType);
     PublishRequest request = new PublishRequest {
            Message = data.contenidoMensaje,
            PhoneNumber = data.numDestino,
            MessageAttributes = messageAttributes
    };

    try
        {
            await client.PublishAsync(request);
           return Ok("sms enviado");
        }
        catch 
        {
            return BadRequest("Error al enviar el sms");
        }






}

[Route("enviar-correo2fa")]
[HttpPost]
public async Task<ActionResult> SendEmail2FA(ModelEmail data)
{
    var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

    var client = new SendGridClient(apiKey);
    SendGridMessage msg = createMessage(data);


    msg.SetTemplateId(Environment.GetEnvironmentVariable("TWOFA_SENDGRID_TEMPLATE"));
    msg.SetTemplateData(new
    {
        name = data.nombreDestino,
        message = data.contenidoCorreo,
        asunto = data.asuntoCorreo
    });
    var response = await client.SendEmailAsync(msg);
    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
    {
        return Ok("Correo 2FA enviando a : " + data.correoDestino);
    }
    else
    {
        return BadRequest("Error enviando el mensaje a : " + data.correoDestino);
    }
}


private SendGridMessage createMessage(ModelEmail data)
{
    var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

    var client = new SendGridClient(apiKey);
    var from = new EmailAddress(Environment.GetEnvironmentVariable("EMAIL_FROM"), Environment.GetEnvironmentVariable("NAME_FROM"));
    var subject = data.asuntoCorreo;
    var to = new EmailAddress(data.correoDestino, data.nombreDestino);
    var plainTextContent = data.contenidoCorreo;
    var htmlContent = data.contenidoCorreo;
    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
    return msg;
}
}
