using System.Text.Json;

namespace FourtitudeTest.Services
{
    public class PartnerService
    {
        private readonly List<Partner> _partners;

        public PartnerService()
        {
            var partnersJson = Environment.GetEnvironmentVariable("PARTNERS");

            if (string.IsNullOrWhiteSpace(partnersJson))
            {
                throw new InvalidOperationException("PARTNERS environment variable is not set or is empty.");
            }

            try
            {
                _partners = JsonSerializer.Deserialize<List<Partner>>(partnersJson);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to parse PARTNERS environment variable.", ex);
            }
        }

        public List<Partner> GetPartners() => _partners;
    }
}
