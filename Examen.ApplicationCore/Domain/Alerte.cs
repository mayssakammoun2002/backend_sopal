
using Examen.ApplicationCore.Domain;

public class Alerte
{
    public int Id { get; set; }

    // ── Clés étrangères ───────────────────────────────
    public int? SeuilId { get; set; }          // null si déclenché par règle des 2 non-conformes
    public int? ResolueParId { get; set; }

    // ── Identification de la source ───────────────────
    public string CodeMachine { get; set; } = string.Empty;
    public string? CodeArticle { get; set; }
    public string? NumOF { get; set; }         // OF concerné (si déclenché par résultat de contrôle)

    // ── Métriques ─────────────────────────────────────
    public decimal TauxDetecte { get; set; }
    public int QuantiteDefauts { get; set; }
    public int QuantiteTotale { get; set; }

    /// <summary>
    /// Nombre de résultats "Non Conforme" consécutifs ayant déclenché l'alerte.
    /// Vaut 2 pour le déclencheur "2 non-conformes consécutifs".
    /// </summary>
    public int NbNonConformesConsecutifs { get; set; } = 0;

    // ── Qualification ─────────────────────────────────
    public NiveauAlerte Niveau { get; set; }
    public StatutAlerte Statut { get; set; } = StatutAlerte.Nouvelle;
    public string Message { get; set; } = string.Empty;
    public string? CommentaireResolution { get; set; }

    // ── Horodatages ───────────────────────────────────
    public DateTime DateAlerte { get; set; } = DateTime.UtcNow;
    public DateTime? DateResolution { get; set; }

    // ── Navigation ────────────────────────────────────
    public Seuil? Seuil { get; set; }
    public Utilisateur? ResoluePar { get; set; }
    public ICollection<CommentaireAlerte> Commentaires { get; set; } = new List<CommentaireAlerte>();
    public ICollection<HistoriqueNotification> Notifications { get; set; } = new List<HistoriqueNotification>();

    // ══════════════════════════════════════════════════
    // MÉTHODES MÉTIER
    // ══════════════════════════════════════════════════

    public void Resoudre(int utilisateurId, string? commentaire = null)
    {
        if (!PeutEtreResolue())
            throw new InvalidOperationException($"L'alerte est déjà {Statut}.");
        Statut = StatutAlerte.Resolue;
        DateResolution = DateTime.UtcNow;
        ResolueParId = utilisateurId;
        CommentaireResolution = commentaire;
    }

    public void MarquerEnCours()
    {
        if (Statut == StatutAlerte.Resolue || Statut == StatutAlerte.Ignoree)
            throw new InvalidOperationException($"Impossible de reprendre une alerte déjà {Statut}.");
        Statut = StatutAlerte.EnCours;
    }

    public void Ignorer(int utilisateurId, string raison)
    {
        if (Statut == StatutAlerte.Resolue || Statut == StatutAlerte.Ignoree)
            throw new InvalidOperationException($"L'alerte est déjà {Statut}.");
        Statut = StatutAlerte.Ignoree;
        ResolueParId = utilisateurId;
        CommentaireResolution = raison;
        DateResolution = DateTime.UtcNow;
    }

    public bool PeutEtreResolue()
        => Statut != StatutAlerte.Resolue && Statut != StatutAlerte.Ignoree;

    /// <summary>
    /// Retourne un badge CSS lisible pour l'UI (Vue / Tailwind).
    /// </summary>
    public string BadgeCouleur() => Niveau switch
    {
        NiveauAlerte.Urgence => "red",
        NiveauAlerte.Critique => "orange",
        NiveauAlerte.Avertissement => "yellow",
        _ => "gray"
    };
}

 