using System.ComponentModel.DataAnnotations;

namespace VegasBackend.ServerLogic
{
    public class Player
    {
        [Key]
        public string Id { get; set; }  // could be a guid ot email or sth
        public string DisplayName { get; set; }

        public List<GameDb> GamesAsPlayer1 { get; set; }
        public List<GameDb> GamesAsPlayer2 { get; set; }
    }
}
