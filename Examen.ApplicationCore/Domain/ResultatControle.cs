using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examen.ApplicationCore.Domain
{
    public class ResultatControle
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime DateControle { get; set; } = DateTime.UtcNow;

        [Required]
        public string CodeMachine { get; set; } = null!;

        [Required]
        public string CodeArticle { get; set; } = null!;

        /*
         * Utilisateur connecté qui a SAISI le contrôle.
         *
         * Ce champ ne représente PAS le contrôleur.
         */
        [Required]
        public int UtilisateurId { get; set; }

        [Required]
        public string NumOF { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantite { get; set; }

        [Range(1, 10000)]
        public int Cadence { get; set; }

        /*
         * Nombre total d'échantillons contrôlés.
         *
         * 3 ou 5 au départ.
         * 6 si Test 2 est déclenché.
         */
        [Range(1, 100)]
        public int NbEchantillons { get; set; }

        /*
         * Conforme = validé
         * Non Conforme = rejeté
         *
         * La distinction "Validé avec 1 défaut"
         * est déterminée avec NbDefautsTest1 + NbDefautsTest2.
         */
        [Required]
        public string StatutLot { get; set; } = "Conforme";

        /*
         * Solution corrective.
         * Obligatoire lorsqu'il y a au moins un défaut.
         */
        public string? SolutionGlobale { get; set; }

        public string? NumLotMatiere { get; set; }

        [StringLength(100)]
        public string? NumConteneur { get; set; }

        /*
         * Contrôleurs réellement sélectionnés
         * dans le formulaire.
         *
         * Exemple :
         * "Ahmed Ben Ali, Sami Trabelsi"
         */
        [StringLength(1000)]
        public string? ControleurNoms { get; set; }

        /*
         * IDs des contrôleurs.
         *
         * Exemple :
         * "4,8,12"
         */
        [StringLength(1000)]
        public string? ControleurIds { get; set; }

        /*
         * Résultats des tests
         */
        public int NbDefautsTest1 { get; set; } = 0;

        public int NbDefautsTest2 { get; set; } = 0;

        /*
         * Défauts (libellé texte libre, saisi depuis le front).
         */
        [StringLength(200)]
        public string? Defaut1 { get; set; }

        [StringLength(200)]
        public string? Defaut2 { get; set; }

        /*
         * Clés étrangères explicites vers TypeDefaut.
         *
         * Nullables : un contrôle peut n'avoir aucun défaut,
         * ou un seul défaut (Test 1 uniquement).
         */
        [ForeignKey(nameof(TypeDefaut1))]
        public int? TypeDefaut1Id { get; set; }

        [ForeignKey(nameof(TypeDefaut2))]
        public int? TypeDefaut2Id { get; set; }

        /*
         * Navigation
         */
        public Machine Machine { get; set; } = null!;

        [NotMapped]
        public Produit? Produit { get; set; }

        public Utilisateur Utilisateur { get; set; } = null!;

        public TypeDefaut? TypeDefaut1 { get; set; }

        public TypeDefaut? TypeDefaut2 { get; set; }
    }
}