using System;
using System.Linq;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConfigurationControleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfigurationControleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        // ============================================================
        // VÉRIFICATION ADMIN
        // ============================================================

        private bool EstAdmin()
        {
            var claim = User.FindFirst("estAdmin")?.Value;

            return string.Equals(
                claim,
                "true",
                StringComparison.OrdinalIgnoreCase
            );
        }

        // ============================================================
        // GET
        // Récupérer la configuration active
        // GET /api/ConfigurationControle
        // ============================================================

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var configuration = _unitOfWork
                    .Repository<ConfigurationControle>()
                    .GetAll()
                    .FirstOrDefault(c => c.Actif);

                // Si aucune configuration n'existe,
                // on crée automatiquement la configuration par défaut.
                if (configuration == null)
                {
                    configuration = new ConfigurationControle
                    {
                        SeuilCadence = 75,
                        NbEchantillonsInferieur = 3,
                        NbEchantillonsSuperieurOuEgal = 5,
                        NbEchantillonsTest2 = 6,
                        Actif = true
                    };

                    _unitOfWork
                        .Repository<ConfigurationControle>()
                        .Add(configuration);

                    _unitOfWork.Save();
                }

                return Ok(new
                {
                    id = configuration.Id,
                    seuilCadence = configuration.SeuilCadence,
                    nbEchantillonsInferieur =
                        configuration.NbEchantillonsInferieur,
                    nbEchantillonsSuperieurOuEgal =
                        configuration.NbEchantillonsSuperieurOuEgal,
                    nbEchantillonsTest2 =
                        configuration.NbEchantillonsTest2,
                    actif = configuration.Actif
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        message = "Erreur lors du chargement de la configuration.",
                        detail = ex.Message
                    }
                );
            }
        }

        // ============================================================
        // PUT
        // Modifier la configuration
        // PUT /api/ConfigurationControle
        // ============================================================

        [HttpPut]
        public IActionResult Update(
            [FromBody] ConfigurationControleDTO dto)
        {
            try
            {
                // Seul l'administrateur peut modifier
                // les règles de contrôle qualité.
                if (!EstAdmin())
                {
                    return StatusCode(
                        403,
                        new
                        {
                            message =
                                "Accès refusé. Seul un administrateur peut modifier la configuration."
                        }
                    );
                }

                if (dto == null)
                {
                    return BadRequest(new
                    {
                        message = "Les données de configuration sont obligatoires."
                    });
                }

                // ----------------------------------------------------
                // VALIDATIONS
                // ----------------------------------------------------

                if (dto.SeuilCadence <= 0)
                {
                    return BadRequest(new
                    {
                        message =
                            "Le seuil de cadence doit être supérieur à 0."
                    });
                }

                if (dto.NbEchantillonsInferieur <= 0)
                {
                    return BadRequest(new
                    {
                        message =
                            "Le nombre d'échantillons pour une cadence inférieure au seuil doit être supérieur à 0."
                    });
                }

                if (dto.NbEchantillonsSuperieurOuEgal <= 0)
                {
                    return BadRequest(new
                    {
                        message =
                            "Le nombre d'échantillons pour une cadence supérieure ou égale au seuil doit être supérieur à 0."
                    });
                }

                if (dto.NbEchantillonsTest2 <= 0)
                {
                    return BadRequest(new
                    {
                        message =
                            "Le nombre d'échantillons du Test 2 doit être supérieur à 0."
                    });
                }

                // ----------------------------------------------------
                // RÉCUPÉRER LA CONFIGURATION ACTIVE
                // ----------------------------------------------------

                var configuration = _unitOfWork
                    .Repository<ConfigurationControle>()
                    .GetAll()
                    .FirstOrDefault(c => c.Actif);

                // Si aucune configuration n'existe,
                // on en crée une.
                if (configuration == null)
                {
                    configuration = new ConfigurationControle
                    {
                        SeuilCadence = dto.SeuilCadence,
                        NbEchantillonsInferieur =
                            dto.NbEchantillonsInferieur,
                        NbEchantillonsSuperieurOuEgal =
                            dto.NbEchantillonsSuperieurOuEgal,
                        NbEchantillonsTest2 =
                            dto.NbEchantillonsTest2,
                        Actif = true
                    };

                    _unitOfWork
                        .Repository<ConfigurationControle>()
                        .Add(configuration);
                }
                else
                {
                    // ------------------------------------------------
                    // MODIFICATION
                    // ------------------------------------------------

                    configuration.SeuilCadence =
                        dto.SeuilCadence;

                    configuration.NbEchantillonsInferieur =
                        dto.NbEchantillonsInferieur;

                    configuration.NbEchantillonsSuperieurOuEgal =
                        dto.NbEchantillonsSuperieurOuEgal;

                    configuration.NbEchantillonsTest2 =
                        dto.NbEchantillonsTest2;

                    configuration.Actif = true;

                    _unitOfWork
                        .Repository<ConfigurationControle>()
                        .Update(configuration);
                }

                _unitOfWork.Save();

                return Ok(new
                {
                    message =
                        "Configuration du contrôle mise à jour avec succès.",

                    configuration = new
                    {
                        id = configuration.Id,
                        seuilCadence =
                            configuration.SeuilCadence,

                        nbEchantillonsInferieur =
                            configuration.NbEchantillonsInferieur,

                        nbEchantillonsSuperieurOuEgal =
                            configuration.NbEchantillonsSuperieurOuEgal,

                        nbEchantillonsTest2 =
                            configuration.NbEchantillonsTest2,

                        actif = configuration.Actif
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        message =
                            "Erreur lors de la modification de la configuration.",

                        detail = ex.Message
                    }
                );
            }
        }
    }
}