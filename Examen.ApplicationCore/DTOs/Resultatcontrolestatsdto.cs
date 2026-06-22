using System;
using System.Collections.Generic;

namespace Examen.ApplicationCore.DTOs
{
    public class ResultatControleStatsDTO
    {
        public int TotalControles { get; set; }
        public int TotalConformes { get; set; }
        public int TotalNonConformes { get; set; }
        public double TauxConformite { get; set; } // en %
        public int TotalDefauts { get; set; }
        public List<StatParMachineDTO> ParMachine { get; set; } = new();
        public List<StatParDefautDTO> ParTypeDefaut { get; set; } = new();
        public List<StatParJourDTO> Evolution { get; set; } = new();
    }

    public class StatParMachineDTO
    {
        public string CodeMachine { get; set; } = "";
        public string NomMachine { get; set; } = "";
        public int TotalControles { get; set; }
        public int Conformes { get; set; }
        public int NonConformes { get; set; }
        public double TauxConformite { get; set; }
    }

    public class StatParDefautDTO
    {
        public string Libelle { get; set; } = "";
        public int Occurrences { get; set; }
    }

    public class StatParJourDTO
    {
        public DateTime Date { get; set; }
        public int TotalControles { get; set; }
        public int NonConformes { get; set; }
    }
}