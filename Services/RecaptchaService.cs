using Newtonsoft.Json.Linq; // Ensure you have Newtonsoft.Json installed

namespace WebApplication1.Services
{
    public class RecaptchaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RecaptchaService> _logger;

        public RecaptchaService(HttpClient httpClient, IConfiguration configuration, ILogger<RecaptchaService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> VerifyRecaptchaAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Recaptcha token is missing.");
                return false;
            }

            var secretKey = _configuration["RecaptchaSettings:SecretKey"];
            var response = await _httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}", null);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to contact Google Recaptcha service.");
                return false;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonString);

            var success = json["success"]?.Value<bool>() ?? false;

            // --- V3 SPECIFIC CHECK ---
            // For v3, we must also check the score (0.0 to 1.0).
            // 1.0 is a good human, 0.0 is a bot.
            // We usually accept anything above 0.5.
            var score = json["score"]?.Value<double>() ?? 0.0;

            if (!success)
            {
                _logger.LogWarning($"Recaptcha API returned success: false. Errors: {json["error-codes"]}");
                return false;
            }

            if (score < 0.5)
            {
                _logger.LogWarning($"Recaptcha score too low: {score}. Access denied.");
                return false;
            }

            return true;
        }
    }
}