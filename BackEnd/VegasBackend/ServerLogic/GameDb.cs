using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegasBackend.ServerLogic
{
    public class GameDb
    {
        // will have to use 
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "jsonb")]
        public string BoardJson { get; set; } // EfCore doesnt support [][]?? not sure if true

        [Column(TypeName = "int")]
        public int MoveCount { get; set; }

        [Column(TypeName = "jsonb")]
        public string MadeMovesJson { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
