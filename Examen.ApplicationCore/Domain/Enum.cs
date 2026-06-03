namespace Examen.ApplicationCore.Domain
{
    /// <summary>
    /// Statut du cycle de vie d'une alerte
    /// </summary>
    public enum StatutAlerte
    {
        Nouvelle = 0,
        EnCours = 1,
        Resolue = 2,
        Ignoree = 3
    }

    /// <summary>
    /// Niveau de sévérité / criticité d'une alerte
    /// </summary>
    public enum NiveauAlerte
    {
        Avertissement = 0,   // Niveau bas
        Critique = 1,        // Niveau moyen
        Urgence = 2          // Niveau haut
    }


    public enum TypeActionAlerte
    {
        Creation = 0,
        PriseEnCharge = 1,
        Resolution = 2,
        Ignorer = 3,
        AjoutCommentaire = 4,
        Reouverture = 5
    }

    /// <summary>
    /// (Optionnel) Priorité globale
    /// </summary>
    public enum Priorite
    {
        Basse = 0,
        Moyenne = 1,
        Haute = 2,
        Critique = 3
    }
}