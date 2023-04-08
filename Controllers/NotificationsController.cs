using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using ms_notificaciones.Models;
namespace ms_notificaciones.Controllers;


    [ApiController]
    [Route("[controller]")]
    public class NotificationsController : ControllerBase
    {
     [Route("email")]
    [HttpPost]
    public async Task<ActionResult> SendEmail(ModelEmail data){
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
         
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("inmobiliaria.akinmueble@gmail.com", "AkinMueble");
            var subject = data.asuntoCorreo;
            var to = new EmailAddress(data.correoDestino, data.nombreDestino);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
                return Ok("Correo enviado a la dirección: " + data.correoDestino);
            }else{
                return BadRequest("Error enviando el mensaje a : " + data.correoDestino);
            }
    }   
}
