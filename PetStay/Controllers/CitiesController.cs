using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PetStay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string IBGE_API_URL = "https://servicodados.ibge.gov.br/api/v1/localidades/municipios";

        public CitiesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Retorna lista de cidades que correspondem ao termo de busca
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<List<CityDto>>> Search([FromQuery] string? termo)
        {
            if (string.IsNullOrWhiteSpace(termo) || termo.Length < 2)
            {
                return Ok(new List<CityDto>());
            }

            try
            {
                // Chamar IBGE API
                var response = await _httpClient.GetAsync(IBGE_API_URL);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                // Configurar desserializador para case-insensitive
                var options = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var cidades = JsonSerializer.Deserialize<List<IbgeCityDto>>(json, options);

                if (cidades == null || cidades.Count == 0)
                {
                    return Ok(new List<CityDto>());
                }

                // Filtrar e formatar resultados
                var resultados = cidades
                    .Where(c => !string.IsNullOrEmpty(c.Nome) && c.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(c => c.Nome)
                    .Take(10)
                    .Select(c => new CityDto
                    {
                        Nome = c.Nome ?? string.Empty,
                        Estado = ExtractEstado(c),
                        Regiao = ExtractRegiao(c)
                    })
                    .ToList();

                return Ok(resultados);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar cidades", detalhes = ex.Message });
            }
            catch (JsonException ex)
            {
                return StatusCode(500, new { erro = "Erro ao processar dados", detalhes = ex.Message });
            }
        }

        private string ExtractEstado(IbgeCityDto cidade)
        {
            try
            {
                return cidade?.Microrregiao?.Mesorregiao?.Uf?.Sigla ?? "BR";
            }
            catch
            {
                return "BR";
            }
        }

        private string ExtractRegiao(IbgeCityDto cidade)
        {
            try
            {
                return cidade?.Microrregiao?.Mesorregiao?.Uf?.Regiao?.Nome ?? "Brasil";
            }
            catch
            {
                return "Brasil";
            }
        }
    }

    // DTOs para serialização
    public class CityDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Regiao { get; set; } = string.Empty;
    }

    public class IbgeCityDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public IbgeMicrorregiao? Microrregiao { get; set; }
    }

    public class IbgeMicrorregiao
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public IbgeMesorregiao? Mesorregiao { get; set; }
    }

    public class IbgeMesorregiao
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public IbgeUf? Uf { get; set; }
    }

    public class IbgeUf
    {
        public int Id { get; set; }
        public string Sigla { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public IbgeRegiao? Regiao { get; set; }
    }

    public class IbgeRegiao
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}
