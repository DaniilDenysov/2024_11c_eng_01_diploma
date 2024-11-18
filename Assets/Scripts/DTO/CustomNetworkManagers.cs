using Mirror;

namespace DTOs
{
    public static class CustomNetworkMessages
    {
        public static void WritePlayerStats(this NetworkWriter writer, Player player)
        {
            writer.WriteString(player.Nickname);
            writer.WriteString(player.CharacterGUID);
            writer.WriteInt(player.ConnectionId);
            writer.WriteBool(player.IsPartyOwner);
            writer.WriteBool(player.IsReady);
        }

        public static Player ReadPlayerStats(this NetworkReader reader)
        {
            Player stats = new Player
            {
                Nickname = reader.ReadString(),
                CharacterGUID = reader.ReadString(),
                ConnectionId = reader.ReadInt(),
                IsPartyOwner = reader.ReadBool(),
                IsReady = reader.ReadBool()
            };
            return stats;
        }
    }
}