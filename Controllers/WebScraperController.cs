using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using HtmlAgilityPack;

[Route("api/webscraper")]
[ApiController]
public class WebScraperController : ControllerBase
{
    private IMongoCollection<BsonDocument> _collection;

    public WebScraperController()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("meu_banco_de_dados"); // Substitua pelo nome do seu banco de dados
        _collection = database.GetCollection<BsonDocument>("meu_colecao"); // Substitua pelo nome da sua coleção
    }

    [HttpGet]
    public async Task<IActionResult> ScrapeAndStoreData()
    {
        try
        {
            string url = "https://exemplo.com"; // Substitua pela URL da página que deseja raspar

            using (var httpClient = new HttpClient())
            {
                var html = await httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var data = doc.DocumentNode.SelectNodes("//seletor");

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var document = new BsonDocument
                        {
                            { "campo1", item.SelectSingleNode("seletor_campo1").InnerText },
                            { "campo2", item.SelectSingleNode("seletor_campo2").InnerText },
                            // Adicione mais campos conforme necessário
                        };

                        await _collection.InsertOneAsync(document);
                    }

                    return Ok("Dados raspados e armazenados com sucesso.");
                }
                else
                {
                    return NotFound("Nenhum dado encontrado na página.");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro: {ex.Message}");
        }
    }
}
