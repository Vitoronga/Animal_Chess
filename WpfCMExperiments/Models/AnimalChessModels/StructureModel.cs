using AnimalChessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCMExperiments.Models.AnimalChessModels
{
    public class StructureModel
    {
        public StructureTypes Structure { get; init; }
        public string ImagePath { get; init; }
        public bool IsTeamStructure => !(Structure == StructureTypes.Empty || Structure == StructureTypes.Water);
        public bool IsTeam1Structure => (Structure == StructureTypes.P1Trap ||  Structure == StructureTypes.P1Den);
        public bool IsTeam2Structure => (Structure == StructureTypes.P2Trap ||  Structure == StructureTypes.P2Den);

        public StructureModel(StructureTypes structure, string imagePath)
        {
            Structure = structure;
            ImagePath = imagePath;
        }
    }
}
