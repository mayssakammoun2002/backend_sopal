using Examen.ApplicationCore.Domain;

public interface IServiceAlerte
{
    void Add(Alerte alerte);
    void Update(Alerte alerte);
    void Delete(Alerte alerte);
    void DeleteById(int id);
    void Commit();
    IEnumerable<Alerte> GetAll();
    Alerte? GetById(int id);
    IEnumerable<Alerte> GetActives();
    void PrendreEnChargeAlerte(int alerteId, int userId);
    void ResoudreAlerte(int alerteId, int userId, string commentaire);
    void IgnorerAlerte(int alerteId, int userId, string raison);
    void AjouterCommentaire(int alerteId, int auteurId, string nomAuteur, string contenu);
    IEnumerable<CommentaireAlerte> GetCommentaires(int alerteId);
    Task VerifierSeuilsAsync();
}
