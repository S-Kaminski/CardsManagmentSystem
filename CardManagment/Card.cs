using System.Text;

namespace CardManagment
{
    public struct Card
    {
        public string? OwnerId { get; set; }
        public string? Pin { get; set; }
        public string? CardSerialNumber { get; set; }
        public  string? CardId { get; set; }
        private readonly int _uniqueIdLength = 32;

        public Card()
        {
            CardId = GenerateUniqueCardId(_uniqueIdLength);
        }
        public Card(string ownerID, string pin, string cardSerialNumber, string cardID)
        {
            OwnerId = ownerID;
            Pin = pin;
            CardSerialNumber = cardSerialNumber;
            CardId = cardID;
        }
        public Card(string ownerID, string pin, string cardSerialNumber)
        {
            OwnerId = ownerID;
            Pin = pin;
            CardSerialNumber = cardSerialNumber;
            CardId = GenerateUniqueCardId(_uniqueIdLength);
        }

        private static string GenerateUniqueCardId(int idLength)
        {
            const string allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            StringBuilder uniqueId = new StringBuilder(idLength);
            for (int i = 0; i < idLength; i++)
            {
                uniqueId.Append(allowedCharacters[random.Next(allowedCharacters.Length-1)]);
            }
            return uniqueId.ToString();
        }

    }
}
