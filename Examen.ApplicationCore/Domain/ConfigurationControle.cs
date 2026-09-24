using System.ComponentModel.DataAnnotations;

namespace Examen.ApplicationCore.Domain
{
    public class ConfigurationControle
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Seuil de cadence.
        /// Exemple : 75
        /// </summary>
        [Required]
        [Range(1, 10000)]
        public int SeuilCadence { get; set; } = 75;

        /// <summary>
        /// Nombre d'échantillons lorsque la cadence est
        /// strictement inférieure au seuil.
        /// Exemple : 3
        /// </summary>
        [Required]
        [Range(1, 100)]
        public int NbEchantillonsInferieur { get; set; } = 3;

        /// <summary>
        /// Nombre d'échantillons lorsque la cadence est
        /// supérieure ou égale au seuil.
        /// Exemple : 5
        /// </summary>
        [Required]
        [Range(1, 100)]
        public int NbEchantillonsSuperieurOuEgal { get; set; } = 5;

        /// <summary>
        /// Nombre total d'échantillons lorsque le Test 2
        /// est déclenché.
        /// Dans ton fonctionnement actuel : 6.
        /// </summary>
        [Required]
        [Range(1, 200)]
        public int NbEchantillonsTest2 { get; set; } = 6;

        /// <summary>
        /// Indique si cette configuration est active.
        /// </summary>
        public bool Actif { get; set; } = true;
    }
}