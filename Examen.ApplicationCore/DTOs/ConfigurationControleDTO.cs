using System.ComponentModel.DataAnnotations;

namespace Examen.ApplicationCore.DTOs
{
    public class ConfigurationControleDTO
    {
        [Required]
        [Range(1, 10000)]
        public int SeuilCadence { get; set; }

        [Required]
        [Range(1, 100)]
        public int NbEchantillonsInferieur { get; set; }

        [Required]
        [Range(1, 100)]
        public int NbEchantillonsSuperieurOuEgal { get; set; }

        [Required]
        [Range(1, 200)]
        public int NbEchantillonsTest2 { get; set; }

        public bool Actif { get; set; } = true;
    }
}