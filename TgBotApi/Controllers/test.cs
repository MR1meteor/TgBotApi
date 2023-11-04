using KafkaClient.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TgBotApi.Controllers;

[Route("TestKafka")]
[ApiController]
public class test : ControllerBase
{
    private readonly IKafkaProducesService kafkaProduces;

    public test(IKafkaProducesService kafkaProduces)
    {
        this.kafkaProduces = kafkaProduces;
    }

    [HttpGet]
    public async Task<IActionResult> GetErrorStatusByUserIdAndName()
    {
        await kafkaProduces.WriteTraceLogAsync("testlog");


        return Ok("testlog");
    }
}