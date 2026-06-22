namespace Examen.ApplicationCore.Domain
{
    /// <summary>
    /// Statut du cycle de vie d'une alerte
    /// </summary>
    public enum NiveauAlerte
    {
        Avertissement = 0,  // 2 non-conformes consécutifs
        Critique = 1,       // taux >= 10 %
        Urgence = 2         // taux >= 20 %
    }

    public enum StatutAlerte
    {
        Nouvelle = 0,
        EnCours = 1,
        Resolue = 2,
        Ignoree = 3
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
    public enum StatutNotification
    {
        EnAttente = 0,
        Envoye = 1,
        Echec = 2,
        Lu = 3
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